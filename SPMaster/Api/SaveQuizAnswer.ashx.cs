using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace SPMaster.Api
{
    public class SaveQuizAnswer : IHttpHandler, IRequiresSessionState
    {
        // =========================================================
        // DATABASE CONNECTION
        // =========================================================

        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["SPMasterConnectionString"].ConnectionString;
            }
        }


        // =========================================================
        // PROCESS REQUEST
        // =========================================================

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            try
            {
                // =================================================
                // POST ONLY
                // =================================================

                if (!context.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    WriteJson(
                        context,
                        405,
                        new
                        {
                            success = false,
                            message = "POST request required."
                        }
                    );

                    return;
                }


                // =================================================
                // LOGIN SESSION
                // =================================================

                if (context.Session == null || context.Session["UserId"] == null)
                {
                    WriteJson(
                        context,
                        401,
                        new
                        {
                            success = false,
                            message = "You are not logged in."
                        }
                    );

                    return;
                }

                int studentUserId;

                if (!int.TryParse(context.Session["UserId"].ToString(), out studentUserId))
                {
                    WriteJson(
                        context,
                        401,
                        new
                        {
                            success = false,
                            message = "Invalid user session."
                        }
                    );

                    return;
                }


                // =================================================
                // ATTEMPT ID
                // =================================================

                int attemptId;

                if (!int.TryParse(context.Request.Form["attemptId"], out attemptId))
                {
                    WriteJson(
                        context,
                        400,
                        new
                        {
                            success = false,
                            message = "A valid attemptId is required."
                        }
                    );

                    return;
                }


                // =================================================
                // QUIZ QUESTION ID
                // =================================================

                int quizQuestionId;

                if (!int.TryParse(context.Request.Form["quizQuestionId"], out quizQuestionId))
                {
                    WriteJson(
                        context,
                        400,
                        new
                        {
                            success = false,
                            message = "A valid quizQuestionId is required."
                        }
                    );

                    return;
                }


                // =================================================
                // SELECTED OPTION
                // =================================================

                int? selectedOptionId = null;
                string selectedOptionValue = context.Request.Form["selectedOptionId"];

                if (!string.IsNullOrWhiteSpace(selectedOptionValue))
                {
                    int optionId;

                    if (!int.TryParse(selectedOptionValue, out optionId))
                    {
                        WriteJson(
                            context,
                            400,
                            new
                            {
                                success = false,
                                message = "Invalid selected option."
                            }
                        );

                        return;
                    }

                    selectedOptionId = optionId;
                }


                // =================================================
                // SUBJECTIVE ANSWER
                // =================================================

                string answerText = context.Request.Form["answerText"];

                if (answerText == null)
                {
                    answerText = "";
                }

                if (answerText.Length > 5000)
                {
                    WriteJson(
                        context,
                        400,
                        new
                        {
                            success = false,
                            message = "Answer cannot exceed 5000 characters."
                        }
                    );

                    return;
                }


                // =================================================
                // SAVE
                // =================================================

                SaveAnswer(
                    context,
                    studentUserId,
                    attemptId,
                    quizQuestionId,
                    selectedOptionId,
                    answerText
                );
            }
            catch (Exception ex)
            {
                // Temporary debugging information.
                // Remove "error" later for production.

                WriteJson(
                    context,
                    500,
                    new
                    {
                        success = false,
                        message = "Unable to auto-save answer.",
                        error = ex.Message
                    }
                );
            }
        }


        // =========================================================
        // SAVE ANSWER
        // =========================================================

        private void SaveAnswer(HttpContext context, int studentUserId, int attemptId, int quizQuestionId, int? selectedOptionId, string answerText)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();


                // =================================================
                // VALIDATE ATTEMPT + QUESTION
                // =================================================

                const string validateSql = @"
                    SELECT
                        q.QuestionId,
                        q.QuestionType

                    FROM QuizAttempts qa

                    INNER JOIN QuizQuestions qq
                        ON qa.QuizId = qq.QuizId

                    INNER JOIN Questions q
                        ON qq.QuestionId = q.QuestionId

                    WHERE
                        qa.AttemptId = @AttemptId
                        AND qa.StudentUserId = @StudentUserId
                        AND qa.AttemptStatus = 'InProgress'
                        AND qa.ExpiresAt > GETDATE()
                        AND qq.QuizQuestionId = @QuizQuestionId;
                ";

                int questionId;
                string questionType;

                using (SqlCommand command = new SqlCommand(validateSql, connection))
                {
                    command.Parameters.Add("@AttemptId", SqlDbType.Int).Value = attemptId;
                    command.Parameters.Add("@StudentUserId", SqlDbType.Int).Value = studentUserId;
                    command.Parameters.Add("@QuizQuestionId", SqlDbType.Int).Value = quizQuestionId;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            WriteJson(
                                context,
                                409,
                                new
                                {
                                    success = false,
                                    expired = true,
                                    message = "The quiz attempt is no longer active."
                                }
                            );

                            return;
                        }

                        questionId = Convert.ToInt32(reader["QuestionId"]);
                        questionType = reader["QuestionType"].ToString();
                    }
                }

                bool isMcq = questionType.Equals("MCQ", StringComparison.OrdinalIgnoreCase);


                // =================================================
                // MCQ
                // =================================================

                if (isMcq)
                {
                    answerText = null;

                    if (selectedOptionId.HasValue)
                    {
                        const string optionSql = @"
                            SELECT COUNT(*)

                            FROM QuestionOptions

                            WHERE
                                OptionId = @OptionId
                                AND QuestionId = @QuestionId;
                        ";

                        using (SqlCommand optionCommand = new SqlCommand(optionSql, connection))
                        {
                            optionCommand.Parameters.Add("@OptionId", SqlDbType.Int).Value = selectedOptionId.Value;
                            optionCommand.Parameters.Add("@QuestionId", SqlDbType.Int).Value = questionId;

                            int validOption = Convert.ToInt32(optionCommand.ExecuteScalar());

                            if (validOption == 0)
                            {
                                WriteJson(
                                    context,
                                    400,
                                    new
                                    {
                                        success = false,
                                        message = "The selected option is not valid for this question."
                                    }
                                );

                                return;
                            }
                        }
                    }
                }


                // =================================================
                // SUBJECTIVE
                // =================================================

                else
                {
                    selectedOptionId = null;

                    answerText = string.IsNullOrWhiteSpace(answerText)
                        ? null
                        : answerText.Trim();
                }


                // =================================================
                // EMPTY ANSWER = CLEAR ANSWER
                // =================================================

                if (!selectedOptionId.HasValue && string.IsNullOrWhiteSpace(answerText))
                {
                    const string deleteSql = @"
                        DELETE FROM StudentAnswers

                        WHERE
                            AttemptId = @AttemptId
                            AND QuizQuestionId = @QuizQuestionId;
                    ";

                    using (SqlCommand deleteCommand = new SqlCommand(deleteSql, connection))
                    {
                        deleteCommand.Parameters.Add("@AttemptId", SqlDbType.Int).Value = attemptId;
                        deleteCommand.Parameters.Add("@QuizQuestionId", SqlDbType.Int).Value = quizQuestionId;

                        deleteCommand.ExecuteNonQuery();
                    }

                    WriteJson(
                        context,
                        200,
                        new
                        {
                            success = true,
                            cleared = true,
                            message = "Answer cleared.",
                            savedAt = DateTime.Now.ToString("HH:mm:ss")
                        }
                    );

                    return;
                }


                // =================================================
                // INSERT / UPDATE ANSWER
                // =================================================

                const string saveSql = @"
                    IF EXISTS
                    (
                        SELECT 1

                        FROM StudentAnswers

                        WHERE
                            AttemptId = @AttemptId
                            AND QuizQuestionId = @QuizQuestionId
                    )

                    BEGIN

                        UPDATE StudentAnswers

                        SET
                            SelectedOptionId = @SelectedOptionId,
                            AnswerText = @AnswerText,
                            MarksAwarded = NULL

                        WHERE
                            AttemptId = @AttemptId
                            AND QuizQuestionId = @QuizQuestionId;

                    END

                    ELSE

                    BEGIN

                        INSERT INTO StudentAnswers
                        (
                            AttemptId,
                            QuizQuestionId,
                            SelectedOptionId,
                            AnswerText,
                            MarksAwarded
                        )

                        VALUES
                        (
                            @AttemptId,
                            @QuizQuestionId,
                            @SelectedOptionId,
                            @AnswerText,
                            NULL
                        );

                    END
                ";

                using (SqlCommand saveCommand = new SqlCommand(saveSql, connection))
                {
                    saveCommand.Parameters.Add("@AttemptId", SqlDbType.Int).Value = attemptId;
                    saveCommand.Parameters.Add("@QuizQuestionId", SqlDbType.Int).Value = quizQuestionId;

                    saveCommand.Parameters.Add("@SelectedOptionId", SqlDbType.Int).Value =
                        selectedOptionId.HasValue
                            ? (object)selectedOptionId.Value
                            : DBNull.Value;

                    saveCommand.Parameters.Add("@AnswerText", SqlDbType.NVarChar, -1).Value =
                        string.IsNullOrWhiteSpace(answerText)
                            ? (object)DBNull.Value
                            : answerText;

                    saveCommand.ExecuteNonQuery();
                }


                // =================================================
                // SUCCESS
                // =================================================

                WriteJson(
                    context,
                    200,
                    new
                    {
                        success = true,
                        cleared = false,
                        message = "Answer saved.",
                        savedAt = DateTime.Now.ToString("HH:mm:ss")
                    }
                );
            }
        }


        // =========================================================
        // WRITE JSON
        // =========================================================

        private void WriteJson(HttpContext context, int statusCode, object data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            context.Response.StatusCode = statusCode;
            context.Response.TrySkipIisCustomErrors = true;
            context.Response.Write(serializer.Serialize(data));
        }


        // =========================================================
        // REUSABLE
        // =========================================================

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}