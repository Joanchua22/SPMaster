using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace SPMaster.Lecturer
{
    public partial class GradeDetails : System.Web.UI.Page
    {
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


        private int CurrentUserId
        {
            get
            {
                return Convert.ToInt32(
                    Session["UserId"]
                );
            }
        }


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
                if (CurrentAttemptId <= 0)
                {
                    Response.Redirect(
                        "~/Lecturer/Grading.aspx"
                    );

                    return;
                }

                if (!LoadGradingAttempt())
                {
                    Response.Redirect(
                        "~/Lecturer/Grading.aspx"
                    );

                    return;
                }
            }
        }


        // =========================================================
        // LOAD GRADING ATTEMPT
        // =========================================================

        private bool LoadGradingAttempt()
        {
            bool loaded =
                LoadAttemptInformation();

            if (!loaded)
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
        // ATTEMPT INFORMATION
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
                        reader["QuizTitle"]
                        .ToString();

                    string studentName =
                        reader["StudentName"]
                        .ToString();

                    lblStudentName.Text =
                        Server.HtmlEncode(
                            studentName
                        );

                    string context =
                        quizTitle;

                    if (
                        reader["SessionId"]
                        != DBNull.Value
                    )
                    {
                        string sessionTitle =
                            reader["SessionTitle"]
                            .ToString();

                        context +=
                            " • "
                            + sessionTitle;
                    }

                    lblDetailContext.Text =
                        Server.HtmlEncode(
                            context
                        );

                    bool alreadyGraded =
                        reader["AttemptStatus"]
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

                    if (alreadyGraded)
                    {
                        lblAttemptBadge.Text =
                            "Attempt #"
                            + CurrentAttemptId
                            + " • Review Only";
                    }
                    else
                    {
                        lblAttemptBadge.Text =
                            "Attempt #"
                            + CurrentAttemptId;
                    }

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
                        ? awarded / maximum * 100
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
                    Question.MediaPath,

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

            table.Columns.Add(
                "HasQuestionImage",
                typeof(bool)
            );

            table.Columns.Add(
                "QuestionImageUrl",
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

                row["QuestionHeading"] =
                    itemNumber
                    + ". Subjective Question "
                    + questionOrder;


                decimal maxMarks =
                    Convert.ToDecimal(
                        row[
                            "MaxMarks"
                        ]
                    );

                row["MaxMarksText"] =
                    maxMarks.ToString(
                        "0.##"
                    )
                    + (
                        maxMarks == 1
                            ? " Mark"
                            : " Marks"
                    );


                row["QuestionText"] =
                    Server.HtmlEncode(
                        row["QuestionText"]
                        .ToString()
                    );


                string mediaPath =
                    row["MediaPath"]
                    == DBNull.Value

                    ? ""

                    : row["MediaPath"]
                        .ToString();


                bool hasImage =
                    !string.IsNullOrWhiteSpace(
                        mediaPath
                    );


                row["HasQuestionImage"] =
                    hasImage;


                row["QuestionImageUrl"] =
                    hasImage

                    ? GetMediaUrl(
                        mediaPath
                    )

                    : "";


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


                row["StudentAnswerText"] =
                    Server.HtmlEncode(
                        studentAnswer
                    );


                string modelAnswer =
                    row["ModelAnswer"]
                    == DBNull.Value

                    ? ""

                    : row["ModelAnswer"]
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


                row["ModelAnswerText"] =
                    Server.HtmlEncode(
                        modelAnswer
                    );


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


                if (
                    row["LecturerFeedback"]
                    == DBNull.Value
                )
                {
                    row["LecturerFeedback"] =
                        "";
                }


                itemNumber++;
            }


            rptSubjectiveQuestions.DataSource =
                table;

            rptSubjectiveQuestions.DataBind();


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

                btnFinalize.Visible =
                    false;
            }
            else
            {
                btnFinalize.Visible =
                    true;
            }


            return table.Rows.Count;
        }


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
                    txtMarks.ReadOnly =
                        true;
                }


                if (txtFeedback != null)
                {
                    txtFeedback.ReadOnly =
                        true;
                }
            }
        }


        // =========================================================
        // MEDIA URL
        // =========================================================

        private string GetMediaUrl(
            string mediaPath)
        {
            if (
                mediaPath.StartsWith(
                    "http://",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                mediaPath.StartsWith(
                    "https://",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return mediaPath;
            }


            if (
                mediaPath.StartsWith(
                    "~/"
                )
            )
            {
                return ResolveUrl(
                    mediaPath
                );
            }


            return ResolveUrl(
                "~/"
                + mediaPath.TrimStart('/')
            );
        }


        // =========================================================
        // TOTAL SCORE
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

                        ? awarded / total * 100

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
        // FINALISE
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


            if (
                IsAttemptAlreadyGraded(
                    CurrentAttemptId
                )
            )
            {
                LoadGradingAttempt();

                ShowDetailError(
                    "This result has already been finalised and can no longer be modified."
                );

                return;
            }


            List<GradeEntry> entries;


            if (
                !CollectGradeEntries(
                    out entries
                )
            )
            {
                ShowDetailError(
                    "Please enter a valid mark for every subjective question before publishing."
                );

                return;
            }


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


                        const string updateAttemptSql = @"
                            UPDATE QuizAttempts

                            SET
                                AttemptStatus =
                                    'Graded'

                            WHERE
                                AttemptId =
                                    @AttemptId

                                AND SubmittedAt
                                    IS NOT NULL

                                AND AttemptStatus =
                                    'PendingGrading';
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
                }
            }
        }


        // =========================================================
        // COLLECT GRADES
        // =========================================================

        private bool CollectGradeEntries(
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


                if (
                    string.IsNullOrWhiteSpace(
                        marksText
                    )
                )
                {
                    lblMarkError.Text =
                        "Please enter a mark.";

                    lblMarkError.Visible =
                        true;

                    valid =
                        false;

                    continue;
                }


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

                    continue;
                }


                if (parsedMarks < 0)
                {
                    lblMarkError.Text =
                        "Marks cannot be below 0.";

                    lblMarkError.Visible =
                        true;

                    valid =
                        false;

                    continue;
                }


                if (
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

                    continue;
                }


                GradeEntry entry =
                    new GradeEntry();


                entry.QuizQuestionId =
                    quizQuestionId;


                entry.MarksAwarded =
                    parsedMarks;


                entry.Feedback =
                    txtFeedback.Text
                    .Trim();


                entries.Add(
                    entry
                );
            }


            if (entries.Count == 0)
            {
                valid =
                    false;
            }


            return valid;
        }


        // =========================================================
        // MAXIMUM MARK
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
        // SAVE GRADES
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
                        entry.MarksAwarded;


                    command.Parameters.Add(
                        "@LecturerFeedback",
                        SqlDbType.NVarChar,
                        -1
                    ).Value =

                        string.IsNullOrWhiteSpace(
                            entry.Feedback
                        )

                        ? (object)DBNull.Value

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
        // ALREADY GRADED
        // =========================================================

        private bool IsAttemptAlreadyGraded(
            int attemptId)
        {
            if (attemptId <= 0)
            {
                return false;
            }


            const string sql = @"
                SELECT COUNT(*)

                FROM QuizAttempts

                WHERE
                    AttemptId =
                        @AttemptId

                    AND AttemptStatus =
                        'Graded';
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


                connection.Open();


                return Convert.ToInt32(
                    command.ExecuteScalar()
                ) > 0;
            }
        }


        // =========================================================
        // AUTHORIZATION
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


                return Convert.ToInt32(
                    command.ExecuteScalar()
                ) > 0;
            }
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
        // GRADE MODEL
        // =========================================================

        private class GradeEntry
        {
            public int QuizQuestionId
            {
                get;
                set;
            }


            public decimal MarksAwarded
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