<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/Master/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="ReviewContent.aspx.cs"
    Inherits="SPMaster.Admin.ReviewContent" %>


<asp:Content
    ID="AdminTitle"
    ContentPlaceHolderID="DashboardTitle"
    runat="server">

    Review Content

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

    <section class="review-content-page">

        <div class="review-content-header">

            <div>

                <h2>
                    Review Flagged Content
                </h2>

                <p>
                    Review the reported question and resolve the issue.
                </p>

            </div>


            <asp:HyperLink
                ID="lnkBack"
                runat="server"
                NavigateUrl="~/Admin/ContentAudit.aspx"
                CssClass="back-user-button">

                ← Back to Content Audit

            </asp:HyperLink>

        </div>


        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="admin-form-message"
            Visible="false">
        </asp:Label>


        <div class="review-content-grid">


            <!-- QUESTION INFORMATION -->
            <div class="review-card">

                <h3>
                    Question Details
                </h3>


                <div class="review-detail-row">

                    <span class="review-label">
                        Question
                    </span>

                    <span class="review-value">

                        <asp:Label
                            ID="lblQuestion"
                            runat="server">
                        </asp:Label>

                    </span>

                </div>


                <div class="review-detail-row">

                    <span class="review-label">
                        Subject
                    </span>

                    <span class="review-value">

                        <asp:Label
                            ID="lblSubject"
                            runat="server">
                        </asp:Label>

                    </span>

                </div>


                <div class="review-detail-row">

                    <span class="review-label">
                        Lecturer
                    </span>

                    <span class="review-value">

                        <asp:Label
                            ID="lblLecturer"
                            runat="server">
                        </asp:Label>

                    </span>

                </div>


                <div class="review-detail-row">

                    <span class="review-label">
                        Question Type
                    </span>

                    <span class="review-value">

                        <asp:Label
                            ID="lblQuestionType"
                            runat="server">
                        </asp:Label>

                    </span>

                </div>


                <div class="review-detail-row">

                    <span class="review-label">
                        Difficulty
                    </span>

                    <span class="review-value">

                        <asp:Label
                            ID="lblDifficulty"
                            runat="server">
                        </asp:Label>

                    </span>

                </div>

            </div>


            <!-- REPORT INFORMATION -->
            <div class="review-card">

                <h3>
                    Report Details
                </h3>


                <div class="review-detail-row">

                    <span class="review-label">
                        Report Reason
                    </span>

                    <span class="review-value">

                        <asp:Label
                            ID="lblReason"
                            runat="server">
                        </asp:Label>

                    </span>

                </div>


                <div class="review-detail-row">

                    <span class="review-label">
                        Submitted By
                    </span>

                    <span class="review-value">

                        <asp:Label
                            ID="lblReportedBy"
                            runat="server">
                        </asp:Label>

                    </span>

                </div>


                <div class="review-detail-row">

                    <span class="review-label">
                        Reported At
                    </span>

                    <span class="review-value">

                        <asp:Label
                            ID="lblReportedAt"
                            runat="server">
                        </asp:Label>

                    </span>

                </div>


                <div class="review-detail-row">

                    <span class="review-label">
                        Status
                    </span>

                    <span class="audit-status flagged-status">

                        <asp:Label
                            ID="lblStatus"
                            runat="server">
                        </asp:Label>

                    </span>

                </div>

            </div>

        </div>


        <!-- ADMIN RESPONSE -->
        <div class="review-card review-response-card">

            <h3>
                Admin Review
            </h3>


            <div class="admin-form-group">

                <label for="<%= txtAdminReply.ClientID %>">
                    Admin Reply
                </label>

                <asp:TextBox
                    ID="txtAdminReply"
                    runat="server"
                    CssClass="review-textarea"
                    TextMode="MultiLine"
                    Rows="5"
                    MaxLength="1000"
                    placeholder="Enter a response or review note...">
                </asp:TextBox>

                <asp:RequiredFieldValidator
                    ID="rfvAdminReply"
                    runat="server"
                    ControlToValidate="txtAdminReply"
                    ErrorMessage="Please enter an admin reply."
                    CssClass="validation-message"
                    Display="Dynamic">
                </asp:RequiredFieldValidator>

            </div>


            <div class="admin-form-actions">

                <asp:HyperLink
                    ID="lnkCancel"
                    runat="server"
                    NavigateUrl="~/Admin/ContentAudit.aspx"
                    CssClass="cancel-user-button">

                    Cancel

                </asp:HyperLink>


                <asp:Button
                    ID="btnResolve"
                    runat="server"
                    Text="Resolve Report"
                    CssClass="create-user-button"
                    OnClick="btnResolve_Click" />

            </div>

        </div>

    </section>

</asp:Content>