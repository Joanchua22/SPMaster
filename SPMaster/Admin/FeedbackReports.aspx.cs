using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace SPMaster.Admin
{
    public partial class FeedbackReports : System.Web.UI.Page
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            pnlSuccess.Visible = false;


            if (!IsPostBack)
            {
                SetActiveTab();

                LoadFeedback();

                LoadStatistics();


                if (
                    Request.QueryString["responded"]
                    == "true"
                )
                {
                    lblSuccessMessage.Text =
                        "Response submitted successfully.";

                    pnlSuccess.Visible =
                        true;
                }
                else if (
                    Request.QueryString["statusupdated"]
                    == "true"
                )
                {
                    lblSuccessMessage.Text =
                        "Feedback status updated successfully.";

                    pnlSuccess.Visible =
                        true;
                }
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


        private string GetCurrentView()
        {
            string view =
                Request.QueryString["view"];


            if (
                string.Equals(
                    view,
                    "lecturer",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return "lecturer";
            }


            return "student";
        }


        private void SetActiveTab()
        {
            string currentView =
                GetCurrentView();


            if (currentView == "lecturer")
            {
                lnkStudentTab.CssClass =
                    "feedback-tab";

                lnkLecturerTab.CssClass =
                    "feedback-tab active";
            }
            else
            {
                lnkStudentTab.CssClass =
                    "feedback-tab active";

                lnkLecturerTab.CssClass =
                    "feedback-tab";
            }
        }


        private void LoadFeedback()
        {
            string currentView =
                GetCurrentView();


            int roleId =
                currentView == "lecturer"
                ? 2
                : 1;


            string query = @"
                SELECT
                    F.FeedbackId,
                    F.SubmittedByUserId,
                    F.QuizId,
                    F.Message,
                    F.Status,
                    F.CreatedAt,
                    F.AdminReply,
                    F.RepliedAt,

                    U.FullName
                        AS SubmittedByName,

                    R.RoleName,

                    Q.Title
                        AS QuizTitle

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

                WHERE
                    U.RoleId = @RoleId

                ORDER BY
                    F.CreatedAt DESC
            ";


            DataTable table =
                new DataTable();


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
                        "@RoleId",
                        roleId
                    );


                    connection.Open();


                    SqlDataAdapter adapter =
                        new SqlDataAdapter(
                            command
                        );


                    adapter.Fill(table);
                }
            }


            table.Columns.Add(
                "CreatedDisplay",
                typeof(string)
            );


            foreach (
                DataRow row
                in table.Rows
            )
            {
                DateTime createdAt =
                    Convert.ToDateTime(
                        row["CreatedAt"]
                    );


                TimeSpan difference =
                    DateTime.UtcNow -
                    createdAt;


                string display;


                if (difference.TotalMinutes < 1)
                {
                    display =
                        "Just now";
                }
                else if (difference.TotalHours < 1)
                {
                    display =
                        ((int)difference.TotalMinutes)
                        + " minutes ago";
                }
                else if (difference.TotalDays < 1)
                {
                    display =
                        ((int)difference.TotalHours)
                        + " hours ago";
                }
                else if (difference.TotalDays < 7)
                {
                    int days =
                        (int)difference.TotalDays;


                    display =
                        days
                        + (days == 1
                            ? " day ago"
                            : " days ago");
                }
                else
                {
                    display =
                        createdAt.ToString(
                            "dd MMM yyyy"
                        );
                }


                row["CreatedDisplay"] =
                    display;
            }


            rptFeedback.DataSource =
                table;

            rptFeedback.DataBind();


            /*
             * Set each status dropdown
             * after repeater binding.
             */
            for (
                int i = 0;
                i < rptFeedback.Items.Count;
                i++
            )
            {
                RepeaterItem item =
                    rptFeedback.Items[i];


                DropDownList ddlStatus =
                    (DropDownList)
                    item.FindControl(
                        "ddlFeedbackStatus"
                    );


                if (ddlStatus != null)
                {
                    string status =
                        table.Rows[i]["Status"]
                        .ToString();


                    ListItem statusItem =
                        ddlStatus.Items
                        .FindByValue(status);


                    if (statusItem != null)
                    {
                        ddlStatus.SelectedValue =
                            status;
                    }
                }
            }


            lblSubmissionCount.Text =
                table.Rows.Count.ToString();


            pnlEmpty.Visible =
                table.Rows.Count == 0;
        }


        protected void FeedbackStatusChanged(
            object sender,
            EventArgs e)
        {
            DropDownList ddlStatus =
                sender as DropDownList;


            if (ddlStatus == null)
            {
                return;
            }


            RepeaterItem item =
                ddlStatus.NamingContainer
                as RepeaterItem;


            if (item == null)
            {
                return;
            }


            HiddenField hfFeedbackId =
                item.FindControl(
                    "hfFeedbackId"
                ) as HiddenField;


            if (hfFeedbackId == null)
            {
                return;
            }


            int feedbackId;


            if (!int.TryParse(
                hfFeedbackId.Value,
                out feedbackId
            ))
            {
                return;
            }


            string status =
                ddlStatus.SelectedValue;


            string query = @"
                UPDATE Feedback

                SET Status = @Status

                WHERE FeedbackId =
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
                        "@Status",
                        status
                    );

                    command.Parameters.AddWithValue(
                        "@FeedbackId",
                        feedbackId
                    );


                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }


            string currentView =
                GetCurrentView();


            Response.Redirect(
                "~/Admin/FeedbackReports.aspx"
                + "?view="
                + currentView
                + "&statusupdated=true"
            );
        }


        private void LoadStatistics()
        {
            string currentView =
                GetCurrentView();


            int roleId =
                currentView == "lecturer"
                ? 2
                : 1;


            string query = @"
                SELECT

                    COUNT(*)
                        AS TotalCount,

                    SUM(
                        CASE
                            WHEN F.Status = 'Resolved'
                            THEN 1
                            ELSE 0
                        END
                    )
                        AS ResolvedCount,

                    AVG(
                        CASE

                            WHEN F.RepliedAt IS NOT NULL

                            THEN
                                CAST(
                                    DATEDIFF(
                                        MINUTE,
                                        F.CreatedAt,
                                        F.RepliedAt
                                    )
                                    AS FLOAT
                                )

                            ELSE NULL

                        END
                    )
                        AS AverageMinutes

                FROM Feedback F

                INNER JOIN Users U
                    ON F.SubmittedByUserId =
                       U.UserId

                WHERE
                    U.RoleId = @RoleId
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
                        "@RoleId",
                        roleId
                    );


                    connection.Open();


                    using (
                        SqlDataReader reader =
                        command.ExecuteReader()
                    )
                    {
                        if (reader.Read())
                        {
                            int total =
                                reader["TotalCount"]
                                == DBNull.Value
                                ? 0
                                : Convert.ToInt32(
                                    reader["TotalCount"]
                                );


                            int resolved =
                                reader["ResolvedCount"]
                                == DBNull.Value
                                ? 0
                                : Convert.ToInt32(
                                    reader["ResolvedCount"]
                                );


                            if (total == 0)
                            {
                                lblResolvedRate.Text =
                                    "0%";
                            }
                            else
                            {
                                double percentage =
                                    (
                                        (double)resolved /
                                        total
                                    )
                                    * 100;


                                lblResolvedRate.Text =
                                    Math.Round(
                                        percentage
                                    )
                                    + "%";
                            }


                            if (
                                reader["AverageMinutes"]
                                == DBNull.Value
                            )
                            {
                                lblAverageResponseTime.Text =
                                    "—";
                            }
                            else
                            {
                                double minutes =
                                    Convert.ToDouble(
                                        reader[
                                            "AverageMinutes"
                                        ]
                                    );


                                if (minutes < 60)
                                {
                                    lblAverageResponseTime.Text =
                                        Math.Round(minutes)
                                        + " mins";
                                }
                                else if (minutes < 1440)
                                {
                                    lblAverageResponseTime.Text =
                                        Math.Round(
                                            minutes / 60,
                                            1
                                        )
                                        + " hours";
                                }
                                else
                                {
                                    lblAverageResponseTime.Text =
                                        Math.Round(
                                            minutes / 1440,
                                            1
                                        )
                                        + " days";
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}