using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace SPMaster.Account
{
    public partial class Profile : System.Web.UI.Page
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
                LoadProfile();
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


        private void LoadProfile()
        {
            int userId =
                GetUserId();


            if (userId == 0)
            {
                Response.Redirect(
                    "~/Login.aspx"
                );

                return;
            }


            string query = @"
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
                        if (!reader.Read())
                        {
                            ShowError(
                                "Unable to find your profile."
                            );

                            btnSaveProfile.Visible =
                                false;

                            return;
                        }


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


                        txtFullName.Text =
                            fullName;

                        txtEmail.Text =
                            email;

                        txtSchool.Text =
                            school;

                        lblInstitutionalRole.Text =
                            role;


                        lblProfileHeading.Text =
                            role + " Profile";


                        lblTopName.Text =
                            fullName;

                        lblTopRole.Text =
                            role;


                        string initials =
                            GetInitials(
                                fullName
                            );


                        lblTopInitials.Text =
                            initials;

                        lblProfileInitials.Text =
                            initials;


                        if (
                            string.IsNullOrWhiteSpace(
                                school
                            )
                        )
                        {
                            lblProfileDescription.Text =
                                role + " Account";
                        }
                        else
                        {
                            lblProfileDescription.Text =
                                school
                                + " "
                                + role
                                + " Account";
                        }


                        ShowProfileImage(
                            profileImagePath
                        );
                    }
                }
            }
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


        private void ShowProfileImage(
            string imagePath)
        {
            if (
                string.IsNullOrWhiteSpace(
                    imagePath
                )
            )
            {
                imgProfile.Visible =
                    false;

                lblProfileInitials.Visible =
                    true;

                return;
            }


            imgProfile.ImageUrl =
                ResolveUrl(
                    imagePath
                );

            imgProfile.Visible =
                true;

            lblProfileInitials.Visible =
                false;
        }


        protected void btnSaveProfile_Click(
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


            string fullName =
                txtFullName.Text.Trim();

            string email =
                txtEmail.Text.Trim();

            string school =
                txtSchool.Text.Trim();


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


            string profileImagePath =
                null;


            bool hasNewImage =
                fuProfilePhoto.HasFile;


            if (hasNewImage)
            {
                string extension =
                    Path.GetExtension(
                        fuProfilePhoto.FileName
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
                        extension ==
                        allowedExtension
                    )
                    {
                        allowed = true;
                        break;
                    }
                }


                if (!allowed)
                {
                    ShowError(
                        "Please upload a JPG, PNG or WEBP image."
                    );

                    return;
                }


                if (
                    fuProfilePhoto.PostedFile
                    .ContentLength
                    > 2 * 1024 * 1024
                )
                {
                    ShowError(
                        "Profile image must be smaller than 2 MB."
                    );

                    return;
                }


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


                fuProfilePhoto.SaveAs(
                    physicalPath
                );


                profileImagePath =
                    "~/Images/ProfileImages/"
                    + fileName;
            }


            string query;


            if (hasNewImage)
            {
                query = @"
                    UPDATE Users

                    SET
                        FullName = @FullName,
                        Email = @Email,
                        SchoolName = @SchoolName,
                        ProfileImagePath =
                            @ProfileImagePath

                    WHERE
                        UserId = @UserId
                ";
            }
            else
            {
                query = @"
                    UPDATE Users

                    SET
                        FullName = @FullName,
                        Email = @Email,
                        SchoolName = @SchoolName

                    WHERE
                        UserId = @UserId
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
                    command.Parameters.AddWithValue(
                        "@FullName",
                        fullName
                    );

                    command.Parameters.AddWithValue(
                        "@Email",
                        email
                    );

                    command.Parameters.AddWithValue(
                        "@SchoolName",
                        school
                    );

                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId
                    );


                    if (hasNewImage)
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


                        Session["FullName"] =
                            fullName;

                        Session["Email"] =
                            email;


                        lblSuccessMessage.Text =
                            "Profile updated successfully.";

                        pnlSuccess.Visible =
                            true;


                        LoadProfile();
                    }
                    catch (Exception)
                    {
                        ShowError(
                            "Unable to update your profile. Please try again."
                        );
                    }
                }
            }
        }


        protected void btnResetPhoto_Click(
            object sender,
            EventArgs e)
        {
            int userId =
                GetUserId();


            if (userId == 0)
            {
                return;
            }


            string query = @"
                UPDATE Users

                SET
                    ProfileImagePath = NULL

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


                    try
                    {
                        connection.Open();

                        command.ExecuteNonQuery();


                        lblSuccessMessage.Text =
                            "Profile photo reset successfully.";

                        pnlSuccess.Visible =
                            true;


                        LoadProfile();
                    }
                    catch (Exception)
                    {
                        ShowError(
                            "Unable to reset your profile photo."
                        );
                    }
                }
            }
        }


        private bool EmailExists(
            string email,
            int userId)
        {
            string query = @"
                SELECT COUNT(*)

                FROM Users

                WHERE
                    Email = @Email

                    AND UserId <> @UserId
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
                        "@Email",
                        email
                    );

                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId
                    );


                    connection.Open();


                    int count =
                        Convert.ToInt32(
                            command.ExecuteScalar()
                        );


                    return count > 0;
                }
            }
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