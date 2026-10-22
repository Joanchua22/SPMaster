using System;
using System.Configuration;
using System.Data.SqlClient;


namespace SPMaster.Admin
{
    public partial class EditUser :
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
                LoadUser();
            }
        }



        // =========================================================
        // GET USER ID FROM URL
        // =========================================================

        private int GetUserId()
        {
            int userId;


            if (
                !int.TryParse(
                    Request.QueryString["id"],
                    out userId
                )
            )
            {
                return 0;
            }


            return userId;
        }



        // =========================================================
        // LOAD USER
        // =========================================================

        private void LoadUser()
        {
            int userId =
                GetUserId();


            if (
                userId == 0
            )
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

                WHERE
                    UserId = @UserId
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
                            txtFullName.Text =
                                reader["FullName"]
                                    .ToString();


                            txtEmail.Text =
                                reader["Email"]
                                    .ToString();


                            ddlRole.SelectedValue =
                                reader["RoleId"]
                                    .ToString();


                            ddlGender.SelectedValue =
                                reader["Gender"]
                                    .ToString();


                            string school =
                                reader["SchoolName"]
                                    .ToString();


                            if (
                                ddlSchool.Items
                                    .FindByValue(
                                        school
                                    )
                                != null
                            )
                            {
                                ddlSchool.SelectedValue =
                                    school;
                            }
                            else
                            {
                                ddlSchool.SelectedValue =
                                    "";
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



        // =========================================================
        // SAVE USER CHANGES
        // =========================================================

        protected void btnSave_Click(
            object sender,
            EventArgs e)
        {
            // Run ASP.NET validators first
            if (
                !Page.IsValid
            )
            {
                return;
            }



            // =====================================================
            // GET USER ID
            // =====================================================

            int userId =
                GetUserId();


            if (
                userId == 0
            )
            {
                ShowError(
                    "Invalid user."
                );

                return;
            }



            // =====================================================
            // GET FORM VALUES
            // =====================================================

            string fullName =
                txtFullName.Text
                    .Trim();


            string email =
                txtEmail.Text
                    .Trim()
                    .ToLower();


            int roleId =
                Convert.ToInt32(
                    ddlRole.SelectedValue
                );


            string gender =
                ddlGender.SelectedValue;


            string schoolName =
                ddlSchool.SelectedValue;



            // =====================================================
            // DATABASE CONNECTION
            // =====================================================

            string connectionString =
                ConfigurationManager
                    .ConnectionStrings[
                        "SPMasterConnectionString"
                    ]
                    .ConnectionString;



            using (
                SqlConnection connection =
                    new SqlConnection(
                        connectionString
                    )
            )
            {
                try
                {
                    connection.Open();



                    // =============================================
                    // CHECK DUPLICATE EMAIL
                    // =============================================

                    string checkQuery = @"
                        SELECT
                            COUNT(*)

                        FROM Users

                        WHERE
                            Email = @Email

                            AND

                            UserId <> @UserId
                    ";


                    using (
                        SqlCommand checkCommand =
                            new SqlCommand(
                                checkQuery,
                                connection
                            )
                    )
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
                                checkCommand
                                    .ExecuteScalar()
                            );


                        if (
                            count > 0
                        )
                        {
                            ShowError(
                                "Another user already uses this email."
                            );

                            return;
                        }
                    }



                    // =============================================
                    // UPDATE USER
                    //
                    // Password is NOT changed here.
                    // =============================================

                    string updateQuery = @"
                        UPDATE Users

                        SET
                            RoleId = @RoleId,
                            FullName = @FullName,
                            Email = @Email,
                            Gender = @Gender,
                            SchoolName = @SchoolName

                        WHERE
                            UserId = @UserId
                    ";



                    using (
                        SqlCommand updateCommand =
                            new SqlCommand(
                                updateQuery,
                                connection
                            )
                    )
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


                        int result =
                            updateCommand
                                .ExecuteNonQuery();


                        if (
                            result > 0
                        )
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
                catch (
                    Exception ex
                )
                {
                    ShowError(
                        "Unable to update user: "
                        + ex.Message
                    );
                }
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


            lblMessage.Visible =
                true;
        }
    }
}