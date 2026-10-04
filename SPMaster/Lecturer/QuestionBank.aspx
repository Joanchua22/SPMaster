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

    <link href="<%= ResolveUrl("~/Content/QuestionBank.css") %>?v=1" rel="stylesheet" type="text/css" />

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

            <a href="#"
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

            <div class="question-search">

                <span class="search-icon"
                      aria-hidden="true">

                    <svg viewBox="0 0 24 24">

                        <circle cx="11"
                                cy="11"
                                r="6"></circle>

                        <path d="M16 16l4 4"></path>

                    </svg>

                </span>

                <input type="text"
                       placeholder="Search by question text or syllabus keyword..."
                       aria-label="Search questions" />

            </div>


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

            </select>


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
                 QUESTION 1
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

                            Preview

                        </button>

                        <button type="button"
                                class="question-action">

                            Edit

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
                 QUESTION 2
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
                            ▣ Image attached
                        </span>

                        <span class="question-owner">
                            Created by You (Mr. Rahman)
                        </span>

                    </div>


                    <div class="question-actions">

                        <button type="button"
                                class="question-action">

                            Preview

                        </button>

                        <button type="button"
                                class="question-action">

                            Duplicate

                        </button>

                        <button type="button"
                                class="question-action">

                            Archive

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
                 QUESTION 3 - SHARED
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

                        <button type="button"
                                class="question-action">

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

</asp:Content>


<asp:Content
    ID="QuestionBankScripts"
    ContentPlaceHolderID="PortalScripts"
    runat="server">

</asp:Content>