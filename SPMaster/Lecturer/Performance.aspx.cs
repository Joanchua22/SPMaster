using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace SPMaster.Lecturer
{
    public partial class Performance :
        System.Web.UI.Page
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
                BindQuizDropdown();


                if (
                    ddlQuiz.Items.Count > 0
                )
                {
                    pnlNoQuizzes.Visible =
                        false;


                    pnlPerformance.Visible =
                        true;


                    LoadPerformanceData();
                }
                else
                {
                    pnlNoQuizzes.Visible =
                        true;


                    pnlPerformance.Visible =
                        false;
                }
            }
        }

        protected void ddlQuiz_SelectedIndexChanged(
            object sender,
            EventArgs e)    
                {
                    LoadPerformanceData();
                }



        // =========================================================
        // QUIZ DROPDOWN
        // =========================================================

        private void BindQuizDropdown()
        {
            const string sql = @"
                SELECT
                    q.QuizId,

                    q.Title
                    + ' ('
                    + RTRIM(s.SubjectName)
                    + ' • '
                    + f.FormName
                    + ')' AS DisplayName

                FROM Quizzes q

                INNER JOIN Subjects s
                    ON q.SubjectId =
                       s.SubjectId

                INNER JOIN Forms f
                    ON q.FormId =
                       f.FormId

                WHERE
                    q.CreatedByUserId =
                        @LecturerId

                    AND
                    q.IsPublished =
                        1

                    AND
                    q.IsArchived =
                        0

                ORDER BY
                    q.CreatedAt DESC;
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
            }


            ddlQuiz.DataSource =
                table;


            ddlQuiz.DataValueField =
                "QuizId";


            ddlQuiz.DataTextField =
                "DisplayName";


            ddlQuiz.DataBind();
        }


        // =========================================================
        // LOAD ALL ANALYTICS
        // =========================================================

        private void LoadPerformanceData()
        {
            int quizId;


            if (
                !int.TryParse(
                    ddlQuiz.SelectedValue,
                    out quizId
                )
            )
            {
                return;
            }


            if (
                !QuizBelongsToCurrentLecturer(
                    quizId
                )
            )
            {
                return;
            }


            PerformanceSummary summary =
                LoadSummary(
                    quizId
                );


            List<decimal> gradedScores =
                LoadGradedScores(
                    quizId
                );


            List<int> completionTimes =
                LoadCompletionTimes(
                    quizId
                );


            DisplaySummary(
                summary,
                gradedScores,
                completionTimes
            );


            DisplayGradeBands(
                gradedScores
            );


            BindQuestionPerformance(
                quizId
            );
        }



        // =========================================================
        // SECURITY CHECK
        // =========================================================

        private bool QuizBelongsToCurrentLecturer(
            int quizId)
        {
            const string sql = @"
                SELECT COUNT(*)

                FROM Quizzes

                WHERE
                    QuizId =
                        @QuizId

                    AND
                    CreatedByUserId =
                        @LecturerId

                    AND
                    IsArchived =
                        0;
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


                    command.Parameters.Add(
                        "@LecturerId",
                        SqlDbType.Int
                    ).Value =
                        CurrentUserId;


                    connection.Open();


                    return Convert.ToInt32(
                        command.ExecuteScalar()
                    ) > 0;
                }
            }
        }



        // =========================================================
        // SUMMARY DATA
        // =========================================================

        private PerformanceSummary LoadSummary(
            int quizId)
        {
            const string sql = @"
                SELECT
                    q.TimeLimitMinutes,

                    (
                        SELECT COUNT(*)

                        FROM QuizAttempts qa

                        WHERE
                            qa.QuizId =
                                q.QuizId

                            AND
                            qa.AttemptStatus IN
                            (
                                'Graded',
                                'PendingGrading'
                            )
                    ) AS TotalAttempts,


                    (
                        SELECT
                            COUNT(
                                DISTINCT qa.StudentUserId
                            )

                        FROM QuizAttempts qa

                        WHERE
                            qa.QuizId =
                                q.QuizId

                            AND
                            qa.AttemptStatus IN
                            (
                                'Graded',
                                'PendingGrading'
                            )
                    ) AS UniqueStudents,


                    (
                        SELECT COUNT(*)

                        FROM QuizAttempts qa

                        WHERE
                            qa.QuizId =
                                q.QuizId

                            AND
                            qa.AttemptStatus =
                                'Graded'
                    ) AS GradedAttempts,


                    (
                        SELECT COUNT(*)

                        FROM QuizAttempts qa

                        WHERE
                            qa.QuizId =
                                q.QuizId

                            AND
                            qa.AttemptStatus =
                                'PendingGrading'
                    ) AS PendingAttempts


                FROM Quizzes q

                WHERE
                    q.QuizId =
                        @QuizId

                    AND
                    q.CreatedByUserId =
                        @LecturerId;
            ";


            PerformanceSummary summary =
                new PerformanceSummary();


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


                    command.Parameters.Add(
                        "@LecturerId",
                        SqlDbType.Int
                    ).Value =
                        CurrentUserId;


                    connection.Open();


                    using (
                        SqlDataReader reader =
                            command.ExecuteReader()
                    )
                    {
                        if (reader.Read())
                        {
                            summary.TotalAttempts =
                                Convert.ToInt32(
                                    reader[
                                        "TotalAttempts"
                                    ]
                                );


                            summary.UniqueStudents =
                                Convert.ToInt32(
                                    reader[
                                        "UniqueStudents"
                                    ]
                                );


                            summary.GradedAttempts =
                                Convert.ToInt32(
                                    reader[
                                        "GradedAttempts"
                                    ]
                                );


                            summary.PendingAttempts =
                                Convert.ToInt32(
                                    reader[
                                        "PendingAttempts"
                                    ]
                                );


                            summary.TimeLimitMinutes =
                                Convert.ToInt32(
                                    reader[
                                        "TimeLimitMinutes"
                                    ]
                                );
                        }
                    }
                }
            }


            return summary;
        }



        // =========================================================
        // GRADED SCORES
        // =========================================================

        private List<decimal> LoadGradedScores(
            int quizId)
        {
            const string sql = @"
                SELECT
                    qa.AttemptId,

                    CASE

                        WHEN totals.TotalMarks > 0
                        THEN
                            (
                                ISNULL(
                                    SUM(
                                        ISNULL(
                                            sa.MarksAwarded,
                                            0
                                        )
                                    ),
                                    0
                                )
                                * 100.0
                                /
                                totals.TotalMarks
                            )

                        ELSE 0

                    END AS Percentage


                FROM QuizAttempts qa


                CROSS APPLY
                (
                    SELECT
                        ISNULL(
                            SUM(
                                qq.MaxMarks
                            ),
                            0
                        ) AS TotalMarks

                    FROM QuizQuestions qq

                    WHERE
                        qq.QuizId =
                            @QuizId

                ) totals


                LEFT JOIN StudentAnswers sa
                    ON sa.AttemptId =
                       qa.AttemptId


                WHERE
                    qa.QuizId =
                        @QuizId

                    AND
                    qa.AttemptStatus =
                        'Graded'


                GROUP BY
                    qa.AttemptId,
                    totals.TotalMarks;
            ";


            List<decimal> scores =
                new List<decimal>();


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


                    using (
                        SqlDataReader reader =
                            command.ExecuteReader()
                    )
                    {
                        while (
                            reader.Read()
                        )
                        {
                            scores.Add(
                                Convert.ToDecimal(
                                    reader[
                                        "Percentage"
                                    ]
                                )
                            );
                        }
                    }
                }
            }


            return scores;
        }



        // =========================================================
        // COMPLETION TIMES
        // =========================================================

        private List<int> LoadCompletionTimes(
            int quizId)
        {
            const string sql = @"
                SELECT
                    DATEDIFF(
                        SECOND,
                        StartedAt,

                        CASE

                            /* -------------------------------------
                               Submitted after timer expired
                               ------------------------------------- */

                            WHEN SubmittedAt > ExpiresAt
                            THEN ExpiresAt


                            /* -------------------------------------
                               Normal submission
                               ------------------------------------- */

                            ELSE SubmittedAt

                        END

                    ) AS CompletionSeconds

                FROM QuizAttempts

                WHERE
                    QuizId =
                        @QuizId

                    AND
                    SubmittedAt IS NOT NULL

                    AND
                    AttemptStatus IN
                    (
                        'Graded',
                        'PendingGrading'
                    )

                ORDER BY
                    CompletionSeconds;
            ";


                    List<int> times =
                        new List<int>();


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


                            using (
                                SqlDataReader reader =
                                    command.ExecuteReader()
                            )
                            {
                                while (
                                    reader.Read()
                                )
                                {
                                    if (
                                        reader[
                                            "CompletionSeconds"
                                        ] == DBNull.Value
                                    )
                                    {
                                        continue;
                                    }


                                    int seconds =
                                        Convert.ToInt32(
                                            reader[
                                                "CompletionSeconds"
                                            ]
                                        );


                                    if (
                                        seconds >= 0
                                    )
                                    {
                                        times.Add(
                                            seconds
                                        );
                                    }
                                }
                            }
                        }
                    }


                    return times;
                }



        // =========================================================
        // DISPLAY SUMMARY
        // =========================================================

        private void DisplaySummary(
            PerformanceSummary summary,
            List<decimal> gradedScores,
            List<int> completionTimes)
        {
            // =============================================
            // TOTAL ATTEMPTS
            // =============================================

            litTotalAttempts.Text =
                summary.TotalAttempts
                    .ToString();


            litUniqueStudents.Text =
                summary.UniqueStudents
                + (
                    summary.UniqueStudents == 1

                        ? " student participated"

                        : " students participated"
                );


            // =============================================
            // GRADED SUBMISSIONS
            // =============================================

            litGradedSubmissions.Text =
                summary.GradedAttempts
                + " / "
                + summary.TotalAttempts;


            litPendingSubmissions.Text =
                summary.PendingAttempts
                + (
                    summary.PendingAttempts == 1

                        ? " awaiting subjective evaluation"

                        : " awaiting subjective evaluation"
                );


            // =============================================
            // AVERAGE SCORE
            // =============================================

            if (
                gradedScores.Count > 0
            )
            {
                decimal average =
                    gradedScores.Average();


                litAverageScore.Text =
                    average.ToString(
                        "0.0"
                    )
                    + "%";


                litAverageDescription.Text =
                    "Calculated from "
                    + gradedScores.Count
                    + (
                        gradedScores.Count == 1

                            ? " graded paper"

                            : " graded papers"
                    );
            }
            else
            {
                litAverageScore.Text =
                    "—";


                litAverageDescription.Text =
                    "No fully graded papers yet";
            }


            // =============================================
            // MEDIAN COMPLETION
            // =============================================

            if (
                completionTimes.Count > 0
            )
            {
                double medianSeconds =
                    GetMedian(
                        completionTimes
                    );


                int medianMinutes =
                    (int)Math.Round(
                        medianSeconds / 60.0,
                        MidpointRounding
                            .AwayFromZero
                    );


                litMedianCompletion.Text =
                    medianMinutes
                    + (
                        medianMinutes == 1

                            ? " Min"

                            : " Mins"
                    );
            }
            else
            {
                litMedianCompletion.Text =
                    "—";
            }


            litAllocatedTime.Text =
                "Allocated limit: "
                + summary.TimeLimitMinutes
                + (
                    summary.TimeLimitMinutes == 1

                        ? " Min"

                        : " Mins"
                );
        }



        // =========================================================
        // MEDIAN
        // =========================================================

        private double GetMedian(
            List<int> values)
        {
            if (
                values == null
                ||
                values.Count == 0
            )
            {
                return 0;
            }


            List<int> sorted =
                values
                    .OrderBy(
                        value => value
                    )
                    .ToList();


            int middle =
                sorted.Count / 2;


            if (
                sorted.Count % 2 == 1
            )
            {
                return sorted[
                    middle
                ];
            }


            return
                (
                    sorted[middle - 1]
                    +
                    sorted[middle]
                )
                / 2.0;
        }



        // =========================================================
        // GRADE BAND DISTRIBUTION
        // =========================================================

        private void DisplayGradeBands(
            List<decimal> scores)
        {
            int total =
                scores.Count;


            int gradeA =
                scores.Count(
                    score =>
                        score >= 75
                );


            int gradeB =
                scores.Count(
                    score =>
                        score >= 60
                        &&
                        score < 75
                );


            int gradeC =
                scores.Count(
                    score =>
                        score >= 40
                        &&
                        score < 60
                );


            int gradeDE =
                scores.Count(
                    score =>
                        score < 40
                );


            SetGradeBand(
                litGradeA,
                barGradeA,
                gradeA,
                total
            );


            SetGradeBand(
                litGradeB,
                barGradeB,
                gradeB,
                total
            );


            SetGradeBand(
                litGradeC,
                barGradeC,
                gradeC,
                total
            );


            SetGradeBand(
                litGradeDE,
                barGradeDE,
                gradeDE,
                total
            );
        }



        // =========================================================
        // GRADE BAND HELPER
        // =========================================================

        private void SetGradeBand(
            System.Web.UI.WebControls.Literal literal,
            System.Web.UI.HtmlControls.HtmlGenericControl bar,
            int count,
            int total)
        {
            decimal percentage =
                0;


            if (
                total > 0
            )
            {
                percentage =
                    (
                        decimal
                    )
                    count
                    /
                    total
                    *
                    100;
            }


            literal.Text =
                count
                + (
                    count == 1

                        ? " Student"

                        : " Students"
                )
                + " ("
                + percentage.ToString(
                    "0"
                )
                + "%)";


            bar.Style[
                "width"
            ] =
                percentage.ToString(
                    "0.##"
                )
                + "%";
        }



        // =========================================================
        // QUESTION PERFORMANCE
        // =========================================================

        private void BindQuestionPerformance(
            int quizId)
        {
            const string sql = @"
                WITH SubmittedAttempts AS
                (
                    SELECT
                        AttemptId

                    FROM QuizAttempts

                    WHERE
                        QuizId =
                            @QuizId

                        AND
                        AttemptStatus IN
                        (
                            'Graded',
                            'PendingGrading'
                        )
                )


                SELECT
                    qq.QuizQuestionId,
                    qq.QuestionOrder,
                    qq.MaxMarks,

                    q.QuestionText,
                    q.QuestionType,


                    CASE

                        /* -------------------------------------
                           MCQ:
                           Correct responses /
                           all submitted attempts
                           ------------------------------------- */

                        WHEN q.QuestionType =
                            'MCQ'

                        THEN
                            CASE

                                WHEN
                                    COUNT(
                                        DISTINCT attempts.AttemptId
                                    ) = 0

                                THEN NULL

                                ELSE
                                    (
                                        SUM(
                                            CASE

                                                WHEN
                                                    optionSelected.IsCorrect = 1

                                                THEN 1

                                                ELSE 0

                                            END
                                        )
                                        * 100.0
                                        /
                                        COUNT(
                                            DISTINCT attempts.AttemptId
                                        )
                                    )

                            END


                        /* -------------------------------------
                           SUBJECTIVE:
                           Average awarded percentage
                           of answers already graded
                           ------------------------------------- */

                        ELSE
                            AVG(
                                CASE

                                    WHEN
                                        sa.MarksAwarded IS NOT NULL

                                        AND
                                        qq.MaxMarks > 0

                                    THEN
                                        (
                                            sa.MarksAwarded
                                            * 100.0
                                            /
                                            qq.MaxMarks
                                        )

                                    ELSE NULL

                                END
                            )

                    END AS PerformanceRate


                FROM QuizQuestions qq


                INNER JOIN Questions q
                    ON qq.QuestionId =
                       q.QuestionId


                LEFT JOIN SubmittedAttempts attempts
                    ON 1 = 1


                LEFT JOIN StudentAnswers sa
                    ON sa.AttemptId =
                       attempts.AttemptId

                    AND
                    sa.QuizQuestionId =
                       qq.QuizQuestionId


                LEFT JOIN QuestionOptions optionSelected
                    ON sa.SelectedOptionId =
                       optionSelected.OptionId


                WHERE
                    qq.QuizId =
                        @QuizId


                GROUP BY
                    qq.QuizQuestionId,
                    qq.QuestionOrder,
                    qq.MaxMarks,

                    q.QuestionText,
                    q.QuestionType


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
                        quizId;


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


            litQuestionCount.Text =
                table.Rows.Count
                + (
                    table.Rows.Count == 1

                        ? " Question"

                        : " Questions"
                );


            pnlNoQuestionPerformance.Visible =
                table.Rows.Count == 0;


            rptQuestionPerformance.Visible =
                table.Rows.Count > 0;


            rptQuestionPerformance.DataSource =
                table;


            rptQuestionPerformance.DataBind();
        }



        // =========================================================
        // QUESTION DISPLAY
        // =========================================================

        protected string GetQuestionDisplay(
            object questionOrder,
            object questionText)
        {
            return
                "Q"
                + Convert.ToString(
                    questionOrder
                )
                + ": "
                + HttpUtility.HtmlEncode(
                    Convert.ToString(
                        questionText
                    )
                );
        }



        // =========================================================
        // QUESTION TYPE
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
                    StringComparison
                        .OrdinalIgnoreCase
                )
            )
            {
                return "MCQ";
            }


            return "Subjective";
        }



        // =========================================================
        // RATE TEXT
        // =========================================================

        protected string GetRateText(
            object questionType,
            object rate)
        {
            if (
                rate == null
                ||
                rate == DBNull.Value
            )
            {
                return "-";
            }


            decimal percentage =
                Convert.ToDecimal(
                    rate
                );


            bool isMcq =
                Convert.ToString(
                    questionType
                )
                .Equals(
                    "MCQ",
                    StringComparison
                        .OrdinalIgnoreCase
                );


            return percentage.ToString(
                "0"
            )
            + "%"
            + (
                isMcq

                    ? ""

                    : " Avg"
            );
        }



        // =========================================================
        // RATE CSS
        // =========================================================

        protected string GetRateCss(
            object rate)
        {
            if (
                rate == null
                ||
                rate == DBNull.Value
            )
            {
                return
                    "question-rate "
                    + "question-rate-empty";
            }


            decimal percentage =
                Convert.ToDecimal(
                    rate
                );


            if (
                percentage >= 75
            )
            {
                return
                    "question-rate "
                    + "question-rate-good";
            }


            if (
                percentage < 60
            )
            {
                return
                    "question-rate "
                    + "question-rate-review";
            }


            return "question-rate";
        }



        // =========================================================
        // ATTRIBUTE ENCODE
        // =========================================================

        protected string AttributeEncode(
            object value)
        {
            return HttpUtility
                .HtmlAttributeEncode(
                    Convert.ToString(
                        value
                    )
                );
        }



        // =========================================================
        // SUMMARY MODEL
        // =========================================================

        private class PerformanceSummary
        {
            public int TotalAttempts
            {
                get;
                set;
            }


            public int UniqueStudents
            {
                get;
                set;
            }


            public int GradedAttempts
            {
                get;
                set;
            }


            public int PendingAttempts
            {
                get;
                set;
            }


            public int TimeLimitMinutes
            {
                get;
                set;
            }
        }
    }
}