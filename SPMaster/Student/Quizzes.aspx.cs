using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;


namespace SPMaster.Student
{
    public partial class Quizzes :
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
                BindSubjects();

                BindExploreQuizzes();

                BindAttempts();
            }
        }


        // =========================================================
        // SUBJECT FILTER
        // =========================================================

        private void BindSubjects()
        {
            const string sql = @"
                SELECT
                    SubjectId,
                    SubjectName

                FROM Subjects

                WHERE
                    IsActive = 1

                ORDER BY
                    SubjectName;
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
                    connection.Open();


                    ddlStudentSubject.DataSource =
                        command.ExecuteReader();


                    ddlStudentSubject.DataTextField =
                        "SubjectName";


                    ddlStudentSubject.DataValueField =
                        "SubjectId";


                    ddlStudentSubject.DataBind();
                }
            }


            ddlStudentSubject.Items.Insert(
                0,
                new ListItem(
                    "All Subjects",
                    ""
                )
            );
        }


        // =========================================================
        // SEARCH / FILTER
        // =========================================================

        protected void FilterChanged(
            object sender,
            EventArgs e)
        {
            BindExploreQuizzes();
        }


        // =========================================================
        // EXPLORE QUIZZES
        // =========================================================

        private void BindExploreQuizzes()
        {
            string sql = @"
                SELECT
                    q.QuizId,
                    q.Title,
                    q.Description,
                    q.TimeLimitMinutes,
                    q.CreatedAt,

                    s.SubjectId,
                    s.SubjectName,

                    f.FormName,

                    u.FullName AS CreatorName,

                    COUNT(
                        qq.QuizQuestionId
                    ) AS QuestionCount,

                    ISNULL(
                        SUM(
                            qq.MaxMarks
                        ),
                        0
                    ) AS TotalMarks,


                    /* =========================================
                       OBJECTIVE QUESTION COUNT
                       ========================================= */

                    (
                        SELECT COUNT(*)

                        FROM QuizQuestions qq2

                        INNER JOIN Questions ques2
                            ON qq2.QuestionId =
                               ques2.QuestionId

                        WHERE
                            qq2.QuizId =
                                q.QuizId

                            AND
                            ques2.QuestionType =
                                'MCQ'

                    ) AS McqCount,


                    /* =========================================
                       SUBJECTIVE QUESTION COUNT
                       ========================================= */

                    (
                        SELECT COUNT(*)

                        FROM QuizQuestions qq3

                        INNER JOIN Questions ques3
                            ON qq3.QuestionId =
                               ques3.QuestionId

                        WHERE
                            qq3.QuizId =
                                q.QuizId

                            AND
                            ques3.QuestionType <>
                                'MCQ'

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


                WHERE
                    q.IsPublished = 1

                    AND
                    q.IsArchived = 0


                    AND
                    (
                        @SubjectId IS NULL

                        OR

                        q.SubjectId =
                            @SubjectId
                    )


                    AND
                    (
                        @Search = ''

                        OR

                        q.Title LIKE
                            @SearchPattern

                        OR

                        ISNULL(
                            q.Description,
                            ''
                        ) LIKE
                            @SearchPattern

                        OR

                        s.SubjectName LIKE
                            @SearchPattern

                        OR

                        f.FormName LIKE
                            @SearchPattern

                        OR

                        u.FullName LIKE
                            @SearchPattern
                    )


                GROUP BY
                    q.QuizId,
                    q.Title,
                    q.Description,
                    q.TimeLimitMinutes,
                    q.CreatedAt,

                    s.SubjectId,
                    s.SubjectName,

                    f.FormName,

                    u.FullName


                ORDER BY
                    q.CreatedAt DESC;
            ";


            DataTable table =
                new DataTable();


            string searchText =
                txtQuizSearch.Text.Trim();


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
                    // ---------------------------------------------
                    // SUBJECT
                    // ---------------------------------------------

                    if (
                        string.IsNullOrEmpty(
                            ddlStudentSubject.SelectedValue
                        )
                    )
                    {
                        command.Parameters.Add(
                            "@SubjectId",
                            SqlDbType.Int
                        ).Value =
                            DBNull.Value;
                    }
                    else
                    {
                        command.Parameters.Add(
                            "@SubjectId",
                            SqlDbType.Int
                        ).Value =
                            Convert.ToInt32(
                                ddlStudentSubject
                                    .SelectedValue
                            );
                    }


                    // ---------------------------------------------
                    // SEARCH
                    // ---------------------------------------------

                    command.Parameters.Add(
                        "@Search",
                        SqlDbType.NVarChar,
                        200
                    ).Value =
                        searchText;


                    command.Parameters.Add(
                        "@SearchPattern",
                        SqlDbType.NVarChar,
                        204
                    ).Value =
                        "%"
                        + searchText
                        + "%";


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


            rptExploreQuizzes.DataSource =
                table;


            rptExploreQuizzes.DataBind();


            pnlNoQuizzes.Visible =
                table.Rows.Count == 0;
        }


        // =========================================================
        // MY ATTEMPTS
        // =========================================================

        private void BindAttempts()
        {
            const string sql = @"
                SELECT
                    qa.AttemptId,
                    qa.QuizId,
                    qa.StartedAt,
                    qa.ExpiresAt,
                    qa.SubmittedAt,
                    qa.AttemptStatus,

                    q.Title,

                    s.SubjectName,

                    ROW_NUMBER()
                    OVER
                    (
                        PARTITION BY
                            qa.StudentUserId,
                            qa.QuizId

                        ORDER BY
                            qa.StartedAt,
                            qa.AttemptId
                    ) AS AttemptNumber,

                    (
                        SELECT COUNT(*)

                        FROM QuizQuestions qq

                        WHERE
                            qq.QuizId =
                                qa.QuizId

                    ) AS TotalQuestions,

                    (
                        SELECT COUNT(*)

                        FROM StudentAnswers sa

                        WHERE
                            sa.AttemptId =
                                qa.AttemptId

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

                    ) AS AnsweredQuestions,

                    (
                        SELECT
                            ISNULL(
                                SUM(
                                    sa.MarksAwarded
                                ),
                                0
                            )

                        FROM StudentAnswers sa

                        WHERE
                            sa.AttemptId =
                                qa.AttemptId

                    ) AS AwardedMarks,

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
                                qa.QuizId

                    ) AS TotalMarks

                FROM QuizAttempts qa

                INNER JOIN Quizzes q
                    ON qa.QuizId =
                       q.QuizId

                INNER JOIN Subjects s
                    ON q.SubjectId =
                       s.SubjectId

                WHERE
                    qa.StudentUserId =
                        @StudentUserId

                ORDER BY
                    qa.StartedAt DESC,
                    qa.AttemptId DESC;
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
                        "@StudentUserId",
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


            rptAttempts.DataSource =
                table;


            rptAttempts.DataBind();


            pnlNoAttempts.Visible =
                table.Rows.Count == 0;


            litAttemptCount.Text =
                table.Rows.Count.ToString();
        }


        // =========================================================
        // QUIZ CARD HELPERS
        // =========================================================

        protected string GetSubjectCss(
            object subjectName)
        {
            string subject =
                Convert.ToString(
                    subjectName
                )
                .Trim()
                .ToLower();


            switch (subject)
            {
                case "mathematics":

                    return "subject-mathematics";


                case "sejarah":

                    return "subject-sejarah";


                case "english":

                    return "subject-english";


                case "bahasa melayu":

                    return "subject-bahasa-melayu";


                case "science":

                    return "subject-science";


                default:

                    return "subject-mathematics";
            }
        }


        protected string GetTeacherAvatarCss(
            object subjectName)
        {
            string subject =
                Convert.ToString(
                    subjectName
                )
                .Trim()
                .ToLower();


            switch (subject)
            {
                case "sejarah":

                    return "teacher-yellow";


                case "english":
                case "science":

                    return "teacher-green";


                default:

                    return "teacher-purple";
            }
        }


        protected string GetInitials(
            object fullName)
        {
            string name =
                Convert.ToString(
                    fullName
                ).Trim();


            if (
                string.IsNullOrWhiteSpace(
                    name
                )
            )
            {
                return "?";
            }


            string[] parts =
                name.Split(
                    new char[] { ' ' },
                    StringSplitOptions
                        .RemoveEmptyEntries
                );


            if (
                parts.Length == 1
            )
            {
                return parts[0]
                    .Substring(
                        0,
                        1
                    )
                    .ToUpper();
            }


            return (
                parts[0].Substring(0, 1)
                +
                parts[
                    parts.Length - 1
                ].Substring(0, 1)
            ).ToUpper();
        }


        protected string GetQuizDescription(
            object description)
        {
            string value =
                Convert.ToString(
                    description
                ).Trim();


            if (
                string.IsNullOrWhiteSpace(
                    value
                )
            )
            {
                return "No description provided.";
            }


            return value;
        }


        protected string FormatQuestionCount(
            object value)
        {
            int count =
                Convert.ToInt32(
                    value
                );


            return count
                + (
                    count == 1
                        ? " Question"
                        : " Questions"
                );
        }


        protected string FormatMarks(
            object value)
        {
            decimal marks =
                Convert.ToDecimal(
                    value
                );


            return marks.ToString(
                "0.##"
            )
            + (
                marks == 1
                    ? " Mark"
                    : " Marks"
            );
        }


        protected string FormatMinutes(
            object value)
        {
            int minutes =
                Convert.ToInt32(
                    value
                );


            return minutes
                + (
                    minutes == 1
                        ? " Min"
                        : " Mins"
                );
        }


        // =========================================================
        // ATTEMPT HELPERS
        // =========================================================

        private string NormalizeAttemptStatus(
            object attemptStatus)
        {
            return Convert.ToString(
                attemptStatus
            )
            .Replace(
                " ",
                ""
            )
            .Trim()
            .ToLower();
        }


        protected string GetAttemptSubtitle(
            object attemptStatus,
            object submittedAt)
        {
            string status =
                NormalizeAttemptStatus(
                    attemptStatus
                );


            if (
                status == "inprogress"
            )
            {
                return "In progress (Saved draft)";
            }


            if (
                submittedAt == DBNull.Value
                ||
                submittedAt == null
            )
            {
                return "Submitted";
            }


            DateTime date =
                Convert.ToDateTime(
                    submittedAt
                );


            return "Submitted "
                + date.ToString(
                    "dd MMM yyyy, hh:mm tt"
                );
        }


        protected string GetAttemptScoreText(
            object attemptStatus,
            object awardedMarks,
            object totalMarks,
            object answeredQuestions,
            object totalQuestions)
        {
            string status =
                NormalizeAttemptStatus(
                    attemptStatus
                );


            if (
                status == "pendinggrading"
            )
            {
                return "Awaiting";
            }


            if (
                status == "inprogress"
            )
            {
                int answered =
                    Convert.ToInt32(
                        answeredQuestions
                    );


                int total =
                    Convert.ToInt32(
                        totalQuestions
                    );


                return answered
                    + " / "
                    + total
                    + " Qs";
            }


            decimal awarded =
                Convert.ToDecimal(
                    awardedMarks
                );


            decimal totalScore =
                Convert.ToDecimal(
                    totalMarks
                );


            return awarded.ToString(
                "0.##"
            )
            + " / "
            + totalScore.ToString(
                "0.##"
            );
        }


        protected string GetAttemptStatusText(
            object attemptStatus,
            object awardedMarks,
            object totalMarks)
        {
            string status =
                NormalizeAttemptStatus(
                    attemptStatus
                );


            if (
                status == "pendinggrading"
            )
            {
                return "Pending Grading";
            }


            if (
                status == "inprogress"
            )
            {
                return "In Progress";
            }


            decimal awarded =
                Convert.ToDecimal(
                    awardedMarks
                );


            decimal total =
                Convert.ToDecimal(
                    totalMarks
                );


            decimal percentage =
                0;


            if (
                total > 0
            )
            {
                percentage =
                    awarded
                    / total
                    * 100;
            }


            return "Graded ("
                + percentage.ToString(
                    "0"
                )
                + "%)";
        }


        protected string GetAttemptStatusCss(
            object attemptStatus)
        {
            string status =
                NormalizeAttemptStatus(
                    attemptStatus
                );


            if (
                status == "pendinggrading"
            )
            {
                return
                    "attempt-status status-pending";
            }


            if (
                status == "inprogress"
            )
            {
                return
                    "attempt-status status-progress";
            }


            return
                "attempt-status status-graded";
        }


        protected string GetAttemptScoreCss(
            object attemptStatus)
        {
            string status =
                NormalizeAttemptStatus(
                    attemptStatus
                );


            if (
                status == "inprogress"
            )
            {
                return
                    "attempt-score attempt-progress-score";
            }


            return "attempt-score";
        }

        protected string GetQuizFormat(
            object mcqCount,
            object subjectiveCount)
        {
            int mcq =
                Convert.ToInt32(
                    mcqCount
                );


            int subjective =
                Convert.ToInt32(
                    subjectiveCount
                );


            if (
                mcq > 0
                &&
                subjective > 0
            )
            {
                return "Obj + Subj";
            }


            if (
                mcq > 0
            )
            {
                return "Objective";
            }


            if (
                subjective > 0
            )
            {
                return "Subjective";
            }


            return "No Questions";
        }


        protected string GetAttemptActionText(
            object attemptStatus)
        {
            string status =
                NormalizeAttemptStatus(
                    attemptStatus
                );


            return status == "inprogress"
                ? "Continue"
                : "View Result";
        }


        protected string GetAttemptActionCss(
            object attemptStatus)
        {
            string status =
                NormalizeAttemptStatus(
                    attemptStatus
                );


            if (
                status == "inprogress"
            )
            {
                return
                    "attempt-action-button "
                    + "attempt-action-primary";
            }


            return
                "attempt-action-button "
                + "attempt-action-light";
        }


        protected string GetAttemptActionUrl(
            object attemptId,
            object attemptStatus)
        {
            string status =
                NormalizeAttemptStatus(
                    attemptStatus
                );


            int id =
                Convert.ToInt32(
                    attemptId
                );


            if (
                status == "inprogress"
            )
            {
                return ResolveUrl(
                    "~/Student/StartQuiz.aspx?attemptId="
                    + id
                );
            }


            // Result page will be connected later.
            return "#";
        }


        // =========================================================
        // ATTEMPT SUBJECT ICON
        // =========================================================

        protected string GetAttemptIcon(
            object subjectName)
        {
            string subject =
                Convert.ToString(
                    subjectName
                )
                .Trim()
                .ToLower();


            switch (subject)
            {
                case "mathematics":

                    return "bi bi-calculator";


                case "sejarah":

                    return "bi bi-bank";


                case "science":

                    return "bi bi-flask";


                case "english":

                    return
                        "bi bi-alphabet-uppercase";


                case "bahasa melayu":

                    return "bi bi-book";


                default:

                    return "bi bi-journal-text";
            }
        }


        protected string GetAttemptIconCss(
            object subjectName)
        {
            string subject =
                Convert.ToString(
                    subjectName
                )
                .Trim()
                .ToLower();


            switch (subject)
            {
                case "mathematics":
                    return "attempt-icon-blue";

                case "sejarah":
                    return "attempt-icon-yellow";

                case "science":
                    return "attempt-icon-purple";

                case "english":
                    return "attempt-icon-green";

                case "bahasa melayu":
                    return "attempt-icon-pink";

                default:
                    return "attempt-icon-purple";
            }
        }

        protected bool CanDeleteAttempt(
            object attemptStatus)
        {
            string status =
                NormalizeAttemptStatus(
                    attemptStatus
                );


            return status ==
                "inprogress";
        }

        private void DeleteAttempt(
            int attemptId)
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
                'InProgress';
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
                'InProgress';
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
                        // VERIFY OWNERSHIP + STATUS
                        // =============================================

                        using (
                            SqlCommand verifyCommand =
                                new SqlCommand(
                                    verifySql,
                                    connection,
                                    transaction
                                )
                        )
                        {
                            verifyCommand.Parameters.Add(
                                "@AttemptId",
                                SqlDbType.Int
                            ).Value =
                                attemptId;


                            verifyCommand.Parameters.Add(
                                "@StudentUserId",
                                SqlDbType.Int
                            ).Value =
                                CurrentUserId;


                            int count =
                                Convert.ToInt32(
                                    verifyCommand
                                        .ExecuteScalar()
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
                        // DELETE SAVED ANSWERS FIRST
                        // =============================================

                        using (
                            SqlCommand answerCommand =
                                new SqlCommand(
                                    deleteAnswersSql,
                                    connection,
                                    transaction
                                )
                        )
                        {
                            answerCommand.Parameters.Add(
                                "@AttemptId",
                                SqlDbType.Int
                            ).Value =
                                attemptId;


                            answerCommand.ExecuteNonQuery();
                        }


                        // =============================================
                        // DELETE ATTEMPT
                        // =============================================

                        using (
                            SqlCommand attemptCommand =
                                new SqlCommand(
                                    deleteAttemptSql,
                                    connection,
                                    transaction
                                )
                        )
                        {
                            attemptCommand.Parameters.Add(
                                "@AttemptId",
                                SqlDbType.Int
                            ).Value =
                                attemptId;


                            attemptCommand.Parameters.Add(
                                "@StudentUserId",
                                SqlDbType.Int
                            ).Value =
                                CurrentUserId;


                            attemptCommand.ExecuteNonQuery();
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

        protected void rptAttempts_ItemCommand(
            object source,
            RepeaterCommandEventArgs e)
        {
            if (
                e.CommandName ==
                "DeleteAttempt"
            )
            {
                int attemptId =
                    Convert.ToInt32(
                        e.CommandArgument
                    );


                DeleteAttempt(
                    attemptId
                );


                // Return to My Attempts tab
                Response.Redirect(
                    "~/Student/Quizzes.aspx?tab=attempts"
                );
            }
        }
    }
}