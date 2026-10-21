using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web.UI.WebControls;


namespace SPMaster.Lecturer
{
    public partial class Sessions : System.Web.UI.Page
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
        // CURRENT LECTURER
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
                hfOpenSessionModal.Value =
                    "0";


                LoadPublishedQuizzes();


                GenerateJoinCode();


                BindSessions();
            }
        }



        //
        // LOAD PUBLISHED QUIZZES
        //
        // Shows all published and non-archived quizzes.

        private void LoadPublishedQuizzes()
        {
            const string sql = @"
        SELECT
            Q.QuizId,

            RTRIM(S.SubjectName)
            + ' - '
            + Q.Title
            + ' (by '
            + U.FullName
            + ')'
            AS DisplayText

        FROM Quizzes Q

        INNER JOIN Subjects S
            ON Q.SubjectId =
               S.SubjectId

        INNER JOIN Users U
            ON Q.CreatedByUserId =
               U.UserId

        WHERE
            Q.IsPublished = 1

            AND Q.IsArchived = 0

        ORDER BY
            S.SubjectName,
            Q.Title;
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
                connection.Open();


                ddlPublishedQuiz.DataSource =
                    command.ExecuteReader();


                ddlPublishedQuiz.DataTextField =
                    "DisplayText";


                ddlPublishedQuiz.DataValueField =
                    "QuizId";


                ddlPublishedQuiz.DataBind();
            }


            ddlPublishedQuiz.Items.Insert(
                0,
                new ListItem(
                    "-- Select Published Quiz --",
                    ""
                )
            );


            if (
                ddlPublishedQuiz.Items.Count <= 1
            )
            {
                btnCreateSession.Enabled =
                    false;
            }
            else
            {
                btnCreateSession.Enabled =
                    true;
            }
        }



        // =========================================================
        // BIND BOTH ACTIVE + CLOSED
        // =========================================================

        private void BindSessions()
        {
            List<SessionItem> active =
                GetSessions(
                    true
                );


            List<SessionItem> history =
                GetSessions(
                    false
                );


            rptActiveSessions.DataSource =
                active;

            rptActiveSessions.DataBind();



            rptHistory.DataSource =
                history;

            rptHistory.DataBind();



            pnlNoActiveSessions.Visible =
                active.Count == 0;


            pnlNoHistory.Visible =
                history.Count == 0;



            lblActiveCount.Text =
                active.Count
                + (
                    active.Count == 1
                        ? " session"
                        : " sessions"
                );


            lblHistoryCount.Text =
                history.Count
                + (
                    history.Count == 1
                        ? " session"
                        : " sessions"
                );
        }



        // =========================================================
        // GET SESSION DATA
        //
        // Submitted attempts are counted as completed.
        //
        // Subjective question:
        // MarksAwarded NULL
        //     = Pending Review
        //
        // No ungraded subjective questions
        //     = Fully Graded
        //
        // Average only includes fully graded attempts.
        // =========================================================

        private List<SessionItem> GetSessions(
            bool isOpen)
        {
            List<SessionItem> items =
                new List<SessionItem>();



            const string sql = @"
                WITH AttemptInfo AS
                (
                    SELECT
                        QA.AttemptId,
                        QA.SessionId,
                        QA.QuizId,


                        CASE
                            WHEN
                                QA.SubmittedAt
                                IS NOT NULL
                            THEN 1
                            ELSE 0
                        END
                        AS IsCompleted,


                        CASE
                            WHEN
                                QA.SubmittedAt
                                IS NOT NULL

                                AND EXISTS
                                (
                                    SELECT 1

                                    FROM QuizQuestions QQ

                                    INNER JOIN Questions Q
                                        ON QQ.QuestionId =
                                           Q.QuestionId

                                    LEFT JOIN StudentAnswers SA
                                        ON SA.AttemptId =
                                           QA.AttemptId

                                        AND SA.QuizQuestionId =
                                            QQ.QuizQuestionId

                                    WHERE
                                        QQ.QuizId =
                                            QA.QuizId

                                        AND UPPER(
                                            LTRIM(
                                                RTRIM(
                                                    Q.QuestionType
                                                )
                                            )
                                        ) <> 'MCQ'

                                        AND
                                        SA.MarksAwarded
                                            IS NULL
                                )

                            THEN 1

                            ELSE 0
                        END
                        AS IsPendingReview,


                        CASE
                            WHEN
                                QA.SubmittedAt
                                IS NOT NULL

                                AND NOT EXISTS
                                (
                                    SELECT 1

                                    FROM QuizQuestions QQ

                                    INNER JOIN Questions Q
                                        ON QQ.QuestionId =
                                           Q.QuestionId

                                    LEFT JOIN StudentAnswers SA
                                        ON SA.AttemptId =
                                           QA.AttemptId

                                        AND SA.QuizQuestionId =
                                            QQ.QuizQuestionId

                                    WHERE
                                        QQ.QuizId =
                                            QA.QuizId

                                        AND UPPER(
                                            LTRIM(
                                                RTRIM(
                                                    Q.QuestionType
                                                )
                                            )
                                        ) <> 'MCQ'

                                        AND
                                        SA.MarksAwarded
                                            IS NULL
                                )

                            THEN 1

                            ELSE 0
                        END
                        AS IsFullyGraded

                    FROM QuizAttempts QA
                ),


                AttemptScores AS
                (
                    SELECT
                        AI.AttemptId,
                        AI.SessionId,
                        AI.IsCompleted,
                        AI.IsPendingReview,
                        AI.IsFullyGraded,


                        CASE
                            WHEN
                                AI.IsFullyGraded = 1

                            THEN
                                CAST(
                                    (
                                        SELECT
                                            ISNULL(
                                                SUM(
                                                    ISNULL(
                                                        SA.MarksAwarded,
                                                        0
                                                    )
                                                ),
                                                0
                                            )

                                        FROM QuizQuestions QQ

                                        LEFT JOIN StudentAnswers SA
                                            ON SA.QuizQuestionId =
                                               QQ.QuizQuestionId

                                            AND SA.AttemptId =
                                                AI.AttemptId

                                        WHERE
                                            QQ.QuizId =
                                                AI.QuizId
                                    )
                                    * 100.0
                                    /
                                    NULLIF(
                                        (
                                            SELECT
                                                SUM(
                                                    QQ2.MaxMarks
                                                )

                                            FROM QuizQuestions QQ2

                                            WHERE
                                                QQ2.QuizId =
                                                    AI.QuizId
                                        ),
                                        0
                                    )

                                    AS DECIMAL(6,2)
                                )

                            ELSE NULL
                        END
                        AS ScorePercentage

                    FROM AttemptInfo AI
                )


                SELECT
                    QS.SessionId,
                    QS.SessionTitle,
                    QS.JoinCode,
                    QS.CreatedAt,

                    Q.Title
                        AS QuizTitle,

                    RTRIM(
                        S.SubjectName
                    )
                        AS SubjectName,


                    ISNULL(
                        SUM(
                            A.IsCompleted
                        ),
                        0
                    )
                        AS CompletedAttempts,


                    ISNULL(
                        SUM(
                            A.IsFullyGraded
                        ),
                        0
                    )
                        AS FullyGraded,


                    ISNULL(
                        SUM(
                            A.IsPendingReview
                        ),
                        0
                    )
                        AS PendingReview,


                    AVG(
                        CASE
                            WHEN
                                A.IsFullyGraded = 1

                            THEN
                                A.ScorePercentage

                            ELSE NULL
                        END
                    )
                        AS AverageScore


                FROM QuizSessions QS


                INNER JOIN Quizzes Q
                    ON QS.QuizId =
                       Q.QuizId


                INNER JOIN Subjects S
                    ON Q.SubjectId =
                       S.SubjectId


                LEFT JOIN AttemptScores A
                    ON QS.SessionId =
                       A.SessionId


                WHERE
                    QS.LecturerUserId =
                        @UserId

                    AND QS.IsOpen =
                        @IsOpen


                GROUP BY
                    QS.SessionId,
                    QS.SessionTitle,
                    QS.JoinCode,
                    QS.CreatedAt,
                    Q.Title,
                    S.SubjectName


                ORDER BY
                    QS.CreatedAt DESC;
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


                command.Parameters.Add(
                    "@IsOpen",
                    SqlDbType.Bit
                ).Value =
                    isOpen;


                connection.Open();



                using (
                    SqlDataReader reader =
                        command.ExecuteReader()
                )
                {
                    while (
                        reader.Read()
                    )
                    {
                        decimal? averageScore =
                            null;


                        if (
                            reader["AverageScore"]
                            != DBNull.Value
                        )
                        {
                            averageScore =
                                Convert.ToDecimal(
                                    reader[
                                        "AverageScore"
                                    ]
                                );
                        }



                        SessionItem item =
                            new SessionItem();


                        item.SessionId =
                            Convert.ToInt32(
                                reader[
                                    "SessionId"
                                ]
                            );


                        item.SessionTitle =
                            reader[
                                "SessionTitle"
                            ].ToString();


                        item.JoinCode =
                            reader[
                                "JoinCode"
                            ].ToString();


                        item.CreatedAt =
                            Convert.ToDateTime(
                                reader[
                                    "CreatedAt"
                                ]
                            );


                        item.QuizTitle =
                            reader[
                                "QuizTitle"
                            ].ToString();


                        item.SubjectName =
                            reader[
                                "SubjectName"
                            ]
                            .ToString()
                            .Trim();


                        item.CompletedAttempts =
                            Convert.ToInt32(
                                reader[
                                    "CompletedAttempts"
                                ]
                            );


                        item.FullyGraded =
                            Convert.ToInt32(
                                reader[
                                    "FullyGraded"
                                ]
                            );


                        item.PendingReview =
                            Convert.ToInt32(
                                reader[
                                    "PendingReview"
                                ]
                            );


                        item.AverageScore =
                            averageScore;


                        item.AverageScoreText =
                            averageScore.HasValue

                            ? averageScore.Value
                                .ToString(
                                    "0.0"
                                )
                                + "%"

                            : "–";


                        items.Add(
                            item
                        );
                    }
                }
            }


            return items;
        }



        // =========================================================
        // REGENERATE CODE
        // =========================================================

        protected void btnRegenerateCode_Click(
            object sender,
            EventArgs e)
        {
            GenerateJoinCode();


            hfOpenSessionModal.Value =
                "1";
        }



        // =========================================================
        // GENERATE JOIN CODE
        // =========================================================

        private void GenerateJoinCode()
        {
            string code =
                GenerateUniqueJoinCode();


            lblGeneratedCode.Text =
                code;


            hfGeneratedCode.Value =
                code;
        }



        // =========================================================
        // GENERATE UNIQUE RANDOM CODE
        //
        // Example:
        // SPM-4827
        // =========================================================

        private string GenerateUniqueJoinCode()
        {
            int attempts =
                0;


            while (
                attempts < 50
            )
            {
                int number =
                    GenerateRandomNumber(
                        1000,
                        9999
                    );


                string code =
                    "SPM-"
                    + number;


                if (
                    !JoinCodeExists(
                        code
                    )
                )
                {
                    return code;
                }


                attempts++;
            }


            throw new Exception(
                "Unable to generate a unique session code."
            );
        }



        // =========================================================
        // RANDOM NUMBER
        // =========================================================

        private int GenerateRandomNumber(
            int minimum,
            int maximum)
        {
            byte[] bytes =
                new byte[4];


            using (
                RandomNumberGenerator rng =
                    RandomNumberGenerator.Create()
            )
            {
                rng.GetBytes(
                    bytes
                );
            }


            uint random =
                BitConverter.ToUInt32(
                    bytes,
                    0
                );


            return
                minimum
                +
                (int)(
                    random
                    %
                    (
                        maximum
                        - minimum
                        + 1
                    )
                );
        }



        // =========================================================
        // CHECK JOIN CODE EXISTS
        // =========================================================

        private bool JoinCodeExists(
            string joinCode)
        {
            const string sql = @"
                SELECT COUNT(*)

                FROM QuizSessions

                WHERE
                    JoinCode =
                        @JoinCode;
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


                int count =
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    );


                return
                    count > 0;
            }
        }



        // =========================================================
        // CREATE SESSION
        // =========================================================

        protected void btnCreateSession_Click(
            object sender,
            EventArgs e)
        {
            Page.Validate(
                "HostSession"
            );


            if (!Page.IsValid)
            {
                hfOpenSessionModal.Value =
                    "1";

                return;
            }



            int quizId;


            if (
                !int.TryParse(
                    ddlPublishedQuiz.SelectedValue,
                    out quizId
                )
            )
            {
                ShowHostError(
                    "Please select a valid quiz."
                );

                return;
            }



            if (
                !CanHostQuiz(
                    quizId
                )
            )
            {
                ShowHostError(
                    "This quiz is not available for hosting."
                );

                return;
            }



            string sessionTitle =
                txtSessionTitle.Text
                    .Trim();


            string joinCode =
                hfGeneratedCode.Value
                    .Trim();



            try
            {
                // Generate another code if the existing
                // displayed code has somehow already been used.

                if (
                    string.IsNullOrWhiteSpace(
                        joinCode
                    )
                    ||
                    JoinCodeExists(
                        joinCode
                    )
                )
                {
                    joinCode =
                        GenerateUniqueJoinCode();
                }



                const string sql = @"
                    INSERT INTO QuizSessions
                    (
                        QuizId,
                        LecturerUserId,
                        SessionTitle,
                        JoinCode,
                        IsOpen,
                        CreatedAt
                    )

                    VALUES
                    (
                        @QuizId,
                        @LecturerUserId,
                        @SessionTitle,
                        @JoinCode,
                        1,
                        GETDATE()
                    );


                    SELECT CAST(
                        SCOPE_IDENTITY()
                        AS INT
                    );
                ";



                int sessionId;



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
                        "@LecturerUserId",
                        SqlDbType.Int
                    ).Value =
                        CurrentUserId;


                    command.Parameters.Add(
                        "@SessionTitle",
                        SqlDbType.NVarChar,
                        150
                    ).Value =
                        sessionTitle;


                    command.Parameters.Add(
                        "@JoinCode",
                        SqlDbType.NVarChar,
                        20
                    ).Value =
                        joinCode;


                    connection.Open();


                    sessionId =
                        Convert.ToInt32(
                            command.ExecuteScalar()
                        );
                }



                Response.Redirect(
                    "~/Lecturer/SessionDetails.aspx?id="
                    + sessionId,
                    false
                );


                Context
                    .ApplicationInstance
                    .CompleteRequest();


                return;
            }
            catch (Exception)
            {
                ShowHostError(
                    "Unable to create the session. Please try again."
                );
            }
        }



        // =========================================================
        // VERIFY QUIZ CAN BE HOSTED
        // =========================================================

        private bool CanHostQuiz(
            int quizId)
        {
            const string sql = @"
        SELECT COUNT(*)

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


                return
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    )
                    > 0;
            }
        }



        // =========================================================
        // CLOSE SESSION
        // =========================================================

        protected void rptActiveSessions_ItemCommand(
            object source,
            RepeaterCommandEventArgs e)
        {
            if (
                e.CommandName !=
                "CloseSession"
            )
            {
                return;
            }



            int sessionId;


            if (
                !int.TryParse(
                    e.CommandArgument
                        .ToString(),
                    out sessionId
                )
            )
            {
                return;
            }



            const string sql = @"
                UPDATE QuizSessions

                SET
                    IsOpen = 0

                WHERE
                    SessionId =
                        @SessionId

                    AND LecturerUserId =
                        @UserId;
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
                    "@UserId",
                    SqlDbType.Int
                ).Value =
                    CurrentUserId;


                connection.Open();


                command.ExecuteNonQuery();
            }



            ShowMessage(
                "Session closed successfully."
            );


            BindSessions();
        }



        // =========================================================
        // HOST MODAL ERROR
        // =========================================================

        private void ShowHostError(
            string message)
        {
            lblHostError.Text =
                message;


            pnlHostError.Visible =
                true;


            hfOpenSessionModal.Value =
                "1";
        }



        // =========================================================
        // PAGE MESSAGE
        // =========================================================

        private void ShowMessage(
            string message)
        {
            lblMessage.Text =
                message;


            pnlMessage.Visible =
                true;
        }



        // =========================================================
        // SESSION MODEL
        // =========================================================

        public class SessionItem
        {
            public int SessionId
            {
                get;
                set;
            }


            public string SessionTitle
            {
                get;
                set;
            }


            public string JoinCode
            {
                get;
                set;
            }


            public DateTime CreatedAt
            {
                get;
                set;
            }


            public string QuizTitle
            {
                get;
                set;
            }


            public string SubjectName
            {
                get;
                set;
            }


            public int CompletedAttempts
            {
                get;
                set;
            }


            public int FullyGraded
            {
                get;
                set;
            }


            public int PendingReview
            {
                get;
                set;
            }


            public decimal? AverageScore
            {
                get;
                set;
            }


            public string AverageScoreText
            {
                get;
                set;
            }
        }
    }
}