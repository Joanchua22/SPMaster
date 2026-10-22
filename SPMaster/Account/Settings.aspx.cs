using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;


namespace SPMaster.Account
{
    public partial class Settings :
        System.Web.UI.Page
    {
        // =========================================================
        // SELECT MASTER PAGE BY ROLE
        // =========================================================

        protected void Page_PreInit(
            object sender,
            EventArgs e)
        {
            if (
                Session["Role"] == null
            )
            {
                Response.Redirect(
                    "~/Login.aspx"
                );

                return;
            }


            string role =
                Session["Role"]
                    .ToString()
                    .Trim();


            // Admin uses Dashboard.Master
            if (
                role.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                MasterPageFile =
                    "~/Master/Dashboard.Master";
            }

            // Student / Lecturer use Portal.Master
            else
            {
                MasterPageFile =
                    "~/Master/Portal.Master";
            }
        }



        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            pnlSuccess.Visible =
                false;


            lblMessage.Visible =
                false;


            lblCurrentPasswordError.Visible =
                false;


            if (
                Session["UserId"] == null
            )
            {
                Response.Redirect(
                    "~/Login.aspx"
                );

                return;
            }


            SetNavigation();


            if (!IsPostBack)
            {
                LoadUserHeader();
            }
        }



        // =========================================================
        // DATABASE CONNECTION
        // =========================================================

        private string GetConnectionString()
        {
            return ConfigurationManager
                .ConnectionStrings[
                    "SPMasterConnectionString"
                ]
                .ConnectionString;
        }



        // =========================================================
        // CURRENT USER ID
        // =========================================================

        private int GetUserId()
        {
            int userId;


            if (
                Session["UserId"] == null
                ||
                !int.TryParse(
                    Session["UserId"]
                        .ToString(),
                    out userId
                )
            )
            {
                return 0;
            }


            return userId;
        }



        // =========================================================
        // ROLE NAVIGATION
        // =========================================================

        private void SetNavigation()
        {
            pnlAdminNavigation.Visible =
                false;


            pnlLecturerNavigation.Visible =
                false;


            pnlStudentNavigation.Visible =
                false;


            string role =
                Session["Role"] == null
                ? ""
                : Session["Role"]
                    .ToString()
                    .Trim();


            if (
                role.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                pnlAdminNavigation.Visible =
                    true;
            }

            else if (
                role.Equals(
                    "Lecturer",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                pnlLecturerNavigation.Visible =
                    true;
            }

            else
            {
                pnlStudentNavigation.Visible =
                    true;
            }
        }



        // =========================================================
        // LOAD USER HEADER
        // =========================================================

        private void LoadUserHeader()
        {
            int userId =
                GetUserId();


            if (
                userId == 0
            )
            {
                return;
            }


            string query = @"
                SELECT
                    U.FullName,
                    R.RoleName

                FROM Users U

                INNER JOIN Roles R
                    ON U.RoleId =
                       R.RoleId

                WHERE
                    U.UserId =
                    @UserId
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        GetConnectionString()
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
                    command.Parameters
                        .AddWithValue(
                            "@UserId",
                            userId
                        );


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
                            string fullName =
                                reader["FullName"]
                                    .ToString();


                            string role =
                                reader["RoleName"]
                                    .ToString();


                            lblTopName.Text =
                                fullName;


                            lblTopRole.Text =
                                role;


                            lblTopInitials.Text =
                                GetInitials(
                                    fullName
                                );
                        }
                    }
                }
            }
        }



        // =========================================================
        // UPDATE PASSWORD
        // =========================================================

        protected void btnUpdatePassword_Click(
            object sender,
            EventArgs e)
        {
            if (
                !Page.IsValid
            )
            {
                return;
            }


            int userId =
                GetUserId();


            if (
                userId == 0
            )
            {
                ShowError(
                    "Your session has expired. Please log in again."
                );

                return;
            }



            // =====================================================
            // GET FORM VALUES
            // =====================================================

            string currentPassword =
                txtCurrentPassword.Text;


            string newPassword =
                txtNewPassword.Text;


            string confirmPassword =
                txtConfirmPassword.Text;



            // =====================================================
            // NEW PASSWORD LENGTH
            // =====================================================

            if (
                newPassword.Length < 8
            )
            {
                ShowError(
                    "New password must be at least 8 characters."
                );

                return;
            }



            // =====================================================
            // CONFIRM PASSWORD
            // =====================================================

            if (
                newPassword !=
                confirmPassword
            )
            {
                ShowError(
                    "New password and confirmation do not match."
                );

                return;
            }



            // =====================================================
            // GET STORED HASH
            // =====================================================

            string storedPassword =
                GetCurrentPassword(
                    userId
                );


            if (
                string.IsNullOrWhiteSpace(
                    storedPassword
                )
            )
            {
                ShowError(
                    "Unable to verify your account."
                );

                return;
            }



            // =====================================================
            // VERIFY CURRENT PASSWORD
            // =====================================================

            bool currentPasswordCorrect =
                VerifyPassword(
                    currentPassword,
                    storedPassword
                );


            if (
                !currentPasswordCorrect
            )
            {
                lblCurrentPasswordError.Text =
                    "Current password is incorrect.";


                lblCurrentPasswordError.Visible =
                    true;


                return;
            }



            // =====================================================
            // NEW PASSWORD CANNOT BE SAME AS CURRENT PASSWORD
            // =====================================================

            bool sameAsCurrentPassword =
                VerifyPassword(
                    newPassword,
                    storedPassword
                );


            if (
                sameAsCurrentPassword
            )
            {
                ShowError(
                    "New password must be different from your current password."
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
            // UPDATE DATABASE
            // =====================================================

            string query = @"
                UPDATE Users

                SET
                    Password =
                        @Password

                WHERE
                    UserId =
                        @UserId
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        GetConnectionString()
                    )
            )
            {
                try
                {
                    using (
                        SqlCommand command =
                            new SqlCommand(
                                query,
                                connection
                            )
                    )
                    {
                        command.Parameters
                            .AddWithValue(
                                "@Password",
                                hashedPassword
                            );


                        command.Parameters
                            .AddWithValue(
                                "@UserId",
                                userId
                            );


                        connection.Open();


                        int rowsAffected =
                            command
                                .ExecuteNonQuery();


                        if (
                            rowsAffected > 0
                        )
                        {
                            txtCurrentPassword.Text =
                                "";


                            txtNewPassword.Text =
                                "";


                            txtConfirmPassword.Text =
                                "";


                            lblCurrentPasswordError.Visible =
                                false;


                            lblSuccessMessage.Text =
                                "Password updated successfully.";


                            pnlSuccess.Visible =
                                true;
                        }
                        else
                        {
                            ShowError(
                                "Unable to update your password."
                            );
                        }
                    }
                }
                catch (
                    Exception
                )
                {
                    ShowError(
                        "Unable to update your password. Please try again."
                    );
                }
            }
        }



        // =========================================================
        // GET CURRENT STORED PASSWORD HASH
        // =========================================================

        private string GetCurrentPassword(
            int userId)
        {
            string query = @"
                SELECT
                    Password

                FROM Users

                WHERE
                    UserId =
                    @UserId
            ";


            using (
                SqlConnection connection =
                    new SqlConnection(
                        GetConnectionString()
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
                    command.Parameters
                        .AddWithValue(
                            "@UserId",
                            userId
                        );


                    connection.Open();


                    object result =
                        command
                            .ExecuteScalar();


                    if (
                        result == null
                        ||
                        result == DBNull.Value
                    )
                    {
                        return null;
                    }


                    return
                        result.ToString();
                }
            }
        }



        // =========================================================
        // HASH PASSWORD
        // =========================================================

        private string HashPassword(
            string password)
        {
            const int iterations =
                100000;


            byte[] salt =
                new byte[16];


            using (
                RandomNumberGenerator rng =
                    RandomNumberGenerator.Create()
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
                        iterations
                    )
            )
            {
                hash =
                    pbkdf2.GetBytes(
                        32
                    );
            }


            return
                iterations
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
        // VERIFY PASSWORD
        // =========================================================

        private bool VerifyPassword(
            string password,
            string storedPassword)
        {
            try
            {
                if (
                    string.IsNullOrWhiteSpace(
                        storedPassword
                    )
                )
                {
                    return false;
                }


                string[] parts =
                    storedPassword.Split(
                        '.'
                    );


                if (
                    parts.Length != 3
                )
                {
                    return false;
                }



                // =============================================
                // READ STORED VALUES
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


                byte[] salt =
                    Convert.FromBase64String(
                        parts[1]
                    );


                byte[] storedHash =
                    Convert.FromBase64String(
                        parts[2]
                    );



                // =============================================
                // HASH ENTERED PASSWORD WITH SAME SALT
                // =============================================

                byte[] enteredHash;


                using (
                    Rfc2898DeriveBytes pbkdf2 =
                        new Rfc2898DeriveBytes(
                            password,
                            salt,
                            iterations
                        )
                )
                {
                    enteredHash =
                        pbkdf2.GetBytes(
                            storedHash.Length
                        );
                }



                // =============================================
                // COMPARE HASHES
                // =============================================

                if (
                    enteredHash.Length
                    !=
                    storedHash.Length
                )
                {
                    return false;
                }


                int difference =
                    0;


                for (
                    int i = 0;
                    i < enteredHash.Length;
                    i++
                )
                {
                    difference |=
                        enteredHash[i]
                        ^
                        storedHash[i];
                }


                return
                    difference == 0;
            }
            catch
            {
                return false;
            }
        }



        // =========================================================
        // LOG OUT
        // =========================================================

        protected void btnLogout_Click(
            object sender,
            EventArgs e)
        {
            Session.Clear();


            Session.Abandon();


            if (
                Request.Cookies[
                    "ASP.NET_SessionId"
                ] != null
            )
            {
                Response.Cookies[
                    "ASP.NET_SessionId"
                ].Expires =
                    DateTime.Now
                        .AddDays(-1);
            }


            Response.Redirect(
                "~/Login.aspx"
            );
        }



        // =========================================================
        // GET INITIALS
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
                return "U";
            }


            string[] parts =
                fullName
                    .Trim()
                    .Split(
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


            string first =
                parts[0]
                    .Substring(
                        0,
                        1
                    );


            string last =
                parts[
                    parts.Length - 1
                ]
                .Substring(
                    0,
                    1
                );


            return (
                first
                +
                last
            )
            .ToUpper();
        }



        // =========================================================
        // GENERAL ERROR MESSAGE
        // =========================================================

        private void ShowError(
            string message)
        {
            lblMessage.Text =
                message;


            lblMessage.Visible =
                true;
        }
    }
}