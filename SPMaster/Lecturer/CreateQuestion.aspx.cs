using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;

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
                return ConfigurationManager
                    .ConnectionStrings["SPMasterConnectionString"]
                    .ConnectionString;
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
        // EDITING QUESTION ID
        // =========================================================

        private int? EditingQuestionId
        {
            get
            {
                int questionId;

                if (int.TryParse(
                    Request.QueryString["id"],
                    out questionId))
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
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }


            Page.Form.Enctype =
                "multipart/form-data";


            if (!IsPostBack)
            {
                BindSubjects();


                if (EditingQuestionId.HasValue)
                {
                    SetupEditMode();

                    LoadQuestion(
                        EditingQuestionId.Value
                    );
                }
                else
                {
                    SetupCreateMode();
                }
            }
        }


        // =========================================================
        // CREATE MODE
        // =========================================================

        private void SetupCreateMode()
        {
            litPageTitle.Text =
                "Create New Question";

            litPageSubtitle.Text =
                "Add an SPM standardized question to your institutional bank";

            btnSaveQuestion.Text =
                "Save to Bank";
        }


        // =========================================================
        // EDIT MODE
        // =========================================================

        private void SetupEditMode()
        {
            litPageTitle.Text =
                "Edit Question";

            litPageSubtitle.Text =
                "Update the selected question in your question bank";

            btnSaveQuestion.Text =
                "Update Question";
        }


        // =========================================================
        // SUBJECTS
        // =========================================================

        private void BindSubjects()
        {
            const string sql = @"
                SELECT
                    SubjectId,
                    SubjectName
                FROM Subjects
                WHERE IsActive = 1
                ORDER BY SubjectName;
            ";


            using (SqlConnection connection =
                new SqlConnection(ConnectionString))
            {
                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    connection.Open();


                    ddlSubject.DataSource =
                        command.ExecuteReader();

                    ddlSubject.DataTextField =
                        "SubjectName";

                    ddlSubject.DataValueField =
                        "SubjectId";

                    ddlSubject.DataBind();
                }
            }
        }


        // =========================================================
        // LOAD EXISTING QUESTION
        // =========================================================

        private void LoadQuestion(int questionId)
        {
            const string sql = @"
                SELECT
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
                    AND CreatedByUserId = @UserId;
            ";


            using (SqlConnection connection =
                new SqlConnection(ConnectionString))
            {
                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(
                        "@QuestionId",
                        SqlDbType.Int
                    ).Value = questionId;


                    command.Parameters.Add(
                        "@UserId",
                        SqlDbType.Int
                    ).Value = CurrentUserId;


                    connection.Open();


                    using (SqlDataReader reader =
                        command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            ShowMessage(
                                "Question not found or you do not have permission to edit it."
                            );

                            return;
                        }


                        ddlSubject.SelectedValue =
                            reader["SubjectId"].ToString();


                        ddlQuestionType.SelectedValue =
                            reader["QuestionType"].ToString();


                        ddlDifficulty.SelectedValue =
                            reader["Difficulty"].ToString();


                        txtQuestion.Text =
                            reader["QuestionText"].ToString();


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


                        hfExistingMediaPath.Value =
                            mediaPath;


                        if (!string.IsNullOrEmpty(mediaPath))
                        {
                            selectedFileName.Text =
                                Path.GetFileName(mediaPath);
                        }
                    }
                }
            }


            // MCQ options are stored separately
            if (ddlQuestionType.SelectedValue == "MCQ")
            {
                LoadQuestionOptions(
                    questionId
                );
            }
        }


        // =========================================================
        // LOAD MCQ OPTIONS
        // =========================================================

        private void LoadQuestionOptions(
            int questionId)
        {
            const string sql = @"
                SELECT
                    OptionText,
                    IsCorrect,
                    DisplayOrder

                FROM QuestionOptions

                WHERE QuestionId = @QuestionId

                ORDER BY DisplayOrder;
            ";


            using (SqlConnection connection =
                new SqlConnection(ConnectionString))
            {
                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(
                        "@QuestionId",
                        SqlDbType.Int
                    ).Value = questionId;


                    connection.Open();


                    using (SqlDataReader reader =
                        command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int order =
                                Convert.ToInt32(
                                    reader["DisplayOrder"]
                                );


                            string text =
                                reader["OptionText"].ToString();


                            bool isCorrect =
                                Convert.ToBoolean(
                                    reader["IsCorrect"]
                                );


                            switch (order)
                            {
                                case 1:

                                    txtOptionA.Text =
                                        text;

                                    correctA.Checked =
                                        isCorrect;

                                    break;


                                case 2:

                                    txtOptionB.Text =
                                        text;

                                    correctB.Checked =
                                        isCorrect;

                                    break;


                                case 3:

                                    txtOptionC.Text =
                                        text;

                                    correctC.Checked =
                                        isCorrect;

                                    break;


                                case 4:

                                    txtOptionD.Text =
                                        text;

                                    correctD.Checked =
                                        isCorrect;

                                    break;
                            }
                        }
                    }
                }
            }
        }


        // =========================================================
        // SAVE / UPDATE
        // =========================================================

        protected void btnSaveQuestion_Click(
            object sender,
            EventArgs e)
        {
            string questionType =
                ddlQuestionType.SelectedValue;


            // -----------------------------------------------------
            // VALIDATION
            // -----------------------------------------------------

            string questionText =
                txtQuestion.Text.Trim();

            if (string.IsNullOrWhiteSpace(questionText))
            {
                ShowMessage(
                    "Please enter the question prompt."
                );

                return;
            }

            if (questionText.Length < 10)
            {
                ShowMessage(
                    "The question prompt must contain at least 10 characters."
                );

                return;
            }


            string correctOption =
                GetCorrectOption();


            if (questionType == "MCQ")
            {
                if (
                    string.IsNullOrWhiteSpace(
                        txtOptionA.Text
                    ) ||

                    string.IsNullOrWhiteSpace(
                        txtOptionB.Text
                    ) ||

                    string.IsNullOrWhiteSpace(
                        txtOptionC.Text
                    ) ||

                    string.IsNullOrWhiteSpace(
                        txtOptionD.Text
                    )
                )
                {
                    ShowMessage(
                        "Please enter all four answer choices."
                    );

                    return;
                }


                if (string.IsNullOrEmpty(
                    correctOption))
                {
                    ShowMessage(
                        "Please select the correct answer."
                    );

                    return;
                }

                string optionA =
                    txtOptionA.Text.Trim();

                string optionB =
                    txtOptionB.Text.Trim();

                string optionC =
                    txtOptionC.Text.Trim();

                string optionD =
                    txtOptionD.Text.Trim();


                string[] options =
                {
                    optionA.ToLower(),
                    optionB.ToLower(),
                    optionC.ToLower(),
                    optionD.ToLower()
                };


                if (
                    options[0] == options[1] ||
                    options[0] == options[2] ||
                    options[0] == options[3] ||
                    options[1] == options[2] ||
                    options[1] == options[3] ||
                    options[2] == options[3]
                )
                {
                    ShowMessage(
                        "Answer choices must be different from each other."
                    );

                    return;
                }
            }


            if (
                questionType == "Subjective"
                &&
                string.IsNullOrWhiteSpace(
                    txtModelAnswer.Text
                )
            )
            {
                ShowMessage(
                    "Please enter the model answer."
                );

                return;
            }

            if (txtQuestion.Text.Trim().Length > 2000)
            {
                ShowMessage(
                    "Question prompt cannot exceed 2000 characters."
                );

                return;
            }

            if (txtHint.Text.Trim().Length > 500)
            {
                ShowMessage(
                    "Student hint cannot exceed 500 characters."
                );

                return;
            }

            if (txtExplanation.Text.Trim().Length > 1000)
            {
                ShowMessage(
                    "Explanation cannot exceed 1000 characters."
                );

                return;
            }


            // -----------------------------------------------------
            // IMAGE
            // -----------------------------------------------------

            string mediaPath =
                hfExistingMediaPath.Value;


            string newlySavedFile =
                null;


            string oldMediaPath =
                hfExistingMediaPath.Value;


            bool removeExisting =
                hfRemoveExistingImage.Value
                    .Equals(
                        "true",
                        StringComparison.OrdinalIgnoreCase
                    );


            if (removeExisting)
            {
                mediaPath = null;
            }


            if (questionImage.HasFile)
            {
                string extension =
                    Path.GetExtension(
                        questionImage.FileName
                    ).ToLower();


                string[] allowed =
                {
                    ".png",
                    ".jpg",
                    ".jpeg",
                    ".webp"
                };


                if (Array.IndexOf(
                    allowed,
                    extension) < 0)
                {
                    ShowMessage(
                        "Only PNG, JPG, JPEG or WebP images are allowed."
                    );

                    return;
                }


                if (
                    questionImage.PostedFile.ContentLength
                    > 5 * 1024 * 1024
                )
                {
                    ShowMessage(
                        "The image must be 5MB or smaller."
                    );

                    return;
                }


                string folder =
                    Server.MapPath(
                        "~/Images/Questions/"
                    );


                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(
                        folder
                    );
                }


                string fileName =
                    Guid.NewGuid().ToString()
                    + extension;


                newlySavedFile =
                    Path.Combine(
                        folder,
                        fileName
                    );


                questionImage.SaveAs(
                    newlySavedFile
                );


                mediaPath =
                    "~/Images/Questions/"
                    + fileName;
            }


            // -----------------------------------------------------
            // DATABASE TRANSACTION
            // -----------------------------------------------------

            using (SqlConnection connection =
                new SqlConnection(ConnectionString))
            {
                connection.Open();


                SqlTransaction transaction =
                    connection.BeginTransaction();


                try
                {
                    int questionId;


                    if (EditingQuestionId.HasValue)
                    {
                        questionId =
                            EditingQuestionId.Value;


                        UpdateQuestion(
                            connection,
                            transaction,
                            questionId,
                            mediaPath
                        );


                        DeleteQuestionOptions(
                            connection,
                            transaction,
                            questionId
                        );
                    }
                    else
                    {
                        questionId =
                            InsertQuestion(
                                connection,
                                transaction,
                                mediaPath
                            );
                    }


                    // MCQ options
                    if (questionType == "MCQ")
                    {
                        InsertOption(
                            connection,
                            transaction,
                            questionId,
                            txtOptionA.Text.Trim(),
                            correctOption == "A",
                            1
                        );


                        InsertOption(
                            connection,
                            transaction,
                            questionId,
                            txtOptionB.Text.Trim(),
                            correctOption == "B",
                            2
                        );


                        InsertOption(
                            connection,
                            transaction,
                            questionId,
                            txtOptionC.Text.Trim(),
                            correctOption == "C",
                            3
                        );


                        InsertOption(
                            connection,
                            transaction,
                            questionId,
                            txtOptionD.Text.Trim(),
                            correctOption == "D",
                            4
                        );
                    }


                    transaction.Commit();


                    // Delete previous image after successful DB update
                    if (
                        EditingQuestionId.HasValue
                        &&
                        !string.IsNullOrEmpty(
                            oldMediaPath
                        )
                        &&
                        oldMediaPath != mediaPath
                    )
                    {
                        DeletePhysicalImage(
                            oldMediaPath
                        );
                    }


                    string message =
                        EditingQuestionId.HasValue
                            ? "Question updated successfully."
                            : "Question saved successfully.";


                    string safeMessage =
                        HttpUtility
                            .JavaScriptStringEncode(
                                message
                            );


                    ClientScript.RegisterStartupScript(
                        GetType(),
                        "QuestionSaved",
                        "alert('" +
                        safeMessage +
                        "');" +
                        "window.location='QuestionBank.aspx';",
                        true
                    );
                }
                catch (Exception ex)
                {
                    transaction.Rollback();


                    // Remove newly uploaded image
                    // because DB save failed
                    if (
                        !string.IsNullOrEmpty(
                            newlySavedFile
                        )
                        &&
                        File.Exists(
                            newlySavedFile
                        )
                    )
                    {
                        File.Delete(
                            newlySavedFile
                        );
                    }


                    ShowMessage(
                        "Unable to save question: "
                        + ex.Message
                    );
                }
            }
        }


        // =========================================================
        // GET CORRECT OPTION
        // =========================================================

        private string GetCorrectOption()
        {
            if (correctA.Checked)
                return "A";

            if (correctB.Checked)
                return "B";

            if (correctC.Checked)
                return "C";

            if (correctD.Checked)
                return "D";


            return "";
        }


        // =========================================================
        // INSERT QUESTION
        // =========================================================

        private int InsertQuestion(
            SqlConnection connection,
            SqlTransaction transaction,
            string mediaPath)
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


            using (SqlCommand command =
                CreateQuestionCommand(
                    sql,
                    connection,
                    transaction,
                    mediaPath
                ))
            {
                return Convert.ToInt32(
                    command.ExecuteScalar()
                );
            }
        }


        // =========================================================
        // UPDATE QUESTION
        // =========================================================

        private void UpdateQuestion(
            SqlConnection connection,
            SqlTransaction transaction,
            int questionId,
            string mediaPath)
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
                    AND CreatedByUserId =
                        @CreatedByUserId;
            ";


            using (SqlCommand command =
                CreateQuestionCommand(
                    sql,
                    connection,
                    transaction,
                    mediaPath
                ))
            {
                command.Parameters.Add(
                    "@QuestionId",
                    SqlDbType.Int
                ).Value =
                    questionId;


                int rows =
                    command.ExecuteNonQuery();


                if (rows == 0)
                {
                    throw new Exception(
                        "Question not found or you do not have permission to edit it."
                    );
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
            SqlCommand command =
                new SqlCommand(
                    sql,
                    connection,
                    transaction
                );


            command.Parameters.Add(
                "@SubjectId",
                SqlDbType.Int
            ).Value =
                Convert.ToInt32(
                    ddlSubject.SelectedValue
                );


            command.Parameters.Add(
                "@CreatedByUserId",
                SqlDbType.Int
            ).Value =
                CurrentUserId;


            command.Parameters.Add(
                "@QuestionText",
                SqlDbType.NVarChar
            ).Value =
                txtQuestion.Text.Trim();


            command.Parameters.Add(
                "@QuestionType",
                SqlDbType.NVarChar,
                30
            ).Value =
                ddlQuestionType.SelectedValue;


            command.Parameters.Add(
                "@Difficulty",
                SqlDbType.NVarChar,
                20
            ).Value =
                ddlDifficulty.SelectedValue;


            command.Parameters.Add(
                "@HintText",
                SqlDbType.NVarChar
            ).Value =
                GetDbValue(
                    txtHint.Text
                );


            command.Parameters.Add(
                "@Explanation",
                SqlDbType.NVarChar
            ).Value =
                GetDbValue(
                    txtExplanation.Text
                );


            command.Parameters.Add(
                "@ModelAnswer",
                SqlDbType.NVarChar
            ).Value =
                ddlQuestionType.SelectedValue
                    == "Subjective"
                    ? GetDbValue(
                        txtModelAnswer.Text
                    )
                    : (object)DBNull.Value;


            command.Parameters.Add(
                "@MediaPath",
                SqlDbType.NVarChar,
                500
            ).Value =
                string.IsNullOrEmpty(
                    mediaPath
                )
                    ? (object)DBNull.Value
                    : mediaPath;


            return command;
        }


        // =========================================================
        // DELETE EXISTING OPTIONS BEFORE UPDATE
        // =========================================================

        private void DeleteQuestionOptions(
            SqlConnection connection,
            SqlTransaction transaction,
            int questionId)
        {
            const string sql = @"
                DELETE FROM QuestionOptions
                WHERE QuestionId =
                      @QuestionId;
            ";


            using (SqlCommand command =
                new SqlCommand(
                    sql,
                    connection,
                    transaction
                ))
            {
                command.Parameters.Add(
                    "@QuestionId",
                    SqlDbType.Int
                ).Value =
                    questionId;


                command.ExecuteNonQuery();
            }
        }


        // =========================================================
        // INSERT OPTION
        // =========================================================

        private void InsertOption(
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


            using (SqlCommand command =
                new SqlCommand(
                    sql,
                    connection,
                    transaction
                ))
            {
                command.Parameters.Add(
                    "@QuestionId",
                    SqlDbType.Int
                ).Value =
                    questionId;


                command.Parameters.Add(
                    "@OptionText",
                    SqlDbType.NVarChar
                ).Value =
                    optionText;


                command.Parameters.Add(
                    "@IsCorrect",
                    SqlDbType.Bit
                ).Value =
                    isCorrect;


                command.Parameters.Add(
                    "@DisplayOrder",
                    SqlDbType.Int
                ).Value =
                    displayOrder;


                command.ExecuteNonQuery();
            }
        }


        // =========================================================
        // DELETE IMAGE FILE
        // =========================================================

        private void DeletePhysicalImage(
            string mediaPath)
        {
            try
            {
                if (
                    string.IsNullOrWhiteSpace(
                        mediaPath
                    )
                )
                {
                    return;
                }


                if (!mediaPath.StartsWith("~/"))
                {
                    return;
                }


                string physicalPath =
                    Server.MapPath(
                        mediaPath
                    );


                if (File.Exists(
                    physicalPath))
                {
                    File.Delete(
                        physicalPath
                    );
                }
            }
            catch
            {
                // Image cleanup failure should
                // not undo successful DB update.
            }
        }


        // =========================================================
        // NULLABLE VALUES
        // =========================================================

        private object GetDbValue(
            string value)
        {
            if (string.IsNullOrWhiteSpace(
                value))
            {
                return DBNull.Value;
            }


            return value.Trim();
        }


        // =========================================================
        // MESSAGE
        // =========================================================

        private void ShowMessage(
            string message)
        {
            string safeMessage =
                HttpUtility
                    .JavaScriptStringEncode(
                        message
                    );


            ClientScript.RegisterStartupScript(
                GetType(),
                Guid.NewGuid().ToString(),
                "alert('" +
                safeMessage +
                "');",
                true
            );
        }
    }
}