<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/Master/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="RespondFeedback.aspx.cs"
    Inherits="SPMaster.Admin.RespondFeedback" %>


<asp:Content
    ID="AdminTitle"
    ContentPlaceHolderID="DashboardTitle"
    runat="server">

    Respond to Feedback

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

    <section class="respond-feedback-page">


        <%-- HEADER --%>
        <div class="respond-feedback-header">

            <div>

                <h2>
                    Respond to Feedback
                </h2>

                <p>
                    Review the submission and provide an administrative response.
                </p>

            </div>


            <asp:HyperLink
                ID="lnkBack"
                runat="server"
                NavigateUrl="~/Admin/FeedbackReports.aspx"
                CssClass="back-user-button">

                ← Back to Feedback

            </asp:HyperLink>

        </div>


        <%-- ERROR MESSAGE --%>
        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="admin-form-message"
            Visible="false">
        </asp:Label>


        <%-- FEEDBACK INFORMATION --%>
        <div class="respond-feedback-card">

            <div class="respond-card-heading">

                <div>

                    <h3>
                        Feedback Details
                    </h3>

                    <p>
                        Information submitted by the user
                    </p>

                </div>


                <span class="respond-status">

                    <asp:Label
                        ID="lblStatus"
                        runat="server">
                    </asp:Label>

                </span>

            </div>


            <div class="respond-detail-grid">


                <div class="respond-detail-item">

                    <span class="respond-detail-label">
                        Submitted By
                    </span>

                    <strong>

                        <asp:Label
                            ID="lblSubmittedBy"
                            runat="server">
                        </asp:Label>

                    </strong>

                </div>


                <div class="respond-detail-item">

                    <span class="respond-detail-label">
                        Role
                    </span>

                    <strong>

                        <asp:Label
                            ID="lblRole"
                            runat="server">
                        </asp:Label>

                    </strong>

                </div>


                <div class="respond-detail-item">

                    <span class="respond-detail-label">
                        Submitted At
                    </span>

                    <strong>

                        <asp:Label
                            ID="lblCreatedAt"
                            runat="server">
                        </asp:Label>

                    </strong>

                </div>


                <div class="respond-detail-item">

                    <span class="respond-detail-label">
                        Related Quiz
                    </span>

                    <strong>

                        <asp:Label
                            ID="lblQuiz"
                            runat="server">
                        </asp:Label>

                    </strong>

                </div>

            </div>


            <div class="respond-message-box">

                <span class="respond-detail-label">
                    Feedback Message
                </span>

                <p>

                    <asp:Label
                        ID="lblFeedbackMessage"
                        runat="server">
                    </asp:Label>

                </p>

            </div>

        </div>


        <%-- EXISTING ADMIN RESPONSE --%>
        <asp:Panel
            ID="pnlExistingReply"
            runat="server"
            CssClass="respond-feedback-card existing-reply-card"
            Visible="false">

            <h3>
                Existing Admin Response
            </h3>


            <div class="existing-reply-content">

                <asp:Label
                    ID="lblExistingReply"
                    runat="server">
                </asp:Label>

            </div>


            <div class="existing-reply-meta">

                Responded by

                <strong>
                    <asp:Label
                        ID="lblRepliedBy"
                        runat="server">
                    </asp:Label>
                </strong>

                on

                <asp:Label
                    ID="lblRepliedAt"
                    runat="server">
                </asp:Label>

            </div>

        </asp:Panel>


        <%-- ADMIN RESPONSE FORM --%>
        <div class="respond-feedback-card">

            <h3>
                Admin Response
            </h3>


            <div class="admin-form-group">

                <label for="<%= ddlStatus.ClientID %>">
                    Status
                </label>


                <asp:DropDownList
                    ID="ddlStatus"
                    runat="server"
                    CssClass="respond-status-dropdown">

                    <asp:ListItem
                        Text="Open"
                        Value="Open">
                    </asp:ListItem>

                    <asp:ListItem
                        Text="Under Review"
                        Value="Under Review">
                    </asp:ListItem>

                    <asp:ListItem
                        Text="Resolved"
                        Value="Resolved">
                    </asp:ListItem>

                </asp:DropDownList>

            </div>


            <div class="admin-form-group">

                <label for="<%= txtAdminReply.ClientID %>">
                    Response
                </label>


                <asp:TextBox
                    ID="txtAdminReply"
                    runat="server"
                    CssClass="respond-feedback-textarea"
                    TextMode="MultiLine"
                    Rows="6"
                    MaxLength="2000"
                    placeholder="Enter your response to the feedback...">
                </asp:TextBox>


                <asp:RequiredFieldValidator
                    ID="rfvAdminReply"
                    runat="server"
                    ControlToValidate="txtAdminReply"
                    ErrorMessage="Please enter a response."
                    CssClass="validation-message"
                    Display="Dynamic">
                </asp:RequiredFieldValidator>

            </div>


            <div class="admin-form-actions">


                <asp:HyperLink
                    ID="lnkCancel"
                    runat="server"
                    NavigateUrl="~/Admin/FeedbackReports.aspx"
                    CssClass="cancel-user-button">

                    Cancel

                </asp:HyperLink>


                <asp:Button
                    ID="btnSubmitResponse"
                    runat="server"
                    Text="Submit Response"
                    CssClass="create-user-button"
                    OnClick="btnSubmitResponse_Click" />

            </div>

        </div>


    </section>

</asp:Content>