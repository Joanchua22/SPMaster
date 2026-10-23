using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;


namespace SPMaster.Admin
{
    public partial class ContentAudit :
        System.Web.UI.Page
    {
        // =========================================================
        // DATABASE CONNECTION
        // =========================================================

        private string ConnectionString
        {
            get
            {
                return ConfigurationManager
                    .ConnectionStrings[
                        "SPMasterConnectionString"
                    ]
                    .ConnectionString;
            }
        }



        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            pnlSuccess.Visible =
                false;


            // User must be logged in
            if (
                Session["UserId"] == null
            )
            {
                Response.Redirect(
                    "~/Login.aspx"
                );

                return;
            }


            if (!IsPostBack)
            {
                LoadSubjects();

                LoadSummary();

                LoadAuditData();



                // =============================================
                // UNPUBLISHED SUCCESS
                // =============================================

                if (
                    Request.QueryString[
                        "unpublished"
                    ] == "true"
                )
                {
                    lblSuccessMessage.Text =
                        "Quiz unpublished successfully.";


                    pnlSuccess.Visible =
                        true;
                }



                // =============================================
                // PUBLISHED SUCCESS
                // =============================================

                else if (
                    Request.QueryString[
                        "published"
                    ] == "true"
                )
                {
                    lblSuccessMessage.Text =
                        "Quiz published successfully.";


                    pnlSuccess.Visible =
                        true;
                }
            }
        }



        // =========================================================
        // LOAD SUBJECT FILTER
        // =========================================================

        private void LoadSubjects()
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


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection
                        )
                )
                {
                    connection.Open();


                    ddlSubjectFilter.DataSource =
                        command.ExecuteReader();


                    ddlSubjectFilter.DataTextField =
                        "SubjectName";


                    ddlSubjectFilter.DataValueField =
                        "SubjectId";


                    ddlSubjectFilter.DataBind();
                }
            }


            ddlSubjectFilter.Items.Insert(
                0,
                new ListItem(
                    "All Subjects",
                    ""
                )
            );
        }



        // =========================================================
        // PUBLISHED QUIZ COUNT
        // =========================================================

        private void LoadSummary()
        {
            const string sql = @"
                SELECT
                    COUNT(*)

                FROM Quizzes

                WHERE
                    IsPublished = 1

                    AND

                    IsArchived = 0;
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection
                        )
                )
                {
                    connection.Open();


                    int count =
                        Convert.ToInt32(
                            command.ExecuteScalar()
                        );


                    lblPublishedQuizzes.Text =
                        count.ToString();
                }
            }
        }



        // =========================================================
        // LOAD ALL NON-ARCHIVED QUIZZES
        //
        // Includes:
        // Published quizzes
        // Draft / unpublished quizzes
        // =========================================================

        private void LoadAuditData()
        {
            string sql = @"
                SELECT

                    z.QuizId,

                    z.Title,

                    z.CreatedAt,

                    z.IsPublished,

                    s.SubjectName,

                    u.FullName
                        AS LecturerName,

                    COUNT(
                        qq.QuestionId
                    ) AS QuestionCount


                FROM Quizzes z


                INNER JOIN Subjects s
                    ON z.SubjectId =
                       s.SubjectId


                INNER JOIN Users u
                    ON z.CreatedByUserId =
                       u.UserId


                LEFT JOIN QuizQuestions qq
                    ON z.QuizId =
                       qq.QuizId


                WHERE
                    z.IsArchived = 0
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand()
                )
                {
                    command.Connection =
                        connection;



                    // =============================================
                    // SUBJECT FILTER
                    // =============================================

                    if (
                        !string.IsNullOrEmpty(
                            ddlSubjectFilter
                                .SelectedValue
                        )
                    )
                    {
                        sql += @"

                            AND
                                z.SubjectId =
                                    @SubjectId
                        ";


                        command.Parameters.Add(
                            "@SubjectId",
                            SqlDbType.Int
                        ).Value =
                            Convert.ToInt32(
                                ddlSubjectFilter
                                    .SelectedValue
                            );
                    }



                    // =============================================
                    // SEARCH
                    // =============================================

                    string search =
                        txtSearch.Text
                            .Trim();


                    if (
                        !string.IsNullOrEmpty(
                            search
                        )
                    )
                    {
                        sql += @"

                            AND
                            (
                                z.Title
                                    LIKE @Search

                                OR

                                s.SubjectName
                                    LIKE @Search

                                OR

                                u.FullName
                                    LIKE @Search
                            )
                        ";


                        command.Parameters.Add(
                            "@Search",
                            SqlDbType.NVarChar,
                            500
                        ).Value =
                            "%"
                            + search
                            + "%";
                    }



                    // =============================================
                    // GROUPING
                    // =============================================

                    sql += @"

                        GROUP BY

                            z.QuizId,

                            z.Title,

                            z.CreatedAt,

                            z.IsPublished,

                            s.SubjectName,

                            u.FullName


                        ORDER BY

                            z.CreatedAt DESC;
                    ";


                    command.CommandText =
                        sql;


                    DataTable table =
                        new DataTable();


                    using (
                        SqlDataAdapter adapter =
                            new SqlDataAdapter(
                                command
                            )
                    )
                    {
                        adapter.Fill(
                            table
                        );
                    }


                    rptAudit.DataSource =
                        table;


                    rptAudit.DataBind();


                    pnlNoQuizzes.Visible =
                        table.Rows.Count == 0;
                }
            }
        }



        // =========================================================
        // FILTER CHANGED
        // =========================================================

        protected void FilterChanged(
            object sender,
            EventArgs e)
        {
            LoadAuditData();
        }



        // =========================================================
        // SEARCH CHANGED
        // =========================================================

        protected void SearchChanged(
            object sender,
            EventArgs e)
        {
            LoadAuditData();
        }



        // =========================================================
        // UNPUBLISH QUIZ
        //
        // Only changes IsPublished.
        //
        // QuizQuestions is NOT modified.
        // Questions remain linked to the quiz.
        // =========================================================

        protected void Unpublish_Command(
            object sender,
            CommandEventArgs e)
        {
            int quizId;


            if (
                !int.TryParse(
                    e.CommandArgument
                        .ToString(),
                    out quizId
                )
            )
            {
                return;
            }


            const string sql = @"
                UPDATE Quizzes

                SET
                    IsPublished = 0

                WHERE
                    QuizId =
                        @QuizId

                    AND

                    IsPublished = 1

                    AND

                    IsArchived = 0;
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection
                        )
                )
                {
                    command.Parameters.Add(
                        "@QuizId",
                        SqlDbType.Int
                    ).Value =
                        quizId;


                    connection.Open();


                    command.ExecuteNonQuery();
                }
            }


            Response.Redirect(
                "~/Admin/ContentAudit.aspx?unpublished=true"
            );
        }



        // =========================================================
        // PUBLISH QUIZ
        //
        // Publishes an existing unpublished quiz.
        //
        // QuizQuestions is NOT modified.
        // =========================================================

        protected void Publish_Command(
            object sender,
            CommandEventArgs e)
        {
            int quizId;


            if (
                !int.TryParse(
                    e.CommandArgument
                        .ToString(),
                    out quizId
                )
            )
            {
                return;
            }



            // =====================================================
            // MAKE SURE QUIZ HAS QUESTIONS
            // =====================================================

            const string countSql = @"
                SELECT
                    COUNT(*)

                FROM QuizQuestions

                WHERE
                    QuizId =
                        @QuizId;
            ";


            int questionCount;


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand(
                            countSql,
                            connection
                        )
                )
                {
                    command.Parameters.Add(
                        "@QuizId",
                        SqlDbType.Int
                    ).Value =
                        quizId;


                    connection.Open();


                    questionCount =
                        Convert.ToInt32(
                            command.ExecuteScalar()
                        );
                }
            }



            // Quiz cannot be published with no questions
            if (
                questionCount == 0
            )
            {
                return;
            }



            // =====================================================
            // PUBLISH
            // =====================================================

            const string sql = @"
                UPDATE Quizzes

                SET
                    IsPublished = 1

                WHERE
                    QuizId =
                        @QuizId

                    AND

                    IsPublished = 0

                    AND

                    IsArchived = 0;
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection
                        )
                )
                {
                    command.Parameters.Add(
                        "@QuizId",
                        SqlDbType.Int
                    ).Value =
                        quizId;


                    connection.Open();


                    command.ExecuteNonQuery();
                }
            }


            Response.Redirect(
                "~/Admin/ContentAudit.aspx?published=true"
            );
        }
    }
}