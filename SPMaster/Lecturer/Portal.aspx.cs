using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;


namespace SPMaster.Lecturer
{
    public partial class Portal :
        System.Web.UI.Page
    {
        // =========================================================
        // CONNECTION STRING
        // =========================================================

        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["SPMasterConnectionString"].ConnectionString;
            }
        }


        // =========================================================
        // CURRENT LECTURER
        // =========================================================

        private int CurrentUserId
        {
            get
            {
                return Convert.ToInt32(Session["UserId"]);
            }
        }


        protected void Page_Load(object sender,EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }


            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }



        // =========================================================
        // LOAD DASHBOARD
        // =========================================================

        private void LoadDashboard()
        {
            LoadQuestionBankSummary();

            LoadQuizSummary();

            LoadGradingSummary();

            BindRecentQuizzes();

            BindGradingQueue();

            LoadCohortPerformance();
        }



        // =========================================================
        // QUESTION BANK SUMMARY
        // =========================================================

        private void LoadQuestionBankSummary()
        {
            const string sql = @"SELECT COUNT(*) AS TotalQuestions, SUM(CASE WHEN QuestionType = 'MCQ' THEN 1 ELSE 0 END) AS McqCount, 
                            SUM(CASE WHEN QuestionType <> 'MCQ' THEN 1 ELSE 0 END) AS SubjectiveCount FROM Questions WHERE CreatedByUserId = @LecturerId AND IsArchived = 0;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            
            using (SqlCommand command = new SqlCommand(sql,connection))
            {
                command.Parameters.Add("@LecturerId",SqlDbType.Int).Value =CurrentUserId;

                connection.Open();

                using (SqlDataReader reader =command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int total = Convert.ToInt32(reader["TotalQuestions"]);
                        int mcq = reader["McqCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["McqCount"]);
                        int subjective = reader["SubjectiveCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SubjectiveCount"]);
                        litQuestionBankTotal.Text = total + (total == 1 ? " Item" : " Items");
                        litQuestionBankBreakdown.Text = mcq + " MCQ • " + subjective + " Subjective";
                    }
                }
            }
        }

        // =========================================================
        // QUIZ SUMMARY
        // =========================================================

        private void LoadQuizSummary()
        {
            const string sql = @"SELECT SUM(CASE WHEN IsPublished = 1 THEN 1 ELSE 0 END) AS PublishedCount, SUM(CASE WHEN IsPublished = 0 THEN 1 ELSE 0 END) AS DraftCount
                                FROM Quizzes WHERE CreatedByUserId = @LecturerId AND IsArchived = 0;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))

            using (SqlCommand command = new SqlCommand(sql,connection))
            {
                command.Parameters.Add("@LecturerId",SqlDbType.Int).Value = CurrentUserId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int published = reader["PublishedCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PublishedCount"]);
                        int drafts = reader["DraftCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DraftCount"]);
                        litPublishedQuizCount.Text = published + (published == 1 ? " Quiz" : " Quizzes");
                        litDraftQuizCount.Text = drafts + (drafts == 1 ? " Draft" : " Drafts");
                    }
                }
            }
        }


        // =========================================================
        // GRADING SUMMARY
        // =========================================================

        private void LoadGradingSummary()
        {
            const string sql = @"SELECT COUNT(*) FROM QuizAttempts qa INNER JOIN Quizzes q ON qa.QuizId = q.QuizId WHERE q.CreatedByUserId = @LecturerId AND qa.AttemptStatus = 'PendingGrading';";

            int pendingCount;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            
            using (SqlCommand command = new SqlCommand(sql,connection))
            {
                command.Parameters.Add("@LecturerId",SqlDbType.Int).Value = CurrentUserId;

                connection.Open();

                pendingCount = Convert.ToInt32(command.ExecuteScalar());
            }

            litGradingQueueCount.Text = pendingCount + " Pending";
            litSideQueueCount.Text = pendingCount.ToString();

            // =============================================
            // NO PENDING GRADING
            // =============================================

            if (pendingCount == 0)
            {
                pnlGradingAlert.Visible = true;
                litGradingAlertCount.Text = "You're all caught up!";
                litGradingAlertDescription.Text = "There are currently no subjective submissions waiting for grading.";
                lnkGradeSubmissions.Visible = false;
                return;
            }

            pnlGradingAlert.Visible = true;
            lnkGradeSubmissions.Visible = true;
            litGradingAlertCount.Text = pendingCount + (pendingCount == 1 ? " Submission Awaiting Grading" : " Submissions Awaiting Grading");

            LoadGradingAlertTitles();
        }



        // =========================================================
        // GRADING ALERT QUIZ TITLES
        // =========================================================

        private void LoadGradingAlertTitles()
        {
            const string sql = @" SELECT TOP 2 q.Title, MAX(qa.SubmittedAt) AS LatestSubmission FROM QuizAttempts qa INNER JOIN Quizzes q ON qa.QuizId = q.QuizId
                            WHERE q.CreatedByUserId = @LecturerId AND qa.AttemptStatus = 'PendingGrading' GROUP BY q.QuizId, q.Title ORDER BY LatestSubmission DESC;";

            string firstTitle = null;
            string secondTitle = null;

            using (SqlConnection connection = new SqlConnection(ConnectionString))

            using (SqlCommand command = new SqlCommand(sql,connection))
            {
                command.Parameters.Add("@LecturerId",SqlDbType.Int).Value = CurrentUserId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        firstTitle = reader["Title"].ToString();
                    }

                    if (reader.Read())
                    {
                        secondTitle = reader["Title"].ToString();
                    }
                }
            }


            if (!string.IsNullOrWhiteSpace(firstTitle) && !string.IsNullOrWhiteSpace(secondTitle))
            {
                litGradingAlertDescription.Text = "Student attempts in <strong>\"" + HttpUtility.HtmlEncode(firstTitle) + "\"</strong> and <strong>\"" + HttpUtility.HtmlEncode(secondTitle) + "\"</strong> are waiting for your subjective mark allocation.";
            }
            else if (!string.IsNullOrWhiteSpace(firstTitle))
            {
                litGradingAlertDescription.Text = "Student attempts in <strong>\"" + HttpUtility.HtmlEncode(firstTitle) + "\"</strong> are waiting for your subjective mark allocation.";
            }
            else
            {
                litGradingAlertDescription.Text = "Student attempts are waiting for your subjective mark allocation.";
            }
        }



        // =========================================================
        // RECENT QUIZZES
        // =========================================================

        private void BindRecentQuizzes()
        {
            const string sql = @"SELECT TOP 3 q.QuizId, q.Title, q.TimeLimitMinutes, q.IsPublished, q.CreatedAt, s.SubjectName, f.FormName,
                            (SELECT COUNT(*) FROM QuizQuestions qq WHERE qq.QuizId = q.QuizId) AS QuestionCount,
                            (SELECT ISNULL (SUM(qq.MaxMarks),0) FROM QuizQuestions qq WHERE qq.QuizId = q.QuizId) AS TotalMarks,
                            (SELECT COUNT(*) FROM QuizAttempts qa WHERE qa.QuizId = q.QuizId) AS AttemptCount,
                            (SELECT COUNT(*) FROM QuizAttempts qa WHERE qa.QuizId = q.QuizId AND qa.AttemptStatus = 'PendingGrading') AS PendingCount
                            FROM Quizzes q INNER JOIN Subjects s ON q.SubjectId = s.SubjectId INNER JOIN Forms f ON q.FormId = f.FormId WHERE q.CreatedByUserId = @LecturerId AND q.IsArchived = 0 ORDER BY q.CreatedAt DESC;";

            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConnectionString))

            using (SqlCommand command = new SqlCommand(sql,connection))
            {
                command.Parameters.Add("@LecturerId",SqlDbType.Int).Value = CurrentUserId;

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            pnlNoRecentQuizzes.Visible = table.Rows.Count == 0;
            rptRecentQuizzes.Visible = table.Rows.Count > 0;
            rptRecentQuizzes.DataSource = table;
            rptRecentQuizzes.DataBind();
        }



        // =========================================================
        // GRADING QUEUE
        // =========================================================

        private void BindGradingQueue()
        {
            const string sql = @"SELECT TOP 3 qa.AttemptId, qa.SubmittedAt, student.FullName AS StudentName, q.Title AS QuizTitle FROM QuizAttempts qa
                            INNER JOIN Quizzes q ON qa.QuizId = q.QuizId INNER JOIN Users student ON qa.StudentUserId = student.UserId WHERE q.CreatedByUserId = @LecturerId AND qa.AttemptStatus = 'PendingGrading' ORDER BY qa.SubmittedAt DESC, qa.AttemptId DESC;";

            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConnectionString))

            using (SqlCommand command = new SqlCommand(sql,connection))
            {
                command.Parameters.Add("@LecturerId",SqlDbType.Int).Value =CurrentUserId;

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table
                    );
                }
            }

            pnlNoGradingQueue.Visible = table.Rows.Count == 0;
            rptGradingQueue.Visible = table.Rows.Count > 0;
            rptGradingQueue.DataSource = table;
            rptGradingQueue.DataBind();
        }



        // =========================================================
        // COHORT PERFORMANCE
        // =========================================================

        private void LoadCohortPerformance()
        {
            const string sql = @"WITH QuizTotals AS (SELECT q.QuizId, ISNULL (SUM( qq.MaxMarks),0) AS TotalMarks FROM Quizzes q LEFT JOIN QuizQuestions qq ON q.QuizId = qq.QuizId
                            WHERE q.CreatedByUserId = @LecturerId AND q.IsArchived = 0 GROUP BY q.QuizId), AttemptScores AS (SELECT qa.AttemptId, CASE WHEN qt.TotalMarks > 0 THEN ISNULL (SUM (ISNULL (sa.MarksAwarded,0)),0) * 100.0 / qt.TotalMarks ELSE 0 END AS Percentage 
                            FROM QuizAttempts qa INNER JOIN QuizTotals qt ON qa.QuizId = qt.QuizId LEFT JOIN StudentAnswers sa ON qa.AttemptId = sa.AttemptId 
                            WHERE qa.AttemptStatus ='Graded' GROUP BY qa.AttemptId, qt.TotalMarks) SELECT COUNT(*) AS GradedAttempts, AVG(Percentage) AS AverageScore FROM AttemptScores;";

            int gradedAttempts = 0;
            decimal averageScore = 0;


            using (SqlConnection connection = new SqlConnection(ConnectionString))

            using (SqlCommand command = new SqlCommand(sql,connection))
            {
                command.Parameters.Add("@LecturerId",SqlDbType.Int).Value = CurrentUserId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        gradedAttempts = Convert.ToInt32(reader["GradedAttempts"]);

                        if (reader["AverageScore"] != DBNull.Value)
                        {
                            averageScore = Convert.ToDecimal(reader["AverageScore"]);
                        }
                    }
                }
            }


            // =============================================
            // DISPLAY AVERAGE
            // =============================================

            litCohortAttemptDescription.Text = "Across " + gradedAttempts + (gradedAttempts == 1 ? " graded attempt" : " graded attempts");

            if (gradedAttempts == 0)
            {
                litCohortAverage.Text = "—";
                passRateChart.Style["background"] ="#eee4ff";
            }
            else
            {
                litCohortAverage.Text = averageScore.ToString("0.0") + "%";
                decimal chartValue = Math.Max(0,Math.Min(100,averageScore));
                passRateChart.Style["background"] ="conic-gradient(" + "#7c28ef 0 " + chartValue.ToString("0.##") + "%, " + "#eee4ff " + chartValue.ToString("0.##") + "% 100%)";
            }

            LoadSubjectPerformance();
        }



        // =========================================================
        // SUBJECT PERFORMANCE
        // =========================================================

        private void LoadSubjectPerformance()
        {
            const string sql = @"WITH QuizTotals AS (SELECT q.QuizId, s.SubjectName, ISNULL(SUM(qq.MaxMarks),0) AS TotalMarks FROM Quizzes q INNER JOIN Subjects s ON q.SubjectId = s.SubjectId
                        LEFT JOIN QuizQuestions qq ON q.QuizId = qq.QuizId WHERE q.CreatedByUserId = @LecturerId AND q.IsArchived = 0 GROUP BY q.QuizId, s.SubjectName), AttemptScores AS
                (
                    SELECT
                        qa.AttemptId,
                        qt.SubjectName,

                        CASE

                            WHEN
                                qt.TotalMarks > 0

                            THEN
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
                                qt.TotalMarks

                            ELSE 0

                        END AS Percentage

                    FROM QuizAttempts qa

                    INNER JOIN QuizTotals qt
                        ON qa.QuizId =
                           qt.QuizId

                    LEFT JOIN StudentAnswers sa
                        ON qa.AttemptId =
                           sa.AttemptId

                    WHERE
                        qa.AttemptStatus =
                            'Graded'

                    GROUP BY
                        qa.AttemptId,
                        qt.SubjectName,
                        qt.TotalMarks
                )

                SELECT
                    SubjectName,

                    AVG(
                        Percentage
                    ) AS AverageScore

                FROM AttemptScores

                GROUP BY
                    SubjectName

                ORDER BY
                    AverageScore DESC;
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


            if (
                table.Rows.Count == 0
            )
            {
                litTopSubject.Text =
                    "—";


                litNeedsAttention.Text =
                    "—";


                return;
            }


            // =============================================
            // TOP SUBJECT
            // =============================================

            DataRow topRow =
                table.Rows[0];


            litTopSubject.Text =
                HttpUtility.HtmlEncode(
                    topRow[
                        "SubjectName"
                    ].ToString()
                )
                + " ("
                + Convert.ToDecimal(
                    topRow[
                        "AverageScore"
                    ]
                ).ToString(
                    "0"
                )
                + "%)";


            // =============================================
            // LOWEST SUBJECT
            // =============================================

            if (
                table.Rows.Count > 1
            )
            {
                DataRow lowestRow =
                    table.Rows[
                        table.Rows.Count - 1
                    ];


                litNeedsAttention.Text =
                    HttpUtility.HtmlEncode(
                        lowestRow[
                            "SubjectName"
                        ].ToString()
                    )
                    + " ("
                    + Convert.ToDecimal(
                        lowestRow[
                            "AverageScore"
                        ]
                    ).ToString(
                        "0"
                    )
                    + "%)";
            }
            else
            {
                litNeedsAttention.Text =
                    "—";
            }
        }



        // =========================================================
        // SUBJECT TAG CSS
        // =========================================================

        protected string GetSubjectTagCss(
            object subjectName)
        {
            string subject =
                Convert.ToString(
                    subjectName
                );


            if (
                subject.Equals(
                    "Sejarah",
                    StringComparison
                        .OrdinalIgnoreCase
                )
            )
            {
                return
                    "quiz-tag quiz-tag-yellow";
            }


            return "quiz-tag";
        }



        // =========================================================
        // QUIZ STATUS
        // =========================================================

        protected string GetQuizStatusCss(
            object isPublished)
        {
            bool published =
                Convert.ToBoolean(
                    isPublished
                );


            return published
                ? "quiz-tag quiz-tag-published"
                : "quiz-tag quiz-tag-draft";
        }


        protected string GetQuizStatusText(
            object isPublished)
        {
            return Convert.ToBoolean(
                isPublished
            )
                ? "Published"
                : "Draft";
        }



        // =========================================================
        // QUIZ SUMMARY TEXT
        // =========================================================

        protected string GetQuizSummary(
            object questionCount,
            object totalMarks,
            object timeLimit,
            object attemptCount,
            object pendingCount,
            object isPublished)
        {
            int questions =
                Convert.ToInt32(
                    questionCount
                );


            decimal marks =
                Convert.ToDecimal(
                    totalMarks
                );


            int minutes =
                Convert.ToInt32(
                    timeLimit
                );


            int attempts =
                Convert.ToInt32(
                    attemptCount
                );


            int pending =
                Convert.ToInt32(
                    pendingCount
                );


            bool published =
                Convert.ToBoolean(
                    isPublished
                );


            string text =
                questions
                + (
                    questions == 1
                        ? " Question"
                        : " Questions"
                )
                + " • "
                + marks.ToString(
                    "0.##"
                )
                + " Marks"
                + " • "
                + minutes
                + (
                    minutes == 1
                        ? " Min"
                        : " Mins"
                );


            if (!published)
            {
                return
                    text
                    + " • Incomplete draft";
            }


            if (
                pending > 0
            )
            {
                return
                    text
                    + " • "
                    + pending
                    + (
                        pending == 1
                            ? " Pending Grading"
                            : " Pending Grading"
                    );
            }


            return
                text
                + " • "
                + attempts
                + (
                    attempts == 1
                        ? " Attempt"
                        : " Attempts"
                );
        }



        // =========================================================
        // PRIMARY QUIZ ACTION TEXT
        // =========================================================

        protected string GetPrimaryQuizActionText(
            object isPublished)
        {
            return Convert.ToBoolean(
                isPublished
            )
                ? "Host Session"
                : "Edit Draft";
        }



        // =========================================================
        // PRIMARY QUIZ ACTION CSS
        // =========================================================

        protected string GetPrimaryQuizActionCss(
            object isPublished)
        {
            return Convert.ToBoolean(
                isPublished
            )
                ? "small-button small-button-primary"
                : "small-button small-button-soft";
        }



        // =========================================================
        // PRIMARY QUIZ ACTION URL
        // =========================================================

        protected string GetPrimaryQuizActionUrl(
            object quizId,
            object isPublished)
        {
            int id =
                Convert.ToInt32(
                    quizId
                );

            if (
                Convert.ToBoolean(
                    isPublished
                )
            )
            {
                return ResolveUrl(
                    "~/Lecturer/Sessions.aspx?quizId="
                    + id
                );
            }

            return ResolveUrl(
                "~/Lecturer/CreateQuiz.aspx?id="
                + id
            );
        }
        // =========================================================
        // PREVIEW URL
        // =========================================================

        protected string GetPreviewQuizUrl(
            object quizId)
        {
            int id =
                Convert.ToInt32(
                    quizId
                );


            return ResolveUrl(
                "~/Lecturer/PreviewQuiz.aspx?id="
                + id
                + "&from=portal"
            );
        }



        // =========================================================
        // GRADING QUEUE SUBTITLE
        // =========================================================

        protected string GetQueueSubtitle(
            object quizTitle,
            object submittedAt)
        {
            string title =
                Convert.ToString(
                    quizTitle
                );


            string dateText =
                "";


            if (
                submittedAt != null
                &&
                submittedAt != DBNull.Value
            )
            {
                dateText =
                    Convert.ToDateTime(
                        submittedAt
                    )
                    .ToString(
                        "dd MMM"
                    );
            }


            if (
                string.IsNullOrWhiteSpace(
                    dateText
                )
            )
            {
                return title;
            }


            return
                title
                + " • "
                + dateText;
        }



        // =========================================================
        // GRADING URL
        // =========================================================

        protected string GetGradingUrl(
            object attemptId)
        {
            int id =
                Convert.ToInt32(
                    attemptId
                );


            return ResolveUrl(
                "~/Lecturer/Grading.aspx?attemptId="
                + id
            );
        }
    }
}