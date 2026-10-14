using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace SPMaster.Lecturer
{
    public partial class PreviewQuiz :
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
        // CURRENT QUIZ ID
        // =========================================================

        private int QuizId
        {
            get
            {
                int quizId;


                if (
                    int.TryParse(
                        Request.QueryString["id"],
                        out quizId
                    )
                )
                {
                    return quizId;
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
            // -----------------------------------------------------
            // User must be logged in
            // -----------------------------------------------------

            if (
                Session["UserId"] == null
            )
            {
                Response.Redirect(
                    "~/Login.aspx"
                );

                return;
            }


            // -----------------------------------------------------
            // Valid QuizId required
            // -----------------------------------------------------

            if (
                QuizId <= 0
            )
            {
                Response.Redirect(
                    "~/Lecturer/Quizzes.aspx"
                );

                return;
            }


            if (!IsPostBack)
            {
                bool quizLoaded =
                    LoadQuiz();


                if (!quizLoaded)
                {
                    Response.Redirect(
                        "~/Lecturer/Quizzes.aspx"
                    );

                    return;
                }


                LoadQuizQuestions();
            }
        }

        // =========================================================
        // LOAD QUIZ INFORMATION
        // =========================================================

        private bool LoadQuiz()
        {
            const string sql = @"
        SELECT
            q.QuizId,
            q.Title,
            q.Description,
            q.TimeLimitMinutes,
            q.PassPercentage,
            q.IsPublished,
            q.CreatedByUserId,

            s.SubjectName,
            f.FormName,

            u.FullName AS CreatorName,

            (
                SELECT COUNT(*)

                FROM QuizQuestions qq

                WHERE
                    qq.QuizId =
                        q.QuizId
            ) AS TotalQuestions,

            (
                SELECT
                    ISNULL(
                        SUM(
                            qq.MaxMarks
                        ),
                        0
                    )

                FROM QuizQuestions qq

                WHERE
                    qq.QuizId =
                        q.QuizId
            ) AS TotalMarks

        FROM Quizzes q

        INNER JOIN Subjects s
            ON q.SubjectId =
               s.SubjectId

        INNER JOIN Forms f
            ON q.FormId =
               f.FormId

        INNER JOIN Users u
            ON q.CreatedByUserId =
               u.UserId

        WHERE
            q.QuizId =
                @QuizId

            AND
            q.IsArchived = 0

            AND
            (
                q.CreatedByUserId =
                    @CurrentUserId

                OR

                q.IsPublished = 1
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
                        "@CurrentUserId",
                        SqlDbType.Int
                    ).Value =
                        CurrentUserId;


                    connection.Open();


                    using (
                        SqlDataReader reader =
                            command.ExecuteReader()
                    )
                    {
                        // =================================================
                        // QUIZ NOT FOUND / NO ACCESS
                        // =================================================

                        if (!reader.Read())
                        {
                            return false;
                        }


                        // =================================================
                        // BASIC QUIZ INFORMATION
                        // =================================================

                        string subjectName =
                            reader[
                                "SubjectName"
                            ].ToString();


                        string formName =
                            reader[
                                "FormName"
                            ].ToString();


                        string title =
                            reader[
                                "Title"
                            ].ToString();


                        string description =
                            reader[
                                "Description"
                            ] == DBNull.Value

                                ? ""

                                : reader[
                                    "Description"
                                ].ToString();


                        int timeLimit =
                            Convert.ToInt32(
                                reader[
                                    "TimeLimitMinutes"
                                ]
                            );


                        decimal passPercentage =
                            reader[
                                "PassPercentage"
                            ] == DBNull.Value

                                ? 0

                                : Convert.ToDecimal(
                                    reader[
                                        "PassPercentage"
                                    ]
                                );


                        int totalQuestions =
                            Convert.ToInt32(
                                reader[
                                    "TotalQuestions"
                                ]
                            );


                        decimal totalMarks =
                            reader[
                                "TotalMarks"
                            ] == DBNull.Value

                                ? 0

                                : Convert.ToDecimal(
                                    reader[
                                        "TotalMarks"
                                    ]
                                );


                        bool isPublished =
                            Convert.ToBoolean(
                                reader[
                                    "IsPublished"
                                ]
                            );


                        int createdByUserId =
                            Convert.ToInt32(
                                reader[
                                    "CreatedByUserId"
                                ]
                            );


                        bool isOwner =
                            createdByUserId ==
                            CurrentUserId;


                        // =================================================
                        // HEADER
                        // =================================================

                        litQuizContext.Text =
                            Server.HtmlEncode(
                                subjectName
                            )
                            + " • "
                            + Server.HtmlEncode(
                                formName
                            );


                        litQuizTitle.Text =
                            Server.HtmlEncode(
                                title
                            );


                        // =================================================
                        // SUMMARY
                        // =================================================

                        litTotalQuestions.Text =
                            totalQuestions
                                .ToString();


                        litTotalMarks.Text =
                            FormatMarks(
                                totalMarks
                            );


                        litTimeLimit.Text =
                            timeLimit
                            + (
                                timeLimit == 1

                                    ? " min"

                                    : " mins"
                            );


                        litPassPercentage.Text =
                            passPercentage
                                .ToString(
                                    "0.##"
                                )
                            + "%";


                        // =================================================
                        // INSTRUCTIONS
                        // =================================================

                        if (
                            !string.IsNullOrWhiteSpace(
                                description
                            )
                        )
                        {
                            lblQuizInstructions.Text =
                                description;


                            pnlQuizInstructions.Visible =
                                true;
                        }
                        else
                        {
                            pnlQuizInstructions.Visible =
                                false;
                        }


                        // =================================================
                        // BACK BUTTON DESTINATION
                        // =================================================

                        string source =
                            Request.QueryString["from"];


                        // -------------------------------------------------
                        // PUBLIC QUIZ LIBRARY
                        // -------------------------------------------------

                        if (
                            string.Equals(
                                source,
                                "public",
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                        {
                            lnkBack.NavigateUrl =
                                ResolveUrl(
                                    "~/Lecturer/Quizzes.aspx?tab=public"
                                );
                        }


                        // -------------------------------------------------
                        // CREATE / EDIT QUIZ
                        // -------------------------------------------------

                        else if (
                            string.Equals(
                                source,
                                "editor",
                                StringComparison.OrdinalIgnoreCase
                            )
                            &&
                            isOwner
                            &&
                            !isPublished
                        )
                        {
                            lnkBack.NavigateUrl =
                                ResolveUrl(
                                    "~/Lecturer/CreateQuiz.aspx?id="
                                    + QuizId
                                );
                        }


                        // -------------------------------------------------
                        // MY QUIZZES
                        // -------------------------------------------------

                        else
                        {
                            lnkBack.NavigateUrl =
                                ResolveUrl(
                                    "~/Lecturer/Quizzes.aspx"
                                );
                        }


                        // =================================================
                        // PUBLISH BUTTON BEHAVIOUR
                        // =================================================

                        if (
                            isOwner
                            &&
                            !isPublished
                        )
                        {
                            btnPublishQuiz.Visible =
                                true;


                            // =============================================
                            // EMPTY QUIZ - CANNOT PUBLISH
                            // =============================================

                            if (
                                totalQuestions == 0
                            )
                            {
                                btnPublishQuiz.Enabled =
                                    false;


                                btnPublishQuiz.CssClass =
                                    "publish-quiz-button "
                                    + "publish-quiz-button-disabled";


                                btnPublishQuiz.ToolTip =
                                    "Add at least one question before publishing.";


                                ShowError(
                                    "This quiz cannot be published because it does not contain any questions. "
                                    + "Please return to the Quiz Editor and add at least one question."
                                );
                            }


                            // =============================================
                            // QUIZ HAS QUESTIONS - CAN PUBLISH
                            // =============================================

                            else
                            {
                                btnPublishQuiz.Enabled =
                                    true;


                                btnPublishQuiz.CssClass =
                                    "publish-quiz-button";


                                btnPublishQuiz.ToolTip =
                                    "";
                            }
                        }
                        else
                        {
                            btnPublishQuiz.Visible =
                                false;
                        }


                        // =================================================
                        // QUIZ LOADED SUCCESSFULLY
                        // =================================================

                        return true;
                    }
                }
            }
        }


        // =========================================================
        // LOAD QUIZ QUESTIONS
        // =========================================================

        private void LoadQuizQuestions()
        {
            const string sql = @"
                SELECT
                    qq.QuizQuestionId,
                    qq.QuestionId,
                    qq.QuestionOrder,
                    qq.MaxMarks,

                    q.QuestionText,
                    q.QuestionType,
                    q.Difficulty,
                    q.MediaPath,

                    (
                        SELECT TOP 1
                            qo.OptionText

                        FROM QuestionOptions qo

                        WHERE
                            qo.QuestionId =
                                q.QuestionId

                            AND
                            qo.DisplayOrder = 1
                    ) AS OptionA,

                    (
                        SELECT TOP 1
                            qo.OptionText

                        FROM QuestionOptions qo

                        WHERE
                            qo.QuestionId =
                                q.QuestionId

                            AND
                            qo.DisplayOrder = 2
                    ) AS OptionB,

                    (
                        SELECT TOP 1
                            qo.OptionText

                        FROM QuestionOptions qo

                        WHERE
                            qo.QuestionId =
                                q.QuestionId

                            AND
                            qo.DisplayOrder = 3
                    ) AS OptionC,

                    (
                        SELECT TOP 1
                            qo.OptionText

                        FROM QuestionOptions qo

                        WHERE
                            qo.QuestionId =
                                q.QuestionId

                            AND
                            qo.DisplayOrder = 4
                    ) AS OptionD

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


            // -----------------------------------------------------
            // Add total question count for Repeater
            // -----------------------------------------------------

            table.Columns.Add(
                "TotalQuestions",
                typeof(int)
            );


            int totalQuestions =
                table.Rows.Count;


            foreach (
                DataRow row
                in table.Rows
            )
            {
                row[
                    "TotalQuestions"
                ] =
                    totalQuestions;
            }


            rptPreviewQuestions.DataSource =
                table;


            rptPreviewQuestions.DataBind();


            pnlNoPreviewQuestions.Visible =
                table.Rows.Count == 0;
        }


        // =========================================================
        // PUBLISH QUIZ BUTTON
        // =========================================================

        protected void btnPublishQuiz_Click(
            object sender,
            EventArgs e)
        {
            if (
                QuizId <= 0
            )
            {
                ShowError(
                    "Unable to identify the quiz."
                );

                return;
            }


            // -----------------------------------------------------
            // Make sure the quiz still contains questions
            // -----------------------------------------------------

            if (
                GetQuestionCount(
                    QuizId
                ) == 0
            )
            {
                ShowError(
                    "The quiz must contain at least one question before it can be published."
                );

                return;
            }


            const string sql = @"
                UPDATE Quizzes

                SET
                    IsPublished = 1

                WHERE
                    QuizId =
                        @QuizId

                    AND
                    CreatedByUserId =
                        @CurrentUserId

                    AND
                    IsPublished = 0

                    AND
                    IsArchived = 0;
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
                        "@CurrentUserId",
                        SqlDbType.Int
                    ).Value =
                        CurrentUserId;


                    connection.Open();


                    int rowsAffected =
                        command.ExecuteNonQuery();


                    if (
                        rowsAffected == 0
                    )
                    {
                        ShowError(
                            "This quiz could not be published. It may already be published or you may not have permission to publish it."
                        );

                        return;
                    }
                }
            }


            // -----------------------------------------------------
            // Return to Quiz Management
            // -----------------------------------------------------

            Response.Redirect(
                "~/Lecturer/Quizzes.aspx?published=1"
            );
        }


        // =========================================================
        // GET QUIZ QUESTION COUNT
        // =========================================================

        private int GetQuestionCount(
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
                        quizId;


                    connection.Open();


                    return Convert.ToInt32(
                        command.ExecuteScalar()
                    );
                }
            }
        }


        // =========================================================
        // QUESTION TYPE DISPLAY
        // =========================================================

        protected string GetQuestionTypeDisplay(
            object questionType)
        {
            string type =
                Convert.ToString(
                    questionType
                );


            if (
                type.Equals(
                    "MCQ",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return "Single Choice";
            }


            return "Subjective";
        }


        // =========================================================
        // CHECK VALUE
        // =========================================================

        protected bool HasValue(
            object value)
        {
            return !string.IsNullOrWhiteSpace(
                Convert.ToString(
                    value
                )
            );
        }


        // =========================================================
        // CHECK MCQ
        // =========================================================

        protected bool IsMcq(
            object questionType)
        {
            return string.Equals(
                Convert.ToString(
                    questionType
                ),
                "MCQ",
                StringComparison.OrdinalIgnoreCase
            );
        }


        // =========================================================
        // MEDIA URL
        // =========================================================

        protected string GetMediaUrl(
            object mediaPath)
        {
            string path =
                Convert.ToString(
                    mediaPath
                );


            if (
                string.IsNullOrWhiteSpace(
                    path
                )
            )
            {
                return "";
            }


            if (
                path.StartsWith(
                    "http://",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                path.StartsWith(
                    "https://",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return path;
            }


            if (
                path.StartsWith(
                    "~/"
                )
            )
            {
                return ResolveUrl(
                    path
                );
            }


            return ResolveUrl(
                "~/"
                + path.TrimStart('/')
            );
        }


        // =========================================================
        // FORMAT MARKS
        // =========================================================

        private string FormatMarks(
            decimal marks)
        {
            string markText =
                marks.ToString(
                    "0.##"
                );


            return markText
                + (
                    marks == 1
                        ? " Mark"
                        : " Marks"
                );
        }


        // =========================================================
        // ERROR MESSAGE
        // =========================================================

        private void ShowError(
            string message)
        {
            lblPreviewMessage.Text =
                message;


            pnlPreviewMessage.Visible =
                true;
        }
    }
}