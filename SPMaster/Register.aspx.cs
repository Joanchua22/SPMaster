using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace SPMaster
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMessage.Visible = false;
            }
        }


        protected void btnRegister_Click(object sender, EventArgs e)
        {
            // 1. Check validators
            if (!Page.IsValid)
            {
                return;
            }

            // 2. Terms must be accepted
            if (!chkTerms.Checked)
            {
                ShowError(
                    "Please agree to the Terms & Privacy Policy."
                );

                return;
            }


            // 3. Get form values
            string fullName =
                txtFullName.Text.Trim();

            string email =
                txtEmail.Text.Trim().ToLower();

            string gender =
                ddlGender.SelectedValue;

            string password =
                txtPassword.Text;

            string schoolName =
                ddlSchool.SelectedValue;

            string selectedRole =
                hfRole.Value;


            // 4. Convert selected role to RoleId
            int roleId;

            if (selectedRole == "Student")
            {
                roleId = 1;
            }
            else if (selectedRole == "Lecturer")
            {
                roleId = 2;
            }
            else
            {
                ShowError("Invalid role selected.");
                return;
            }


            // 5. Connection string
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


                    // 6. Check duplicate email
                    string checkQuery = @"
                        SELECT COUNT(*)
                        FROM Users
                        WHERE Email = @Email
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


                        int existingUser =
                            Convert.ToInt32(
                                checkCommand.ExecuteScalar()
                            );


                        if (existingUser > 0)
                        {
                            ShowError(
                                "An account with this email already exists."
                            );

                            return;
                        }
                    }


                    // 7. Hash password
                    string hashedPassword =
                        HashPassword(password);


                    // 8. Insert user
                    string insertQuery = @"
                        INSERT INTO Users
                        (
                            RoleId,
                            FullName,
                            Email,
                            Gender,
                            Password,
                            SchoolName,
                            ProfileImagePath
                        )
                        VALUES
                        (
                            @RoleId,
                            @FullName,
                            @Email,
                            @Gender,
                            @Password,
                            @SchoolName,
                            @ProfileImagePath
                        )
                    ";


                    using (SqlCommand insertCommand =
                           new SqlCommand(
                               insertQuery,
                               connection))
                    {
                        insertCommand.Parameters
                            .AddWithValue(
                                "@RoleId",
                                roleId
                            );

                        insertCommand.Parameters
                            .AddWithValue(
                                "@FullName",
                                fullName
                            );

                        insertCommand.Parameters
                            .AddWithValue(
                                "@Email",
                                email
                            );

                        insertCommand.Parameters
                            .AddWithValue(
                                "@Gender",
                                gender
                            );

                        insertCommand.Parameters
                            .AddWithValue(
                                "@Password",
                                hashedPassword
                            );

                        insertCommand.Parameters
                            .AddWithValue(
                                "@SchoolName",
                                schoolName
                            );

                        insertCommand.Parameters
                            .AddWithValue(
                                "@ProfileImagePath",
                                DBNull.Value
                            );


                        int result =
                            insertCommand.ExecuteNonQuery();


                        if (result > 0)
                        {
                            Response.Redirect(
                                "~/Login.aspx?registered=true"
                            );
                        }
                        else
                        {
                            ShowError(
                                "Account could not be created."
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError(
                        "Unable to create account: "
                        + ex.Message
                    );
                }
            }
        }


        private string HashPassword(string password)
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
                hash = pbkdf2.GetBytes(32);
            }


            return
                iterations
                + "."
                + Convert.ToBase64String(salt)
                + "."
                + Convert.ToBase64String(hash);
        }


        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;
        }
    }
}