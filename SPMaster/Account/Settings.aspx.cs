using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SPMaster.Account
{
    public partial class Settings : System.Web.UI.Page
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            pnlSuccess.Visible = false;
            lblMessage.Visible = false;


            if (Session["UserId"] == null)
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


        private string GetConnectionString()
        {
            return ConfigurationManager
                .ConnectionStrings[
                    "SPMasterConnectionString"
                ]
                .ConnectionString;
        }


        private int GetUserId()
        {
            int userId;


            if (
                Session["UserId"] == null
                ||
                !int.TryParse(
                    Session["UserId"].ToString(),
                    out userId
                )
            )
            {
                return 0;
            }


            return userId;
        }


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


        private void LoadUserHeader()
        {
            int userId =
                GetUserId();


            if (userId == 0)
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
                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId
                    );


                    connection.Open();


                    using (
                        SqlDataReader reader =
                        command.ExecuteReader()
                    )
                    {
                        if (reader.Read())
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


        protected void btnUpdatePassword_Click(
            object sender,
            EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }


            int userId =
                GetUserId();


            if (userId == 0)
            {
                ShowError(
                    "Your session has expired. Please log in again."
                );

                return;
            }


            string currentPassword =
                txtCurrentPassword.Text;


            string newPassword =
                txtNewPassword.Text;


            string confirmPassword =
                txtConfirmPassword.Text;


            if (newPassword.Length < 8)
            {
                ShowError(
                    "New password must be at least 8 characters."
                );

                return;
            }


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


            string storedPassword =
                GetCurrentPassword(
                    userId
                );


            if (
                storedPassword == null
            )
            {
                ShowError(
                    "Unable to verify your account."
                );

                return;
            }


            if (
                currentPassword !=
                storedPassword
            )
            {
                ShowError(
                    "Current password is incorrect."
                );

                return;
            }


            if (
                currentPassword ==
                newPassword
            )
            {
                ShowError(
                    "New password must be different from your current password."
                );

                return;
            }


            string query = @"
                UPDATE Users

                SET
                    Password = @Password

                WHERE
                    UserId = @UserId
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
                        command.Parameters.AddWithValue(
                            "@Password",
                            newPassword
                        );

                        command.Parameters.AddWithValue(
                            "@UserId",
                            userId
                        );


                        connection.Open();


                        int rowsAffected =
                            command.ExecuteNonQuery();


                        if (rowsAffected > 0)
                        {
                            txtCurrentPassword.Text =
                                "";

                            txtNewPassword.Text =
                                "";

                            txtConfirmPassword.Text =
                                "";


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
                catch (Exception)
                {
                    ShowError(
                        "Unable to update your password. Please try again."
                    );
                }
            }
        }


        private string GetCurrentPassword(
            int userId)
        {
            string query = @"
                SELECT Password

                FROM Users

                WHERE
                    UserId = @UserId
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
                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId
                    );


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


                    return result.ToString();
                }
            }
        }


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
                    DateTime.Now.AddDays(-1);
            }


            Response.Redirect(
                "~/Login.aspx"
            );
        }


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


            if (parts.Length == 1)
            {
                return parts[0]
                    .Substring(0, 1)
                    .ToUpper();
            }


            string first =
                parts[0]
                .Substring(0, 1);


            string last =
                parts[
                    parts.Length - 1
                ]
                .Substring(0, 1);


            return (
                first + last
            ).ToUpper();
        }


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