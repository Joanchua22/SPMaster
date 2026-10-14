using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SPMaster.Admin
{
    public partial class UserManagement : System.Web.UI.Page
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
                {
                    pnlSuccess.Visible = false;


                    if (!IsPostBack)
                    {
                        LoadUsers();


                        if (
                            Request.QueryString["added"]
                            == "true"
                        )
                        {
                            lblSuccessMessage.Text =
                                "User created successfully.";

                            pnlSuccess.Visible = true;
                        }


                        else if (
                            Request.QueryString["updated"]
                            == "true"
                        )
                        {
                            lblSuccessMessage.Text =
                                "User updated successfully.";

                            pnlSuccess.Visible = true;
                        }


                        else if (
                            Request.QueryString["statuschanged"]
                            == "true"
                        )
                        {
                            lblSuccessMessage.Text =
                                "User status updated successfully.";

                            pnlSuccess.Visible = true;
                        }

                        else if (
                            Request.QueryString["deleted"]
                            == "true"
                         )
                         {
                             lblSuccessMessage.Text =
                                 "User deleted successfully.";

                            pnlSuccess.Visible = true;
                        }
            }
        }


        private void LoadUsers()
        {
            string connectionString =
                ConfigurationManager
                .ConnectionStrings["SPMasterConnectionString"]
                .ConnectionString;


            string query = @"
                SELECT
                    U.UserId,
                    U.FullName,
                    U.Email,
                    U.IsActive,
                    U.CreatedAt,
                    U.RoleId,
                    R.RoleName

                FROM Users U

                INNER JOIN Roles R
                    ON U.RoleId = R.RoleId

                WHERE 1 = 1
            ";


            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand command =
                       new SqlCommand())
                {
                    command.Connection = connection;


                    // ROLE FILTER
                    if (!string.IsNullOrEmpty(
                        ddlRoleFilter.SelectedValue))
                    {
                        query += @"
                    AND U.RoleId = @RoleId
                ";

                        command.Parameters.AddWithValue(
                            "@RoleId",
                            Convert.ToInt32(
                                ddlRoleFilter.SelectedValue
                            )
                        );
                    }


                    // STATUS FILTER
                    if (!string.IsNullOrEmpty(
                        ddlStatusFilter.SelectedValue))
                    {
                        query += @"
                    AND U.IsActive = @IsActive
                ";

                        command.Parameters.AddWithValue(
                            "@IsActive",
                            ddlStatusFilter.SelectedValue == "1"
                        );
                    }


                    // SEARCH
                    string search =
                        txtSearch.Text.Trim();

                    if (!string.IsNullOrEmpty(search))
                    {
                        query += @"
                    AND
                    (
                        U.FullName LIKE @Search
                        OR
                        U.Email LIKE @Search
                    )
                ";

                        command.Parameters.AddWithValue(
                            "@Search",
                            "%" + search + "%"
                        );
                    }


                    query += @"
                ORDER BY U.CreatedAt DESC
            ";


                    command.CommandText = query;


                    connection.Open();


                    SqlDataAdapter adapter =
                        new SqlDataAdapter(command);


                    DataTable table =
                        new DataTable();


                    adapter.Fill(table);


                    rptUsers.DataSource =
                        table;

                    rptUsers.DataBind();
                }
            }
        }

        protected void FilterChanged(
            object sender,
            EventArgs e)
                {
                    LoadUsers();
                }


                protected void SearchChanged(
                    object sender,
                    EventArgs e)
                {
                    LoadUsers();
                }
        protected void ToggleStatus_Command(
            object sender,
            System.Web.UI.WebControls.CommandEventArgs e)
                {
                    int userId;

                    if (!int.TryParse(
                        e.CommandArgument.ToString(),
                        out userId))
                    {
                        return;
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


                            // Get current status
                            string statusQuery = @"
                        SELECT IsActive
                        FROM Users
                        WHERE UserId = @UserId
                    ";


                            bool currentStatus;


                            using (SqlCommand statusCommand =
                                   new SqlCommand(
                                       statusQuery,
                                       connection))
                            {
                                statusCommand.Parameters
                                    .AddWithValue(
                                        "@UserId",
                                        userId
                                    );


                                object result =
                                    statusCommand.ExecuteScalar();


                                if (result == null)
                                {
                                    return;
                                }


                                currentStatus =
                                    Convert.ToBoolean(result);
                            }


                            // Toggle status
                            bool newStatus =
                                !currentStatus;


                            string updateQuery = @"
                        UPDATE Users
                        SET IsActive = @IsActive
                        WHERE UserId = @UserId
                    ";


                            using (SqlCommand updateCommand =
                                   new SqlCommand(
                                       updateQuery,
                                       connection))
                            {
                                updateCommand.Parameters
                                    .AddWithValue(
                                        "@IsActive",
                                        newStatus
                                    );


                                updateCommand.Parameters
                                    .AddWithValue(
                                        "@UserId",
                                        userId
                                    );


                                int rows =
                                    updateCommand.ExecuteNonQuery();


                                if (rows > 0)
                                {
                                    Response.Redirect(
                                        "~/Admin/UserManagement.aspx?statuschanged=true"
                                    );
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // For now just reload the page.
                            // We can add an error popup later if needed.
                            LoadUsers();
                        }
                    }
                }

        protected void DeleteUser_Command(
            object sender,
            System.Web.UI.WebControls.CommandEventArgs e)
                {
                    int userId;

                    if (!int.TryParse(
                        e.CommandArgument.ToString(),
                        out userId))
                    {
                        return;
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


                            string deleteQuery = @"
                        DELETE FROM Users
                        WHERE UserId = @UserId
                    ";


                            using (SqlCommand deleteCommand =
                                   new SqlCommand(
                                       deleteQuery,
                                       connection))
                            {
                                deleteCommand.Parameters
                                    .AddWithValue(
                                        "@UserId",
                                        userId
                                    );


                                int rows =
                                    deleteCommand.ExecuteNonQuery();


                                if (rows > 0)
                                {
                                    Response.Redirect(
                                        "~/Admin/UserManagement.aspx?deleted=true"
                                    );
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // If deletion fails, show the reason.
                            // For example, foreign-key references may prevent deletion.
                            lblSuccessMessage.Text =
                                "Unable to delete user: " + ex.Message;

                            pnlSuccess.CssClass =
                                "error-popup";

                            pnlSuccess.Visible = true;
                        }
                    }
                }
    }
}