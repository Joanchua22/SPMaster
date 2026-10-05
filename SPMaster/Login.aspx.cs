using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace SPMaster
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(
    object sender,
    EventArgs e)
        {

            if (!IsPostBack)
            {

                lblMessage.Visible = false;


                if (
                    Request.QueryString["registered"]
                    == "true"
                )
                {

                    lblMessage.Text =
                        "Account created successfully. You can now log in.";

                    lblMessage.CssClass =
                        "success-message";

                    lblMessage.Visible = true;

                }


                if (
                    Request.Cookies[
                        "RememberedEmail"
                    ] != null
                )
                {

                    txtEmail.Text =
                        Request.Cookies[
                            "RememberedEmail"
                        ].Value;

                    chkRemember.Checked = true;

                }

            }

        }


        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Run ASP.NET validation first
            if (!Page.IsValid)
            {
                return;
            }


            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;


            string connectionString =
                ConfigurationManager
                .ConnectionStrings["SPMasterConnectionString"]
                .ConnectionString;


            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        U.UserId,
                        U.FullName,
                        U.Email,
                        U.Password,
                        U.RoleId,
                        R.RoleName
                    FROM Users U
                    INNER JOIN Roles R
                        ON U.RoleId = R.RoleId
                    WHERE U.Email = @Email
                      AND U.IsActive = 1
                ";


                using (SqlCommand command =
                       new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Email",
                        email
                    );


                    try
                    {
                        connection.Open();


                        using (SqlDataReader reader =
                               command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPassword =
                                    reader["Password"].ToString();


                                /*
                                 * TEMPORARY:
                                 * This assumes your database currently
                                 * stores the same password value.
                                 *
                                 * Later, replace this with proper
                                 * password hashing verification.
                                 */

                                if (VerifyPassword(password, storedPassword))
                                {
                                    Session["UserId"] =
                                        reader["UserId"].ToString();

                                    Session["FullName"] =
                                        reader["FullName"].ToString();

                                    Session["Email"] =
                                        reader["Email"].ToString();

                                    Session["RoleId"] =
                                        reader["RoleId"].ToString();

                                    Session["Role"] =
                                        reader["RoleName"].ToString();

                                    string role =
                                        reader["RoleName"].ToString().Trim();

                                    RedirectUser(role);
                                }
                                else
                                {
                                    ShowError("Incorrect email or password.");
                                }
                            }
                            else
                            {
                                ShowError(
                                    "Incorrect email or password."
                                );
                            }
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


        private void RedirectUser(string role)
        {
            switch (role.ToLower())
            {
                case "student":
                    Response.Redirect("~/Student/Portal.aspx");
                    break;

                case "lecturer":
                    Response.Redirect("~/Lecturer/Portal.aspx");
                    break;

                case "admin":
                    Response.Redirect("~/Admin/Dashboard.aspx");
                    break;

                default:
                    ShowError("Your account role is not recognised.");
                    break;
            }
        }

        private bool VerifyPassword(
            string password,
            string storedPassword)
                {

                    try
                    {

                        string[] parts =
                            storedPassword.Split('.');


                        if (parts.Length != 3)
                        {
                            return false;
                        }


                        int iterations =
                            int.Parse(parts[0]);


                        byte[] salt =
                            Convert.FromBase64String(
                                parts[1]
                            );


                        byte[] storedHash =
                            Convert.FromBase64String(
                                parts[2]
                            );


                        byte[] newHash;


                        using (
                            Rfc2898DeriveBytes pbkdf2 =
                            new Rfc2898DeriveBytes(
                                password,
                                salt,
                                iterations
                            )
                        )
                        {

                            newHash =
                                pbkdf2.GetBytes(
                                    storedHash.Length
                                );

                        }


                        if (newHash.Length !=
                            storedHash.Length)
                        {
                            return false;
                        }


                        int difference = 0;


                        for (
                            int i = 0;
                            i < newHash.Length;
                            i++
                        )
                        {

                            difference |=
                                newHash[i] ^
                                storedHash[i];

                        }


                        return difference == 0;

                    }

                    catch
                    {

                        return false;

                    }
                }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;
        }
    }
}