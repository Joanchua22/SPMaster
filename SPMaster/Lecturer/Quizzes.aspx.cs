using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;


namespace SPMaster.Lecturer
{
    public partial class Quizzes : System.Web.UI.Page
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
        // QUIZ MODE
        // mine / public
        // =========================================================

        private string QuizMode
        {
            get
            {
                if (ViewState["QuizMode"] == null)
                {
                    return "mine";
                }

                return ViewState["QuizMode"].ToString();
            }

            set
            {
                ViewState["QuizMode"] = value;
            }
        }


        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(object sender,EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }


            if (!IsPostBack)
            {
                string requestedTab = Request.QueryString["tab"];

                if (string.Equals(requestedTab, "public", StringComparison.OrdinalIgnoreCase))
                {
                    QuizMode = "public";
                }
                else
                {
                    QuizMode = "mine";
                }

                BindSubjectFilter();
                BindFormFilter();
                BindQuizCounts();
                BindQuizzes();
            }


            UpdateTabStyle();
        }


        // =========================================================
        // QUIZ COUNTS
        // =========================================================

        private void BindQuizCounts()
        {
            const string sql = @"SELECT SUM(CASE WHEN CreatedByUserId = @UserId AND IsArchived = 0 THEN 1 ELSE 0 END) AS MyQuizCount,
                                SUM(CASE WHEN CreatedByUserId <> @UserId AND IsPublished = 1 AND IsArchived = 0 THEN 1 ELSE 0 END) AS PublicQuizCount FROM Quizzes;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@UserId",SqlDbType.Int).Value =CurrentUserId;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int myCount = reader["MyQuizCount"] == DBNull.Value ? 0: Convert.ToInt32(reader["MyQuizCount"]);
                            int publicCount = reader["PublicQuizCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PublicQuizCount"]);
                            btnMyQuizzes.Text ="My Quizzes (" + myCount + ")";
                            btnPublicLibrary.Text ="Public Quiz Library (" + publicCount +")";
                        }
                    }
                }
            }
        }


        // =========================================================
        // BIND SUBJECT FILTER
        // =========================================================

        private void BindSubjectFilter()
        {
            const string sql = @" SELECT SubjectId, SubjectName FROM Subjects WHERE IsActive = 1 ORDER BY SubjectName;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                ddlSubjectFilter.DataSource = command.ExecuteReader();
                ddlSubjectFilter.DataTextField = "SubjectName";
                ddlSubjectFilter.DataValueField = "SubjectId";
                ddlSubjectFilter.DataBind();
            }

            ddlSubjectFilter.Items.Insert(0, new ListItem("All Subjects", ""));
        }

        // =========================================================
        // BIND FORM FILTER
        // =========================================================

        private void BindFormFilter()
        {
            const string sql = @"SELECT FormId, FormName FROM Forms WHERE IsActive = 1 ORDER BY FormId;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                ddlFormFilter.DataSource = command.ExecuteReader();
                ddlFormFilter.DataTextField = "FormName";
                ddlFormFilter.DataValueField = "FormId";
                ddlFormFilter.DataBind();
            }

            ddlFormFilter.Items.Insert(0, new ListItem("All Forms", ""));
        }


        // =========================================================
        // BIND QUIZZES
        // =========================================================

        private void BindQuizzes()
        {
            const string sql = @"SELECT q.QuizId, q.Title, q.Description, q.TimeLimitMinutes, q.IsPublished, q.CreatedByUserId, q.CreatedAt, s.SubjectName, f.FormName, u.FullName AS CreatorName,
                            COUNT(qq.QuizQuestionId) AS QuestionCount, ISNULL(SUM(qq.MaxMarks), 0) AS TotalMarks, CASE WHEN q.CreatedByUserId = @CurrentUserId THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsOwner
                            FROM Quizzes q INNER JOIN Subjects s ON q.SubjectId = s.SubjectId INNER JOIN Forms f ON q.FormId = f.FormId 
                            INNER JOIN Users u ON q.CreatedByUserId = u.UserId LEFT JOIN QuizQuestions qq ON q.QuizId = qq.QuizId WHERE q.IsArchived = 0 
                            AND ((@Mode = 'mine' AND q.CreatedByUserId = @CurrentUserId)
                            OR (@Mode = 'public' AND q.CreatedByUserId <> @CurrentUserId AND q.IsPublished = 1))
                            AND (@SubjectId IS NULL OR q.SubjectId = @SubjectId) AND (@FormId IS NULL OR q.FormId = @FormId) 
                            AND (@Search = '' OR q.Title LIKE @SearchPattern OR ISNULL(q.Description, '') LIKE @SearchPattern OR s.SubjectName LIKE @SearchPattern OR f.FormName LIKE @SearchPattern OR u.FullName LIKE @SearchPattern)
                            GROUP BY q.QuizId, q.Title, q.Description, q.TimeLimitMinutes, q.IsPublished, q.CreatedByUserId, q.CreatedAt, s.SubjectName, f.FormName, u.FullName ORDER BY q.CreatedAt DESC; ";

            DataTable quizTable = new DataTable();

            string searchText = txtSearch.Text.Trim();
            int subjectId;
            bool hasSubjectFilter = int.TryParse(ddlSubjectFilter.SelectedValue, out subjectId);
            int formId;
            bool hasFormFilter = int.TryParse(ddlFormFilter.SelectedValue, out formId);

            using (SqlConnection connection = new SqlConnection(ConnectionString))

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                // =============================================
                // USER
                // =============================================

                command.Parameters.Add("@CurrentUserId", SqlDbType.Int).Value = CurrentUserId;


                // =============================================
                // TAB MODE
                // =============================================

                command.Parameters.Add("@Mode", SqlDbType.VarChar, 10).Value = QuizMode;


                // =============================================
                // SUBJECT FILTER
                // =============================================

                command.Parameters.Add("@SubjectId", SqlDbType.Int).Value = hasSubjectFilter ? (object)subjectId : DBNull.Value;


                // =============================================
                // FORM FILTER
                // =============================================

                command.Parameters.Add("@FormId", SqlDbType.Int).Value = hasFormFilter ? (object)formId : DBNull.Value;


                // =============================================
                // SEARCH
                // =============================================

                command.Parameters.Add("@Search", SqlDbType.NVarChar,200).Value = searchText;
                command.Parameters.Add("@SearchPattern", SqlDbType.NVarChar,204).Value = "%" + searchText + "%";
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(quizTable);
                }
            }

            rptQuizzes.DataSource = quizTable;
            rptQuizzes.DataBind();
            pnlNoQuizzes.Visible = quizTable.Rows.Count == 0;
        }


        // =========================================================
        // MY QUIZZES TAB
        // =========================================================

        protected void btnMyQuizzes_Click(object sender,EventArgs e)
        {
            QuizMode ="mine";
            BindQuizzes();
            UpdateTabStyle();
        }


        // =========================================================
        // PUBLIC QUIZ LIBRARY TAB
        // =========================================================

        protected void btnPublicLibrary_Click(object sender,EventArgs e)
        {
            QuizMode = "public";
            BindQuizzes();
            UpdateTabStyle();
        }


        // =========================================================
        // TAB STYLE
        // =========================================================

        private void UpdateTabStyle()
        {
            if (QuizMode == "mine")
            {
                btnMyQuizzes.CssClass = "quiz-tab active";
                btnPublicLibrary.CssClass = "quiz-tab";
            }
            else
            {
                btnMyQuizzes.CssClass ="quiz-tab";
                btnPublicLibrary.CssClass ="quiz-tab active";
            }
        }


        // =========================================================
        // QUIZ ACTIONS
        // =========================================================

        protected void rptQuizzes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int quizId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "ArchiveQuiz": 
                    ArchiveQuiz(quizId);
                    break;

                case "DeleteQuiz":
                    DeleteDraftQuiz(quizId);
                    break;
            }
        }


        // =========================================================
        // ARCHIVE QUIZ
        // =========================================================

        private void ArchiveQuiz(int quizId)
        {
            const string sql = @"UPDATE Quizzes SET IsArchived = 1 WHERE QuizId = @QuizId AND CreatedByUserId = @UserId AND IsPublished = 1;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@QuizId",SqlDbType.Int).Value = quizId;
                    command.Parameters.Add("@UserId",SqlDbType.Int).Value = CurrentUserId;
                    connection.Open();

                    int rows = command.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        ShowMessage("Unable to archive this quiz.");
                        return;
                    }
                }
            }

            RefreshQuizPage();
        }


        // =========================================================
        // DELETE DRAFT QUIZ
        // =========================================================

        private void DeleteDraftQuiz(int quizId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                // -----------------------------------------
                // Confirm this is the user's draft
                // -----------------------------------------

                const string checkSql = @"SELECT COUNT(*) FROM Quizzes WHERE QuizId = @QuizId AND CreatedByUserId = @UserId AND IsPublished = 0;";

                using (SqlCommand checkCommand = new SqlCommand(checkSql, connection))
                {
                    checkCommand.Parameters.Add("@QuizId",SqlDbType.Int).Value = quizId;
                    checkCommand.Parameters.Add("@UserId",SqlDbType.Int).Value = CurrentUserId;
                    int exists = Convert.ToInt32( checkCommand.ExecuteScalar());

                    if (exists == 0)
                    {
                        ShowMessage("Only your draft quizzes can be deleted.");
                        return;
                    }
                }


                // -----------------------------------------
                // TRANSACTION
                // -----------------------------------------

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // -------------------------------------
                    // Delete question mappings
                    // -------------------------------------

                    const string deleteQuestionsSql = @"DELETE FROM QuizQuestions WHERE QuizId = @QuizId;";

                    using (SqlCommand command = new SqlCommand(deleteQuestionsSql, connection, transaction))
                    {
                        command.Parameters.Add("@QuizId",SqlDbType.Int).Value = quizId;
                        command.ExecuteNonQuery();
                    }


                    // -------------------------------------
                    // Delete quiz
                    // -------------------------------------

                    const string deleteQuizSql = @"DELETE FROM Quizzes WHERE QuizId = @QuizId AND CreatedByUserId = @UserId AND IsPublished = 0;";

                    using (SqlCommand command = new SqlCommand( deleteQuizSql, connection, transaction))
                    {
                        command.Parameters.Add("@QuizId", SqlDbType.Int).Value = quizId;
                        command.Parameters.Add("@UserId",SqlDbType.Int).Value = CurrentUserId;
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ShowMessage("Unable to delete quiz: " + ex.Message);
                    return;
                }
            }

            RefreshQuizPage();
        }


        // =========================================================
        // REFRESH
        // =========================================================

        private void RefreshQuizPage()
        {
            BindQuizCounts();
            BindQuizzes();
            UpdateTabStyle();
        }


        // =========================================================
        // STATUS
        // =========================================================

        protected string GetStatusText(object isPublished)
        {
            return Convert.ToBoolean(isPublished) ? "Published" : "Draft";
        }


        protected string GetStatusCss(object isPublished)
        {
            return Convert.ToBoolean(isPublished) ? "status-published" : "status-draft";
        }


        // =========================================================
        // SUBJECT CSS
        // =========================================================

        protected string GetSubjectCss(object subjectName)
        {
            string subject = Convert.ToString(subjectName).Trim().ToLower();


            switch (subject)
            {
                case "mathematics":
                    return "subject-purple";

                case "sejarah":
                    return "subject-yellow";

                case "bahasa melayu":
                    return "subject-blue";

                case "english":
                    return "subject-purple";

                case "science":
                    return "subject-blue";

                default:
                    return "subject-purple";
            }
        }


        // =========================================================
        // DESCRIPTION
        // =========================================================

        protected string GetDescription(object description)
        {
            string value = Convert.ToString(description);

            if (string.IsNullOrWhiteSpace(value))
            {
                return "No description provided.";
            }

            return value;
        }


        // =========================================================
        // MESSAGE
        // =========================================================

        private void ShowMessage(string message)
        {
            string safeMessage = HttpUtility.JavaScriptStringEncode(message);
            ClientScript.RegisterStartupScript(GetType(), Guid.NewGuid().ToString(),"alert('" + safeMessage + "');", true);
        }

        protected void FilterChanged(object sender,EventArgs e)
        {
            BindQuizzes();
        }
    }
}