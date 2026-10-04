<%@ Page Title="Question Bank"
    Language="C#"
    MasterPageFile="~/Master/Portal.Master"
    AutoEventWireup="true"
    CodeBehind="QuestionBank.aspx.cs"
    Inherits="SPMaster.Lecturer.QuestionBank" %>


<asp:Content
    ID="QuestionBankHead"
    ContentPlaceHolderID="PortalHead"
    runat="server">

    <link href="<%= ResolveUrl("~/Content/QuestionBank.css") %>?v=2"
          rel="stylesheet"
          type="text/css" />

    <link href="<%= ResolveUrl("~/Content/QuestionPreview.css") %>?v=1"
          rel="stylesheet"
          type="text/css" />

</asp:Content>


<asp:Content
    ID="QuestionBankContent"
    ContentPlaceHolderID="PortalContent"
    runat="server">

    <div class="question-bank-page">


        <%-- =========================
             PAGE HEADER
             ========================= --%>

        <section class="question-bank-header">

            <div>

                <h1>
                    Question Bank
                </h1>

                <p>
                    Browse, create, and organize SPM curriculum questions
                </p>

            </div>


            <a href="<%= ResolveUrl("~/Lecturer/CreateQuestion.aspx") %>"
               class="add-question-button">

                <i class="bi bi-plus-circle"
                   aria-hidden="true"></i>

                Add New Question

            </a>

        </section>



        <%-- =========================
             SEARCH AND FILTERS
             ========================= --%>

        <section class="question-filter-card">


            <%-- SEARCH --%>

            <div class="question-search">

                <i class="bi bi-search search-icon"
                   aria-hidden="true"></i>

                <input type="text"
                       placeholder="Search by question text or keyword..."
                       aria-label="Search questions" />

            </div>



            <%-- SUBJECT FILTER --%>

            <div class="question-filter-wrapper">

                <select class="question-filter"
                        aria-label="Filter by subject">

                    <option>
                        All Subjects
                    </option>

                    <option>
                        Mathematics
                    </option>

                    <option>
                        Sejarah
                    </option>

                    <option>
                        English
                    </option>

                    <option>
                        Bahasa Melayu
                    </option>

                    <option>
                        Science
                    </option>

                </select>


                <i class="bi bi-caret-down-fill filter-arrow"
                   aria-hidden="true"></i>

            </div>



            <%-- QUESTION TYPE FILTER --%>

            <div class="question-filter-wrapper">

                <select class="question-filter"
                        aria-label="Filter by question type">

                    <option>
                        All Types
                    </option>

                    <option>
                        MCQ
                    </option>

                    <option>
                        Subjective
                    </option>

                </select>


                <i class="bi bi-caret-down-fill filter-arrow"
                   aria-hidden="true"></i>

            </div>



            <%-- DIFFICULTY FILTER --%>

            <div class="question-filter-wrapper">

                <select class="question-filter"
                        aria-label="Filter by difficulty">

                    <option>
                        All Difficulties
                    </option>

                    <option>
                        Easy
                    </option>

                    <option>
                        Medium
                    </option>

                    <option>
                        Hard
                    </option>

                </select>


                <i class="bi bi-caret-down-fill filter-arrow"
                   aria-hidden="true"></i>

            </div>

        </section>



        <%-- =========================
             QUESTION BANK TABS
             ========================= --%>

        <div class="question-tabs">

            <button type="button"
                    class="question-tab active">

                My Questions (42)

            </button>


            <button type="button"
                    class="question-tab">

                Shared Faculty Library (128)

            </button>

        </div>



        <%-- =========================
             QUESTION LIST
             ========================= --%>

        <section class="question-list">


            <%-- =====================================
                 QUESTION 1 - CREATED BY CURRENT USER
                 ===================================== --%>

            <article class="question-card">

                <div class="question-card-top">

                    <div class="question-tags">

                        <span class="question-tag subject-tag">
                            Mathematics
                        </span>

                        <span class="question-tag difficulty-medium">
                            Medium
                        </span>

                        <span class="question-tag type-tag">
                            Single Choice
                        </span>

                        <span class="question-owner">
                            Created by You (Mr. Rahman)
                        </span>

                    </div>


                    <div class="question-actions">

                        <button type="button"
                                class="question-action">

                            Edit

                        </button>


                        <button type="button"
                                class="question-action">

                            Archive

                        </button>


                        <button type="button"
                                class="question-action question-action-danger">

                            Delete

                        </button>

                    </div>

                </div>


                <h2 class="question-text">

                    Diberi persamaan kuadratik
                    2x² - 5x + c = 0 mempunyai dua punca nyata
                    yang sama. Cari nilai pemalar c.

                </h2>


                <div class="question-meta">

                    <span>
                        Correct Answer: Option C (25/8)
                    </span>

                    <span class="meta-dot">
                        •
                    </span>

                    <span>
                        Used in 2 Quizzes
                    </span>

                </div>

            </article>



            <%-- =====================================
                 QUESTION 2 - CREATED BY CURRENT USER
                 ===================================== --%>

            <article class="question-card">

                <div class="question-card-top">

                    <div class="question-tags">

                        <span class="question-tag subject-tag subject-yellow">
                            Sejarah
                        </span>

                        <span class="question-tag difficulty-hard">
                            Hard
                        </span>

                        <span class="question-tag type-tag">
                            Subjective
                        </span>

                        <span class="question-tag media-tag">

                            <i class="bi bi-image"
                               aria-hidden="true"></i>

                            Image attached

                        </span>

                        <span class="question-owner">
                            Created by You (Mr. Rahman)
                        </span>

                    </div>


                    <div class="question-actions">

                        <button type="button"
                                class="question-action">

                            Edit

                        </button>


                        <button type="button"
                                class="question-action">

                            Archive

                        </button>


                        <button type="button"
                                class="question-action question-action-danger">

                            Delete

                        </button>

                    </div>

                </div>


                <h2 class="question-text">

                    Berdasarkan rajah piagam perjanjian
                    persekutuan yang diberi, jelaskan dua faktor
                    pembentukan Gagasan Malaysia pada tahun 1961.

                </h2>


                <div class="question-meta">

                    <span>
                        Model scoring key provided
                    </span>

                    <span class="meta-dot">
                        •
                    </span>

                    <span>
                        Active in "Sejarah Kertas 2"
                    </span>

                </div>

            </article>



            <%-- =====================================
                 QUESTION 3 - SHARED QUESTION
                 ===================================== --%>

            <article class="question-card">

                <div class="question-card-top">

                    <div class="question-tags">

                        <span class="question-tag subject-tag subject-blue">
                            English
                        </span>

                        <span class="question-tag difficulty-easy">
                            Easy
                        </span>

                        <span class="question-tag type-tag">
                            Single Choice
                        </span>

                        <span class="question-owner">
                            Contributed by Ms. Tan (English Dept)
                        </span>

                    </div>


                    <div class="question-actions">

                        <%-- Preview is available because
                             this question belongs to another lecturer. --%>

                        <button type="button"
                                class="question-action preview-question-button"

                                data-preview-question

                                data-subject="English"

                                data-difficulty="Easy"

                                data-type="MCQ"

                                data-question="Identify the most suitable synonym for the word &quot;resilient&quot; in paragraph 3 of the SPM Reading passage."

                                data-option-a="Strong"

                                data-option-b="Adaptable &amp; Tough"

                                data-option-c="Careless"

                                data-option-d="Weak"

                                data-correct-option="B">

                            Preview

                        </button>


                        <button type="button"
                                class="question-action question-action-primary">

                            Add to My Quiz

                        </button>

                    </div>

                </div>


                <h2 class="question-text">

                    Identify the most suitable synonym for the word
                    "resilient" in paragraph 3 of the SPM Reading passage.

                </h2>


                <div class="question-meta">

                    <span>
                        Correct Answer:
                        Option B (Adaptable &amp; Tough)
                    </span>

                    <span class="meta-dot">
                        •
                    </span>

                    <span>
                        Shared across SMK network
                    </span>

                </div>

            </article>

        </section>

    </div>



    <%-- =========================================================
         QUESTION PREVIEW MODAL
         Reused with QuestionPreview.css / QuestionPreview.js
         ========================================================= --%>

    <div id="questionPreviewModal"
         class="question-preview-modal"
         hidden>


        <%-- OVERLAY --%>

        <div id="previewOverlay"
             class="question-preview-overlay"
             data-preview-close>
        </div>



        <%-- MODAL DIALOG --%>

        <div class="question-preview-dialog"
             role="dialog"
             aria-modal="true"
             aria-labelledby="previewTitle">


            <%-- HEADER --%>

            <div class="question-preview-header">

                <div>

                    <span class="preview-label">
                        Question Preview
                    </span>

                    <h2 id="previewTitle">
                        Student View
                    </h2>

                </div>


                <button type="button"
                        id="btnClosePreview"
                        class="preview-close-button"
                        data-preview-close
                        aria-label="Close preview">

                    <i class="bi bi-x-lg"
                       aria-hidden="true"></i>

                </button>

            </div>



            <%-- BODY --%>

            <div class="question-preview-body">


                <%-- TAGS --%>

                <div class="preview-tags">

                    <span id="previewSubject"
                          class="preview-tag">
                    </span>

                    <span id="previewDifficulty"
                          class="preview-tag">
                    </span>

                    <span id="previewType"
                          class="preview-tag">
                    </span>

                </div>



                <%-- QUESTION TEXT --%>

                <h3 id="previewQuestionText"
                    class="preview-question-text">
                </h3>



                <%-- OPTIONAL IMAGE --%>

                <div id="previewImageArea"
                     class="preview-image-area"
                     hidden>

                    <img id="previewImage"
                         src=""
                         alt="Question stimulus preview" />

                </div>



                <%-- MCQ ANSWERS --%>

                <div id="previewMcqAnswers"
                     class="preview-answer-list">

                    <div class="preview-answer"
                         data-preview-option="A">

                        <span>
                            A
                        </span>

                        <p id="previewOptionA">
                        </p>

                    </div>


                    <div class="preview-answer"
                         data-preview-option="B">

                        <span>
                            B
                        </span>

                        <p id="previewOptionB">
                        </p>

                    </div>


                    <div class="preview-answer"
                         data-preview-option="C">

                        <span>
                            C
                        </span>

                        <p id="previewOptionC">
                        </p>

                    </div>


                    <div class="preview-answer"
                         data-preview-option="D">

                        <span>
                            D
                        </span>

                        <p id="previewOptionD">
                        </p>

                    </div>

                </div>



                <%-- SUBJECTIVE MODEL ANSWER --%>

                <div id="previewSubjectiveAnswer"
                     class="preview-subjective"
                     hidden>

                    <label>
                        Model Answer / Scoring Guide
                    </label>

                    <p id="previewModelAnswer">
                    </p>

                </div>

            </div>

        </div>

    </div>

</asp:Content>



<asp:Content
    ID="QuestionBankScripts"
    ContentPlaceHolderID="PortalScripts"
    runat="server">

    <script src="<%= ResolveUrl("~/Scripts/QuestionPreview.js") %>?v=1">
    </script>

</asp:Content>