using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace SPMaster.Lecturer
{
    public partial class SessionDetails :
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
        // SESSION ID FROM URL
        // =========================================================

        private int CurrentSessionId
        {
            get
            {
                int sessionId;


                if (
                    int.TryParse(
                        Request.QueryString["id"],
                        out sessionId
                    )
                )
                {
                    return sessionId;
                }


                return 0;
            }
        }


        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (
                Session["UserId"] == null
            )
            {
                Response.Redirect(
                    "~/Login.aspx"
                );

                return;
            }


            if (
                CurrentSessionId <= 0
            )
            {
                Response.Redirect(
                    "~/Lecturer/Sessions.aspx"
                );

                return;
            }


            if (!IsPostBack)
            {
                LoadPage();
            }
        }


        // =========================================================
        // LOAD PAGE
        // =========================================================

        private void LoadPage()
        {
            bool sessionExists =
                LoadSessionInformation();


            if (!sessionExists)
            {
                Response.Redirect(
                    "~/Lecturer/Sessions.aspx"
                );

                return;
            }


            LoadParticipants();
        }


        // =========================================================
        // LOAD SESSION INFORMATION
        // =========================================================

        private bool LoadSessionInformation()
        {
            const string sql = @"
                SELECT
                    QS.SessionId,
                    QS.SessionTitle,
                    QS.JoinCode,
                    QS.IsOpen,
                    QS.CreatedAt,

                    Q.Title AS QuizTitle,
                    Q.TimeLimitMinutes,

                    S.SubjectName

                FROM QuizSessions QS

                INNER JOIN Quizzes Q
                    ON QS.QuizId =
                       Q.QuizId

                INNER JOIN Subjects S
                    ON Q.SubjectId =
                       S.SubjectId

                WHERE
                    QS.SessionId =
                    @SessionId

                    AND
                    QS.LecturerUserId =
                    @LecturerUserId;
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
                    "@SessionId",
                    SqlDbType.Int
                ).Value =
                    CurrentSessionId;


                command.Parameters.Add(
                    "@LecturerUserId",
                    SqlDbType.Int
                ).Value =
                    CurrentUserId;


                connection.Open();


                using (
                    SqlDataReader reader =
                        command.ExecuteReader()
                )
                {
                    if (!reader.Read())
                    {
                        return false;
                    }


                    lblSessionTitle.Text =
                        reader["SessionTitle"]
                            .ToString();


                    lblQuizTitle.Text =
                        reader["QuizTitle"]
                            .ToString();


                    lblSubjectName.Text =
                        reader["SubjectName"]
                            .ToString()
                            .Trim();


                    lblJoinCode.Text =
                        reader["JoinCode"]
                            .ToString();


                    lblTimeLimit.Text =
                        reader[
                            "TimeLimitMinutes"
                        ]
                        .ToString();


                    DateTime createdAt =
                        Convert.ToDateTime(
                            reader["CreatedAt"]
                        );


                    lblCreatedAt.Text =
                        createdAt.ToString(
                            "dd MMM yyyy, hh:mm tt"
                        );


                    bool isOpen =
                        Convert.ToBoolean(
                            reader["IsOpen"]
                        );


                    if (isOpen)
                    {
                        lblSessionStatus.Text =
                            "Live";

                        lblSessionStatus.CssClass =
                            "session-details-status "
                            + "status-live";


                        pnlJoinCode.Visible =
                            true;

                        pnlClosedSession.Visible =
                            false;

                        btnCloseSession.Visible =
                            true;
                    }
                    else
                    {
                        lblSessionStatus.Text =
                            "Closed";

                        lblSessionStatus.CssClass =
                            "session-details-status "
                            + "status-closed";


                        pnlJoinCode.Visible =
                            false;

                        pnlClosedSession.Visible =
                            true;

                        btnCloseSession.Visible =
                            false;
                    }


                    return true;
                }
            }
        }


        // =========================================================
        // LOAD PARTICIPANTS
        // =========================================================

        private void LoadParticipants()
        {
            const string sql = @"
                SELECT
                    QA.AttemptId,
                    QA.StudentUserId,
                    QA.StartedAt,
                    QA.SubmittedAt,

                    U.FullName
                        AS StudentName,

                    U.Email
                        AS StudentEmail,


                    CASE

                        WHEN QA.SubmittedAt IS NULL
                        THEN 'In Progress'


                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM QuizQuestions QQ2

                            INNER JOIN Questions Q2
                                ON QQ2.QuestionId =
                                   Q2.QuestionId

                            LEFT JOIN StudentAnswers SA2
                                ON SA2.AttemptId =
                                   QA.AttemptId

                                AND
                                SA2.QuizQuestionId =
                                   QQ2.QuizQuestionId

                            WHERE
                                QQ2.QuizId =
                                QA.QuizId

                                AND
                                UPPER(
                                    LTRIM(
                                        RTRIM(
                                            Q2.QuestionType
                                        )
                                    )
                                ) <> 'MCQ'

                                AND
                                (
                                    SA2.AnswerId IS NULL

                                    OR

                                    SA2.MarksAwarded
                                        IS NULL
                                )
                        )

                        THEN 'Pending Review'


                        ELSE 'Graded'

                    END
                    AS GradingStatus,


                    CASE

                        WHEN
                            QA.SubmittedAt IS NOT NULL

                            AND NOT EXISTS
                            (
                                SELECT 1

                                FROM QuizQuestions QQ3

                                INNER JOIN Questions Q3
                                    ON QQ3.QuestionId =
                                       Q3.QuestionId

                                LEFT JOIN StudentAnswers SA3
                                    ON SA3.AttemptId =
                                       QA.AttemptId

                                    AND
                                    SA3.QuizQuestionId =
                                       QQ3.QuizQuestionId

                                WHERE
                                    QQ3.QuizId =
                                    QA.QuizId

                                    AND
                                    UPPER(
                                        LTRIM(
                                            RTRIM(
                                                Q3.QuestionType
                                            )
                                        )
                                    ) <> 'MCQ'

                                    AND
                                    (
                                        SA3.AnswerId
                                            IS NULL

                                        OR

                                        SA3.MarksAwarded
                                            IS NULL
                                    )
                            )

                        THEN
                        (
                            SELECT
                                CAST
                                (
                                    (
                                        ISNULL(
                                            SUM(
                                                ISNULL(
                                                    SA4.MarksAwarded,
                                                    0
                                                )
                                            ),
                                            0
                                        )

                                        * 100.0
                                    )

                                    /

                                    NULLIF(
                                        SUM(
                                            QQ4.MaxMarks
                                        ),
                                        0
                                    )

                                    AS DECIMAL(6,2)
                                )

                            FROM QuizQuestions QQ4

                            LEFT JOIN StudentAnswers SA4
                                ON SA4.AttemptId =
                                   QA.AttemptId

                                AND
                                SA4.QuizQuestionId =
                                   QQ4.QuizQuestionId

                            WHERE
                                QQ4.QuizId =
                                QA.QuizId
                        )

                        ELSE NULL

                    END
                    AS ScorePercentage


                FROM QuizAttempts QA

                INNER JOIN Users U
                    ON QA.StudentUserId =
                       U.UserId

                WHERE
                    QA.SessionId =
                    @SessionId

                ORDER BY
                    QA.StartedAt DESC;
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
                    "@SessionId",
                    SqlDbType.Int
                ).Value =
                    CurrentSessionId;


                SqlDataAdapter adapter =
                    new SqlDataAdapter(
                        command
                    );


                adapter.Fill(
                    table
                );
            }


            // =====================================================
            // UI COLUMNS
            // =====================================================

            table.Columns.Add(
                "Initials",
                typeof(string)
            );


            table.Columns.Add(
                "StatusText",
                typeof(string)
            );


            table.Columns.Add(
                "StatusCss",
                typeof(string)
            );


            table.Columns.Add(
                "ScoreText",
                typeof(string)
            );


            table.Columns.Add(
                "HasAction",
                typeof(bool)
            );


            table.Columns.Add(
                "ActionText",
                typeof(string)
            );


            table.Columns.Add(
                "ActionUrl",
                typeof(string)
            );


            table.Columns.Add(
                "ActionCss",
                typeof(string)
            );


            int submittedCount = 0;

            int fullyGradedCount = 0;

            int pendingCount = 0;

            decimal totalGradedScore = 0;


            foreach (
                DataRow row
                in table.Rows
            )
            {
                // -------------------------------------------------
                // INITIALS
                // -------------------------------------------------

                string studentName =
                    row["StudentName"]
                        .ToString();


                row["Initials"] =
                    GetInitials(
                        studentName
                    );


                // -------------------------------------------------
                // STATUS
                // -------------------------------------------------

                string status =
                    row["GradingStatus"]
                        .ToString();


                row["StatusText"] =
                    status;


                if (
                    status == "In Progress"
                )
                {
                    row["StatusCss"] =
                        "participant-status "
                        + "participant-progress";

                    row["ScoreText"] =
                        "—";

                    row["HasAction"] =
                        false;

                    row["ActionText"] =
                        "";

                    row["ActionUrl"] =
                        "";

                    row["ActionCss"] =
                        "";
                }


                else if (
                    status == "Pending Review"
                )
                {
                    submittedCount++;

                    pendingCount++;


                    row["StatusCss"] =
                        "participant-status "
                        + "participant-pending";


                    row["ScoreText"] =
                        "Pending";


                    row["HasAction"] =
                        true;


                    row["ActionText"] =
                        "Grade Now";


                    row["ActionUrl"] =
                        ResolveUrl(
                            "~/Lecturer/Grading.aspx"
                            + "?attemptId="
                            + row["AttemptId"]
                        );


                    row["ActionCss"] =
                        "participant-action "
                        + "participant-action-primary";
                }


                else
                {
                    submittedCount++;

                    fullyGradedCount++;


                    row["StatusCss"] =
                        "participant-status "
                        + "participant-graded";


                    decimal score = 0;


                    if (
                        row["ScorePercentage"]
                        != DBNull.Value
                    )
                    {
                        score =
                            Convert.ToDecimal(
                                row[
                                    "ScorePercentage"
                                ]
                            );


                        totalGradedScore +=
                            score;


                        row["ScoreText"] =
                            score.ToString(
                                "0.##"
                            )
                            + "%";
                    }
                    else
                    {
                        row["ScoreText"] =
                            "0%";
                    }


                    row["HasAction"] =
                        true;


                    row["ActionText"] =
                        "Review";


                    row["ActionUrl"] =
                        ResolveUrl(
                            "~/Lecturer/Grading.aspx"
                            + "?attemptId="
                            + row["AttemptId"]
                        );


                    row["ActionCss"] =
                        "participant-action "
                        + "participant-action-light";
                }
            }


            // =====================================================
            // BIND TABLE
            // =====================================================

            rptParticipants.DataSource =
                table;

            rptParticipants.DataBind();


            int totalAttempts =
                table.Rows.Count;


            pnlNoParticipants.Visible =
                totalAttempts == 0;


            pnlParticipants.Visible =
                totalAttempts > 0;


            // =====================================================
            // SUMMARY
            // =====================================================

            lblTotalAttempts.Text =
                totalAttempts.ToString();


            lblSubmitted.Text =
                submittedCount.ToString();


            lblFullyGraded.Text =
                fullyGradedCount.ToString();


            lblPendingReview.Text =
                pendingCount.ToString();


            lblParticipantCount.Text =
                totalAttempts
                + (
                    totalAttempts == 1
                        ? " Student"
                        : " Students"
                );


            if (
                fullyGradedCount > 0
            )
            {
                decimal average =
                    totalGradedScore
                    /
                    fullyGradedCount;


                lblAverageScore.Text =
                    average.ToString(
                        "0.##"
                    )
                    + "%";
            }
            else
            {
                lblAverageScore.Text =
                    "—";
            }
        }


        // =========================================================
        // REFRESH
        // =========================================================

        protected void btnRefresh_Click(
            object sender,
            EventArgs e)
        {
            LoadPage();
        }


        // =========================================================
        // CLOSE SESSION
        // =========================================================

        protected void btnCloseSession_Click(
            object sender,
            EventArgs e)
        {
            const string sql = @"
                UPDATE QuizSessions

                SET
                    IsOpen = 0

                WHERE
                    SessionId =
                    @SessionId

                    AND
                    LecturerUserId =
                    @LecturerUserId;
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
                    "@SessionId",
                    SqlDbType.Int
                ).Value =
                    CurrentSessionId;


                command.Parameters.Add(
                    "@LecturerUserId",
                    SqlDbType.Int
                ).Value =
                    CurrentUserId;


                connection.Open();

                command.ExecuteNonQuery();
            }


            LoadPage();
        }


        // =========================================================
        // GET STUDENT INITIALS
        // =========================================================

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


            if (
                parts.Length == 1
            )
            {
                return parts[0]
                    .Substring(
                        0,
                        1
                    )
                    .ToUpper();
            }


            return (
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
    }
}