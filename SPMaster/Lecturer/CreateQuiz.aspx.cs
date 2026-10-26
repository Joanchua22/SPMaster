using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace SPMaster.Lecturer
{
    public partial class CreateQuiz : System.Web.UI.Page
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
        // EDITING QUIZ ID
        // CreateQuiz.aspx?id=5
        // =========================================================

        private int? EditingQuizId
        {
            get
            {
                int quizId;

                if (int.TryParse(Request.QueryString["id"], out quizId))
                {
                    return quizId;
                }

                return null;
            }
        }


        // =========================================================
        // ALLOCATED QUESTIONS
        //
        // Stored temporarily in ViewState while lecturer edits
        // the quiz before saving it into QuizQuestions.
        // =========================================================

        private DataTable AllocatedQuestions
        {
            get
            {
                if (ViewState["AllocatedQuestions"] == null)
                {
                    ViewState["AllocatedQuestions"] = CreateAllocatedQuestionTable();
                }

                return (DataTable)ViewState["AllocatedQuestions"];
            }

            set
            {
                ViewState["AllocatedQuestions"] = value;
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

            lblQuestionValidation.Visible = false;

            if (!IsPostBack)
            {
                BindSubjects();
                BindForms();


                // =================================================
                // EDIT EXISTING DRAFT
                // =================================================

                if (EditingQuizId.HasValue)
                {
                    bool loaded = LoadQuiz(EditingQuizId.Value);

                    if (!loaded)
                    {
                        Response.Redirect("~/Lecturer/Quizzes.aspx");
                        return;
                    }

                    LoadAllocatedQuestions(EditingQuizId.Value);
                }


                // =================================================
                // CREATE NEW QUIZ
                // =================================================

                else
                {
                    AllocatedQuestions = CreateAllocatedQuestionTable();

                    // Default pass percentage
                    txtPassPercentage.Text = "50";

                    // Example:
                    // Question Bank ->
                    // Add to My Quiz ->
                    // Create New Quiz ->
                    // CreateQuiz.aspx?questionId=12
                    LoadIncomingQuestion();
                }

                BindAllocatedQuestions();
                BindBankQuestions();

                if (Request.QueryString["saved"] == "1")
                {
                    lblSaveSuccess.Text = "Draft saved successfully.";
                    pnlSaveSuccess.Visible = true;
                }
            }
        }


        // =========================================================
        // CREATE ALLOCATED QUESTION TABLE
        // =========================================================

        private DataTable CreateAllocatedQuestionTable()
        {
            DataTable table = new DataTable();

            table.Columns.Add("QuestionId", typeof(int));
            table.Columns.Add("SubjectId", typeof(int));
            table.Columns.Add("SubjectName", typeof(string));
            table.Columns.Add("QuestionType", typeof(string));
            table.Columns.Add("Difficulty", typeof(string));
            table.Columns.Add("QuestionText", typeof(string));
            table.Columns.Add("MaxMarks", typeof(int));
            table.Columns.Add("QuestionOrder", typeof(int));

            return table;
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

                ddlQuizSubject.DataSource = command.ExecuteReader();
                ddlQuizSubject.DataTextField = "SubjectName";
                ddlQuizSubject.DataValueField = "SubjectId";
                ddlQuizSubject.DataBind();
            }

            ddlQuizSubject.Items.Insert(
                0,
                new ListItem("Select Subject", "")
            );
        }


        // =========================================================
        // LOAD FORM LEVELS
        // =========================================================

        private void BindForms()
        {
            const string sql = @"
                SELECT
                    FormId,
                    FormName
                FROM Forms
                WHERE
                    IsActive = 1
                ORDER BY
                    FormId;
            ";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                ddlFormLevel.DataSource = command.ExecuteReader();
                ddlFormLevel.DataTextField = "FormName";
                ddlFormLevel.DataValueField = "FormId";
                ddlFormLevel.DataBind();
            }

            ddlFormLevel.Items.Insert(
                0,
                new ListItem("Select Form", "")
            );
        }


        // =========================================================
        // LOAD EXISTING DRAFT QUIZ
        // =========================================================

        private bool LoadQuiz(int quizId)
        {
            const string sql = @"
                SELECT
                    SubjectId,
                    FormId,
                    Title,
                    Description,
                    TimeLimitMinutes,
                    PassPercentage
                FROM Quizzes
                WHERE
                    QuizId = @QuizId
                    AND CreatedByUserId = @UserId
                    AND IsPublished = 0
                    AND IsArchived = 0;
            ";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@QuizId", SqlDbType.Int).Value = quizId;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = CurrentUserId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return false;
                    }

                    txtQuizTitle.Text = reader["Title"].ToString();
                    ddlQuizSubject.SelectedValue = reader["SubjectId"].ToString();
                    ddlFormLevel.SelectedValue = reader["FormId"].ToString();
                    txtTimeLimit.Text = reader["TimeLimitMinutes"].ToString();
                    txtPassPercentage.Text = reader["PassPercentage"].ToString();

                    txtQuizInstructions.Text =
                        reader["Description"] == DBNull.Value
                            ? ""
                            : reader["Description"].ToString();

                    return true;
                }
            }
        }


        // =========================================================
        // LOAD QUESTIONS ALREADY IN DRAFT QUIZ
        // =========================================================

        private void LoadAllocatedQuestions(int quizId)
        {
            const string sql = @"
                SELECT
                    q.QuestionId,
                    q.SubjectId,
                    s.SubjectName,
                    q.QuestionType,
                    q.Difficulty,
                    q.QuestionText,
                    qq.MaxMarks,
                    qq.QuestionOrder
                FROM QuizQuestions qq
                INNER JOIN Questions q
                    ON qq.QuestionId = q.QuestionId
                INNER JOIN Subjects s
                    ON q.SubjectId = s.SubjectId
                WHERE
                    qq.QuizId = @QuizId
                ORDER BY
                    qq.QuestionOrder;
            ";

            DataTable table = CreateAllocatedQuestionTable();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@QuizId", SqlDbType.Int).Value = quizId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        table.Rows.Add(
                            Convert.ToInt32(reader["QuestionId"]),
                            Convert.ToInt32(reader["SubjectId"]),
                            reader["SubjectName"].ToString(),
                            reader["QuestionType"].ToString(),
                            reader["Difficulty"].ToString(),
                            reader["QuestionText"].ToString(),
                            Convert.ToInt32(reader["MaxMarks"]),
                            Convert.ToInt32(reader["QuestionOrder"])
                        );
                    }
                }
            }

            AllocatedQuestions = table;
        }


        // =========================================================
        // QUESTION PASSED FROM SHARED QUESTION BANK
        //
        // Example:
        // CreateQuiz.aspx?questionId=7
        // =========================================================

        private void LoadIncomingQuestion()
        {
            int questionId;

            if (!int.TryParse(Request.QueryString["questionId"], out questionId))
            {
                return;
            }

            AddQuestionToAllocation(
                questionId,
                true
            );
        }


        // =========================================================
        // ADD QUESTION TO TEMPORARY ALLOCATION
        // =========================================================

        private bool AddQuestionToAllocation(
            int questionId,
            bool autoSelectSubject)
        {
            // Prevent duplicate question
            if (IsQuestionAllocated(questionId))
            {
                return false;
            }

            const string sql = @"
                SELECT
                    q.QuestionId,
                    q.SubjectId,
                    s.SubjectName,
                    q.QuestionType,
                    q.Difficulty,
                    q.QuestionText
                FROM Questions q
                INNER JOIN Subjects s
                    ON q.SubjectId = s.SubjectId
                WHERE
                    q.QuestionId = @QuestionId
                    AND q.IsArchived = 0;
            ";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@QuestionId", SqlDbType.Int).Value = questionId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return false;
                    }

                    int subjectId = Convert.ToInt32(reader["SubjectId"]);


                    // =================================================
                    // QUESTION PASSED FROM QUESTION BANK
                    // Automatically choose its subject
                    // =================================================

                    if (
                        autoSelectSubject
                        && string.IsNullOrEmpty(ddlQuizSubject.SelectedValue)
                    )
                    {
                        ddlQuizSubject.SelectedValue = subjectId.ToString();
                    }


                    // =================================================
                    // PREVENT SUBJECT MIXING
                    // =================================================

                    if (
                        !string.IsNullOrEmpty(ddlQuizSubject.SelectedValue)
                        && Convert.ToInt32(ddlQuizSubject.SelectedValue) != subjectId
                    )
                    {
                        return false;
                    }

                    DataTable table = AllocatedQuestions;
                    DataRow row = table.NewRow();

                    row["QuestionId"] = questionId;
                    row["SubjectId"] = subjectId;
                    row["SubjectName"] = reader["SubjectName"].ToString();
                    row["QuestionType"] = reader["QuestionType"].ToString();
                    row["Difficulty"] = reader["Difficulty"].ToString();
                    row["QuestionText"] = reader["QuestionText"].ToString();

                    // Default marks for newly added question
                    row["MaxMarks"] = 1;
                    row["QuestionOrder"] = table.Rows.Count + 1;

                    table.Rows.Add(row);

                    AllocatedQuestions = table;

                    return true;
                }
            }
        }


        // =========================================================
        // CHECK WHETHER QUESTION IS ALREADY ALLOCATED
        // =========================================================

        private bool IsQuestionAllocated(int questionId)
        {
            foreach (DataRow row in AllocatedQuestions.Rows)
            {
                if (Convert.ToInt32(row["QuestionId"]) == questionId)
                {
                    return true;
                }
            }

            return false;
        }


        // =========================================================
        // BIND ALLOCATED QUESTIONS
        // =========================================================

        private void BindAllocatedQuestions()
        {
            DataTable table = AllocatedQuestions;


            // -----------------------------------------------------
            // RECALCULATE ORDER
            // -----------------------------------------------------

            for (int i = 0; i < table.Rows.Count; i++)
            {
                table.Rows[i]["QuestionOrder"] = i + 1;
            }

            AllocatedQuestions = table;


            // -----------------------------------------------------
            // BIND REPEATER
            // -----------------------------------------------------

            rptAllocatedQuestions.DataSource = table;
            rptAllocatedQuestions.DataBind();


            // -----------------------------------------------------
            // EMPTY STATE
            // -----------------------------------------------------

            pnlNoAllocatedQuestions.Visible = table.Rows.Count == 0;


            // -----------------------------------------------------
            // TOTAL MARKS
            // -----------------------------------------------------

            int totalMarks = 0;

            foreach (DataRow row in table.Rows)
            {
                totalMarks += Convert.ToInt32(row["MaxMarks"]);
            }

            litQuizSummary.Text =
                table.Rows.Count
                + " Questions • "
                + totalMarks
                + " Total Marks";
        }


        // =========================================================
        // CAPTURE MARK VALUES FROM UI
        //
        // JavaScript updates the visible summary instantly.
        // This method copies those edited values back into the
        // ViewState DataTable before server-side operations.
        // =========================================================

        private void CaptureAllocatedMarks()
        {
            DataTable table = AllocatedQuestions;

            foreach (RepeaterItem item in rptAllocatedQuestions.Items)
            {
                HiddenField hiddenQuestionId =
                    (HiddenField)item.FindControl("hfAllocatedQuestionId");

                TextBox marksBox =
                    (TextBox)item.FindControl("txtMarks");

                int questionId;
                int marks;

                if (
                    hiddenQuestionId == null
                    || marksBox == null
                    || !int.TryParse(hiddenQuestionId.Value, out questionId)
                )
                {
                    continue;
                }

                if (
                    !int.TryParse(marksBox.Text, out marks)
                    || marks < 1
                )
                {
                    marks = 1;
                }

                DataRow[] rows = table.Select(
                    "QuestionId = " + questionId
                );

                if (rows.Length > 0)
                {
                    rows[0]["MaxMarks"] = marks;
                }
            }

            AllocatedQuestions = table;
        }


        // =========================================================
        // REMOVE ALLOCATED QUESTION
        // =========================================================

        protected void rptAllocatedQuestions_ItemCommand(
            object source,
            RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "RemoveQuestion")
            {
                return;
            }

            lblQuestionValidation.Visible = false;

            CaptureAllocatedMarks();

            int questionId = Convert.ToInt32(e.CommandArgument);
            DataTable table = AllocatedQuestions;

            DataRow[] rows = table.Select(
                "QuestionId = " + questionId
            );

            if (rows.Length > 0)
            {
                table.Rows.Remove(rows[0]);
            }

            AllocatedQuestions = table;

            BindAllocatedQuestions();
            BindBankQuestions();
        }


        // =========================================================
        // SUBJECT CHANGED
        //
        // JavaScript asks for confirmation BEFORE the postback.
        // If lecturer confirms, incompatible questions are removed.
        // =========================================================

        protected void ddlQuizSubject_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            lblQuestionValidation.Visible = false;

            // Preserve marks entered before postback
            CaptureAllocatedMarks();


            // -----------------------------------------------------
            // NO SUBJECT SELECTED
            // -----------------------------------------------------

            if (string.IsNullOrEmpty(ddlQuizSubject.SelectedValue))
            {
                BindAllocatedQuestions();
                BindBankQuestions();
                return;
            }

            int selectedSubjectId =
                Convert.ToInt32(ddlQuizSubject.SelectedValue);

            DataTable table = AllocatedQuestions;


            // -----------------------------------------------------
            // REMOVE QUESTIONS THAT NO LONGER MATCH SUBJECT
            // -----------------------------------------------------

            for (int i = table.Rows.Count - 1; i >= 0; i--)
            {
                int questionSubjectId =
                    Convert.ToInt32(table.Rows[i]["SubjectId"]);

                if (questionSubjectId != selectedSubjectId)
                {
                    table.Rows.RemoveAt(i);
                }
            }

            AllocatedQuestions = table;

            BindAllocatedQuestions();
            BindBankQuestions();
        }


        // =========================================================
        // LOAD QUESTIONS FOR QUESTION BANK MODAL
        // =========================================================

        private void BindBankQuestions()
        {
            // -----------------------------------------------------
            // SUBJECT MUST BE SELECTED FIRST
            // -----------------------------------------------------

            if (string.IsNullOrEmpty(ddlQuizSubject.SelectedValue))
            {
                rptBankQuestions.DataSource = null;
                rptBankQuestions.DataBind();

                pnlNoBankQuestions.Visible = true;
                litBankSubject.Text = "selected subject";

                return;
            }

            int subjectId =
                Convert.ToInt32(ddlQuizSubject.SelectedValue);

            const string sql = @"
                SELECT
                    q.QuestionId,
                    q.QuestionText,
                    q.QuestionType,
                    q.Difficulty,
                    u.FullName AS CreatorName
                FROM Questions q
                INNER JOIN Users u
                    ON q.CreatedByUserId = u.UserId
                WHERE
                    q.SubjectId = @SubjectId
                    AND q.IsArchived = 0
                ORDER BY
                    q.CreatedAt DESC;
            ";

            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@SubjectId", SqlDbType.Int).Value = subjectId;

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }


            // -----------------------------------------------------
            // REMOVE QUESTIONS ALREADY ALLOCATED
            // -----------------------------------------------------

            for (int i = table.Rows.Count - 1; i >= 0; i--)
            {
                int questionId =
                    Convert.ToInt32(table.Rows[i]["QuestionId"]);

                if (IsQuestionAllocated(questionId))
                {
                    table.Rows.RemoveAt(i);
                }
            }

            rptBankQuestions.DataSource = table;
            rptBankQuestions.DataBind();

            pnlNoBankQuestions.Visible = table.Rows.Count == 0;
            litBankSubject.Text = ddlQuizSubject.SelectedItem.Text;
        }


        // =========================================================
        // ADD SELECTED BANK QUESTIONS
        // =========================================================

        protected void btnAddToQuiz_Click(object sender, EventArgs e)
        {
            lblQuestionValidation.Visible = false;

            CaptureAllocatedMarks();

            int addedCount = 0;

            foreach (RepeaterItem item in rptBankQuestions.Items)
            {
                CheckBox checkBox =
                    (CheckBox)item.FindControl("chkSelectQuestion");

                HiddenField hiddenId =
                    (HiddenField)item.FindControl("hfBankQuestionId");

                if (
                    checkBox == null
                    || hiddenId == null
                    || !checkBox.Checked
                )
                {
                    continue;
                }

                int questionId;

                if (
                    int.TryParse(hiddenId.Value, out questionId)
                    && AddQuestionToAllocation(questionId, false)
                )
                {
                    addedCount++;
                }
            }

            BindAllocatedQuestions();
            BindBankQuestions();


            // -----------------------------------------------------
            // NOTHING SELECTED
            // -----------------------------------------------------

            if (addedCount == 0)
            {
                ShowQuestionMessage(
                    "Please select at least one question from the Question Bank."
                );
            }
        }


        // =========================================================
        // SAVE DRAFT BUTTON
        // =========================================================

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            // ASP.NET validators:
            // errors appear directly under each input
            Page.Validate("QuizValidation");

            if (!Page.IsValid)
            {
                return;
            }

            SaveQuiz(false);
        }


        // =========================================================
        // PREVIEW & PUBLISH BUTTON
        // =========================================================

        protected void btnPreviewPublish_Click(object sender, EventArgs e)
        {
            Page.Validate("QuizValidation");

            if (!Page.IsValid)
            {
                return;
            }

            // Preview requires questions
            if (AllocatedQuestions.Rows.Count == 0)
            {
                ShowQuestionMessage(
                    "Please add at least one question before previewing the quiz."
                );

                return;
            }

            SaveQuiz(true);
        }


        // =========================================================
        // SAVE QUIZ
        // =========================================================

        private void SaveQuiz(bool goToPreview)
        {
            lblQuestionValidation.Visible = false;

            // Store current mark values
            CaptureAllocatedMarks();

            if (!Page.IsValid)
            {
                return;
            }


            // -----------------------------------------------------
            // PARSE VALIDATED VALUES
            // -----------------------------------------------------

            int timeLimit;

            if (!int.TryParse(txtTimeLimit.Text, out timeLimit))
            {
                return;
            }

            decimal passPercentage;

            if (!decimal.TryParse(txtPassPercentage.Text, out passPercentage))
            {
                return;
            }

            int selectedSubjectId =
                Convert.ToInt32(ddlQuizSubject.SelectedValue);


            // =====================================================
            // PREVIEW REQUIRES QUESTIONS
            // =====================================================

            if (
                goToPreview
                && AllocatedQuestions.Rows.Count == 0
            )
            {
                ShowQuestionMessage(
                    "Please add at least one question before previewing the quiz."
                );

                return;
            }


            // =====================================================
            // FINAL SUBJECT SAFETY CHECK
            // =====================================================

            foreach (DataRow row in AllocatedQuestions.Rows)
            {
                if (Convert.ToInt32(row["SubjectId"]) != selectedSubjectId)
                {
                    ShowQuestionMessage(
                        "All allocated questions must match the selected quiz subject."
                    );

                    return;
                }
            }


            // =====================================================
            // DATABASE TRANSACTION
            // =====================================================

            int savedQuizId = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // =============================================
                        // UPDATE EXISTING DRAFT
                        // =============================================

                        if (EditingQuizId.HasValue)
                        {
                            savedQuizId = EditingQuizId.Value;

                            UpdateQuiz(
                                connection,
                                transaction,
                                savedQuizId,
                                timeLimit,
                                passPercentage
                            );
                        }


                        // =============================================
                        // INSERT NEW DRAFT
                        // =============================================

                        else
                        {
                            savedQuizId = InsertQuiz(
                                connection,
                                transaction,
                                timeLimit,
                                passPercentage
                            );
                        }


                        // =============================================
                        // REPLACE QUESTION MAPPINGS
                        // =============================================

                        DeleteQuizQuestions(
                            connection,
                            transaction,
                            savedQuizId
                        );

                        InsertQuizQuestions(
                            connection,
                            transaction,
                            savedQuizId
                        );

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        if (transaction.Connection != null)
                        {
                            transaction.Rollback();
                        }

                        ShowQuestionMessage(
                            "Unable to save the quiz. "
                            + ex.Message
                        );

                        return;
                    }
                }
            }


            // =====================================================
            // PREVIEW
            // =====================================================

            if (goToPreview)
            {
                Response.Redirect(
                    "~/Lecturer/PreviewQuiz.aspx?id="
                    + savedQuizId
                    + "&from=editor",
                    false
                );

                Context.ApplicationInstance.CompleteRequest();
                return;
            }


            // =====================================================
            // SAVE DRAFT
            //
            // Reload same quiz as Edit mode.
            // This also resets the unsaved-changes flag.
            // =====================================================

            Response.Redirect(
                "~/Lecturer/CreateQuiz.aspx?id="
                + savedQuizId
                + "&saved=1",
                false
            );

            Context.ApplicationInstance.CompleteRequest();
        }


        // =========================================================
        // INSERT NEW QUIZ
        // =========================================================

        private int InsertQuiz(
            SqlConnection connection,
            SqlTransaction transaction,
            int timeLimit,
            decimal passPercentage)
        {
            const string sql = @"
                INSERT INTO Quizzes
                (
                    SubjectId,
                    FormId,
                    CreatedByUserId,
                    Title,
                    Description,
                    TimeLimitMinutes,
                    PassPercentage,
                    IsPublished,
                    IsArchived,
                    CreatedAt
                )
                VALUES
                (
                    @SubjectId,
                    @FormId,
                    @CreatedByUserId,
                    @Title,
                    @Description,
                    @TimeLimitMinutes,
                    @PassPercentage,
                    0,
                    0,
                    GETDATE()
                );

                SELECT CAST(
                    SCOPE_IDENTITY()
                    AS INT
                );
            ";

            using (SqlCommand command = CreateQuizCommand(
                sql,
                connection,
                transaction,
                timeLimit,
                passPercentage
            ))
            {
                return Convert.ToInt32(
                    command.ExecuteScalar()
                );
            }
        }


        // =========================================================
        // UPDATE EXISTING DRAFT
        // =========================================================

        private void UpdateQuiz(
            SqlConnection connection,
            SqlTransaction transaction,
            int quizId,
            int timeLimit,
            decimal passPercentage)
        {
            const string sql = @"
                UPDATE Quizzes
                SET
                    SubjectId = @SubjectId,
                    FormId = @FormId,
                    Title = @Title,
                    Description = @Description,
                    TimeLimitMinutes = @TimeLimitMinutes,
                    PassPercentage = @PassPercentage
                WHERE
                    QuizId = @QuizId
                    AND CreatedByUserId = @CreatedByUserId
                    AND IsPublished = 0
                    AND IsArchived = 0;
            ";

            using (SqlCommand command = CreateQuizCommand(
                sql,
                connection,
                transaction,
                timeLimit,
                passPercentage
            ))
            {
                command.Parameters.Add("@QuizId", SqlDbType.Int).Value = quizId;

                int affectedRows = command.ExecuteNonQuery();

                if (affectedRows == 0)
                {
                    throw new Exception(
                        "Quiz was not found or cannot be edited."
                    );
                }
            }
        }


        // =========================================================
        // COMMON QUIZ PARAMETERS
        // =========================================================

        private SqlCommand CreateQuizCommand(
            string sql,
            SqlConnection connection,
            SqlTransaction transaction,
            int timeLimit,
            decimal passPercentage)
        {
            SqlCommand command = new SqlCommand(
                sql,
                connection,
                transaction
            );

            command.Parameters.Add("@SubjectId", SqlDbType.Int).Value =
                Convert.ToInt32(ddlQuizSubject.SelectedValue);

            command.Parameters.Add("@FormId", SqlDbType.Int).Value =
                Convert.ToInt32(ddlFormLevel.SelectedValue);

            command.Parameters.Add("@CreatedByUserId", SqlDbType.Int).Value =
                CurrentUserId;

            command.Parameters.Add("@Title", SqlDbType.NVarChar, 250).Value =
                txtQuizTitle.Text.Trim();


            // -----------------------------------------------------
            // Current schema:
            // Instructions are stored in Quizzes.Description.
            // -----------------------------------------------------

            command.Parameters.Add("@Description", SqlDbType.NVarChar).Value =
                string.IsNullOrWhiteSpace(txtQuizInstructions.Text)
                    ? (object)DBNull.Value
                    : txtQuizInstructions.Text.Trim();

            command.Parameters.Add("@TimeLimitMinutes", SqlDbType.Int).Value =
                timeLimit;

            SqlParameter passParameter =
                command.Parameters.Add(
                    "@PassPercentage",
                    SqlDbType.Decimal
                );

            passParameter.Precision = 5;
            passParameter.Scale = 2;
            passParameter.Value = passPercentage;

            return command;
        }


        // =========================================================
        // REMOVE OLD QUIZQUESTION RECORDS
        //
        // When updating a draft, current mappings are replaced
        // with the lecturer's latest allocation.
        // =========================================================

        private void DeleteQuizQuestions(
            SqlConnection connection,
            SqlTransaction transaction,
            int quizId)
        {
            const string sql = @"
                DELETE FROM QuizQuestions
                WHERE
                    QuizId = @QuizId;
            ";

            using (SqlCommand command = new SqlCommand(
                sql,
                connection,
                transaction
            ))
            {
                command.Parameters.Add("@QuizId", SqlDbType.Int).Value = quizId;

                command.ExecuteNonQuery();
            }
        }


        // =========================================================
        // SAVE CURRENT QUESTION ALLOCATION
        // =========================================================

        private void InsertQuizQuestions(
            SqlConnection connection,
            SqlTransaction transaction,
            int quizId)
        {
            const string sql = @"
                INSERT INTO QuizQuestions
                (
                    QuizId,
                    QuestionId,
                    QuestionOrder,
                    MaxMarks
                )
                VALUES
                (
                    @QuizId,
                    @QuestionId,
                    @QuestionOrder,
                    @MaxMarks
                );
            ";

            int order = 1;

            foreach (DataRow row in AllocatedQuestions.Rows)
            {
                using (SqlCommand command = new SqlCommand(
                    sql,
                    connection,
                    transaction
                ))
                {
                    command.Parameters.Add("@QuizId", SqlDbType.Int).Value =
                        quizId;

                    command.Parameters.Add("@QuestionId", SqlDbType.Int).Value =
                        Convert.ToInt32(row["QuestionId"]);

                    command.Parameters.Add("@QuestionOrder", SqlDbType.Int).Value =
                        order;

                    command.Parameters.Add("@MaxMarks", SqlDbType.Int).Value =
                        Convert.ToInt32(row["MaxMarks"]);

                    command.ExecuteNonQuery();
                }

                order++;
            }
        }


        // =========================================================
        // QUESTION TYPE DISPLAY
        // =========================================================

        protected string GetQuestionTypeDisplay(object questionType)
        {
            string type = Convert.ToString(questionType);

            if (
                type.Equals(
                    "MCQ",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return "Single Choice";
            }

            return "Subjective";
        }


        // =========================================================
        // QUESTION TYPE CSS
        // =========================================================

        protected string GetQuestionTypeCss(object questionType)
        {
            string type = Convert.ToString(questionType);

            return type.Equals(
                "MCQ",
                StringComparison.OrdinalIgnoreCase
            )
                ? "type-mcq"
                : "type-subjective";
        }


        // =========================================================
        // QUESTION / QUIZ-LEVEL INLINE MESSAGE
        // =========================================================

        private void ShowQuestionMessage(string message)
        {
            lblQuestionValidation.Text = message;
            lblQuestionValidation.Visible = true;
        }
    }
}