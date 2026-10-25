using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;


namespace SPMaster.Admin
{
    public partial class ViewContent :
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
        // QUIZ ID
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
            // User must be logged in
            if (
                Session["UserId"] == null
            )
            {
                Response.Redirect(
                    "~/Login.aspx"
                );

                return;
            }


            // Must have valid QuizId
            if (
                QuizId <= 0
            )
            {
                ShowError(
                    "Unable to identify the selected quiz."
                );

                return;
            }


            if (!IsPostBack)
            {
                bool loaded =
                    LoadQuiz();


                if (
                    !loaded
                )
                {
                    ShowError(
                        "The selected quiz could not be found."
                    );

                    return;
                }


                LoadQuestions();
            }
        }



        // =========================================================
        // LOAD QUIZ
        //
        // IMPORTANT:
        // Admin can view BOTH:
        //
        // IsPublished = 1
        // IsPublished = 0
        //
        // Only archived quizzes are excluded.
        // =========================================================

        private bool LoadQuiz()
        {
            const string sql = @"
                SELECT

                    z.QuizId,

                    z.Title,

                    z.Description,

                    z.TimeLimitMinutes,

                    z.PassPercentage,

                    z.IsPublished,

                    z.CreatedAt,

                    s.SubjectName,

                    f.FormName,

                    u.FullName
                        AS LecturerName,

                    (
                        SELECT COUNT(*)

                        FROM QuizQuestions qq

                        WHERE
                            qq.QuizId =
                                z.QuizId
                    ) AS QuestionCount


                FROM Quizzes z


                INNER JOIN Subjects s
                    ON z.SubjectId =
                       s.SubjectId


                INNER JOIN Forms f
                    ON z.FormId =
                       f.FormId


                INNER JOIN Users u
                    ON z.CreatedByUserId =
                       u.UserId


                WHERE
                    z.QuizId =
                        @QuizId

                    AND

                    z.IsArchived = 0;
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
                        if (
                            !reader.Read()
                        )
                        {
                            return false;
                        }



                        // =========================================
                        // TITLE
                        // =========================================

                        lblQuizTitle.Text =
                            Convert.ToString(
                                reader["Title"]
                            );



                        // =========================================
                        // SUBJECT
                        // =========================================

                        lblSubject.Text =
                            Convert.ToString(
                                reader["SubjectName"]
                            );



                        // =========================================
                        // LECTURER
                        // =========================================

                        lblLecturer.Text =
                            Convert.ToString(
                                reader["LecturerName"]
                            );



                        // =========================================
                        // FORM
                        // =========================================

                        lblForm.Text =
                            Convert.ToString(
                                reader["FormName"]
                            );



                        // =========================================
                        // CREATED DATE
                        // =========================================

                        lblCreatedDate.Text =
                            Convert.ToDateTime(
                                reader["CreatedAt"]
                            ).ToString(
                                "dd MMM yyyy"
                            );



                        // =========================================
                        // QUESTION COUNT
                        // =========================================

                        int questionCount =
                            Convert.ToInt32(
                                reader["QuestionCount"]
                            );


                        lblQuestionCount.Text =
                            questionCount
                            +
                            (
                                questionCount == 1
                                ? " question"
                                : " questions"
                            );



                        // =========================================
                        // TIME LIMIT
                        // =========================================

                        int timeLimit =
                            Convert.ToInt32(
                                reader["TimeLimitMinutes"]
                            );


                        lblTimeLimit.Text =
                            timeLimit
                            +
                            (
                                timeLimit == 1
                                ? " minute"
                                : " minutes"
                            );



                        // =========================================
                        // PASS PERCENTAGE
                        // =========================================

                        decimal passPercentage =
                            Convert.ToDecimal(
                                reader["PassPercentage"]
                            );


                        lblPassPercentage.Text =
                            passPercentage
                                .ToString("0.##")
                            + "%";



                        // =========================================
                        // STATUS
                        // =========================================

                        bool isPublished =
                            Convert.ToBoolean(
                                reader["IsPublished"]
                            );


                        lblQuizStatus.Text =
                            isPublished
                            ? "Published"
                            : "Draft";


                        lblQuizStatus.CssClass =
                            isPublished
                            ? "view-status-badge published"
                            : "view-status-badge draft";



                        // =========================================
                        // DESCRIPTION
                        // =========================================

                        string description =
                            reader["Description"]
                                == DBNull.Value

                            ? ""

                            : Convert.ToString(
                                reader["Description"]
                            );


                        if (
                            !string.IsNullOrWhiteSpace(
                                description
                            )
                        )
                        {
                            lblDescription.Text =
                                description;


                            pnlDescription.Visible =
                                true;
                        }
                        else
                        {
                            pnlDescription.Visible =
                                false;
                        }



                        pnlQuiz.Visible =
                            true;


                        return true;
                    }
                }
            }
        }



        // =========================================================
        // LOAD QUESTIONS IN QUIZ
        //
        // Questions remain linked even when quiz is unpublished.
        // =========================================================

        private void LoadQuestions()
        {
            const string sql = @"
                SELECT

                    q.QuestionId,

                    q.QuestionText,

                    q.QuestionType,

                    q.Difficulty,

                    q.ModelAnswer,

                    q.Explanation,

                    q.MediaPath,

                    qq.QuestionOrder


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


            rptQuestions.DataSource =
                table;


            rptQuestions.DataBind();


            pnlNoQuestions.Visible =
                table.Rows.Count == 0;
        }



        // =========================================================
        // QUESTION ITEM DATA BOUND
        // =========================================================

        protected void rptQuestions_ItemDataBound(
            object sender,
            RepeaterItemEventArgs e)
        {
            if (
                e.Item.ItemType
                    != ListItemType.Item
                &&
                e.Item.ItemType
                    != ListItemType.AlternatingItem
            )
            {
                return;
            }


            DataRowView row =
                e.Item.DataItem
                    as DataRowView;


            if (
                row == null
            )
            {
                return;
            }



            int questionId =
                Convert.ToInt32(
                    row["QuestionId"]
                );


            string questionType =
                Convert.ToString(
                    row["QuestionType"]
                );


            bool isMcq =
                questionType.Equals(
                    "MCQ",
                    StringComparison.OrdinalIgnoreCase
                );



            // =====================================================
            // FIND CONTROLS
            // =====================================================

            Panel pnlQuestionMcq =
                e.Item.FindControl(
                    "pnlQuestionMcq"
                ) as Panel;


            Panel pnlQuestionSubjective =
                e.Item.FindControl(
                    "pnlQuestionSubjective"
                ) as Panel;


            Panel pnlQuestionExplanation =
                e.Item.FindControl(
                    "pnlQuestionExplanation"
                ) as Panel;


            Panel pnlQuestionMedia =
                e.Item.FindControl(
                    "pnlQuestionMedia"
                ) as Panel;


            Repeater rptOptions =
                e.Item.FindControl(
                    "rptOptions"
                ) as Repeater;


            Label lblModelAnswer =
                e.Item.FindControl(
                    "lblModelAnswer"
                ) as Label;


            Label lblExplanation =
                e.Item.FindControl(
                    "lblExplanation"
                ) as Label;


            Image imgQuestion =
                e.Item.FindControl(
                    "imgQuestion"
                ) as Image;



            // =====================================================
            // MCQ / SUBJECTIVE
            // =====================================================

            if (
                pnlQuestionMcq != null
            )
            {
                pnlQuestionMcq.Visible =
                    isMcq;
            }


            if (
                pnlQuestionSubjective != null
            )
            {
                pnlQuestionSubjective.Visible =
                    !isMcq;
            }



            // =====================================================
            // LOAD MCQ OPTIONS
            // =====================================================

            if (
                isMcq
                &&
                rptOptions != null
            )
            {
                LoadOptions(
                    questionId,
                    rptOptions
                );
            }



            // =====================================================
            // SUBJECTIVE MODEL ANSWER
            // =====================================================

            if (
                !isMcq
                &&
                lblModelAnswer != null
            )
            {
                string modelAnswer =
                    row["ModelAnswer"]
                        == DBNull.Value

                    ? ""

                    : Convert.ToString(
                        row["ModelAnswer"]
                    );


                lblModelAnswer.Text =
                    string.IsNullOrWhiteSpace(
                        modelAnswer
                    )

                    ? "No model answer provided."

                    : modelAnswer;
            }



            // =====================================================
            // EXPLANATION
            // =====================================================

            string explanation =
                row["Explanation"]
                    == DBNull.Value

                ? ""

                : Convert.ToString(
                    row["Explanation"]
                );


            if (
                pnlQuestionExplanation != null
                &&
                lblExplanation != null
            )
            {
                if (
                    !string.IsNullOrWhiteSpace(
                        explanation
                    )
                )
                {
                    lblExplanation.Text =
                        explanation;


                    pnlQuestionExplanation.Visible =
                        true;
                }
                else
                {
                    pnlQuestionExplanation.Visible =
                        false;
                }
            }



            // =====================================================
            // QUESTION IMAGE
            // =====================================================

            string mediaPath =
                row["MediaPath"]
                    == DBNull.Value

                ? ""

                : Convert.ToString(
                    row["MediaPath"]
                );


            if (
                pnlQuestionMedia != null
                &&
                imgQuestion != null
            )
            {
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


                    pnlQuestionMedia.Visible =
                        true;
                }
                else
                {
                    pnlQuestionMedia.Visible =
                        false;
                }
            }
        }



        // =========================================================
        // LOAD MCQ OPTIONS
        // =========================================================

        private void LoadOptions(
            int questionId,
            Repeater repeater)
        {
            const string sql = @"
                SELECT

                    OptionText,

                    IsCorrect,

                    DisplayOrder

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


            repeater.DataSource =
                table;


            repeater.DataBind();
        }



        // =========================================================
        // QUESTION TYPE DISPLAY
        // =========================================================

        protected string GetQuestionTypeDisplay(
            string questionType)
        {
            if (
                questionType.Equals(
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
        // ERROR
        // =========================================================

        private void ShowError(
            string message)
        {
            pnlQuiz.Visible =
                false;


            lblError.Text =
                message;


            pnlError.Visible =
                true;
        }
    }
}