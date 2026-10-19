<%@ Page Title="Lecturer Portal" Language="C#" MasterPageFile="~/Master/Portal.Master" AutoEventWireup="true" CodeBehind="Portal.aspx.cs" Inherits="SPMaster.Lecturer.Portal" %>

<asp:Content ID="LecturerPortalHead" ContentPlaceHolderID="PortalHead" runat="server">
    <link href="<%= ResolveUrl("~/Content/LecturerPortal.css") %>?v=1" rel="stylesheet" type="text/css" />
</asp:Content>


<asp:Content ID="LecturerPortalContent" ContentPlaceHolderID="PortalContent" runat="server">

    <div class="lecturer-dashboard">

        <asp:Panel
            ID="pnlGradingAlert"
            runat="server"
            CssClass="grading-alert">


            <div class="grading-alert-content">

                <h1>

                    <asp:Literal
                        ID="litGradingAlertCount"
                        runat="server">
                    </asp:Literal>

                </h1>


                <p>

                    <asp:Literal
                        ID="litGradingAlertDescription"
                        runat="server">
                    </asp:Literal>

                </p>

            </div>


            <a id="lnkGradeSubmissions"
               runat="server"
               href="<%= ResolveUrl("~/Lecturer/Grading.aspx") %>"
               class="grading-alert-button">

                Grade Submissions Now

                <span aria-hidden="true">
                    →
                </span>

            </a>


        </asp:Panel>


        <%-- =========================================
             DASHBOARD SUMMARY
             ========================================= --%>

        <section class="lecturer-stat-grid">


            <%-- =================================================
                 QUESTION BANK
                 ================================================= --%>

            <a href="<%= ResolveUrl("~/Lecturer/QuestionBank.aspx") %>"
               class="lecturer-stat-card">

                <div class="lecturer-stat-icon icon-purple">

                    <i class="bi bi-card-text"
                       aria-hidden="true"></i>

                </div>


                <div class="lecturer-stat-content">

                    <span class="lecturer-stat-heading">
                        My Question Bank
                    </span>


                    <strong>

                        <asp:Literal
                            ID="litQuestionBankTotal"
                            runat="server">
                        </asp:Literal>

                    </strong>


                    <span class="lecturer-stat-detail">

                        <asp:Literal
                            ID="litQuestionBankBreakdown"
                            runat="server">
                        </asp:Literal>

                    </span>

                </div>

            </a>



            <%-- =================================================
                 PUBLISHED QUIZZES
                 ================================================= --%>

            <a href="<%= ResolveUrl("~/Lecturer/Quizzes.aspx") %>"
               class="lecturer-stat-card">

                <div class="lecturer-stat-icon icon-purple">

                    <i class="bi bi-check2-circle"
                       aria-hidden="true"></i>

                </div>


                <div class="lecturer-stat-content">

                    <span class="lecturer-stat-heading">
                        Published Quizzes
                    </span>


                    <strong>

                        <asp:Literal
                            ID="litPublishedQuizCount"
                            runat="server">
                        </asp:Literal>

                    </strong>


                    <span class="lecturer-stat-detail">

                        <asp:Literal
                            ID="litDraftQuizCount"
                            runat="server">
                        </asp:Literal>

                    </span>

                </div>

            </a>



            <%-- =================================================
                 LIVE SESSIONS
                 KEEP HARD-CODED FOR NOW
                 ================================================= --%>

            <a href="<%= ResolveUrl("~/Lecturer/Sessions.aspx") %>"
               class="lecturer-stat-card">

                <div class="lecturer-stat-icon icon-purple">

                    <i class="bi bi-people-fill"
                       aria-hidden="true"></i>

                </div>


                <div class="lecturer-stat-content">

                    <span class="lecturer-stat-heading">
                        Live Sessions
                    </span>


                    <strong>
                        3 Active
                    </strong>


                    <span class="lecturer-stat-detail">
                        148 Total attempts
                    </span>

                </div>

            </a>



            <%-- =================================================
                 GRADING QUEUE
                 ================================================= --%>

            <a href="<%= ResolveUrl("~/Lecturer/Grading.aspx") %>"
               class="lecturer-stat-card">

                <div class="lecturer-stat-icon icon-red">

                    <i class="bi bi-pen"
                       aria-hidden="true"></i>

                </div>


                <div class="lecturer-stat-content">

                    <span class="lecturer-stat-heading">
                        Grading Queue
                    </span>


                    <strong class="pending-text">

                        <asp:Literal
                            ID="litGradingQueueCount"
                            runat="server">
                        </asp:Literal>

                    </strong>


                    <span class="lecturer-stat-detail">
                        Requires review
                    </span>

                </div>

            </a>


</section>


        <%-- =========================================
             MAIN DASHBOARD GRID
             ========================================= --%>

        <div class="lecturer-dashboard-grid">

            <%-- =====================================
                 LEFT COLUMN
                 ===================================== --%>

            <div class="lecturer-main-column">


                <%-- RECENT QUIZZES --%>

                <section class="dashboard-card recent-quizzes-card">

                    <div class="dashboard-card-header">

                        <h2>

                            My Recent Quizzes

                        </h2>

                        <a href="<%= ResolveUrl("~/Lecturer/Quizzes.aspx") %>"
                           class="card-header-link">

                            View All Quizzes →

                        </a>

                    </div>


                    <div class="recent-quiz-list">


                        <asp:Repeater
                            ID="rptRecentQuizzes"
                            runat="server">

                            <ItemTemplate>


                                <article class="recent-quiz-row">


                                    <div class="recent-quiz-main">


                                        <%-- =====================================
                                             TAGS
                                             ===================================== --%>

                                        <div class="quiz-tags">


                                            <span class='<%# GetSubjectTagCss(
                                                Eval("SubjectName")
                                            ) %>'>

                                                <%#: Eval("SubjectName") %>

                                            </span>


                                            <span class="quiz-tag">

                                                <%#: Eval("FormName") %>

                                            </span>


                                            <span class='<%# GetQuizStatusCss(
                                                Eval("IsPublished")
                                            ) %>'>

                                                <%#: GetQuizStatusText(
                                                    Eval("IsPublished")
                                                ) %>

                                            </span>


                                        </div>



                                        <%-- =====================================
                                             TITLE
                                             ===================================== --%>

                                        <h3>

                                            <%#: Eval("Title") %>

                                        </h3>



                                        <%-- =====================================
                                             DETAILS
                                             ===================================== --%>

                                        <p>

                                            <%#: GetQuizSummary(
                                                Eval("QuestionCount"),
                                                Eval("TotalMarks"),
                                                Eval("TimeLimitMinutes"),
                                                Eval("AttemptCount"),
                                                Eval("PendingCount"),
                                                Eval("IsPublished")
                                            ) %>

                                        </p>


                                    </div>



                                    <%-- =====================================
                                         ACTIONS
                                         ===================================== --%>

                                    <div class="recent-quiz-actions">


                                        <a href='<%# GetPrimaryQuizActionUrl(
                                                Eval("QuizId"),
                                                Eval("IsPublished")
                                            ) %>'
                                           class='<%# GetPrimaryQuizActionCss(
                                                Eval("IsPublished")
                                            ) %>'>

                                            <%#: GetPrimaryQuizActionText(
                                                Eval("IsPublished")
                                            ) %>

                                        </a>


                                        <a href='<%# GetPreviewQuizUrl(
                                                Eval("QuizId")
                                            ) %>'
                                           class="small-button small-button-light">

                                            Preview

                                        </a>


                                    </div>


                                </article>


                            </ItemTemplate>

                        </asp:Repeater>



                        <asp:Panel
                            ID="pnlNoRecentQuizzes"
                            runat="server"
                            Visible="false"
                            CssClass="dashboard-empty-message">

                            No quizzes created yet.

                        </asp:Panel>


                    </div>

                </section>


                <%-- ACTIVE CLASSROOM SESSIONS --%>

                <section class="dashboard-card classroom-card">

                    <div class="dashboard-card-header">

                        <h2>

                            Active Classroom Sessions

                        </h2>

                        <a href="<%= ResolveUrl("~/Lecturer/Sessions.aspx") %>"
                           class="card-header-link">

                            Manage All Sessions →

                        </a>

                    </div>


                    <div class="session-table-wrapper">

                        <table class="session-table">

                            <thead>

                                <tr>

                                    <th>
                                        Session Title
                                    </th>

                                    <th>
                                        Code
                                    </th>

                                    <th>
                                        Status
                                    </th>

                                    <th>
                                        Attempts
                                    </th>

                                    <th>
                                        Actions
                                    </th>

                                </tr>

                            </thead>


                            <tbody>

                                <tr>

                                    <td>

                                        <strong>
                                            Maths Form 5 Intensive Rev
                                        </strong>

                                        <span>
                                            Maths Final-Year Practice
                                        </span>

                                    </td>

                                    <td>

                                        <span class="session-code">
                                            MATH-582
                                        </span>

                                    </td>

                                    <td>

                                        <span class="session-status">
                                            Open
                                        </span>

                                    </td>

                                    <td>
                                        42 attempts
                                    </td>

                                    <td>

                                        <a href="<%= ResolveUrl("~/Lecturer/Sessions.aspx") %>"
                                           class="table-action-link">

                                            Details

                                        </a>

                                    </td>

                                </tr>


                                <tr>

                                    <td>

                                        <strong>
                                            Sejarah SPM Trial Prep
                                        </strong>

                                        <span>
                                            Sejarah Kertas 2:
                                            Kedaulatan
                                        </span>

                                    </td>

                                    <td>

                                        <span class="session-code">
                                            SEJ-481
                                        </span>

                                    </td>

                                    <td>

                                        <span class="session-status">
                                            Open
                                        </span>

                                    </td>

                                    <td>

                                        28

                                        <span class="pending-attempts">
                                            (4 pending)
                                        </span>

                                    </td>

                                    <td>

                                        <a href="<%= ResolveUrl("~/Lecturer/Sessions.aspx") %>"
                                           class="table-action-link">

                                            Details

                                        </a>

                                    </td>

                                </tr>

                            </tbody>

                        </table>

                    </div>

                </section>

            </div>


            <%-- =====================================
                 RIGHT COLUMN
                 ===================================== --%>

            <aside class="lecturer-side-column">


                <%-- GRADING QUEUE --%>

                <section class="dashboard-card grading-queue-card">


                    <div class="side-card-heading">


                        <div>

                            <h2>
                                Grading Queue
                            </h2>


                            <p>
                                Pending subjective evaluations
                            </p>

                        </div>


                        <span class="queue-count">

                            <asp:Literal
                                ID="litSideQueueCount"
                                runat="server">
                            </asp:Literal>

                        </span>


                    </div>



                    <div class="grading-student-list">


                        <asp:Repeater
                            ID="rptGradingQueue"
                            runat="server">

                            <ItemTemplate>


                                <div class="grading-student">


                                    <div>

                                        <strong>

                                            <%#: Eval("StudentName") %>

                                        </strong>


                                        <span>

                                            <%#: GetQueueSubtitle(
                                                Eval("QuizTitle"),
                                                Eval("SubmittedAt")
                                            ) %>

                                        </span>

                                    </div>


                                    <a href='<%# GetGradingUrl(
                                            Eval("AttemptId")
                                        ) %>'
                                       class="grade-button">

                                        Grade

                                    </a>


                                </div>


                            </ItemTemplate>

                        </asp:Repeater>



                        <asp:Panel
                            ID="pnlNoGradingQueue"
                            runat="server"
                            Visible="false"
                            CssClass="dashboard-empty-message">

                            No submissions require grading.

                        </asp:Panel>


                    </div>



                    <a href="<%= ResolveUrl("~/Lecturer/Grading.aspx") %>"
                       class="side-card-button">

                        See All Submissions →

                    </a>


                </section>


                <%-- COHORT PASS RATE --%>

                <section class="dashboard-card performance-summary-card">


                    <div class="side-card-heading">


                        <div>

                            <h2>
                                Cohort Performance
                            </h2>


                            <p>

                                <asp:Literal
                                    ID="litCohortAttemptDescription"
                                    runat="server">
                                </asp:Literal>

                            </p>

                        </div>


                    </div>



                    <div class="pass-rate-wrapper">


                        <div
                            ID="passRateChart"
                            runat="server"
                            class="pass-rate-chart">


                            <div class="pass-rate-chart-center">


                                <strong>

                                    <asp:Literal
                                        ID="litCohortAverage"
                                        runat="server">
                                    </asp:Literal>

                                </strong>


                                <span>
                                    Avg Score
                                </span>


                            </div>


                        </div>


                    </div>



                    <div class="performance-detail-row">

                        <span>
                            Top Performing Subject:
                        </span>


                        <strong class="performance-pill performance-good">

                            <asp:Literal
                                ID="litTopSubject"
                                runat="server">
                            </asp:Literal>

                        </strong>

                    </div>



                    <div class="performance-detail-row">

                        <span>
                            Needs Attention:
                        </span>


                        <strong class="performance-pill performance-warning">

                            <asp:Literal
                                ID="litNeedsAttention"
                                runat="server">
                            </asp:Literal>

                        </strong>

                    </div>



                    <a href="<%= ResolveUrl("~/Lecturer/Performance.aspx") %>"
                       class="side-card-button">

                        View Cohort Analytics →

                    </a>


                </section>

            </aside>

        </div>

    </div>

</asp:Content>


<asp:Content
    ID="LecturerPortalScripts"
    ContentPlaceHolderID="PortalScripts"
    runat="server">

</asp:Content>