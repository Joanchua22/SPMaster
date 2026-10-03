using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace SPMaster.Admin
{
    public partial class AddUser : System.Web.UI.Page
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMessage.Visible = false;
            }
        }


        protected void btnAddUser_Click(
            object sender,
            EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }


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
                txtSchool.Text.Trim();

            string password =
                txtPassword.Text;


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


                    // Check duplicate email
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


                        int count =
                            Convert.ToInt32(
                                checkCommand.ExecuteScalar()
                            );


                        if (count > 0)
                        {
                            ShowError(
                                "A user with this email already exists."
                            );

                            return;
                        }
                    }


                    string hashedPassword =
                        HashPassword(password);


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
                                "~/Admin/UserManagement.aspx?added=true"
                            );
                        }
                        else
                        {
                            ShowError(
                                "The user could not be created."
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError(
                        "Unable to create user: "
                        + ex.Message
                    );
                }
            }
        }


        private string HashPassword(
            string password)
        {
            const int iterations = 100000;

            byte[] salt =
                new byte[16];


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