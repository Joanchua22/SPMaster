<%@ Page Title="Quiz Management"
    Language="C#"
    MasterPageFile="~/Master/Portal.Master"
    AutoEventWireup="true"
    CodeBehind="Quizzes.aspx.cs"
    Inherits="SPMaster.Lecturer.Quizzes" %>


<asp:Content
    ID="LecturerQuizzesHead"
    ContentPlaceHolderID="PortalHead"
    runat="server">

    <link href="<%= ResolveUrl("~/Content/LecturerQuizzes.css") %>?v=3  "
          rel="stylesheet"
          type="text/css" />

</asp:Content>



<asp:Content
    ID="LecturerQuizzesContent"
    ContentPlaceHolderID="PortalContent"
    runat="server">

    <div class="lecturer-quizzes-page">


        <%-- =====================================================
             PAGE HEADER
             ===================================================== --%>

        <section class="quiz-page-header">

            <div>

                <h1>
                    Quizzes
                </h1>

                <p>
                    Construct exam sets, organize assessments, and launch cohorts
                </p>

            </div>


            <a href="<%= ResolveUrl("~/Lecturer/CreateQuiz.aspx") %>"
               class="create-quiz-button">

                <i class="bi bi-plus-circle"
                   aria-hidden="true"></i>

                Create New Quiz

            </a>

        </section>

        <%-- =====================================================
             SEARCH
             ===================================================== --%>

        <section class="quiz-search-card">

            <div class="quiz-search">

                <i class="bi bi-search quiz-search-icon"
                   aria-hidden="true"></i>

                <asp:TextBox
                    ID="txtSearch"
                    runat="server"
                    CssClass="quiz-search-input"
                    AutoPostBack="true"
                    OnTextChanged="FilterChanged"
                    placeholder="Search by quiz title or keyword..."
                    aria-label="Search quizzes">
                </asp:TextBox>

            </div>

        </section>



        <%-- =====================================================
             QUIZ TABS
             ===================================================== --%>

        <div class="quiz-tabs">

            <asp:LinkButton
                ID="btnMyQuizzes"
                runat="server"
                CssClass="quiz-tab active"
                OnClick="btnMyQuizzes_Click">
            </asp:LinkButton>


            <asp:LinkButton
                ID="btnPublicLibrary"
                runat="server"
                CssClass="quiz-tab"
                OnClick="btnPublicLibrary_Click">
            </asp:LinkButton>

        </div>



        <%-- =====================================================
             QUIZ LIST
             ===================================================== --%>

        <section class="quiz-card-grid">


            <asp:Repeater
                ID="rptQuizzes"
                runat="server"
                OnItemCommand="rptQuizzes_ItemCommand">

                <ItemTemplate>


                    <article class="quiz-management-card">


                        <%-- =========================
                             CARD TOP
                             ========================= --%>

                        <div class="quiz-card-top">

                            <div class="quiz-card-tags">

                                <span class='quiz-subject-tag <%# GetSubjectCss(Eval("SubjectName")) %>'>
                                    <%#: Eval("SubjectName") %>
                                </span>


                                <span class="quiz-form-tag">
                                    <%#: Eval("FormName") %>
                                </span>

                            </div>


                            <span class='quiz-status-tag <%# GetStatusCss(Eval("IsPublished")) %>'>

                                <%#: GetStatusText(Eval("IsPublished")) %>

                            </span>

                        </div>



                        <%-- =========================
                             QUIZ INFORMATION
                             ========================= --%>

                        <div class="quiz-card-content">


                            <h2>
                                <%#: Eval("Title") %>
                            </h2>


                            <p class="quiz-description">

                                <%#: GetDescription(Eval("Description")) %>

                            </p>


                            <div class="quiz-information">


                                <span>
                                    <%#: Eval("QuestionCount") %>
                                    Questions
                                </span>


                                <span class="quiz-info-dot">
                                    •
                                </span>


                                <span>
                                    <%#: Eval("TotalMarks") %>
                                    Total Marks
                                </span>


                                <span class="quiz-info-dot">
                                    •
                                </span>


                                <span>
                                    <%#: Eval("TimeLimitMinutes") %>
                                    Mins
                                </span>


                            </div>


                        </div>



                        <%-- =========================
                             CARD ACTIONS
                             ========================= --%>

                        <div class="quiz-card-actions">


                            <%-- =================================
                                 MY QUIZ
                                 ================================= --%>

                            <asp:PlaceHolder
                                runat="server"
                                Visible='<%# Convert.ToBoolean(Eval("IsOwner")) %>'>


                                <%-- PUBLISHED --%>

                                <asp:PlaceHolder
                                    runat="server"
                                    Visible='<%# Convert.ToBoolean(Eval("IsPublished")) %>'>


                                    <a href='<%# ResolveUrl(
                                        "~/Lecturer/Sessions.aspx?quizId="
                                        + Eval("QuizId")) %>'
                                       class="quiz-action-button quiz-action-primary">

                                        Create Session

                                    </a>


                                    <a href='<%# ResolveUrl(
                                        "~/Lecturer/PreviewQuiz.aspx?id="
                                        + Eval("QuizId")
                                        + "&from=mine") %>'
                                       class="quiz-action-button quiz-action-light">

                                        Preview

                                    </a>


                                    <asp:LinkButton
                                        runat="server"
                                        CssClass="quiz-action-button quiz-action-light"
                                        CommandName="ArchiveQuiz"
                                        CommandArgument='<%# Eval("QuizId") %>'
                                        OnClientClick="return confirm('Archive this quiz?');">

                                        Archive

                                    </asp:LinkButton>


                                </asp:PlaceHolder>



                                <%-- DRAFT --%>

                                <asp:PlaceHolder
                                    runat="server"
                                    Visible='<%# !Convert.ToBoolean(Eval("IsPublished")) %>'>


                                    <a href='<%# ResolveUrl(
                                        "~/Lecturer/CreateQuiz.aspx?id="
                                        + Eval("QuizId")) %>'
                                       class="quiz-action-button quiz-action-primary">

                                        Edit Draft

                                    </a>


                                    <a href='<%# ResolveUrl(
                                        "~/Lecturer/PreviewQuiz.aspx?id="
                                        + Eval("QuizId")
                                        + "&from=mine") %>'
                                       class="quiz-action-button quiz-action-light">

                                        Preview

                                    </a>


                                    <asp:LinkButton
                                        runat="server"
                                        CssClass="quiz-action-button quiz-action-danger"
                                        CommandName="DeleteQuiz"
                                        CommandArgument='<%# Eval("QuizId") %>'
                                        OnClientClick="return confirm('Permanently delete this draft quiz?');">

                                        Delete

                                    </asp:LinkButton>


                                </asp:PlaceHolder>


                            </asp:PlaceHolder>



                            <%-- =================================
                                 PUBLIC LIBRARY QUIZ
                                 ================================= --%>

                            <asp:PlaceHolder
                                runat="server"
                                Visible='<%# !Convert.ToBoolean(Eval("IsOwner")) %>'>


                                <a href='<%# ResolveUrl(
                                    "~/Lecturer/Sessions.aspx?quizId="
                                    + Eval("QuizId")) %>'
                                   class="quiz-action-button quiz-action-primary">

                                    Host My Session

                                </a>


                                <a href='<%# ResolveUrl(
                                    "~/Lecturer/PreviewQuiz.aspx?id="
                                    + Eval("QuizId")
                                    + "&from=public") %>'
                                   class="quiz-action-button quiz-action-light">

                                    Preview

                                </a>


                            </asp:PlaceHolder>


                        </div>


                    </article>


                </ItemTemplate>

            </asp:Repeater>



            <%-- =================================================
                 NO QUIZZES
                 ================================================= --%>

            <asp:Panel
                ID="pnlNoQuizzes"
                runat="server"
                Visible="false">

                <article class="quiz-management-card">

                    <div class="quiz-card-content">

                        <h2>
                            No quizzes found.
                        </h2>

                        <p class="quiz-description">

                            No quizzes are currently available in this section.

                        </p>

                    </div>

                </article>

            </asp:Panel>


        </section>


    </div>

</asp:Content>



<asp:Content
    ID="LecturerQuizzesScripts"
    ContentPlaceHolderID="PortalScripts"
    runat="server">

</asp:Content>