<%@ Page Title="Quiz Preview"
    Language="C#"
    MasterPageFile="~/Master/Portal.Master"
    AutoEventWireup="true"
    CodeBehind="PreviewQuiz.aspx.cs"
    Inherits="SPMaster.Lecturer.PreviewQuiz" %>


<asp:Content
    ID="PreviewQuizHead"
    ContentPlaceHolderID="PortalHead"
    runat="server">

    <link href="<%= ResolveUrl("~/Content/LecturerPreviewQuiz.css") %>?v=1"
          rel="stylesheet"
          type="text/css" />

</asp:Content>



<asp:Content
    ID="PreviewQuizContent"
    ContentPlaceHolderID="PortalContent"
    runat="server">

    <div class="quiz-preview-page">


        <%-- =========================
             PAGE HEADER
             ========================= --%>

        <section class="quiz-preview-header">


            <%-- LEFT SIDE --%>

            <div class="quiz-preview-heading-group">

                <a href="<%= ResolveUrl("~/Lecturer/CreateQuiz.aspx") %>"
                   class="quiz-preview-back-button"
                   aria-label="Back to Quiz Editor">

                    <i class="bi bi-arrow-left"
                       aria-hidden="true"></i>

                </a>


                <div class="quiz-preview-heading">

                    <span class="quiz-preview-context">
                        Sejarah • Form 4
                    </span>

                    <h1>
                        Sejarah Kertas 2: Kedaulatan Negara
                    </h1>

                </div>

            </div>



            <%-- PUBLISH BUTTON --%>

            <button type="button"
                    class="publish-quiz-button">

                Publish Quiz Now

            </button>

        </section>



        <%-- =========================
             QUIZ SUMMARY
             ========================= --%>

        <section class="quiz-preview-summary">

            <div class="quiz-summary-item">

                <strong>
                    Total Questions:
                </strong>

                <span>
                    2
                </span>

            </div>


            <span class="quiz-summary-dot">
                •
            </span>


            <div class="quiz-summary-item">

                <strong>
                    Total Marks:
                </strong>

                <span>
                    8 Marks
                </span>

            </div>


            <span class="quiz-summary-dot">
                •
            </span>


            <div class="quiz-summary-item">

                <strong>
                    Time Limit:
                </strong>

                <span>
                    60 mins
                </span>

            </div>

        </section>



        <%-- =========================
             QUESTION LIST
             ========================= --%>

        <section class="quiz-preview-question-list">


            <%-- =====================================
                 QUESTION 1 - MCQ
                 ===================================== --%>

            <article class="quiz-preview-question-card">


                <%-- QUESTION HEADER --%>

                <div class="preview-question-header">

                    <span class="preview-question-number">

                        Question 1 of 2
                        (Single Choice)

                    </span>


                    <span class="preview-question-marks">
                        4 Marks
                    </span>

                </div>



                <%-- QUESTION TEXT --%>

                <h2 class="preview-question-title">

                    Apakah faktor utama yang mendorong pembentukan
                    Jawatankuasa Setia Kawan Malaysia (JSKM)
                    pada Julai 1961?

                </h2>



                <%-- MCQ OPTIONS --%>

                <div class="preview-option-grid">


                    <%-- OPTION A --%>

                    <div class="preview-option">

                        <span class="preview-option-letter">
                            A.
                        </span>

                        <span>
                            Menggubal perlembagaan baharu
                        </span>

                    </div>



                    <%-- OPTION B - CORRECT --%>

                    <div class="preview-option">

                        <span class="preview-option-letter">
                            B.
                        </span>

                        <span>
                            Meyakinkan penduduk Sabah &amp; Sarawak
                        </span>

                    </div>



                    <%-- OPTION C --%>

                    <div class="preview-option">

                        <span class="preview-option-letter">
                            C.
                        </span>

                        <span>
                            Membubarkan Majlis Perundangan Singapura
                        </span>

                    </div>



                    <%-- OPTION D --%>

                    <div class="preview-option">

                        <span class="preview-option-letter">
                            D.
                        </span>

                        <span>
                            Menyerahkan kuasa pertahanan kepada British
                        </span>

                    </div>

                </div>

            </article>



            <%-- =====================================
                 QUESTION 2 - SUBJECTIVE
                 ===================================== --%>

            <article class="quiz-preview-question-card">


                <%-- QUESTION HEADER --%>

                <div class="preview-question-header">

                    <span class="preview-question-number">

                        Question 2 of 2
                        (Subjective)

                    </span>


                    <span class="preview-question-marks">
                        4 Marks
                    </span>

                </div>



                <%-- QUESTION TEXT --%>

                <h2 class="preview-question-title">

                    Jelaskan dua faktor pembentukan
                    Gagasan Malaysia pada tahun 1961.

                </h2>

            </article>

        </section>

    </div>

</asp:Content>



<asp:Content
    ID="PreviewQuizScripts"
    ContentPlaceHolderID="PortalScripts"
    runat="server">

</asp:Content>