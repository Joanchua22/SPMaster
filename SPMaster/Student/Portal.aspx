<%@ Page Title="Student Dashboard"
    Language="C#"
    MasterPageFile="~/Master/Portal.Master"
    AutoEventWireup="true"
    CodeBehind="Portal.aspx.cs"
    Inherits="SPMaster.Student.Portal" %>


<asp:Content
    ID="StudentPortalHead"
    ContentPlaceHolderID="PortalHead"
    runat="server">

    <link href="<%= ResolveUrl("~/Content/StudentPortal.css") %>?v=1"
          rel="stylesheet"
          type="text/css" />

</asp:Content>



<asp:Content
    ID="StudentPortalContent"
    ContentPlaceHolderID="PortalContent"
    runat="server">

    <main class="student-dashboard">


        <%-- =====================================================
             HERO
             ===================================================== --%>

        <section class="student-hero">

            <div class="student-hero-content">

                <span class="student-hero-badge">

                    <i class="bi bi-stars"></i>

                    SPM Practice Mode

                </span>


                <h2>

                    Welcome back,
                    <asp:Label
                        ID="lblHeroName"
                        runat="server">
                    </asp:Label>!

                </h2>


                <p>
                    A little practice today. More confidence tomorrow.
                    Master SPM questions with instant feedback and
                    detailed marking.
                </p>



                <div class="student-hero-actions">

                    <a href="<%= ResolveUrl("~/Student/Quizzes.aspx") %>"
                       class="student-explore-button">

                        <i class="bi bi-compass"></i>

                        Explore Quizzes

                    </a>


                    <button type="button"
                            id="btnOpenJoinModal"
                            class="student-join-button">

                        <i class="bi bi-diagram-3"></i>

                        Join with Code

                    </button>

                </div>

            </div>


            <div class="student-hero-decoration">

                <span class="hero-circle hero-circle-large"></span>

                <span class="hero-circle hero-circle-small"></span>

            </div>

        </section>



        <%-- =====================================================
             SUMMARY STATISTICS
             ===================================================== --%>

        <section class="student-stat-grid">


            <%-- COMPLETED --%>

            <article class="student-stat-card">

                <div class="student-stat-heading">

                    <span>
                        Quizzes Completed
                    </span>


                    <div class="student-stat-icon stat-icon-purple">

                        <i class="bi bi-check2-square"></i>

                    </div>

                </div>


                <div class="student-stat-value">

                    <asp:Label
                        ID="lblCompletedAttempts"
                        runat="server"
                        Text="0">
                    </asp:Label>

                    <small>
                        attempts
                    </small>

                </div>


                <p class="student-stat-note stat-note-green">

                    <i class="bi bi-circle-fill"></i>

                    Submitted quiz attempts

                </p>

            </article>



            <%-- AVERAGE SCORE --%>

            <article class="student-stat-card">

                <div class="student-stat-heading">

                    <span>
                        Average Graded Score
                    </span>


                    <div class="student-stat-icon stat-icon-lavender">

                        <i class="bi bi-graph-up-arrow"></i>

                    </div>

                </div>


                <div class="student-stat-value">

                    <asp:Label
                        ID="lblAverageScore"
                        runat="server"
                        Text="–">
                    </asp:Label>

                </div>


                <p class="student-stat-note stat-note-purple">

                    <i class="bi bi-patch-check"></i>

                    <asp:Label
                        ID="lblAverageDescription"
                        runat="server"
                        Text="No graded attempts yet">
                    </asp:Label>

                </p>

            </article>



            <%-- WAITING GRADING --%>

            <article class="student-stat-card">

                <div class="student-stat-heading">

                    <span>
                        Awaiting Grading
                    </span>


                    <div class="student-stat-icon stat-icon-yellow">

                        <i class="bi bi-hourglass-split"></i>

                    </div>

                </div>


                <div class="student-stat-value">

                    <asp:Label
                        ID="lblPendingGrading"
                        runat="server"
                        Text="0">
                    </asp:Label>

                    <small>
                        attempts
                    </small>

                </div>


                <p class="student-stat-note stat-note-yellow">

                    <i class="bi bi-circle-fill"></i>

                    Subjective answers under lecturer review

                </p>

            </article>

        </section>



        <%-- =====================================================
             SUBJECT HUBS
             ===================================================== --%>

        <section class="student-subject-section">

            <div class="student-section-header">

                <div>

                    <h2>
                        Subject Hubs
                    </h2>

                    <p>
                        Browse available SPM quizzes by subject.
                    </p>

                </div>


                <a href="<%= ResolveUrl("~/Student/Quizzes.aspx") %>"
                   class="student-browse-link">

                    Browse all

                    <i class="bi bi-arrow-right"></i>

                </a>

            </div>



            <asp:Panel
                ID="pnlNoSubjects"
                runat="server"
                Visible="false"
                CssClass="student-empty-card">

                No subjects are currently available.

            </asp:Panel>



            <div class="student-subject-grid">

                <asp:Repeater
                    ID="rptSubjectHubs"
                    runat="server">

                    <ItemTemplate>

                        <article class="student-subject-card">

                            <div class="subject-card-top">

                                <span class='subject-card-icon <%# Eval("IconCss") %>'>

                                    <i class='<%# Eval("IconClass") %>'></i>

                                </span>


                                <span class="subject-quiz-count">

                                    <%# Eval("QuizCount") %>

                                    <%# Convert.ToInt32(
                                            Eval("QuizCount")
                                        ) == 1
                                            ? " Quiz"
                                            : " Quizzes"
                                    %>

                                </span>

                            </div>



                            <h3>
                                <%# Eval("SubjectName") %>
                            </h3>


                            <p>
                                <%# Eval("Description") %>
                            </p>


                            <a href='<%#
                                ResolveUrl(
                                    "~/Student/Quizzes.aspx?subject="
                                    + Eval("SubjectId")
                                )
                            %>'
                               class="subject-explore-button">

                                Explore quizzes

                                <i class="bi bi-chevron-right"></i>

                            </a>

                        </article>

                    </ItemTemplate>

                </asp:Repeater>

            </div>

        </section>



        <%-- =====================================================
             LOWER DASHBOARD
             ===================================================== --%>

        <section class="student-dashboard-bottom">


            <%-- =================================================
                 WEEKLY ACTIVITY
                 ================================================= --%>

            <article class="student-dashboard-card weekly-card">

                <div class="student-card-header">

                    <div>

                        <h2>
                            Weekly Practice Activity
                        </h2>

                        <p>
                            Your quiz activity during the last 7 days.
                        </p>

                    </div>


                    <span class="weekly-total">

                        <asp:Label
                            ID="lblWeeklyAttempts"
                            runat="server"
                            Text="0 attempts">
                        </asp:Label>

                    </span>

                </div>



                <div class="weekly-chart">

                    <asp:Repeater
                        ID="rptWeeklyActivity"
                        runat="server">

                        <ItemTemplate>

                            <div class="weekly-day">

                                <div class="weekly-bar-area">

                                    <span class="weekly-number">
                                        <%# Eval("AttemptCount") %>
                                    </span>


                                    <div class="weekly-bar-track">

                                        <div class="weekly-bar"
                                             style='<%#
                                                "height:"
                                                + Eval("BarHeight")
                                                + "%;"
                                             %>'>
                                        </div>

                                    </div>

                                </div>


                                <span class="weekly-day-name">
                                    <%# Eval("DayName") %>
                                </span>

                            </div>

                        </ItemTemplate>

                    </asp:Repeater>

                </div>

            </article>



            <%-- =================================================
                 RECENT ATTEMPTS
                 ================================================= --%>

            <article class="student-dashboard-card recent-card">

                <div class="student-card-header">

                    <div>

                        <h2>
                            Recent Attempts
                        </h2>

                        <p>
                            Track your latest results and
                            lecturer grading status.
                        </p>

                    </div>


                    <a href="<%= ResolveUrl("~/Student/Quizzes.aspx?view=attempts") %>"
                       class="recent-view-all">

                        View all

                    </a>

                </div>



                <asp:Panel
                    ID="pnlNoRecentAttempts"
                    runat="server"
                    Visible="false"
                    CssClass="student-recent-empty">

                    <i class="bi bi-journal-text"></i>

                    <span>
                        You have not attempted a quiz yet.
                    </span>

                </asp:Panel>



                <div class="recent-attempt-list">

                    <asp:Repeater
                        ID="rptRecentAttempts"
                        runat="server">

                        <ItemTemplate>

                            <article class="recent-attempt-item">

                                <div class="recent-attempt-main">

                                    <span class='recent-attempt-icon <%# Eval("IconCss") %>'>

                                        <i class='<%# Eval("IconClass") %>'></i>

                                    </span>


                                    <div class="recent-attempt-info">

                                        <h3>
                                            <%# Eval("QuizTitle") %>
                                        </h3>


                                        <p>
                                            <%# Eval("AttemptDescription") %>
                                        </p>

                                    </div>

                                </div>



                                <div class="recent-attempt-actions">

                                    <span class='recent-status <%# Eval("StatusCss") %>'>

                                        <%# Eval("StatusText") %>

                                    </span>


                                    <a href='<%# Eval("ActionUrl") %>'
                                       class='<%# Eval("ActionCss") %>'>

                                        <%# Eval("ActionText") %>

                                    </a>

                                </div>

                            </article>

                        </ItemTemplate>

                    </asp:Repeater>

                </div>

            </article>

        </section>



        <%-- =====================================================
             JOIN SESSION MODAL
             ===================================================== --%>

        <div id="studentJoinModal"
             class="student-join-modal"
             hidden>


            <div id="studentJoinOverlay"
                 class="student-join-overlay">
            </div>



            <div class="student-join-dialog">

                <div class="student-join-header">

                    <div class="student-join-title">

                        <span class="student-join-title-icon">

                            <i class="bi bi-key"></i>

                        </span>


                        <div>

                            <h2>
                                Join Live Quiz Session
                            </h2>

                            <p>
                                Enter the code given by your lecturer.
                            </p>

                        </div>

                    </div>


                    <button type="button"
                            id="btnCloseJoinModal"
                            class="student-join-close">

                        <i class="bi bi-x-lg"></i>

                    </button>

                </div>



                <%-- =============================================
                     JOIN ERROR
                     ============================================= --%>

                <asp:Panel
                    ID="pnlJoinError"
                    runat="server"
                    Visible="false"
                    CssClass="join-error-message">

                    <i class="bi bi-exclamation-circle"></i>

                    <asp:Label
                        ID="lblJoinError"
                        runat="server">
                    </asp:Label>

                </asp:Panel>



                <%-- =============================================
                     JOIN CODE FIELD
                     ============================================= --%>

                <div class="join-code-field">

                    <label for="<%= txtJoinCode.ClientID %>">

                        Session Access Code

                        <span class="required-mark">
                            *
                        </span>

                    </label>


                    <div class="join-code-input-row">

                        <asp:TextBox
                            ID="txtJoinCode"
                            runat="server"
                            CssClass="join-code-input"
                            MaxLength="20"
                            placeholder="SPM-1234">
                        </asp:TextBox>


                        <asp:Button
                            ID="btnFindSession"
                            runat="server"
                            Text="Find"
                            CssClass="join-find-button"
                            ValidationGroup="JoinSession"
                            CausesValidation="true"
                            OnClick="btnFindSession_Click" />

                    </div>


                    <asp:RequiredFieldValidator
                        ID="rfvJoinCode"
                        runat="server"
                        ControlToValidate="txtJoinCode"
                        ErrorMessage="Please enter a session code."
                        CssClass="join-field-error"
                        ValidationGroup="JoinSession"
                        Display="Dynamic"
                        SetFocusOnError="true">
                    </asp:RequiredFieldValidator>

                </div>



                <%-- =============================================
                     FOUND SESSION
                     ============================================= --%>

                <asp:Panel
                    ID="pnlFoundSession"
                    runat="server"
                    Visible="false"
                    CssClass="found-session-card">


                    <span class="found-session-status">

                        <i class="bi bi-broadcast"></i>

                        Session Live

                    </span>



                    <h3>

                        <asp:Label
                            ID="lblFoundQuizTitle"
                            runat="server">
                        </asp:Label>

                    </h3>



                    <p class="found-session-name">

                        <asp:Label
                            ID="lblFoundSessionTitle"
                            runat="server">
                        </asp:Label>

                    </p>



                    <div class="found-session-meta">


                        <span>

                            <i class="bi bi-list-check"></i>

                            <asp:Label
                                ID="lblFoundQuestionCount"
                                runat="server">
                            </asp:Label>

                            Questions

                        </span>



                        <span>

                            <i class="bi bi-award"></i>

                            <asp:Label
                                ID="lblFoundMarks"
                                runat="server">
                            </asp:Label>

                            Marks

                        </span>



                        <span>

                            <i class="bi bi-clock"></i>

                            <asp:Label
                                ID="lblFoundTime"
                                runat="server">
                            </asp:Label>

                            Minutes

                        </span>

                    </div>

                </asp:Panel>



                <%-- Selected session after Find --%>

                <asp:HiddenField
                    ID="hfFoundSessionId"
                    runat="server" />


                <asp:HiddenField
                    ID="hfFoundQuizId"
                    runat="server" />



                <%--
                    JS uses this after a server postback.

                    1 = reopen modal.
                    0 = leave modal closed.
                --%>

                <asp:HiddenField
                    ID="hfOpenJoinModal"
                    runat="server"
                    ClientIDMode="Static"
                    Value="0" />



                <%-- =============================================
                     ACTIONS
                     ============================================= --%>

                <div class="student-join-actions">

                    <button type="button"
                            id="btnCancelJoin"
                            class="join-cancel-button">

                        Cancel

                    </button>


                    <asp:Button
                        ID="btnStartSession"
                        runat="server"
                        Text="Start Attempt"
                        CssClass="join-start-button"
                        Visible="false"
                        CausesValidation="false"
                        OnClick="btnStartSession_Click" />

                </div>

            </div>

        </div>

    </main>

</asp:Content>



<asp:Content
    ID="StudentPortalScripts"
    ContentPlaceHolderID="PortalScripts"
    runat="server">

    <script src="<%= ResolveUrl("~/Scripts/StudentPortal.js") %>?v=1">
    </script>

</asp:Content>