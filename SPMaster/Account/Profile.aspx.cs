using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;


namespace SPMaster.Account
{
    public partial class Profile :
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
                LoadProfile();
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
        // LOAD PROFILE
        // =========================================================

        private void LoadProfile()
        {
            int userId =
                GetUserId();


            if (
                userId == 0
            )
            {
                Response.Redirect(
                    "~/Login.aspx"
                );

                return;
            }


            const string query = @"
                SELECT

                    U.UserId,
                    U.FullName,
                    U.Email,
                    U.SchoolName,
                    U.ProfileImagePath,
                    U.CreatedAt,

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
                            !reader.Read()
                        )
                        {
                            ShowError(
                                "Unable to find your profile."
                            );


                            btnSaveProfile.Visible =
                                false;


                            return;
                        }


                        // =========================================
                        // USER INFORMATION
                        // =========================================

                        string fullName =
                            reader["FullName"]
                                .ToString();


                        string email =
                            reader["Email"]
                                .ToString();


                        string school =
                            reader["SchoolName"]
                                .ToString();


                        string role =
                            reader["RoleName"]
                                .ToString();


                        string profileImagePath =
                            reader["ProfileImagePath"]
                            == DBNull.Value
                            ? ""
                            : reader[
                                "ProfileImagePath"
                              ].ToString();



                        // =========================================
                        // FORM VALUES
                        // =========================================

                        txtFullName.Text =
                            fullName;


                        txtEmail.Text =
                            email;


                        if (ddlSchool.Items.FindByValue(school) != null)
                        {
                            ddlSchool.SelectedValue =
                                school;
                        }
                        else
                        {
                            ddlSchool.SelectedValue =
                                "";
                        }


                        lblInstitutionalRole.Text =
                            role;



                        // =========================================
                        // PAGE HEADING
                        // =========================================

                        lblProfileHeading.Text =
                            role
                            + " Profile";



                        // =========================================
                        // PROFILE INITIALS
                        // =========================================

                        string initials =
                            GetInitials(
                                fullName
                            );


                        lblProfileInitials.Text =
                            initials;



                        // =========================================
                        // PROFILE DESCRIPTION
                        // =========================================

                        if (
                            string.IsNullOrWhiteSpace(
                                school
                            )
                        )
                        {
                            lblProfileDescription.Text =
                                role
                                + " Account";
                        }
                        else
                        {
                            lblProfileDescription.Text =
                                school
                                + " "
                                + role
                                + " Account";
                        }



                        // =========================================
                        // PROFILE PHOTO
                        // =========================================

                        ShowProfileImage(
                            profileImagePath
                        );
                    }
                }
            }
        }



        // =========================================================
        // GET USER INITIALS
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
                + last
            ).ToUpper();
        }



        // =========================================================
        // DISPLAY SAVED PROFILE IMAGE
        // =========================================================

        private void ShowProfileImage(
            string imagePath)
        {
            if (
                string.IsNullOrWhiteSpace(
                    imagePath
                )
            )
            {
                imgProfile.ImageUrl =
                    "";


                imgProfile.Style[
                    "display"
                ] =
                    "none";


                lblProfileInitials.Style[
                    "display"
                ] =
                    "inline";


                return;
            }


            imgProfile.ImageUrl =
                ResolveUrl(
                    imagePath
                );


            imgProfile.Style[
                "display"
            ] =
                "block";


            lblProfileInitials.Style[
                "display"
            ] =
                "none";
        }



        // =========================================================
        // SAVE PROFILE
        // =========================================================

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            int userId = GetUserId();


            if (userId == 0)
            {
                ShowError("Your session has expired. Please log in again.");
                return;
            }

            string fullName = txtFullName.Text.Trim();

            string email = txtEmail.Text.Trim();

            string school = ddlSchool.SelectedValue.Trim();



            // =====================================================
            // EMAIL DUPLICATE CHECK
            // =====================================================

            if (
                EmailExists(
                    email,
                    userId
                )
            )
            {
                ShowError(
                    "This email address is already used by another account."
                );

                return;
            }



            // =====================================================
            // NEW PROFILE IMAGE
            // =====================================================

            string profileImagePath =
                null;


            bool hasNewImage =
                fuProfilePhoto.HasFile;



            if (
                hasNewImage
            )
            {
                string extension =
                    Path.GetExtension(
                        fuProfilePhoto
                            .FileName
                    )
                    .ToLower();



                string[] allowedExtensions =
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".webp"
                };


                bool allowed =
                    false;


                foreach (
                    string allowedExtension
                    in allowedExtensions
                )
                {
                    if (
                        extension
                        ==
                        allowedExtension
                    )
                    {
                        allowed =
                            true;

                        break;
                    }
                }



                // =================================================
                // INVALID FILE TYPE
                // =================================================

                if (
                    !allowed
                )
                {
                    ShowError(
                        "Please upload a JPG, PNG or WEBP image."
                    );

                    return;
                }



                // =================================================
                // FILE SIZE LIMIT
                // =================================================

                if (
                    fuProfilePhoto
                        .PostedFile
                        .ContentLength
                    >
                    2 * 1024 * 1024
                )
                {
                    ShowError(
                        "Profile image must be smaller than 2 MB."
                    );

                    return;
                }



                // =================================================
                // PROFILE IMAGE FOLDER
                // =================================================

                string folder =
                    Server.MapPath(
                        "~/Images/ProfileImages/"
                    );


                if (
                    !Directory.Exists(
                        folder
                    )
                )
                {
                    Directory.CreateDirectory(
                        folder
                    );
                }



                // =================================================
                // UNIQUE FILE NAME
                // =================================================

                string fileName =
                    "profile_"
                    + userId
                    + "_"
                    + Guid.NewGuid()
                        .ToString("N")
                    + extension;



                string physicalPath =
                    Path.Combine(
                        folder,
                        fileName
                    );



                // =================================================
                // SAVE PHYSICAL IMAGE
                // =================================================

                fuProfilePhoto
                    .SaveAs(
                        physicalPath
                    );



                profileImagePath =
                    "~/Images/ProfileImages/"
                    + fileName;
            }



            // =====================================================
            // DATABASE UPDATE
            // =====================================================

            string query;


            if (
                hasNewImage
            )
            {
                query = @"
                    UPDATE Users

                    SET
                        FullName =
                            @FullName,

                        Email =
                            @Email,

                        SchoolName =
                            @SchoolName,

                        ProfileImagePath =
                            @ProfileImagePath

                    WHERE
                        UserId =
                            @UserId;
                ";
            }
            else
            {
                query = @"
                    UPDATE Users

                    SET
                        FullName =
                            @FullName,

                        Email =
                            @Email,

                        SchoolName =
                            @SchoolName

                    WHERE
                        UserId =
                            @UserId;
                ";
            }



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
                            "@FullName",
                            fullName
                        );


                    command.Parameters
                        .AddWithValue(
                            "@Email",
                            email
                        );


                    command.Parameters
                        .AddWithValue(
                            "@SchoolName",
                            school
                        );


                    command.Parameters
                        .AddWithValue(
                            "@UserId",
                            userId
                        );



                    if (
                        hasNewImage
                    )
                    {
                        command.Parameters
                            .AddWithValue(
                                "@ProfileImagePath",
                                profileImagePath
                            );
                    }



                    try
                    {
                        connection.Open();


                        command.ExecuteNonQuery();



                        // =========================================
                        // UPDATE SESSION
                        // =========================================

                        Session["FullName"] =
                            fullName;


                        Session["Email"] =
                            email;



                        // =========================================
                        // RELOAD PROFILE CARD
                        // =========================================

                        LoadProfile();



                        // =========================================
                        // RELOAD TOP-RIGHT USER HEADER
                        //
                        // Admin:
                        //     Dashboard.Master
                        //
                        // Student / Lecturer:
                        //     Portal.Master
                        // =========================================

                        RefreshMasterUserHeader();



                        // =========================================
                        // SUCCESS MESSAGE
                        // =========================================

                        lblSuccessMessage.Text =
                            "Profile updated successfully.";


                        pnlSuccess.Visible =
                            true;
                    }
                    catch (
                        Exception
                    )
                    {
                        ShowError(
                            "Unable to update your profile. Please try again."
                        );
                    }
                }
            }
        }



        // =========================================================
        // RESET PROFILE PHOTO
        // =========================================================

        protected void btnResetPhoto_Click(
            object sender,
            EventArgs e)
        {
            int userId =
                GetUserId();


            if (
                userId == 0
            )
            {
                return;
            }


            const string query = @"
                UPDATE Users

                SET
                    ProfileImagePath =
                        NULL

                WHERE
                    UserId =
                        @UserId;
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


                    try
                    {
                        connection.Open();


                        command.ExecuteNonQuery();



                        // =========================================
                        // RELOAD PROFILE CARD
                        // =========================================

                        LoadProfile();



                        // =========================================
                        // RELOAD TOP-RIGHT USER HEADER
                        // =========================================

                        RefreshMasterUserHeader();



                        // =========================================
                        // SUCCESS MESSAGE
                        // =========================================

                        lblSuccessMessage.Text =
                            "Profile photo reset successfully.";


                        pnlSuccess.Visible =
                            true;
                    }
                    catch (
                        Exception
                    )
                    {
                        ShowError(
                            "Unable to reset your profile photo."
                        );
                    }
                }
            }
        }



        // =========================================================
        // REFRESH CURRENT MASTER PAGE USER HEADER
        //
        // Admin:
        //     Dashboard.Master
        //
        // Student / Lecturer:
        //     Portal.Master
        // =========================================================

        private void RefreshMasterUserHeader()
        {
            // =====================================================
            // ADMIN
            // =====================================================

            SPMaster.Dashboard dashboardMaster =
                Master
                as SPMaster.Dashboard;


            if (
                dashboardMaster != null
            )
            {
                dashboardMaster
                    .LoadCurrentUser();

                return;
            }



            // =====================================================
            // STUDENT / LECTURER
            // =====================================================

            SPMaster.Master.Portal portalMaster =
                Master
                as SPMaster.Master.Portal;


            if (
                portalMaster != null
            )
            {
                portalMaster
                    .LoadCurrentUser();
            }
        }



        // =========================================================
        // CHECK EMAIL DUPLICATE
        // =========================================================

        private bool EmailExists(
            string email,
            int userId)
        {
            const string query = @"
                SELECT
                    COUNT(*)

                FROM Users

                WHERE
                    Email =
                        @Email

                    AND

                    UserId <>
                        @UserId;
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
                            "@Email",
                            email
                        );


                    command.Parameters
                        .AddWithValue(
                            "@UserId",
                            userId
                        );


                    connection.Open();


                    int count =
                        Convert.ToInt32(
                            command.ExecuteScalar()
                        );


                    return
                        count > 0;
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