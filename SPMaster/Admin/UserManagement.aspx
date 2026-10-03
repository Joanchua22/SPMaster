<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="UserManagement.aspx.cs"
    Inherits="SPMaster.Admin.UserManagement" %>


<asp:Content
    ID="AdminTitle"
    ContentPlaceHolderID="DashboardTitle"
    runat="server">

    User Management

</asp:Content>


<asp:Content
    ID="AdminNavigation"
    ContentPlaceHolderID="DashboardNavigation"
    runat="server">

    <nav class="sidebar-nav" aria-label="Admin navigation">

        <a href="<%= ResolveUrl("~/Admin/Dashboard.aspx") %>"
           class="nav-item">

            <span class="nav-icon">
                <i class="bi bi-grid"></i>
            </span>

            <span>Dashboard</span>

        </a>


        <a href="<%= ResolveUrl("~/Admin/UserManagement.aspx") %>"
           class="nav-item">

            <span class="nav-icon">
                <i class="bi bi-people"></i>
            </span>

            <span>User Management</span>

        </a>


        <a href="<%= ResolveUrl("~/Admin/ContentAudit.aspx") %>"
           class="nav-item">

            <span class="nav-icon">
                <i class="bi bi-file-earmark-text"></i>
            </span>

            <span>Content Audit</span>

        </a>


        <a href="<%= ResolveUrl("~/Admin/FeedbackReports.aspx") %>"
           class="nav-item">

            <span class="nav-icon">
                <i class="bi bi-chat-left-text"></i>
            </span>

            <span>Feedback &amp; Reports</span>

        </a>

    </nav>

</asp:Content>


<asp:Content
    ID="AdminAccountNavigation"
    ContentPlaceHolderID="DashboardAccountNavigation"
    runat="server">

    <nav aria-label="Admin account navigation">

        <a href="#"
           class="nav-item">

            <span class="nav-icon">
                <i class="bi bi-person-circle"></i>
            </span>

            <span>Profile</span>

        </a>


        <a href="#"
           class="nav-item">

            <span class="nav-icon">
                <i class="bi bi-gear"></i>
            </span>

            <span>Settings</span>

        </a>

    </nav>

</asp:Content>


<asp:Content
    ID="AdminUser"
    ContentPlaceHolderID="DashboardUser"
    runat="server">

    <div class="user-summary">

        <span class="user-avatar">
            AD
        </span>

        <div class="user-details">

            <span class="user-name">
                Admin
            </span>

            <span class="user-role">
                Admin
            </span>

        </div>

    </div>

</asp:Content>


<asp:Content
    ID="AdminContent"
    ContentPlaceHolderID="DashboardContent"
    runat="server">

    <section class="user-management-page">


        <!-- SUCCESS POPUP -->
        <asp:Panel
            ID="pnlSuccess"
            runat="server"
            CssClass="success-popup"
            Visible="false"
            EnableViewState="false">

            <div class="success-popup-content">

                <span class="success-icon">
                    ✓
                </span>

                <span>
                    User created successfully.
                </span>

                <button
                    type="button"
                    class="success-close"
                    onclick="closeSuccessPopup()">

                    ×

                </button>

            </div>

        </asp:Panel>


        <!-- PAGE HEADER -->
        <div class="user-management-header">

            <div>

                <h2>
                    User Management
                </h2>

                <p>
                    Manage student, lecturer and admin accounts
                </p>

            </div>


            <asp:HyperLink
                ID="lnkAddUser"
                runat="server"
                NavigateUrl="~/Admin/AddUser.aspx"
                CssClass="add-user-button">

                + Add User

            </asp:HyperLink>

        </div>


        <!-- FILTER BAR -->
        <div class="user-filter-bar">


            <!-- ROLE FILTER -->
            <asp:DropDownList
                ID="ddlRoleFilter"
                runat="server"
                CssClass="filter-select"
                AutoPostBack="true"
                OnSelectedIndexChanged="FilterChanged">

                <asp:ListItem
                    Text="All Roles"
                    Value="">
                </asp:ListItem>

                <asp:ListItem
                    Text="Student"
                    Value="1">
                </asp:ListItem>

                <asp:ListItem
                    Text="Lecturer"
                    Value="2">
                </asp:ListItem>

                <asp:ListItem
                    Text="Admin"
                    Value="3">
                </asp:ListItem>

            </asp:DropDownList>


            <!-- STATUS FILTER -->
            <asp:DropDownList
                ID="ddlStatusFilter"
                runat="server"
                CssClass="filter-select"
                AutoPostBack="true"
                OnSelectedIndexChanged="FilterChanged">

                <asp:ListItem
                    Text="All Status"
                    Value="">
                </asp:ListItem>

                <asp:ListItem
                    Text="Active"
                    Value="1">
                </asp:ListItem>

                <asp:ListItem
                    Text="Deactivated"
                    Value="0">
                </asp:ListItem>

            </asp:DropDownList>


            <!-- SEARCH -->
            <asp:TextBox
                ID="txtSearch"
                runat="server"
                CssClass="user-search"
                placeholder="Search by name or email..."
                AutoPostBack="true"
                OnTextChanged="SearchChanged">
            </asp:TextBox>

        </div>


        <!-- USER TABLE -->
        <div class="user-table-card">

            <div class="user-table-wrapper">

                <table class="user-table">

                    <thead>

                        <tr>

                            <th>Name</th>
                            <th>Email</th>
                            <th>Role</th>
                            <th>Status</th>
                            <th>Joined</th>
                            <th>Actions</th>

                        </tr>

                    </thead>


                    <tbody>

                        <asp:Repeater
                            ID="rptUsers"
                            runat="server">

                            <ItemTemplate>

                                <tr>

                                    <!-- NAME -->
                                    <td class="user-name-cell">

                                        <%# Eval("FullName") %>

                                    </td>


                                    <!-- EMAIL -->
                                    <td>

                                        <%# Eval("Email") %>

                                    </td>


                                    <!-- ROLE -->
                                    <td>

                                        <span class='<%#
                                            Eval("RoleName").ToString() == "Student"
                                            ? "role-pill student-role-pill"
                                            : Eval("RoleName").ToString() == "Lecturer"
                                            ? "role-pill lecturer-role-pill"
                                            : "role-pill admin-role-pill"
                                        %>'>

                                            <%# Eval("RoleName") %>

                                        </span>

                                    </td>


                                    <!-- STATUS -->
                                    <td>

                                        <span class='<%#
                                            Convert.ToBoolean(Eval("IsActive"))
                                            ? "status-pill active-status"
                                            : "status-pill inactive-status"
                                        %>'>

                                            <%#
                                                Convert.ToBoolean(Eval("IsActive"))
                                                ? "Active"
                                                : "Deactivated"
                                            %>

                                        </span>

                                    </td>


                                    <!-- JOINED DATE -->
                                    <td>

                                        <%#
                                            Convert.ToDateTime(
                                                Eval("CreatedAt")
                                            ).ToString("dd MMM yyyy")
                                        %>

                                    </td>


                                    <!-- ACTIONS -->
                                    <td>

                                        <div class="action-buttons">

                                            <!-- EDIT -->
                                            <button
                                                type="button"
                                                class="action-button"
                                                title="Edit">

                                                <i class="bi bi-pencil"></i>

                                            </button>


                                            <!-- ACTIVATE / DEACTIVATE -->
                                            <button
                                                type="button"
                                                class="action-button"
                                                title="Activate / Deactivate">

                                                <i class="bi bi-person-x"></i>

                                            </button>


                                            <!-- DELETE -->
                                            <button
                                                type="button"
                                                class="action-button delete-action"
                                                title="Delete">

                                                <i class="bi bi-trash"></i>

                                            </button>

                                        </div>

                                    </td>

                                </tr>

                            </ItemTemplate>

                        </asp:Repeater>

                    </tbody>

                </table>

            </div>

        </div>

    </section>

</asp:Content>

<asp:Content
    ID="AdminScripts"
    ContentPlaceHolderID="DashboardScripts"
    runat="server">

    <script>

        function closeSuccessPopup() {

            const popup =
                document.querySelector(".success-popup");

            if (popup) {
                popup.style.display = "none";
            }
        }


        window.addEventListener(
            "load",
            function () {

                const popup =
                    document.querySelector(".success-popup");


                // Remove ?added=true from the URL
                const url =
                    new URL(window.location.href);

                if (url.searchParams.get("added") === "true") {

                    url.searchParams.delete("added");

                    window.history.replaceState(
                        {},
                        document.title,
                        url.pathname + url.search
                    );
                }


                // Auto-hide popup
                if (popup) {

                    setTimeout(
                        function () {

                            popup.style.opacity = "0";

                            setTimeout(
                                function () {

                                    popup.style.display =
                                        "none";

                                },
                                300
                            );

                        },
                        3000
                    );

                }

            }
        );

    </script>

</asp:Content>