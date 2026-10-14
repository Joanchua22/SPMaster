using System;
using System.Configuration;
using System.Data.SqlClient;

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


        protected void btnLogin_Click(
            object sender,
            EventArgs e)
        {
            // Run ASP.NET validation first
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


            using (
                SqlConnection connection =
                new SqlConnection(
                    connectionString
                )
            )
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

                    WHERE
                        U.Email = @Email
                        AND U.IsActive = 1
                ";


                using (
                    SqlCommand command =
                    new SqlCommand(
                        query,
                        connection
                    )
                )
                {
                    command.Parameters.AddWithValue(
                        "@Email",
                        email
                    );


                    try
                    {
                        connection.Open();


                        using (
                            SqlDataReader reader =
                            command.ExecuteReader()
                        )
                        {
                            if (reader.Read())
                            {
                                string storedPassword =
                                    reader["Password"]
                                    .ToString();


                                // TEMPORARY:
                                // Plain-text password comparison
                                if (password == storedPassword)
                                {
                                    Session["UserId"] =
                                        reader["UserId"]
                                        .ToString();

                                    Session["FullName"] =
                                        reader["FullName"]
                                        .ToString();

                                    Session["Email"] =
                                        reader["Email"]
                                        .ToString();

                                    Session["RoleId"] =
                                        reader["RoleId"]
                                        .ToString();

                                    Session["Role"] =
                                        reader["RoleName"]
                                        .ToString();


                                    string role =
                                        reader["RoleName"]
                                        .ToString()
                                        .Trim();


                                    // Remember email
                                    if (chkRemember.Checked)
                                    {
                                        Response.Cookies[
                                            "RememberedEmail"
                                        ].Value = email;

                                        Response.Cookies[
                                            "RememberedEmail"
                                        ].Expires =
                                            DateTime.Now.AddDays(30);
                                    }
                                    else
                                    {
                                        if (
                                            Request.Cookies[
                                                "RememberedEmail"
                                            ] != null
                                        )
                                        {
                                            Response.Cookies[
                                                "RememberedEmail"
                                            ].Expires =
                                                DateTime.Now.AddDays(-1);
                                        }
                                    }


                                    RedirectUser(role);
                                }
                                else
                                {
                                    ShowError(
                                        "Incorrect email or password."
                                    );
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


        private void RedirectUser(
            string role)
        {
            switch (role.ToLower())
            {
                case "student":

                    Response.Redirect(
                        "~/Student/Portal.aspx"
                    );

                    break;


                case "lecturer":

                    Response.Redirect(
                        "~/Lecturer/Portal.aspx"
                    );

                    break;


                case "admin":

                    Response.Redirect(
                        "~/Admin/Dashboard.aspx"
                    );

                    break;


                default:

                    ShowError(
                        "Your account role is not recognised."
                    );

                    break;
            }
        }


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