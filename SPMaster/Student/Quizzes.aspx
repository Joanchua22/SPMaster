<%@ Page Title="Quizzes"
    Language="C#"
    MasterPageFile="~/Master/Portal.Master"
    AutoEventWireup="true"
    CodeBehind="Quizzes.aspx.cs"
    Inherits="SPMaster.Student.Quizzes" %>


<asp:Content
    ID="StudentQuizzesHead"
    ContentPlaceHolderID="PortalHead"
    runat="server">

    <link href="<%= ResolveUrl("~/Content/StudentQuizzes.css") %>?v=2"
          rel="stylesheet"
          type="text/css" />

</asp:Content>



<asp:Content
    ID="StudentQuizzesContent"
    ContentPlaceHolderID="PortalContent"
    runat="server">

    <div class="student-quizzes-page">


        <%-- =====================================================
             PAGE HEADER
             ===================================================== --%>

        <section class="student-quizzes-header">

            <div class="student-quizzes-heading">

                <h1>
                    Quizzes
                </h1>

                <p>
                    Practice syllabus drills, past-year sets, or review your attempt scores
                </p>

            </div>


            <%-- TOP VIEW SWITCH --%>

            <div class="quiz-view-switch">

                <button type="button"
                        id="btnExploreTab"
                        class="quiz-view-tab active"
                        data-quiz-view="explore">

                    Explore Quizzes

                </button>


                <button type="button"
                        id="btnAttemptsTab"
                        class="quiz-view-tab"
                        data-quiz-view="attempts">

                    My Attempts

                    <span class="attempt-count">
                        (<asp:Literal
                            ID="litAttemptCount"
                            runat="server"
                            Text="0">
                        </asp:Literal>)
                    </span>

                </button>

            </div>

        </section>



        <%-- =====================================================
             EXPLORE QUIZZES VIEW
             ===================================================== --%>

        <section id="exploreQuizzesView"
                 class="quiz-view-section">


            <%-- =================================================
                 SEARCH AND FILTER
                 ================================================= --%>

            <section class="student-quiz-filter-card">


                <%-- SEARCH --%>

                <div class="student-quiz-search">

                    <i class="bi bi-search"
                       aria-hidden="true"></i>

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

                    <i class="bi bi-chevron-down"
                       aria-hidden="true"></i>

                </div>



                <%-- JOIN CODE --%>

                <button type="button"
                        id="btnJoinCode"
                        class="join-code-button">

                    <i class="bi bi-key"
                       aria-hidden="true"></i>

                    Join Code

                </button>

            </section>



            <%-- =================================================
                 QUIZ CARDS
                 ================================================= --%>

            <section class="student-quiz-grid">

                <asp:Repeater
                    ID="rptExploreQuizzes"
                    runat="server">

                    <ItemTemplate>

                        <article class="student-quiz-card">


                            <%-- TOP --%>

                            <div class="student-quiz-card-top">

                                <span class='student-quiz-subject <%# GetSubjectCss(Eval("SubjectName")) %>'>

                                    <%#: Eval("SubjectName") %>
                                    •
                                    <%#: Eval("FormName") %>

                                </span>


                                <%-- QUESTION FORMAT --%>

                                <span class="student-quiz-format">

                                    <%#: GetQuizFormat(
                                        Eval("McqCount"),
                                        Eval("SubjectiveCount")
                                    ) %>

                                </span>

                            </div>



                            <%-- CONTENT --%>

                            <div class="student-quiz-card-content">

                                <h2>
                                    <%#: Eval("Title") %>
                                </h2>

                                <p>
                                    <%#: GetQuizDescription(Eval("Description")) %>
                                </p>

                            </div>



                            <%-- META --%>

                            <div class="student-quiz-meta">

                                <span>
                                    <%# FormatQuestionCount(Eval("QuestionCount")) %>
                                </span>

                                <span>
                                    <%# FormatMarks(Eval("TotalMarks")) %>
                                </span>

                                <span>
                                    <%# FormatMinutes(Eval("TimeLimitMinutes")) %>
                                </span>

                            </div>



                            <%-- FOOTER --%>

                            <div class="student-quiz-card-footer">

                                <div class="student-quiz-teacher">

                                    <span class='teacher-avatar <%# GetTeacherAvatarCss(Eval("SubjectName")) %>'>

                                        <%#: GetInitials(Eval("CreatorName")) %>

                                    </span>

                                    <span>
                                        <%#: Eval("CreatorName") %>
                                    </span>

                                </div>


                                <a href='<%# ResolveUrl(
                                    "~/Student/ViewQuiz.aspx?id="
                                    + Eval("QuizId")) %>'
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

                <i class="bi bi-journal-x"
                   aria-hidden="true"></i>

                <h3>
                    No quizzes found
                </h3>

                <p>
                    Try changing your search or subject filter.
                </p>

            </asp:Panel>

        </section>



        <%-- =====================================================
             MY ATTEMPTS VIEW
             ===================================================== --%>

        <section id="myAttemptsView"
                 class="quiz-view-section"
                 hidden>


            <section class="student-attempts-card">

                <div class="student-attempts-header">

                    <h2>
                        Submitted &amp; In-Progress Quizzes
                    </h2>

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

                                        <h3>
                                            <%#: Eval("Title") %>
                                        </h3>


                                        <p>

                                            Attempt #
                                            <%#: Eval("AttemptNumber") %>

                                            •

                                            <%#: GetAttemptSubtitle(
                                                Eval("AttemptStatus"),
                                                Eval("SubmittedAt")
                                            ) %>

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



                                <%-- ACTION --%>

                                <div class="attempt-actions">

                                    <%-- CONTINUE / VIEW RESULT --%>

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


                                    <%-- DELETE - IN PROGRESS ONLY --%>

                                    <asp:LinkButton
                                        ID="btnDeleteAttempt"
                                        runat="server"
                                        Visible='<%# CanDeleteAttempt(
                                            Eval("AttemptStatus")
                                        ) %>'
                                        CssClass="attempt-delete-button"
                                        CommandName="DeleteAttempt"
                                        CommandArgument='<%# Eval("AttemptId") %>'
                                        CausesValidation="false"
                                        OnClientClick="return confirm('Delete this in-progress attempt? Your saved answers will also be removed.');">

                                        <i class="bi bi-trash"
                                           aria-hidden="true"></i>

                                    </asp:LinkButton>

                                </div>


                            </article>

                        </ItemTemplate>

                    </asp:Repeater>

                </div>

            </section>



            <%-- =================================================
                 EMPTY ATTEMPTS STATE
                 ================================================= --%>

            <asp:Panel
                ID="pnlNoAttempts"
                runat="server"
                Visible="false"
                CssClass="student-empty-state">

                <i class="bi bi-clipboard"
                   aria-hidden="true"></i>

                <h3>
                    No quiz attempts yet
                </h3>

                <p>
                    Start a quiz from Explore Quizzes and your attempts
                    will appear here.
                </p>

            </asp:Panel>

        </section>


    </div>

</asp:Content>



<asp:Content
    ID="StudentQuizzesScripts"
    ContentPlaceHolderID="PortalScripts"
    runat="server">

    <script>

        document.addEventListener(
            "DOMContentLoaded",
            function () {

                const exploreTab =
                    document.getElementById(
                        "btnExploreTab"
                    );

                const attemptsTab =
                    document.getElementById(
                        "btnAttemptsTab"
                    );

                const exploreView =
                    document.getElementById(
                        "exploreQuizzesView"
                    );

                const attemptsView =
                    document.getElementById(
                        "myAttemptsView"
                    );


                function showQuizView(viewName) {

                    const showExplore =
                        viewName === "explore";


                    exploreView.hidden =
                        !showExplore;


                    attemptsView.hidden =
                        showExplore;


                    exploreTab.classList.toggle(
                        "active",
                        showExplore
                    );


                    attemptsTab.classList.toggle(
                        "active",
                        !showExplore
                    );
                }


                const requestedTab =
                    new URLSearchParams(
                        window.location.search
                    ).get("tab");


                if (
                    requestedTab === "attempts"
                ) {
                    showQuizView(
                        "attempts"
                    );
                }
                else {
                    showQuizView(
                        "explore"
                    );
                }


                exploreTab.addEventListener(
                    "click",
                    function () {

                        showQuizView(
                            "explore"
                        );

                    }
                );


                attemptsTab.addEventListener(
                    "click",
                    function () {

                        showQuizView(
                            "attempts"
                        );

                    }
                );

            }
        );

    </script>

</asp:Content>