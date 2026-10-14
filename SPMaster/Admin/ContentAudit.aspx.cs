using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace SPMaster.Admin
{
    public partial class ContentAudit : System.Web.UI.Page
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
                {
                    pnlSuccess.Visible = false;


                    if (!IsPostBack)
                    {
                        LoadSubjects();

                        LoadSummary();

                        LoadAuditData();


                        if (
                            Request.QueryString["reviewed"]
                            == "true"
                        )
                        {
                            lblSuccessMessage.Text =
                                "Content report resolved successfully.";

                            pnlSuccess.Visible =
                                true;
                        }
                    }
                }


        private string GetConnectionString()
        {
            return
                ConfigurationManager
                .ConnectionStrings[
                    "SPMasterConnectionString"
                ]
                .ConnectionString;
        }


        private void LoadSubjects()
        {
            string query = @"
                SELECT
                    SubjectId,
                    SubjectName
                FROM Subjects
                WHERE IsActive = 1
                ORDER BY SubjectName
            ";


            using (SqlConnection connection =
                   new SqlConnection(
                       GetConnectionString()))
            {
                using (SqlCommand command =
                       new SqlCommand(
                           query,
                           connection))
                {
                    connection.Open();


                    SqlDataReader reader =
                        command.ExecuteReader();


                    ddlSubjectFilter.DataSource =
                        reader;

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


        private void LoadSummary()
        {
            using (SqlConnection connection =
                   new SqlConnection(
                       GetConnectionString()))
            {
                connection.Open();


                // TOTAL QUESTIONS
                string totalQuery = @"
                    SELECT COUNT(*)
                    FROM Questions
                    WHERE IsArchived = 0
                ";


                using (SqlCommand command =
                       new SqlCommand(
                           totalQuery,
                           connection))
                {
                    lblTotalQuestions.Text =
                        Convert.ToInt32(
                            command.ExecuteScalar()
                        ).ToString();
                }


                // FLAGGED QUESTIONS
                string flaggedQuery = @"
                    SELECT COUNT(DISTINCT Q.QuestionId)

                    FROM Questions Q

                    INNER JOIN QuizQuestions QQ
                        ON Q.QuestionId = QQ.QuestionId

                    INNER JOIN Feedback F
                        ON QQ.QuizId = F.QuizId

                    WHERE
                        Q.IsArchived = 0
                        AND F.Status = 'Open'
                ";


                using (SqlCommand command =
                       new SqlCommand(
                           flaggedQuery,
                           connection))
                {
                    lblFlaggedQuestions.Text =
                        Convert.ToInt32(
                            command.ExecuteScalar()
                        ).ToString();
                }


                // APPROVED QUESTIONS
                string approvedQuery = @"
                    SELECT COUNT(*)

                    FROM Questions Q

                    WHERE
                        Q.IsArchived = 0

                        AND NOT EXISTS
                        (
                            SELECT 1

                            FROM QuizQuestions QQ

                            INNER JOIN Feedback F
                                ON QQ.QuizId = F.QuizId

                            WHERE
                                QQ.QuestionId = Q.QuestionId
                                AND F.Status = 'Open'
                        )
                ";


                using (SqlCommand command =
                       new SqlCommand(
                           approvedQuery,
                           connection))
                {
                    lblApprovedQuestions.Text =
                        Convert.ToInt32(
                            command.ExecuteScalar()
                        ).ToString();
                }
            }
        }


        private void LoadAuditData()
        {
            string query = @"
                SELECT

                    Q.QuestionId,

                    Q.QuestionText,

                    S.SubjectName,

                    U.FullName AS LecturerName,

                    CASE

                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM QuizQuestions QQ2

                            INNER JOIN Feedback F2
                                ON QQ2.QuizId = F2.QuizId

                            WHERE
                                QQ2.QuestionId = Q.QuestionId
                                AND F2.Status = 'Open'
                        )

                        THEN 'Flagged'

                        ELSE 'Approved'

                    END AS AuditStatus,


                    COALESCE
                    (
                        (
                            SELECT TOP 1
                                F3.Message

                            FROM QuizQuestions QQ3

                            INNER JOIN Feedback F3
                                ON QQ3.QuizId = F3.QuizId

                            WHERE
                                QQ3.QuestionId = Q.QuestionId
                                AND F3.Status = 'Open'

                            ORDER BY
                                F3.CreatedAt DESC
                        ),

                        '—'

                    ) AS FlagReason


                FROM Questions Q

                INNER JOIN Subjects S
                    ON Q.SubjectId = S.SubjectId

                INNER JOIN Users U
                    ON Q.CreatedByUserId = U.UserId

                WHERE
                    Q.IsArchived = 0
            ";


            using (SqlConnection connection =
                   new SqlConnection(
                       GetConnectionString()))
            {
                using (SqlCommand command =
                       new SqlCommand())
                {
                    command.Connection =
                        connection;


                    // SUBJECT FILTER
                    if (!string.IsNullOrEmpty(
                        ddlSubjectFilter.SelectedValue))
                    {
                        query += @"
                            AND Q.SubjectId = @SubjectId
                        ";


                        command.Parameters
                            .AddWithValue(
                                "@SubjectId",
                                Convert.ToInt32(
                                    ddlSubjectFilter.SelectedValue
                                )
                            );
                    }


                    // STATUS FILTER
                    if (
                        ddlStatusFilter.SelectedValue
                        == "Flagged"
                    )
                    {
                        query += @"

                            AND EXISTS
                            (
                                SELECT 1

                                FROM QuizQuestions QQ4

                                INNER JOIN Feedback F4
                                    ON QQ4.QuizId = F4.QuizId

                                WHERE
                                    QQ4.QuestionId = Q.QuestionId
                                    AND F4.Status = 'Open'
                            )

                        ";
                    }


                    else if (
                        ddlStatusFilter.SelectedValue
                        == "Approved"
                    )
                    {
                        query += @"

                            AND NOT EXISTS
                            (
                                SELECT 1

                                FROM QuizQuestions QQ5

                                INNER JOIN Feedback F5
                                    ON QQ5.QuizId = F5.QuizId

                                WHERE
                                    QQ5.QuestionId = Q.QuestionId
                                    AND F5.Status = 'Open'
                            )

                        ";
                    }


                    // SEARCH
                    string search =
                        txtSearch.Text.Trim();


                    if (!string.IsNullOrEmpty(search))
                    {
                        query += @"

                            AND
                            (
                                Q.QuestionText LIKE @Search
                                OR
                                S.SubjectName LIKE @Search
                                OR
                                U.FullName LIKE @Search
                            )

                        ";


                        command.Parameters
                            .AddWithValue(
                                "@Search",
                                "%" + search + "%"
                            );
                    }


                    query += @"
                        ORDER BY Q.CreatedAt DESC
                    ";


                    command.CommandText =
                        query;


                    connection.Open();


                    SqlDataAdapter adapter =
                        new SqlDataAdapter(
                            command);


                    DataTable table =
                        new DataTable();


                    adapter.Fill(table);


                    rptAudit.DataSource =
                        table;

                    rptAudit.DataBind();
                }
            }
        }


        protected void FilterChanged(
            object sender,
            EventArgs e)
        {
            LoadAuditData();
        }


        protected void SearchChanged(
            object sender,
            EventArgs e)
        {
            LoadAuditData();
        }
    }
}