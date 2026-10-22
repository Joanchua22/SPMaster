<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/Master/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="EditUser.aspx.cs"
    Inherits="SPMaster.Admin.EditUser" %>

<asp:Content
    ID="EditUserHead"
    ContentPlaceHolderID="DashboardHead"
    runat="server">

    <link
        href="<%= ResolveUrl("~/Content/EditUser.css") %>"
        rel="stylesheet" />

</asp:Content>

<asp:Content
    ID="AdminTitle"
    ContentPlaceHolderID="DashboardTitle"
    runat="server">

    Edit User

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

        <a href="<%= ResolveUrl("~/Account/Profile.aspx") %>"
         class="nav-item">

            <span class="nav-icon">
                <i class="bi bi-person-circle"></i>
            </span>

            <span>Profile</span>
        </a>


        <a href="<%= ResolveUrl("~/Account/Settings.aspx") %>"
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

    <section class="add-user-page">

        <div class="edit-user-actions">

            <p class="edit-user-description">
                Update the selected user's account details.
            </p>


            <asp:HyperLink
                ID="lnkBack"
                runat="server"
                NavigateUrl="~/Admin/UserManagement.aspx"
                CssClass="back-user-button">

                ← Back to Users

            </asp:HyperLink>

        </div>


        <div class="add-user-card">

            <asp:Label
                ID="lblMessage"
                runat="server"
                CssClass="admin-form-message"
                Visible="false">
            </asp:Label>


            <div class="admin-form-grid">

                <div class="admin-form-group">

                    <label for="<%= txtFullName.ClientID %>">
                        Full Name
                    </label>

                    <asp:TextBox
                        ID="txtFullName"
                        runat="server"
                        CssClass="admin-form-input"
                        MaxLength="50">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvFullName"
                        runat="server"
                        ControlToValidate="txtFullName"
                        ErrorMessage="Full name is required."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>

                </div>


                <div class="admin-form-group">

                    <label for="<%= txtEmail.ClientID %>">
                        Email
                    </label>

                    <asp:TextBox
                        ID="txtEmail"
                        runat="server"
                        CssClass="admin-form-input"
                        TextMode="Email"
                        MaxLength="254">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvEmail"
                        runat="server"
                        ControlToValidate="txtEmail"
                        ErrorMessage="Email is required."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>

                    <asp:RegularExpressionValidator
                        ID="revEmail"
                        runat="server"
                        ControlToValidate="txtEmail"
                        ErrorMessage="Please enter a valid email."
                        CssClass="validation-message"
                        Display="Dynamic"
                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$">
                    </asp:RegularExpressionValidator>

                </div>


                <div class="admin-form-group">

                    <label for="<%= ddlRole.ClientID %>">
                        Role
                    </label>

                    <asp:DropDownList
                        ID="ddlRole"
                        runat="server"
                        CssClass="admin-form-input">

                        <asp:ListItem Text="Student" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Lecturer" Value="2"></asp:ListItem>
                        <asp:ListItem Text="Admin" Value="3"></asp:ListItem>

                    </asp:DropDownList>

                </div>


                <div class="admin-form-group">

                    <label for="<%= ddlGender.ClientID %>">
                        Gender
                    </label>

                    <asp:DropDownList
                        ID="ddlGender"
                        runat="server"
                        CssClass="admin-form-input">

                        <asp:ListItem Text="Male" Value="Male"></asp:ListItem>
                        <asp:ListItem Text="Female" Value="Female"></asp:ListItem>
                    </asp:DropDownList>

                </div>


                <div class="admin-form-group form-group-full">

                    <label for="<%= ddlSchool.ClientID %>">
                        School / Institution
                    </label>

                    <asp:DropDownList
                        ID="ddlSchool"
                        runat="server"
                        CssClass="admin-form-input">

                        <asp:ListItem Text="Asia Pacific University (APU)" Value="Asia Pacific University"></asp:ListItem>
                        <asp:ListItem Text="SMK" Value="SMK"></asp:ListItem>
                        <asp:ListItem Text="Private School" Value="Private School"></asp:ListItem>
                        <asp:ListItem Text="International School" Value="International School"></asp:ListItem>
                        <asp:ListItem Text="Other" Value="Other"></asp:ListItem>

                    </asp:DropDownList>

                </div>

            <div class="edit-user-form-actions">

                <asp:HyperLink
                    ID="lnkCancel"
                    runat="server"
                    NavigateUrl="~/Admin/UserManagement.aspx"
                    CssClass="edit-user-cancel-button">
                    Cancel
                </asp:HyperLink>


                <asp:Button
                    ID="btnSave"
                    runat="server"
                    Text="Save Changes"
                    CssClass="edit-user-save-button"
                    OnClick="btnSave_Click" />

            </div>

        </div>

    </section>

</asp:Content>