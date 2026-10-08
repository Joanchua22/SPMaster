<%@ Page Title="Create New Question"
    Language="C#"
    MasterPageFile="~/Master/Portal.Master"
    AutoEventWireup="true"
    CodeBehind="CreateQuestion.aspx.cs"
    Inherits="SPMaster.Lecturer.CreateQuestion" %>


<asp:Content
    ID="CreateQuestionHead"
    ContentPlaceHolderID="PortalHead"
    runat="server">

    <link href="<%= ResolveUrl("~/Content/CreateQuestion.css") %>?v=2"
          rel="stylesheet"
          type="text/css" />

    <link href="<%= ResolveUrl("~/Content/QuestionPreview.css") %>?v=1"
          rel="stylesheet"
          type="text/css" />

</asp:Content>



<asp:Content
    ID="CreateQuestionContent"
    ContentPlaceHolderID="PortalContent"
    runat="server">

    <div class="create-question-page">
               
        <asp:HiddenField
            ID="hfExistingMediaPath"
            runat="server"
            ClientIDMode="Static" />

        <asp:HiddenField
            ID="hfRemoveExistingImage"
            runat="server"
            ClientIDMode="Static"
            Value="false" />


        <%-- =========================
             PAGE HEADER
             ========================= --%>

        <div class="create-question-header">

            <div>

                <h1>
                    <asp:Literal
                        ID="litPageTitle"
                        runat="server">
                    </asp:Literal>
                </h1>

                <p>
                    <asp:Literal
                        ID="litPageSubtitle"
                        runat="server">
                    </asp:Literal>
                </p>

            </div>


            <a href="<%= ResolveUrl("~/Lecturer/QuestionBank.aspx") %>"
               class="cq-cancel-button">

                Cancel

            </a>

        </div>



        <%-- =========================
             CREATE QUESTION FORM
             ========================= --%>

        <section class="create-question-card">


            <%-- =====================
                 BASIC DETAILS
                 ===================== --%>

            <div class="cq-details-grid">


                <%-- SUBJECT --%>

                <div class="cq-field">

                    <label for="ddlSubject">
                        Subject
                    </label>

                    <div class="cq-select-wrapper">

                        <asp:DropDownList
                            ID="ddlSubject"
                            runat="server"
                            ClientIDMode="Static"
                            CssClass="cq-select">
                        </asp:DropDownList>


                        <i class="bi bi-caret-down-fill cq-select-arrow"
                           aria-hidden="true"></i>

                    </div>

                </div>



                <%-- QUESTION TYPE --%>

                <div class="cq-field">

                    <label for="ddlQuestionType">
                        Question Type
                    </label>

                    <div class="cq-select-wrapper">

                        <asp:DropDownList
                            ID="ddlQuestionType"
                            runat="server"
                            ClientIDMode="Static"
                            CssClass="cq-select">

                            <asp:ListItem
                                Text="Single Choice (MCQ)"
                                Value="MCQ">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Subjective (Open-ended)"
                                Value="Subjective">
                            </asp:ListItem>

                        </asp:DropDownList>


                        <i class="bi bi-caret-down-fill cq-select-arrow"
                           aria-hidden="true"></i>

                    </div>

                </div>



                <%-- DIFFICULTY --%>

                <div class="cq-field">

                    <label for="ddlDifficulty">
                        Difficulty Level
                    </label>

                    <div class="cq-select-wrapper">

                        <asp:DropDownList
                            ID="ddlDifficulty"
                            runat="server"
                            ClientIDMode="Static"
                            CssClass="cq-select">

                            <asp:ListItem
                                Text="Easy"
                                Value="Easy">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Medium"
                                Value="Medium"
                                Selected="True">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Hard"
                                Value="Hard">
                            </asp:ListItem>

                        </asp:DropDownList>


                        <i class="bi bi-caret-down-fill cq-select-arrow"
                           aria-hidden="true"></i>

                    </div>

                </div>

            </div>



            <%-- =========================
                 QUESTION PROMPT
                 ========================= --%>

            <div class="cq-field cq-full-field">

                <label for="txtQuestion">

                    Question Prompt / Stem

                    <span class="required-mark">
                        *
                    </span>

                </label>


                <asp:TextBox
                    ID="txtQuestion"
                    runat="server"
                    ClientIDMode="Static"
                    CssClass="cq-textarea cq-question-textarea"
                    TextMode="MultiLine"
                    placeholder="Enter the examination question here...">
                </asp:TextBox>

            </div>



            <%-- =========================
                 IMAGE / STIMULUS
                 ========================= --%>

            <div class="cq-field cq-full-field">

                <label>

                    Question Stimulus / Diagram

                    <span class="optional-text">
                        (Optional)
                    </span>

                </label>


                <div class="cq-upload-area">

                    <div class="cq-upload-inner">


                        <div class="cq-upload-icon">

                            <i class="bi bi-image"
                               aria-hidden="true"></i>

                        </div>



                        <div class="cq-upload-details">

                            <strong>
                                Upload Diagram, Chart, or Source Extract
                            </strong>

                            <span>
                                PNG, JPG or WebP up to 5MB
                            </span>



                            <div class="cq-upload-actions">

                                <label for="questionImage"
                                       class="cq-file-button">

                                    Select File

                                </label>


                                <asp:FileUpload
                                    ID="questionImage"
                                    runat="server"
                                    ClientIDMode="Static"
                                    accept=".png,.jpg,.jpeg,.webp"
                                    style="display:none;" />


                                <button type="button"
                                        id="removeImageButton"
                                        class="cq-remove-button">

                                    Remove

                                </button>

                            </div>


                            <asp:Label
                                ID="selectedFileName"
                                runat="server"
                                ClientIDMode="Static"
                                CssClass="cq-file-name">
                            </asp:Label>

                        </div>

                    </div>

                </div>

            </div>



            <%-- =====================================================
                 MCQ SECTION
                 ===================================================== --%>

            <div id="mcqSection"
                 class="cq-answer-section">


                <div class="cq-answer-heading">

                    <label>

                        Answer Choices

                        <span class="required-mark">
                            *
                        </span>

                    </label>


                    <span>
                        Exactly one correct option must be chosen
                    </span>

                </div>



                <%-- OPTION A --%>

                <div class="cq-option-row">

                    <input type="radio"
                           id="correctA"
                           runat="server"
                           ClientIDMode="Static"
                           name="correctOption"
                           value="A"
                           checked />


                    <label for="correctA"
                           class="cq-option-letter">

                        A

                    </label>


                    <asp:TextBox
                        ID="txtOptionA"
                        runat="server"
                        ClientIDMode="Static"
                        CssClass="cq-option-input"
                        placeholder="Enter option A">
                    </asp:TextBox>

                </div>



                <%-- OPTION B --%>

                <div class="cq-option-row">

                    <input type="radio"
                           id="correctB"
                           runat="server"
                           ClientIDMode="Static"
                           name="correctOption"
                           value="B" />


                    <label for="correctB"
                           class="cq-option-letter">

                        B

                    </label>


                    <asp:TextBox
                        ID="txtOptionB"
                        runat="server"
                        ClientIDMode="Static"
                        CssClass="cq-option-input"
                        placeholder="Enter option B">
                    </asp:TextBox>

                </div>



                <%-- OPTION C --%>

                <div class="cq-option-row">

                    <input type="radio"
                           id="correctC"
                           runat="server"
                           ClientIDMode="Static"
                           name="correctOption"
                           value="C" />


                    <label for="correctC"
                           class="cq-option-letter">

                        C

                    </label>


                    <asp:TextBox
                        ID="txtOptionC"
                        runat="server"
                        ClientIDMode="Static"
                        CssClass="cq-option-input"
                        placeholder="Enter option C">
                    </asp:TextBox>

                </div>



                <%-- OPTION D --%>

                <div class="cq-option-row">

                    <input type="radio"
                           id="correctD"
                           runat="server"
                           ClientIDMode="Static"
                           name="correctOption"
                           value="D" />


                    <label for="correctD"
                           class="cq-option-letter">

                        D

                    </label>


                    <asp:TextBox
                        ID="txtOptionD"
                        runat="server"
                        ClientIDMode="Static"
                        CssClass="cq-option-input"
                        placeholder="Enter option D">
                    </asp:TextBox>

                </div>

            </div>



            <%-- =====================================================
                 SUBJECTIVE SECTION
                 ===================================================== --%>

            <div id="subjectiveSection"
                 class="cq-answer-section"
                 hidden>


                <div class="cq-field cq-full-field">

                    <label for="txtModelAnswer">

                        Model Answer &amp; Scoring Guide

                        <span class="required-mark">
                            *
                        </span>

                    </label>


                    <asp:TextBox
                        ID="txtModelAnswer"
                        runat="server"
                        ClientIDMode="Static"
                        CssClass="cq-textarea cq-model-textarea"
                        TextMode="MultiLine"
                        placeholder="Enter facts, descriptions or key points used as the grading reference...">
                    </asp:TextBox>

                </div>


                <p class="cq-model-note">

                    Note: Marks are allocated when this question is added to a quiz.

                </p>

            </div>



            <%-- =========================
                 OPTIONAL SUPPORT
                 ========================= --%>

            <div class="cq-support-grid">


                <%-- HINT --%>

                <div class="cq-field">

                    <label for="txtHint">

                        Student Hint

                        <span class="optional-text">
                            (Optional)
                        </span>

                    </label>


                    <asp:TextBox
                        ID="txtHint"
                        runat="server"
                        ClientIDMode="Static"
                        CssClass="cq-input"
                        placeholder="e.g. Use the discriminant formula b² - 4ac = 0">
                    </asp:TextBox>

                </div>



                <%-- EXPLANATION --%>

                <div class="cq-field">

                    <label for="txtExplanation">

                        Post-Submission Explanation

                        <span class="optional-text">
                            (Optional)
                        </span>

                    </label>


                    <asp:TextBox
                        ID="txtExplanation"
                        runat="server"
                        ClientIDMode="Static"
                        CssClass="cq-input"
                        placeholder="Enter a short explanation of the solution...">
                    </asp:TextBox>

                </div>

            </div>



            <%-- =========================
                 FORM ACTIONS
                 ========================= --%>

            <div class="cq-form-actions">


                <button type="button"
                        id="btnPreviewQuestion"
                        class="cq-preview-button">

                    Preview Question

                </button>


                <asp:Button
                    ID="btnSaveQuestion"
                    runat="server"
                    CssClass="cq-save-button"
                    Text="Save to Bank"
                    OnClick="btnSaveQuestion_Click" />

            </div>

        </section>

    </div>



    <%-- =========================================================
         QUESTION PREVIEW MODAL
         Uses QuestionPreview.css + QuestionPreview.js
         ========================================================= --%>

    <div id="questionPreviewModal"
         class="question-preview-modal"
         hidden>


        <%-- OVERLAY --%>

        <div class="question-preview-overlay"
             data-preview-close>
        </div>



        <%-- DIALOG --%>

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
                        class="preview-close-button"
                        data-preview-close
                        aria-label="Close preview">

                    <i class="bi bi-x-lg"
                       aria-hidden="true"></i>

                </button>

            </div>



            <%-- BODY --%>

            <div class="question-preview-body">


                <%-- QUESTION TAGS --%>

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



                <%-- SUBJECTIVE ANSWER --%>

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
    ID="CreateQuestionScripts"
    ContentPlaceHolderID="PortalScripts"
    runat="server">


    <%-- Shared question preview JavaScript --%>

    <script src="<%= ResolveUrl("~/Scripts/QuestionPreview.js") %>?v=1">
    </script>



    <%-- Create Question page-specific JavaScript --%>

    <script>

        (function () {


            /* =========================================
               FORM ELEMENTS
               ========================================= */

            const subject =
                document.getElementById("ddlSubject");

            const questionType =
                document.getElementById("ddlQuestionType");

            const difficulty =
                document.getElementById("ddlDifficulty");

            const questionText =
                document.getElementById("txtQuestion");


            const optionA =
                document.getElementById("txtOptionA");

            const optionB =
                document.getElementById("txtOptionB");

            const optionC =
                document.getElementById("txtOptionC");

            const optionD =
                document.getElementById("txtOptionD");


            const modelAnswer =
                document.getElementById("txtModelAnswer");


            const imageInput =
                document.getElementById("questionImage");

            const removeImageButton =
                document.getElementById("removeImageButton");

            const selectedFileName =
                document.getElementById("selectedFileName");


            const previewButton =
                document.getElementById("btnPreviewQuestion");



            /* =========================================
               QUESTION TYPE SECTIONS
               ========================================= */

            const mcqSection =
                document.getElementById("mcqSection");

            const subjectiveSection =
                document.getElementById("subjectiveSection");



            /* =========================================
               MCQ / SUBJECTIVE SWITCHING
               ========================================= */

            function updateQuestionType() {

                const isMCQ =
                    questionType.value === "MCQ";

                mcqSection.hidden =
                    !isMCQ;

                subjectiveSection.hidden =
                    isMCQ;
            }


            questionType.addEventListener(
                "change",
                updateQuestionType
            );



            /* =========================================
               IMAGE SELECTION
               ========================================= */

            imageInput.addEventListener(
                "change",
                function() {

                    if (imageInput.files.length > 0) {

                        selectedFileName.textContent =
                            imageInput.files[0].name;

                        document.getElementById(
                            "hfRemoveExistingImage"
                        ).value = "false";

                    } else {

                        selectedFileName.textContent = "";
                    }
                }
            );



            /* =========================================
               REMOVE IMAGE
               ========================================= */

            removeImageButton.addEventListener(
                "click",
                function() {

                    imageInput.value = "";

                    selectedFileName.textContent = "";

                    document.getElementById(
                        "hfRemoveExistingImage"
                    ).value = "true";
                }
            );



            /* =========================================
               PREVIEW QUESTION
               ========================================= */

            previewButton.addEventListener(
                "click",
                function () {


                    const selectedType =
                        questionType.value;


                    const correctAnswer =
                        [
                            document.getElementById("correctA"),
                            document.getElementById("correctB"),
                            document.getElementById("correctC"),
                            document.getElementById("correctD")
                        ].find(option => option.checked);


                    let imageUrl = "";


                    if (imageInput.files.length > 0) {

                        imageUrl =
                            URL.createObjectURL(
                                imageInput.files[0]
                            );
                    }



                    QuestionPreview.open({

                        subject:
                            subject.options[
                                subject.selectedIndex
                            ].text,

                        difficulty:
                            difficulty.options[
                                difficulty.selectedIndex
                            ].text,

                        type:
                            selectedType === "MCQ"
                                ? "Single Choice (MCQ)"
                                : "Subjective (Open-ended)",

                        questionText:
                            questionText.value.trim()
                            || "No question entered yet.",


                        options: [

                            optionA.value.trim()
                            || "No answer entered",

                            optionB.value.trim()
                            || "No answer entered",

                            optionC.value.trim()
                            || "No answer entered",

                            optionD.value.trim()
                            || "No answer entered"

                        ],


                        correctOption:
                            correctAnswer
                                ? correctAnswer.value
                                : "",


                        modelAnswer:
                            modelAnswer.value.trim()
                            || "No model answer entered yet.",


                        imageUrl:
                            imageUrl

                    });

                }
            );



            /* =========================================
               INITIAL PAGE STATE
               ========================================= */

            updateQuestionType();

        })();

    </script>

</asp:Content>