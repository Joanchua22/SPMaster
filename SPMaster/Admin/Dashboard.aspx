<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs"
    Inherits="SPMaster.Admin.Dashboard" %>

<asp:Content
    ID="AdminTitle"
    ContentPlaceHolderID="DashboardTitle"
    runat="server">
    System Overview
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

        <a href="<%= ResolveUrl("~/Admin/Profile.aspx") %>"
           class="nav-item">

            <span class="nav-icon">
                <i class="bi bi-person-circle"></i>
            </span>

            <span>Profile</span>
        </a>

        <a href="<%= ResolveUrl("~/Admin/Settings.aspx") %>"
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

        <span class="user-avatar" aria-hidden="true">
            AD
        </span>

        <div class="user-details">
            <span class="user-name">Admin</span>
            <span class="user-role">Admin</span>
        </div>

    </div>

</asp:Content>

<asp:Content ID="AdminContent"
    ContentPlaceHolderID="DashboardContent"
    runat="server">

    <!-- SYSTEM HEALTH -->
    <section class="admin-health-card">

        <div class="admin-health-left">
            <span class="health-dot"></span>

            <span class="health-text">
                System Health: All services operational
            </span>
        </div>

        <span class="backup-text">
            Last database backup: 2 hours ago
        </span>

    </section>


    <!-- SUMMARY CARDS -->
    <section class="admin-summary-grid">

        <div class="summary-card">

            <span class="summary-label">
                Total Users
            </span>

            <span class="summary-value">
                1,248
            </span>

            <span class="summary-growth">
                +32 this week
            </span>

        </div>


        <div class="summary-card">

            <span class="summary-label">
                Active Lecturers
            </span>

            <span class="summary-value">
                24
            </span>

        </div>


        <div class="summary-card">

            <span class="summary-label">
                Pending Reports
            </span>

            <span class="summary-value pending-value">
                7
            </span>

        </div>


        <div class="summary-card">

            <span class="summary-label">
                Database Size
            </span>

            <span class="summary-value">
                4.2 GB
            </span>

        </div>

    </section>


    <!-- PLATFORM ACTIVITY -->
    <section class="admin-card activity-section">

        <h2 class="admin-card-title">
            Platform Activity
        </h2>

        <div class="activity-chart">

            <div class="chart-grid-line line-1"></div>
            <div class="chart-grid-line line-2"></div>
            <div class="chart-grid-line line-3"></div>

            <svg
                class="activity-svg"
                viewBox="0 0 700 180"
                preserveAspectRatio="none">

                <polygon
                    points="
                    20,140
                    130,128
                    240,116
                    350,98
                    460,82
                    570,50
                    680,20
                    680,160
                    20,160"
                    fill="rgba(145,72,255,0.12)">
                </polygon>

                <polyline
                    points="
                    20,140
                    130,128
                    240,116
                    350,98
                    460,82
                    570,50
                    680,20"
                    fill="none"
                    stroke="#9148ff"
                    stroke-width="4">
                </polyline>

                <circle cx="20" cy="140" r="5" fill="#9148ff"></circle>
                <circle cx="130" cy="128" r="5" fill="#9148ff"></circle>
                <circle cx="240" cy="116" r="5" fill="#9148ff"></circle>
                <circle cx="350" cy="98" r="5" fill="#9148ff"></circle>
                <circle cx="460" cy="82" r="5" fill="#9148ff"></circle>
                <circle cx="570" cy="50" r="5" fill="#9148ff"></circle>
                <circle cx="680" cy="20" r="5" fill="#9148ff"></circle>

            </svg>

            <div class="chart-days">

                <span>Mon</span>
                <span>Tue</span>
                <span>Wed</span>
                <span>Thu</span>
                <span>Fri</span>
                <span>Sat</span>
                <span>Sun</span>

            </div>

        </div>

    </section>


    <!-- BOTTOM SECTION -->
    <section class="admin-bottom-grid">


        <!-- RECENT SIGNUPS -->
        <div class="admin-card">

            <h2 class="admin-card-title">
                Recent User Signups
            </h2>


            <div class="signup-row">

                <div>

                    <div class="signup-name">
                        Farah Aziz
                    </div>

                    <div class="signup-date">
                        joined today
                    </div>

                </div>

                <span class="role-badge student-badge">
                    Student
                </span>

            </div>


            <div class="signup-row">

                <div>

                    <div class="signup-name">
                        Hafiz Salleh
                    </div>

                    <div class="signup-date">
                        joined today
                    </div>

                </div>

                <span class="role-badge student-badge">
                    Student
                </span>

            </div>


            <div class="signup-row">

                <div>

                    <div class="signup-name">
                        Dr. Wong Mei
                    </div>

                    <div class="signup-date">
                        joined yesterday
                    </div>

                </div>

                <span class="role-badge lecturer-badge">
                    Lecturer
                </span>

            </div>

        </div>


        <!-- PENDING REPORTS -->
        <div class="admin-card">

            <h2 class="admin-card-title">
                Pending Feedback Reports
            </h2>


            <div class="report-row">

                <span class="report-tag content-bug">
                    Content Bug
                </span>

                <span class="report-description">
                    Mathematics quiz
                </span>

                <button type="button"
                    class="review-button">

                    Review

                </button>

            </div>


            <div class="report-row">

                <span class="report-tag technical-issue">
                    Technical Issue
                </span>

                <span class="report-description">
                    Login page
                </span>

                <button type="button"
                    class="review-button">

                    Review

                </button>

            </div>


            <div class="report-row">

                <span class="report-tag student-conduct">
                    Student Conduct
                </span>

                <span class="report-description">
                    reported by Mr. Rahman
                </span>

                <button type="button"
                    class="review-button">

                    Review

                </button>

            </div>

        </div>

    </section>

</asp:Content>