<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/Master/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="Settings.aspx.cs"
    Inherits="SPMaster.Account.Settings" %>


<asp:Content
    ID="SettingsTitle"
    ContentPlaceHolderID="DashboardTitle"
    runat="server">

    Settings

</asp:Content>


<asp:Content
    ID="SettingsNavigation"
    ContentPlaceHolderID="DashboardNavigation"
    runat="server">

    <%-- ADMIN NAVIGATION --%>
    <asp:Panel
        ID="pnlAdminNavigation"
        runat="server"
        Visible="false">

        <nav class="sidebar-nav"
             aria-label="Admin navigation">

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

    </asp:Panel>


    <%-- LECTURER NAVIGATION --%>
    <asp:Panel
        ID="pnlLecturerNavigation"
        runat="server"
        Visible="false">

        <nav class="sidebar-nav"
             aria-label="Lecturer navigation">

            <a href="<%= ResolveUrl("~/Lecturer/Portal.aspx") %>"
               class="nav-item">

                <span class="nav-icon">
                    <i class="bi bi-grid"></i>
                </span>

                <span>Dashboard</span>

            </a>


            <a href="<%= ResolveUrl("~/Lecturer/QuestionBank.aspx") %>"
               class="nav-item">

                <span class="nav-icon">
                    <i class="bi bi-bank"></i>
                </span>

                <span>Question Bank</span>

            </a>


            <a href="<%= ResolveUrl("~/Lecturer/Quizzes.aspx") %>"
               class="nav-item">

                <span class="nav-icon">
                    <i class="bi bi-journal-text"></i>
                </span>

                <span>Quizzes</span>

            </a>

        </nav>

    </asp:Panel>


    <%-- STUDENT NAVIGATION --%>
    <asp:Panel
        ID="pnlStudentNavigation"
        runat="server"
        Visible="false">

        <nav class="sidebar-nav"
             aria-label="Student navigation">

            <a href="<%= ResolveUrl("~/Student/Portal.aspx") %>"
               class="nav-item">

                <span class="nav-icon">
                    <i class="bi bi-grid"></i>
                </span>

                <span>Dashboard</span>

            </a>

        </nav>

    </asp:Panel>

</asp:Content>


<asp:Content
    ID="SettingsAccountNavigation"
    ContentPlaceHolderID="DashboardAccountNavigation"
    runat="server">

    <nav aria-label="Account navigation">

        <a href="<%= ResolveUrl("~/Account/Profile.aspx") %>"
           class="nav-item">

            <span class="nav-icon">
                <i class="bi bi-person-circle"></i>
            </span>

            <span>Profile</span>

        </a>


        <a href="<%= ResolveUrl("~/Account/Settings.aspx") %>"
           class="nav-item active"
           aria-current="page">

            <span class="nav-icon">
                <i class="bi bi-gear"></i>
            </span>

            <span>Settings</span>

        </a>

    </nav>

</asp:Content>


<asp:Content
    ID="SettingsUser"
    ContentPlaceHolderID="DashboardUser"
    runat="server">

    <div class="user-summary">

        <span class="user-avatar">

            <asp:Label
                ID="lblTopInitials"
                runat="server">
            </asp:Label>

        </span>


        <div class="user-details">

            <span class="user-name">

                <asp:Label
                    ID="lblTopName"
                    runat="server">
                </asp:Label>

            </span>


            <span class="user-role">

                <asp:Label
                    ID="lblTopRole"
                    runat="server">
                </asp:Label>

            </span>

        </div>

    </div>

</asp:Content>


<asp:Content
    ID="SettingsContent"
    ContentPlaceHolderID="DashboardContent"
    runat="server">

    <section class="shared-settings-page">


        <%-- SUCCESS MESSAGE --%>
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

                <asp:Label
                    ID="lblSuccessMessage"
                    runat="server">
                </asp:Label>

            </div>

        </asp:Panel>


        <%-- PAGE HEADER --%>
        <div class="shared-settings-header">

            <h2>
                Account Security &amp; Settings
            </h2>

            <p>
                Update password and manage your current account session
            </p>

        </div>


        <%-- ERROR MESSAGE --%>
        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="admin-form-message"
            Visible="false">
        </asp:Label>


        <%-- CHANGE PASSWORD CARD --%>
        <div class="settings-card">

            <h3>
                Change Password
            </h3>


            <div class="settings-form-group">

                <label for="<%= txtCurrentPassword.ClientID %>">
                    CURRENT PASSWORD *
                </label>

                <div class="settings-password-wrapper">

                    <asp:TextBox
                        ID="txtCurrentPassword"
                        runat="server"
                        CssClass="settings-password-input"
                        TextMode="Password">
                    </asp:TextBox>

                    <button
                        type="button"
                        class="settings-eye-button"
                        onclick="togglePassword('<%= txtCurrentPassword.ClientID %>', this)"
                        aria-label="Show or hide current password">

                        <i class="bi bi-eye"></i>

                    </button>

                </div>


                <asp:RequiredFieldValidator
                    ID="rfvCurrentPassword"
                    runat="server"
                    ControlToValidate="txtCurrentPassword"
                    ErrorMessage="Current password is required."
                    CssClass="validation-message"
                    Display="Dynamic">
                </asp:RequiredFieldValidator>

                <asp:Label
                    ID="lblCurrentPasswordError"
                    runat="server"
                    CssClass="validation-message"
                    Visible="false">
                </asp:Label>

            </div>


            <div class="settings-form-group">

                <label for="<%= txtNewPassword.ClientID %>">
                    NEW PASSWORD *
                </label>

                <div class="settings-password-wrapper">

                    <asp:TextBox
                        ID="txtNewPassword"
                        runat="server"
                        CssClass="settings-password-input"
                        TextMode="Password">
                    </asp:TextBox>

                    <button
                        type="button"
                        class="settings-eye-button"
                        onclick="togglePassword('<%= txtNewPassword.ClientID %>', this)"
                        aria-label="Show or hide new password">

                        <i class="bi bi-eye"></i>

                    </button>

                </div>


                <asp:RequiredFieldValidator
                    ID="rfvNewPassword"
                    runat="server"
                    ControlToValidate="txtNewPassword"
                    ErrorMessage="New password is required."
                    CssClass="validation-message"
                    Display="Dynamic">
                </asp:RequiredFieldValidator>


                <asp:RegularExpressionValidator
                    ID="revNewPassword"
                    runat="server"
                    ControlToValidate="txtNewPassword"
                    ValidationExpression="^.{8,}$"
                    ErrorMessage="Password must be at least 8 characters."
                    CssClass="validation-message"
                    Display="Dynamic">
                </asp:RegularExpressionValidator>

            </div>


            <div class="settings-form-group">

                <label for="<%= txtConfirmPassword.ClientID %>">
                    CONFIRM NEW PASSWORD *
                </label>

                <div class="settings-password-wrapper">

                    <asp:TextBox
                        ID="txtConfirmPassword"
                        runat="server"
                        CssClass="settings-password-input"
                        TextMode="Password">
                    </asp:TextBox>

                    <button
                        type="button"
                        class="settings-eye-button"
                        onclick="togglePassword('<%= txtConfirmPassword.ClientID %>', this)"
                        aria-label="Show or hide confirm password">

                        <i class="bi bi-eye"></i>

                    </button>

                </div>


                <asp:RequiredFieldValidator
                    ID="rfvConfirmPassword"
                    runat="server"
                    ControlToValidate="txtConfirmPassword"
                    ErrorMessage="Please confirm your new password."
                    CssClass="validation-message"
                    Display="Dynamic">
                </asp:RequiredFieldValidator>


                <asp:CompareValidator
                    ID="cvPassword"
                    runat="server"
                    ControlToValidate="txtConfirmPassword"
                    ControlToCompare="txtNewPassword"
                    ErrorMessage="Passwords do not match."
                    CssClass="validation-message"
                    Display="Dynamic">
                </asp:CompareValidator>

            </div>


            <div class="settings-action-row">

                <asp:Button
                    ID="btnUpdatePassword"
                    runat="server"
                    Text="Update Password"
                    CssClass="settings-primary-button"
                    OnClick="btnUpdatePassword_Click" />

            </div>

        </div>


        <%-- SIGN OUT CARD --%>
        <div class="settings-card settings-signout-card">

            <div class="settings-signout-content">

                <div>

                    <h3>
                        Account Sign Out
                    </h3>

                    <p>
                        Terminate your current active session on this device.
                    </p>

                </div>


                <asp:Button
                    ID="btnLogout"
                    runat="server"
                    Text="Log Out"
                    CssClass="settings-logout-button"
                    CausesValidation="false"
                    OnClick="btnLogout_Click" />

            </div>

        </div>

    </section>

</asp:Content>


<asp:Content
    ID="SettingsScripts"
    ContentPlaceHolderID="DashboardScripts"
    runat="server">

    <script src="<%= ResolveUrl("~/Scripts/Account.js") %>"></script>

</asp:Content>