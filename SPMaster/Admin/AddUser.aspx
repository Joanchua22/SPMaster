<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="AddUser.aspx.cs"
    Inherits="SPMaster.Admin.AddUser" %>


<asp:Content
    ID="AdminTitle"
    ContentPlaceHolderID="DashboardTitle"
    runat="server">

    Add User

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
                    Add New User
                </h2>

                <p>
                    Create a new student, lecturer or admin account.
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


                <!-- FULL NAME -->
                <div class="admin-form-group">

                    <label for="<%= txtFullName.ClientID %>">
                        Full Name
                    </label>

                    <asp:TextBox
                        ID="txtFullName"
                        runat="server"
                        CssClass="admin-form-input"
                        MaxLength="50"
                        placeholder="Enter full name">
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


                <!-- EMAIL -->
                <div class="admin-form-group">

                    <label for="<%= txtEmail.ClientID %>">
                        Email
                    </label>

                    <asp:TextBox
                        ID="txtEmail"
                        runat="server"
                        CssClass="admin-form-input"
                        TextMode="Email"
                        MaxLength="254"
                        placeholder="user@example.com">
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


                <!-- ROLE -->
                <div class="admin-form-group">

                    <label for="<%= ddlRole.ClientID %>">
                        Role
                    </label>

                    <asp:DropDownList
                        ID="ddlRole"
                        runat="server"
                        CssClass="admin-form-input">

                        <asp:ListItem
                            Text="Select role"
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

                    <asp:RequiredFieldValidator
                        ID="rfvRole"
                        runat="server"
                        ControlToValidate="ddlRole"
                        InitialValue=""
                        ErrorMessage="Please select a role."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>

                </div>


                <!-- GENDER -->
                <div class="admin-form-group">

                    <label for="<%= ddlGender.ClientID %>">
                        Gender
                    </label>

                    <asp:DropDownList
                        ID="ddlGender"
                        runat="server"
                        CssClass="admin-form-input">

                        <asp:ListItem
                            Text="Select gender"
                            Value="">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Male"
                            Value="Male">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Female"
                            Value="Female">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Prefer not to say"
                            Value="Prefer not to say">
                        </asp:ListItem>

                    </asp:DropDownList>

                    <asp:RequiredFieldValidator
                        ID="rfvGender"
                        runat="server"
                        ControlToValidate="ddlGender"
                        InitialValue=""
                        ErrorMessage="Please select a gender."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>

                </div>


                <!-- SCHOOL -->
                <div class="admin-form-group form-group-full">

                    <label for="<%= txtSchool.ClientID %>">
                        School / Institution
                    </label>

                    <asp:TextBox
                        ID="txtSchool"
                        runat="server"
                        CssClass="admin-form-input"
                        MaxLength="150"
                        placeholder="Enter school or institution">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvSchool"
                        runat="server"
                        ControlToValidate="txtSchool"
                        ErrorMessage="School / Institution is required."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>

                </div>


                <!-- PASSWORD -->
                <div class="admin-form-group">

                    <label for="<%= txtPassword.ClientID %>">
                        Password
                    </label>

                    <asp:TextBox
                        ID="txtPassword"
                        runat="server"
                        CssClass="admin-form-input"
                        TextMode="Password"
                        placeholder="Enter password">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvPassword"
                        runat="server"
                        ControlToValidate="txtPassword"
                        ErrorMessage="Password is required."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>

                    <asp:RegularExpressionValidator
                        ID="revPassword"
                        runat="server"
                        ControlToValidate="txtPassword"
                        ValidationExpression="^.{8,}$"
                        ErrorMessage="Password must be at least 8 characters."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RegularExpressionValidator>

                </div>


                <!-- CONFIRM PASSWORD -->
                <div class="admin-form-group">

                    <label for="<%= txtConfirmPassword.ClientID %>">
                        Confirm Password
                    </label>

                    <asp:TextBox
                        ID="txtConfirmPassword"
                        runat="server"
                        CssClass="admin-form-input"
                        TextMode="Password"
                        placeholder="Confirm password">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvConfirmPassword"
                        runat="server"
                        ControlToValidate="txtConfirmPassword"
                        ErrorMessage="Please confirm the password."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>

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
                    ID="btnAddUser"
                    runat="server"
                    Text="Create User"
                    CssClass="create-user-button"
                    OnClick="btnAddUser_Click" />

            </div>

        </div>

    </section>

</asp:Content>