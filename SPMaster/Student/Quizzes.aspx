
<%@ Page Title="Quizzes" Language="C#" MasterPageFile="~/Master/Portal.Master" AutoEventWireup="true" CodeBehind="Quizzes.aspx.cs" Inherits="SPMaster.Student.Quizzes" %>

<asp:Content ID="StudentQuizzesHead" ContentPlaceHolderID="PortalHead" runat="server">
    <link href="<%= ResolveUrl("~/Content/StudentPortal.css") %>" rel="stylesheet" type="text/css" />
    <link href="<%= ResolveUrl("~/Content/StudentQuizzes.css") %>?v=3" rel="stylesheet" type="text/css" />
</asp:Content>


<asp:Content ID="StudentQuizzesContent" ContentPlaceHolderID="PortalContent" runat="server">

    <div class="student-quizzes-page">

        <%-- =====================================================
             PAGE HEADER
             ===================================================== --%>

        <section class="student-quizzes-header">

            <div class="student-quizzes-heading">
                <h1>Quizzes</h1>
                <p>Practice syllabus drills, past-year sets, or review your attempt scores</p>
            </div>

            <div class="quiz-view-switch">

                <button type="button" id="btnExploreTab" class="quiz-view-tab active" data-quiz-view="explore">
                    Explore Quizzes
                </button>

                <button type="button" id="btnAttemptsTab" class="quiz-view-tab" data-quiz-view="attempts">
                    My Attempts
                    <span class="attempt-count">(<asp:Literal ID="litAttemptCount" runat="server" Text="0"></asp:Literal>)</span>
                </button>

            </div>

        </section>


        <%-- =====================================================
             EXPLORE QUIZZES
             ===================================================== --%>

        <section id="exploreQuizzesView" class="quiz-view-section">

            <%-- =================================================
                 SEARCH AND FILTER
                 ================================================= --%>

            <section class="student-quiz-filter-card">

                <%-- SEARCH --%>
                <div class="student-quiz-search">
                    <i class="bi bi-search" aria-hidden="true"></i>

                    <asp:TextBox
                        ID="txtQuizSearch"
                        runat="server"
                        AutoPostBack="true"
                        OnTextChanged="FilterChanged"
                        placeholder="Search by quiz title, teacher, or topic..."
                        aria-label="Search quizzes">
                    </asp:TextBox>
                </div>

                <%-- SUBJECT --%>
                <div class="student-subject-filter">

                    <asp:DropDownList
                        ID="ddlStudentSubject"
                        runat="server"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="FilterChanged"
                        aria-label="Filter quizzes by subject">
                    </asp:DropDownList>

                    <i class="bi bi-chevron-down" aria-hidden="true"></i>

                </div>

                <%-- JOIN CODE BUTTON --%>
                <button type="button" id="btnJoinCode" class="join-code-button">
                    <i class="bi bi-key" aria-hidden="true"></i>
                    Join Code
                </button>

            </section>


            <%-- =================================================
                 QUIZ CARDS
                 ================================================= --%>

            <section class="student-quiz-grid">

                <asp:Repeater ID="rptExploreQuizzes" runat="server">

                    <ItemTemplate>

                        <article class="student-quiz-card">

                            <%-- TOP --%>
                            <div class="student-quiz-card-top">

                                <span class='student-quiz-subject <%# GetSubjectCss(Eval("SubjectName")) %>'>
                                    <%#: Eval("SubjectName") %> • <%#: Eval("FormName") %>
                                </span>

                                <span class="student-quiz-format">
                                    <%#: GetQuizFormat(Eval("McqCount"), Eval("SubjectiveCount")) %>
                                </span>

                            </div>


                            <%-- CONTENT --%>
                            <div class="student-quiz-card-content">

                                <h2><%#: Eval("Title") %></h2>

                                <p><%#: GetQuizDescription(Eval("Description")) %></p>

                            </div>


                            <%-- META --%>
                            <div class="student-quiz-meta">

                                <span><%# FormatQuestionCount(Eval("QuestionCount")) %></span>

                                <span><%# FormatMarks(Eval("TotalMarks")) %></span>

                                <span><%# FormatMinutes(Eval("TimeLimitMinutes")) %></span>

                            </div>


                            <%-- FOOTER --%>
                            <div class="student-quiz-card-footer">

                                <div class="student-quiz-teacher">

                                    <%-- PROFILE IMAGE --%>
                                    <asp:PlaceHolder
                                        runat="server"
                                        Visible='<%# HasProfileImage(Eval("CreatorProfileImage")) %>'>

                                        <img
                                            src='<%# GetProfileImageUrl(Eval("CreatorProfileImage")) %>'
                                            class="teacher-profile-image"
                                            alt="Lecturer profile" />

                                    </asp:PlaceHolder>

                                    <%-- FALLBACK INITIALS --%>
                                    <asp:PlaceHolder
                                        runat="server"
                                        Visible='<%# !HasProfileImage(Eval("CreatorProfileImage")) %>'>

                                        <span class="teacher-avatar">
                                            <%#: GetInitials(Eval("CreatorName")) %>
                                        </span>

                                    </asp:PlaceHolder>

                                    <span><%#: Eval("CreatorName") %></span>

                                </div>

                                <a href='<%# ResolveUrl("~/Student/ViewQuiz.aspx?id=" + Eval("QuizId")) %>'
                                   class="view-quiz-button">
                                    View Quiz
                                </a>

                            </div>

                        </article>

                    </ItemTemplate>

                </asp:Repeater>

            </section>


            <%-- =================================================
                 EMPTY QUIZ STATE
                 ================================================= --%>

            <asp:Panel
                ID="pnlNoQuizzes"
                runat="server"
                Visible="false"
                CssClass="student-empty-state">

                <i class="bi bi-journal-x" aria-hidden="true"></i>

                <h3>No quizzes found</h3>

                <p>Try changing your search or subject filter.</p>

            </asp:Panel>

        </section>


        <%-- =====================================================
             MY ATTEMPTS
             ===================================================== --%>

        <section id="myAttemptsView" class="quiz-view-section" hidden>

            <section class="student-attempts-card">

                <div class="student-attempts-header">
                    <h2>Submitted &amp; In-Progress Quizzes</h2>
                </div>

                <div class="student-attempt-list">

                    <asp:Repeater
                        ID="rptAttempts"
                        runat="server"
                        OnItemCommand="rptAttempts_ItemCommand">

                        <ItemTemplate>

                            <article class="student-attempt-row">

                                <%-- LEFT SIDE --%>
                                <div class="attempt-main">

                                    <span class='attempt-icon <%# GetAttemptIconCss(Eval("SubjectName")) %>'>
                                        <i class='<%# GetAttemptIcon(Eval("SubjectName")) %>'
                                           aria-hidden="true"></i>
                                    </span>

                                    <div class="attempt-info">

                                        <h3><%#: Eval("Title") %></h3>

                                        <p>
                                            Attempt #<%#: Eval("AttemptNumber") %>
                                            •
                                            <%#: GetAttemptSubtitle(Eval("AttemptStatus"), Eval("SubmittedAt")) %>
                                        </p>

                                    </div>

                                </div>


                                <%-- RESULT --%>
                                <div class="attempt-result">

                                    <strong class='<%# GetAttemptScoreCss(Eval("AttemptStatus")) %>'>

                                        <%#: GetAttemptScoreText(
                                            Eval("AttemptStatus"),
                                            Eval("AwardedMarks"),
                                            Eval("TotalMarks"),
                                            Eval("AnsweredQuestions"),
                                            Eval("TotalQuestions")
                                        ) %>

                                    </strong>

                                    <span class='<%# GetAttemptStatusCss(Eval("AttemptStatus")) %>'>

                                        <%#: GetAttemptStatusText(
                                            Eval("AttemptStatus"),
                                            Eval("AwardedMarks"),
                                            Eval("TotalMarks")
                                        ) %>

                                    </span>

                                </div>


                                <%-- ACTIONS --%>
                                <div class="attempt-actions">

                                    <a href='<%# GetAttemptActionUrl(
                                        Eval("AttemptId"),
                                        Eval("AttemptStatus")
                                    ) %>'
                                       class='<%# GetAttemptActionCss(
                                           Eval("AttemptStatus")
                                       ) %>'>

                                        <%#: GetAttemptActionText(
                                            Eval("AttemptStatus")
                                        ) %>

                                    </a>

                                    <asp:LinkButton
                                        ID="btnDeleteAttempt"
                                        runat="server"
                                        Visible='<%# CanDeleteAttempt(Eval("AttemptStatus")) %>'
                                        CssClass="attempt-delete-button"
                                        CommandName="DeleteAttempt"
                                        CommandArgument='<%# Eval("AttemptId") %>'
                                        CausesValidation="false"
                                        OnClientClick="return confirm('Delete this in-progress attempt? Your saved answers will also be removed.');">

                                        <i class="bi bi-trash" aria-hidden="true"></i>

                                    </asp:LinkButton>

                                </div>

                            </article>

                        </ItemTemplate>

                    </asp:Repeater>

                </div>

            </section>
            <%-- Empty Attemps --%>
            <asp:Panel
                ID="pnlNoAttempts"
                runat="server"
                Visible="false"
                CssClass="student-empty-state">
                <i class="bi bi-clipboard" aria-hidden="true"></i>
                <h3>No quiz attempts yet</h3>
                <p>Start a quiz from Explore Quizzes and your attempts will appear here.</p>
           </asp:Panel>
        </section>
        <%-- Join Session Modal --%>
        <div id="studentJoinModal" class="student-join-modal" hidden>
            <%-- OVERLAY --%>
            <div id="studentJoinOverlay" class="student-join-overlay"></div>
            <div class="student-join-dialog">
                <%-- MODAL HEADER --%>
                <div class="student-join-header">
                    <div class="student-join-title">
                        <span class="student-join-title-icon">
                            <i class="bi bi-key"></i>
                        </span>
                        <div>
                            <h2>Join Live Quiz Session</h2>
                            <p>Enter the code given by your lecturer.</p>
                        </div>
                    </div>
                    <button
                        type="button"
                        id="btnCloseJoinModal"
                        class="student-join-close">
                        <i class="bi bi-x-lg"></i>
                    </button>
                </div>
                <%-- Join Error --%>
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
                <%-- Join Code --%>
                <div class="join-code-field">
                    <label for="<%= txtJoinCode.ClientID %>">
                        Session Access Code
                        <span class="required-mark">*</span>
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
            <%-- Found Session --%>
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
                <%-- Hidden Session Data --%>
                <asp:HiddenField
                    ID="hfFoundSessionId"
                    runat="server" />
                <asp:HiddenField
                    ID="hfFoundQuizId"
                    runat="server" />
                <asp:HiddenField
                    ID="hfOpenJoinModal"
                    runat="server"
                    ClientIDMode="Static"
                    Value="0" />
                <%-- MODAL ACTIONS --%>
                <div class="student-join-actions">
                    <button type="button" id="btnCancelJoin" class="join-cancel-button">
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
              </div> <%-- END student-join-dialog --%>
            </div> <%-- END studentJoinModal --%>
</asp:Content>
<%-- =====================================================
     JAVASCRIPT
     ===================================================== --%>
<asp:Content ID="StudentQuizzesScripts" ContentPlaceHolderID="PortalScripts" runat="server">
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            const exploreTab = document.getElementById("btnExploreTab");
            const attemptsTab = document.getElementById("btnAttemptsTab");
            const exploreView = document.getElementById("exploreQuizzesView");
            const attemptsView = document.getElementById("myAttemptsView");
            const joinModal = document.getElementById("studentJoinModal");
            const joinButton = document.getElementById("btnJoinCode");
            const closeJoinButton = document.getElementById("btnCloseJoinModal");
            const cancelJoinButton = document.getElementById("btnCancelJoin");
            const joinOverlay = document.getElementById("studentJoinOverlay");
            const reopenJoinField = document.getElementById("hfOpenJoinModal");
            <%-- Open Join Modal --%>
            function openJoinModal() {
                joinModal.hidden = false;
                document.body.classList.add("modal-open");
            }
            <%-- Close Join Modal --%>
            function closeJoinModal() {
                joinModal.hidden = true;
                document.body.classList.remove("modal-open");
                if (reopenJoinField) {
                    reopenJoinField.value = "0";
                }
            }
            joinButton.addEventListener("click", openJoinModal);
            closeJoinButton.addEventListener("click", closeJoinModal);
            cancelJoinButton.addEventListener("click", closeJoinModal);
            joinOverlay.addEventListener("click", closeJoinModal);

            document.addEventListener("keydown", function (e) {
                if (e.key === "Escape" && !joinModal.hidden) {
                    closeJoinModal();
                }
            });
            // REOPEN AFTER SERVER POSTBACK
            if (reopenJoinField && reopenJoinField.value === "1") {
                openJoinModal();
            }
            <%-- Switch between Explore and Attempts --%>
            function showQuizView(viewName) {
                const showExplore = viewName === "explore";
                exploreView.hidden = !showExplore;
                attemptsView.hidden = showExplore;
                exploreTab.classList.toggle("active", showExplore);
                attemptsTab.classList.toggle("active", !showExplore);
            }
            const requestedTab =
                new URLSearchParams(window.location.search).get("tab");
            if (requestedTab === "attempts") {
                showQuizView("attempts");
            }
            else {
                showQuizView("explore");
            }
            exploreTab.addEventListener("click", function () {
                showQuizView("explore");
            });
            attemptsTab.addEventListener("click", function () {
                showQuizView("attempts");
            });
        });
    </script>
</asp:Content>