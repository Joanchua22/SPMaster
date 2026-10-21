using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace SPMaster
{
    public partial class ForgotPassword :
        System.Web.UI.Page
    {
        // =========================================================
        // RESET SETTINGS
        // =========================================================

        private const int ResetCodeMinutes =
            10;


        private const int PasswordIterations =
            100000;



        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMessage.Visible =
                    false;


                lblResetMessage.Visible =
                    false;


                pnlEmailStep.Visible =
                    true;


                pnlResetStep.Visible =
                    false;
            }
        }



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
        // SEND RESET CODE
        // =========================================================

        protected void btnSendCode_Click(
            object sender,
            EventArgs e)
        {
            Page.Validate(
                "EmailStep"
            );


            if (!Page.IsValid)
            {
                return;
            }


            lblMessage.Visible =
                false;


            string email =
                txtEmail.Text
                    .Trim();



            // =====================================================
            // FIND USER
            // =====================================================

            int userId =
                0;


            string fullName =
                "";


            bool isActive =
                false;


            const string query = @"
                SELECT

                    UserId,
                    FullName,
                    IsActive

                FROM Users

                WHERE
                    Email =
                        @Email;
            ";


            try
            {
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
                                query,
                                connection
                            )
                    )
                    {
                        command.Parameters.Add(
                            "@Email",
                            SqlDbType.NVarChar,
                            254
                        ).Value =
                            email;


                        connection.Open();


                        using (
                            SqlDataReader reader =
                                command.ExecuteReader()
                        )
                        {
                            if (
                                reader.Read()
                            )
                            {
                                userId =
                                    Convert.ToInt32(
                                        reader[
                                            "UserId"
                                        ]
                                    );


                                fullName =
                                    Convert.ToString(
                                        reader[
                                            "FullName"
                                        ]
                                    );


                                isActive =
                                    Convert.ToBoolean(
                                        reader[
                                            "IsActive"
                                        ]
                                    );
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                ShowEmailError(
                    "Unable to process the request at the moment. Please try again."
                );

                return;
            }



            // =====================================================
            // DO NOT CONTINUE FOR INVALID / INACTIVE ACCOUNT
            // =====================================================

            if (
                userId <= 0
                ||
                !isActive
            )
            {
                // Generic message avoids exposing
                // whether an email exists in the system.

                ShowEmailError(
                    "If this email is registered and active, a reset code will be sent."
                );

                return;
            }



            // =====================================================
            // GENERATE RESET CODE
            // =====================================================

            string resetCode =
                GenerateResetCode();



            // =====================================================
            // STORE TEMPORARY RESET INFORMATION IN SESSION
            // =====================================================

            Session[
                "PasswordResetUserId"
            ] =
                userId;


            Session[
                "PasswordResetEmail"
            ] =
                email;


            Session[
                "PasswordResetCode"
            ] =
                resetCode;


            Session[
                "PasswordResetExpiry"
            ] =
                DateTime.Now.AddMinutes(
                    ResetCodeMinutes
                );



            // =====================================================
            // SEND EMAIL
            // =====================================================

            bool emailSent =
                SendResetEmail(
                    email,
                    fullName,
                    resetCode
                );


            if (!emailSent)
            {
                ClearResetSession();


                //ShowEmailError(
                //    "Unable to send the reset code. Please try again later."
                //);

                return;
            }



            // =====================================================
            // MOVE TO RESET STEP
            // =====================================================

            pnlEmailStep.Visible =
                false;


            pnlResetStep.Visible =
                true;


            lblResetMessage.Text =
                "A 6-digit verification code has been sent to your email. The code expires in 10 minutes.";


            lblResetMessage.CssClass =
                "forgot-message forgot-success-message";


            lblResetMessage.Visible =
                true;
        }



        // =========================================================
        // RESET PASSWORD
        // =========================================================

        protected void btnResetPassword_Click(
            object sender,
            EventArgs e)
        {
            Page.Validate(
                "ResetStep"
            );


            if (!Page.IsValid)
            {
                return;
            }


            lblResetMessage.Visible =
                false;



            // =====================================================
            // CHECK RESET SESSION EXISTS
            // =====================================================

            if (
                Session[
                    "PasswordResetUserId"
                ] == null
                ||
                Session[
                    "PasswordResetEmail"
                ] == null
                ||
                Session[
                    "PasswordResetCode"
                ] == null
                ||
                Session[
                    "PasswordResetExpiry"
                ] == null
            )
            {
                ShowResetError(
                    "Your password reset session has expired. Please request a new code."
                );

                return;
            }



            // =====================================================
            // CHECK CODE EXPIRY
            // =====================================================

            DateTime expiry =
                Convert.ToDateTime(
                    Session[
                        "PasswordResetExpiry"
                    ]
                );


            if (
                DateTime.Now >
                expiry
            )
            {
                ClearResetSession();


                pnlResetStep.Visible =
                    false;


                pnlEmailStep.Visible =
                    true;


                ShowEmailError(
                    "The reset code has expired. Please request a new code."
                );

                return;
            }



            // =====================================================
            // VERIFY RESET CODE
            // =====================================================

            string enteredCode =
                txtResetCode.Text
                    .Trim();


            string storedCode =
                Convert.ToString(
                    Session[
                        "PasswordResetCode"
                    ]
                );


            if (
                enteredCode
                !=
                storedCode
            )
            {
                ShowResetError(
                    "The verification code is incorrect."
                );

                return;
            }



            // =====================================================
            // PASSWORD VALIDATION
            // =====================================================

            string newPassword =
                txtNewPassword.Text;


            string confirmPassword =
                txtConfirmPassword.Text;


            if (
                newPassword.Length <
                8
            )
            {
                ShowResetError(
                    "Password must be at least 8 characters."
                );

                return;
            }


            if (
                newPassword
                !=
                confirmPassword
            )
            {
                ShowResetError(
                    "New password and confirmation do not match."
                );

                return;
            }



            // =====================================================
            // HASH NEW PASSWORD
            // =====================================================

            string hashedPassword =
                HashPassword(
                    newPassword
                );



            // =====================================================
            // UPDATE PASSWORD
            // =====================================================

            int userId =
                Convert.ToInt32(
                    Session[
                        "PasswordResetUserId"
                    ]
                );


            const string updateQuery = @"
                UPDATE Users

                SET
                    Password =
                        @Password

                WHERE
                    UserId =
                        @UserId;
            ";


            try
            {
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
                                updateQuery,
                                connection
                            )
                    )
                    {
                        command.Parameters.Add(
                            "@Password",
                            SqlDbType.NVarChar,
                            512
                        ).Value =
                            hashedPassword;


                        command.Parameters.Add(
                            "@UserId",
                            SqlDbType.Int
                        ).Value =
                            userId;


                        connection.Open();


                        int rowsAffected =
                            command.ExecuteNonQuery();


                        if (
                            rowsAffected <= 0
                        )
                        {
                            ShowResetError(
                                "Unable to reset your password."
                            );

                            return;
                        }
                    }
                }
            }
            catch (Exception)
            {
                ShowResetError(
                    "Unable to reset your password at the moment. Please try again."
                );

                return;
            }



            // =====================================================
            // SUCCESS
            // =====================================================

            ClearResetSession();


            Response.Redirect(
                "~/Login.aspx?reset=true"
            );
        }



        // =========================================================
        // BACK TO EMAIL STEP
        // =========================================================

        protected void btnBackToEmail_Click(
            object sender,
            EventArgs e)
        {
            ClearResetSession();


            txtResetCode.Text =
                "";


            txtNewPassword.Text =
                "";


            txtConfirmPassword.Text =
                "";


            pnlResetStep.Visible =
                false;


            pnlEmailStep.Visible =
                true;


            lblMessage.Visible =
                false;
        }



        // =========================================================
        // GENERATE 6-DIGIT RESET CODE
        // =========================================================

        private string GenerateResetCode()
        {
            byte[] randomBytes =
                new byte[4];


            using (
                RandomNumberGenerator rng =
                    RandomNumberGenerator
                        .Create()
            )
            {
                rng.GetBytes(
                    randomBytes
                );
            }


            uint randomValue =
                BitConverter.ToUInt32(
                    randomBytes,
                    0
                );


            int code =
                (int)(
                    randomValue
                    % 1000000
                );


            return code.ToString(
                "D6"
            );
        }



        // =========================================================
        // SEND RESET EMAIL
        // =========================================================

        private bool SendResetEmail(
            string email,
            string fullName,
            string resetCode)
        {
            try
            {
                string smtpHost =
                    ConfigurationManager.AppSettings["SmtpHost"];

                string smtpPortText =
                    ConfigurationManager.AppSettings["SmtpPort"];

                string smtpUsername =
                    ConfigurationManager.AppSettings["SmtpUsername"];

                string smtpPassword =
                    ConfigurationManager.AppSettings["SmtpPassword"];

                string smtpFromEmail =
                    ConfigurationManager.AppSettings["SmtpFromEmail"];

                string smtpFromName =
                    ConfigurationManager.AppSettings["SmtpFromName"];


                if (
                    string.IsNullOrWhiteSpace(smtpHost)
                    ||
                    string.IsNullOrWhiteSpace(smtpUsername)
                    ||
                    string.IsNullOrWhiteSpace(smtpPassword)
                    ||
                    string.IsNullOrWhiteSpace(smtpFromEmail)
                )
                {
                    ShowEmailError(
                        "SMTP configuration is incomplete."
                    );

                    return false;
                }


                int smtpPort;

                if (
                    !int.TryParse(
                        smtpPortText,
                        out smtpPort
                    )
                )
                {
                    smtpPort =
                        587;
                }


                MimeMessage message =
                    new MimeMessage();


                message.From.Add(
                    new MailboxAddress(
                        string.IsNullOrWhiteSpace(smtpFromName)
                            ? "SPMaster"
                            : smtpFromName,
                        smtpFromEmail
                    )
                );


                message.To.Add(
                    MailboxAddress.Parse(
                        email
                    )
                );


                message.Subject =
                    "SPMaster Password Reset Code";


                BodyBuilder bodyBuilder =
                    new BodyBuilder();


                bodyBuilder.HtmlBody =
                    BuildResetEmailBody(
                        fullName,
                        resetCode
                    );


                message.Body =
                    bodyBuilder.ToMessageBody();


                using (
                    SmtpClient smtp =
                        new SmtpClient()
                )
                {
                    smtp.Connect(
                        smtpHost,
                        smtpPort,
                        SecureSocketOptions.StartTls
                    );


                    smtp.Authenticate(
                        smtpUsername,
                        smtpPassword
                    );


                    smtp.Send(
                        message
                    );


                    smtp.Disconnect(
                        true
                    );
                }


                return true;
            }
            catch (Exception ex)
            {
                ShowEmailError(
                    "SMTP Error: "
                    + ex.Message
                );

                return false;
            }
        }



        // =========================================================
        // EMAIL BODY
        // =========================================================

        private string BuildResetEmailBody(
            string fullName,
            string resetCode)
        {
            string safeName =
                string.IsNullOrWhiteSpace(
                    fullName
                )
                ? "User"
                : fullName;


            return
                "<div style=\"font-family:Arial,sans-serif;"
                + "max-width:520px;margin:auto;"
                + "padding:24px;"
                + "color:#2d2365;\">"

                + "<h2>SPMaster Password Reset</h2>"

                + "<p>Hello "
                + Server.HtmlEncode(
                    safeName
                )
                + ",</p>"

                + "<p>We received a request to reset your SPMaster password.</p>"

                + "<p>Your verification code is:</p>"

                + "<div style=\""
                + "font-size:30px;"
                + "font-weight:bold;"
                + "letter-spacing:8px;"
                + "padding:16px 0;"
                + "color:#8c3fff;\">"

                + resetCode

                + "</div>"

                + "<p>This code expires in "
                + ResetCodeMinutes
                + " minutes.</p>"

                + "<p>If you did not request a password reset, you can ignore this email.</p>"

                + "<p style=\"margin-top:28px;color:#777382;\">"
                + "SPMaster"
                + "</p>"

                + "</div>";
        }



        // =========================================================
        // HASH PASSWORD
        //
        // Same format used by Register/Login:
        //
        // iterations.salt.hash
        // =========================================================

        private string HashPassword(
            string password)
        {
            byte[] salt =
                new byte[16];


            using (
                RandomNumberGenerator rng =
                    RandomNumberGenerator
                        .Create()
            )
            {
                rng.GetBytes(
                    salt
                );
            }


            byte[] hash;


            using (
                Rfc2898DeriveBytes pbkdf2 =
                    new Rfc2898DeriveBytes(
                        password,
                        salt,
                        PasswordIterations
                    )
            )
            {
                hash =
                    pbkdf2.GetBytes(
                        32
                    );
            }


            return
                PasswordIterations
                    .ToString()
                + "."
                + Convert.ToBase64String(
                    salt
                )
                + "."
                + Convert.ToBase64String(
                    hash
                );
        }



        // =========================================================
        // CLEAR RESET SESSION
        // =========================================================

        private void ClearResetSession()
        {
            Session.Remove(
                "PasswordResetUserId"
            );


            Session.Remove(
                "PasswordResetEmail"
            );


            Session.Remove(
                "PasswordResetCode"
            );


            Session.Remove(
                "PasswordResetExpiry"
            );
        }



        // =========================================================
        // EMAIL STEP ERROR
        // =========================================================

        private void ShowEmailError(
            string message)
        {
            lblMessage.Text =
                message;


            lblMessage.CssClass =
                "forgot-message";


            lblMessage.Visible =
                true;
        }



        // =========================================================
        // RESET STEP ERROR
        // =========================================================

        private void ShowResetError(
            string message)
        {
            lblResetMessage.Text =
                message;


            lblResetMessage.CssClass =
                "forgot-message";


            lblResetMessage.Visible =
                true;
        }
    }
}