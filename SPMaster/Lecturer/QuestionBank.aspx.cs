using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SPMaster.Lecturer
{
    public partial class QuestionBank : System.Web.UI.Page
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
        // CURRENT TAB
        // mine / shared
        // =========================================================

        private string QuestionMode
        {
            get
            {
                if (ViewState["QuestionMode"] == null)
                {
                    return "mine";
                }

                return ViewState["QuestionMode"].ToString();
            }
            set
            {
                ViewState["QuestionMode"] = value;
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


            if (!IsPostBack)
            {
                QuestionMode = "mine";
                BindSubjects();
                BindQuestionCounts();
                BindQuestions();
            }

            UpdateTabStyle();
        }


        // =========================================================
        // BIND SUBJECT DROPDOWN
        // =========================================================

        private void BindSubjects()
        {
            const string sql = @"SELECT SubjectId,SubjectName FROM Subjects WHERE IsActive = 1 ORDER BY SubjectName;";


            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    ddlSubject.DataSource = command.ExecuteReader();
                    ddlSubject.DataTextField = "SubjectName";
                    ddlSubject.DataValueField = "SubjectId";
                    ddlSubject.DataBind();
                }
            }

            ddlSubject.Items.Insert(0,new ListItem("All Subjects", ""));
        }


        // =========================================================
        // QUESTION COUNTS
        // =========================================================

        private void BindQuestionCounts(){
            const string sql = @"SELECT SUM(CASE WHEN CreatedByUserId = @UserId AND IsArchived = 0 THEN 1 ELSE 0 END) AS MyQuestionCount, 
                                SUM(CASE WHEN CreatedByUserId <> @UserId AND IsArchived = 0 THEN 1 ELSE 0 END) AS SharedQuestionCount FROM Questions;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = CurrentUserId;
                    
                    connection.Open();
                    
                    using (SqlDataReader reader = command.ExecuteReader()){
                        if (reader.Read()){
                            int myCount = reader["MyQuestionCount"] == DBNull.Value? 0: Convert.ToInt32(reader["MyQuestionCount"]);
                            int sharedCount = reader["SharedQuestionCount"] == DBNull.Value? 0: Convert.ToInt32(reader["SharedQuestionCount"]);
                            btnMyQuestions.Text ="My Questions (" + myCount +")";
                            btnSharedQuestions.Text ="Shared Faculty Library (" + sharedCount +")";
                        }
                    }
                }
            }
        }


        // =========================================================
        // BIND QUESTIONS
        // =========================================================

        private void BindQuestions()
        {
            string sql = @"SELECT q.QuestionId, q.SubjectId, s.SubjectName, q.QuestionText, q.QuestionType,q.Difficulty,q.ModelAnswer, q.MediaPath, q.CreatedAt, u.FullName AS CreatorName,
                        CASE WHEN q.CreatedByUserId = @CurrentUserId THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsOwner,
                        (SELECT COUNT(DISTINCT qq.QuizId) FROM QuizQuestions qq WHERE qq.QuestionId = q.QuestionId) AS UsedInQuizzes,
                        (SELECT TOP 1 qo.OptionText FROM QuestionOptions qo WHERE qo.QuestionId = q.QuestionId AND qo.DisplayOrder = 1) AS OptionA,
                        (SELECT TOP 1 qo.OptionText FROM QuestionOptions qo WHERE qo.QuestionId = q.QuestionId AND qo.DisplayOrder = 2) AS OptionB,
                        (SELECT TOP 1 qo.OptionText FROM QuestionOptions qo WHERE qo.QuestionId = q.QuestionId AND qo.DisplayOrder = 3) AS OptionC,
                        (SELECT TOP 1 qo.OptionText FROM QuestionOptions qo WHERE qo.QuestionId = q.QuestionId AND qo.DisplayOrder = 4) AS OptionD,
                        (SELECT TOP 1 CASE qo.DisplayOrder WHEN 1 THEN 'A' WHEN 2 THEN 'B' WHEN 3 THEN 'C' WHEN 4 THEN 'D' END FROM QuestionOptions qo WHERE qo.QuestionId = q.QuestionId AND qo.IsCorrect = 1) AS CorrectOption,
                        (SELECT TOP 1 qo.OptionText FROM QuestionOptions qo WHERE qo.QuestionId = q.QuestionId AND qo.IsCorrect = 1) AS CorrectAnswerText FROM Questions q
                        INNER JOIN Subjects s ON q.SubjectId = s.SubjectId
                        INNER JOIN Users u ON q.CreatedByUserId = u.UserId WHERE q.IsArchived = 0 AND (
                        (@Mode = 'mine' AND q.CreatedByUserId = @CurrentUserId) OR (@Mode = 'shared' AND q.CreatedByUserId <> @CurrentUserId)) AND
                        (@SubjectId IS NULL OR q.SubjectId = @SubjectId) AND
                        (@QuestionType = '' OR q.QuestionType = @QuestionType) AND
                        (@Difficulty = ''OR q.Difficulty =@Difficulty) AND
                        (@Search = '' OR q.QuestionText LIKE '%' + @Search + '%') ORDER BY q.CreatedAt DESC;";

            DataTable questionTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@CurrentUserId",SqlDbType.Int).Value = CurrentUserId;
                    command.Parameters.Add("@Mode",SqlDbType.VarChar,10).Value = QuestionMode;
                    SqlParameter subjectParameter = command.Parameters.Add( "@SubjectId", SqlDbType.Int);

                    if (string.IsNullOrEmpty(ddlSubject.SelectedValue))
                    {
                        subjectParameter.Value =
                            DBNull.Value;
                    }
                    else
                    {
                        subjectParameter.Value = Convert.ToInt32(ddlSubject.SelectedValue);
                    }

                    command.Parameters.Add("@QuestionType",SqlDbType.NVarChar,30).Value =ddlQuestionType.SelectedValue;
                    command.Parameters.Add("@Difficulty",SqlDbType.NVarChar,30).Value = ddlDifficulty.SelectedValue;
                    command.Parameters.Add("@Search", SqlDbType.NVarChar, 500).Value = txtSearch.Text.Trim();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(questionTable);
                    }
                }
            }


            rptQuestions.DataSource = questionTable;
            rptQuestions.DataBind();
            pnlNoQuestions.Visible = questionTable.Rows.Count == 0;
        }


        // =========================================================
        // FILTER CHANGED
        // =========================================================

        protected void FilterChanged(object sender,EventArgs e)
        {
            BindQuestions();
        }


        // =========================================================
        // MY QUESTIONS TAB
        // =========================================================

        protected void btnMyQuestions_Click(object sender, EventArgs e)
        {
            QuestionMode = "mine";
            UpdateTabStyle();
            BindQuestions();
        }


        // =========================================================
        // SHARED QUESTIONS TAB
        // =========================================================

        protected void btnSharedQuestions_Click(object sender, EventArgs e)
        {
            QuestionMode = "shared";
            UpdateTabStyle();
            BindQuestions();
        }


        // =========================================================
        // UPDATE TAB APPEARANCE
        // =========================================================

        private void UpdateTabStyle()
        {
            if (QuestionMode == "mine")
            {
                btnMyQuestions.CssClass = "question-tab active";
                btnSharedQuestions.CssClass ="question-tab";
            }
            else
            {
                btnMyQuestions.CssClass = "question-tab";
                btnSharedQuestions.CssClass ="question-tab active";
            }
        }


        // =========================================================
        // QUESTION ACTIONS
        // =========================================================

        protected void rptQuestions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int questionId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "EditQuestion":
                    Response.Redirect("~/Lecturer/CreateQuestion.aspx?id=" + questionId);
                    break;

                case "ArchiveQuestion":
                    ArchiveQuestion(questionId);
                    break;

                case "DeleteQuestion":
                    DeleteQuestion(questionId);
                    break;


                case "AddToQuiz":
                    Response.Redirect("~/Lecturer/CreateQuiz.aspx?questionId=" + questionId);
                    break;
            }
        }


        // =========================================================
        // ARCHIVE QUESTION
        // =========================================================

        private void ArchiveQuestion(int questionId)
        {
            const string sql = @"UPDATE Questions SET IsArchived = 1 WHERE QuestionId = @QuestionId AND CreatedByUserId = @UserId;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@QuestionId", SqlDbType.Int).Value = questionId;
                    command.Parameters.Add("@UserId", SqlDbType.Int).Value = CurrentUserId;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            RefreshQuestionBank();
        }


        // =========================================================
        // DELETE QUESTION
        // =========================================================

        private void DeleteQuestion(int questionId)
        {
            // -----------------------------------------
            // Check if question has already been used
            // -----------------------------------------

            const string checkSql = @" SELECT COUNT(*) FROM QuizQuestions WHERE QuestionId = @QuestionId;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                int usageCount;

                using (SqlCommand checkCommand = new SqlCommand( checkSql, connection))
                {
                    checkCommand.Parameters.Add("@QuestionId", SqlDbType.Int).Value = questionId;
                    usageCount = Convert.ToInt32( checkCommand.ExecuteScalar());
                }

                // Question is already being used
                if (usageCount > 0)
                {
                    ShowMessage("This question is already used in a quiz. " + "Please archive it instead of deleting it.");
                    return;
                }


                // -----------------------------------------
                // Delete question safely
                // -----------------------------------------

                SqlTransaction transaction = connection.BeginTransaction();


                try
                {
                    const string deleteOptionsSql = @"DELETE FROM QuestionOptions WHERE QuestionId = @QuestionId;";
                    using (SqlCommand optionCommand = new SqlCommand(deleteOptionsSql, connection, transaction))
                    {
                        optionCommand.Parameters.Add("@QuestionId",SqlDbType.Int).Value = questionId;
                        optionCommand.ExecuteNonQuery();
                    }

                    const string deleteQuestionSql = @"DELETE FROM Questions WHERE QuestionId = @QuestionId AND CreatedByUserId = @UserId;";

                    using (SqlCommand questionCommand = new SqlCommand(deleteQuestionSql, connection, transaction))
                    {
                        questionCommand.Parameters.Add("@QuestionId", SqlDbType.Int).Value = questionId;
                        questionCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = CurrentUserId;
                        questionCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();

                    throw;
                }
            }


            RefreshQuestionBank();
        }


        // =========================================================
        // REFRESH PAGE DATA
        // =========================================================

        private void RefreshQuestionBank()
        {
            BindQuestionCounts();
            BindQuestions();
        }


        // =========================================================
        // HELPER METHODS
        // =========================================================

        protected bool IsMcq(object questionType)
        {
            return string.Equals(Convert.ToString(questionType),"MCQ", StringComparison.OrdinalIgnoreCase);
        }


        protected bool HasValue(object value)
        {
            return !string.IsNullOrWhiteSpace(Convert.ToString(value));
        }


        protected string GetQuestionTypeDisplay(object questionType)
        {
            string type =Convert.ToString(questionType);

            if (
                type.Equals("MCQ", StringComparison.OrdinalIgnoreCase))
            {
                return "Single Choice";
            }

            return "Subjective";
        }


        protected string GetOwnerText( object isOwner, object creatorName)
        {
            string name = Convert.ToString( creatorName);

            if (Convert.ToBoolean(isOwner))
            {
                return "Created by You ("+ name + ")";
            }

            return "Contributed by " + name;
        }


        protected string GetDifficultyCss(object difficulty)
        {
            string value = Convert.ToString(difficulty) .ToLower();

            switch (value)
            {
                case "easy":
                    return "difficulty-easy";
                case "hard":
                    return "difficulty-hard";
                default:
                    return "difficulty-medium";
            }
        }


        protected string GetSubjectCss(object subjectName)
        {
            string subject =Convert.ToString(subjectName).ToLower();

            switch (subject)
            {
                case "sejarah":
                    return "subject-yellow";
                case "english":
                    return "subject-blue";
                default:
                    return "";
            }
        }


        protected string FormatCorrectAnswer(object option,object answerText)
        {
            string optionLetter = Convert.ToString(option);
            string answer = Convert.ToString(answerText);

            if (
                string.IsNullOrEmpty(optionLetter))
            {
                return "Correct answer not set";
            }

            return "Correct Answer: Option " + optionLetter + " (" + answer + ")";
        }


        protected string GetModelAnswerStatus(object modelAnswer)
        {
            if (string.IsNullOrWhiteSpace(Convert.ToString(modelAnswer)))
            {
                return "No model answer provided";
            }

            return "Model scoring key provided";
        }


        protected string AttributeEncode(object value)
        {
            return HttpUtility.HtmlAttributeEncode(Convert.ToString(value));
        }


        protected string GetMediaUrl(object mediaPath)
        {
            string path = Convert.ToString(mediaPath);

            if (string.IsNullOrWhiteSpace(path))
            {
                return "";
            }


            if (path.StartsWith("http://",StringComparison.OrdinalIgnoreCase) || path.StartsWith("https://",StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }


            if (path.StartsWith("~/"))
            {
                return ResolveUrl(path);
            }

            return ResolveUrl("~/" +path.TrimStart('/')
            );
        }


        // =========================================================
        // MESSAGE
        // =========================================================

        private void ShowMessage(string message)
        {
            string safeMessage = HttpUtility.JavaScriptStringEncode(message);

            ClientScript.RegisterStartupScript(GetType(),Guid.NewGuid().ToString(),"alert('" + safeMessage + "');", true);
        }
    }
}