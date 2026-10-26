using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace SPMaster.Lecturer
{
    public partial class CreateQuestion : System.Web.UI.Page
    {
        // =========================================================
        // DATABASE CONNECTION
        // =========================================================

        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["SPMasterConnectionString"].ConnectionString;
            }
        }


        // =========================================================
        // CURRENT USER
        // =========================================================

        private int CurrentUserId
        {
            get
            {
                return Convert.ToInt32(Session["UserId"]);
            }
        }


        // =========================================================
        // EDIT QUESTION ID
        // =========================================================

        private int? EditingQuestionId
        {
            get
            {
                int questionId;

                if (int.TryParse(Request.QueryString["id"], out questionId))
                {
                    return questionId;
                }

                return null;
            }
        }


        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(object sender, EventArgs e)
        {
            // -----------------------------------------------------
            // LOGIN REQUIRED
            // -----------------------------------------------------

            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            pnlQuestionSuccess.Visible = false;
            pnlQuestionError.Visible = false;

            if (!IsPostBack)
            {
                BindSubjects();


                // =================================================
                // EDIT MODE
                // =================================================

                if (EditingQuestionId.HasValue)
                {
                    bool loaded = LoadQuestion(EditingQuestionId.Value);

                    if (!loaded)
                    {
                        Response.Redirect("~/Lecturer/QuestionBank.aspx");
                        return;
                    }

                    litPageTitle.Text = "Edit Question";
                    litPageSubtitle.Text = "Update the selected question in your institutional bank";
                    btnSaveQuestion.Text = "Update Question";
                }


                // =================================================
                // CREATE MODE
                // =================================================

                else
                {
                    litPageTitle.Text = "Create New Question";
                    litPageSubtitle.Text = "Add an SPM standardized question to your institutional bank";
                    btnSaveQuestion.Text = "Save to Bank";
                }


                // =================================================
                // SUCCESS MESSAGE
                // =================================================

                if (Request.QueryString["saved"] == "1")
                {
                    lblQuestionSuccess.Text = "Question saved successfully.";
                    pnlQuestionSuccess.Visible = true;
                }

                if (Request.QueryString["updated"] == "1")
                {
                    lblQuestionSuccess.Text = "Question updated successfully.";
                    pnlQuestionSuccess.Visible = true;
                }
            }
        }


        // =========================================================
        // LOAD SUBJECTS
        // =========================================================

        private void BindSubjects()
        {
            const string sql = @"
                SELECT
                    SubjectId,
                    SubjectName
                FROM Subjects
                WHERE
                    IsActive = 1
                ORDER BY
                    SubjectName;
            ";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                ddlSubject.DataSource = command.ExecuteReader();
                ddlSubject.DataTextField = "SubjectName";
                ddlSubject.DataValueField = "SubjectId";
                ddlSubject.DataBind();
            }

            ddlSubject.Items.Insert(
                0,
                new ListItem("Select Subject", "")
            );
        }


        // =========================================================
        // LOAD EXISTING QUESTION
        // =========================================================

        private bool LoadQuestion(int questionId)
        {
            const string sql = @"
                SELECT
                    QuestionId,
                    SubjectId,
                    QuestionText,
                    QuestionType,
                    Difficulty,
                    HintText,
                    Explanation,
                    ModelAnswer,
                    MediaPath
                FROM Questions
                WHERE
                    QuestionId = @QuestionId
                    AND CreatedByUserId = @UserId
                    AND IsArchived = 0;
            ";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@QuestionId", SqlDbType.Int).Value = questionId;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = CurrentUserId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return false;
                    }

                    ddlSubject.SelectedValue = reader["SubjectId"].ToString();
                    txtQuestion.Text = reader["QuestionText"].ToString();
                    ddlQuestionType.SelectedValue = reader["QuestionType"].ToString();
                    ddlDifficulty.SelectedValue = reader["Difficulty"].ToString();

                    txtHint.Text =
                        reader["HintText"] == DBNull.Value
                            ? ""
                            : reader["HintText"].ToString();

                    txtExplanation.Text =
                        reader["Explanation"] == DBNull.Value
                            ? ""
                            : reader["Explanation"].ToString();

                    txtModelAnswer.Text =
                        reader["ModelAnswer"] == DBNull.Value
                            ? ""
                            : reader["ModelAnswer"].ToString();

                    string mediaPath =
                        reader["MediaPath"] == DBNull.Value
                            ? ""
                            : reader["MediaPath"].ToString();

                    hfExistingMediaPath.Value = mediaPath;
                    hfRemoveExistingImage.Value = "false";

                    if (!string.IsNullOrWhiteSpace(mediaPath))
                    {
                        hfExistingMediaUrl.Value = ResolveUrl(mediaPath);
                        selectedFileName.Text = Path.GetFileName(mediaPath);
                    }
                    else
                    {
                        hfExistingMediaUrl.Value = "";
                        selectedFileName.Text = "";
                    }
                }
            }


            // -----------------------------------------------------
            // LOAD MCQ OPTIONS
            // -----------------------------------------------------

            if (ddlQuestionType.SelectedValue == "MCQ")
            {
                LoadQuestionOptions(questionId);
            }

            return true;
        }


        // =========================================================
        // LOAD EXISTING MCQ OPTIONS
        // =========================================================

        private void LoadQuestionOptions(int questionId)
        {
            const string sql = @"
                SELECT
                    OptionText,
                    IsCorrect,
                    DisplayOrder
                FROM QuestionOptions
                WHERE
                    QuestionId = @QuestionId
                ORDER BY
                    DisplayOrder;
            ";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@QuestionId", SqlDbType.Int).Value = questionId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int order = Convert.ToInt32(reader["DisplayOrder"]);
                        string optionText = reader["OptionText"].ToString();
                        bool isCorrect = Convert.ToBoolean(reader["IsCorrect"]);

                        switch (order)
                        {
                            case 1:
                                txtOptionA.Text = optionText;
                                correctA.Checked = isCorrect;
                                break;

                            case 2:
                                txtOptionB.Text = optionText;
                                correctB.Checked = isCorrect;
                                break;

                            case 3:
                                txtOptionC.Text = optionText;
                                correctC.Checked = isCorrect;
                                break;

                            case 4:
                                txtOptionD.Text = optionText;
                                correctD.Checked = isCorrect;
                                break;
                        }
                    }
                }
            }
        }


        // =========================================================
        // SAVE BUTTON
        // =========================================================

        protected void btnSaveQuestion_Click(object sender, EventArgs e)
        {
            pnlQuestionSuccess.Visible = false;
            pnlQuestionError.Visible = false;

            Page.Validate("QuestionValidation");

            if (!Page.IsValid)
            {
                return;
            }

            try
            {
                string mediaPath = PrepareMediaPath();

                // Media validation can become invalid
                // while PrepareMediaPath executes.
                if (!Page.IsValid)
                {
                    return;
                }

                int questionId;

                if (EditingQuestionId.HasValue)
                {
                    questionId = EditingQuestionId.Value;

                    UpdateQuestion(
                        questionId,
                        mediaPath
                    );

                    Response.Redirect(
                        "~/Lecturer/CreateQuestion.aspx?id="
                        + questionId
                        + "&updated=1"
                    );

                    return;
                }

                questionId = InsertQuestion(mediaPath);

                Response.Redirect(
                    "~/Lecturer/CreateQuestion.aspx?id="
                    + questionId
                    + "&saved=1"
                );
            }
            catch (Exception ex)
            {
                ShowError(
                    "Unable to save the question. "
                    + ex.Message
                );
            }
        }


        // =========================================================
        // SERVER VALIDATION - MCQ OPTION
        // =========================================================

        protected void ValidateMcqOption(object source, ServerValidateEventArgs args)
        {
            if (ddlQuestionType.SelectedValue != "MCQ")
            {
                args.IsValid = true;
                return;
            }

            args.IsValid = !string.IsNullOrWhiteSpace(args.Value);
        }


        // =========================================================
        // SERVER VALIDATION - CORRECT OPTION
        // =========================================================

        protected void ValidateCorrectOption(object source, ServerValidateEventArgs args)
        {
            if (ddlQuestionType.SelectedValue != "MCQ")
            {
                args.IsValid = true;
                return;
            }

            args.IsValid =
                correctA.Checked
                || correctB.Checked
                || correctC.Checked
                || correctD.Checked;
        }


        // =========================================================
        // SERVER VALIDATION - MODEL ANSWER
        // =========================================================

        protected void ValidateModelAnswer(object source, ServerValidateEventArgs args)
        {
            if (ddlQuestionType.SelectedValue != "Subjective")
            {
                args.IsValid = true;
                return;
            }

            args.IsValid = !string.IsNullOrWhiteSpace(args.Value);
        }


        // =========================================================
        // SERVER VALIDATION - IMAGE
        // =========================================================

        protected void ValidateImage(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            valImage.ErrorMessage = "";

            if (!questionImage.HasFile)
            {
                return;
            }

            string extension = Path.GetExtension(questionImage.FileName).ToLowerInvariant();

            bool validExtension =
                extension == ".png"
                || extension == ".jpg"
                || extension == ".jpeg"
                || extension == ".webp";

            if (!validExtension)
            {
                args.IsValid = false;
                valImage.ErrorMessage = "Only PNG, JPG, JPEG or WebP images are allowed.";
                return;
            }

            const int maxFileSize = 5 * 1024 * 1024;

            if (questionImage.PostedFile.ContentLength > maxFileSize)
            {
                args.IsValid = false;
                valImage.ErrorMessage = "Image size must not exceed 5MB.";
            }
        }


        // =========================================================
        // PREPARE IMAGE
        // =========================================================

        private string PrepareMediaPath()
        {
            // -----------------------------------------------------
            // NEW IMAGE UPLOADED
            // -----------------------------------------------------

            if (questionImage.HasFile)
            {
                string extension = Path.GetExtension(questionImage.FileName).ToLowerInvariant();
                string fileName = Guid.NewGuid().ToString("N") + extension;
                string relativeFolder = "~/Images/Questions/";
                string physicalFolder = Server.MapPath(relativeFolder);

                if (!Directory.Exists(physicalFolder))
                {
                    Directory.CreateDirectory(physicalFolder);
                }

                string physicalPath = Path.Combine(
                    physicalFolder,
                    fileName
                );

                questionImage.SaveAs(physicalPath);

                // Delete old image when replacing
                DeleteExistingImage();

                return relativeFolder + fileName;
            }


            // -----------------------------------------------------
            // EXISTING IMAGE EXPLICITLY REMOVED
            // -----------------------------------------------------

            if (hfRemoveExistingImage.Value == "true")
            {
                DeleteExistingImage();
                return null;
            }


            // -----------------------------------------------------
            // KEEP OLD IMAGE
            // -----------------------------------------------------

            if (!string.IsNullOrWhiteSpace(hfExistingMediaPath.Value))
            {
                return hfExistingMediaPath.Value;
            }

            return null;
        }


        // =========================================================
        // DELETE EXISTING IMAGE FILE
        // =========================================================

        private void DeleteExistingImage()
        {
            string existingPath = hfExistingMediaPath.Value;

            if (string.IsNullOrWhiteSpace(existingPath))
            {
                return;
            }

            try
            {
                string physicalPath = Server.MapPath(existingPath);

                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }
            }
            catch
            {
                // Image cleanup failure should not
                // stop the question from saving.
            }
        }


        // =========================================================
        // INSERT NEW QUESTION
        // =========================================================

        private int InsertQuestion(string mediaPath)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    const string sql = @"
                        INSERT INTO Questions
                        (
                            SubjectId,
                            CreatedByUserId,
                            QuestionText,
                            QuestionType,
                            Difficulty,
                            HintText,
                            Explanation,
                            ModelAnswer,
                            MediaPath,
                            IsArchived,
                            CreatedAt
                        )
                        VALUES
                        (
                            @SubjectId,
                            @CreatedByUserId,
                            @QuestionText,
                            @QuestionType,
                            @Difficulty,
                            @HintText,
                            @Explanation,
                            @ModelAnswer,
                            @MediaPath,
                            0,
                            GETDATE()
                        );

                        SELECT CAST(
                            SCOPE_IDENTITY()
                            AS INT
                        );
                    ";

                    int questionId;

                    using (SqlCommand command = CreateQuestionCommand(
                        sql,
                        connection,
                        transaction,
                        mediaPath
                    ))
                    {
                        questionId = Convert.ToInt32(
                            command.ExecuteScalar()
                        );
                    }

                    if (ddlQuestionType.SelectedValue == "MCQ")
                    {
                        InsertQuestionOptions(
                            connection,
                            transaction,
                            questionId
                        );
                    }

                    transaction.Commit();

                    return questionId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        // =========================================================
        // UPDATE EXISTING QUESTION
        // =========================================================

        private void UpdateQuestion(int questionId, string mediaPath)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    const string sql = @"
                        UPDATE Questions
                        SET
                            SubjectId = @SubjectId,
                            QuestionText = @QuestionText,
                            QuestionType = @QuestionType,
                            Difficulty = @Difficulty,
                            HintText = @HintText,
                            Explanation = @Explanation,
                            ModelAnswer = @ModelAnswer,
                            MediaPath = @MediaPath
                        WHERE
                            QuestionId = @QuestionId
                            AND CreatedByUserId = @CreatedByUserId
                            AND IsArchived = 0;
                    ";

                    using (SqlCommand command = CreateQuestionCommand(
                        sql,
                        connection,
                        transaction,
                        mediaPath
                    ))
                    {
                        command.Parameters.Add("@QuestionId", SqlDbType.Int).Value = questionId;

                        int rows = command.ExecuteNonQuery();

                        if (rows == 0)
                        {
                            throw new Exception(
                                "Question was not found or cannot be edited."
                            );
                        }
                    }

                    // Remove old MCQ options first
                    DeleteQuestionOptions(
                        connection,
                        transaction,
                        questionId
                    );

                    // Reinsert only for MCQ
                    if (ddlQuestionType.SelectedValue == "MCQ")
                    {
                        InsertQuestionOptions(
                            connection,
                            transaction,
                            questionId
                        );
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        // =========================================================
        // COMMON QUESTION PARAMETERS
        // =========================================================

        private SqlCommand CreateQuestionCommand(
            string sql,
            SqlConnection connection,
            SqlTransaction transaction,
            string mediaPath)
        {
            SqlCommand command = new SqlCommand(
                sql,
                connection,
                transaction
            );

            command.Parameters.Add("@SubjectId", SqlDbType.Int).Value = Convert.ToInt32(ddlSubject.SelectedValue);
            command.Parameters.Add("@CreatedByUserId", SqlDbType.Int).Value = CurrentUserId;
            command.Parameters.Add("@QuestionText", SqlDbType.NVarChar).Value = txtQuestion.Text.Trim();
            command.Parameters.Add("@QuestionType", SqlDbType.NVarChar, 30).Value = ddlQuestionType.SelectedValue;
            command.Parameters.Add("@Difficulty", SqlDbType.NVarChar, 30).Value = ddlDifficulty.SelectedValue;
            command.Parameters.Add("@HintText", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(txtHint.Text)
                ? (object)DBNull.Value
                : txtHint.Text.Trim();

            command.Parameters.Add("@Explanation", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(txtExplanation.Text)
                ? (object)DBNull.Value
                : txtExplanation.Text.Trim();

            // Model Answer belongs only to Subjective
            command.Parameters.Add("@ModelAnswer", SqlDbType.NVarChar).Value = ddlQuestionType.SelectedValue == "Subjective" && !string.IsNullOrWhiteSpace(txtModelAnswer.Text)
                ? (object)txtModelAnswer.Text.Trim()
                : DBNull.Value;

            command.Parameters.Add("@MediaPath", SqlDbType.NVarChar, 500).Value = string.IsNullOrWhiteSpace(mediaPath)
                ? (object)DBNull.Value
                : mediaPath;

            return command;
        }


        // =========================================================
        // DELETE OLD OPTIONS
        // =========================================================

        private void DeleteQuestionOptions(
            SqlConnection connection,
            SqlTransaction transaction,
            int questionId)
        {
            const string sql = @"
                DELETE FROM QuestionOptions
                WHERE
                    QuestionId = @QuestionId;
            ";

            using (SqlCommand command = new SqlCommand(
                sql,
                connection,
                transaction
            ))
            {
                command.Parameters.Add("@QuestionId", SqlDbType.Int).Value = questionId;

                command.ExecuteNonQuery();
            }
        }


        // =========================================================
        // INSERT MCQ OPTIONS
        // =========================================================

        private void InsertQuestionOptions(
            SqlConnection connection,
            SqlTransaction transaction,
            int questionId)
        {
            string correctOption = GetCorrectOption();

            InsertQuestionOption(
                connection,
                transaction,
                questionId,
                txtOptionA.Text.Trim(),
                correctOption == "A",
                1
            );

            InsertQuestionOption(
                connection,
                transaction,
                questionId,
                txtOptionB.Text.Trim(),
                correctOption == "B",
                2
            );

            InsertQuestionOption(
                connection,
                transaction,
                questionId,
                txtOptionC.Text.Trim(),
                correctOption == "C",
                3
            );

            InsertQuestionOption(
                connection,
                transaction,
                questionId,
                txtOptionD.Text.Trim(),
                correctOption == "D",
                4
            );
        }


        // =========================================================
        // INSERT ONE OPTION
        // =========================================================

        private void InsertQuestionOption(
            SqlConnection connection,
            SqlTransaction transaction,
            int questionId,
            string optionText,
            bool isCorrect,
            int displayOrder)
        {
            const string sql = @"
                INSERT INTO QuestionOptions
                (
                    QuestionId,
                    OptionText,
                    IsCorrect,
                    DisplayOrder
                )
                VALUES
                (
                    @QuestionId,
                    @OptionText,
                    @IsCorrect,
                    @DisplayOrder
                );
            ";

            using (SqlCommand command = new SqlCommand(
                sql,
                connection,
                transaction
            ))
            {
                command.Parameters.Add("@QuestionId", SqlDbType.Int).Value = questionId;
                command.Parameters.Add("@OptionText", SqlDbType.NVarChar).Value = optionText;
                command.Parameters.Add("@IsCorrect", SqlDbType.Bit).Value = isCorrect;
                command.Parameters.Add("@DisplayOrder", SqlDbType.Int).Value = displayOrder;

                command.ExecuteNonQuery();
            }
        }


        // =========================================================
        // GET CORRECT OPTION
        // =========================================================

        private string GetCorrectOption()
        {
            if (correctA.Checked)
            {
                return "A";
            }

            if (correctB.Checked)
            {
                return "B";
            }

            if (correctC.Checked)
            {
                return "C";
            }

            if (correctD.Checked)
            {
                return "D";
            }

            return "";
        }


        // =========================================================
        // GENERAL SERVER ERROR
        // =========================================================

        private void ShowError(string message)
        {
            lblQuestionError.Text = message;
            pnlQuestionError.Visible = true;
        }
    }
}