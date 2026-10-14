using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace SPMaster.Student
{
    public partial class ViewQuiz :
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
        // QUIZ ID
        // =========================================================

        private int QuizId
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


            int quizId;


            if (
                !int.TryParse(
                    Request.QueryString["id"],
                    out quizId
                )
            )
            {
                Response.Redirect(
                    "~/Student/Quizzes.aspx"
                );

                return;
            }


            QuizId =
                quizId;


            if (!IsPostBack)
            {
                if (!LoadQuiz())
                {
                    Response.Redirect(
                        "~/Student/Quizzes.aspx"
                    );

                    return;
                }
            }
        }


        // =========================================================
        // LOAD QUIZ
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

                    s.SubjectName,

                    f.FormName,

                    u.FullName AS CreatorName,

                    COUNT(
                        qq.QuizQuestionId
                    ) AS TotalQuestions,

                    ISNULL(
                        SUM(
                            qq.MaxMarks
                        ),
                        0
                    ) AS TotalMarks,

                    SUM(
                        CASE

                            WHEN ques.QuestionType =
                                'MCQ'

                            THEN 1

                            ELSE 0

                        END
                    ) AS McqCount,

                    SUM(
                        CASE

                            WHEN ques.QuestionType <>
                                'MCQ'

                            THEN 1

                            ELSE 0

                        END
                    ) AS SubjectiveCount

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

                LEFT JOIN QuizQuestions qq
                    ON q.QuizId =
                       qq.QuizId

                LEFT JOIN Questions ques
                    ON qq.QuestionId =
                       ques.QuestionId

                WHERE
                    q.QuizId =
                        @QuizId

                    AND
                    q.IsPublished = 1

                    AND
                    q.IsArchived = 0

                GROUP BY
                    q.QuizId,
                    q.Title,
                    q.Description,
                    q.TimeLimitMinutes,
                    q.PassPercentage,

                    s.SubjectName,

                    f.FormName,

                    u.FullName;
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


                    using (
                        SqlDataReader reader =
                            command.ExecuteReader()
                    )
                    {
                        // =========================================
                        // QUIZ NOT FOUND
                        // =========================================

                        if (!reader.Read())
                        {
                            return false;
                        }


                        // =========================================
                        // DATABASE VALUES
                        // =========================================

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


                        string subjectName =
                            reader[
                                "SubjectName"
                            ].ToString();


                        string formName =
                            reader[
                                "FormName"
                            ].ToString();


                        string creatorName =
                            reader[
                                "CreatorName"
                            ].ToString();


                        int timeLimit =
                            Convert.ToInt32(
                                reader[
                                    "TimeLimitMinutes"
                                ]
                            );


                        int totalQuestions =
                            reader[
                                "TotalQuestions"
                            ] == DBNull.Value

                                ? 0

                                : Convert.ToInt32(
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


                        int mcqCount =
                            reader[
                                "McqCount"
                            ] == DBNull.Value

                                ? 0

                                : Convert.ToInt32(
                                    reader[
                                        "McqCount"
                                    ]
                                );


                        int subjectiveCount =
                            reader[
                                "SubjectiveCount"
                            ] == DBNull.Value

                                ? 0

                                : Convert.ToInt32(
                                    reader[
                                        "SubjectiveCount"
                                    ]
                                );


                        // =========================================
                        // EMPTY QUIZ SAFETY
                        // =========================================

                        if (
                            totalQuestions == 0
                        )
                        {
                            return false;
                        }


                        // =========================================
                        // CONTEXT
                        // =========================================

                        litQuizContext.Text =
                            Server.HtmlEncode(
                                subjectName
                            )
                            + " • "
                            + Server.HtmlEncode(
                                formName
                            );


                        // =========================================
                        // TITLE
                        // =========================================

                        litQuizTitle.Text =
                            Server.HtmlEncode(
                                title
                            );


                        // =========================================
                        // CREATOR
                        // =========================================

                        litCreatorName.Text =
                            Server.HtmlEncode(
                                creatorName
                            );


                        // =========================================
                        // QUESTION COUNT
                        // =========================================

                        litTotalQuestions.Text =
                            totalQuestions
                            + (
                                totalQuestions == 1

                                    ? " Question"

                                    : " Questions"
                            );


                        // =========================================
                        // MARKS
                        // =========================================

                        litTotalMarks.Text =
                            totalMarks.ToString(
                                "0.##"
                            )
                            + (
                                totalMarks == 1

                                    ? " Mark"

                                    : " Marks"
                            );


                        // =========================================
                        // DURATION
                        // =========================================

                        litDuration.Text =
                            timeLimit
                            + (
                                timeLimit == 1

                                    ? " Min"

                                    : " Mins"
                            );


                        litTimerMinutes.Text =
                            timeLimit + "-minute";


                        // =========================================
                        // FORMAT
                        // =========================================

                        if (
                            mcqCount > 0
                            &&
                            subjectiveCount > 0
                        )
                        {
                            litFormat.Text =
                                "Obj + Subj";
                        }
                        else if (
                            mcqCount > 0
                        )
                        {
                            litFormat.Text =
                                "Objective";
                        }
                        else
                        {
                            litFormat.Text =
                                "Subjective";
                        }


                        // =========================================
                        // DESCRIPTION / INSTRUCTIONS
                        // =========================================

                        if (
                            !string.IsNullOrWhiteSpace(
                                description
                            )
                        )
                        {
                            litQuizInstructions.Text =
                                Server.HtmlEncode(
                                    description
                                );


                            pnlQuizInstructions.Visible =
                                true;
                        }
                        else
                        {
                            // Still show the generated
                            // system instructions below.

                            litQuizInstructions.Text =
                                "";


                            pnlQuizInstructions.Visible =
                                true;
                        }


                        // =========================================
                        // MCQ INSTRUCTION
                        // =========================================

                        if (
                            mcqCount > 0
                        )
                        {
                            phMcqInstruction.Visible =
                                true;


                            litMcqInstruction.Text =
                                "This quiz contains "
                                + mcqCount
                                + (
                                    mcqCount == 1

                                        ? " single-choice objective question."

                                        : " single-choice objective questions."
                                );
                        }
                        else
                        {
                            phMcqInstruction.Visible =
                                false;
                        }


                        // =========================================
                        // SUBJECTIVE INSTRUCTION
                        // =========================================

                        if (
                            subjectiveCount > 0
                        )
                        {
                            phSubjectiveInstruction.Visible =
                                true;


                            litSubjectiveInstruction.Text =
                                "This quiz contains "
                                + subjectiveCount
                                + (
                                    subjectiveCount == 1

                                        ? " subjective question."

                                        : " subjective questions."
                                );
                        }
                        else
                        {
                            phSubjectiveInstruction.Visible =
                                false;
                        }


                        // =========================================
                        // START QUIZ URL
                        // =========================================

                        lnkStartQuiz.NavigateUrl =
                            ResolveUrl(
                                "~/Student/StartQuiz.aspx?quizId="
                                + QuizId
                            );


                        // =========================================
                        // SUCCESS
                        // =========================================

                        return true;
                    }
                }
            }
        }
    }
}