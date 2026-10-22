using System;
using System.Configuration;
using System.Data.SqlClient;


namespace SPMaster.Master
{
    public partial class Portal :
        System.Web.UI.MasterPage
    {
        protected string CurrentRole
        {
            get
            {
                if (Session["Role"] == null)
                {
                    return "";
                }

                return Session["Role"]
                    .ToString()
                    .Trim();
            }
        }


        protected bool IsLecturer
        {
            get
            {
                return CurrentRole.Equals(
                    "Lecturer",
                    StringComparison.OrdinalIgnoreCase
                );
            }
        }


        protected string PortalFolder
        {
            get
            {
                return IsLecturer
                    ? "~/Lecturer/"
                    : "~/Student/";
            }
        }


        protected string DisplayName
        {
            get
            {
                if (Session["FullName"] == null)
                {
                    return "User";
                }

                return Session["FullName"]
                    .ToString()
                    .Trim();
            }
        }


        protected string DisplayRole
        {
            get
            {
                if (
                    string.IsNullOrWhiteSpace(
                        CurrentRole
                    )
                )
                {
                    return "User";
                }

                return CurrentRole;
            }
        }


        protected string UserInitials
        {
            get
            {
                return GetInitials(
                    DisplayName
                );
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
        // LOAD CURRENT USER
        // =========================================================

        public void LoadCurrentUser()
        {
            if (
                Session["UserId"] == null
            )
            {
                return;
            }


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


            string connectionString =
                ConfigurationManager
                    .ConnectionStrings[
                        "SPMasterConnectionString"
                    ]
                    .ConnectionString;


            const string query = @"
                SELECT
                    U.FullName,
                    U.ProfileImagePath,
                    R.RoleName

                FROM Users U

                INNER JOIN Roles R
                    ON U.RoleId = R.RoleId

                WHERE
                    U.UserId = @UserId;
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
                            string fullName =
                                reader[
                                    "FullName"
                                ]
                                .ToString();


                            string role =
                                reader[
                                    "RoleName"
                                ]
                                .ToString();


                            string profileImagePath =
                                reader[
                                    "ProfileImagePath"
                                ] == DBNull.Value
                                ? ""
                                : reader[
                                    "ProfileImagePath"
                                ]
                                .ToString();



                            // =====================================
                            // UPDATE SESSION
                            // =====================================

                            Session["FullName"] =
                                fullName;


                            Session["Role"] =
                                role;



                            // =====================================
                            // TOP-RIGHT NAME
                            // =====================================

                            lblPortalUserName.Text =
                                fullName;



                            // =====================================
                            // INITIALS
                            // =====================================

                            lblPortalInitials.Text =
                                GetInitials(
                                    fullName
                                );



                            // =====================================
                            // PROFILE IMAGE
                            // =====================================

                            if (
                                string.IsNullOrWhiteSpace(
                                    profileImagePath
                                )
                            )
                            {
                                imgPortalAvatar.Visible =
                                    false;


                                lblPortalInitials.Visible =
                                    true;
                            }
                            else
                            {
                                imgPortalAvatar.ImageUrl =
                                    ResolveUrl(
                                        profileImagePath
                                    );


                                imgPortalAvatar.Visible =
                                    true;


                                lblPortalInitials.Visible =
                                    false;
                            }
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
                first
                +
                last
            )
            .ToUpper();
        }
    }
}