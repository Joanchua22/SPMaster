using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;


namespace SPMaster.Lecturer
{
    public partial class Grading : System.Web.UI.Page
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
        // CURRENT ATTEMPT
        // =========================================================

        private int CurrentAttemptId
        {
            get
            {
                int attemptId;


                if (
                    int.TryParse(
                        Request.QueryString["attemptId"],
                        out attemptId
                    )
                )
                {
                    return attemptId;
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
            if (Session["UserId"] == null)
            {
                Response.Redirect(
                    "~/Login.aspx"
                );

                return;
            }


            if (!IsPostBack)
            {
                // =============================================
                // QUEUE MODE
                // =============================================

                if (CurrentAttemptId <= 0)
                {
                    ShowQueueView();

                    LoadQuizFilter();

                    BindGradingQueue();


                    if (
                        Request.QueryString[
                            "published"
                        ] == "1"
                    )
                    {
                        ShowMessage(
                            "Result published successfully."
                        );
                    }
                }

                // =============================================
                // INDIVIDUAL GRADING MODE
                // =============================================

                else
                {
                    ShowDetailView();

                    if (
                        !LoadGradingAttempt()
                    )
                    {
                        Response.Redirect(
                            "~/Lecturer/Grading.aspx"
                        );

                        return;
                    }
                }
            }
        }


        // =========================================================
        // VIEW CONTROL
        // =========================================================

        private void ShowQueueView()
        {
            pnlQueueView.Visible =
                true;

            pnlGradingView.Visible =
                false;
        }


        private void ShowDetailView()
        {
            pnlQueueView.Visible =
                false;

            pnlGradingView.Visible =
                true;
        }


        // =========================================================
        // LOAD QUIZ FILTER
        // =========================================================

        private void LoadQuizFilter()
        {
            const string sql = @"
                SELECT
                    Q.QuizId,
                    Q.Title,

                    COUNT(
                        DISTINCT QA.AttemptId
                    ) AS PendingCount

                FROM QuizAttempts QA

                INNER JOIN Quizzes Q
                    ON QA.QuizId =
                       Q.QuizId

                LEFT JOIN QuizSessions QS
                    ON QA.SessionId =
                       QS.SessionId

                WHERE
                    QA.AttemptStatus =
                        'PendingGrading'

                    AND
                    (
                        Q.CreatedByUserId =
                            @LecturerId

                        OR

                        QS.LecturerUserId =
                            @LecturerId
                    )

                GROUP BY
                    Q.QuizId,
                    Q.Title

                ORDER BY
                    Q.Title;
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


                SqlDataAdapter adapter =
                    new SqlDataAdapter(
                        command
                    );


                adapter.Fill(
                    table
                );
            }


            table.Columns.Add(
                "DisplayText",
                typeof(string)
            );


            int totalPending = 0;


            foreach (
                DataRow row
                in table.Rows
            )
            {
                int count =
                    Convert.ToInt32(
                        row["PendingCount"]
                    );


                totalPending +=
                    count;


                row["DisplayText"] =
                    row["Title"].ToString()
                    + " ("
                    + count
                    + " Pending)";
            }


            ddlQuizFilter.DataSource =
                table;

            ddlQuizFilter.DataTextField =
                "DisplayText";

            ddlQuizFilter.DataValueField =
                "QuizId";

            ddlQuizFilter.DataBind();


            ddlQuizFilter.Items.Insert(
                0,
                new ListItem(
                    "All Quizzes ("
                    + totalPending
                    + " Pending)",
                    ""
                )
            );
        }


        // =========================================================
        // QUIZ FILTER CHANGED
        // =========================================================

        protected void ddlQuizFilter_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            BindGradingQueue();
        }


        // =========================================================
        // BIND GRADING QUEUE
        // =========================================================

        private void BindGradingQueue()
        {
            int? selectedQuizId =
                null;


            int quizId;


            if (
                int.TryParse(
                    ddlQuizFilter.SelectedValue,
                    out quizId
                )
            )
            {
                selectedQuizId =
                    quizId;
            }


            const string sql = @"
                SELECT
                    QA.AttemptId,
                    QA.QuizId,
                    QA.SessionId,
                    QA.SubmittedAt,

                    Student.FullName
                        AS StudentName,

                    Q.Title
                        AS QuizTitle,

                    QS.SessionTitle,

                    SUM(
                        CASE

                            WHEN
                                UPPER(
                                    LTRIM(
                                        RTRIM(
                                            Question.QuestionType
                                        )
                                    )
                                ) <> 'MCQ'

                            THEN 1

                            ELSE 0

                        END
                    ) AS SubjectiveCount

                FROM QuizAttempts QA

                INNER JOIN Quizzes Q
                    ON QA.QuizId =
                       Q.QuizId

                INNER JOIN Users Student
                    ON QA.StudentUserId =
                       Student.UserId

                INNER JOIN QuizQuestions QQ
                    ON QA.QuizId =
                       QQ.QuizId

                INNER JOIN Questions Question
                    ON QQ.QuestionId =
                       Question.QuestionId

                LEFT JOIN QuizSessions QS
                    ON QA.SessionId =
                       QS.SessionId

                WHERE
                    QA.AttemptStatus =
                        'PendingGrading'

                    AND
                    (
                        Q.CreatedByUserId =
                            @LecturerId

                        OR

                        QS.LecturerUserId =
                            @LecturerId
                    )

                    AND
                    (
                        @QuizId IS NULL

                        OR

                        QA.QuizId =
                            @QuizId
                    )

                GROUP BY
                    QA.AttemptId,
                    QA.QuizId,
                    QA.SessionId,
                    QA.SubmittedAt,
                    Student.FullName,
                    Q.Title,
                    QS.SessionTitle

                HAVING
                    SUM(
                        CASE

                            WHEN
                                UPPER(
                                    LTRIM(
                                        RTRIM(
                                            Question.QuestionType
                                        )
                                    )
                                ) <> 'MCQ'

                            THEN 1

                            ELSE 0

                        END
                    ) > 0

                ORDER BY
                    QA.SubmittedAt ASC;
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


                command.Parameters.Add(
                    "@QuizId",
                    SqlDbType.Int
                ).Value =
                    selectedQuizId.HasValue

                    ? (object)
                        selectedQuizId.Value

                    : DBNull.Value;


                SqlDataAdapter adapter =
                    new SqlDataAdapter(
                        command
                    );


                adapter.Fill(
                    table
                );
            }


            // =====================================================
            // UI COLUMNS
            // =====================================================

            table.Columns.Add(
                "StudentInitials",
                typeof(string)
            );


            table.Columns.Add(
                "HasSession",
                typeof(bool)
            );


            table.Columns.Add(
                "SubjectiveCountText",
                typeof(string)
            );


            table.Columns.Add(
                "SubmittedAtText",
                typeof(string)
            );


            table.Columns.Add(
                "GradeUrl",
                typeof(string)
            );


            foreach (
                DataRow row
                in table.Rows
            )
            {
                string studentName =
                    row["StudentName"]
                        .ToString();


                row["StudentInitials"] =
                    GetInitials(
                        studentName
                    );


                row["StudentName"] =
                    Server.HtmlEncode(
                        studentName
                    );


                row["QuizTitle"] =
                    Server.HtmlEncode(
                        row["QuizTitle"]
                            .ToString()
                    );


                bool hasSession =
                    row["SessionId"]
                        != DBNull.Value;


                row["HasSession"] =
                    hasSession;


                if (hasSession)
                {
                    row["SessionTitle"] =
                        Server.HtmlEncode(
                            row[
                                "SessionTitle"
                            ]
                            .ToString()
                        );
                }


                int subjectiveCount =
                    Convert.ToInt32(
                        row[
                            "SubjectiveCount"
                        ]
                    );


                row[
                    "SubjectiveCountText"
                ] =
                    subjectiveCount
                    + (
                        subjectiveCount == 1
                            ? " Subjective Item"
                            : " Subjective Items"
                    );


                DateTime submittedAt =
                    Convert.ToDateTime(
                        row["SubmittedAt"]
                    );


                row[
                    "SubmittedAtText"
                ] =
                    submittedAt.ToString(
                        "dd MMM yyyy, hh:mm tt"
                    );


                row["GradeUrl"] =
                    ResolveUrl(
                        "~/Lecturer/Grading.aspx"
                        + "?attemptId="
                        + row["AttemptId"]
                    );
            }


            rptGradingQueue.DataSource =
                table;

            rptGradingQueue.DataBind();


            pnlNoPending.Visible =
                table.Rows.Count == 0;


            pnlQueueList.Visible =
                table.Rows.Count > 0;
        }


        // =========================================================
        // LOAD INDIVIDUAL ATTEMPT
        // =========================================================

        private bool LoadGradingAttempt()
        {
            bool attemptLoaded =
                LoadAttemptInformation();


            if (!attemptLoaded)
            {
                return false;
            }


            LoadObjectiveSummary();


            int subjectiveCount =
                LoadSubjectiveQuestions();


            pnlNoSubjective.Visible =
                subjectiveCount == 0;


            pnlGradingFooter.Visible =
                subjectiveCount > 0;


            LoadProvisionalScore();


            return true;
        }


        // =========================================================
        // LOAD ATTEMPT INFORMATION
        // =========================================================

        private bool LoadAttemptInformation()
        {
            const string sql = @"
                SELECT
                    QA.AttemptId,
                    QA.QuizId,
                    QA.StudentUserId,
                    QA.SessionId,
                    QA.SubmittedAt,
                    QA.AttemptStatus,

                    Q.Title
                        AS QuizTitle,

                    Student.FullName
                        AS StudentName,

                    QS.SessionTitle

                FROM QuizAttempts QA

                INNER JOIN Quizzes Q
                    ON QA.QuizId =
                       Q.QuizId

                INNER JOIN Users Student
                    ON QA.StudentUserId =
                       Student.UserId

                LEFT JOIN QuizSessions QS
                    ON QA.SessionId =
                       QS.SessionId

                WHERE
                    QA.AttemptId =
                        @AttemptId

                    AND QA.SubmittedAt
                        IS NOT NULL

                    AND
                    (
                        Q.CreatedByUserId =
                            @LecturerId

                        OR

                        QS.LecturerUserId =
                            @LecturerId
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
                    "@AttemptId",
                    SqlDbType.Int
                ).Value =
                    CurrentAttemptId;


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
                    if (!reader.Read())
                    {
                        return false;
                    }


                    string quizTitle =
                        reader[
                            "QuizTitle"
                        ].ToString();


                    string studentName =
                        reader[
                            "StudentName"
                        ].ToString();


                    lblStudentName.Text =
                        Server.HtmlEncode(
                            studentName
                        );


                    lblAttemptBadge.Text =
                        "Attempt #"
                        + CurrentAttemptId;


                    string context =
                        quizTitle;


                    if (
                        reader["SessionId"]
                        != DBNull.Value
                    )
                    {
                        string sessionTitle =
                            reader[
                                "SessionTitle"
                            ].ToString();


                        context +=
                            " • "
                            + sessionTitle;
                    }


                    lblDetailContext.Text =
                        Server.HtmlEncode(
                            context
                        );


                    bool alreadyGraded =
                        reader[
                            "AttemptStatus"
                        ]
                        .ToString()
                        .Equals(
                            "Graded",
                            StringComparison
                                .OrdinalIgnoreCase
                        );


                    ViewState[
                        "AttemptAlreadyGraded"
                    ] =
                        alreadyGraded;


                    return true;
                }
            }
        }


        // =========================================================
        // OBJECTIVE SCORE
        // =========================================================

        private void LoadObjectiveSummary()
        {
            const string sql = @"
                SELECT

                    ISNULL(
                        SUM(
                            CASE

                                WHEN
                                    UPPER(
                                        LTRIM(
                                            RTRIM(
                                                Question.QuestionType
                                            )
                                        )
                                    ) = 'MCQ'

                                THEN
                                    ISNULL(
                                        SA.MarksAwarded,
                                        0
                                    )

                                ELSE 0

                            END
                        ),
                        0
                    ) AS AwardedMarks,


                    ISNULL(
                        SUM(
                            CASE

                                WHEN
                                    UPPER(
                                        LTRIM(
                                            RTRIM(
                                                Question.QuestionType
                                            )
                                        )
                                    ) = 'MCQ'

                                THEN QQ.MaxMarks

                                ELSE 0

                            END
                        ),
                        0
                    ) AS MaximumMarks

                FROM QuizAttempts QA

                INNER JOIN QuizQuestions QQ
                    ON QA.QuizId =
                       QQ.QuizId

                INNER JOIN Questions Question
                    ON QQ.QuestionId =
                       Question.QuestionId

                LEFT JOIN StudentAnswers SA
                    ON SA.AttemptId =
                       QA.AttemptId

                    AND SA.QuizQuestionId =
                       QQ.QuizQuestionId

                WHERE
                    QA.AttemptId =
                        @AttemptId;
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
                    "@AttemptId",
                    SqlDbType.Int
                ).Value =
                    CurrentAttemptId;


                connection.Open();


                using (
                    SqlDataReader reader =
                        command.ExecuteReader()
                )
                {
                    if (!reader.Read())
                    {
                        lblObjectiveScore.Text =
                            "0 / 0 Marks";

                        return;
                    }


                    decimal awarded =
                        Convert.ToDecimal(
                            reader[
                                "AwardedMarks"
                            ]
                        );


                    decimal maximum =
                        Convert.ToDecimal(
                            reader[
                                "MaximumMarks"
                            ]
                        );


                    decimal percentage =
                        maximum > 0

                        ? awarded
                          / maximum
                          * 100

                        : 0;


                    lblObjectiveScore.Text =
                        awarded.ToString(
                            "0.##"
                        )
                        + " / "
                        + maximum.ToString(
                            "0.##"
                        )
                        + " Marks ("
                        + percentage.ToString(
                            "0.##"
                        )
                        + "%)";
                }
            }
        }


        // =========================================================
        // SUBJECTIVE QUESTIONS
        // =========================================================

        private int LoadSubjectiveQuestions()
        {
            const string sql = @"
                SELECT
                    QQ.QuizQuestionId,
                    QQ.QuestionOrder,
                    QQ.MaxMarks,

                    Question.QuestionText,
                    Question.ModelAnswer,

                    SA.AnswerId,
                    SA.AnswerText,
                    SA.MarksAwarded,
                    SA.LecturerFeedback

                FROM QuizAttempts QA

                INNER JOIN QuizQuestions QQ
                    ON QA.QuizId =
                       QQ.QuizId

                INNER JOIN Questions Question
                    ON QQ.QuestionId =
                       Question.QuestionId

                LEFT JOIN StudentAnswers SA
                    ON SA.AttemptId =
                       QA.AttemptId

                    AND SA.QuizQuestionId =
                       QQ.QuizQuestionId

                WHERE
                    QA.AttemptId =
                        @AttemptId

                    AND
                    UPPER(
                        LTRIM(
                            RTRIM(
                                Question.QuestionType
                            )
                        )
                    ) <> 'MCQ'

                ORDER BY
                    QQ.QuestionOrder;
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
                    "@AttemptId",
                    SqlDbType.Int
                ).Value =
                    CurrentAttemptId;


                SqlDataAdapter adapter =
                    new SqlDataAdapter(
                        command
                    );


                adapter.Fill(
                    table
                );
            }


            // =====================================================
            // UI COLUMNS
            // =====================================================

            table.Columns.Add(
                "QuestionHeading",
                typeof(string)
            );


            table.Columns.Add(
                "MaxMarksText",
                typeof(string)
            );


            table.Columns.Add(
                "StudentAnswerText",
                typeof(string)
            );


            table.Columns.Add(
                "ModelAnswerText",
                typeof(string)
            );


            table.Columns.Add(
                "MarksText",
                typeof(string)
            );


            int itemNumber =
                2;


            foreach (
                DataRow row
                in table.Rows
            )
            {
                int questionOrder =
                    Convert.ToInt32(
                        row[
                            "QuestionOrder"
                        ]
                    );


                row[
                    "QuestionHeading"
                ] =
                    itemNumber
                    + ". Subjective Question "
                    + questionOrder;


                decimal maxMarks =
                    Convert.ToDecimal(
                        row["MaxMarks"]
                    );


                row[
                    "MaxMarksText"
                ] =
                    maxMarks.ToString(
                        "0.##"
                    )
                    + (
                        maxMarks == 1
                            ? " Mark"
                            : " Marks"
                    );


                // =============================================
                // QUESTION
                // =============================================

                row["QuestionText"] =
                    Server.HtmlEncode(
                        row[
                            "QuestionText"
                        ]
                        .ToString()
                    );


                // =============================================
                // STUDENT ANSWER
                // =============================================

                string studentAnswer =
                    row["AnswerText"]
                        == DBNull.Value

                    ? ""

                    : row["AnswerText"]
                        .ToString();


                if (
                    string.IsNullOrWhiteSpace(
                        studentAnswer
                    )
                )
                {
                    studentAnswer =
                        "No answer submitted.";
                }


                row[
                    "StudentAnswerText"
                ] =
                    Server.HtmlEncode(
                        studentAnswer
                    );


                // =============================================
                // MODEL ANSWER
                // =============================================

                string modelAnswer =
                    row["ModelAnswer"]
                        == DBNull.Value

                    ? ""

                    : row[
                        "ModelAnswer"
                    ]
                    .ToString();


                if (
                    string.IsNullOrWhiteSpace(
                        modelAnswer
                    )
                )
                {
                    modelAnswer =
                        "No model answer was provided for this question.";
                }


                row[
                    "ModelAnswerText"
                ] =
                    Server.HtmlEncode(
                        modelAnswer
                    );


                // =============================================
                // EXISTING MARK
                // =============================================

                row["MarksText"] =
                    row["MarksAwarded"]
                        == DBNull.Value

                    ? ""

                    : Convert.ToDecimal(
                        row[
                            "MarksAwarded"
                        ]
                    )
                    .ToString(
                        "0.##"
                    );


                // =============================================
                // EXISTING FEEDBACK
                // =============================================

                if (
                    row[
                        "LecturerFeedback"
                    ]
                    == DBNull.Value
                )
                {
                    row[
                        "LecturerFeedback"
                    ] =
                        "";
                }


                itemNumber++;
            }


            rptSubjectiveQuestions.DataSource =
                table;

            rptSubjectiveQuestions.DataBind();


            // =====================================================
            // READ-ONLY IF ALREADY GRADED
            // =====================================================

            bool alreadyGraded =
                ViewState[
                    "AttemptAlreadyGraded"
                ] != null

                &&
                Convert.ToBoolean(
                    ViewState[
                        "AttemptAlreadyGraded"
                    ]
                );


            if (alreadyGraded)
            {
                SetGradingFieldsReadOnly();

                btnSaveDraft.Visible =
                    false;

                btnFinalize.Visible =
                    false;
            }
            else
            {
                btnSaveDraft.Visible =
                    true;

                btnFinalize.Visible =
                    true;
            }


            return table.Rows.Count;
        }


        // =========================================================
        // MAKE PUBLISHED RESULT READ ONLY
        // =========================================================

        private void SetGradingFieldsReadOnly()
        {
            foreach (
                RepeaterItem item
                in rptSubjectiveQuestions.Items
            )
            {
                TextBox txtMarks =
                    item.FindControl(
                        "txtAwardedMarks"
                    )
                    as TextBox;


                TextBox txtFeedback =
                    item.FindControl(
                        "txtLecturerFeedback"
                    )
                    as TextBox;


                if (txtMarks != null)
                {
                    txtMarks.Enabled =
                        false;
                }


                if (txtFeedback != null)
                {
                    txtFeedback.Enabled =
                        false;
                }
            }
        }


        // =========================================================
        // PROVISIONAL SCORE
        // =========================================================

        private void LoadProvisionalScore()
        {
            const string sql = @"
                SELECT

                    ISNULL(
                        SUM(
                            ISNULL(
                                SA.MarksAwarded,
                                0
                            )
                        ),
                        0
                    ) AS AwardedMarks,


                    ISNULL(
                        SUM(
                            QQ.MaxMarks
                        ),
                        0
                    ) AS TotalMarks

                FROM QuizAttempts QA

                INNER JOIN QuizQuestions QQ
                    ON QA.QuizId =
                       QQ.QuizId

                LEFT JOIN StudentAnswers SA
                    ON SA.AttemptId =
                       QA.AttemptId

                    AND SA.QuizQuestionId =
                       QQ.QuizQuestionId

                WHERE
                    QA.AttemptId =
                        @AttemptId;
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
                    "@AttemptId",
                    SqlDbType.Int
                ).Value =
                    CurrentAttemptId;


                connection.Open();


                using (
                    SqlDataReader reader =
                        command.ExecuteReader()
                )
                {
                    if (!reader.Read())
                    {
                        lblProvisionalScore.Text =
                            "0 / 0 Total";

                        return;
                    }


                    decimal awarded =
                        Convert.ToDecimal(
                            reader[
                                "AwardedMarks"
                            ]
                        );


                    decimal total =
                        Convert.ToDecimal(
                            reader[
                                "TotalMarks"
                            ]
                        );


                    decimal percentage =
                        total > 0

                        ? awarded
                          / total
                          * 100

                        : 0;


                    lblProvisionalScore.Text =
                        awarded.ToString(
                            "0.##"
                        )
                        + " / "
                        + total.ToString(
                            "0.##"
                        )
                        + " Total ("
                        + percentage.ToString(
                            "0.##"
                        )
                        + "%)";
                }
            }
        }


        // =========================================================
        // SAVE DRAFT
        // =========================================================

        protected void btnSaveDraft_Click(
            object sender,
            EventArgs e)
        {
            pnlDetailMessage.Visible =
                false;


            if (
                !CanGradeAttempt(
                    CurrentAttemptId
                )
            )
            {
                ShowDetailError(
                    "You do not have permission to grade this submission."
                );

                return;
            }


            List<GradeEntry> entries;


            if (
                !CollectGradeEntries(
                    false,
                    out entries
                )
            )
            {
                ShowDetailError(
                    "Please correct the grading errors shown below."
                );

                return;
            }


            try
            {
                SaveGradeEntries(
                    entries
                );


                LoadGradingAttempt();


                ShowDetailSuccess(
                    "Draft grading saved successfully."
                );
            }
            catch (SqlException)
            {
                ShowDetailError(
                    "Unable to save the grading draft. Please try again."
                );
            }
            catch (Exception)
            {
                ShowDetailError(
                    "Something went wrong while saving the grading draft."
                );
            }
        }


        // =========================================================
        // FINALISE AND PUBLISH
        // =========================================================

        protected void btnFinalize_Click(
            object sender,
            EventArgs e)
        {
            pnlDetailMessage.Visible =
                false;


            if (
                !CanGradeAttempt(
                    CurrentAttemptId
                )
            )
            {
                ShowDetailError(
                    "You do not have permission to grade this submission."
                );

                return;
            }


            List<GradeEntry> entries;


            // true = every subjective question requires marks
            if (
                !CollectGradeEntries(
                    true,
                    out entries
                )
            )
            {
                ShowDetailError(
                    "Please enter a valid mark for every subjective question before publishing."
                );

                return;
            }


            SqlConnection connection =
                new SqlConnection(
                    ConnectionString
                );


            connection.Open();


            SqlTransaction transaction =
                connection.BeginTransaction();


            try
            {
                // =============================================
                // SAVE ALL SUBJECTIVE GRADES
                // =============================================

                SaveGradeEntries(
                    entries,
                    connection,
                    transaction
                );


                // =============================================
                // PUBLISH ATTEMPT
                // =============================================

                const string updateAttemptSql = @"
                    UPDATE QuizAttempts

                    SET
                        AttemptStatus =
                            'Graded'

                    WHERE
                        AttemptId =
                            @AttemptId

                        AND SubmittedAt
                            IS NOT NULL;
                ";


                using (
                    SqlCommand command =
                        new SqlCommand(
                            updateAttemptSql,
                            connection,
                            transaction
                        )
                )
                {
                    command.Parameters.Add(
                        "@AttemptId",
                        SqlDbType.Int
                    ).Value =
                        CurrentAttemptId;


                    int rows =
                        command.ExecuteNonQuery();


                    if (rows == 0)
                    {
                        throw new Exception(
                            "Unable to publish the result."
                        );
                    }
                }


                transaction.Commit();


                Response.Redirect(
                    "~/Lecturer/Grading.aspx?published=1",
                    false
                );


                Context
                    .ApplicationInstance
                    .CompleteRequest();
            }
            catch (SqlException)
            {
                transaction.Rollback();


                ShowDetailError(
                    "Unable to publish the result. Please try again."
                );
            }
            catch (Exception)
            {
                transaction.Rollback();


                ShowDetailError(
                    "Something went wrong while publishing the result."
                );
            }
            finally
            {
                connection.Close();

                connection.Dispose();
            }
        }


        // =========================================================
        // COLLECT + VALIDATE SUBJECTIVE MARKS
        // =========================================================

        private bool CollectGradeEntries(
            bool requireAllMarks,
            out List<GradeEntry> entries)
        {
            entries =
                new List<GradeEntry>();


            bool valid =
                true;


            foreach (
                RepeaterItem item
                in rptSubjectiveQuestions.Items
            )
            {
                HiddenField hfQuizQuestionId =
                    item.FindControl(
                        "hfQuizQuestionId"
                    )
                    as HiddenField;


                TextBox txtMarks =
                    item.FindControl(
                        "txtAwardedMarks"
                    )
                    as TextBox;


                TextBox txtFeedback =
                    item.FindControl(
                        "txtLecturerFeedback"
                    )
                    as TextBox;


                Label lblMarkError =
                    item.FindControl(
                        "lblMarkError"
                    )
                    as Label;


                if (
                    hfQuizQuestionId == null
                    ||
                    txtMarks == null
                    ||
                    txtFeedback == null
                    ||
                    lblMarkError == null
                )
                {
                    valid =
                        false;

                    continue;
                }


                lblMarkError.Visible =
                    false;

                lblMarkError.Text =
                    "";


                int quizQuestionId;


                if (
                    !int.TryParse(
                        hfQuizQuestionId.Value,
                        out quizQuestionId
                    )
                )
                {
                    lblMarkError.Text =
                        "Invalid question.";

                    lblMarkError.Visible =
                        true;

                    valid =
                        false;

                    continue;
                }


                decimal? maximumMarks =
                    GetMaximumMarks(
                        CurrentAttemptId,
                        quizQuestionId
                    );


                if (!maximumMarks.HasValue)
                {
                    lblMarkError.Text =
                        "Unable to validate this question.";

                    lblMarkError.Visible =
                        true;

                    valid =
                        false;

                    continue;
                }


                string marksText =
                    txtMarks.Text
                        .Trim();


                decimal? awardedMarks =
                    null;


                // =============================================
                // EMPTY MARK
                // =============================================

                if (
                    string.IsNullOrWhiteSpace(
                        marksText
                    )
                )
                {
                    if (requireAllMarks)
                    {
                        lblMarkError.Text =
                            "Please enter a mark.";

                        lblMarkError.Visible =
                            true;

                        valid =
                            false;
                    }
                }

                // =============================================
                // INVALID NUMBER
                // =============================================

                else
                {
                    decimal parsedMarks;


                    if (
                        !decimal.TryParse(
                            marksText,
                            out parsedMarks
                        )
                    )
                    {
                        lblMarkError.Text =
                            "Enter a valid number.";

                        lblMarkError.Visible =
                            true;

                        valid =
                            false;
                    }

                    // =========================================
                    // BELOW ZERO
                    // =========================================

                    else if (
                        parsedMarks < 0
                    )
                    {
                        lblMarkError.Text =
                            "Marks cannot be below 0.";

                        lblMarkError.Visible =
                            true;

                        valid =
                            false;
                    }

                    // =========================================
                    // ABOVE MAXIMUM
                    // =========================================

                    else if (
                        parsedMarks
                        > maximumMarks.Value
                    )
                    {
                        lblMarkError.Text =
                            "Maximum mark is "
                            + maximumMarks.Value
                                .ToString(
                                    "0.##"
                                )
                            + ".";

                        lblMarkError.Visible =
                            true;

                        valid =
                            false;
                    }

                    else
                    {
                        awardedMarks =
                            parsedMarks;
                    }
                }


                GradeEntry entry =
                    new GradeEntry();


                entry.QuizQuestionId =
                    quizQuestionId;


                entry.MarksAwarded =
                    awardedMarks;


                entry.Feedback =
                    txtFeedback.Text
                        .Trim();


                entries.Add(
                    entry
                );
            }


            if (
                entries.Count == 0
            )
            {
                valid =
                    false;
            }


            return valid;
        }


        // =========================================================
        // GET MAXIMUM MARKS FROM DATABASE
        //
        // Do not trust the hidden field value from the browser.
        // =========================================================

        private decimal? GetMaximumMarks(
            int attemptId,
            int quizQuestionId)
        {
            const string sql = @"
                SELECT
                    QQ.MaxMarks

                FROM QuizAttempts QA

                INNER JOIN QuizQuestions QQ
                    ON QA.QuizId =
                       QQ.QuizId

                INNER JOIN Questions Question
                    ON QQ.QuestionId =
                       Question.QuestionId

                WHERE
                    QA.AttemptId =
                        @AttemptId

                    AND QQ.QuizQuestionId =
                        @QuizQuestionId

                    AND
                    UPPER(
                        LTRIM(
                            RTRIM(
                                Question.QuestionType
                            )
                        )
                    ) <> 'MCQ';
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
                    "@AttemptId",
                    SqlDbType.Int
                ).Value =
                    attemptId;


                command.Parameters.Add(
                    "@QuizQuestionId",
                    SqlDbType.Int
                ).Value =
                    quizQuestionId;


                connection.Open();


                object result =
                    command.ExecuteScalar();


                if (
                    result == null
                    ||
                    result == DBNull.Value
                )
                {
                    return null;
                }


                return Convert.ToDecimal(
                    result
                );
            }
        }


        // =========================================================
        // SAVE GRADE ENTRIES
        // =========================================================

        private void SaveGradeEntries(
            List<GradeEntry> entries)
        {
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
                        SaveGradeEntries(
                            entries,
                            connection,
                            transaction
                        );


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


        // =========================================================
        // SAVE GRADE ENTRIES USING TRANSACTION
        // =========================================================

        private void SaveGradeEntries(
            List<GradeEntry> entries,
            SqlConnection connection,
            SqlTransaction transaction)
        {
            const string sql = @"
                IF EXISTS
                (
                    SELECT 1

                    FROM StudentAnswers

                    WHERE
                        AttemptId =
                            @AttemptId

                        AND QuizQuestionId =
                            @QuizQuestionId
                )

                BEGIN

                    UPDATE StudentAnswers

                    SET
                        MarksAwarded =
                            @MarksAwarded,

                        LecturerFeedback =
                            @LecturerFeedback,

                        GradedByUserId =
                            @LecturerId,

                        GradedAt =
                            GETDATE()

                    WHERE
                        AttemptId =
                            @AttemptId

                        AND QuizQuestionId =
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
                        MarksAwarded,
                        LecturerFeedback,
                        GradedByUserId,
                        GradedAt
                    )

                    VALUES
                    (
                        @AttemptId,
                        @QuizQuestionId,
                        NULL,
                        NULL,
                        @MarksAwarded,
                        @LecturerFeedback,
                        @LecturerId,
                        GETDATE()
                    );

                END
            ";


            foreach (
                GradeEntry entry
                in entries
            )
            {
                using (
                    SqlCommand command =
                        new SqlCommand(
                            sql,
                            connection,
                            transaction
                        )
                )
                {
                    command.Parameters.Add(
                        "@AttemptId",
                        SqlDbType.Int
                    ).Value =
                        CurrentAttemptId;


                    command.Parameters.Add(
                        "@QuizQuestionId",
                        SqlDbType.Int
                    ).Value =
                        entry.QuizQuestionId;


                    SqlParameter marksParameter =
                        command.Parameters.Add(
                            "@MarksAwarded",
                            SqlDbType.Decimal
                        );


                    marksParameter.Precision =
                        6;

                    marksParameter.Scale =
                        2;


                    marksParameter.Value =
                        entry.MarksAwarded
                            .HasValue

                        ? (object)
                            entry.MarksAwarded
                                .Value

                        : DBNull.Value;


                    command.Parameters.Add(
                        "@LecturerFeedback",
                        SqlDbType.NVarChar,
                        -1
                    ).Value =
                        string.IsNullOrWhiteSpace(
                            entry.Feedback
                        )

                        ? (object)
                            DBNull.Value

                        : entry.Feedback;


                    command.Parameters.Add(
                        "@LecturerId",
                        SqlDbType.Int
                    ).Value =
                        CurrentUserId;


                    command.ExecuteNonQuery();
                }
            }
        }


        // =========================================================
        // CHECK LECTURER CAN GRADE ATTEMPT
        //
        // Allowed when:
        // 1. Lecturer owns the quiz
        // OR
        // 2. Lecturer hosted the session
        // =========================================================

        private bool CanGradeAttempt(
            int attemptId)
        {
            if (attemptId <= 0)
            {
                return false;
            }


            const string sql = @"
                SELECT COUNT(*)

                FROM QuizAttempts QA

                INNER JOIN Quizzes Q
                    ON QA.QuizId =
                       Q.QuizId

                LEFT JOIN QuizSessions QS
                    ON QA.SessionId =
                       QS.SessionId

                WHERE
                    QA.AttemptId =
                        @AttemptId

                    AND QA.SubmittedAt
                        IS NOT NULL

                    AND
                    (
                        Q.CreatedByUserId =
                            @LecturerId

                        OR

                        QS.LecturerUserId =
                            @LecturerId
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
                    "@AttemptId",
                    SqlDbType.Int
                ).Value =
                    attemptId;


                command.Parameters.Add(
                    "@LecturerId",
                    SqlDbType.Int
                ).Value =
                    CurrentUserId;


                connection.Open();


                return
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    )
                    > 0;
            }
        }


        // =========================================================
        // STUDENT INITIALS
        // =========================================================

        private string GetInitials(
            string fullName)
        {
            if (
                string.IsNullOrWhiteSpace(
                    fullName
                )
            )
            {
                return "S";
            }


            string[] parts =
                fullName.Split(
                    new char[]
                    {
                        ' '
                    },
                    StringSplitOptions
                        .RemoveEmptyEntries
                );


            if (parts.Length == 1)
            {
                return parts[0]
                    .Substring(
                        0,
                        1
                    )
                    .ToUpper();
            }


            return
                (
                    parts[0]
                        .Substring(
                            0,
                            1
                        )

                    +

                    parts[
                        parts.Length - 1
                    ]
                    .Substring(
                        0,
                        1
                    )
                )
                .ToUpper();
        }


        // =========================================================
        // QUEUE MESSAGE
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
        // DETAIL ERROR
        // =========================================================

        private void ShowDetailError(
            string message)
        {
            lblDetailMessage.Text =
                message;


            pnlDetailMessage.CssClass =
                "grading-detail-message "
                + "grading-detail-error";


            pnlDetailMessage.Visible =
                true;
        }


        // =========================================================
        // DETAIL SUCCESS
        // =========================================================

        private void ShowDetailSuccess(
            string message)
        {
            lblDetailMessage.Text =
                message;


            pnlDetailMessage.CssClass =
                "grading-detail-message "
                + "grading-detail-success";


            pnlDetailMessage.Visible =
                true;
        }


        // =========================================================
        // GRADE MODEL
        // =========================================================

        private class GradeEntry
        {
            public int QuizQuestionId
            {
                get;
                set;
            }


            public decimal? MarksAwarded
            {
                get;
                set;
            }


            public string Feedback
            {
                get;
                set;
            }
        }
    }
}