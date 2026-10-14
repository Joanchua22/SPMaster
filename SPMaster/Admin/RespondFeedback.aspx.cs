using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SPMaster.Admin
{
    public partial class RespondFeedback : System.Web.UI.Page
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadFeedback();
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


        private int GetFeedbackId()
        {
            int feedbackId;


            if (!int.TryParse(
                Request.QueryString["id"],
                out feedbackId
            ))
            {
                return 0;
            }


            return feedbackId;
        }


        private void LoadFeedback()
        {
            int feedbackId =
                GetFeedbackId();


            if (feedbackId == 0)
            {
                Response.Redirect(
                    "~/Admin/FeedbackReports.aspx"
                );

                return;
            }


            string query = @"
                SELECT

                    F.FeedbackId,
                    F.Message,
                    F.Status,
                    F.CreatedAt,
                    F.AdminReply,
                    F.RepliedAt,

                    U.FullName
                        AS SubmittedByName,

                    U.RoleId,

                    R.RoleName,

                    Q.Title
                        AS QuizTitle,

                    ReplyUser.FullName
                        AS RepliedByName

                FROM Feedback F

                INNER JOIN Users U
                    ON F.SubmittedByUserId =
                       U.UserId

                INNER JOIN Roles R
                    ON U.RoleId =
                       R.RoleId

                LEFT JOIN Quizzes Q
                    ON F.QuizId =
                       Q.QuizId

                LEFT JOIN Users ReplyUser
                    ON F.RepliedByUserId =
                       ReplyUser.UserId

                WHERE
                    F.FeedbackId =
                    @FeedbackId
            ";


            using (
                SqlConnection connection =
                new SqlConnection(
                    GetConnectionString()
                )
            )
            {
                using (
                    SqlCommand command =
                    new SqlCommand(
                        query,
                        connection
                    )
                )
                {
                    command.Parameters.AddWithValue(
                        "@FeedbackId",
                        feedbackId
                    );


                    connection.Open();


                    using (
                        SqlDataReader reader =
                        command.ExecuteReader()
                    )
                    {
                        if (!reader.Read())
                        {
                            ShowError(
                                "Feedback could not be found."
                            );

                            btnSubmitResponse.Visible =
                                false;

                            return;
                        }


                        lblSubmittedBy.Text =
                            reader[
                                "SubmittedByName"
                            ].ToString();


                        lblRole.Text =
                            reader[
                                "RoleName"
                            ].ToString();


                        lblFeedbackMessage.Text =
                            reader[
                                "Message"
                            ].ToString();


                        string status =
                            reader[
                                "Status"
                            ].ToString();


                        lblStatus.Text =
                            status;


                        ListItemSelection(
                            ddlStatus,
                            status
                        );


                        DateTime createdAt =
                            Convert.ToDateTime(
                                reader[
                                    "CreatedAt"
                                ]
                            );


                        lblCreatedAt.Text =
                            createdAt.ToString(
                                "dd MMM yyyy, hh:mm tt"
                            );


                        if (
                            reader["QuizTitle"]
                            == DBNull.Value
                        )
                        {
                            lblQuiz.Text =
                                "General Feedback";
                        }
                        else
                        {
                            lblQuiz.Text =
                                reader[
                                    "QuizTitle"
                                ].ToString();
                        }


                        if (
                            reader["AdminReply"]
                            != DBNull.Value
                            &&
                            !string.IsNullOrWhiteSpace(
                                reader[
                                    "AdminReply"
                                ].ToString()
                            )
                        )
                        {
                            pnlExistingReply.Visible =
                                true;


                            lblExistingReply.Text =
                                reader[
                                    "AdminReply"
                                ].ToString();


                            txtAdminReply.Text =
                                reader[
                                    "AdminReply"
                                ].ToString();


                            if (
                                reader[
                                    "RepliedByName"
                                ]
                                != DBNull.Value
                            )
                            {
                                lblRepliedBy.Text =
                                    reader[
                                        "RepliedByName"
                                    ].ToString();
                            }
                            else
                            {
                                lblRepliedBy.Text =
                                    "Admin";
                            }


                            if (
                                reader["RepliedAt"]
                                != DBNull.Value
                            )
                            {
                                DateTime repliedAt =
                                    Convert.ToDateTime(
                                        reader[
                                            "RepliedAt"
                                        ]
                                    );


                                lblRepliedAt.Text =
                                    repliedAt.ToString(
                                        "dd MMM yyyy, hh:mm tt"
                                    );
                            }
                            else
                            {
                                lblRepliedAt.Text =
                                    "—";
                            }
                        }
                    }
                }
            }
        }


        private void ListItemSelection(
            System.Web.UI.WebControls.DropDownList dropdown,
            string value)
        {
            System.Web.UI.WebControls.ListItem item =
                dropdown.Items.FindByValue(
                    value
                );


            if (item != null)
            {
                dropdown.ClearSelection();

                item.Selected = true;
            }
        }


        protected void btnSubmitResponse_Click(
            object sender,
            EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }


            int feedbackId =
                GetFeedbackId();


            if (feedbackId == 0)
            {
                ShowError(
                    "Invalid feedback."
                );

                return;
            }


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
                    "Admin session could not be found. Please log in again."
                );

                return;
            }


            string adminReply =
                txtAdminReply.Text.Trim();


            string status =
                ddlStatus.SelectedValue;


            string query = @"
                UPDATE Feedback

                SET
                    AdminReply = @AdminReply,
                    Status = @Status,
                    RepliedByUserId = @AdminUserId,
                    RepliedAt = SYSUTCDATETIME()

                WHERE
                    FeedbackId = @FeedbackId
            ";


            using (
                SqlConnection connection =
                new SqlConnection(
                    GetConnectionString()
                )
            )
            {
                try
                {
                    using (
                        SqlCommand command =
                        new SqlCommand(
                            query,
                            connection
                        )
                    )
                    {
                        command.Parameters.AddWithValue(
                            "@AdminReply",
                            adminReply
                        );

                        command.Parameters.AddWithValue(
                            "@Status",
                            status
                        );

                        command.Parameters.AddWithValue(
                            "@AdminUserId",
                            adminUserId
                        );

                        command.Parameters.AddWithValue(
                            "@FeedbackId",
                            feedbackId
                        );


                        connection.Open();


                        int rowsAffected =
                            command.ExecuteNonQuery();


                        if (rowsAffected > 0)
                        {
                            string view =
                                GetFeedbackView(
                                    feedbackId
                                );


                            Response.Redirect(
                                "~/Admin/FeedbackReports.aspx"
                                + "?view="
                                + view
                                + "&responded=true"
                            );
                        }
                        else
                        {
                            ShowError(
                                "The feedback could not be updated."
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError(
                        "Unable to submit response: "
                        + ex.Message
                    );
                }
            }
        }


        private string GetFeedbackView(
            int feedbackId)
        {
            string query = @"
                SELECT U.RoleId

                FROM Feedback F

                INNER JOIN Users U
                    ON F.SubmittedByUserId =
                       U.UserId

                WHERE
                    F.FeedbackId =
                    @FeedbackId
            ";


            using (
                SqlConnection connection =
                new SqlConnection(
                    GetConnectionString()
                )
            )
            {
                using (
                    SqlCommand command =
                    new SqlCommand(
                        query,
                        connection
                    )
                )
                {
                    command.Parameters.AddWithValue(
                        "@FeedbackId",
                        feedbackId
                    );


                    connection.Open();


                    object result =
                        command.ExecuteScalar();


                    if (
                        result != null
                        &&
                        Convert.ToInt32(result)
                        == 2
                    )
                    {
                        return "lecturer";
                    }
                }
            }


            return "student";
        }


        private void ShowError(
            string message)
        {
            lblMessage.Text =
                message;

            lblMessage.CssClass =
                "admin-form-message error-message";

            lblMessage.Visible =
                true;
        }
    }
}