using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;


namespace SPMaster.Student
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
        // CURRENT STUDENT USER ID
        //
        // Login.aspx stores UserId inside Session["UserId"].
        // This allows every student to see only their own
        // performance records.
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

            // -----------------------------------------
            // USER MUST BE LOGGED IN
            // -----------------------------------------

            if (
                Session["UserId"] == null
            )
            {
                Response.Redirect(
                    "~/Login.aspx"
                );

                return;
            }



            // -----------------------------------------
            // STUDENT ROLE ONLY
            // -----------------------------------------

            if (
                Session["Role"] == null
                ||
                !Session["Role"]
                    .ToString()
                    .Trim()
                    .Equals(
                        "student",
                        StringComparison
                            .OrdinalIgnoreCase
                    )
            )
            {
                Response.Redirect(
                    "~/Login.aspx"
                );

                return;
            }



            // -----------------------------------------
            // FIRST PAGE LOAD
            // -----------------------------------------

            if (!IsPostBack)
            {
                LoadSubjectFilter();

                LoadPerformanceDashboard();
            }
        }



        // =========================================================
        // LOAD SUBJECT FILTER
        //
        // Only displays subjects where the current student
        // has at least one submitted quiz attempt.
        // =========================================================

        private void LoadSubjectFilter()
        {
            const string sql = @"
                SELECT DISTINCT
                    S.SubjectId,

                    RTRIM(
                        S.SubjectName
                    ) AS SubjectName

                FROM QuizAttempts QA

                INNER JOIN Quizzes QZ
                    ON QA.QuizId =
                       QZ.QuizId

                INNER JOIN Subjects S
                    ON QZ.SubjectId =
                       S.SubjectId

                WHERE
                    QA.StudentUserId =
                        @StudentUserId

                    AND
                    QA.SubmittedAt
                        IS NOT NULL

                ORDER BY
                    SubjectName;
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
                    ddlSubject.DataSource =
                        reader;


                    ddlSubject.DataTextField =
                        "SubjectName";


                    ddlSubject.DataValueField =
                        "SubjectId";


                    ddlSubject.DataBind();
                }
            }



            // -----------------------------------------
            // DEFAULT OPTION
            // -----------------------------------------

            ddlSubject.Items.Insert(
                0,
                new ListItem(
                    "All Subjects",
                    "0"
                )
            );
        }



        // =========================================================
        // FILTER EVENTS
        // =========================================================

        protected void ddlSubject_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            LoadPerformanceDashboard();
        }



        protected void ddlPeriod_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            LoadPerformanceDashboard();
        }



        // =========================================================
        // LOAD COMPLETE PERFORMANCE PAGE
        // =========================================================

        private void LoadPerformanceDashboard()
        {
            int subjectId =
                GetSelectedSubjectId();


            DateTime? fromDate =
                GetSelectedFromDate();



            LoadCompletedAttempts(
                subjectId,
                fromDate
            );


            LoadAverageScore(
                subjectId,
                fromDate
            );


            LoadPendingReviews(
                subjectId,
                fromDate
            );


            LoadScoreTrend(
                subjectId,
                fromDate
            );


            LoadSubjectMastery(
                subjectId,
                fromDate
            );
        }



        // =========================================================
        // GET SELECTED SUBJECT
        // =========================================================

        private int GetSelectedSubjectId()
        {
            int subjectId;


            if (
                !int.TryParse(
                    ddlSubject.SelectedValue,
                    out subjectId
                )
            )
            {
                subjectId = 0;
            }


            return subjectId;
        }



        // =========================================================
        // GET SELECTED TIME PERIOD
        // =========================================================

        private DateTime? GetSelectedFromDate()
        {
            switch (
                ddlPeriod.SelectedValue
            )
            {
                case "1m":

                    return DateTime.Today
                        .AddMonths(-1);


                case "3m":

                    return DateTime.Today
                        .AddMonths(-3);


                case "6m":

                    return DateTime.Today
                        .AddMonths(-6);


                default:

                    // All Time
                    return null;
            }
        }



        // =========================================================
        // ADD COMMON SQL PARAMETERS
        // =========================================================

        private void AddFilterParameters(
            SqlCommand command,
            int subjectId,
            DateTime? fromDate)
        {
            command.Parameters.Add(
                "@StudentUserId",
                SqlDbType.Int
            ).Value =
                CurrentStudentUserId;



            command.Parameters.Add(
                "@SubjectId",
                SqlDbType.Int
            ).Value =
                subjectId;



            SqlParameter fromDateParameter =
                command.Parameters.Add(
                    "@FromDate",
                    SqlDbType.DateTime2
                );


            if (fromDate.HasValue)
            {
                fromDateParameter.Value =
                    fromDate.Value;
            }
            else
            {
                fromDateParameter.Value =
                    DBNull.Value;
            }
        }



        // =========================================================
        // COMPLETED ATTEMPTS
        //
        // A quiz is considered completed once SubmittedAt
        // contains a value.
        // =========================================================

        private void LoadCompletedAttempts(
            int subjectId,
            DateTime? fromDate)
        {
            const string sql = @"
                SELECT
                    COUNT(*)

                FROM QuizAttempts QA

                INNER JOIN Quizzes QZ
                    ON QA.QuizId =
                       QZ.QuizId

                WHERE
                    QA.StudentUserId =
                        @StudentUserId

                    AND
                    QA.SubmittedAt
                        IS NOT NULL

                    AND
                    (
                        @SubjectId = 0

                        OR

                        QZ.SubjectId =
                            @SubjectId
                    )

                    AND
                    (
                        @FromDate IS NULL

                        OR

                        QA.SubmittedAt >=
                            @FromDate
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
                AddFilterParameters(
                    command,
                    subjectId,
                    fromDate
                );


                connection.Open();


                int completedAttempts =
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    );


                lblCompletedAttempts.Text =
                    completedAttempts
                        .ToString();
            }
        }



        // =========================================================
        // AVERAGE GRADED SCORE
        //
        // Score calculation:
        //
        // SUM(StudentAnswers.MarksAwarded)
        // -------------------------------- × 100
        // SUM(QuizQuestions.MaxMarks)
        //
        // Only fully graded attempts are included.
        //
        // This prevents an unanswered lecturer grading task
        // from incorrectly appearing as zero marks.
        // =========================================================

        private void LoadAverageScore(
            int subjectId,
            DateTime? fromDate)
        {
            const string sql = @"
                WITH GradedAttempts AS
                (
                    SELECT
                        QA.AttemptId,

                        CAST(
                            SUM(
                                ISNULL(
                                    SA.MarksAwarded,
                                    0
                                )
                            )
                            * 100.0
                            /
                            NULLIF(
                                SUM(
                                    QQ.MaxMarks
                                ),
                                0
                            )

                            AS DECIMAL(6,2)
                        )
                        AS AttemptScore

                    FROM QuizAttempts QA

                    INNER JOIN Quizzes QZ
                        ON QA.QuizId =
                           QZ.QuizId

                    INNER JOIN QuizQuestions QQ
                        ON QA.QuizId =
                           QQ.QuizId

                    LEFT JOIN StudentAnswers SA
                        ON QA.AttemptId =
                           SA.AttemptId

                        AND
                        QQ.QuizQuestionId =
                            SA.QuizQuestionId

                    WHERE
                        QA.StudentUserId =
                            @StudentUserId

                        AND
                        QA.SubmittedAt
                            IS NOT NULL

                        AND
                        (
                            @SubjectId = 0

                            OR

                            QZ.SubjectId =
                                @SubjectId
                        )

                        AND
                        (
                            @FromDate IS NULL

                            OR

                            QA.SubmittedAt >=
                                @FromDate
                        )

                    GROUP BY
                        QA.AttemptId

                    HAVING
                        COUNT(
                            QQ.QuizQuestionId
                        )
                        =
                        COUNT(
                            SA.MarksAwarded
                        )
                )


                SELECT
                    AVG(
                        CAST(
                            AttemptScore
                            AS DECIMAL(10,2)
                        )
                    )

                FROM GradedAttempts;
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
                AddFilterParameters(
                    command,
                    subjectId,
                    fromDate
                );


                connection.Open();


                object result =
                    command.ExecuteScalar();



                // -----------------------------------------
                // NO GRADED DATA
                // -----------------------------------------

                if (
                    result == null
                    ||
                    result == DBNull.Value
                )
                {
                    lblAverageScore.Text =
                        "–";

                    return;
                }



                decimal averageScore =
                    Convert.ToDecimal(
                        result
                    );


                lblAverageScore.Text =
                    averageScore
                        .ToString(
                            "0.0"
                        )
                    + "%";
            }
        }



        // =========================================================
        // PENDING LECTURER REVIEW
        //
        // Any non-MCQ question is treated as subjective,
        // matching the existing Question Bank logic.
        //
        // This counts quiz attempts rather than individual
        // questions, so one quiz with three pending subjective
        // questions is shown as one pending attempt.
        // =========================================================

        private void LoadPendingReviews(
            int subjectId,
            DateTime? fromDate)
        {
            const string sql = @"
                SELECT
                    COUNT(
                        DISTINCT
                        QA.AttemptId
                    )

                FROM QuizAttempts QA

                INNER JOIN Quizzes QZ
                    ON QA.QuizId =
                       QZ.QuizId

                INNER JOIN QuizQuestions QQ
                    ON QA.QuizId =
                       QQ.QuizId

                INNER JOIN Questions QN
                    ON QQ.QuestionId =
                       QN.QuestionId

                LEFT JOIN StudentAnswers SA
                    ON QA.AttemptId =
                       SA.AttemptId

                    AND
                    QQ.QuizQuestionId =
                        SA.QuizQuestionId

                WHERE
                    QA.StudentUserId =
                        @StudentUserId

                    AND
                    QA.SubmittedAt
                        IS NOT NULL

                    AND
                    UPPER(
                        LTRIM(
                            RTRIM(
                                QN.QuestionType
                            )
                        )
                    ) <> 'MCQ'

                    AND
                    (
                        SA.MarksAwarded
                            IS NULL

                        OR

                        SA.GradedAt
                            IS NULL
                    )

                    AND
                    (
                        @SubjectId = 0

                        OR

                        QZ.SubjectId =
                            @SubjectId
                    )

                    AND
                    (
                        @FromDate IS NULL

                        OR

                        QA.SubmittedAt >=
                            @FromDate
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
                AddFilterParameters(
                    command,
                    subjectId,
                    fromDate
                );


                connection.Open();


                int pendingReview =
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    );


                lblPendingReview.Text =
                    pendingReview
                        .ToString();
            }
        }



        // =========================================================
        // SCORE PROGRESS TREND
        //
        // Calculates each fully graded attempt first,
        // then groups the student's results by month.
        //
        // The result is converted into JSON and stored
        // inside hfTrendJson for Performance.js.
        // =========================================================

        private void LoadScoreTrend(
            int subjectId,
            DateTime? fromDate)
        {
            const string sql = @"
                WITH GradedAttempts AS
                (
                    SELECT
                        QA.AttemptId,

                        QA.SubmittedAt,

                        CAST(
                            SUM(
                                ISNULL(
                                    SA.MarksAwarded,
                                    0
                                )
                            )
                            * 100.0
                            /
                            NULLIF(
                                SUM(
                                    QQ.MaxMarks
                                ),
                                0
                            )

                            AS DECIMAL(6,2)
                        )
                        AS AttemptScore

                    FROM QuizAttempts QA

                    INNER JOIN Quizzes QZ
                        ON QA.QuizId =
                           QZ.QuizId

                    INNER JOIN QuizQuestions QQ
                        ON QA.QuizId =
                           QQ.QuizId

                    LEFT JOIN StudentAnswers SA
                        ON QA.AttemptId =
                           SA.AttemptId

                        AND
                        QQ.QuizQuestionId =
                            SA.QuizQuestionId

                    WHERE
                        QA.StudentUserId =
                            @StudentUserId

                        AND
                        QA.SubmittedAt
                            IS NOT NULL

                        AND
                        (
                            @SubjectId = 0

                            OR

                            QZ.SubjectId =
                                @SubjectId
                        )

                        AND
                        (
                            @FromDate IS NULL

                            OR

                            QA.SubmittedAt >=
                                @FromDate
                        )

                    GROUP BY
                        QA.AttemptId,
                        QA.SubmittedAt

                    HAVING
                        COUNT(
                            QQ.QuizQuestionId
                        )
                        =
                        COUNT(
                            SA.MarksAwarded
                        )
                )


                SELECT
                    DATEFROMPARTS(
                        YEAR(
                            SubmittedAt
                        ),
                        MONTH(
                            SubmittedAt
                        ),
                        1
                    )
                    AS MonthStart,


                    AVG(
                        CAST(
                            AttemptScore
                            AS DECIMAL(10,2)
                        )
                    )
                    AS AverageScore

                FROM GradedAttempts

                GROUP BY
                    YEAR(
                        SubmittedAt
                    ),

                    MONTH(
                        SubmittedAt
                    )

                ORDER BY
                    MonthStart;
            ";



            List<TrendPoint>
                trendPoints =
                    new List<TrendPoint>();



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
                AddFilterParameters(
                    command,
                    subjectId,
                    fromDate
                );


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
                        DateTime month =
                            Convert.ToDateTime(
                                reader[
                                    "MonthStart"
                                ]
                            );


                        decimal score =
                            Convert.ToDecimal(
                                reader[
                                    "AverageScore"
                                ]
                            );


                        trendPoints.Add(
                            new TrendPoint
                            {
                                Label =
                                    month.ToString(
                                        "MMM yyyy"
                                    ),

                                Score =
                                    Math.Round(
                                        score,
                                        1
                                    )
                            }
                        );
                    }
                }
            }



            // -----------------------------------------
            // CONVERT DATABASE DATA TO JSON
            //
            // Performance.js reads this JSON through
            // the hidden hfTrendJson field.
            // -----------------------------------------

            JavaScriptSerializer serializer =
                new JavaScriptSerializer();


            hfTrendJson.Value =
                serializer.Serialize(
                    trendPoints
                );
        }



        // =========================================================
        // SUBJECT MASTERY
        //
        // Calculates the average percentage for every
        // subject using only fully graded attempts.
        // =========================================================

        private void LoadSubjectMastery(
            int subjectId,
            DateTime? fromDate)
        {
            const string sql = @"
                WITH GradedAttempts AS
                (
                    SELECT
                        QA.AttemptId,

                        QZ.SubjectId,

                        CAST(
                            SUM(
                                ISNULL(
                                    SA.MarksAwarded,
                                    0
                                )
                            )
                            * 100.0
                            /
                            NULLIF(
                                SUM(
                                    QQ.MaxMarks
                                ),
                                0
                            )

                            AS DECIMAL(6,2)
                        )
                        AS AttemptScore

                    FROM QuizAttempts QA

                    INNER JOIN Quizzes QZ
                        ON QA.QuizId =
                           QZ.QuizId

                    INNER JOIN QuizQuestions QQ
                        ON QA.QuizId =
                           QQ.QuizId

                    LEFT JOIN StudentAnswers SA
                        ON QA.AttemptId =
                           SA.AttemptId

                        AND
                        QQ.QuizQuestionId =
                            SA.QuizQuestionId

                    WHERE
                        QA.StudentUserId =
                            @StudentUserId

                        AND
                        QA.SubmittedAt
                            IS NOT NULL

                        AND
                        (
                            @SubjectId = 0

                            OR

                            QZ.SubjectId =
                                @SubjectId
                        )

                        AND
                        (
                            @FromDate IS NULL

                            OR

                            QA.SubmittedAt >=
                                @FromDate
                        )

                    GROUP BY
                        QA.AttemptId,
                        QZ.SubjectId

                    HAVING
                        COUNT(
                            QQ.QuizQuestionId
                        )
                        =
                        COUNT(
                            SA.MarksAwarded
                        )
                )


                SELECT
                    S.SubjectId,

                    RTRIM(
                        S.SubjectName
                    )
                    AS SubjectName,

                    COUNT(
                        GA.AttemptId
                    )
                    AS AttemptCount,

                    AVG(
                        CAST(
                            GA.AttemptScore
                            AS DECIMAL(10,2)
                        )
                    )
                    AS AverageScore

                FROM GradedAttempts GA

                INNER JOIN Subjects S
                    ON GA.SubjectId =
                       S.SubjectId

                GROUP BY
                    S.SubjectId,
                    S.SubjectName

                ORDER BY
                    AverageScore DESC;
            ";



            List<SubjectPerformanceItem>
                subjectItems =
                    new List<SubjectPerformanceItem>();



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
                AddFilterParameters(
                    command,
                    subjectId,
                    fromDate
                );


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
                        decimal averageScore =
                            Convert.ToDecimal(
                                reader[
                                    "AverageScore"
                                ]
                            );


                        subjectItems.Add(
                            new SubjectPerformanceItem
                            {
                                SubjectName =
                                    Convert.ToString(
                                        reader[
                                            "SubjectName"
                                        ]
                                    )
                                    .Trim(),


                                AttemptCount =
                                    Convert.ToInt32(
                                        reader[
                                            "AttemptCount"
                                        ]
                                    ),


                                AverageScore =
                                    Math.Round(
                                        averageScore,
                                        1
                                    ),


                                Grade =
                                    GetGrade(
                                        averageScore
                                    ),


                                ProgressClass =
                                    GetProgressClass(
                                        averageScore
                                    )
                            }
                        );
                    }
                }
            }



            rptSubjectMastery.DataSource =
                subjectItems;


            rptSubjectMastery.DataBind();



            pnlNoSubjectData.Visible =
                subjectItems.Count == 0;
        }



        // =========================================================
        // GRADE CALCULATION
        //
        // These are application display bands.
        // Change the thresholds here if your project uses
        // a different official grading scale.
        // =========================================================

        private string GetGrade(
            decimal score)
        {
            if (score >= 80)
            {
                return "A";
            }


            if (score >= 70)
            {
                return "B";
            }


            if (score >= 60)
            {
                return "C";
            }


            if (score >= 50)
            {
                return "D";
            }


            return "F";
        }



        // =========================================================
        // PROGRESS BAR CLASS
        // =========================================================

        private string GetProgressClass(
            decimal score)
        {
            if (score >= 80)
            {
                return "mastery-excellent";
            }


            if (score >= 70)
            {
                return "mastery-good";
            }


            if (score >= 60)
            {
                return "mastery-average";
            }


            return "mastery-weak";
        }



        // =========================================================
        // SCORE TREND OBJECT
        //
        // Converted into JSON for Performance.js.
        // =========================================================

        public class TrendPoint
        {
            public string Label
            {
                get;
                set;
            }


            public decimal Score
            {
                get;
                set;
            }
        }



        // =========================================================
        // SUBJECT PERFORMANCE OBJECT
        //
        // Used by the Subject Mastery Repeater.
        // =========================================================

        public class SubjectPerformanceItem
        {
            public string SubjectName
            {
                get;
                set;
            }


            public int AttemptCount
            {
                get;
                set;
            }


            public decimal AverageScore
            {
                get;
                set;
            }


            public string Grade
            {
                get;
                set;
            }


            public string ProgressClass
            {
                get;
                set;
            }
        }
    }
}