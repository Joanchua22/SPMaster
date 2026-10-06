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

    <link href="<%= ResolveUrl("~/Content/LecturerQuizzes.css") %>?v=1"
          rel="stylesheet"
          type="text/css" />

</asp:Content>



<asp:Content
    ID="LecturerQuizzesContent"
    ContentPlaceHolderID="PortalContent"
    runat="server">

    <div class="lecturer-quizzes-page">


        <%-- =========================
             PAGE HEADER
             ========================= --%>

        <section class="quiz-page-header">

            <div>

                <h1>
                    Quiz Management
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



        <%-- =========================
             QUIZ TABS
             ========================= --%>

        <div class="quiz-tabs">

            <button type="button"
                    class="quiz-tab active">

                My Quizzes (8)

            </button>


            <button type="button"
                    class="quiz-tab">

                Public Quiz Library (24)

            </button>

        </div>



        <%-- =========================
             QUIZ LIST
             ========================= --%>

        <section class="quiz-card-grid">


            <%-- =====================================
                 QUIZ 1
                 MATHEMATICS - PUBLISHED
                 ===================================== --%>

            <article class="quiz-management-card">


                <%-- CARD TOP --%>

                <div class="quiz-card-top">

                    <span class="quiz-subject-tag subject-purple">
                        Mathematics
                    </span>


                    <span class="quiz-status-tag status-published">
                        Published
                    </span>

                </div>



                <%-- QUIZ INFORMATION --%>

                <div class="quiz-card-content">

                    <h2>
                        Mathematics Final-Year Practice
                    </h2>


                    <p class="quiz-description">

                        Full SPM Paper 1 &amp; 2 preparation with quadratics,
                        matrices, and coordinate geometry.

                    </p>


                    <div class="quiz-information">

                        <span>
                            25 Questions
                        </span>

                        <span class="quiz-info-dot">
                            •
                        </span>

                        <span>
                            50 Total Marks
                        </span>

                        <span class="quiz-info-dot">
                            •
                        </span>

                        <span>
                            45 Mins
                        </span>

                    </div>

                </div>



                <%-- CARD ACTIONS --%>

                <div class="quiz-card-actions">

                    <a href="<%= ResolveUrl("~/Lecturer/Sessions.aspx") %>"
                       class="quiz-action-button quiz-action-primary">

                        Create Session

                    </a>


                    <button type="button"
                            class="quiz-action-button quiz-action-light">

                        Preview

                    </button>

                </div>

            </article>



            <%-- =====================================
                 QUIZ 2
                 SEJARAH - PUBLISHED
                 ===================================== --%>

            <article class="quiz-management-card">


                <%-- CARD TOP --%>

                <div class="quiz-card-top">

                    <span class="quiz-subject-tag subject-yellow">
                        Sejarah
                    </span>


                    <span class="quiz-status-tag status-published">
                        Published
                    </span>

                </div>



                <%-- QUIZ INFORMATION --%>

                <div class="quiz-card-content">

                    <h2>
                        Sejarah Kertas 2: Kedaulatan Negara
                    </h2>


                    <p class="quiz-description">

                        Focuses on Form 4 and Form 5 national sovereignty topics
                        with subjective evaluation criteria.

                    </p>


                    <div class="quiz-information">

                        <span>
                            10 Questions
                        </span>

                        <span class="quiz-info-dot">
                            •
                        </span>

                        <span>
                            40 Total Marks
                        </span>

                        <span class="quiz-info-dot">
                            •
                        </span>

                        <span>
                            60 Mins
                        </span>

                    </div>

                </div>



                <%-- CARD ACTIONS --%>

                <div class="quiz-card-actions">

                    <a href="<%= ResolveUrl("~/Lecturer/Sessions.aspx") %>"
                       class="quiz-action-button quiz-action-primary">

                        Create Session

                    </a>


                    <button type="button"
                            class="quiz-action-button quiz-action-light">

                        Preview

                    </button>


                    <button type="button"
                            class="quiz-action-button quiz-action-light">

                        Archive

                    </button>

                </div>

            </article>



            <%-- =====================================
                 QUIZ 3
                 BAHASA MELAYU - DRAFT
                 ===================================== --%>

            <article class="quiz-management-card">


                <%-- CARD TOP --%>

                <div class="quiz-card-top">

                    <span class="quiz-subject-tag subject-blue">
                        Bahasa Melayu
                    </span>


                    <span class="quiz-status-tag status-draft">
                        Draft
                    </span>

                </div>



                <%-- QUIZ INFORMATION --%>

                <div class="quiz-card-content">

                    <h2>
                        Bahasa Melayu Sintaksis Kilat
                    </h2>


                    <p class="quiz-description">

                        Draft exercise for accelerated Form 5 grammar drills.
                        Still needs 5 more questions.

                    </p>


                    <div class="quiz-information">

                        <span>
                            15 Questions
                        </span>

                        <span class="quiz-info-dot">
                            •
                        </span>

                        <span>
                            30 Total Marks
                        </span>

                        <span class="quiz-info-dot">
                            •
                        </span>

                        <span>
                            30 Mins
                        </span>

                    </div>

                </div>



                <%-- CARD ACTIONS --%>

                <div class="quiz-card-actions">
                        
                    <a href="<%= ResolveUrl("~/Lecturer/CreateQuiz.aspx") %>"
                       class="quiz-action-button quiz-action-primary">

                        Edit Draft

                    </a>


                    <button type="button"
                            class="quiz-action-button quiz-action-light">

                        Preview

                    </button>


                    <button type="button"
                            class="quiz-action-button quiz-action-danger">

                        Delete

                    </button>

                </div>

            </article>

        </section>

    </div>

</asp:Content>



<asp:Content
    ID="LecturerQuizzesScripts"
    ContentPlaceHolderID="PortalScripts"
    runat="server">

</asp:Content>