<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/Master/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="ContentAudit.aspx.cs"
    Inherits="SPMaster.Admin.ContentAudit" %>


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


        <%-- PAGE HEADER --%>
        <div class="content-audit-header">

            <div>

                <h2>
                    Content Audit
                </h2>

                <p>
                    Review quiz questions and flagged content across the platform
                </p>

            </div>

        </div>


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


            <asp:DropDownList
                ID="ddlStatusFilter"
                runat="server"
                CssClass="filter-select"
                AutoPostBack="true"
                OnSelectedIndexChanged="FilterChanged">

                <asp:ListItem
                    Text="All"
                    Value="">
                </asp:ListItem>

                <asp:ListItem
                    Text="Approved"
                    Value="Approved">
                </asp:ListItem>

                <asp:ListItem
                    Text="Flagged"
                    Value="Flagged">
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


        <%-- SUMMARY CARDS --%>
        <div class="audit-summary-grid">


            <div class="audit-summary-card">

                <span class="audit-summary-label">
                    Total Questions
                </span>


                <span class="audit-summary-value">

                    <asp:Label
                        ID="lblTotalQuestions"
                        runat="server"
                        Text="0">
                    </asp:Label>

                </span>

            </div>


            <div class="audit-summary-card">

                <span class="audit-summary-label">
                    Flagged for Review
                </span>


                <span class="audit-summary-value flagged-number">

                    <asp:Label
                        ID="lblFlaggedQuestions"
                        runat="server"
                        Text="0">
                    </asp:Label>

                </span>

            </div>


            <div class="audit-summary-card">

                <span class="audit-summary-label">
                    Approved
                </span>


                <span class="audit-summary-value">

                    <asp:Label
                        ID="lblApprovedQuestions"
                        runat="server"
                        Text="0">
                    </asp:Label>

                </span>

            </div>

        </div>


        <%-- AUDIT TABLE --%>
        <div class="audit-table-card">

            <div class="audit-table-wrapper">

                <table class="audit-table">

                    <thead>

                        <tr>

                            <th>Question</th>
                            <th>Subject</th>
                            <th>Lecturer</th>
                            <th>Flag Reason</th>
                            <th>Status</th>
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

                                        <%# Eval("FlagReason") %>

                                    </td>


                                    <td>

                                        <span class='<%#
                                            Eval("AuditStatus").ToString() == "Flagged"
                                            ? "audit-status flagged-status"
                                            : "audit-status approved-status"
                                        %>'>

                                            <%# Eval("AuditStatus") %>

                                        </span>

                                    </td>


                                    <td>

                                        <%#
                                            Eval("AuditStatus").ToString() == "Flagged"
                                            ? "<a class='review-button' href='ReviewContent.aspx?id="
                                                + Eval("QuestionId")
                                                + "'>Review</a>"
                                            : "—"
                                        %>

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
                document.querySelector(
                    ".success-popup"
                );

            if (popup) {

                popup.style.display =
                    "none";

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
                    url.searchParams.has(
                        "reviewed"
                    )
                ) {

                    url.searchParams.delete(
                        "reviewed"
                    );


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

                            popup.style.opacity =
                                "0";


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