using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace SPMaster.Student
{
    public partial class Portal : System.Web.UI.Page
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
        // CURRENT STUDENT
        // =========================================================

        private int CurrentStudentUserId
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
                hfOpenJoinModal.Value =
                    "0";

                pnlJoinError.Visible =
                    false;

                pnlFoundSession.Visible =
                    false;

                btnStartSession.Visible =
                    false;


                // =============================================
                // LOAD STUDENT DASHBOARD
                // =============================================

                LoadStudentHeader();

                LoadStudentSummary();

                LoadSubjectHubs();
                
                LoadWeeklyActivity();

                LoadRecentAttempts();
            }
        }
        private void LoadWeeklyActivity()
        {
            const string sql = @"
        SELECT
            CAST(StartedAt AS DATE) AS AttemptDate,
            COUNT(*) AS AttemptCount

        FROM QuizAttempts

        WHERE
            StudentUserId = @StudentUserId

            AND StartedAt >=
                DATEADD(
                    DAY,
                    -6,
                    CAST(GETDATE() AS DATE)
                )

            AND StartedAt <
                DATEADD(
                    DAY,
                    1,
                    CAST(GETDATE() AS DATE)
                )

        GROUP BY
            CAST(StartedAt AS DATE);
    ";


            DataTable databaseTable =
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
                    "@StudentUserId",
                    SqlDbType.Int
                ).Value =
                    CurrentStudentUserId;


                using (
                    SqlDataAdapter adapter =
                        new SqlDataAdapter(
                            command
                        )
                )
                {
                    adapter.Fill(
                        databaseTable
                    );
                }
            }


            DataTable chartTable =
                new DataTable();


            chartTable.Columns.Add(
                "DayName",
                typeof(string)
            );


            chartTable.Columns.Add(
                "AttemptCount",
                typeof(int)
            );


            chartTable.Columns.Add(
                "BarHeight",
                typeof(int)
            );


            int totalAttempts = 0;
            int maxAttempts = 0;


            // =====================================================
            // CREATE LAST 7 DAYS
            // =====================================================

            for (int i = 6; i >= 0; i--)
            {
                DateTime date =
                    DateTime.Today.AddDays(
                        -i
                    );


                int attemptCount = 0;


                foreach (
                    DataRow dbRow
                    in databaseTable.Rows
                )
                {
                    DateTime attemptDate =
                        Convert.ToDateTime(
                            dbRow["AttemptDate"]
                        );


                    if (
                        attemptDate.Date
                        == date.Date
                    )
                    {
                        attemptCount =
                            Convert.ToInt32(
                                dbRow[
                                    "AttemptCount"
                                ]
                            );

                        break;
                    }
                }


                totalAttempts +=
                    attemptCount;


                if (
                    attemptCount >
                    maxAttempts
                )
                {
                    maxAttempts =
                        attemptCount;
                }


                DataRow row =
                    chartTable.NewRow();


                row["DayName"] =
                    date.ToString(
                        "ddd"
                    );


                row["AttemptCount"] =
                    attemptCount;


                chartTable.Rows.Add(
                    row
                );
            }


            // =====================================================
            // CALCULATE BAR HEIGHT
            // =====================================================

            foreach (
                DataRow row
                in chartTable.Rows
            )
            {
                int count =
                    Convert.ToInt32(
                        row[
                            "AttemptCount"
                        ]
                    );


                int barHeight = 0;


                if (
                    maxAttempts > 0
                    &&
                    count > 0
                )
                {
                    barHeight =
                        Convert.ToInt32(
                            Math.Round(
                                count
                                * 100.0
                                / maxAttempts
                            )
                        );


                    if (barHeight < 15)
                    {
                        barHeight = 15;
                    }
                }


                row["BarHeight"] =
                    barHeight;
            }


            // =====================================================
            // TOTAL LABEL
            // =====================================================

            lblWeeklyAttempts.Text =
                totalAttempts
                + (
                    totalAttempts == 1
                    ? " attempt"
                    : " attempts"
                );


            // =====================================================
            // BIND CHART
            // =====================================================

            rptWeeklyActivity.DataSource =
                chartTable;


            rptWeeklyActivity.DataBind();
        }
        // =========================================================
        // LOAD SUBJECT HUBS
        // =========================================================

        private void LoadSubjectHubs()
        {
            const string sql = @"
        SELECT
            S.SubjectId,
            RTRIM(S.SubjectName) AS SubjectName,

            ISNULL(
                RTRIM(S.Description),
                ''
            ) AS Description,

            COUNT(
                CASE
                    WHEN Q.IsPublished = 1
                         AND Q.IsArchived = 0
                    THEN Q.QuizId
                END
            ) AS QuizCount

        FROM Subjects S

        LEFT JOIN Quizzes Q
            ON S.SubjectId =
               Q.SubjectId

        WHERE
            S.IsActive = 1

        GROUP BY
            S.SubjectId,
            S.SubjectName,
            S.Description

        ORDER BY
            S.SubjectName;
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


            // =====================================================
            // EXTRA UI COLUMNS
            // =====================================================

            table.Columns.Add(
                "IconCss",
                typeof(string)
            );


            table.Columns.Add(
                "IconClass",
                typeof(string)
            );


            foreach (
                DataRow row
                in table.Rows
            )
            {
                string subject =
                    row["SubjectName"]
                        .ToString()
                        .Trim();


                // =============================================
                // MATHEMATICS
                // =============================================

                if (
                    subject.Equals(
                        "Mathematics",
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    row["IconCss"] =
                        "subject-icon-purple";

                    row["IconClass"] =
                        "bi bi-calculator";
                }

                // =============================================
                // ENGLISH
                // =============================================

                else if (
                    subject.IndexOf(
                        "English",
                        StringComparison.OrdinalIgnoreCase
                    ) >= 0
                )
                {
                    row["IconCss"] =
                        "subject-icon-green";

                    row["IconClass"] =
                        "bi bi-translate";
                }

                // =============================================
                // SEJARAH
                // =============================================

                else if (
                    subject.IndexOf(
                        "Sejarah",
                        StringComparison.OrdinalIgnoreCase
                    ) >= 0
                    ||
                    subject.IndexOf(
                        "History",
                        StringComparison.OrdinalIgnoreCase
                    ) >= 0
                )
                {
                    row["IconCss"] =
                        "subject-icon-yellow";

                    row["IconClass"] =
                        "bi bi-bank";
                }

                // =============================================
                // SCIENCE
                // =============================================

                else if (
                    subject.IndexOf(
                        "Science",
                        StringComparison.OrdinalIgnoreCase
                    ) >= 0
                    ||
                    subject.IndexOf(
                        "Sains",
                        StringComparison.OrdinalIgnoreCase
                    ) >= 0
                )
                {
                    row["IconCss"] =
                        "subject-icon-lavender";

                    row["IconClass"] =
                        "bi bi-flask";
                }

                // =============================================
                // BAHASA MELAYU
                // =============================================

                else if (
                    subject.IndexOf(
                        "Bahasa",
                        StringComparison.OrdinalIgnoreCase
                    ) >= 0
                )
                {
                    row["IconCss"] =
                        "subject-icon-pink";

                    row["IconClass"] =
                        "bi bi-book";
                }

                // =============================================
                // ADDITIONAL MATHEMATICS
                // =============================================

                else if (
                    subject.IndexOf(
                        "Additional",
                        StringComparison.OrdinalIgnoreCase
                    ) >= 0
                )
                {
                    row["IconCss"] =
                        "subject-icon-green";

                    row["IconClass"] =
                        "bi bi-sigma";
                }

                // =============================================
                // DEFAULT
                // =============================================

                else
                {
                    row["IconCss"] =
                        "subject-icon-purple";

                    row["IconClass"] =
                        "bi bi-journal-bookmark";
                }


                // =============================================
                // FALLBACK DESCRIPTION
                // =============================================

                if (
                    string.IsNullOrWhiteSpace(
                        row["Description"]
                            .ToString()
                    )
                )
                {
                    row["Description"] =
                        "Explore available quizzes for this subject.";
                }
            }


            rptSubjectHubs.DataSource =
                table;


            rptSubjectHubs.DataBind();


            pnlNoSubjects.Visible =
                table.Rows.Count == 0;


            rptSubjectHubs.Visible =
                table.Rows.Count > 0;
        }
        private void LoadStudentHeader()
        {
            string studentName =
                Session["FullName"] == null
                ? "Student"
                : Session["FullName"]
                    .ToString();


            lblStudentName.Text =
                studentName;


            lblHeroName.Text =
                studentName;


            lblCurrentDate.Text =
                DateTime.Now.ToString(
                    "dd MMMM yyyy"
                );
        }

        private void LoadStudentSummary()
        {
            const string sql = @"
        SELECT

            COUNT(
                CASE
                    WHEN QA.SubmittedAt
                         IS NOT NULL
                    THEN 1
                END
            ) AS CompletedAttempts,


            COUNT(
                CASE
                    WHEN QA.AttemptStatus =
                         'PendingGrading'
                    THEN 1
                END
            ) AS PendingGrading,


            COUNT(
                CASE
                    WHEN QA.AttemptStatus =
                         'Graded'
                    THEN 1
                END
            ) AS GradedAttempts

        FROM QuizAttempts QA

        WHERE
            QA.StudentUserId =
                @StudentUserId;
    ";


            int completedAttempts = 0;
            int pendingGrading = 0;
            int gradedAttempts = 0;


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
                    "@StudentUserId",
                    SqlDbType.Int
                ).Value =
                    CurrentStudentUserId;


                connection.Open();


                using (
                    SqlDataReader reader =
                        command.ExecuteReader()
                )
                {
                    if (reader.Read())
                    {
                        completedAttempts =
                            Convert.ToInt32(
                                reader[
                                    "CompletedAttempts"
                                ]
                            );


                        pendingGrading =
                            Convert.ToInt32(
                                reader[
                                    "PendingGrading"
                                ]
                            );


                        gradedAttempts =
                            Convert.ToInt32(
                                reader[
                                    "GradedAttempts"
                                ]
                            );
                    }
                }
            }


            lblCompletedAttempts.Text =
                completedAttempts
                    .ToString();


            lblPendingGrading.Text =
                pendingGrading
                    .ToString();


            LoadAverageScore(
                gradedAttempts
            );
        }

        private void LoadAverageScore(
    int gradedAttempts)
        {
            const string sql = @"
        WITH AttemptScores AS
        (
            SELECT
                QA.AttemptId,

                CASE

                    WHEN
                        SUM(
                            QQ.MaxMarks
                        ) > 0

                    THEN

                        SUM(
                            ISNULL(
                                SA.MarksAwarded,
                                0
                            )
                        )
                        * 100.0
                        /
                        SUM(
                            QQ.MaxMarks
                        )

                    ELSE 0

                END AS Percentage

            FROM QuizAttempts QA

            INNER JOIN QuizQuestions QQ
                ON QA.QuizId =
                   QQ.QuizId

            LEFT JOIN StudentAnswers SA
                ON SA.AttemptId =
                   QA.AttemptId

                AND SA.QuizQuestionId =
                   QQ.QuizQuestionId

            WHERE
                QA.StudentUserId =
                    @StudentUserId

                AND QA.AttemptStatus =
                    'Graded'

            GROUP BY
                QA.AttemptId
        )

        SELECT
            AVG(
                Percentage
            ) AS AverageScore

        FROM AttemptScores;
    ";


            decimal averageScore =
                0;


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
                    "@StudentUserId",
                    SqlDbType.Int
                ).Value =
                    CurrentStudentUserId;


                connection.Open();


                object result =
                    command.ExecuteScalar();


                if (
                    result != null
                    &&
                    result != DBNull.Value
                )
                {
                    averageScore =
                        Convert.ToDecimal(
                            result
                        );
                }
            }


            if (gradedAttempts == 0)
            {
                lblAverageScore.Text =
                    "–";


                lblAverageDescription.Text =
                    "No graded attempts yet";
            }
            else
            {
                lblAverageScore.Text =
                    averageScore.ToString(
                        "0.0"
                    )
                    + "%";


                lblAverageDescription.Text =
                    "Calculated from "
                    + gradedAttempts
                    + (
                        gradedAttempts == 1
                        ? " graded attempt"
                        : " graded attempts"
                    );
            }
        }
        private void LoadRecentAttempts()
        {
            const string sql = @"
        SELECT TOP 5

            QA.AttemptId,
            QA.AttemptStatus,
            QA.StartedAt,
            QA.SubmittedAt,

            Q.Title AS QuizTitle

        FROM QuizAttempts QA

        INNER JOIN Quizzes Q
            ON QA.QuizId =
               Q.QuizId

        WHERE
            QA.StudentUserId =
                @StudentUserId

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
                    "@StudentUserId",
                    SqlDbType.Int
                ).Value =
                    CurrentStudentUserId;


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
                "AttemptDescription",
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
                "IconCss",
                typeof(string)
            );


            table.Columns.Add(
                "IconClass",
                typeof(string)
            );


            table.Columns.Add(
                "ActionText",
                typeof(string)
            );


            table.Columns.Add(
                "ActionCss",
                typeof(string)
            );


            table.Columns.Add(
                "ActionUrl",
                typeof(string)
            );


            foreach (
                DataRow row
                in table.Rows
            )
            {
                string status =
                    row["AttemptStatus"]
                        .ToString();


                DateTime startedAt =
                    Convert.ToDateTime(
                        row["StartedAt"]
                    );


                row["AttemptDescription"] =
                    "Started "
                    + startedAt.ToString(
                        "dd MMM yyyy, hh:mm tt"
                    );


                if (
                    status.Equals(
                        "InProgress",
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    row["StatusText"] =
                        "In Progress";

                    row["StatusCss"] ="recent-status-progress";

                    row["IconCss"] =
                        "attempt-icon-progress";

                    row["IconClass"] =
                        "bi bi-play-circle";

                    row["ActionText"] =
                        "Continue";

                    row["ActionCss"] =
                        "recent-attempt-button";

                    row["ActionUrl"] =
                        ResolveUrl(
                            "~/Student/StartQuiz.aspx?attemptId="
                            + row["AttemptId"]
                        );
                }

                else if (
                    status.Equals(
                        "PendingGrading",
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    row["StatusText"] =
                        "Awaiting Grading";
                    row["StatusCss"] ="recent-status-pending";

                    row["IconCss"] =
                        "attempt-icon-pending";

                    row["IconClass"] =
                        "bi bi-hourglass-split";

                    row["ActionText"] =
                        "Pending";

                    row["ActionCss"] =
                        "recent-attempt-button disabled";

                    row["ActionUrl"] =
                        "#";
                }

                else
                {
                    row["StatusText"] =
                        "Graded";

                    row["StatusCss"] ="recent-status-graded";

                    row["IconCss"] =
                        "attempt-icon-graded";

                    row["IconClass"] =
                        "bi bi-check-circle";

                    row["ActionText"] =
                        "Completed";

                    row["ActionCss"] =
                        "recent-attempt-button completed-button";

                    row["ActionUrl"] =
                        ResolveUrl(
                            "~/Student/Performance.aspx"
                        );
                }
            }


            rptRecentAttempts.DataSource =
                table;

            rptRecentAttempts.DataBind();


            pnlNoRecentAttempts.Visible =
                table.Rows.Count == 0;


            rptRecentAttempts.Visible =
                table.Rows.Count > 0;
        }
        // =========================================================
        // FIND SESSION
        // =========================================================

        protected void btnFindSession_Click(
            object sender,
            EventArgs e)
        {
            Page.Validate(
                "JoinSession"
            );


            hfOpenJoinModal.Value =
                "1";


            pnlJoinError.Visible =
                false;

            pnlFoundSession.Visible =
                false;

            btnStartSession.Visible =
                false;


            // =====================================================
            // ASP.NET VALIDATION
            // =====================================================

            if (!Page.IsValid)
            {
                return;
            }


            try
            {
                string joinCode =
                    txtJoinCode.Text
                        .Trim()
                        .ToUpper();


                // =============================================
                // EMPTY CODE
                // =============================================

                if (
                    string.IsNullOrWhiteSpace(
                        joinCode
                    )
                )
                {
                    ShowJoinError(
                        "Please enter a session join code."
                    );

                    return;
                }


                // =============================================
                // JOIN CODE FORMAT
                // =============================================

                if (
                    !joinCode.StartsWith(
                        "SPM-",
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    ShowJoinError(
                        "Invalid join code format. Please enter a code such as SPM-1234."
                    );

                    return;
                }


                // =============================================
                // FIND SESSION
                // =============================================

                const string sql = @"
                    SELECT
                        QS.SessionId,
                        QS.SessionTitle,
                        QS.QuizId,
                        QS.IsOpen,

                        Q.Title,
                        Q.TimeLimitMinutes,

                        COUNT(
                            QQ.QuizQuestionId
                        ) AS QuestionCount,

                        ISNULL(
                            SUM(
                                QQ.MaxMarks
                            ),
                            0
                        ) AS TotalMarks

                    FROM QuizSessions QS

                    INNER JOIN Quizzes Q
                        ON QS.QuizId =
                           Q.QuizId

                    LEFT JOIN QuizQuestions QQ
                        ON Q.QuizId =
                           QQ.QuizId

                    WHERE
                        UPPER(QS.JoinCode) =
                        @JoinCode

                    GROUP BY
                        QS.SessionId,
                        QS.SessionTitle,
                        QS.QuizId,
                        QS.IsOpen,
                        Q.Title,
                        Q.TimeLimitMinutes;
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
                        "@JoinCode",
                        SqlDbType.NVarChar,
                        20
                    ).Value =
                        joinCode;


                    connection.Open();


                    using (
                        SqlDataReader reader =
                            command.ExecuteReader()
                    )
                    {
                        // =====================================
                        // SESSION NOT FOUND
                        // =====================================

                        if (!reader.Read())
                        {
                            ShowJoinError(
                                "Session not found. Please check the join code."
                            );

                            return;
                        }


                        int sessionId =
                            Convert.ToInt32(
                                reader["SessionId"]
                            );


                        int quizId =
                            Convert.ToInt32(
                                reader["QuizId"]
                            );


                        bool isOpen =
                            Convert.ToBoolean(
                                reader["IsOpen"]
                            );


                        // =====================================
                        // SESSION CLOSED
                        // =====================================

                        if (!isOpen)
                        {
                            ShowJoinError(
                                "This session has already been closed."
                            );

                            return;
                        }


                        // =====================================
                        // ALREADY SUBMITTED
                        // =====================================

                        if (
                            HasSubmittedSession(
                                sessionId
                            )
                        )
                        {
                            ShowJoinError(
                                "You have already submitted this quiz."
                            );

                            return;
                        }


                        // =====================================
                        // CHECK QUESTIONS
                        // =====================================

                        int questionCount =
                            Convert.ToInt32(
                                reader[
                                    "QuestionCount"
                                ]
                            );


                        if (questionCount <= 0)
                        {
                            ShowJoinError(
                                "This quiz does not contain any questions."
                            );

                            return;
                        }


                        // =====================================
                        // CHECK TIME LIMIT
                        // =====================================

                        int timeLimit =
                            Convert.ToInt32(
                                reader[
                                    "TimeLimitMinutes"
                                ]
                            );


                        if (timeLimit <= 0)
                        {
                            ShowJoinError(
                                "This quiz has an invalid time limit."
                            );

                            return;
                        }


                        // =====================================
                        // DISPLAY FOUND SESSION
                        // =====================================

                        lblFoundQuizTitle.Text =
                            reader["Title"]
                                .ToString();


                        lblFoundSessionTitle.Text =
                            reader[
                                "SessionTitle"
                            ]
                            .ToString();


                        lblFoundQuestionCount.Text =
                            questionCount
                                .ToString();


                        lblFoundMarks.Text =
                            Convert.ToDecimal(
                                reader[
                                    "TotalMarks"
                                ]
                            )
                            .ToString(
                                "0.##"
                            );


                        lblFoundTime.Text =
                            timeLimit
                                .ToString();


                        // =====================================
                        // SAVE FOUND IDS
                        // =====================================

                        hfFoundSessionId.Value =
                            sessionId
                                .ToString();


                        hfFoundQuizId.Value =
                            quizId
                                .ToString();


                        pnlFoundSession.Visible =
                            true;


                        btnStartSession.Visible =
                            true;


                        // =====================================
                        // START OR CONTINUE
                        // =====================================

                        int existingAttemptId =
                            GetExistingInProgressAttempt(
                                sessionId
                            );


                        btnStartSession.Text =
                            existingAttemptId > 0

                            ? "Continue Attempt"

                            : "Start Attempt";
                    }
                }
            }
            catch (SqlException)
            {
                ShowJoinError(
                    "Unable to connect to the database. Please try again."
                );
            }
            catch (Exception)
            {
                ShowJoinError(
                    "Something went wrong while finding the session. Please try again."
                );
            }
        }


        // =========================================================
        // START / CONTINUE SESSION
        // =========================================================

        protected void btnStartSession_Click(
            object sender,
            EventArgs e)
        {
            hfOpenJoinModal.Value =
                "1";


            pnlJoinError.Visible =
                false;


            try
            {
                int sessionId;
                int quizId;


                // =============================================
                // VALID SESSION + QUIZ IDS
                // =============================================

                if (
                    !int.TryParse(
                        hfFoundSessionId.Value,
                        out sessionId
                    )
                    ||
                    !int.TryParse(
                        hfFoundQuizId.Value,
                        out quizId
                    )
                )
                {
                    ShowJoinError(
                        "Unable to identify the selected session. Please search for the join code again."
                    );

                    return;
                }


                if (
                    sessionId <= 0
                    ||
                    quizId <= 0
                )
                {
                    ShowJoinError(
                        "Invalid session information. Please search for the join code again."
                    );

                    return;
                }


                // =============================================
                // CHECK SESSION STILL OPEN
                // =============================================

                if (
                    !SessionStillOpen(
                        sessionId,
                        quizId
                    )
                )
                {
                    ShowJoinError(
                        "This session has been closed and can no longer be joined."
                    );

                    return;
                }


                // =============================================
                // ALREADY SUBMITTED
                // =============================================

                if (
                    HasSubmittedSession(
                        sessionId
                    )
                )
                {
                    ShowJoinError(
                        "You have already submitted this quiz."
                    );

                    return;
                }


                // =============================================
                // CONTINUE EXISTING ATTEMPT
                // =============================================

                int existingAttemptId =
                    GetExistingInProgressAttempt(
                        sessionId
                    );


                if (existingAttemptId > 0)
                {
                    GoToQuiz(
                        existingAttemptId
                    );

                    return;
                }


                // =============================================
                // GET TIME LIMIT
                // =============================================

                int timeLimit =
                    GetQuizTimeLimit(
                        quizId
                    );


                if (timeLimit <= 0)
                {
                    ShowJoinError(
                        "Unable to start this quiz because the time limit is invalid."
                    );

                    return;
                }


                // =============================================
                // CHECK QUIZ HAS QUESTIONS
                // =============================================

                if (
                    !QuizHasQuestions(
                        quizId
                    )
                )
                {
                    ShowJoinError(
                        "This quiz does not contain any questions."
                    );

                    return;
                }


                // =============================================
                // CREATE ATTEMPT
                // =============================================

                int attemptId =
                    CreateSessionAttempt(
                        sessionId,
                        quizId,
                        timeLimit
                    );


                if (attemptId <= 0)
                {
                    ShowJoinError(
                        "Unable to create your quiz attempt. Please try again."
                    );

                    return;
                }


                // =============================================
                // GO TO QUIZ
                // =============================================

                GoToQuiz(
                    attemptId
                );
            }
            catch (SqlException)
            {
                ShowJoinError(
                    "Unable to connect to the database. Please try again."
                );
            }
            catch (Exception)
            {
                ShowJoinError(
                    "Something went wrong while starting the quiz. Please try again."
                );
            }
        }


        // =========================================================
        // CREATE SESSION ATTEMPT
        // =========================================================

        private int CreateSessionAttempt(
            int sessionId,
            int quizId,
            int timeLimit)
        {
            const string sql = @"
                INSERT INTO QuizAttempts
                (
                    QuizId,
                    StudentUserId,
                    SessionId,
                    StartedAt,
                    ExpiresAt,
                    SubmittedAt,
                    AttemptStatus
                )

                VALUES
                (
                    @QuizId,
                    @StudentUserId,
                    @SessionId,
                    GETDATE(),
                    DATEADD(
                        MINUTE,
                        @TimeLimitMinutes,
                        GETDATE()
                    ),
                    NULL,
                    'InProgress'
                );

                SELECT CAST(
                    SCOPE_IDENTITY()
                    AS INT
                );
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
                    "@QuizId",
                    SqlDbType.Int
                ).Value =
                    quizId;


                command.Parameters.Add(
                    "@StudentUserId",
                    SqlDbType.Int
                ).Value =
                    CurrentStudentUserId;


                command.Parameters.Add(
                    "@SessionId",
                    SqlDbType.Int
                ).Value =
                    sessionId;


                command.Parameters.Add(
                    "@TimeLimitMinutes",
                    SqlDbType.Int
                ).Value =
                    timeLimit;


                connection.Open();


                object result =
                    command.ExecuteScalar();


                if (
                    result == null
                    ||
                    result == DBNull.Value
                )
                {
                    return 0;
                }


                return Convert.ToInt32(
                    result
                );
            }
        }


        // =========================================================
        // CHECK EXISTING IN-PROGRESS ATTEMPT
        // =========================================================

        private int GetExistingInProgressAttempt(
            int sessionId)
        {
            const string sql = @"
                SELECT TOP 1
                    AttemptId

                FROM QuizAttempts

                WHERE
                    SessionId =
                        @SessionId

                    AND StudentUserId =
                        @StudentUserId

                    AND SubmittedAt
                        IS NULL

                    AND AttemptStatus =
                        'InProgress'

                ORDER BY
                    StartedAt DESC;
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
                    sessionId;


                command.Parameters.Add(
                    "@StudentUserId",
                    SqlDbType.Int
                ).Value =
                    CurrentStudentUserId;


                connection.Open();


                object result =
                    command.ExecuteScalar();


                if (
                    result == null
                    ||
                    result == DBNull.Value
                )
                {
                    return 0;
                }


                return Convert.ToInt32(
                    result
                );
            }
        }


        // =========================================================
        // CHECK IF STUDENT ALREADY SUBMITTED
        // =========================================================

        private bool HasSubmittedSession(
            int sessionId)
        {
            const string sql = @"
                SELECT COUNT(*)

                FROM QuizAttempts

                WHERE
                    SessionId =
                        @SessionId

                    AND StudentUserId =
                        @StudentUserId

                    AND SubmittedAt
                        IS NOT NULL;
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
                    sessionId;


                command.Parameters.Add(
                    "@StudentUserId",
                    SqlDbType.Int
                ).Value =
                    CurrentStudentUserId;


                connection.Open();


                return
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    )
                    > 0;
            }
        }


        // =========================================================
        // CHECK SESSION STILL OPEN
        // =========================================================

        private bool SessionStillOpen(
            int sessionId,
            int quizId)
        {
            const string sql = @"
                SELECT COUNT(*)

                FROM QuizSessions QS

                INNER JOIN Quizzes Q
                    ON QS.QuizId =
                       Q.QuizId

                WHERE
                    QS.SessionId =
                        @SessionId

                    AND QS.QuizId =
                        @QuizId

                    AND QS.IsOpen = 1

                    AND Q.IsPublished = 1

                    AND Q.IsArchived = 0;
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
                    sessionId;


                command.Parameters.Add(
                    "@QuizId",
                    SqlDbType.Int
                ).Value =
                    quizId;


                connection.Open();


                return
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    )
                    > 0;
            }
        }


        // =========================================================
        // GET QUIZ TIME LIMIT
        // =========================================================

        private int GetQuizTimeLimit(
            int quizId)
        {
            const string sql = @"
                SELECT
                    TimeLimitMinutes

                FROM Quizzes

                WHERE
                    QuizId =
                        @QuizId

                    AND IsPublished = 1

                    AND IsArchived = 0;
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
                    "@QuizId",
                    SqlDbType.Int
                ).Value =
                    quizId;


                connection.Open();


                object result =
                    command.ExecuteScalar();


                if (
                    result == null
                    ||
                    result == DBNull.Value
                )
                {
                    return 0;
                }


                return Convert.ToInt32(
                    result
                );
            }
        }


        // =========================================================
        // CHECK QUIZ HAS QUESTIONS
        // =========================================================

        private bool QuizHasQuestions(
            int quizId)
        {
            const string sql = @"
                SELECT COUNT(*)

                FROM QuizQuestions

                WHERE
                    QuizId =
                        @QuizId;
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
                    "@QuizId",
                    SqlDbType.Int
                ).Value =
                    quizId;


                connection.Open();


                return
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    )
                    > 0;
            }
        }


        // =========================================================
        // GO TO QUIZ
        // =========================================================

        private void GoToQuiz(
            int attemptId)
        {
            Response.Redirect(
                "~/Student/StartQuiz.aspx?attemptId="
                + attemptId,
                false
            );


            Context
                .ApplicationInstance
                .CompleteRequest();
        }


        // =========================================================
        // SHOW JOIN ERROR
        // =========================================================

        private void ShowJoinError(
            string message)
        {
            lblJoinError.Text =
                message;


            pnlJoinError.Visible =
                true;


            pnlFoundSession.Visible =
                false;


            btnStartSession.Visible =
                false;


            hfOpenJoinModal.Value =
                "1";
        }
    }
}