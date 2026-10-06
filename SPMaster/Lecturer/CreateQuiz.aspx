<%@ Page Title="Quiz Editor"
    Language="C#"
    MasterPageFile="~/Master/Portal.Master"
    AutoEventWireup="true"
    CodeBehind="CreateQuiz.aspx.cs"
    Inherits="SPMaster.Lecturer.CreateQuiz" %>


<asp:Content
    ID="CreateQuizHead"
    ContentPlaceHolderID="PortalHead"
    runat="server">

    <link href="<%= ResolveUrl("~/Content/LecturerCreateQuiz.css") %>?v=1"
          rel="stylesheet"
          type="text/css" />

</asp:Content>



<asp:Content
    ID="CreateQuizContent"
    ContentPlaceHolderID="PortalContent"
    runat="server">

    <div class="create-quiz-page">


        <%-- =========================
             PAGE HEADER
             ========================= --%>

        <section class="create-quiz-header">

            <div>

                <h1>
                    Quiz Editor
                </h1>

                <p>
                    Define quiz parameters and curate questions from your repository
                </p>

            </div>


            <div class="create-quiz-header-actions">

                <a href="<%= ResolveUrl("~/Lecturer/Quizzes.aspx") %>"
                   class="create-quiz-cancel-button">

                    Cancel

                </a>


                <a href="<%= ResolveUrl("~/Lecturer/PreviewQuiz.aspx") %>"
                   class="create-quiz-publish-button">

                    Preview &amp; Publish

                    <i class="bi bi-arrow-right"
                       aria-hidden="true"></i>

                </a>

            </div>

        </section>



        <%-- =====================================================
             1. GENERAL INFORMATION
             ===================================================== --%>

        <section class="create-quiz-card">

            <div class="create-quiz-section-heading">

                <h2>
                    1. General Information
                </h2>

            </div>


            <div class="quiz-general-grid">


                <%-- QUIZ TITLE --%>

                <div class="quiz-field quiz-field-wide">

                    <label for="txtQuizTitle">

                        Quiz Title

                        <span class="required-mark">
                            *
                        </span>

                    </label>


                    <input type="text"
                           id="txtQuizTitle"
                           class="quiz-input"
                           value="Sejarah Kertas 2: Kedaulatan Negara"
                           placeholder="Enter quiz title" />

                </div>



                <%-- SUBJECT --%>

                <div class="quiz-field">

                    <label for="ddlQuizSubject">

                        Subject

                        <span class="required-mark">
                            *
                        </span>

                    </label>


                    <div class="quiz-select-wrapper">

                        <select id="ddlQuizSubject"
                                class="quiz-select">

                            <option>
                                Mathematics
                            </option>

                            <option selected>
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


                        <i class="bi bi-caret-down-fill quiz-select-arrow"
                           aria-hidden="true"></i>

                    </div>

                </div>



                <%-- FORM LEVEL --%>

                <div class="quiz-field">

                    <label for="ddlFormLevel">

                        Form Level

                        <span class="required-mark">
                            *
                        </span>

                    </label>


                    <div class="quiz-select-wrapper">

                        <select id="ddlFormLevel"
                                class="quiz-select">

                            <option>
                                Form 4
                            </option>

                            <option selected>
                                Form 5
                            </option>

                        </select>


                        <i class="bi bi-caret-down-fill quiz-select-arrow"
                           aria-hidden="true"></i>

                    </div>

                </div>



                <%-- TIME LIMIT --%>

                <div class="quiz-field">

                    <label for="txtTimeLimit">

                        Time Limit (Minutes)

                        <span class="required-mark">
                            *
                        </span>

                    </label>


                    <input type="number"
                           id="txtTimeLimit"
                           class="quiz-input"
                           value="60"
                           min="1" />

                </div>



                <%-- PASS PERCENTAGE --%>

                <div class="quiz-field">

                    <label for="txtPassPercentage">

                        Pass Percentage

                        <span class="required-mark">
                            *
                        </span>

                    </label>


                    <div class="quiz-percentage-input">

                        <input type="number"
                               id="txtPassPercentage"
                               class="quiz-input"
                               value="50"
                               min="0"
                               max="100" />

                        <span>
                            %
                        </span>

                    </div>

                </div>



                <%-- INSTRUCTIONS --%>

                <div class="quiz-field quiz-field-instructions">

                    <label for="txtQuizInstructions">
                        Instructions for Students
                    </label>


                    <input type="text"
                           id="txtQuizInstructions"
                           class="quiz-input"
                           value="Jawab semua soalan objektif dan berikan penjelasan bertulis bagi soalan subjektif."
                           placeholder="Enter instructions for students" />

                </div>

            </div>

        </section>



        <%-- =====================================================
             2. QUESTIONS ALLOCATION
             ===================================================== --%>

        <section class="create-quiz-card questions-allocation-card">


            <%-- SECTION HEADER --%>

            <div class="question-allocation-header">

                <div>

                    <h2>
                        2. Questions Allocation
                    </h2>

                    <p>
                        Reorder items, designate mark weightage, and manage structure
                    </p>

                </div>


                <div class="question-allocation-actions">

                    <span class="quiz-summary-pill">
                        3 Questions • 12 Total Marks
                    </span>


                    <button type="button"
                            id="btnOpenQuestionBank"
                            class="add-from-bank-button">

                        <i class="bi bi-plus-circle"
                           aria-hidden="true"></i>

                        Add from Bank

                    </button>

                </div>

            </div>



            <%-- =========================
                 QUESTION 1
                 ========================= --%>

            <article class="allocated-question-row">

                <div class="question-order">
                    1
                </div>


                <div class="allocated-question-content">

                    <div class="allocated-question-tags">

                        <span class="allocated-type-tag type-mcq">
                            Single Choice
                        </span>

                        <span class="allocated-question-source">
                            Sejarah Bab 5
                        </span>

                    </div>


                    <h3>
                        Apakah faktor utama yang mendorong pembentukan
                        Jawatankuasa Setia Kawan Malaysia (JSKM) pada Julai 1961?
                    </h3>

                </div>


                <div class="allocated-question-controls">

                    <label for="marksQuestion1">
                        Marks:
                    </label>


                    <input type="number"
                           id="marksQuestion1"
                           class="question-marks-input"
                           value="4"
                           min="1" />


                    <button type="button"
                            class="remove-question-button"
                            aria-label="Remove question">

                        <i class="bi bi-trash3"
                           aria-hidden="true"></i>

                    </button>

                </div>

            </article>



            <%-- =========================
                 QUESTION 2
                 ========================= --%>

            <article class="allocated-question-row">

                <div class="question-order">
                    2
                </div>


                <div class="allocated-question-content">

                    <div class="allocated-question-tags">

                        <span class="allocated-type-tag type-subjective">
                            Subjective
                        </span>

                        <span class="allocated-question-source">
                            Sejarah Kertas 2
                        </span>

                    </div>


                    <h3>
                        Jelaskan dua faktor pembentukan Gagasan Malaysia
                        pada tahun 1961.
                    </h3>

                </div>


                <div class="allocated-question-controls">

                    <label for="marksQuestion2">
                        Marks:
                    </label>


                    <input type="number"
                           id="marksQuestion2"
                           class="question-marks-input"
                           value="4"
                           min="1" />


                    <button type="button"
                            class="remove-question-button"
                            aria-label="Remove question">

                        <i class="bi bi-trash3"
                           aria-hidden="true"></i>

                    </button>

                </div>

            </article>



            <%-- =========================
                 QUESTION 3
                 ========================= --%>

            <article class="allocated-question-row">

                <div class="question-order">
                    3
                </div>


                <div class="allocated-question-content">

                    <div class="allocated-question-tags">

                        <span class="allocated-type-tag type-mcq">
                            Single Choice
                        </span>

                        <span class="allocated-question-source">
                            Bank Item
                        </span>

                    </div>


                    <h3>
                        Apakah matlamat penubuhan Suruhanjaya Reid
                        pada tahun 1956?
                    </h3>

                </div>


                <div class="allocated-question-controls">

                    <label for="marksQuestion3">
                        Marks:
                    </label>


                    <input type="number"
                           id="marksQuestion3"
                           class="question-marks-input"
                           value="4"
                           min="1" />


                    <button type="button"
                            class="remove-question-button"
                            aria-label="Remove question">

                        <i class="bi bi-trash3"
                           aria-hidden="true"></i>

                    </button>

                </div>

            </article>

        </section>

    </div>



    <%-- =========================================================
         CHOOSE QUESTIONS FROM BANK MODAL
         ========================================================= --%>

    <div id="questionBankModal"
         class="question-bank-modal"
         hidden>


        <%-- OVERLAY --%>

        <div class="question-bank-modal-overlay"
             data-bank-close>
        </div>



        <%-- MODAL --%>

        <div class="question-bank-modal-dialog"
             role="dialog"
             aria-modal="true"
             aria-labelledby="questionBankModalTitle">


            <%-- HEADER --%>

            <div class="question-bank-modal-header">

                <div>

                    <h2 id="questionBankModalTitle">
                        Choose Questions from Bank
                    </h2>

                    <p>
                        Matching Sejarah syllabus items
                    </p>

                </div>


                <button type="button"
                        class="question-bank-modal-close"
                        data-bank-close
                        aria-label="Close">

                    <i class="bi bi-x-lg"
                       aria-hidden="true"></i>

                </button>

            </div>



            <%-- QUESTION LIST --%>

            <div class="bank-question-list">


                <%-- BANK QUESTION 1 --%>

                <label class="bank-question-item">

                    <input type="checkbox"
                           class="bank-question-checkbox" />


                    <div>

                        <div class="bank-question-meta">

                            <span>
                                Single Choice
                            </span>

                            <span>
                                •
                            </span>

                            <span>
                                Easy
                            </span>

                        </div>


                        <p>
                            Apakah matlamat penubuhan Suruhanjaya Reid
                            pada tahun 1956?
                        </p>

                    </div>

                </label>



                <%-- BANK QUESTION 2 --%>

                <label class="bank-question-item">

                    <input type="checkbox"
                           class="bank-question-checkbox" />


                    <div>

                        <div class="bank-question-meta">

                            <span>
                                Subjective
                            </span>

                            <span>
                                •
                            </span>

                            <span>
                                Hard
                            </span>

                        </div>


                        <p>
                            Huraikan reaksi parti-parti politik di Sarawak
                            terhadap cadangan pembentukan Malaysia.
                        </p>

                    </div>

                </label>

            </div>



            <%-- MODAL FOOTER --%>

            <div class="question-bank-modal-footer">

                <span>
                    Duplicate checking enabled
                </span>

                <button type="button"
                        id="btnAddToQuiz"
                        class="add-selected-questions-button">

                    Add to Quiz

                </button>

            </div>

        </div>

    </div>

</asp:Content>



<asp:Content
    ID="CreateQuizScripts"
    ContentPlaceHolderID="PortalScripts"
    runat="server">

    <script>

        (function () {

            const openButton =
                document.getElementById(
                    "btnOpenQuestionBank"
                );

            const modal =
                document.getElementById(
                    "questionBankModal"
                );


            /* =========================
               OPEN MODAL
               ========================= */

            openButton.addEventListener(
                "click",
                function () {

                    modal.hidden = false;

                    document.body.style.overflow =
                        "hidden";

                    const closeButton =
                        modal.querySelector(
                            ".question-bank-modal-close"
                        );

                    if (closeButton) {
                        closeButton.focus();
                    }

                }
            );


            /* =========================
               CLOSE MODAL
               ========================= */

            function closeQuestionBankModal() {

                modal.hidden = true;

                document.body.style.overflow =
                    "";

                openButton.focus();
            }


            document.addEventListener(
                "click",
                function (event) {

                    if (
                        event.target.closest(
                            "[data-bank-close]"
                        )
                    ) {

                        closeQuestionBankModal();
                    }

                }
            );


            /* =========================
               ESC KEY
               ========================= */

            document.addEventListener(
                "keydown",
                function (event) {

                    if (
                        event.key === "Escape" &&
                        !modal.hidden
                    ) {

                        closeQuestionBankModal();
                    }

                }
            );

        })();

    </script>

</asp:Content>