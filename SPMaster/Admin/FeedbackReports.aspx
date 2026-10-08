<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/Master/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="FeedbackReports.aspx.cs"
    Inherits="SPMaster.Admin.FeedbackReports" %>


<asp:Content
    ID="AdminTitle"
    ContentPlaceHolderID="DashboardTitle"
    runat="server">

    Feedback &amp; Reports

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

    <section class="feedback-page">


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


        <%-- HEADER --%>
        <div class="feedback-header">

            <h2>
                Feedback &amp; Reports
            </h2>

            <p>
                System-wide feedback review and response management
            </p>

        </div>


        <%-- TABS --%>
        <div class="feedback-tabs">

            <asp:HyperLink
                ID="lnkStudentTab"
                runat="server"
                NavigateUrl="~/Admin/FeedbackReports.aspx?view=student"
                CssClass="feedback-tab">

                User Feedback

            </asp:HyperLink>


            <asp:HyperLink
                ID="lnkLecturerTab"
                runat="server"
                NavigateUrl="~/Admin/FeedbackReports.aspx?view=lecturer"
                CssClass="feedback-tab">

                Lecturer Reports

            </asp:HyperLink>

        </div>


        <%-- SECTION HEADING --%>
        <div class="feedback-section-title">

            <h3>
                Recent feedback
            </h3>

            <span>

                <asp:Label
                    ID="lblSubmissionCount"
                    runat="server"
                    Text="0">
                </asp:Label>

                submissions

            </span>

        </div>


        <%-- EMPTY STATE --%>
        <asp:Panel
            ID="pnlEmpty"
            runat="server"
            CssClass="feedback-empty"
            Visible="false">

            <i class="bi bi-chat-left-text"></i>

            <p>
                No feedback found.
            </p>

        </asp:Panel>


        <%-- FEEDBACK LIST --%>
        <div class="feedback-list">

            <asp:Repeater
                ID="rptFeedback"
                runat="server">

                <ItemTemplate>

                    <div class="feedback-card">


                        <div class="feedback-main">


                            <%-- NAME + BADGES --%>
                            <div class="feedback-user-line">

                                <strong>
                                    <%# Eval("SubmittedByName") %>
                                </strong>


                                <span class='<%#
                                    Eval("RoleName").ToString() == "Lecturer"
                                    ? "feedback-role lecturer-role"
                                    : "feedback-role student-role"
                                %>'>

                                    <%# Eval("RoleName") %>

                                </span>


                                <span class="feedback-type">

                                    <%#
                                        Eval("QuizId") == DBNull.Value
                                        ? "General Feedback"
                                        : "Quiz Feedback"
                                    %>

                                </span>

                            </div>


                            <%-- QUIZ --%>
                            <asp:Panel
                                runat="server"
                                Visible='<%# Eval("QuizId") != DBNull.Value %>'>

                                <div class="feedback-quiz">

                                    <i class="bi bi-journal-text"></i>

                                    <%# Eval("QuizTitle") %>

                                </div>

                            </asp:Panel>


                            <%-- MESSAGE --%>
                            <div class="feedback-message">

                                <%# Eval("Message") %>

                            </div>


                            <%-- DATE --%>
                            <div class="feedback-date">

                                <%# Eval("CreatedDisplay") %>

                            </div>

                        </div>


                        <div class="feedback-actions">


                            <asp:HiddenField
                                ID="hfFeedbackId"
                                runat="server"
                                Value='<%# Eval("FeedbackId") %>' />


                            <%-- STATUS --%>
                            <asp:DropDownList
                                ID="ddlFeedbackStatus"
                                runat="server"
                                CssClass="feedback-status-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="FeedbackStatusChanged">

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


                            <asp:HyperLink
                                ID="lnkRespond"
                                runat="server"
                                CssClass="feedback-respond-button"
                                NavigateUrl='<%#
                                    "~/Admin/RespondFeedback.aspx?id="
                                    + Eval("FeedbackId")
                                %>'>

                                Respond

                            </asp:HyperLink>

                        </div>


                    </div>

                </ItemTemplate>

            </asp:Repeater>

        </div>


        <%-- STATISTICS --%>
        <div class="feedback-stat-card">

            <h3>
                Response Performance
            </h3>


            <div class="feedback-stat-grid">


                <div class="feedback-stat">

                    <span class="feedback-stat-label">
                        Average response time
                    </span>

                    <strong>

                        <asp:Label
                            ID="lblAverageResponseTime"
                            runat="server"
                            Text="—">
                        </asp:Label>

                    </strong>

                </div>


                <div class="feedback-stat">

                    <span class="feedback-stat-label">
                        Resolved rate
                    </span>

                    <strong>

                        <asp:Label
                            ID="lblResolvedRate"
                            runat="server"
                            Text="0%">
                        </asp:Label>

                    </strong>

                </div>


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
        document.querySelector(
            ".success-popup"
        );

    if (popup) {
        popup.style.display = "none";
    }

}


window.addEventListener(
    "load",
    function () {

        const popup =
            document.querySelector(
                ".success-popup"
            );

        const url =
            new URL(
                window.location.href
            );


        if (
            url.searchParams.has("responded") ||
            url.searchParams.has("statusupdated")
        ) {

            url.searchParams.delete("responded");
            url.searchParams.delete("statusupdated");


            window.history.replaceState(
                {},
                document.title,
                url.pathname +
                url.search +
                url.hash
            );

        }


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