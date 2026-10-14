using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace SPMaster.Admin
{
    public partial class EditUser : System.Web.UI.Page
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUser();
            }
        }


        private int GetUserId()
        {
            int userId;

            if (!int.TryParse(
                Request.QueryString["id"],
                out userId))
            {
                return 0;
            }

            return userId;
        }


        private void LoadUser()
        {
            int userId = GetUserId();

            if (userId == 0)
            {
                Response.Redirect(
                    "~/Admin/UserManagement.aspx"
                );

                return;
            }


            string connectionString =
                ConfigurationManager
                .ConnectionStrings[
                    "SPMasterConnectionString"
                ]
                .ConnectionString;


            string query = @"
                SELECT
                    UserId,
                    RoleId,
                    FullName,
                    Email,
                    Gender,
                    SchoolName
                FROM Users
                WHERE UserId = @UserId
            ";


            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand command =
                       new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId
                    );


                    connection.Open();


                    using (SqlDataReader reader =
                           command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtFullName.Text =
                                reader["FullName"].ToString();

                            txtEmail.Text =
                                reader["Email"].ToString();

                            ddlRole.SelectedValue =
                                reader["RoleId"].ToString();

                            ddlGender.SelectedValue =
                                reader["Gender"].ToString();

                            string school =
                                reader["SchoolName"].ToString();


                            if (
                                ddlSchool.Items.FindByValue(school)
                                != null
                            )
                            {
                                ddlSchool.SelectedValue =
                                    school;
                            }
                        }
                        else
                        {
                            Response.Redirect(
                                "~/Admin/UserManagement.aspx"
                            );
                        }
                    }
                }
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
            {
                // Run ASP.NET validators first
                if (!Page.IsValid)
                {
                    return;
                }


                // Get selected UserId from URL
                int userId = GetUserId();

                if (userId == 0)
                {
                    ShowError("Invalid user.");

                    return;
                }


                // Get form values
                string fullName =
                    txtFullName.Text.Trim();

                string email =
                    txtEmail.Text.Trim().ToLower();

                int roleId =
                    Convert.ToInt32(
                        ddlRole.SelectedValue
                    );

                string gender =
                    ddlGender.SelectedValue;

                string schoolName =
                    ddlSchool.SelectedValue;

                string newPassword =
                    txtPassword.Text;

                string confirmPassword =
                    txtConfirmPassword.Text;


                // =====================================
                // PASSWORD VALIDATION
                // =====================================

                // If either password field is filled,
                // both fields must be completed
                if (!string.IsNullOrWhiteSpace(newPassword) ||
                    !string.IsNullOrWhiteSpace(confirmPassword))
                {
                    // New password entered but confirm password blank
                    if (string.IsNullOrWhiteSpace(confirmPassword))
                    {
                        ShowError(
                            "Please confirm the new password."
                        );

                        return;
                    }


                    // Confirm password entered but new password blank
                    if (string.IsNullOrWhiteSpace(newPassword))
                    {
                        ShowError(
                            "Please enter the new password."
                        );

                        return;
                    }


                    // Minimum length
                    if (newPassword.Length < 8)
                    {
                        ShowError(
                            "Password must be at least 8 characters."
                        );

                        return;
                    }


                    // Passwords must match
                    if (newPassword != confirmPassword)
                    {
                        ShowError(
                            "New password and confirm password do not match."
                        );

                        return;
                    }
                }


                string connectionString =
                    ConfigurationManager
                    .ConnectionStrings[
                        "SPMasterConnectionString"
                    ]
                    .ConnectionString;


                using (SqlConnection connection =
                       new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();


                        // =====================================
                        // CHECK DUPLICATE EMAIL
                        // =====================================

                        string checkQuery = @"
                    SELECT COUNT(*)
                    FROM Users
                    WHERE Email = @Email
                      AND UserId <> @UserId
                ";


                        using (SqlCommand checkCommand =
                               new SqlCommand(
                                   checkQuery,
                                   connection))
                        {
                            checkCommand.Parameters
                                .AddWithValue(
                                    "@Email",
                                    email
                                );

                            checkCommand.Parameters
                                .AddWithValue(
                                    "@UserId",
                                    userId
                                );


                            int count =
                                Convert.ToInt32(
                                    checkCommand.ExecuteScalar()
                                );


                            if (count > 0)
                            {
                                ShowError(
                                    "Another user already uses this email."
                                );

                                return;
                            }
                        }


                        // =====================================
                        // UPDATE USER
                        // =====================================

                        string updateQuery;


                        // No new password entered
                        if (string.IsNullOrWhiteSpace(newPassword))
                        {
                            updateQuery = @"
                        UPDATE Users

                        SET
                            RoleId = @RoleId,
                            FullName = @FullName,
                            Email = @Email,
                            Gender = @Gender,
                            SchoolName = @SchoolName

                        WHERE UserId = @UserId
                    ";
                        }

                        // New password entered
                        else
                        {
                            updateQuery = @"
                        UPDATE Users

                        SET
                            RoleId = @RoleId,
                            FullName = @FullName,
                            Email = @Email,
                            Gender = @Gender,
                            SchoolName = @SchoolName,
                            Password = @Password

                        WHERE UserId = @UserId
                    ";
                        }


                        using (SqlCommand updateCommand =
                               new SqlCommand(
                                   updateQuery,
                                   connection))
                        {
                            updateCommand.Parameters
                                .AddWithValue(
                                    "@RoleId",
                                    roleId
                                );

                            updateCommand.Parameters
                                .AddWithValue(
                                    "@FullName",
                                    fullName
                                );

                            updateCommand.Parameters
                                .AddWithValue(
                                    "@Email",
                                    email
                                );

                            updateCommand.Parameters
                                .AddWithValue(
                                    "@Gender",
                                    gender
                                );

                            updateCommand.Parameters
                                .AddWithValue(
                                    "@SchoolName",
                                    schoolName
                                );

                            updateCommand.Parameters
                                .AddWithValue(
                                    "@UserId",
                                    userId
                                );


                            // Only update Password
                            // if a new password was entered
                            if (!string.IsNullOrWhiteSpace(newPassword))
                            {
                                string hashedPassword =
                                    HashPassword(newPassword);


                                updateCommand.Parameters
                                    .AddWithValue(
                                        "@Password",
                                        hashedPassword
                                    );
                            }


                            int result =
                                updateCommand.ExecuteNonQuery();


                            if (result > 0)
                            {
                                Response.Redirect(
                                    "~/Admin/UserManagement.aspx?updated=true"
                                );
                            }
                            else
                            {
                                ShowError(
                                    "The user could not be updated."
                                );
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(
                            "Unable to update user: "
                            + ex.Message
                        );
                    }
                }
            }


        private string HashPassword(
            string password)
        {
            const int iterations = 100000;

            byte[] salt = new byte[16];


            using (RandomNumberGenerator rng =
                   RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }


            byte[] hash;


            using (Rfc2898DeriveBytes pbkdf2 =
                   new Rfc2898DeriveBytes(
                       password,
                       salt,
                       iterations))
            {
                hash =
                    pbkdf2.GetBytes(32);
            }


            return
                iterations
                + "."
                + Convert.ToBase64String(salt)
                + "."
                + Convert.ToBase64String(hash);
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