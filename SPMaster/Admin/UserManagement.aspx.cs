using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SPMaster.Admin
{
    public partial class UserManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Always hide the popup first.
            // This prevents it from appearing again during search/filter postbacks.
            pnlSuccess.Visible = false;


            if (!IsPostBack)
            {
                LoadUsers();


                // Only show immediately after AddUser redirects here.
                if (Request.QueryString["added"] == "true")
                {
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
    }
}