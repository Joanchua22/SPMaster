using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace SPMaster.Lecturer
{
    public partial class Grading : System.Web.UI.Page
    {
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

        private int CurrentUserId
        {
            get
            {
                return Convert.ToInt32(
                    Session["UserId"]
                );
            }
        }

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
                LoadQuizFilter();

                BindGradingQueue();

                BindReviewHistory();

                if (
                    Request.QueryString["published"]
                    == "1"
                )
                {
                    ShowMessage(
                        "Result published successfully."
                    );
                }
            }
        }

        private void LoadQuizFilter()
        {
            const string sql = @"
                SELECT DISTINCT
                    Q.QuizId,
                    Q.Title

                FROM QuizAttempts QA

                INNER JOIN Quizzes Q
                    ON QA.QuizId =
                       Q.QuizId

                LEFT JOIN QuizSessions QS
                    ON QA.SessionId =
                       QS.SessionId

                WHERE
                    QA.SubmittedAt
                        IS NOT NULL

                    AND
                    (
                        Q.CreatedByUserId =
                            @LecturerId

                        OR

                        QS.LecturerUserId =
                            @LecturerId
                    )

                ORDER BY
                    Q.Title;
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
                    "@LecturerId",
                    SqlDbType.Int
                ).Value =
                    CurrentUserId;

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

            ddlQuizFilter.DataSource =
                table;

            ddlQuizFilter.DataTextField =
                "Title";

            ddlQuizFilter.DataValueField =
                "QuizId";

            ddlQuizFilter.DataBind();

            ddlQuizFilter.Items.Insert(
                0,
                new ListItem(
                    "All Quizzes",
                    ""
                )
            );
        }

        protected void ddlQuizFilter_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            BindGradingQueue();

            BindReviewHistory();
        }

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            BindGradingQueue();

            BindReviewHistory();
        }

        protected void btnClearSearch_Click(
            object sender,
            EventArgs e)
        {
            txtSearch.Text =
                "";

            ddlQuizFilter.SelectedIndex =
                0;

            BindGradingQueue();

            BindReviewHistory();
        }

        private int? GetSelectedQuizId()
        {
            int quizId;

            if (
                int.TryParse(
                    ddlQuizFilter.SelectedValue,
                    out quizId
                )
            )
            {
                return quizId;
            }

            return null;
        }

        private string GetSearchText()
        {
            return txtSearch.Text
                .Trim();
        }

        private void BindGradingQueue()
        {
            int? selectedQuizId =
                GetSelectedQuizId();

            string search =
                GetSearchText();

            const string sql = @"
                SELECT
                    QA.AttemptId,
                    QA.QuizId,
                    QA.SessionId,
                    QA.SubmittedAt,

                    Student.FullName
                        AS StudentName,

                    Q.Title
                        AS QuizTitle,

                    QS.SessionTitle,

                    SUM(
                        CASE

                            WHEN
                                UPPER(
                                    LTRIM(
                                        RTRIM(
                                            Question.QuestionType
                                        )
                                    )
                                ) <> 'MCQ'

                            THEN 1

                            ELSE 0

                        END
                    ) AS SubjectiveCount

                FROM QuizAttempts QA

                INNER JOIN Quizzes Q
                    ON QA.QuizId =
                       Q.QuizId

                INNER JOIN Users Student
                    ON QA.StudentUserId =
                       Student.UserId

                INNER JOIN QuizQuestions QQ
                    ON QA.QuizId =
                       QQ.QuizId

                INNER JOIN Questions Question
                    ON QQ.QuestionId =
                       Question.QuestionId

                LEFT JOIN QuizSessions QS
                    ON QA.SessionId =
                       QS.SessionId

                WHERE
                    QA.AttemptStatus =
                        'PendingGrading'

                    AND
                    (
                        Q.CreatedByUserId =
                            @LecturerId

                        OR

                        QS.LecturerUserId =
                            @LecturerId
                    )

                    AND
                    (
                        @QuizId IS NULL

                        OR

                        QA.QuizId =
                            @QuizId
                    )

                    AND
                    (
                        @Search = ''

                        OR

                        Student.FullName
                            LIKE '%' + @Search + '%'

                        OR

                        Q.Title
                            LIKE '%' + @Search + '%'

                        OR

                        ISNULL(
                            QS.SessionTitle,
                            ''
                        )
                            LIKE '%' + @Search + '%'
                    )

                GROUP BY
                    QA.AttemptId,
                    QA.QuizId,
                    QA.SessionId,
                    QA.SubmittedAt,
                    Student.FullName,
                    Q.Title,
                    QS.SessionTitle

                HAVING
                    SUM(
                        CASE

                            WHEN
                                UPPER(
                                    LTRIM(
                                        RTRIM(
                                            Question.QuestionType
                                        )
                                    )
                                ) <> 'MCQ'

                            THEN 1

                            ELSE 0

                        END
                    ) > 0

                ORDER BY
                    QA.SubmittedAt ASC;
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
                    "@LecturerId",
                    SqlDbType.Int
                ).Value =
                    CurrentUserId;

                command.Parameters.Add(
                    "@QuizId",
                    SqlDbType.Int
                ).Value =
                    selectedQuizId.HasValue

                    ? (object)
                        selectedQuizId.Value

                    : DBNull.Value;

                command.Parameters.Add(
                    "@Search",
                    SqlDbType.NVarChar,
                    300
                ).Value =
                    search;

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

            table.Columns.Add(
                "StudentInitials",
                typeof(string)
            );

            table.Columns.Add(
                "HasSession",
                typeof(bool)
            );

            table.Columns.Add(
                "SubjectiveCountText",
                typeof(string)
            );

            table.Columns.Add(
                "SubmittedAtText",
                typeof(string)
            );

            table.Columns.Add(
                "GradeUrl",
                typeof(string)
            );

            foreach (
                DataRow row
                in table.Rows
            )
            {
                string studentName =
                    row["StudentName"]
                    .ToString();

                row["StudentInitials"] =
                    GetInitials(
                        studentName
                    );

                row["StudentName"] =
                    Server.HtmlEncode(
                        studentName
                    );

                row["QuizTitle"] =
                    Server.HtmlEncode(
                        row["QuizTitle"]
                        .ToString()
                    );

                bool hasSession =
                    row["SessionId"]
                    != DBNull.Value;

                row["HasSession"] =
                    hasSession;

                if (hasSession)
                {
                    row["SessionTitle"] =
                        Server.HtmlEncode(
                            row["SessionTitle"]
                            .ToString()
                        );
                }

                int subjectiveCount =
                    Convert.ToInt32(
                        row[
                            "SubjectiveCount"
                        ]
                    );

                row[
                    "SubjectiveCountText"
                ] =
                    subjectiveCount
                    + (
                        subjectiveCount == 1
                            ? " Subjective Item"
                            : " Subjective Items"
                    );

                DateTime submittedAt =
                    Convert.ToDateTime(
                        row[
                            "SubmittedAt"
                        ]
                    );

                row["SubmittedAtText"] =
                    submittedAt.ToString(
                        "dd MMM yyyy, hh:mm tt"
                    );

                row["GradeUrl"] =
                    ResolveUrl(
                        "~/Lecturer/GradingDetails.aspx?attemptId="
                        + row["AttemptId"]
                    );
            }

            rptGradingQueue.DataSource =
                table;

            rptGradingQueue.DataBind();

            pnlNoPending.Visible =
                table.Rows.Count == 0;

            pnlQueueList.Visible =
                table.Rows.Count > 0;

            if (table.Rows.Count == 0)
            {
                bool filtered =
                    selectedQuizId.HasValue
                    ||
                    !string.IsNullOrWhiteSpace(
                        search
                    );

                lblNoPendingTitle.Text =
                    filtered
                    ? "No matching submissions"
                    : "You're all caught up!";

                lblNoPendingDescription.Text =
                    filtered
                    ? "No pending grading matches your current search or quiz filter."
                    : "There are currently no subjective submissions waiting for grading.";
            }
        }

        private void BindReviewHistory()
        {
            int? selectedQuizId =
                GetSelectedQuizId();

            string search =
                GetSearchText();

            const string sql = @"
                SELECT
                    QA.AttemptId,
                    QA.QuizId,
                    QA.SessionId,
                    QA.SubmittedAt,

                    Student.FullName
                        AS StudentName,

                    Q.Title
                        AS QuizTitle,

                    QS.SessionTitle,

                    (
                        SELECT
                            MAX(
                                SAHistory.GradedAt
                            )

                        FROM StudentAnswers SAHistory

                        WHERE
                            SAHistory.AttemptId =
                                QA.AttemptId

                            AND SAHistory.GradedByUserId =
                                @LecturerId
                    ) AS GradedAt,

                    (
                        SELECT
                            ISNULL(
                                SUM(
                                    ISNULL(
                                        SAScore.MarksAwarded,
                                        0
                                    )
                                ),
                                0
                            )

                        FROM QuizQuestions QQScore

                        LEFT JOIN StudentAnswers SAScore
                            ON SAScore.AttemptId =
                               QA.AttemptId

                            AND SAScore.QuizQuestionId =
                               QQScore.QuizQuestionId

                        WHERE
                            QQScore.QuizId =
                                QA.QuizId
                    ) AS AwardedMarks,

                    (
                        SELECT
                            ISNULL(
                                SUM(
                                    QQTotal.MaxMarks
                                ),
                                0
                            )

                        FROM QuizQuestions QQTotal

                        WHERE
                            QQTotal.QuizId =
                                QA.QuizId
                    ) AS TotalMarks

                FROM QuizAttempts QA

                INNER JOIN Quizzes Q
                    ON QA.QuizId =
                       Q.QuizId

                INNER JOIN Users Student
                    ON QA.StudentUserId =
                       Student.UserId

                LEFT JOIN QuizSessions QS
                    ON QA.SessionId =
                       QS.SessionId

                WHERE
                    QA.AttemptStatus =
                        'Graded'

                    AND
                    QA.SubmittedAt
                        IS NOT NULL

                    AND
                    (
                        Q.CreatedByUserId =
                            @LecturerId

                        OR

                        QS.LecturerUserId =
                            @LecturerId
                    )

                    AND EXISTS
                    (
                        SELECT 1

                        FROM StudentAnswers SAGraded

                        WHERE
                            SAGraded.AttemptId =
                                QA.AttemptId

                            AND SAGraded.GradedByUserId =
                                @LecturerId
                    )

                    AND
                    (
                        @QuizId IS NULL

                        OR

                        QA.QuizId =
                            @QuizId
                    )

                    AND
                    (
                        @Search = ''

                        OR

                        Student.FullName
                            LIKE '%' + @Search + '%'

                        OR

                        Q.Title
                            LIKE '%' + @Search + '%'

                        OR

                        ISNULL(
                            QS.SessionTitle,
                            ''
                        )
                            LIKE '%' + @Search + '%'
                    )

                ORDER BY
                    GradedAt DESC,
                    QA.AttemptId DESC;
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
                    "@LecturerId",
                    SqlDbType.Int
                ).Value =
                    CurrentUserId;

                command.Parameters.Add(
                    "@QuizId",
                    SqlDbType.Int
                ).Value =
                    selectedQuizId.HasValue

                    ? (object)
                        selectedQuizId.Value

                    : DBNull.Value;

                command.Parameters.Add(
                    "@Search",
                    SqlDbType.NVarChar,
                    300
                ).Value =
                    search;

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

            table.Columns.Add(
                "StudentInitials",
                typeof(string)
            );

            table.Columns.Add(
                "HasSession",
                typeof(bool)
            );

            table.Columns.Add(
                "FinalScoreText",
                typeof(string)
            );

            table.Columns.Add(
                "GradedAtText",
                typeof(string)
            );

            table.Columns.Add(
                "ReviewUrl",
                typeof(string)
            );

            foreach (
                DataRow row
                in table.Rows
            )
            {
                string studentName =
                    row["StudentName"]
                    .ToString();

                row["StudentInitials"] =
                    GetInitials(
                        studentName
                    );

                row["StudentName"] =
                    Server.HtmlEncode(
                        studentName
                    );

                row["QuizTitle"] =
                    Server.HtmlEncode(
                        row["QuizTitle"]
                        .ToString()
                    );

                bool hasSession =
                    row["SessionId"]
                    != DBNull.Value;

                row["HasSession"] =
                    hasSession;

                if (hasSession)
                {
                    row["SessionTitle"] =
                        Server.HtmlEncode(
                            row[
                                "SessionTitle"
                            ]
                            .ToString()
                        );
                }

                decimal awarded =
                    Convert.ToDecimal(
                        row[
                            "AwardedMarks"
                        ]
                    );

                decimal total =
                    Convert.ToDecimal(
                        row[
                            "TotalMarks"
                        ]
                    );

                decimal percentage =
                    total > 0
                    ? awarded / total * 100
                    : 0;

                row["FinalScoreText"] =
                    awarded.ToString(
                        "0.##"
                    )
                    + " / "
                    + total.ToString(
                        "0.##"
                    )
                    + " Marks ("
                    + percentage.ToString(
                        "0.##"
                    )
                    + "%)";

                if (
                    row["GradedAt"]
                    == DBNull.Value
                )
                {
                    row["GradedAtText"] =
                        "—";
                }
                else
                {
                    row["GradedAtText"] =
                        Convert.ToDateTime(
                            row["GradedAt"]
                        )
                        .ToString(
                            "dd MMM yyyy, hh:mm tt"
                        );
                }

                row["ReviewUrl"] =
                    ResolveUrl(
                        "~/Lecturer/GradingDetails.aspx?attemptId="
                        + row["AttemptId"]
                    );
            }

            rptReviewHistory.DataSource =
                table;

            rptReviewHistory.DataBind();

            pnlNoReviews.Visible =
                table.Rows.Count == 0;

            pnlReviewList.Visible =
                table.Rows.Count > 0;
        }

        private string GetInitials(
            string fullName)
        {
            if (
                string.IsNullOrWhiteSpace(
                    fullName
                )
            )
            {
                return "S";
            }

            string[] parts =
                fullName.Split(
                    new char[]
                    {
                        ' '
                    },
                    StringSplitOptions
                    .RemoveEmptyEntries
                );

            if (parts.Length == 1)
            {
                return parts[0]
                    .Substring(
                        0,
                        1
                    )
                    .ToUpper();
            }

            return
                (
                    parts[0]
                    .Substring(
                        0,
                        1
                    )
                    +
                    parts[
                        parts.Length - 1
                    ]
                    .Substring(
                        0,
                        1
                    )
                )
                .ToUpper();
        }

        private void ShowMessage(
            string message)
        {
            lblMessage.Text =
                message;

            pnlMessage.Visible =
                true;
        }
    }
}