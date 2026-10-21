<%@ Page Title="System Overview"
    Language="C#"
    MasterPageFile="~/Master/Dashboard.Master"
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
    ID="AdminContent"
    ContentPlaceHolderID="DashboardContent"
    runat="server">


    <%-- =====================================================
         SYSTEM HEALTH
         ===================================================== --%>

    <section class="admin-health-card">

        <div class="admin-health-left">

            <span
                class="health-dot"
                aria-hidden="true">
            </span>


            <span class="health-text">

                <asp:Label
                    ID="lblSystemHealth"
                    runat="server"
                    Text="System Health: Checking...">
                </asp:Label>

            </span>

        </div>


        <span class="backup-text">

            <asp:Label
                ID="lblDatabaseStatus"
                runat="server"
                Text="Database: Checking...">
            </asp:Label>

        </span>

    </section>



    <%-- =====================================================
         SUMMARY CARDS
         ===================================================== --%>

    <section
        class="admin-summary-grid"
        aria-label="Platform summary">


        <%-- TOTAL USERS --%>

        <div class="summary-card">

            <span class="summary-label">
                Total Users
            </span>


            <span class="summary-value">

                <asp:Label
                    ID="lblTotalUsers"
                    runat="server"
                    Text="0">
                </asp:Label>

            </span>


            <span class="summary-growth">

                +<asp:Label
                    ID="lblNewUsersThisWeek"
                    runat="server"
                    Text="0">
                </asp:Label>

                this week

            </span>

        </div>



        <%-- ACTIVE LECTURERS --%>

        <div class="summary-card">

            <span class="summary-label">
                Active Lecturers
            </span>


            <span class="summary-value">

                <asp:Label
                    ID="lblActiveLecturers"
                    runat="server"
                    Text="0">
                </asp:Label>

            </span>

        </div>



        <%-- PENDING REPORTS --%>

        <div class="summary-card">

            <span class="summary-label">
                Pending Reports
            </span>


            <span class="summary-value pending-value">

                <asp:Label
                    ID="lblPendingReports"
                    runat="server"
                    Text="0">
                </asp:Label>

            </span>

        </div>



        <%-- DATABASE SIZE --%>

        <div class="summary-card">

            <span class="summary-label">
                Database Size
            </span>


            <span class="summary-value">

                <asp:Label
                    ID="lblDatabaseSize"
                    runat="server"
                    Text="0 MB">
                </asp:Label>

            </span>

        </div>

    </section>



    <%-- =====================================================
         PLATFORM ACTIVITY
         ===================================================== --%>

    <section
        class="admin-card activity-section"
        aria-labelledby="activity-title">


        <h2
            id="activity-title"
            class="admin-card-title">

            Platform Activity

        </h2>


        <div class="activity-chart">


            <%-- =============================================
                 Y AXIS
                 ============================================= --%>

            <div class="activity-y-axis">

                <span class="activity-y-title">
                    Quiz Attempts
                </span>

                <div class="activity-y-values">

                    <span><%= ActivityYAxisMax %></span>
                    <span><%= ActivityYAxisMidHigh %></span>
                    <span><%= ActivityYAxisMidLow %></span>
                    <span>0</span>

                </div>

            </div>



            <%-- =============================================
                 CHART BODY
                 ============================================= --%>

            <div class="activity-chart-body">


                <%-- GRID LINES --%>

                <div
                    class="chart-grid-line line-1"
                    aria-hidden="true">
                </div>


                <div
                    class="chart-grid-line line-2"
                    aria-hidden="true">
                </div>


                <div
                    class="chart-grid-line line-3"
                    aria-hidden="true">
                </div>



                <%-- SVG CHART --%>

                <svg
                    class="activity-svg"
                    viewBox="0 0 700 180"
                    preserveAspectRatio="none"
                    role="img"
                    aria-labelledby="activity-chart-title">


                    <title id="activity-chart-title">
                        Number of quiz attempts recorded from Monday to Sunday.
                    </title>



                    <%-- AREA UNDER LINE --%>

                    <polygon
                        points="<%= ActivityPolygonPoints %>"
                        fill="rgba(145,72,255,0.12)">
                    </polygon>



                    <%-- ACTIVITY LINE --%>

                    <polyline
                        points="<%= ActivityPolylinePoints %>"
                        fill="none"
                        stroke="#9148ff"
                        stroke-width="4">
                    </polyline>



                    <%-- DATA POINTS --%>

                    <%= ActivityCircles %>


                </svg>



                <%-- =========================================
                     DAYS + EXACT VALUES
                     ========================================= --%>

                <div class="chart-days">


                    <span>

                        Mon

                        <small>
                            <%= ActivityCounts[0] %>
                        </small>

                    </span>


                    <span>

                        Tue

                        <small>
                            <%= ActivityCounts[1] %>
                        </small>

                    </span>


                    <span>

                        Wed

                        <small>
                            <%= ActivityCounts[2] %>
                        </small>

                    </span>


                    <span>

                        Thu

                        <small>
                            <%= ActivityCounts[3] %>
                        </small>

                    </span>


                    <span>

                        Fri

                        <small>
                            <%= ActivityCounts[4] %>
                        </small>

                    </span>


                    <span>

                        Sat

                        <small>
                            <%= ActivityCounts[5] %>
                        </small>

                    </span>


                    <span>

                        Sun

                        <small>
                            <%= ActivityCounts[6] %>
                        </small>

                    </span>


                </div>

            </div>

        </div>

    </section>



    <%-- =====================================================
         BOTTOM SECTION
         ===================================================== --%>

    <section
        class="admin-bottom-grid"
        aria-label="Recent signups and reports">


        <%-- =================================================
             RECENT USER SIGNUPS
             ================================================= --%>

        <div class="admin-card">


            <h2 class="admin-card-title">
                Recent User Signups
            </h2>


            <asp:Repeater
                ID="rptRecentUsers"
                runat="server">


                <ItemTemplate>


                    <div class="signup-row">


                        <div>


                            <div class="signup-name">

                                <%# Eval("FullName") %>

                            </div>


                            <div class="signup-date">

                                <%#
                                    GetJoinedText(
                                        Eval("CreatedAt")
                                    )
                                %>

                            </div>


                        </div>



                        <span class='<%#
                            GetRoleBadgeClass(
                                Eval("RoleName")
                            )
                        %>'>

                            <%# Eval("RoleName") %>

                        </span>


                    </div>


                </ItemTemplate>


            </asp:Repeater>



            <asp:Panel
                ID="pnlNoRecentUsers"
                runat="server"
                Visible="false"
                CssClass="dashboard-empty">

                No recent user signups.

            </asp:Panel>


        </div>



        <%-- =================================================
             PENDING FEEDBACK REPORTS
             ================================================= --%>

        <div class="admin-card">


            <h2 class="admin-card-title">
                Pending Feedback Reports
            </h2>


            <asp:Repeater
                ID="rptPendingReports"
                runat="server">


                <ItemTemplate>


                    <div class="report-row">


                        <span class='<%#
                            GetReportStatusClass(
                                Eval("Status")
                            )
                        %>'>

                            <%# Eval("Status") %>

                        </span>



                        <span class="report-description">

                            <%# Eval("DisplayTitle") %>

                        </span>



                        <a
                            href='<%#
                                ResolveUrl(
                                    "~/Admin/RespondFeedback.aspx?id="
                                    + Eval("FeedbackId")
                                )
                            %>'
                            class="review-button">

                            Review

                        </a>


                    </div>


                </ItemTemplate>


            </asp:Repeater>



            <asp:Panel
                ID="pnlNoPendingReports"
                runat="server"
                Visible="false"
                CssClass="dashboard-empty">

                No pending feedback reports.

            </asp:Panel>


        </div>


    </section>


</asp:Content>