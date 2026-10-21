<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/Master/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="ContentAudit.aspx.cs"
    Inherits="SPMaster.Admin.ContentAudit" %>

<asp:Content
    ID="ContentAuditHead"
    ContentPlaceHolderID="DashboardHead"
    runat="server">

    <link
        href="<%= ResolveUrl("~/Content/ContentAudit.css") %>"
        rel="stylesheet" />

</asp:Content>

<asp:Content
    ID="AdminTitle"
    ContentPlaceHolderID="DashboardTitle"
    runat="server">

    Content Audit

</asp:Content>


<asp:Content
    ID="AdminNavigation"
    ContentPlaceHolderID="DashboardNavigation"
    runat="server">

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
           class="nav-item active"
           aria-current="page">

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

    <section class="content-audit-page">


        <%-- SUCCESS POPUP --%>
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

                <button
                    type="button"
                    class="success-close"
                    onclick="closeSuccessPopup()">

                    ×

                </button>

            </div>

        </asp:Panel>


               <%-- PAGE DESCRIPTION --%>
        <p class="content-audit-description">
            View and manage questions currently used in published quizzes
        </p>


        <%-- FILTER BAR --%>
        <div class="audit-filter-bar">

            <asp:DropDownList
                ID="ddlSubjectFilter"
                runat="server"
                CssClass="filter-select"
                AutoPostBack="true"
                OnSelectedIndexChanged="FilterChanged">

                <asp:ListItem
                    Text="All Subjects"
                    Value="">
                </asp:ListItem>

            </asp:DropDownList>


            <asp:TextBox
                ID="txtSearch"
                runat="server"
                CssClass="user-search"
                placeholder="Search questions..."
                AutoPostBack="true"
                OnTextChanged="SearchChanged">
            </asp:TextBox>

        </div>


        <%-- SUMMARY --%>
        <div class="audit-summary-grid audit-summary-single">

            <div class="audit-summary-card">

                <span class="audit-summary-label">
                    Published Questions
                </span>

                <span class="audit-summary-value">

                    <asp:Label
                        ID="lblPublishedQuestions"
                        runat="server"
                        Text="0">
                    </asp:Label>

                </span>

            </div>

        </div>


        <%-- TABLE --%>
        <div class="audit-table-card">

            <div class="audit-table-wrapper">

                <table class="audit-table">

                    <thead>

                        <tr>

                            <th>Question</th>
                            <th>Subject</th>
                            <th>Lecturer</th>
                            <th>Published In</th>
                            <th>Created Date</th>
                            <th>Actions</th>

                        </tr>

                    </thead>


                    <tbody>

                        <asp:Repeater
                            ID="rptAudit"
                            runat="server">

                            <ItemTemplate>

                                <tr>

                                    <td class="audit-question-cell">

                                        <%# Eval("QuestionText") %>

                                    </td>


                                    <td>

                                        <%# Eval("SubjectName") %>

                                    </td>


                                    <td>

                                        <%# Eval("LecturerName") %>

                                    </td>


                                    <td>

                                        <%# Eval("PublishedQuizCount") %>
                                        <%#
                                            Convert.ToInt32(
                                                Eval("PublishedQuizCount")
                                            ) == 1
                                            ? " quiz"
                                            : " quizzes"
                                        %>

                                    </td>


                                    <td>

                                        <%#
                                            Convert.ToDateTime(
                                                Eval("CreatedAt")
                                            ).ToString(
                                                "dd MMM yyyy"
                                            )
                                        %>

                                    </td>


                                    <td>

                                        <div class="audit-action-buttons">


                                            <%-- VIEW --%>
                                            <a
                                                href='<%#
                                                    "ViewContent.aspx?id="
                                                    + Eval("QuestionId")
                                                %>'
                                                class="audit-view-button">

                                                <i class="bi bi-eye"></i>

                                                View

                                            </a>


                                            <%-- UNPUBLISH --%>
                                            <asp:LinkButton
                                                ID="btnUnpublish"
                                                runat="server"
                                                CssClass="audit-unpublish-button"
                                                CommandArgument='<%#
                                                    Eval("QuestionId")
                                                %>'
                                                OnCommand="Unpublish_Command"
                                                CausesValidation="false"
                                                OnClientClick="return confirm('Are you sure you want to unpublish this question from all published quizzes?');">

                                                <i class="bi bi-eye-slash"></i>

                                                Unpublish

                                            </asp:LinkButton>


                                        </div>

                                    </td>

                                </tr>

                            </ItemTemplate>

                        </asp:Repeater>

                    </tbody>

                </table>


                <%-- EMPTY STATE --%>
                <asp:Panel
                    ID="pnlNoQuestions"
                    runat="server"
                    CssClass="audit-empty-state"
                    Visible="false">

                    <i class="bi bi-file-earmark-x"></i>

                    <p>
                        No published questions found.
                    </p>

                </asp:Panel>

            </div>

        </div>

    </section>

</asp:Content>


<asp:Content
    ID="AdminScripts"
    ContentPlaceHolderID="DashboardScripts"
    runat="server">

    <script src="<%= ResolveUrl("~/Scripts/ContentAudit.js") %>"></script>

</asp:Content>