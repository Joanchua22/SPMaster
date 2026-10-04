<%@ Page Title="Lecturer Portal" Language="C#" MasterPageFile="~/Master/Portal.Master" AutoEventWireup="true" CodeBehind="Portal.aspx.cs" Inherits="SPMaster.Lecturer.Portal" %>

<asp:Content ID="LecturerPortalHead" ContentPlaceHolderID="PortalHead" runat="server">
    <link href="<%= ResolveUrl("~/Content/LecturerPortal.css") %>?v=1" rel="stylesheet" type="text/css" />
</asp:Content>


<asp:Content ID="LecturerPortalContent" ContentPlaceHolderID="PortalContent" runat="server">

    <div class="lecturer-dashboard">

        <section class="grading-alert">

            <div class="grading-alert-content">

                <h1>
                    4 Subjective Submissions Awaiting Grading
                </h1>

                <p>
                    Student attempts in
                    <strong>"Sejarah Kertas 2: Kedaulatan Negara"</strong>
                    and
                    <strong>"Mathematics Mid-Year"</strong>
                    are waiting for your subjective mark allocation.
                </p>

            </div>

            <a href="<%= ResolveUrl("~/Lecturer/Grading.aspx") %>"
               class="grading-alert-button">

                Grade Submissions Now

                <span aria-hidden="true">
                    →
                </span>

            </a>

        </section>


        <%-- =========================================
             DASHBOARD SUMMARY
             ========================================= --%>

        <section class="lecturer-stat-grid">

            <%-- QUESTION BANK --%>

            <a href="<%= ResolveUrl("~/Lecturer/QuestionBank.aspx") %>"
               class="lecturer-stat-card">

                <div class="lecturer-stat-icon icon-purple">

                    <svg viewBox="0 0 24 24"
                         aria-hidden="true">

                        <rect x="5"
                              y="4"
                              width="14"
                              height="16"
                              rx="2"></rect>

                        <path d="M8 8h8"></path>
                        <path d="M8 11h8"></path>
                        <path d="M8 14h5"></path>

                    </svg>

                </div>

                <div class="lecturer-stat-content">

                    <span class="lecturer-stat-heading">
                        My Question Bank
                    </span>

                    <strong>
                        42 Items
                    </strong>

                    <span class="lecturer-stat-detail">
                        32 MCQ • 10 Subjective
                    </span>

                </div>

            </a>


            <%-- PUBLISHED QUIZZES --%>

            <a href="<%= ResolveUrl("~/Lecturer/Quizzes.aspx") %>"
               class="lecturer-stat-card">

                <div class="lecturer-stat-icon icon-purple">

                    <svg viewBox="0 0 24 24"
                         aria-hidden="true">

                        <circle cx="12"
                                cy="12"
                                r="8"></circle>

                        <path d="M8.5 12.5l2.2 2.2 4.8-5.3"></path>

                    </svg>

                </div>

                <div class="lecturer-stat-content">

                    <span class="lecturer-stat-heading">
                        Published Quizzes
                    </span>

                    <strong>
                        8 Quizzes
                    </strong>

                    <span class="lecturer-stat-detail">
                        6 Authored • 2 Drafts
                    </span>

                </div>

            </a>


            <%-- LIVE SESSIONS --%>

            <a href="<%= ResolveUrl("~/Lecturer/Sessions.aspx") %>"
               class="lecturer-stat-card">

                <div class="lecturer-stat-icon icon-purple">

                    <svg viewBox="0 0 24 24"
                         aria-hidden="true">

                        <circle cx="12"
                                cy="8"
                                r="2"></circle>

                        <circle cx="6"
                                cy="11"
                                r="2"></circle>

                        <circle cx="18"
                                cy="11"
                                r="2"></circle>

                        <path d="M8 18c0-2 1.6-3.5 4-3.5s4 1.5 4 3.5"></path>

                        <path d="M2.5 17c0-1.5 1.3-2.7 3.5-2.7"></path>

                        <path d="M21.5 17c0-1.5-1.3-2.7-3.5-2.7"></path>

                    </svg>

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


            <%-- GRADING QUEUE --%>

            <a href="<%= ResolveUrl("~/Lecturer/Grading.aspx") %>"
               class="lecturer-stat-card">

                <div class="lecturer-stat-icon icon-red">

                    <svg viewBox="0 0 24 24"
                         aria-hidden="true">

                        <path d="M7 17l10-10"></path>

                        <path d="M5 7l2-2 3 3-2 2"></path>

                        <path d="M14 16l3 3 2-2-3-3"></path>

                        <path d="M5 19l4-1-3-3z"></path>

                    </svg>

                </div>

                <div class="lecturer-stat-content">

                    <span class="lecturer-stat-heading">
                        Grading Queue
                    </span>

                    <strong class="pending-text">
                        4 Pending
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

                            <span class="heading-icon"
                                  aria-hidden="true">
                                ▣
                            </span>

                            My Recent Quizzes

                        </h2>

                        <a href="<%= ResolveUrl("~/Lecturer/Quizzes.aspx") %>"
                           class="card-header-link">

                            View All Quizzes →

                        </a>

                    </div>


                    <div class="recent-quiz-list">


                        <%-- QUIZ 1 --%>

                        <article class="recent-quiz-row">

                            <div class="recent-quiz-main">

                                <div class="quiz-tags">

                                    <span class="quiz-tag">
                                        Mathematics
                                    </span>

                                    <span class="quiz-tag">
                                        Form 5
                                    </span>

                                    <span class="quiz-tag quiz-tag-published">
                                        Published
                                    </span>

                                </div>

                                <h3>
                                    Mathematics Final-Year Practice
                                    (Kertas 1 &amp; 2)
                                </h3>

                                <p>
                                    25 Questions • 50 Marks •
                                    45 Mins • 42 Live Attempts
                                </p>

                            </div>

                            <div class="recent-quiz-actions">

                                <a href="<%= ResolveUrl("~/Lecturer/Sessions.aspx") %>"
                                   class="small-button small-button-primary">

                                    Host Session

                                </a>

                                <a href="#"
                                   class="small-button small-button-light">

                                    Preview

                                </a>

                            </div>

                        </article>


                        <%-- QUIZ 2 --%>

                        <article class="recent-quiz-row">

                            <div class="recent-quiz-main">

                                <div class="quiz-tags">

                                    <span class="quiz-tag quiz-tag-yellow">
                                        Sejarah
                                    </span>

                                    <span class="quiz-tag">
                                        Form 4
                                    </span>

                                    <span class="quiz-tag quiz-tag-published">
                                        Published
                                    </span>

                                </div>

                                <h3>
                                    Sejarah Kertas 2:
                                    Kedaulatan Negara &amp;
                                    Gagasan Malaysia
                                </h3>

                                <p>
                                    10 Questions • 40 Marks •
                                    60 Mins • 4 Pending Grading
                                </p>

                            </div>

                            <div class="recent-quiz-actions">

                                <a href="<%= ResolveUrl("~/Lecturer/Sessions.aspx") %>"
                                   class="small-button small-button-primary">

                                    Host Session

                                </a>

                                <a href="#"
                                   class="small-button small-button-light">

                                    Preview

                                </a>

                            </div>

                        </article>


                        <%-- QUIZ 3 --%>

                        <article class="recent-quiz-row">

                            <div class="recent-quiz-main">

                                <div class="quiz-tags">

                                    <span class="quiz-tag">
                                        Bahasa Melayu
                                    </span>

                                    <span class="quiz-tag">
                                        Form 5
                                    </span>

                                    <span class="quiz-tag quiz-tag-draft">
                                        Draft
                                    </span>

                                </div>

                                <h3>
                                    Bahasa Melayu Sintaksis
                                    Kilat &amp; Tatabahasa SPM
                                </h3>

                                <p>
                                    15 Questions • 30 Marks •
                                    30 Mins • Incomplete draft
                                </p>

                            </div>

                            <div class="recent-quiz-actions">

                                <a href="<%= ResolveUrl("~/Lecturer/Quizzes.aspx") %>"
                                   class="small-button small-button-soft">

                                    Edit Draft

                                </a>

                                <a href="#"
                                   class="small-button small-button-light">

                                    Preview

                                </a>

                            </div>

                        </article>

                    </div>

                </section>


                <%-- ACTIVE CLASSROOM SESSIONS --%>

                <section class="dashboard-card classroom-card">

                    <div class="dashboard-card-header">

                        <h2>

                            <span class="heading-icon"
                                  aria-hidden="true">
                                ⌁
                            </span>

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
                                Pending evaluation from
                                Form 4 &amp; 5 papers
                            </p>

                        </div>

                        <span class="queue-count">
                            4
                        </span>

                    </div>


                    <div class="grading-student-list">


                        <div class="grading-student">

                            <div>

                                <strong>
                                    Aina Lee
                                </strong>

                                <span>
                                    Sejarah Kertas 2 • 20 Oct
                                </span>

                            </div>

                            <a href="<%= ResolveUrl("~/Lecturer/Grading.aspx") %>"
                               class="grade-button">

                                Grade

                            </a>

                        </div>


                        <div class="grading-student">

                            <div>

                                <strong>
                                    Muhammad Danial
                                </strong>

                                <span>
                                    Sejarah Kertas 2 • 20 Oct
                                </span>

                            </div>

                            <a href="<%= ResolveUrl("~/Lecturer/Grading.aspx") %>"
                               class="grade-button">

                                Grade

                            </a>

                        </div>


                        <div class="grading-student">

                            <div>

                                <strong>
                                    Tan Wei Ming
                                </strong>

                                <span>
                                    Maths Mid-Year • 21 Oct
                                </span>

                            </div>

                            <a href="<%= ResolveUrl("~/Lecturer/Grading.aspx") %>"
                               class="grade-button">

                                Grade

                            </a>

                        </div>

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
                                Cohort Pass Rate
                            </h2>

                            <p>
                                Across 148 submitted
                                attempts in October
                            </p>

                        </div>

                    </div>


                    <div class="pass-rate-wrapper">

                        <div class="pass-rate-chart">

                            <div class="pass-rate-chart-center">

                                <strong>
                                    76.4%
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
                            Mathematics (82%)
                        </strong>

                    </div>


                    <div class="performance-detail-row">

                        <span>
                            Needs Attention:
                        </span>

                        <strong class="performance-pill performance-warning">
                            Sejarah Paper 2 (61%)
                        </strong>

                    </div>


                    <a href="<%= ResolveUrl("~/Lecturer/Performance.aspx") %>"
                       class="side-card-button">

                        Cohort Analytics &amp; Export →

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