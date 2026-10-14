using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;


namespace SPMaster.Student
{
    public partial class StartQuiz :
        System.Web.UI.Page
    {
        // =========================================================
        // CONNECTION
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
        // CURRENT QUIZ
        // =========================================================

        private int QuizId
        {
            get
            {
                return ViewState["QuizId"] == null
                    ? 0
                    : Convert.ToInt32(
                        ViewState["QuizId"]
                    );
            }

            set
            {
                ViewState["QuizId"] =
                    value;
            }
        }


        // =========================================================
        // CURRENT ATTEMPT
        // =========================================================

        private int AttemptId
        {
            get
            {
                return ViewState["AttemptId"] == null
                    ? 0
                    : Convert.ToInt32(
                        ViewState["AttemptId"]
                    );
            }

            set
            {
                ViewState["AttemptId"] =
                    value;
            }
        }


        // =========================================================
        // CURRENT QUESTION INDEX
        // =========================================================

        private int CurrentQuestionIndex
        {
            get
            {
                return ViewState[
                    "CurrentQuestionIndex"
                ] == null

                    ? 0

                    : Convert.ToInt32(
                        ViewState[
                            "CurrentQuestionIndex"
                        ]
                    );
            }

            set
            {
                ViewState[
                    "CurrentQuestionIndex"
                ] =
                    value;
            }
        }


        // =========================================================
        // CURRENT QUIZ QUESTION
        // =========================================================

        private int CurrentQuizQuestionId
        {
            get
            {
                return ViewState[
                    "CurrentQuizQuestionId"
                ] == null

                    ? 0

                    : Convert.ToInt32(
                        ViewState[
                            "CurrentQuizQuestionId"
                        ]
                    );
            }

            set
            {
                ViewState[
                    "CurrentQuizQuestionId"
                ] =
                    value;
            }
        }


        // =========================================================
        // CURRENT QUESTION TYPE
        // =========================================================

        private string CurrentQuestionType
        {
            get
            {
                return ViewState[
                    "CurrentQuestionType"
                ] == null

                    ? ""

                    : ViewState[
                        "CurrentQuestionType"
                    ].ToString();
            }

            set
            {
                ViewState[
                    "CurrentQuestionType"
                ] =
                    value;
            }
        }

        private bool AttemptExpired
        {
            get
            {
                return ViewState["AttemptExpired"] != null
                    &&
                    Convert.ToBoolean(
                        ViewState["AttemptExpired"]
                    );
            }

            set
            {
                ViewState["AttemptExpired"] =
                    value;
            }
        }


        // Used while binding MCQ options
        private int? SelectedOptionId
        {
            get;
            set;
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
                if (!InitializeAttempt())
                {
                    Response.Redirect(
                        "~/Student/Quizzes.aspx?tab=attempts"
                    );

                    return;
                }


                // =============================================
                // EXPIRED ATTEMPT
                // =============================================

                if (AttemptExpired)
                {
                    pnlQuizInterface.Visible =
                        false;


                    pnlExpiredAttempt.Visible =
                        true;


                    return;
                }


                // =============================================
                // VALID ATTEMPT
                // =============================================

                pnlQuizInterface.Visible =
                    true;


                pnlExpiredAttempt.Visible =
                    false;


                CurrentQuestionIndex =
                    0;


                BindQuizScreen();
            }
        }


        // =========================================================
        // INITIALIZE ATTEMPT
        // =========================================================

        private bool InitializeAttempt()
        {
            int attemptId;


            // -----------------------------------------------------
            // CONTINUE EXISTING ATTEMPT
            // -----------------------------------------------------

            if (
                int.TryParse(
                    Request.QueryString[
                        "attemptId"
                    ],
                    out attemptId
                )
            )
            {
                return LoadExistingAttempt(
                    attemptId
                );
            }


            int quizId;


            // -----------------------------------------------------
            // START QUIZ
            // -----------------------------------------------------

            if (
                !int.TryParse(
                    Request.QueryString[
                        "quizId"
                    ],
                    out quizId
                )
            )
            {
                return false;
            }


            QuizId =
                quizId;


            if (!QuizCanBeStarted())
            {
                return false;
            }


            // Check whether student already has
            // an unfinished attempt

            int existingAttemptId =
                GetExistingAttempt();


            if (
                existingAttemptId > 0
            )
            {
                AttemptId =
                    existingAttemptId;


                return true;
            }


            AttemptId =
                CreateAttempt();


            return AttemptId > 0;
        }


        // =========================================================
        // LOAD EXISTING ATTEMPT
        // =========================================================

        private bool LoadExistingAttempt(
            int attemptId)
        {
            const string sql = @"
                SELECT
                    AttemptId,
                    QuizId,
                    ExpiresAt,
                    AttemptStatus

                FROM QuizAttempts

                WHERE
                    AttemptId =
                        @AttemptId

                    AND
                    StudentUserId =
                        @StudentUserId;
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection
                        )
                )
                {
                    command.Parameters.Add(
                        "@AttemptId",
                        SqlDbType.Int
                    ).Value =
                        attemptId;


                    command.Parameters.Add(
                        "@StudentUserId",
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


                        string status =
                            reader[
                                "AttemptStatus"
                            ].ToString();


                        if (
                            !status.Equals(
                                "InProgress",
                                StringComparison
                                    .OrdinalIgnoreCase
                            )
                        )
                        {
                            return false;
                        }


                        AttemptId =
                            attemptId;


                        QuizId =
                            Convert.ToInt32(
                                reader[
                                    "QuizId"
                                ]
                            );


                        DateTime expiresAt =
                            Convert.ToDateTime(
                                reader[
                                    "ExpiresAt"
                                ]
                            );


                        if (
                            expiresAt <=
                            DateTime.Now
                        )
                        {
                            AttemptId =
                                attemptId;


                            QuizId =
                                Convert.ToInt32(
                                    reader["QuizId"]
                                );


                            AttemptExpired =
                                true;


                            return true;
                        }

                        AttemptExpired = false;

                        return true;
                    }
                }
            }
        }


        // =========================================================
        // QUIZ VALIDATION
        // =========================================================

        private bool QuizCanBeStarted()
        {
            const string sql = @"
                SELECT COUNT(*)

                FROM Quizzes q

                WHERE
                    q.QuizId =
                        @QuizId

                    AND
                    q.IsPublished = 1

                    AND
                    q.IsArchived = 0

                    AND EXISTS
                    (
                        SELECT 1

                        FROM QuizQuestions qq

                        WHERE
                            qq.QuizId =
                                q.QuizId
                    );
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
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
                        QuizId;


                    connection.Open();


                    return Convert.ToInt32(
                        command.ExecuteScalar()
                    ) > 0;
                }
            }
        }


        // =========================================================
        // FIND EXISTING IN-PROGRESS ATTEMPT
        // =========================================================

        private int GetExistingAttempt()
        {
            const string sql = @"
                SELECT TOP 1
                    AttemptId

                FROM QuizAttempts

                WHERE
                    QuizId =
                        @QuizId

                    AND
                    StudentUserId =
                        @StudentUserId

                    AND
                    AttemptStatus =
                        'InProgress'

                    AND
                    ExpiresAt >
                        GETDATE()

                ORDER BY
                    StartedAt DESC;
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
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
                        QuizId;


                    command.Parameters.Add(
                        "@StudentUserId",
                        SqlDbType.Int
                    ).Value =
                        CurrentUserId;


                    connection.Open();


                    object result =
                        command.ExecuteScalar();


                    return result == null
                        || result == DBNull.Value

                        ? 0

                        : Convert.ToInt32(
                            result
                        );
                }
            }
        }


        // =========================================================
        // CREATE ATTEMPT
        // =========================================================

        private int CreateAttempt()
        {
            const string sql = @"
                DECLARE @TimeLimit INT;


                SELECT
                    @TimeLimit =
                        TimeLimitMinutes

                FROM Quizzes

                WHERE
                    QuizId =
                        @QuizId;


                INSERT INTO QuizAttempts
                (
                    QuizId,
                    StudentUserId,
                    StartedAt,
                    ExpiresAt,
                    SubmittedAt,
                    AttemptStatus
                )

                VALUES
                (
                    @QuizId,
                    @StudentUserId,
                    GETDATE(),
                    DATEADD(
                        MINUTE,
                        @TimeLimit,
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
            {
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
                        QuizId;


                    command.Parameters.Add(
                        "@StudentUserId",
                        SqlDbType.Int
                    ).Value =
                        CurrentUserId;


                    connection.Open();


                    return Convert.ToInt32(
                        command.ExecuteScalar()
                    );
                }
            }
        }


        // =========================================================
        // BIND WHOLE QUIZ SCREEN
        // =========================================================

        private void BindQuizScreen()
        {
            BindQuizHeader();

            BindQuestionNavigation();

            BindCurrentQuestion();

            BindTimer();

            UpdateNavigationButtons();
        }


        // =========================================================
        // HEADER
        // =========================================================

        private void BindQuizHeader()
        {
            const string sql = @"
                SELECT
                    Title

                FROM Quizzes

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
            {
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
                        QuizId;


                    connection.Open();


                    object title =
                        command.ExecuteScalar();


                    litQuizTitle.Text =
                        Server.HtmlEncode(
                            Convert.ToString(
                                title
                            )
                        );
                }
            }
        }


        // =========================================================
        // LOAD QUIZ QUESTIONS
        // =========================================================

        private DataTable GetQuizQuestions()
        {
            const string sql = @"
                SELECT
                    qq.QuizQuestionId,
                    qq.QuestionId,
                    qq.QuestionOrder,
                    qq.MaxMarks,

                    q.QuestionText,
                    q.QuestionType,
                    q.MediaPath

                FROM QuizQuestions qq

                INNER JOIN Questions q
                    ON qq.QuestionId =
                       q.QuestionId

                WHERE
                    qq.QuizId =
                        @QuizId

                ORDER BY
                    qq.QuestionOrder;
            ";


            DataTable table =
                new DataTable();


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
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
                        QuizId;


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
            }


            return table;
        }


        // =========================================================
        // QUESTION NAVIGATION
        // =========================================================

        private void BindQuestionNavigation()
        {
            const string sql = @"
                SELECT
                    ROW_NUMBER()
                    OVER
                    (
                        ORDER BY
                            qq.QuestionOrder
                    ) - 1
                        AS QuestionIndex,

                    ROW_NUMBER()
                    OVER
                    (
                        ORDER BY
                            qq.QuestionOrder
                    )
                        AS QuestionNumber,

                    CASE
                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM StudentAnswers sa

                            WHERE
                                sa.AttemptId =
                                    @AttemptId

                                AND
                                sa.QuizQuestionId =
                                    qq.QuizQuestionId

                                AND
                                (
                                    sa.SelectedOptionId
                                        IS NOT NULL

                                    OR

                                    LTRIM(
                                        RTRIM(
                                            ISNULL(
                                                sa.AnswerText,
                                                ''
                                            )
                                        )
                                    ) <> ''
                                )
                        )

                        THEN CAST(
                            1 AS BIT
                        )

                        ELSE CAST(
                            0 AS BIT
                        )

                    END AS IsAnswered

                FROM QuizQuestions qq

                WHERE
                    qq.QuizId =
                        @QuizId

                ORDER BY
                    qq.QuestionOrder;
            ";


            DataTable table =
                new DataTable();


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection
                        )
                )
                {
                    command.Parameters.Add(
                        "@AttemptId",
                        SqlDbType.Int
                    ).Value =
                        AttemptId;


                    command.Parameters.Add(
                        "@QuizId",
                        SqlDbType.Int
                    ).Value =
                        QuizId;


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
            }


            rptQuestionNavigation.DataSource =
                table;


            rptQuestionNavigation.DataBind();
        }


        // =========================================================
        // CURRENT QUESTION
        // =========================================================

        private void BindCurrentQuestion()
        {
            DataTable questions =
                GetQuizQuestions();


            if (
                questions.Rows.Count == 0
            )
            {
                Response.Redirect(
                    "~/Student/Quizzes.aspx"
                );

                return;
            }


            if (
                CurrentQuestionIndex < 0
            )
            {
                CurrentQuestionIndex =
                    0;
            }


            if (
                CurrentQuestionIndex >=
                questions.Rows.Count
            )
            {
                CurrentQuestionIndex =
                    questions.Rows.Count - 1;
            }


            DataRow row =
                questions.Rows[
                    CurrentQuestionIndex
                ];


            int questionId =
                Convert.ToInt32(
                    row[
                        "QuestionId"
                    ]
                );


            CurrentQuizQuestionId =
                Convert.ToInt32(
                    row[
                        "QuizQuestionId"
                    ]
                );


            CurrentQuestionType =
                row[
                    "QuestionType"
                ].ToString();


            bool isMcq =
                CurrentQuestionType.Equals(
                    "MCQ",
                    StringComparison
                        .OrdinalIgnoreCase
                );


            // -----------------------------------------------------
            // QUESTION COUNT
            // -----------------------------------------------------

            litCurrentQuestion.Text =
                "Question "
                + (
                    CurrentQuestionIndex
                    + 1
                )
                + " of "
                + questions.Rows.Count;


            // -----------------------------------------------------
            // TYPE
            // -----------------------------------------------------

            litQuestionType.Text =
                isMcq

                    ? "Single Choice • Section A"

                    : "Subjective • Section B";


            // -----------------------------------------------------
            // MARKS
            // -----------------------------------------------------

            decimal marks =
                Convert.ToDecimal(
                    row[
                        "MaxMarks"
                    ]
                );


            litQuestionMarks.Text =
                "+"
                + marks.ToString(
                    "0.##"
                )
                + (
                    marks == 1
                        ? " Mark"
                        : " Marks"
                );


            // -----------------------------------------------------
            // QUESTION
            // -----------------------------------------------------

            litQuestionText.Text =
                Server.HtmlEncode(
                    "Question "
                    + (
                        CurrentQuestionIndex
                        + 1
                    )
                    + ": "
                    + row[
                        "QuestionText"
                    ].ToString()
                );


            litQuestionInstruction.Text =
                isMcq

                    ? "Select one correct answer from the options below."

                    : "Write your answer clearly and include any relevant working or explanation.";


            // -----------------------------------------------------
            // IMAGE
            // -----------------------------------------------------

            string mediaPath =
                row[
                    "MediaPath"
                ] == DBNull.Value

                    ? ""

                    : row[
                        "MediaPath"
                    ].ToString();


            if (
                !string.IsNullOrWhiteSpace(
                    mediaPath
                )
            )
            {
                imgQuestion.ImageUrl =
                    GetMediaUrl(
                        mediaPath
                    );


                pnlQuestionImage.Visible =
                    true;
            }
            else
            {
                pnlQuestionImage.Visible =
                    false;
            }


            // -----------------------------------------------------
            // LOAD EXISTING ANSWER
            // -----------------------------------------------------

            object selectedOption;
            string answerText;


            LoadSavedAnswer(
                out selectedOption,
                out answerText
            );


            // -----------------------------------------------------
            // MCQ
            // -----------------------------------------------------

            if (isMcq)
            {
                phObjective.Visible =
                    true;


                phSubjective.Visible =
                    false;


                SelectedOptionId =
                    selectedOption == null
                    || selectedOption == DBNull.Value

                        ? (int?)null

                        : Convert.ToInt32(
                            selectedOption
                        );


                BindOptions(
                    questionId
                );
            }


            // -----------------------------------------------------
            // SUBJECTIVE
            // -----------------------------------------------------

            else
            {
                phObjective.Visible =
                    false;


                phSubjective.Visible =
                    true;


                txtSubjectiveAnswer.Text =
                    answerText;
            }
        }


        // =========================================================
        // LOAD OPTIONS
        // =========================================================

        private void BindOptions(
            int questionId)
        {
            const string sql = @"
                SELECT
                    OptionId,
                    OptionText,

                    CASE DisplayOrder
                        WHEN 1 THEN 'A'
                        WHEN 2 THEN 'B'
                        WHEN 3 THEN 'C'
                        WHEN 4 THEN 'D'
                        ELSE CAST(
                            DisplayOrder
                            AS VARCHAR(10)
                        )
                    END AS OptionLetter

                FROM QuestionOptions

                WHERE
                    QuestionId =
                        @QuestionId

                ORDER BY
                    DisplayOrder;
            ";


            DataTable table =
                new DataTable();


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection
                        )
                )
                {
                    command.Parameters.Add(
                        "@QuestionId",
                        SqlDbType.Int
                    ).Value =
                        questionId;


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
            }


            rptOptions.DataSource =
                table;


            rptOptions.DataBind();
        }


        // =========================================================
        // EXISTING ANSWER
        // =========================================================

        private void LoadSavedAnswer(
            out object selectedOptionId,
            out string answerText)
        {
            selectedOptionId =
                null;


            answerText =
                "";


            const string sql = @"
                SELECT
                    SelectedOptionId,
                    AnswerText

                FROM StudentAnswers

                WHERE
                    AttemptId =
                        @AttemptId

                    AND
                    QuizQuestionId =
                        @QuizQuestionId;
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection
                        )
                )
                {
                    command.Parameters.Add(
                        "@AttemptId",
                        SqlDbType.Int
                    ).Value =
                        AttemptId;


                    command.Parameters.Add(
                        "@QuizQuestionId",
                        SqlDbType.Int
                    ).Value =
                        CurrentQuizQuestionId;


                    connection.Open();


                    using (
                        SqlDataReader reader =
                            command.ExecuteReader()
                    )
                    {
                        if (!reader.Read())
                        {
                            return;
                        }


                        selectedOptionId =
                            reader[
                                "SelectedOptionId"
                            ];


                        answerText =
                            reader[
                                "AnswerText"
                            ] == DBNull.Value

                                ? ""

                                : reader[
                                    "AnswerText"
                                ].ToString();
                    }
                }
            }
        }


        // =========================================================
        // SAVE CURRENT ANSWER
        // =========================================================

        private void SaveCurrentAnswer()
        {
            if (
                CurrentQuizQuestionId <= 0
            )
            {
                return;
            }


            bool isMcq =
                CurrentQuestionType.Equals(
                    "MCQ",
                    StringComparison
                        .OrdinalIgnoreCase
                );


            int? selectedOptionId =
                null;


            string answerText =
                null;


            // -----------------------------------------------------
            // MCQ
            // -----------------------------------------------------

            if (isMcq)
            {
                int optionId;


                if (
                    int.TryParse(
                        Request.Form[
                            "objectiveAnswer"
                        ],
                        out optionId
                    )
                )
                {
                    selectedOptionId =
                        optionId;
                }
            }


            // -----------------------------------------------------
            // SUBJECTIVE
            // -----------------------------------------------------

            else
            {
                answerText =
                    txtSubjectiveAnswer
                        .Text
                        .Trim();
            }


            // -----------------------------------------------------
            // NO ANSWER
            // -----------------------------------------------------

            if (
                selectedOptionId == null
                &&
                string.IsNullOrWhiteSpace(
                    answerText
                )
            )
            {
                DeleteCurrentAnswer();

                return;
            }


            const string sql = @"
                IF EXISTS
                (
                    SELECT 1

                    FROM StudentAnswers

                    WHERE
                        AttemptId =
                            @AttemptId

                        AND
                        QuizQuestionId =
                            @QuizQuestionId
                )

                BEGIN

                    UPDATE StudentAnswers

                    SET
                        SelectedOptionId =
                            @SelectedOptionId,

                        AnswerText =
                            @AnswerText,

                        MarksAwarded =
                            NULL

                    WHERE
                        AttemptId =
                            @AttemptId

                        AND
                        QuizQuestionId =
                            @QuizQuestionId;

                END

                ELSE

                BEGIN

                    INSERT INTO StudentAnswers
                    (
                        AttemptId,
                        QuizQuestionId,
                        SelectedOptionId,
                        AnswerText,
                        MarksAwarded
                    )

                    VALUES
                    (
                        @AttemptId,
                        @QuizQuestionId,
                        @SelectedOptionId,
                        @AnswerText,
                        NULL
                    );

                END
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection
                        )
                )
                {
                    command.Parameters.Add(
                        "@AttemptId",
                        SqlDbType.Int
                    ).Value =
                        AttemptId;


                    command.Parameters.Add(
                        "@QuizQuestionId",
                        SqlDbType.Int
                    ).Value =
                        CurrentQuizQuestionId;


                    command.Parameters.Add(
                        "@SelectedOptionId",
                        SqlDbType.Int
                    ).Value =
                        selectedOptionId.HasValue

                            ? (object)
                                selectedOptionId.Value

                            : DBNull.Value;


                    command.Parameters.Add(
                        "@AnswerText",
                        SqlDbType.NVarChar,
                        -1
                    ).Value =
                        string.IsNullOrWhiteSpace(
                            answerText
                        )

                            ? (object)DBNull.Value

                            : answerText;


                    connection.Open();


                    command.ExecuteNonQuery();
                }
            }
        }


        // =========================================================
        // DELETE CURRENT ANSWER
        // =========================================================

        private void DeleteCurrentAnswer()
        {
            const string sql = @"
                DELETE FROM StudentAnswers

                WHERE
                    AttemptId =
                        @AttemptId

                    AND
                    QuizQuestionId =
                        @QuizQuestionId;
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection
                        )
                )
                {
                    command.Parameters.Add(
                        "@AttemptId",
                        SqlDbType.Int
                    ).Value =
                        AttemptId;


                    command.Parameters.Add(
                        "@QuizQuestionId",
                        SqlDbType.Int
                    ).Value =
                        CurrentQuizQuestionId;


                    connection.Open();


                    command.ExecuteNonQuery();
                }
            }
        }


        // =========================================================
        // PREVIOUS
        // =========================================================

        protected void btnPreviousQuestion_Click(
            object sender,
            EventArgs e)
        {
            SaveCurrentAnswer();


            if (
                CurrentQuestionIndex > 0
            )
            {
                CurrentQuestionIndex--;
            }


            BindQuizScreen();
        }


        // =========================================================
        // NEXT
        // =========================================================

        protected void btnNextQuestion_Click(
            object sender,
            EventArgs e)
        {
            SaveCurrentAnswer();


            int totalQuestions =
                GetQuestionCount();


            if (
                CurrentQuestionIndex <
                totalQuestions - 1
            )
            {
                CurrentQuestionIndex++;
            }


            BindQuizScreen();
        }


        // =========================================================
        // QUESTION NUMBER
        // =========================================================

        protected void btnGoQuestionServer_Click(
            object sender,
            EventArgs e)
        {
            SaveCurrentAnswer();


            int targetIndex;


            if (
                int.TryParse(
                    hfGoQuestionIndex.Value,
                    out targetIndex
                )
            )
            {
                int totalQuestions =
                    GetQuestionCount();


                if (
                    targetIndex >= 0
                    &&
                    targetIndex <
                    totalQuestions
                )
                {
                    CurrentQuestionIndex =
                        targetIndex;
                }
            }


            BindQuizScreen();
        }


        // =========================================================
        // CLEAR
        // =========================================================

        protected void btnClearAnswer_Click(
            object sender,
            EventArgs e)
        {
            DeleteCurrentAnswer();


            BindQuizScreen();
        }


        // =========================================================
        // EXIT
        // =========================================================

        protected void btnExitQuiz_Click(
            object sender,
            EventArgs e)
        {
            SaveCurrentAnswer();


            Response.Redirect(
                "~/Student/Quizzes.aspx?tab=attempts"
            );
        }


        // =========================================================
        // MANUAL SUBMIT
        // =========================================================

        protected void btnConfirmSubmit_Click(
            object sender,
            EventArgs e)
        {
            SaveCurrentAnswer();


            FinalizeAttempt();


            Response.Redirect(
                "~/Student/Quizzes.aspx?tab=attempts"
            );
        }


        // =========================================================
        // TIME UP
        // =========================================================

        protected void btnTimeUp_Click(
            object sender,
            EventArgs e)
        {
            SaveCurrentAnswer();


            FinalizeAttempt();


            Response.Redirect(
                "~/Student/Quizzes.aspx?tab=attempts"
            );
        }


        // =========================================================
        // FINALIZE ATTEMPT
        // =========================================================

        private void FinalizeAttempt()
        {
            AutoGradeMcq();


            bool hasSubjective =
                QuizHasSubjectiveQuestions();


            string finalStatus =
                hasSubjective

                    ? "PendingGrading"

                    : "Graded";


            const string sql = @"
                UPDATE QuizAttempts

                SET
                    SubmittedAt =
                        GETDATE(),

                    AttemptStatus =
                        @AttemptStatus

                WHERE
                    AttemptId =
                        @AttemptId

                    AND
                    StudentUserId =
                        @StudentUserId

                    AND
                    AttemptStatus =
                        'InProgress';
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection
                        )
                )
                {
                    command.Parameters.Add(
                        "@AttemptStatus",
                        SqlDbType.NVarChar,
                        50
                    ).Value =
                        finalStatus;


                    command.Parameters.Add(
                        "@AttemptId",
                        SqlDbType.Int
                    ).Value =
                        AttemptId;


                    command.Parameters.Add(
                        "@StudentUserId",
                        SqlDbType.Int
                    ).Value =
                        CurrentUserId;


                    connection.Open();


                    command.ExecuteNonQuery();
                }
            }
        }


        // =========================================================
        // AUTO-GRADE MCQ
        // =========================================================

        private void AutoGradeMcq()
        {
            const string sql = @"
                UPDATE sa

                SET
                    sa.MarksAwarded =
                        CASE
                            WHEN qo.IsCorrect = 1
                            THEN qq.MaxMarks
                            ELSE 0
                        END

                FROM StudentAnswers sa

                INNER JOIN QuizQuestions qq
                    ON sa.QuizQuestionId =
                       qq.QuizQuestionId

                INNER JOIN Questions q
                    ON qq.QuestionId =
                       q.QuestionId

                LEFT JOIN QuestionOptions qo
                    ON sa.SelectedOptionId =
                       qo.OptionId

                WHERE
                    sa.AttemptId =
                        @AttemptId

                    AND
                    q.QuestionType =
                        'MCQ';
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection
                        )
                )
                {
                    command.Parameters.Add(
                        "@AttemptId",
                        SqlDbType.Int
                    ).Value =
                        AttemptId;


                    connection.Open();


                    command.ExecuteNonQuery();
                }
            }
        }


        // =========================================================
        // HAS SUBJECTIVE
        // =========================================================

        private bool QuizHasSubjectiveQuestions()
        {
            const string sql = @"
                SELECT COUNT(*)

                FROM QuizQuestions qq

                INNER JOIN Questions q
                    ON qq.QuestionId =
                       q.QuestionId

                WHERE
                    qq.QuizId =
                        @QuizId

                    AND
                    q.QuestionType <>
                        'MCQ';
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
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
                        QuizId;


                    connection.Open();


                    return Convert.ToInt32(
                        command.ExecuteScalar()
                    ) > 0;
                }
            }
        }


        // =========================================================
        // TIMER
        // =========================================================

        private void BindTimer()
        {
            const string sql = @"
                SELECT
                    ExpiresAt

                FROM QuizAttempts

                WHERE
                    AttemptId =
                        @AttemptId

                    AND
                    StudentUserId =
                        @StudentUserId;
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection
                        )
                )
                {
                    command.Parameters.Add(
                        "@AttemptId",
                        SqlDbType.Int
                    ).Value =
                        AttemptId;


                    command.Parameters.Add(
                        "@StudentUserId",
                        SqlDbType.Int
                    ).Value =
                        CurrentUserId;


                    connection.Open();


                    DateTime expiresAt =
                        Convert.ToDateTime(
                            command.ExecuteScalar()
                        );


                    int remainingSeconds =
                        Convert.ToInt32(
                            Math.Ceiling(
                                (
                                    expiresAt
                                    -
                                    DateTime.Now
                                )
                                .TotalSeconds
                            )
                        );


                    if (
                        remainingSeconds < 0
                    )
                    {
                        remainingSeconds =
                            0;
                    }


                    hfRemainingSeconds.Value =
                        remainingSeconds
                            .ToString();
                }
            }
        }


        // =========================================================
        // QUESTION COUNT
        // =========================================================

        private int GetQuestionCount()
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
            {
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
                        QuizId;


                    connection.Open();


                    return Convert.ToInt32(
                        command.ExecuteScalar()
                    );
                }
            }
        }


        // =========================================================
        // BUTTON STATES
        // =========================================================

        private void UpdateNavigationButtons()
        {
            int total =
                GetQuestionCount();


            btnPreviousQuestion.Enabled =
                CurrentQuestionIndex > 0;


            btnNextQuestion.Enabled =
                CurrentQuestionIndex
                <
                total - 1;


            if (
                CurrentQuestionIndex ==
                total - 1
            )
            {
                btnNextQuestion.Text =
                    "Last Question";
            }
            else
            {
                btnNextQuestion.Text =
                    "Next";
            }
        }


        // =========================================================
        // OPTION CHECKED
        // =========================================================

        protected string GetOptionChecked(
            object optionId)
        {
            if (
                !SelectedOptionId.HasValue
            )
            {
                return "";
            }


            return SelectedOptionId.Value
                ==
                Convert.ToInt32(
                    optionId
                )

                    ? "checked=\"checked\""

                    : "";
        }


        // =========================================================
        // QUESTION NAV CSS
        // =========================================================

        protected string GetQuestionNavigationCss(
            object questionIndex,
            object isAnswered)
        {
            int index =
                Convert.ToInt32(
                    questionIndex
                );


            bool answered =
                Convert.ToBoolean(
                    isAnswered
                );


            string css =
                "question-number";


            if (
                index ==
                CurrentQuestionIndex
            )
            {
                css +=
                    " active";
            }
            else if (answered)
            {
                css +=
                    " answered";
            }


            return css;
        }


        // =========================================================
        // MEDIA URL
        // =========================================================

        protected string GetMediaUrl(
            string mediaPath)
        {
            if (
                string.IsNullOrWhiteSpace(
                    mediaPath
                )
            )
            {
                return "";
            }


            if (
                mediaPath.StartsWith(
                    "~"
                )
            )
            {
                return ResolveUrl(
                    mediaPath
                );
            }


            return ResolveUrl(
                "~/"
                +
                mediaPath.TrimStart(
                    '/'
                )
            );
        }

        protected void btnLeaveExpiredAttempt_Click(
            object sender,
            EventArgs e)
        {
            Response.Redirect(
                "~/Student/Quizzes.aspx?tab=attempts"
            );
        }


        protected void btnRedoExpiredAttempt_Click(
            object sender,
            EventArgs e)
        {
            int quizId =
                QuizId;


            DeleteExpiredAttempt();


            Response.Redirect(
                "~/Student/StartQuiz.aspx?quizId="
                + quizId
            );
        }

        private void DeleteExpiredAttempt()
        {
            const string verifySql = @"
        SELECT COUNT(*)

        FROM QuizAttempts

        WHERE
            AttemptId =
                @AttemptId

            AND
            StudentUserId =
                @StudentUserId

            AND
            AttemptStatus =
                'InProgress'

            AND
            ExpiresAt <=
                GETDATE();
    ";


            const string deleteAnswersSql = @"
        DELETE FROM StudentAnswers

        WHERE
            AttemptId =
                @AttemptId;
    ";


            const string deleteAttemptSql = @"
        DELETE FROM QuizAttempts

        WHERE
            AttemptId =
                @AttemptId

            AND
            StudentUserId =
                @StudentUserId

            AND
            AttemptStatus =
                'InProgress'

            AND
            ExpiresAt <=
                GETDATE();
    ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        ConnectionString
                    )
            )
            {
                connection.Open();


                using (
                    SqlTransaction transaction =
                        connection.BeginTransaction()
                )
                {
                    try
                    {
                        // =============================================
                        // VERIFY
                        // =============================================

                        using (
                            SqlCommand command =
                                new SqlCommand(
                                    verifySql,
                                    connection,
                                    transaction
                                )
                        )
                        {
                            command.Parameters.Add(
                                "@AttemptId",
                                SqlDbType.Int
                            ).Value =
                                AttemptId;


                            command.Parameters.Add(
                                "@StudentUserId",
                                SqlDbType.Int
                            ).Value =
                                CurrentUserId;


                            int count =
                                Convert.ToInt32(
                                    command.ExecuteScalar()
                                );


                            if (
                                count == 0
                            )
                            {
                                transaction.Rollback();

                                return;
                            }
                        }


                        // =============================================
                        // DELETE SAVED ANSWERS
                        // =============================================

                        using (
                            SqlCommand command =
                                new SqlCommand(
                                    deleteAnswersSql,
                                    connection,
                                    transaction
                                )
                        )
                        {
                            command.Parameters.Add(
                                "@AttemptId",
                                SqlDbType.Int
                            ).Value =
                                AttemptId;


                            command.ExecuteNonQuery();
                        }


                        // =============================================
                        // DELETE OLD ATTEMPT
                        // =============================================

                        using (
                            SqlCommand command =
                                new SqlCommand(
                                    deleteAttemptSql,
                                    connection,
                                    transaction
                                )
                        )
                        {
                            command.Parameters.Add(
                                "@AttemptId",
                                SqlDbType.Int
                            ).Value =
                                AttemptId;


                            command.Parameters.Add(
                                "@StudentUserId",
                                SqlDbType.Int
                            ).Value =
                                CurrentUserId;


                            command.ExecuteNonQuery();
                        }


                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();

                        throw;
                    }
                }
            }
        }

    }
}