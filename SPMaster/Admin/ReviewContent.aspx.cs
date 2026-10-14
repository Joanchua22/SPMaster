using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SPMaster.Admin
{
    public partial class ReviewContent : System.Web.UI.Page
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadReview();
            }
        }


        private string GetConnectionString()
        {
            return ConfigurationManager
                .ConnectionStrings[
                    "SPMasterConnectionString"
                ]
                .ConnectionString;
        }


        private int GetQuestionId()
        {
            int questionId;

            if (!int.TryParse(
                Request.QueryString["id"],
                out questionId))
            {
                return 0;
            }

            return questionId;
        }


        private void LoadReview()
        {
            int questionId =
                GetQuestionId();


            if (questionId == 0)
            {
                Response.Redirect(
                    "~/Admin/ContentAudit.aspx"
                );

                return;
            }


            string query = @"
                SELECT TOP 1

                    Q.QuestionId,
                    Q.QuestionText,
                    Q.QuestionType,
                    Q.Difficulty,

                    S.SubjectName,

                    Lecturer.FullName
                        AS LecturerName,

                    F.FeedbackId,
                    F.Message,
                    F.Status,
                    F.CreatedAt,

                    Reporter.FullName
                        AS ReporterName

                FROM Questions Q

                INNER JOIN Subjects S
                    ON Q.SubjectId = S.SubjectId

                INNER JOIN Users Lecturer
                    ON Q.CreatedByUserId =
                       Lecturer.UserId

                INNER JOIN QuizQuestions QQ
                    ON Q.QuestionId =
                       QQ.QuestionId

                INNER JOIN Feedback F
                    ON QQ.QuizId =
                       F.QuizId

                INNER JOIN Users Reporter
                    ON F.SubmittedByUserId =
                       Reporter.UserId

                WHERE
                    Q.QuestionId = @QuestionId
                    AND F.Status = 'Open'

                ORDER BY
                    F.CreatedAt DESC
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
                    command.Parameters
                        .AddWithValue(
                            "@QuestionId",
                            questionId
                        );


                    connection.Open();


                    using (SqlDataReader reader =
                           command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblQuestion.Text =
                                reader["QuestionText"]
                                .ToString();

                            lblSubject.Text =
                                reader["SubjectName"]
                                .ToString();

                            lblLecturer.Text =
                                reader["LecturerName"]
                                .ToString();

                            lblQuestionType.Text =
                                reader["QuestionType"]
                                .ToString();

                            lblDifficulty.Text =
                                reader["Difficulty"]
                                .ToString();

                            lblReason.Text =
                                reader["Message"]
                                .ToString();

                            lblReportedBy.Text =
                                reader["ReporterName"]
                                .ToString();

                            lblStatus.Text =
                                reader["Status"]
                                .ToString();


                            DateTime reportedAt =
                                Convert.ToDateTime(
                                    reader["CreatedAt"]
                                );


                            lblReportedAt.Text =
                                reportedAt.ToString(
                                    "dd MMM yyyy, hh:mm tt"
                                );


                            ViewState["FeedbackId"] =
                                Convert.ToInt32(
                                    reader["FeedbackId"]
                                );
                        }
                        else
                        {
                            ShowError(
                                "No open report was found for this question."
                            );

                            btnResolve.Visible =
                                false;
                        }
                    }
                }
            }
        }


        protected void btnResolve_Click(
            object sender,
            EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }


            if (ViewState["FeedbackId"] == null)
            {
                ShowError(
                    "Unable to identify the report."
                );

                return;
            }


            int feedbackId =
                Convert.ToInt32(
                    ViewState["FeedbackId"]
                );


            string adminReply =
                txtAdminReply.Text.Trim();


            int adminUserId = 0;


            if (Session["UserId"] != null)
            {
                int.TryParse(
                    Session["UserId"].ToString(),
                    out adminUserId
                );
            }


            if (adminUserId == 0)
            {
                ShowError(
                    "Admin session could not be found."
                );

                return;
            }


            string query = @"
                UPDATE Feedback

                SET
                    Status = 'Resolved',
                    AdminReply = @AdminReply,
                    RepliedByUserId = @AdminUserId,
                    RepliedAt = SYSUTCDATETIME()

                WHERE FeedbackId = @FeedbackId
            ";


            using (SqlConnection connection =
                   new SqlConnection(
                       GetConnectionString()))
            {
                try
                {
                    using (SqlCommand command =
                           new SqlCommand(
                               query,
                               connection))
                    {
                        command.Parameters
                            .AddWithValue(
                                "@AdminReply",
                                adminReply
                            );

                        command.Parameters
                            .AddWithValue(
                                "@AdminUserId",
                                adminUserId
                            );

                        command.Parameters
                            .AddWithValue(
                                "@FeedbackId",
                                feedbackId
                            );


                        connection.Open();


                        int rows =
                            command.ExecuteNonQuery();


                        if (rows > 0)
                        {
                            Response.Redirect(
                                "~/Admin/ContentAudit.aspx?reviewed=true"
                            );
                        }
                        else
                        {
                            ShowError(
                                "The report could not be updated."
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError(
                        "Unable to resolve report: "
                        + ex.Message
                    );
                }
            }
        }


        private void ShowError(
            string message)
        {
            lblMessage.Text =
                message;

            lblMessage.Visible =
                true;
        }
    }
}