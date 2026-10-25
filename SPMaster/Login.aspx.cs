using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;


namespace SPMaster
{
    public partial class Login :
        System.Web.UI.Page
    {
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


                // =============================================
                // REGISTRATION SUCCESS MESSAGE
                // =============================================

                if (
                    Request.QueryString[
                        "registered"
                    ]
                    == "true"
                )
                {
                    lblMessage.Text =
                        "Account created successfully. You can now log in.";


                    lblMessage.CssClass =
                        "success-message";


                    lblMessage.Visible =
                        true;
                }


                // =============================================
                // REMEMBERED EMAIL
                // =============================================

                if (
                    Request.Cookies[
                        "RememberedEmail"
                    ]
                    != null
                )
                {
                    txtEmail.Text =
                        Request.Cookies[
                            "RememberedEmail"
                        ].Value;
                }
            }
        }



        // =========================================================
        // LOGIN
        // =========================================================

        protected void btnLogin_Click(
           object sender,
           EventArgs e)
        {
            // =============================================
            // ASP.NET VALIDATION
            // =============================================

            if (!Page.IsValid)
            {
                return;
            }


            string email =
                txtEmail.Text.Trim();


            string password =
                txtPassword.Text;


            string connectionString =
                ConfigurationManager
                    .ConnectionStrings[
                        "SPMasterConnectionString"
                    ]
                    .ConnectionString;


            const string query = @"
        SELECT
            U.UserId,
            U.FullName,
            U.Email,
            U.Password,
            U.RoleId,
            U.IsActive,
            R.RoleName

        FROM Users U

        INNER JOIN Roles R
            ON U.RoleId =
               R.RoleId

        WHERE
            U.Email =
                @Email;
    ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        connectionString
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
                    // =============================================
                    // EMAIL PARAMETER
                    // =============================================

                    command.Parameters.Add(
                        "@Email",
                        SqlDbType.NVarChar,
                        255
                    ).Value =
                        email;


                    try
                    {
                        connection.Open();


                        using (
                            SqlDataReader reader =
                                command.ExecuteReader()
                        )
                        {
                            // =====================================
                            // ACCOUNT NOT FOUND
                            // =====================================

                            if (!reader.Read())
                            {
                                ShowError(
                                    "Incorrect email or password."
                                );

                                return;
                            }


                            // =====================================
                            // ACCOUNT STATUS
                            // =====================================

                            bool isActive =
                                Convert.ToBoolean(
                                    reader[
                                        "IsActive"
                                    ]
                                );


                            if (!isActive)
                            {
                                ShowError(
                                    "This account is currently inactive."
                                );

                                return;
                            }


                            // =====================================
                            // VERIFY HASHED PASSWORD
                            // =====================================

                            string storedPassword =
                                Convert.ToString(
                                    reader[
                                        "Password"
                                    ]
                                );


                            if (
                                !VerifyPassword(
                                    password,
                                    storedPassword
                                )
                            )
                            {
                                ShowError(
                                    "Incorrect email or password."
                                );

                                return;
                            }


                            // =====================================
                            // LOGIN SUCCESS
                            // =====================================

                            Session["UserId"] =
                                reader[
                                    "UserId"
                                ].ToString();


                            Session["FullName"] =
                                reader[
                                    "FullName"
                                ].ToString();


                            Session["Email"] =
                                reader[
                                    "Email"
                                ].ToString();


                            Session["RoleId"] =
                                reader[
                                    "RoleId"
                                ].ToString();


                            Session["Role"] =
                                reader[
                                    "RoleName"
                                ].ToString();


                            string role =
                                reader[
                                    "RoleName"
                                ]
                                .ToString()
                                .Trim();

                            // =====================================
                            // REDIRECT USER
                            // =====================================

                            RedirectUser(
                                role
                            );
                        }
                    }
                    catch (Exception)
                    {
                        ShowError(
                            "Unable to log in at the moment. Please try again."
                        );
                    }
                }
            }
        }



        // =========================================================
        // VERIFY PASSWORD
        //
        // Stored format:
        //
        // iterations.salt.hash
        //
        // Example:
        //
        // 100000.xxxxx.xxxxx
        //
        // =========================================================

        private bool VerifyPassword(
            string password,
            string storedPassword)
        {
            if (
                string.IsNullOrWhiteSpace(password)
                ||
                string.IsNullOrWhiteSpace(storedPassword)
            )
            {
                return false;
            }


            // =============================================
            // STORED FORMAT:
            // iterations.salt.hash
            // =============================================

            string[] parts =
                storedPassword.Split('.');


            if (
                parts.Length != 3
            )
            {
                return false;
            }


            // =============================================
            // ITERATION COUNT
            // =============================================

            int iterations;


            if (
                !int.TryParse(
                    parts[0],
                    out iterations
                )
            )
            {
                return false;
            }


            // =============================================
            // SALT + HASH
            // =============================================

            byte[] salt;

            byte[] storedHash;


            try
            {
                salt =
                    Convert.FromBase64String(
                        parts[1]
                    );


                storedHash =
                    Convert.FromBase64String(
                        parts[2]
                    );
            }
            catch (
                FormatException
            )
            {
                return false;
            }


            // =============================================
            // GENERATE HASH USING SAME METHOD AS REGISTER
            // =============================================

            byte[] calculatedHash;


            using (
                Rfc2898DeriveBytes pbkdf2 =
                    new Rfc2898DeriveBytes(
                        password,
                        salt,
                        iterations
                    )
            )
            {
                calculatedHash =
                    pbkdf2.GetBytes(
                        32
                    );
            }


            // =============================================
            // COMPARE HASHES
            // =============================================

            return SlowEquals(
                storedHash,
                calculatedHash
            );
        }



        // =========================================================
        // SAFE HASH COMPARISON
        // =========================================================

        private bool SlowEquals(
            byte[] first,
            byte[] second)
        {
            if (
                first == null
                ||
                second == null
                ||
                first.Length != second.Length
            )
            {
                return false;
            }


            int difference =
                0;


            for (
                int i = 0;
                i < first.Length;
                i++
            )
            {
                difference |=
                    first[i]
                    ^
                    second[i];
            }


            return difference == 0;
        }    

        // =========================================================
        // REDIRECT USER BY ROLE
        // =========================================================

        private void RedirectUser(
            string role)
        {
            switch (
                role.ToLower()
            )
            {
                // =============================================
                // STUDENT
                // =============================================

                case "student":

                    Response.Redirect(
                        "~/Student/Portal.aspx"
                    );

                    break;



                // =============================================
                // LECTURER
                // =============================================

                case "lecturer":

                    Response.Redirect(
                        "~/Lecturer/Portal.aspx"
                    );

                    break;



                // =============================================
                // ADMIN
                // =============================================

                case "admin":

                    Response.Redirect(
                        "~/Admin/Dashboard.aspx"
                    );

                    break;



                // =============================================
                // UNKNOWN ROLE
                // =============================================

                default:

                    ShowError(
                        "Your account role is not recognised."
                    );

                    break;
            }
        }



        // =========================================================
        // ERROR MESSAGE
        // =========================================================

        private void ShowError(
            string message)
        {
            lblMessage.Text =
                message;


            lblMessage.CssClass =
                "error-message";


            lblMessage.Visible =
                true;
        }
    }
}