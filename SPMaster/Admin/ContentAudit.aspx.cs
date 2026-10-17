using System;
using System.Collections.Generic;
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


                if (
                    Request.QueryString[
                        "unpublished"
                    ] == "true"
                )
                {
                    lblSuccessMessage.Text =
                        "Question unpublished successfully.";

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
        // PUBLISHED QUESTION COUNT
        // =========================================================

        private void LoadSummary()
        {
            const string sql = @"
                SELECT
                    COUNT(
                        DISTINCT q.QuestionId
                    )

                FROM Questions q

                INNER JOIN QuizQuestions qq
                    ON q.QuestionId =
                       qq.QuestionId

                INNER JOIN Quizzes z
                    ON qq.QuizId =
                       z.QuizId

                WHERE
                    q.IsArchived = 0

                    AND
                    z.IsPublished = 1

                    AND
                    z.IsArchived = 0;
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


                    lblPublishedQuestions.Text =
                        count.ToString();
                }
            }
        }


        // =========================================================
        // LOAD PUBLISHED QUESTIONS
        // =========================================================

        private void LoadAuditData()
        {
            string sql = @"
                SELECT

                    q.QuestionId,

                    q.QuestionText,

                    q.CreatedAt,

                    s.SubjectName,

                    u.FullName
                        AS LecturerName,

                    COUNT(
                        DISTINCT z.QuizId
                    ) AS PublishedQuizCount


                FROM Questions q


                INNER JOIN Subjects s
                    ON q.SubjectId =
                       s.SubjectId


                INNER JOIN Users u
                    ON q.CreatedByUserId =
                       u.UserId


                INNER JOIN QuizQuestions qq
                    ON q.QuestionId =
                       qq.QuestionId


                INNER JOIN Quizzes z
                    ON qq.QuizId =
                       z.QuizId


                WHERE
                    q.IsArchived = 0

                    AND
                    z.IsPublished = 1

                    AND
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


                    // -----------------------------------------
                    // SUBJECT FILTER
                    // -----------------------------------------

                    if (
                        !string.IsNullOrEmpty(
                            ddlSubjectFilter
                                .SelectedValue
                        )
                    )
                    {
                        sql += @"

                            AND
                            q.SubjectId =
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


                    // -----------------------------------------
                    // SEARCH
                    // -----------------------------------------

                    string search =
                        txtSearch.Text.Trim();


                    if (
                        !string.IsNullOrEmpty(
                            search
                        )
                    )
                    {
                        sql += @"

                            AND
                            (
                                q.QuestionText
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


                    // -----------------------------------------
                    // GROUPING
                    // -----------------------------------------

                    sql += @"

                        GROUP BY

                            q.QuestionId,

                            q.QuestionText,

                            q.CreatedAt,

                            s.SubjectName,

                            u.FullName


                        ORDER BY

                            q.CreatedAt DESC;
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


                    pnlNoQuestions.Visible =
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
        // UNPUBLISH QUESTION
        //
        // Removes the question from every currently published quiz.
        //
        // The original question remains in the lecturer's
        // Question Bank.
        // =========================================================

        protected void Unpublish_Command(
            object sender,
            CommandEventArgs e)
        {
            int questionId;


            if (
                !int.TryParse(
                    e.CommandArgument.ToString(),
                    out questionId
                )
            )
            {
                return;
            }


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                connection.Open();


                SqlTransaction transaction =
                    connection.BeginTransaction();


                try
                {
                    // =========================================
                    // STEP 1
                    // Find published quizzes currently using
                    // this question.
                    // =========================================

                    List<int> affectedQuizIds =
                        new List<int>();


                    const string findSql = @"
                        SELECT DISTINCT
                            z.QuizId

                        FROM Quizzes z

                        INNER JOIN QuizQuestions qq
                            ON z.QuizId =
                               qq.QuizId

                        WHERE
                            qq.QuestionId =
                                @QuestionId

                            AND
                            z.IsPublished = 1

                            AND
                            z.IsArchived = 0;
                    ";


                    using (
                        SqlCommand command =
                            new SqlCommand(
                                findSql,
                                connection,
                                transaction
                            )
                    )
                    {
                        command.Parameters.Add(
                            "@QuestionId",
                            SqlDbType.Int
                        ).Value =
                            questionId;


                        using (
                            SqlDataReader reader =
                                command.ExecuteReader()
                        )
                        {
                            while (
                                reader.Read()
                            )
                            {
                                affectedQuizIds.Add(
                                    Convert.ToInt32(
                                        reader[
                                            "QuizId"
                                        ]
                                    )
                                );
                            }
                        }
                    }


                    if (
                        affectedQuizIds.Count == 0
                    )
                    {
                        transaction.Rollback();

                        Response.Redirect(
                            "~/Admin/ContentAudit.aspx"
                        );

                        return;
                    }


                    // =========================================
                    // STEP 2
                    // Remove question only from published
                    // quizzes.
                    // =========================================

                    const string deleteSql = @"
                        DELETE qq

                        FROM QuizQuestions qq

                        INNER JOIN Quizzes z
                            ON qq.QuizId =
                               z.QuizId

                        WHERE
                            qq.QuestionId =
                                @QuestionId

                            AND
                            z.IsPublished = 1

                            AND
                            z.IsArchived = 0;
                    ";


                    using (
                        SqlCommand command =
                            new SqlCommand(
                                deleteSql,
                                connection,
                                transaction
                            )
                    )
                    {
                        command.Parameters.Add(
                            "@QuestionId",
                            SqlDbType.Int
                        ).Value =
                            questionId;


                        command.ExecuteNonQuery();
                    }


                    // =========================================
                    // STEP 3
                    // A published quiz is not allowed to have
                    // zero questions.
                    //
                    // If removing this question makes one of
                    // the affected quizzes empty, return that
                    // quiz to Draft status.
                    // =========================================

                    foreach (
                        int quizId
                        in affectedQuizIds
                    )
                    {
                        const string countSql = @"
                            SELECT COUNT(*)

                            FROM QuizQuestions

                            WHERE
                                QuizId =
                                    @QuizId;
                        ";


                        int remainingQuestions;


                        using (
                            SqlCommand command =
                                new SqlCommand(
                                    countSql,
                                    connection,
                                    transaction
                                )
                        )
                        {
                            command.Parameters.Add(
                                "@QuizId",
                                SqlDbType.Int
                            ).Value =
                                quizId;


                            remainingQuestions =
                                Convert.ToInt32(
                                    command.ExecuteScalar()
                                );
                        }


                        if (
                            remainingQuestions == 0
                        )
                        {
                            const string draftSql = @"
                                UPDATE Quizzes

                                SET
                                    IsPublished = 0

                                WHERE
                                    QuizId =
                                        @QuizId

                                    AND
                                    IsPublished = 1;
                            ";


                            using (
                                SqlCommand command =
                                    new SqlCommand(
                                        draftSql,
                                        connection,
                                        transaction
                                    )
                            )
                            {
                                command.Parameters.Add(
                                    "@QuizId",
                                    SqlDbType.Int
                                ).Value =
                                    quizId;


                                command.ExecuteNonQuery();
                            }
                        }
                    }


                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();

                    throw;
                }
            }


            Response.Redirect(
                "~/Admin/ContentAudit.aspx?unpublished=true"
            );
        }
    }
}