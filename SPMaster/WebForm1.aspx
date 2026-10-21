<%@ Page
    Title="Performance Analytics"
    Language="C#"
    MasterPageFile="~/Master/Portal.Master"
    AutoEventWireup="true"
    CodeBehind="WebForm1.aspx.cs"
    Inherits="SPMaster.Student.Performance" %>


<asp:Content
    ID="PerformanceHead"
    ContentPlaceHolderID="PortalHead"
    runat="server">

    <link
        href="<%= ResolveUrl("StyleSheet1.css") %>?v=1"
        rel="stylesheet"
        type="text/css" />

</asp:Content>



<asp:Content
    ID="PerformanceContent"
    ContentPlaceHolderID="PortalContent"
    runat="server">

    <div class="performance-page">


        <%-- =====================================================
             PAGE HEADER
             ===================================================== --%>

        <section class="performance-header">

            <div class="performance-header-text">

                <h1>
                    Performance Analytics
                </h1>

                <p>
                    Continuous readiness indicators based on
                    your quiz performance.
                </p>

            </div>


            <%-- ================================================
                 FILTERS
                 ================================================ --%>

            <div class="performance-filters">


                <%-- SUBJECT FILTER --%>

                <div class="performance-filter-group">

                    <label for="<%= ddlSubject.ClientID %>">
                        Subject
                    </label>

                    <asp:DropDownList
                        ID="ddlSubject"
                        runat="server"
                        CssClass="performance-filter-select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlSubject_SelectedIndexChanged">
                    </asp:DropDownList>

                </div>


                <%-- TIME PERIOD FILTER --%>

                <div class="performance-filter-group">

                    <label for="<%= ddlPeriod.ClientID %>">
                        Period
                    </label>

                    <asp:DropDownList
                        ID="ddlPeriod"
                        runat="server"
                        CssClass="performance-filter-select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged">

                        <asp:ListItem
                            Text="Past 30 Days"
                            Value="1m">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Past 3 Months"
                            Value="3m"
                            Selected="True">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Past 6 Months"
                            Value="6m">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="All Time"
                            Value="all">
                        </asp:ListItem>

                    </asp:DropDownList>

                </div>

            </div>

        </section>



        <%-- =====================================================
             SUMMARY CARDS
             ===================================================== --%>

        <section class="performance-summary-grid">


            <%-- COMPLETED ATTEMPTS --%>

            <article class="performance-summary-card">

                <div class="summary-card-top">

                    <span class="summary-label">
                        Completed Attempts
                    </span>

                    <span class="summary-icon summary-icon-purple">
                        <i class="bi bi-check2-circle"></i>
                    </span>

                </div>


                <strong class="summary-value">

                    <asp:Label
                        ID="lblCompletedAttempts"
                        runat="server"
                        Text="0">
                    </asp:Label>

                </strong>


                <span class="summary-description">
                    Submitted quizzes
                </span>

            </article>



            <%-- AVERAGE GRADED SCORE --%>

            <article class="performance-summary-card">

                <div class="summary-card-top">

                    <span class="summary-label">
                        Average Graded Score
                    </span>

                    <span class="summary-icon summary-icon-purple">
                        <i class="bi bi-graph-up-arrow"></i>
                    </span>

                </div>


                <strong class="summary-value summary-value-purple">

                    <asp:Label
                        ID="lblAverageScore"
                        runat="server"
                        Text="–">
                    </asp:Label>

                </strong>


                <span class="summary-description">
                    Fully graded quiz attempts
                </span>

            </article>



            <%-- PENDING REVIEW --%>

            <article class="performance-summary-card">

                <div class="summary-card-top">

                    <span class="summary-label">
                        Pending Lecturer Review
                    </span>

                    <span class="summary-icon summary-icon-yellow">
                        <i class="bi bi-hourglass-split"></i>
                    </span>

                </div>


                <strong class="summary-value summary-value-yellow">

                    <asp:Label
                        ID="lblPendingReview"
                        runat="server"
                        Text="0">
                    </asp:Label>

                </strong>


                <span class="summary-description">
                    Attempts awaiting subjective grading
                </span>

            </article>

        </section>



        <%-- =====================================================
             SCORE PROGRESS TREND
             ===================================================== --%>

        <section class="performance-chart-card">

            <div class="performance-card-header">

                <div>

                    <h2>
                        Score Progress Trend
                    </h2>

                    <p>
                        Monthly average based on your
                        fully graded quiz attempts.
                    </p>

                </div>


                <span class="chart-score-label">
                    Score (%)
                </span>

            </div>


            <div class="performance-chart-wrapper">

                <canvas
                    id="scoreTrendCanvas">
                </canvas>

            </div>

        </section>



        <%-- =====================================================
             HIDDEN SCORE TREND DATA

             Stores score trend data generated in
             Performance.aspx.cs.

             The C# code converts the database results into JSON.
             Performance.js reads this hidden value and uses it
             to draw the Score Progress Trend chart.

             This field is not visible to the student.
             ===================================================== --%>

        <asp:HiddenField
            ID="hfTrendJson"
            runat="server"
            ClientIDMode="Static" />



        <%-- =====================================================
             SUBJECT MASTERY
             ===================================================== --%>

        <section class="performance-mastery-section">

            <div class="performance-section-heading">

                <div>

                    <h2>
                        Subject Mastery Breakdown
                    </h2>

                    <p>
                        Your average performance across
                        completed and fully graded subjects.
                    </p>

                </div>

            </div>



            <%-- NO DATA --%>

            <asp:Panel
                ID="pnlNoSubjectData"
                runat="server"
                CssClass="performance-empty-state"
                Visible="false">

                <i class="bi bi-bar-chart"></i>

                <strong>
                    No graded performance data yet
                </strong>

                <span>
                    Complete a quiz and wait for all questions
                    to be graded to see your subject performance.
                </span>

            </asp:Panel>



            <%-- DYNAMIC SUBJECT CARDS --%>

            <div class="mastery-grid">

                <asp:Repeater
                    ID="rptSubjectMastery"
                    runat="server">

                    <ItemTemplate>

                        <article class="mastery-card">


                            <%-- CARD HEADER --%>

                            <div class="mastery-card-header">

                                <span class="mastery-subject">

                                    <%# Eval("SubjectName") %>

                                </span>


                                <span class="mastery-score">

                                    <%# Eval(
                                        "AverageScore",
                                        "{0:0.0}%"
                                    ) %>

                                </span>

                            </div>



                            <%-- PROGRESS BAR --%>

                            <div class="mastery-progress-track">

                                <div
                                    class='mastery-progress-fill <%# Eval("ProgressClass") %>'
                                    style='width:<%# Eval(
                                        "AverageScore",
                                        "{0:0.0}"
                                    ) %>%;'>
                                </div>

                            </div>



                            <%-- DETAILS --%>

                            <div class="mastery-card-details">

                                <span>

                                    <i class="bi bi-check2-circle"></i>

                                    <%# Eval("AttemptCount") %>

                                    graded
                                    <%# Convert.ToInt32(
                                        Eval("AttemptCount")
                                    ) == 1
                                        ? "attempt"
                                        : "attempts" %>

                                </span>


                                <span class="mastery-grade">

                                    Grade

                                    <strong>
                                        <%# Eval("Grade") %>
                                    </strong>

                                </span>

                            </div>

                        </article>

                    </ItemTemplate>

                </asp:Repeater>

            </div>

        </section>

    </div>

</asp:Content>



<asp:Content
    ID="PerformanceScripts"
    ContentPlaceHolderID="PortalScripts"
    runat="server">

    <script
        src="<%= ResolveUrl("~/Scripts/JavaScript.js") %>?v=1">
    </script>

</asp:Content>