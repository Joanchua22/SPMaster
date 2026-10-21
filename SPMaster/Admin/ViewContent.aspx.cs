using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


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
        // QUESTION ID
        // =========================================================

        private int QuestionId
        {
            get
            {
                int questionId;


                if (
                    int.TryParse(
                        Request.QueryString["id"],
                        out questionId
                    )
                )
                {
                    return questionId;
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
            if (
                Session["UserId"] == null
            )
            {
                Response.Redirect(
                    "~/Login.aspx"
                );

                return;
            }


            if (
                QuestionId <= 0
            )
            {
                ShowError(
                    "Unable to identify the selected question."
                );

                return;
            }


            if (!IsPostBack)
            {
                bool loaded =
                    LoadQuestion();


                if (!loaded)
                {
                    ShowError(
                        "The selected question could not be found."
                    );

                    return;
                }


                LoadOptions();

                LoadPublishedQuizzes();
            }
        }


        // =========================================================
        // LOAD QUESTION
        // =========================================================

        private bool LoadQuestion()
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

                    q.CreatedAt,

                    s.SubjectName,

                    u.FullName
                        AS LecturerName


                FROM Questions q


                INNER JOIN Subjects s
                    ON q.SubjectId =
                       s.SubjectId


                INNER JOIN Users u
                    ON q.CreatedByUserId =
                       u.UserId


                WHERE
                    q.QuestionId =
                        @QuestionId

                    AND
                    q.IsArchived = 0;
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
                        "@QuestionId",
                        SqlDbType.Int
                    ).Value =
                        QuestionId;


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


                        string questionType =
                            Convert.ToString(
                                reader[
                                    "QuestionType"
                                ]
                            );


                        lblQuestionText.Text =
                            Convert.ToString(
                                reader[
                                    "QuestionText"
                                ]
                            );


                        lblSubject.Text =
                            Convert.ToString(
                                reader[
                                    "SubjectName"
                                ]
                            );


                        lblLecturer.Text =
                            Convert.ToString(
                                reader[
                                    "LecturerName"
                                ]
                            );


                        lblQuestionType.Text =
                            GetQuestionTypeDisplay(
                                questionType
                            );


                        lblDifficulty.Text =
                            Convert.ToString(
                                reader[
                                    "Difficulty"
                                ]
                            );


                        lblCreatedDate.Text =
                            Convert.ToDateTime(
                                reader[
                                    "CreatedAt"
                                ]
                            ).ToString(
                                "dd MMM yyyy"
                            );


                        // =========================================
                        // QUESTION TYPE
                        // =========================================

                        bool isMcq =
                            questionType.Equals(
                                "MCQ",
                                StringComparison
                                    .OrdinalIgnoreCase
                            );


                        pnlMcq.Visible =
                            isMcq;


                        pnlSubjective.Visible =
                            !isMcq;


                        // =========================================
                        // MODEL ANSWER
                        // =========================================

                        string modelAnswer =
                            reader[
                                "ModelAnswer"
                            ] == DBNull.Value

                            ? ""

                            : Convert.ToString(
                                reader[
                                    "ModelAnswer"
                                ]
                            );


                        lblModelAnswer.Text =
                            string.IsNullOrWhiteSpace(
                                modelAnswer
                            )

                            ? "No model answer provided."

                            : modelAnswer;


                        // =========================================
                        // EXPLANATION
                        // =========================================

                        string explanation =
                            reader[
                                "Explanation"
                            ] == DBNull.Value

                            ? ""

                            : Convert.ToString(
                                reader[
                                    "Explanation"
                                ]
                            );


                        if (
                            !string.IsNullOrWhiteSpace(
                                explanation
                            )
                        )
                        {
                            lblExplanation.Text =
                                explanation;


                            pnlExplanation.Visible =
                                true;
                        }
                        else
                        {
                            pnlExplanation.Visible =
                                false;
                        }


                        // =========================================
                        // IMAGE
                        // =========================================

                        string mediaPath =
                            reader[
                                "MediaPath"
                            ] == DBNull.Value

                            ? ""

                            : Convert.ToString(
                                reader[
                                    "MediaPath"
                                ]
                            );


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


                            pnlMedia.Visible =
                                true;
                        }
                        else
                        {
                            pnlMedia.Visible =
                                false;
                        }


                        pnlQuestion.Visible =
                            true;


                        return true;
                    }
                }
            }
        }


        // =========================================================
        // LOAD MCQ OPTIONS
        // =========================================================

        private void LoadOptions()
        {
            if (!pnlMcq.Visible)
            {
                return;
            }


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
                        QuestionId;


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


            rptOptions.DataSource =
                table;


            rptOptions.DataBind();
        }


        // =========================================================
        // LOAD PUBLISHED QUIZZES
        // =========================================================

        private void LoadPublishedQuizzes()
        {
            const string sql = @"
                SELECT DISTINCT

                    z.QuizId,

                    z.Title,

                    f.FormName


                FROM Quizzes z


                INNER JOIN QuizQuestions qq
                    ON z.QuizId =
                       qq.QuizId


                INNER JOIN Forms f
                    ON z.FormId =
                       f.FormId


                WHERE
                    qq.QuestionId =
                        @QuestionId

                    AND
                    z.IsPublished = 1

                    AND
                    z.IsArchived = 0


                ORDER BY
                    z.Title;
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
                        QuestionId;


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


            rptPublishedQuizzes.DataSource =
                table;


            rptPublishedQuizzes.DataBind();


            pnlNoPublishedQuiz.Visible =
                table.Rows.Count == 0;
        }


        // =========================================================
        // QUESTION TYPE DISPLAY
        // =========================================================

        private string GetQuestionTypeDisplay(
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
            pnlQuestion.Visible =
                false;


            lblError.Text =
                message;


            pnlError.Visible =
                true;
        }
    }
}