<%@ Page Title="Question Bank" Language="C#" MasterPageFile="~/Master/Portal.Master" AutoEventWireup="true" CodeBehind="QuestionBank.aspx.cs" Inherits="SPMaster.Lecturer.QuestionBank" %>

<asp:Content ID="QuestionBankHead" ContentPlaceHolderID="PortalHead" runat="server">
    <link href="<%= ResolveUrl("~/Content/QuestionBank.css") %>?v=7" rel="stylesheet" type="text/css" />
    <link href="<%= ResolveUrl("~/Content/QuestionPreview.css") %>?v=3" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="QuestionBankContent" ContentPlaceHolderID="PortalContent" runat="server">

    <div class="question-bank-page">



        <%-- =========================
             PAGE HEADER
             ========================= --%>

        <section class="question-bank-header">

            <div>
                <h1>Question Bank</h1>
                <p> Browse, create, and organize SPM curriculum questions</p>
            </div>

            <a href="<%= ResolveUrl("~/Lecturer/CreateQuestion.aspx") %>" class="add-question-button">
                <i class="bi bi-plus-circle" aria-hidden="true"></i>
                Add New Question
            </a>

        </section>



        <%-- =========================
             SEARCH AND FILTERS
             ========================= --%>

        <section class="question-filter-card">

            <%-- SEARCH --%>

            <div class="question-search">

                <i class="bi bi-search search-icon" aria-hidden="true"></i>
                <asp:TextBox ID="txtSearch" runat="server" AutoPostBack="true" OnTextChanged="FilterChanged" placeholder="Search by question text or keyword..." aria-label="Search questions"></asp:TextBox>

            </div>


            <%-- SUBJECT --%>

            <div class="question-filter-wrapper">

                <asp:DropDownList ID="ddlSubject" runat="server" CssClass="question-filter" AutoPostBack="true" OnSelectedIndexChanged="FilterChanged" aria-label="Filter by subject"></asp:DropDownList>
                <i class="bi bi-caret-down-fill filter-arrow" aria-hidden="true"></i>

            </div>


            <%-- TYPE --%>

            <div class="question-filter-wrapper">

                <asp:DropDownList ID="ddlQuestionType" runat="server" CssClass="question-filter" AutoPostBack="true" OnSelectedIndexChanged="FilterChanged" aria-label="Filter by question type">

                    <asp:ListItem Text="All Types" Value=""></asp:ListItem>
                    <asp:ListItem Text="MCQ" Value="MCQ"></asp:ListItem>
                    <asp:ListItem Text="Subjective" Value="Subjective"> </asp:ListItem>

                </asp:DropDownList>

                <i class="bi bi-caret-down-fill filter-arrow" aria-hidden="true"></i>

            </div>


            <%-- DIFFICULTY --%>

            <div class="question-filter-wrapper">

                <asp:DropDownList ID="ddlDifficulty" runat="server" CssClass="question-filter" AutoPostBack="true" OnSelectedIndexChanged="FilterChanged" aria-label="Filter by difficulty">

                    <asp:ListItem Text="All Difficulties" Value=""></asp:ListItem>
                    <asp:ListItem Text="Easy" Value="Easy"></asp:ListItem>
                    <asp:ListItem Text="Medium" Value="Medium"></asp:ListItem>
                    <asp:ListItem Text="Hard" Value="Hard"></asp:ListItem>

                </asp:DropDownList>

                <i class="bi bi-caret-down-fill filter-arrow" aria-hidden="true"></i>

            </div>

        </section>



        <%-- =========================
             QUESTION BANK TABS
             ========================= --%>

        <div class="question-tabs">

            <asp:LinkButton ID="btnMyQuestions" runat="server" CssClass="question-tab active" OnClick="btnMyQuestions_Click"></asp:LinkButton>
            <asp:LinkButton ID="btnSharedQuestions" runat="server" CssClass="question-tab" OnClick="btnSharedQuestions_Click"></asp:LinkButton>

        </div>



        <%-- =========================
             QUESTION LIST
             ========================= --%>

        <section class="question-list">

            <asp:Repeater ID="rptQuestions" runat="server" OnItemCommand="rptQuestions_ItemCommand">
                <ItemTemplate>
                    <article class="question-card">
                        <div class="question-card-top">


                            <%-- QUESTION TAGS --%>

                            <div class="question-tags">

                                <span class='question-tag subject-tag <%# GetSubjectCss(Eval("SubjectName")) %>'><%#: Eval("SubjectName") %></span>
                                <span class='question-tag <%# GetDifficultyCss(Eval("Difficulty")) %>'> <%#: Eval("Difficulty") %> </span>
                                <span class="question-tag type-tag"> <%#: GetQuestionTypeDisplay(Eval("QuestionType")) %></span>


                                <%-- IMAGE TAG --%>

                                <asp:PlaceHolder runat="server" Visible='<%# HasValue(Eval("MediaPath")) %>'>

                                    <span class="question-tag media-tag">
                                        <i class="bi bi-image"aria-hidden="true"></i>
                                        Image attached
                                    </span>

                                </asp:PlaceHolder>


                                <span class="question-owner"> <%#: GetOwnerText(Eval("IsOwner"),Eval("CreatorName")) %></span>

                            </div>



                            <%-- ACTION BUTTONS --%>

                            <div class="question-actions">

                                <%-- CURRENT LECTURER'S QUESTION --%>

                                <asp:PlaceHolder runat="server" Visible='<%# Convert.ToBoolean(Eval("IsOwner")) %>'>

                                    <asp:LinkButton runat="server" CssClass="question-action" CommandName="EditQuestion" CommandArgument='<%# Eval("QuestionId") %>'> Edit</asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="question-action" CommandName="ArchiveQuestion" CommandArgument='<%# Eval("QuestionId") %>' OnClientClick="return confirm('Archive this question?');">Archive</asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="question-action question-action-danger" CommandName="DeleteQuestion" CommandArgument='<%# Eval("QuestionId") %>' OnClientClick="return confirm('Permanently delete this question?');">Delete</asp:LinkButton>

                                </asp:PlaceHolder>


                                <%-- OTHER LECTURER'S QUESTION --%>

                                <asp:PlaceHolder
                                    runat="server"
                                    Visible='<%# !Convert.ToBoolean(Eval("IsOwner")) %>'>

                                    <button type="button"
                                            class="question-action preview-question-button"
                                            data-preview-question
                                            data-subject='<%# AttributeEncode(Eval("SubjectName")) %>'
                                            data-difficulty='<%# AttributeEncode(Eval("Difficulty")) %>'
                                            data-type='<%# AttributeEncode(Eval("QuestionType")) %>'
                                            data-question='<%# AttributeEncode(Eval("QuestionText")) %>'
                                            data-option-a='<%# AttributeEncode(Eval("OptionA")) %>'
                                            data-option-b='<%# AttributeEncode(Eval("OptionB")) %>'
                                            data-option-c='<%# AttributeEncode(Eval("OptionC")) %>'
                                            data-option-d='<%# AttributeEncode(Eval("OptionD")) %>'
                                            data-correct-option='<%# AttributeEncode(Eval("CorrectOption")) %>'
                                            data-model-answer='<%# AttributeEncode(Eval("ModelAnswer")) %>'
                                            data-image-url='<%# AttributeEncode(GetMediaUrl(Eval("MediaPath"))) %>'>

                                        Preview

                                    </button>


                                    <button type="button"
                                            class="question-action question-action-primary"
                                            data-add-to-quiz
                                            data-question-id='<%# Eval("QuestionId") %>'
                                            data-subject-id='<%# Eval("SubjectId") %>'
                                            data-subject-name='<%# AttributeEncode(Eval("SubjectName")) %>'>

                                        Add to My Quiz

                                    </button>

                                </asp:PlaceHolder>

                            </div>

                        </div>



                        <%-- QUESTION TEXT --%>

                        <h2 class="question-text"><%#: Eval("QuestionText") %></h2>

                        <%-- QUESTION META --%>

                        <div class="question-meta">


                            <%-- MCQ --%>

                            <asp:PlaceHolder runat="server" Visible='<%# IsMcq(Eval("QuestionType")) %>'>
                                <span> <%#: FormatCorrectAnswer(Eval("CorrectOption"),Eval("CorrectAnswerText")) %></span>
                            </asp:PlaceHolder>


                            <%-- SUBJECTIVE --%>

                            <asp:PlaceHolder runat="server" Visible='<%# !IsMcq(Eval("QuestionType")) %>'>
                                <span><%#: GetModelAnswerStatus(Eval("ModelAnswer")) %></span>
                            </asp:PlaceHolder>

                            <span class="meta-dot">•</span>


                            <span>Used in <%#: Eval("UsedInQuizzes") %> Quiz(es)</span>

                        </div>

                    </article>

                </ItemTemplate>

            </asp:Repeater>



            <%-- NO RESULTS --%>

            <asp:Panel ID="pnlNoQuestions" runat="server" Visible="false">

                <div class="question-card">
                    <h2 class="question-text">No questions found.</h2>

                    <div class="question-meta">
                        Try changing your search or filter.
                    </div>

                </div>

            </asp:Panel>

        </section>
    
    </div>



    <%-- =========================================================
         QUESTION PREVIEW MODAL
         Reused with QuestionPreview.css / QuestionPreview.js
         ========================================================= --%>

    <div id="questionPreviewModal" class="question-preview-modal" hidden>


        <%-- OVERLAY --%>
        <div id="previewOverlay" class="question-preview-overlay" data-preview-close>
        </div>


        <%-- MODAL DIALOG --%>

        <div class="question-preview-dialog" role="dialog" aria-modal="true" aria-labelledby="previewTitle">


            <%-- HEADER --%>

            <div class="question-preview-header">

                <div>
                    <span class="preview-label">Question Preview</span>
                    <h2 id="previewTitle">Student View</h2>
                </div>

                <button type="button" id="btnClosePreview" class="preview-close-button" data-preview-close aria-label="Close preview">
                    <i class="bi bi-x-lg" aria-hidden="true"></i>
                </button>

            </div>



            <%-- BODY --%>

            <div class="question-preview-body">


                <%-- TAGS --%>

                <div class="preview-tags">

                    <span id="previewSubject" class="preview-tag"></span>
                    <span id="previewDifficulty" class="preview-tag"></span>
                    <span id="previewType" class="preview-tag"></span>

                </div>


                <%-- QUESTION TEXT --%>

                <h3 id="previewQuestionText" class="preview-question-text"></h3>


                <%-- OPTIONAL IMAGE --%>

                <div id="previewImageArea" class="preview-image-area" hidden>
                    <img id="previewImage" src="" alt="Question stimulus preview" />
                </div>


                <%-- MCQ ANSWERS --%>

                <div id="previewMcqAnswers" class="preview-answer-list">

                    <div class="preview-answer" data-preview-option="A">

                        <span> A </span>

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

    <%-- =========================================================
         ADD TO QUIZ MODAL
         ========================================================= --%>

    <asp:HiddenField
        ID="hfSelectedQuestionId"
        runat="server"
        ClientIDMode="Static" />

    <asp:HiddenField
        ID="hfSelectedQuestionSubjectId"
        runat="server"
        ClientIDMode="Static" />

    <asp:HiddenField
        ID="hfSelectedQuestionSubjectName"
        runat="server"
        ClientIDMode="Static" />


    <div id="addToQuizModal"
         class="add-quiz-modal"
         hidden>

        <div class="add-quiz-overlay"
             data-add-quiz-close>
        </div>


        <div class="add-quiz-dialog"
             role="dialog"
             aria-modal="true"
             aria-labelledby="addQuizTitle">


            <%-- HEADER --%>

            <div class="add-quiz-header">

                <div>

                    <span class="add-quiz-label">
                        Shared Question
                    </span>

                    <h2 id="addQuizTitle">
                        Add Question to Quiz
                    </h2>

                    <p id="addQuizSubtitle">
                        Choose one of your draft quizzes
                    </p>

                </div>


                <button type="button"
                        class="add-quiz-close"
                        data-add-quiz-close
                        aria-label="Close">

                    <i class="bi bi-x-lg"
                       aria-hidden="true"></i>

                </button>

            </div>


            <%-- DRAFT QUIZZES --%>

            <div class="draft-quiz-section">

            <asp:HiddenField
                ID="hfSelectedQuizId"
                runat="server"
                ClientIDMode="Static" />


            <%-- INLINE QUIZ SELECTION ERROR --%>

            <div id="addQuizError"
                 class="add-quiz-error"
                 hidden>

                <i class="bi bi-exclamation-circle"
                   aria-hidden="true"></i>

                <span id="addQuizErrorText">
                </span>

            </div>

            <%-- INLINE QUIZ SUCCESSFUL MESSAGE --%>

            <asp:Panel
                ID="pnlAddQuizSuccess"
                runat="server"
                ClientIDMode="Static"
                Visible="false"
                CssClass="add-quiz-success">

                <i class="bi bi-check-circle-fill"
                   aria-hidden="true"></i>

                <asp:Label
                    ID="lblAddQuizSuccess"
                    runat="server">
                </asp:Label>

            </asp:Panel>


            <div class="draft-quiz-list">

                <asp:Repeater
                    ID="rptDraftQuizzes"
                    runat="server">

                    <ItemTemplate>

                    <label class="draft-quiz-card"
                           data-subject-id='<%# Eval("SubjectId") %>'
                           data-question-ids='<%# Eval("QuestionIds") %>'>

                        <input type="radio"
                               name="draftQuizChoice"
                               class="draft-quiz-radio"
                               value='<%# Eval("QuizId") %>' />


                        <div class="draft-quiz-content">

                            <div class="draft-quiz-top">

                                <div>

                                    <h3 class="draft-quiz-title">
                                        <%#: Eval("Title") %>
                                    </h3>

                                    <span class="draft-quiz-subject">
                                        <%#: Eval("SubjectName") %>
                                    </span>

                                </div>


                                <div class="draft-quiz-card-right">

                                    <span class="draft-quiz-meta">
                                        <%#: Eval("QuestionCount") %> Questions
                                    </span>

                                    <span class="draft-already-added"
                                          hidden>
                                        Already Added
                                    </span>

                                </div>

                            </div>


                            <p class="draft-quiz-description">
                                <%#: Eval("Description") %>
                            </p>

                        </div>

                    </label>

                    </ItemTemplate>

                </asp:Repeater>

                <div id="noMatchingDraftQuiz"
                     class="no-draft-message"
                     hidden>

                    You do not have any draft quiz for this subject.

                </div>

            </div>


                <asp:Panel
                    ID="pnlNoDraftQuiz"
                    runat="server"
                    Visible="false"
                    CssClass="no-draft-message">

                    You do not have any draft quizzes yet.

                </asp:Panel>

            </div>


            <%-- FOOTER --%>

            <div class="add-quiz-footer">

                <asp:Button
                    ID="btnCreateNewQuiz"
                    runat="server"
                    CssClass="create-new-quiz-button"
                    Text="+ Create New Quiz"
                    OnClick="btnCreateNewQuiz_Click" />


                <div class="add-quiz-footer-right">

                    <button type="button"
                            class="add-quiz-cancel"
                            data-add-quiz-close>

                        Cancel

                    </button>


                    <asp:Button
                        ID="btnAddSelectedQuiz"
                        runat="server"
                        ClientIDMode="Static"
                        CssClass="add-selected-quiz-button"
                        Text="Add to Quiz"
                        OnClientClick="return validateDraftQuizSelection();"
                        OnClick="btnAddSelectedQuiz_Click" />

                </div>

            </div>

        </div>

    </div>

</asp:Content>



<asp:Content ID="QuestionBankScripts" ContentPlaceHolderID="PortalScripts" runat="server">

    <script src="<%= ResolveUrl("~/Scripts/QuestionPreview.js") %>?v=1">
    </script>

<script>

    (function () {

        /* =====================================================
           ELEMENTS
           ===================================================== */

        const modal =
            document.getElementById(
                "addToQuizModal"
            );


        const selectedQuestion =
            document.getElementById(
                "hfSelectedQuestionId"
            );


        const selectedQuestionSubjectId =
            document.getElementById(
                "hfSelectedQuestionSubjectId"
            );


        const selectedQuestionSubjectName =
            document.getElementById(
                "hfSelectedQuestionSubjectName"
            );


        const selectedQuiz =
            document.getElementById(
                "hfSelectedQuizId"
            );


        /* =====================================================
           INLINE ERROR
           ===================================================== */

        window.showAddQuizError =
            function (message) {

                const errorBox =
                    document.getElementById(
                        "addQuizError"
                    );


                const errorText =
                    document.getElementById(
                        "addQuizErrorText"
                    );


                if (
                    !errorBox ||
                    !errorText
                ) {
                    return;
                }


                errorText.textContent =
                    message;


                errorBox.hidden =
                    false;

            };


        window.hideAddQuizError =
            function () {

                const errorBox =
                    document.getElementById(
                        "addQuizError"
                    );


                if (errorBox) {

                    errorBox.hidden =
                        true;

                }

            };


        /* =====================================================
           VALIDATE QUIZ SELECTION
           ===================================================== */

        window.validateDraftQuizSelection =
            function () {

                if (
                    !selectedQuiz ||
                    !selectedQuiz.value
                ) {

                    window.showAddQuizError(
                        "Please select a draft quiz."
                    );


                    return false;
                }


                window.hideAddQuizError();


                return true;

            };


        /* =====================================================
           REQUIRED MODAL ELEMENTS
           ===================================================== */

        if (
            !modal ||
            !selectedQuestion ||
            !selectedQuestionSubjectId ||
            !selectedQuestionSubjectName ||
            !selectedQuiz
        ) {
            return;
        }


        /* =====================================================
           RESET QUIZ SELECTION
           ===================================================== */

        function resetQuizSelection() {

            selectedQuiz.value =
                "";

            window.hideAddQuizError();


            const radios =
                modal.querySelectorAll(
                    ".draft-quiz-radio"
                );


            radios.forEach(
                function (radio) {

                    radio.checked =
                        false;

                    radio.disabled =
                        false;

                }
            );


            const cards =
                modal.querySelectorAll(
                    ".draft-quiz-card"
                );


            cards.forEach(
                function (card) {

                    card.classList.remove(
                        "is-selected"
                    );


                    card.classList.remove(
                        "is-unavailable"
                    );


                    card.hidden =
                        false;


                    const alreadyAddedLabel =
                        card.querySelector(
                            ".draft-already-added"
                        );


                    if (alreadyAddedLabel) {

                        alreadyAddedLabel.hidden =
                            true;

                    }

                }
            );


            const noMatchingMessage =
                document.getElementById(
                    "noMatchingDraftQuiz"
                );


            if (noMatchingMessage) {

                noMatchingMessage.hidden =
                    true;

            }

        }


        /* =====================================================
           OPEN MODAL
           ===================================================== */

        window.openAddQuizModal =
            function(
                questionId,
                subjectId,
                subjectName
            ) {

                selectedQuestion.value =
                    questionId;


                document.getElementById(
                    "hfSelectedQuestionSubjectId"
                ).value =
                    subjectId;


                document.getElementById(
                    "hfSelectedQuestionSubjectName"
                ).value =
                    subjectName;


                resetQuizSelection();


            const cards =
                modal.querySelectorAll(
                    ".draft-quiz-card"
                );


            const noMatchingMessage =
                document.getElementById(
                    "noMatchingDraftQuiz"
                );


            const subtitle =
                document.getElementById(
                    "addQuizSubtitle"
                );


            let matchingSubjectCount =
                0;

            let availableQuizCount =
                0;


            cards.forEach(
                function (card) {

                    const quizSubjectId =
                        card.dataset.subjectId;


                    const questionIds =
                        card.dataset.questionIds
                            ? card.dataset.questionIds
                                .split(",")
                                .map(function (id) {
                                    return id.trim();
                                })
                            : [];


                    const radio =
                        card.querySelector(
                            ".draft-quiz-radio"
                        );


                    const alreadyAddedLabel =
                        card.querySelector(
                            ".draft-already-added"
                        );


                    /* =====================================
                       DIFFERENT SUBJECT
                       ===================================== */

                    if (
                        quizSubjectId !==
                        subjectId
                    ) {

                        card.hidden =
                            true;

                        return;

                    }


                    /* =====================================
                       SAME SUBJECT
                       ===================================== */

                    card.hidden =
                        false;


                    matchingSubjectCount++;


                    const alreadyAdded =
                        questionIds.includes(
                            questionId
                        );


                    /* =====================================
                       QUESTION ALREADY IN QUIZ
                       ===================================== */

                    if (alreadyAdded) {

                        card.classList.add(
                            "is-unavailable"
                        );


                        if (radio) {

                            radio.disabled =
                                true;

                        }


                        if (alreadyAddedLabel) {

                            alreadyAddedLabel.hidden =
                                false;

                        }

                    }


                    /* =====================================
                       QUESTION CAN BE ADDED
                       ===================================== */

                    else {

                        card.classList.remove(
                            "is-unavailable"
                        );


                        if (radio) {

                            radio.disabled =
                                false;

                        }


                        if (alreadyAddedLabel) {

                            alreadyAddedLabel.hidden =
                                true;

                        }


                        availableQuizCount++;

                    }

                }
            );


            /* =========================================
               SUBTITLE
               ========================================= */

            if (subtitle) {

                subtitle.textContent =
                    "Choose one of your "
                    + subjectName
                    + " draft quizzes";

            }


            /* =========================================
               NO MATCHING / AVAILABLE QUIZ MESSAGE
               ========================================= */

            if (noMatchingMessage) {

                if (
                    matchingSubjectCount === 0
                ) {

                    noMatchingMessage.textContent =
                        "You do not have any "
                        + subjectName
                        + " draft quiz yet.";


                    noMatchingMessage.hidden =
                        false;

                }

                else if (
                    availableQuizCount === 0
                ) {

                    noMatchingMessage.textContent =
                        "This question is already included in all of your "
                        + subjectName
                        + " draft quizzes.";


                    noMatchingMessage.hidden =
                        false;

                }

                else {

                    noMatchingMessage.hidden =
                        true;

                }

            }


            modal.hidden =
                false;


            document.body.style.overflow =
                "hidden";


            const closeButton =
                modal.querySelector(
                    ".add-quiz-close"
                );


            if (closeButton) {

                closeButton.focus();

            }

        }


        /* =====================================================
           CLOSE MODAL
           ===================================================== */

        function closeModal() {

            modal.hidden =
                true;


            document.body.style.overflow =
                "";


            window.hideAddQuizError();

            window.hideAddQuizSuccess();

        }


        /* =====================================================
           ADD TO MY QUIZ BUTTON
           ===================================================== */

        document.addEventListener(
            "click",
            function(event) {

                const button =
                    event.target.closest(
                        "[data-add-to-quiz]"
                    );


                if (!button) {
                    return;
                }


                // Hide success message from
                // previous Add to Quiz operation
                window.hideAddQuizSuccess();


                const questionId =
                    button.dataset.questionId;


                const subjectId =
                    button.dataset.subjectId;


                const subjectName =
                    button.dataset.subjectName;


                if (
                    !questionId ||
                    !subjectId
                ) {
                    return;
                }


                window.openAddQuizModal(
                    questionId,
                    subjectId,
                    subjectName
                );

            }
        );


        /* =====================================================
           SELECT DRAFT QUIZ
           ===================================================== */

        document.addEventListener(
            "change",
            function (event) {

                const radio =
                    event.target.closest(
                        ".draft-quiz-radio"
                    );


                if (
                    !radio ||
                    radio.disabled
                ) {
                    return;
                }


                /* -----------------------------------------
                   Save selected QuizId
                   ----------------------------------------- */

                selectedQuiz.value =
                    radio.value;

                window.hideAddQuizError();


                /* -----------------------------------------
                   Remove existing selected card style
                   ----------------------------------------- */

                const cards =
                    modal.querySelectorAll(
                        ".draft-quiz-card"
                    );


                cards.forEach(
                    function (card) {

                        card.classList.remove(
                            "is-selected"
                        );

                    }
                );


                /* -----------------------------------------
                   Highlight selected card
                   ----------------------------------------- */

                const selectedCard =
                    radio.closest(
                        ".draft-quiz-card"
                    );


                if (selectedCard) {

                    selectedCard.classList.add(
                        "is-selected"
                    );

                }

            }
        );


        /* =====================================================
           CLICK CARD TO SELECT QUIZ
           ===================================================== */

        document.addEventListener(
            "click",
            function(event) {

                const card =
                    event.target.closest(
                        ".draft-quiz-card"
                    );


                if (!card) {
                    return;
                }


                const radio =
                    card.querySelector(
                        ".draft-quiz-radio"
                    );


                // =============================================
                // DO NOT SELECT UNAVAILABLE QUIZ
                // =============================================

                if (
                    !radio ||
                    radio.disabled ||
                    card.classList.contains(
                        "is-unavailable"
                    )
                ) {
                    return;
                }


                /*
                   Do not manually trigger again
                   when user directly clicks radio.
                */

                if (
                    event.target === radio
                ) {
                    return;
                }


                radio.checked =
                    true;


                selectedQuiz.value =
                    radio.value;


                window.hideAddQuizError();


                const cards =
                    modal.querySelectorAll(
                        ".draft-quiz-card"
                    );


                cards.forEach(
                    function(quizCard) {

                        quizCard.classList.remove(
                            "is-selected"
                        );

                    }
                );


                card.classList.add(
                    "is-selected"
                );

            }
        );


        window.hideAddQuizSuccess =
            function() {

                const successBox =
                    document.getElementById(
                        "pnlAddQuizSuccess"
                    );


                if (successBox) {

                    successBox.style.display =
                        "none";

                }

            };


        /* =====================================================
           CLOSE BUTTON / OVERLAY / CANCEL
           ===================================================== */

        document.addEventListener(
            "click",
            function (event) {

                const closeElement =
                    event.target.closest(
                        "[data-add-quiz-close]"
                    );


                if (!closeElement) {
                    return;
                }


                closeModal();

            }
        );



        /* =====================================================
           ESCAPE KEY
           ===================================================== */

        document.addEventListener(
            "keydown",
            function (event) {

                if (
                    event.key !== "Escape"
                ) {
                    return;
                }


                if (
                    !modal.hidden
                ) {

                    closeModal();

                }

            }
        );


        /* =====================================================
           REOPEN MODAL AFTER SUCCESSFUL SERVER POSTBACK
           ===================================================== */

        const successPanel =
            document.getElementById(
                "pnlAddQuizSuccess"
            );


        if (
            successPanel &&
            selectedQuestion.value &&
            selectedQuestionSubjectId.value
        ) {

            window.openAddQuizModal(
                selectedQuestion.value,
                selectedQuestionSubjectId.value,
                selectedQuestionSubjectName.value
            );

        }

    })();

</script>

</asp:Content>