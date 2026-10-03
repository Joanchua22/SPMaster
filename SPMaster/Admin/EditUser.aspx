<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/Master/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="EditUser.aspx.cs"
    Inherits="SPMaster.Admin.EditUser" %>

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

    <section class="add-user-page">

        <div class="add-user-header">

            <div>

                <h2>
                    Edit User
                </h2>

                <p>
                    Update the selected user's account details.
                </p>

            </div>


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


                <div class="admin-form-group">

                    <label for="<%= txtPassword.ClientID %>">
                        New Password
                    </label>

                    <asp:TextBox
                        ID="txtPassword"
                        runat="server"
                        CssClass="admin-form-input"
                        TextMode="Password"
                        placeholder="Leave blank to keep current password">
                    </asp:TextBox>
                    <asp:RegularExpressionValidator
                        ID="revPassword"
                        runat="server"
                        ControlToValidate="txtPassword"
                        ValidationExpression="^$|^.{8,}$"
                        ErrorMessage="Password must be at least 8 characters."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RegularExpressionValidator>

                </div>


                <div class="admin-form-group">

                    <label for="<%= txtConfirmPassword.ClientID %>">
                        Confirm New Password
                    </label>

                    <asp:TextBox
                        ID="txtConfirmPassword"
                        runat="server"
                        CssClass="admin-form-input"
                        TextMode="Password"
                        placeholder="Leave blank to keep current password">
                    </asp:TextBox>

                    <asp:CompareValidator
                        ID="cvPassword"
                        runat="server"
                        ControlToValidate="txtConfirmPassword"
                        ControlToCompare="txtPassword"
                        ErrorMessage="Passwords do not match."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:CompareValidator>

                </div>

            </div>


            <div class="admin-form-actions">

                <asp:HyperLink
                    ID="lnkCancel"
                    runat="server"
                    NavigateUrl="~/Admin/UserManagement.aspx"
                    CssClass="cancel-user-button">

                    Cancel

                </asp:HyperLink>


                <asp:Button
                    ID="btnSave"
                    runat="server"
                    Text="Save Changes"
                    CssClass="create-user-button"
                    OnClick="btnSave_Click" />

            </div>

        </div>

    </section>

</asp:Content>