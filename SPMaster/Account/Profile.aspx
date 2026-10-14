<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/Master/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="Profile.aspx.cs"
    Inherits="SPMaster.Account.Profile" %>


<asp:Content
    ID="ProfileTitle"
    ContentPlaceHolderID="DashboardTitle"
    runat="server">

    Profile

</asp:Content>


<asp:Content
    ID="ProfileNavigation"
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
    ID="ProfileAccountNavigation"
    ContentPlaceHolderID="DashboardAccountNavigation"
    runat="server">

    <nav aria-label="Account navigation">

        <a href="<%= ResolveUrl("~/Account/Profile.aspx") %>"
           class="nav-item active"
           aria-current="page">

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
    ID="ProfileUser"
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
    ID="ProfileContent"
    ContentPlaceHolderID="DashboardContent"
    runat="server">

    <section class="shared-profile-page">


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


        <%-- HEADER --%>
        <div class="shared-profile-header">

            <h2>
                <asp:Label
                    ID="lblProfileHeading"
                    runat="server">
                </asp:Label>
            </h2>

            <p>
                Manage your personal account information and profile details
            </p>

        </div>


        <%-- ERROR MESSAGE --%>
        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="admin-form-message"
            Visible="false">
        </asp:Label>


        <%-- PROFILE CARD --%>
        <div class="shared-profile-card">


            <%-- AVATAR SECTION --%>
            <div class="profile-avatar-section">


                <div class="profile-avatar-large">


                    <asp:Image
                        ID="imgProfile"
                        runat="server"
                        CssClass="profile-avatar-image"
                        Visible="false"
                        AlternateText="Profile photo" />


                    <asp:Label
                        ID="lblProfileInitials"
                        runat="server"
                        CssClass="profile-avatar-initials">
                    </asp:Label>

                </div>


                <div class="profile-avatar-info">

                    <strong>
                        Profile Avatar
                    </strong>


                    <span class="profile-avatar-description">

                        <asp:Label
                            ID="lblProfileDescription"
                            runat="server">
                        </asp:Label>

                    </span>


                    <div class="profile-avatar-actions">


                        <label
                            for="<%= fuProfilePhoto.ClientID %>"
                            class="profile-photo-button">

                            Change Photo

                        </label>


                        <asp:FileUpload
                            ID="fuProfilePhoto"
                            runat="server"
                            CssClass="profile-file-input"
                            accept=".jpg,.jpeg,.png,.webp" />


                        <asp:Button
                            ID="btnResetPhoto"
                            runat="server"
                            Text="Reset"
                            CssClass="profile-reset-button"
                            CausesValidation="false"
                            OnClick="btnResetPhoto_Click" />

                    </div>

                </div>

            </div>


            <%-- FORM --%>
            <div class="profile-form-grid">


                <%-- FULL NAME --%>
                <div class="profile-form-group">

                    <label for="<%= txtFullName.ClientID %>">
                        FULL NAME
                    </label>


                    <asp:TextBox
                        ID="txtFullName"
                        runat="server"
                        CssClass="profile-form-input"
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


                <%-- EMAIL --%>
                <div class="profile-form-group">

                    <label for="<%= txtEmail.ClientID %>">
                        EMAIL ADDRESS
                    </label>


                    <asp:TextBox
                        ID="txtEmail"
                        runat="server"
                        CssClass="profile-form-input"
                        MaxLength="254"
                        TextMode="Email">
                    </asp:TextBox>


                    <asp:RequiredFieldValidator
                        ID="rfvEmail"
                        runat="server"
                        ControlToValidate="txtEmail"
                        ErrorMessage="Email address is required."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>


                    <asp:RegularExpressionValidator
                        ID="revEmail"
                        runat="server"
                        ControlToValidate="txtEmail"
                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                        ErrorMessage="Please enter a valid email address."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RegularExpressionValidator>

                </div>


                <%-- SCHOOL --%>
                <div class="profile-form-group">

                    <label for="<%= txtSchool.ClientID %>">
                        SCHOOL / INSTITUTION
                    </label>


                    <asp:TextBox
                        ID="txtSchool"
                        runat="server"
                        CssClass="profile-form-input"
                        MaxLength="150">
                    </asp:TextBox>


                    <asp:RequiredFieldValidator
                        ID="rfvSchool"
                        runat="server"
                        ControlToValidate="txtSchool"
                        ErrorMessage="School or institution is required."
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>

                </div>


                <%-- ROLE --%>
                <div class="profile-form-group">

                    <label>
                        INSTITUTIONAL ROLE
                    </label>


                    <div class="profile-role-field">

                        <asp:Label
                            ID="lblInstitutionalRole"
                            runat="server">
                        </asp:Label>


                        <i class="bi bi-patch-check"></i>

                    </div>

                </div>

            </div>


            <div class="profile-card-divider">
            </div>


            <%-- SAVE --%>
            <div class="profile-save-row">

                <asp:Button
                    ID="btnSaveProfile"
                    runat="server"
                    Text="Save Profile Changes"
                    CssClass="profile-save-button"
                    OnClick="btnSaveProfile_Click" />

            </div>

        </div>

    </section>

</asp:Content>


<asp:Content
    ID="ProfileScripts"
    ContentPlaceHolderID="DashboardScripts"
    runat="server">

    <script src="<%= ResolveUrl("~/Scripts/Account.js") %>"></script>

</asp:Content>