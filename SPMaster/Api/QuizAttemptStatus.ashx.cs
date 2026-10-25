using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace SPMaster.Api
{
    public class QuizAttemptStatus :
        IHttpHandler,
        IRequiresSessionState
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
        // PROCESS REQUEST
        // =========================================================

        public void ProcessRequest(
            HttpContext context)
        {
            context.Response.Clear();

            context.Response.ContentType =
                "application/json";

            context.Response.Cache.SetCacheability(
                HttpCacheability.NoCache
            );

            context.Response.Cache.SetNoStore();


            try
            {
                // =================================================
                // SESSION CHECK
                // =================================================

                if (
                    context.Session == null ||
                    context.Session["UserId"] == null
                )
                {
                    WriteJson(
                        context,
                        new
                        {
                            success = false,
                            message =
                                "You are not logged in."
                        }
                    );

                    return;
                }


                int studentUserId;


                if (
                    !int.TryParse(
                        context.Session[
                            "UserId"
                        ].ToString(),
                        out studentUserId
                    )
                )
                {
                    WriteJson(
                        context,
                        new
                        {
                            success = false,
                            message =
                                "Invalid UserId in session."
                        }
                    );

                    return;
                }


                // =================================================
                // ATTEMPT ID
                // =================================================

                int attemptId;


                if (
                    !int.TryParse(
                        context.Request.QueryString[
                            "attemptId"
                        ],
                        out attemptId
                    )
                )
                {
                    WriteJson(
                        context,
                        new
                        {
                            success = false,
                            message =
                                "A valid attemptId is required."
                        }
                    );

                    return;
                }


                // =================================================
                // LOAD ATTEMPT
                // =================================================

                LoadAttempt(
                    context,
                    attemptId,
                    studentUserId
                );
            }
            catch (Exception ex)
            {
                // =================================================
                // TEMPORARY DEBUG RESPONSE
                // Remove detailed exception later
                // =================================================

                WriteJson(
                    context,
                    new
                    {
                        success = false,

                        message =
                            "API error occurred.",

                        error =
                            ex.Message,

                        exceptionType =
                            ex.GetType().Name
                    }
                );
            }
        }


        // =========================================================
        // LOAD QUIZ ATTEMPT
        // =========================================================

        private void LoadAttempt(
            HttpContext context,
            int attemptId,
            int studentUserId)
        {
            const string sql = @"
                SELECT
                    AttemptId,
                    QuizId,
                    StudentUserId,
                    StartedAt,
                    ExpiresAt,
                    SubmittedAt,
                    AttemptStatus

                FROM QuizAttempts

                WHERE
                    AttemptId = @AttemptId

                    AND

                    StudentUserId =
                        @StudentUserId;
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
                    "@StudentUserId",
                    SqlDbType.Int
                ).Value =
                    studentUserId;


                connection.Open();


                using (
                    SqlDataReader reader =
                        command.ExecuteReader()
                )
                {
                    // =============================================
                    // NOT FOUND / NOT OWNED BY THIS STUDENT
                    // =============================================

                    if (!reader.Read())
                    {
                        WriteJson(
                            context,
                            new
                            {
                                success = false,

                                message =
                                    "Attempt was not found for the logged-in student.",

                                attemptId =
                                    attemptId,

                                studentUserId =
                                    studentUserId
                            }
                        );

                        return;
                    }


                    // =============================================
                    // BASIC VALUES
                    // =============================================

                    int quizId =
                        Convert.ToInt32(
                            reader["QuizId"]
                        );


                    string attemptStatus =
                        Convert.ToString(
                            reader["AttemptStatus"]
                        );


                    DateTime startedAt =
                        Convert.ToDateTime(
                            reader["StartedAt"]
                        );


                    // =============================================
                    // EXPIRES AT
                    // =============================================

                    DateTime? expiresAt =
                        null;


                    if (
                        reader["ExpiresAt"] !=
                        DBNull.Value
                    )
                    {
                        expiresAt =
                            Convert.ToDateTime(
                                reader["ExpiresAt"]
                            );
                    }


                    // =============================================
                    // SUBMITTED AT
                    // =============================================

                    DateTime? submittedAt =
                        null;


                    if (
                        reader["SubmittedAt"] !=
                        DBNull.Value
                    )
                    {
                        submittedAt =
                            Convert.ToDateTime(
                                reader["SubmittedAt"]
                            );
                    }


                    // =============================================
                    // SERVER TIME
                    // =============================================

                    DateTime serverTime =
                        DateTime.Now;


                    // =============================================
                    // REMAINING TIME
                    // =============================================

                    int remainingSeconds =
                        0;


                    bool expired =
                        false;


                    if (
                        expiresAt.HasValue &&
                        !submittedAt.HasValue
                    )
                    {
                        remainingSeconds =
                            (int)Math.Max(
                                0,
                                (
                                    expiresAt.Value -
                                    serverTime
                                ).TotalSeconds
                            );


                        expired =
                            serverTime >=
                            expiresAt.Value;
                    }


                    // =============================================
                    // RESPONSE
                    // =============================================

                    WriteJson(
                        context,
                        new
                        {
                            success = true,

                            attemptId =
                                attemptId,

                            quizId =
                                quizId,

                            studentUserId =
                                studentUserId,

                            status =
                                attemptStatus,

                            startedAt =
                                startedAt.ToString(
                                    "yyyy-MM-dd HH:mm:ss"
                                ),

                            expiresAt =
                                expiresAt.HasValue
                                    ? expiresAt.Value
                                        .ToString(
                                            "yyyy-MM-dd HH:mm:ss"
                                        )
                                    : null,

                            submittedAt =
                                submittedAt.HasValue
                                    ? submittedAt.Value
                                        .ToString(
                                            "yyyy-MM-dd HH:mm:ss"
                                        )
                                    : null,

                            serverTime =
                                serverTime.ToString(
                                    "yyyy-MM-dd HH:mm:ss"
                                ),

                            remainingSeconds =
                                remainingSeconds,

                            expired =
                                expired
                        }
                    );
                }
            }
        }


        // =========================================================
        // WRITE JSON
        // =========================================================

        private void WriteJson(
            HttpContext context,
            object data)
        {
            JavaScriptSerializer serializer =
                new JavaScriptSerializer();


            context.Response.Write(
                serializer.Serialize(
                    data
                )
            );
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