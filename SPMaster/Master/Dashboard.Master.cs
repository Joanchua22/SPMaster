using System;
using System.Configuration;
using System.Data.SqlClient;


namespace SPMaster
{
    public partial class Dashboard :
        System.Web.UI.MasterPage
    {
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
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (
                Session["UserId"] == null
            )
            {
                Response.Redirect(
                    "~/Login.aspx"
                );

                return;
            }


            if (!IsPostBack)
            {
                LoadCurrentUser();
            }
        }


        // =========================================================
        // LOAD CURRENT LOGGED-IN USER
        // =========================================================

        public void LoadCurrentUser()
        {
            int userId;


            if (
                !int.TryParse(
                    Session["UserId"]
                        .ToString(),
                    out userId
                )
            )
            {
                return;
            }


            const string sql = @"
                SELECT

                    U.FullName,
                    U.ProfileImagePath,

                    R.RoleName

                FROM Users U

                INNER JOIN Roles R
                    ON U.RoleId =
                       R.RoleId

                WHERE
                    U.UserId =
                    @UserId;
            ";


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
                            sql,
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
                        if (!reader.Read())
                        {
                            return;
                        }


                        string fullName =
                            reader["FullName"]
                                .ToString();


                        string role =
                            reader["RoleName"]
                                .ToString();


                        string imagePath =
                            reader[
                                "ProfileImagePath"
                            ] == DBNull.Value
                            ? ""
                            : reader[
                                "ProfileImagePath"
                              ].ToString();


                        // =========================================
                        // NAME + ROLE
                        // =========================================

                        lblDashboardUserName.Text =
                            fullName;


                        lblDashboardUserRole.Text =
                            role;


                        lblDashboardInitials.Text =
                            GetInitials(
                                fullName
                            );


                        // =========================================
                        // PROFILE IMAGE
                        // =========================================

                        if (
                            string.IsNullOrWhiteSpace(
                                imagePath
                            )
                        )
                        {
                            imgDashboardAvatar.Visible =
                                false;


                            lblDashboardInitials.Visible =
                                true;
                        }
                        else
                        {
                            imgDashboardAvatar.ImageUrl =
                                ResolveUrl(
                                    imagePath
                                );


                            imgDashboardAvatar.Visible =
                                true;


                            lblDashboardInitials.Visible =
                                false;
                        }
                    }
                }
            }
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
                first + last
            ).ToUpper();
        }
    }
}