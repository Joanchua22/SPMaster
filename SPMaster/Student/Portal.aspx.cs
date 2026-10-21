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
            }
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