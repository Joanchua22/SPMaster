using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SPMaster.Student
{
    public partial class QuizResult : System.Web.UI.Page
    {
        // =========================================================
        // CONNECTION
        // =========================================================

        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["SPMasterConnectionString"].ConnectionString;
            }
        }


        // =========================================================
        // CURRENT STUDENT
        // =========================================================

        private int CurrentUserId
        {
            get
            {
                return Convert.ToInt32(Session["UserId"]);
            }
        }


        // =========================================================
        // ATTEMPT / QUIZ
        // =========================================================

        private int AttemptId { get; set; }

        private int QuizId { get; set; }


        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            int attemptId;

            if (!int.TryParse(Request.QueryString["attemptId"], out attemptId))
            {
                Response.Redirect("~/Student/Quizzes.aspx?tab=attempts");
                return;
            }

            AttemptId = attemptId;

            if (!IsPostBack)
            {
                if (!LoadResultSummary())
                {
                    Response.Redirect("~/Student/Quizzes.aspx?tab=attempts");
                    return;
                }

                BindQuestionBreakdown();
            }
        }


        // =========================================================
        // RESULT SUMMARY
        // =========================================================

        private bool LoadResultSummary()
        {
            const string sql = @"
                SELECT
                    qa.AttemptId,
                    qa.QuizId,
                    qa.StartedAt,
                    qa.SubmittedAt,
                    qa.AttemptStatus,

                    q.Title,

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


                    /* -----------------------------------------
                       MARKS ACTUALLY AWARDED
                       ----------------------------------------- */

                    ISNULL(
                        SUM(
                            CASE
                                WHEN ques.QuestionType = 'MCQ'
                                THEN ISNULL(
                                    sa.MarksAwarded,
                                    0
                                )

                                ELSE ISNULL(
                                    sa.MarksAwarded,
                                    0
                                )
                            END
                        ),
                        0
                    ) AS AwardedMarks,


                    /* -----------------------------------------
                       TOTAL MARKS CURRENTLY GRADED

                       MCQ is always graded after submission.

                       Subjective is only included once
                       MarksAwarded is not NULL.
                       ----------------------------------------- */

                    ISNULL(
                        SUM(
                            CASE
                                WHEN ques.QuestionType = 'MCQ'
                                THEN qq.MaxMarks

                                WHEN sa.MarksAwarded IS NOT NULL
                                THEN qq.MaxMarks

                                ELSE 0
                            END
                        ),
                        0
                    ) AS GradedPossibleMarks,


                    /* -----------------------------------------
                       SUBJECTIVE QUESTIONS STILL PENDING
                       ----------------------------------------- */

                    SUM(
                        CASE
                            WHEN ques.QuestionType <> 'MCQ'
                                 AND sa.MarksAwarded IS NULL
                            THEN 1
                            ELSE 0
                        END
                    ) AS PendingSubjectiveCount

                FROM QuizAttempts qa

                INNER JOIN Quizzes q
                    ON qa.QuizId = q.QuizId

                INNER JOIN Users u
                    ON q.CreatedByUserId = u.UserId

                INNER JOIN QuizQuestions qq
                    ON q.QuizId = qq.QuizId

                INNER JOIN Questions ques
                    ON qq.QuestionId = ques.QuestionId

                LEFT JOIN StudentAnswers sa
                    ON sa.AttemptId = qa.AttemptId
                    AND sa.QuizQuestionId = qq.QuizQuestionId

                WHERE
                    qa.AttemptId = @AttemptId
                    AND qa.StudentUserId = @StudentUserId

                GROUP BY
                    qa.AttemptId,
                    qa.QuizId,
                    qa.StartedAt,
                    qa.SubmittedAt,
                    qa.AttemptStatus,
                    q.Title,
                    u.FullName;
            ";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@AttemptId", SqlDbType.Int).Value = AttemptId;
                command.Parameters.Add("@StudentUserId", SqlDbType.Int).Value = CurrentUserId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return false;
                    }

                    // =============================================
                    // ATTEMPT STATUS
                    // =============================================

                    string status = reader["AttemptStatus"].ToString();

                    // Student should not view a result
                    // for an active quiz.
                    if (status.Equals("InProgress", StringComparison.OrdinalIgnoreCase))
                    {
                        int activeAttemptId = Convert.ToInt32(reader["AttemptId"]);

                        Response.Redirect(
                            "~/Student/StartQuiz.aspx?attemptId=" + activeAttemptId
                        );

                        return false;
                    }


                    // =============================================
                    // VALUES
                    // =============================================

                    QuizId = Convert.ToInt32(reader["QuizId"]);

                    string title = reader["Title"].ToString();

                    DateTime startedAt = Convert.ToDateTime(reader["StartedAt"]);

                    DateTime? submittedAt =
                        reader["SubmittedAt"] == DBNull.Value
                            ? (DateTime?)null
                            : Convert.ToDateTime(reader["SubmittedAt"]);

                    decimal totalMarks = Convert.ToDecimal(reader["TotalMarks"]);
                    decimal awardedMarks = Convert.ToDecimal(reader["AwardedMarks"]);
                    decimal gradedPossibleMarks = Convert.ToDecimal(reader["GradedPossibleMarks"]);
                    int pendingSubjectiveCount = Convert.ToInt32(reader["PendingSubjectiveCount"]);


                    // =============================================
                    // TITLE
                    // =============================================

                    litQuizTitle.Text = Server.HtmlEncode(title);


                    // =============================================
                    // COMPLETION DATE
                    // =============================================

                    if (submittedAt.HasValue)
                    {
                        litCompletedDate.Text = submittedAt.Value.ToString("dd MMMM yyyy");
                    }
                    else
                    {
                        litCompletedDate.Text = "-";
                    }


                    // =============================================
                    // TIME SPENT
                    // =============================================

                    DateTime finishTime = submittedAt.HasValue
                        ? submittedAt.Value
                        : DateTime.Now;

                    TimeSpan duration = finishTime - startedAt;

                    if (duration.TotalSeconds < 0)
                    {
                        duration = TimeSpan.Zero;
                    }

                    litTimeSpent.Text = FormatDuration(duration);


                    // =============================================
                    // PENDING GRADING
                    // =============================================

                    bool pending = status.Equals(
                        "PendingGrading",
                        StringComparison.OrdinalIgnoreCase
                    );

                    if (pending)
                    {
                        pnlPendingBadge.Visible = true;
                        pnlPendingScore.Visible = true;

                        pnlGradedBadge.Visible = false;
                        pnlGradedScore.Visible = false;

                        // -----------------------------------------
                        // CURRENT GRADED SCORE
                        // -----------------------------------------

                        litCurrentScore.Text =
                            awardedMarks.ToString("0.##")
                            + " / "
                            + gradedPossibleMarks.ToString("0.##")
                            + "*";

                        // -----------------------------------------
                        // PENDING COUNT
                        // -----------------------------------------

                        litPendingCount.Text =
                            pendingSubjectiveCount
                            + (
                                pendingSubjectiveCount == 1
                                    ? " subjective question under review"
                                    : " subjective questions under review"
                            );
                    }


                    // =============================================
                    // FULLY GRADED
                    // =============================================

                    else
                    {
                        pnlPendingBadge.Visible = false;
                        pnlPendingScore.Visible = false;

                        pnlGradedBadge.Visible = true;
                        pnlGradedScore.Visible = true;

                        litFinalScore.Text =
                            awardedMarks.ToString("0.##")
                            + " / "
                            + totalMarks.ToString("0.##");

                        decimal percentage = 0;

                        if (totalMarks > 0)
                        {
                            percentage = awardedMarks / totalMarks * 100;
                        }

                        litFinalPercentage.Text =
                            "Graded ("
                            + percentage.ToString("0")
                            + "%)";
                    }


                    // =============================================
                    // TRY AGAIN
                    // =============================================

                    lnkTryAgain.NavigateUrl =
                        ResolveUrl(
                            "~/Student/StartQuiz.aspx?quizId=" + QuizId
                        );

                    return true;
                }
            }
        }


        // =========================================================
        // BIND QUESTION BREAKDOWN
        // =========================================================

        private void BindQuestionBreakdown()
        {
            const string sql = @"
                SELECT
                    qq.QuizQuestionId,
                    qq.QuestionOrder,
                    qq.MaxMarks,

                    ques.QuestionId,
                    ques.QuestionText,
                    ques.QuestionType,
                    ques.Explanation,
                    ques.ModelAnswer,

                    sa.SelectedOptionId,
                    sa.AnswerText,
                    sa.LecturerFeedback,


                    /* -----------------------------------------
                       DISPLAY MARKS

                       Unanswered MCQ = 0 marks.

                       Pending subjective = NULL.
                       ----------------------------------------- */

                    CASE
                        WHEN ques.QuestionType = 'MCQ'
                        THEN ISNULL(
                            sa.MarksAwarded,
                            0
                        )

                        ELSE sa.MarksAwarded
                    END AS DisplayMarksAwarded,


                    /* -----------------------------------------
                       STUDENT SELECTED OPTION
                       ----------------------------------------- */

                    selectedOption.OptionText
                        AS SelectedOptionText,


                    /* -----------------------------------------
                       WHETHER SELECTED OPTION IS CORRECT
                       ----------------------------------------- */

                    ISNULL(
                        selectedOption.IsCorrect,
                        0
                    ) AS SelectedOptionIsCorrect,


                    /* -----------------------------------------
                       CORRECT MCQ OPTION
                       ----------------------------------------- */

                    correctOption.OptionText
                        AS CorrectOptionText

                FROM QuizQuestions qq


                /* =============================================
                   QUESTION DETAILS
                   ============================================= */

                INNER JOIN Questions ques
                    ON qq.QuestionId = ques.QuestionId


                /* =============================================
                   STUDENT ANSWER

                   LEFT JOIN is used so unanswered questions
                   still appear in the result breakdown.
                   ============================================= */

                LEFT JOIN StudentAnswers sa
                    ON sa.QuizQuestionId = qq.QuizQuestionId
                    AND sa.AttemptId = @AttemptId


                /* =============================================
                   STUDENT SELECTED MCQ OPTION
                   ============================================= */

                LEFT JOIN QuestionOptions selectedOption
                    ON sa.SelectedOptionId = selectedOption.OptionId


                /* =============================================
                   CORRECT MCQ OPTION
                   ============================================= */

                OUTER APPLY
                (
                    SELECT TOP 1
                        qo.OptionText

                    FROM QuestionOptions qo

                    WHERE
                        qo.QuestionId = ques.QuestionId
                        AND qo.IsCorrect = 1

                    ORDER BY
                        qo.DisplayOrder
                ) correctOption


                /* =============================================
                   CURRENT QUIZ ONLY
                   ============================================= */

                WHERE
                    qq.QuizId = @QuizId

                ORDER BY
                    qq.QuestionOrder;
            ";

            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@AttemptId", SqlDbType.Int).Value = AttemptId;
                command.Parameters.Add("@QuizId", SqlDbType.Int).Value = QuizId;

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            rptResultQuestions.DataSource = table;
            rptResultQuestions.DataBind();
        }


        // =========================================================
        // QUESTION TYPE
        // =========================================================

        protected bool IsMcq(object questionType)
        {
            return string.Equals(
                Convert.ToString(questionType),
                "MCQ",
                StringComparison.OrdinalIgnoreCase
            );
        }

        protected string GetQuestionTypeText(object questionType)
        {
            return IsMcq(questionType)
                ? "Objective"
                : "Subjective";
        }


        // =========================================================
        // QUESTION ICON
        // =========================================================

        protected string GetQuestionIcon(object questionType, object marksAwarded, object maxMarks)
        {
            if (!IsMcq(questionType) &&
                (marksAwarded == null || marksAwarded == DBNull.Value))
            {
                return "bi bi-hourglass-split";
            }

            decimal awarded = Convert.ToDecimal(marksAwarded);
            decimal maximum = Convert.ToDecimal(maxMarks);

            if (awarded >= maximum)
            {
                return "bi bi-check-lg";
            }

            return "bi bi-x-lg";
        }

        protected string GetQuestionIconCss(object questionType, object marksAwarded, object maxMarks)
        {
            if (!IsMcq(questionType) &&
                (marksAwarded == null || marksAwarded == DBNull.Value))
            {
                return "result-question-icon result-icon-pending";
            }

            decimal awarded = Convert.ToDecimal(marksAwarded);
            decimal maximum = Convert.ToDecimal(maxMarks);

            if (awarded >= maximum)
            {
                return "result-question-icon result-icon-correct";
            }

            return "result-question-icon result-icon-incorrect";
        }


        // =========================================================
        // QUESTION MARKS
        // =========================================================

        protected string GetQuestionMarksText(object questionType, object marksAwarded, object maxMarks)
        {
            if (!IsMcq(questionType) &&
                (marksAwarded == null || marksAwarded == DBNull.Value))
            {
                return "Pending Lecturer Review";
            }

            decimal awarded = Convert.ToDecimal(marksAwarded);
            decimal maximum = Convert.ToDecimal(maxMarks);

            return awarded.ToString("0.##")
                + " / "
                + maximum.ToString("0.##")
                + " Marks";
        }

        protected string GetQuestionMarksCss(object questionType, object marksAwarded)
        {
            if (!IsMcq(questionType) &&
                (marksAwarded == null || marksAwarded == DBNull.Value))
            {
                return "result-question-pending";
            }

            return "result-question-marks result-marks-correct";
        }


        // =========================================================
        // OBJECTIVE ANSWER
        // =========================================================

        protected string GetObjectiveAnswerCss(object selectedOptionId, object selectedOptionIsCorrect)
        {
            if (selectedOptionId == null || selectedOptionId == DBNull.Value)
            {
                return "result-answer-box result-answer-incorrect";
            }

            bool correct = Convert.ToBoolean(selectedOptionIsCorrect);

            return correct
                ? "result-answer-box result-answer-correct"
                : "result-answer-box result-answer-incorrect";
        }

        protected string GetSelectedAnswerText(object selectedOptionText)
        {
            if (selectedOptionText == null ||
                selectedOptionText == DBNull.Value ||
                string.IsNullOrWhiteSpace(Convert.ToString(selectedOptionText)))
            {
                return "No answer submitted";
            }

            return Convert.ToString(selectedOptionText);
        }

        protected string GetObjectiveResultText(object selectedOptionId, object selectedOptionIsCorrect)
        {
            if (selectedOptionId == null || selectedOptionId == DBNull.Value)
            {
                return "(No Answer)";
            }

            return Convert.ToBoolean(selectedOptionIsCorrect)
                ? "(Correct)"
                : "(Incorrect)";
        }

        protected bool ShowCorrectAnswer(object selectedOptionId, object selectedOptionIsCorrect)
        {
            if (selectedOptionId == null || selectedOptionId == DBNull.Value)
            {
                return true;
            }

            return !Convert.ToBoolean(selectedOptionIsCorrect);
        }


        // =========================================================
        // SUBJECTIVE
        // =========================================================

        protected string GetSubjectiveAnswer(object answerText)
        {
            string answer = Convert.ToString(answerText);

            if (string.IsNullOrWhiteSpace(answer))
            {
                return "No answer submitted.";
            }

            return answer;
        }


        // =========================================================
        // HAS TEXT
        // =========================================================

        protected bool HasText(object value)
        {
            return !string.IsNullOrWhiteSpace(Convert.ToString(value));
        }


        // =========================================================
        // DURATION
        // =========================================================

        private string FormatDuration(TimeSpan duration)
        {
            int totalMinutes = (int)duration.TotalMinutes;
            int seconds = duration.Seconds;

            if (totalMinutes <= 0)
            {
                return seconds + (seconds == 1 ? " sec" : " secs");
            }

            return totalMinutes + (totalMinutes == 1 ? " min " : " mins ") + seconds + (seconds == 1 ? " sec" : " secs");
        }
    }
}