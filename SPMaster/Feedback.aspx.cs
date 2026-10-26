using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace SPMaster
{
    public partial class Feedback : System.Web.UI.Page
    {
        // =========================================================
        // CONNECTION STRING
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
        // CURRENT USER
        // =========================================================

        private int CurrentUserId
        {
            get
            {
                return Convert.ToInt32(
                    Session["UserId"]
                );
            }
        }


        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect(
                    "~/Login.aspx"
                );

                return;
            }


            if (!IsPostBack)
            {
                hfOpenFeedbackModal.Value = "0";

                pnlSubmitError.Visible = false;

                LoadQuizDropdown();

                LoadFeedback();


                if (
                    Request.QueryString["submitted"]
                    == "1"
                )
                {
                    pnlFeedbackMessage.Visible = true;

                    lblFeedbackMessage.Text =
                        "Feedback submitted successfully.";
                }
            }
        }

        // =========================================================
        // LOAD / SEARCH QUIZZES INTO POPUP DROPDOWN
        // =========================================================
        private void LoadQuizDropdown(
            string search = "")
        {
            const string sql = @"
        SELECT
            QuizId,
            Title

        FROM Quizzes

        WHERE
            IsPublished = 1
            AND IsArchived = 0

            AND
            (
                @Search = ''
                OR Title LIKE '%' + @Search + '%'
            )

        ORDER BY Title;
    ";


            DataTable table =
                new DataTable();


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            using (
                SqlCommand command =
                    new SqlCommand(
                        sql,
                        connection
                    )
            )
            {
                command.Parameters.Add(
                    "@Search",
                    SqlDbType.NVarChar,
                    200
                ).Value =
                    search.Trim();


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
            }


            ddlRelatedQuiz.DataSource =
                table;

            ddlRelatedQuiz.DataTextField =
                "Title";

            ddlRelatedQuiz.DataValueField =
                "QuizId";

            ddlRelatedQuiz.DataBind();


            ddlRelatedQuiz.Items.Insert(
                0,
                new ListItem(
                    "General Feedback / No Related Quiz",
                    "GENERAL"
                )
            );

            ddlRelatedQuiz.Items.Insert(
                0,
                new ListItem(
                    "Select feedback type / related quiz",
                    ""
                )
            );
        }
        // =========================================================
        // SEARCH QUIZ
        // =========================================================

        protected void btnSearchQuiz_Click(
            object sender,
            EventArgs e)
        {
            hfOpenFeedbackModal.Value =
                "1";


            LoadQuizDropdown(
                txtQuizSearch.Text.Trim()
            );
        }


        // =========================================================
        // CLEAR QUIZ SEARCH
        // =========================================================

        protected void btnClearQuizSearch_Click(
            object sender,
            EventArgs e)
        {
            hfOpenFeedbackModal.Value =
                "1";


            txtQuizSearch.Text =
                "";


            LoadQuizDropdown();
        }
        // =========================================================
        // LOAD CURRENT USER'S FEEDBACK
        // =========================================================

        private void LoadFeedback()
        {
            const string sql = @"
                SELECT
                    F.FeedbackId,
                    F.QuizId,
                    F.Message,
                    F.Status,
                    F.CreatedAt,
                    F.AdminReply,
                    F.RepliedAt,

                    Q.Title AS QuizTitle,

                    ReplyUser.FullName
                        AS RepliedByName

                FROM Feedback F

                LEFT JOIN Quizzes Q
                    ON F.QuizId =
                       Q.QuizId

                LEFT JOIN Users ReplyUser
                    ON F.RepliedByUserId =
                       ReplyUser.UserId

                WHERE
                    F.SubmittedByUserId =
                    @UserId

                ORDER BY
                    F.CreatedAt DESC;
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            using (
                SqlCommand command =
                    new SqlCommand(
                        sql,
                        connection
                    )
            )
            {
                command.Parameters.Add(
                    "@UserId",
                    SqlDbType.Int
                ).Value =
                    CurrentUserId;


                SqlDataAdapter adapter =
                    new SqlDataAdapter(
                        command
                    );


                DataTable table =
                    new DataTable();


                adapter.Fill(
                    table
                );


                rptFeedback.DataSource =
                    table;

                rptFeedback.DataBind();


                pnlNoFeedback.Visible =
                    table.Rows.Count == 0;
            }
        }


        // =========================================================
        // SUBMIT FEEDBACK
        // =========================================================

        protected void btnSubmitFeedback_Click(
            object sender,
            EventArgs e)
        {
            Page.Validate(
                "SubmitFeedback"
            );


            hfOpenFeedbackModal.Value =
                "1";


            if (!Page.IsValid)
            {
                return;
            }


            string message =
                txtFeedbackMessage
                    .Text
                    .Trim();


            if (
                string.IsNullOrWhiteSpace(
                    message
                )
            )
            {
                ShowSubmitError(
                    "Please enter your feedback."
                );

                return;
            }
            object quizId =
                DBNull.Value;


            if (
                ddlRelatedQuiz.SelectedValue
                != "GENERAL"
            )
            {
                quizId =
                    Convert.ToInt32(
                        ddlRelatedQuiz.SelectedValue
                    );
            }
            const string sql = @"
                INSERT INTO Feedback
                (
                    SubmittedByUserId,
                    QuizId,
                    Message,
                    Status,
                    CreatedAt,
                    AdminReply,
                    RepliedByUserId,
                    RepliedAt
                )

                VALUES
                (
                    @SubmittedByUserId,
                    @QuizId,
                    @Message,
                    'Open',
                    GETDATE(),
                    NULL,
                    NULL,
                    NULL
                );
            ";


            try
            {
                using (
                    SqlConnection connection =
                        new SqlConnection(
                            ConnectionString
                        )
                )
                using (
                    SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection
                        )
                )
                {
                    command.Parameters.Add(
                        "@SubmittedByUserId",
                        SqlDbType.Int
                    ).Value =
                        CurrentUserId;


                    command.Parameters.Add(
                        "@QuizId",
                        SqlDbType.Int
                    ).Value =
                        quizId;


                    command.Parameters.Add(
                        "@Message",
                        SqlDbType.NVarChar,
                        -1
                    ).Value =
                        message;


                    connection.Open();

                    command.ExecuteNonQuery();
                }


                Response.Redirect(
                    "~/Feedback.aspx?submitted=1"
                );
            }
            catch (Exception ex)
            {
                ShowSubmitError(
                    "Unable to submit feedback: "
                    + ex.Message
                );
            }
        }


        // =========================================================
        // SHOW POPUP ERROR
        // =========================================================

        private void ShowSubmitError(
            string message)
        {
            pnlSubmitError.Visible = true;

            lblSubmitError.Text =
                message;

            hfOpenFeedbackModal.Value =
                "1";
        }
    }
}