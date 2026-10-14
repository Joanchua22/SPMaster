<%@ Page Title="Question Editor"
    Language="C#"
    MasterPageFile="~/Master/Portal.Master"
    AutoEventWireup="true"
    CodeBehind="CreateQuestion.aspx.cs"
    Inherits="SPMaster.Lecturer.CreateQuestion" %>


<asp:Content
    ID="CreateQuestionHead"
    ContentPlaceHolderID="PortalHead"
    runat="server">

    <link href="<%= ResolveUrl("~/Content/CreateQuestion.css") %>?v=5"
          rel="stylesheet"
          type="text/css" />

    <link href="<%= ResolveUrl("~/Content/QuestionPreview.css") %>?v=3"
          rel="stylesheet"
          type="text/css" />

</asp:Content>


<asp:Content
    ID="CreateQuestionContent"
    ContentPlaceHolderID="PortalContent"
    runat="server">


    <div class="create-question-page">

        <asp:HiddenField
            ID="hfQuestionDirty"
            runat="server"
            ClientIDMode="Static"
            Value="false" />


        <%-- =====================================================
             PAGE HEADER
             ===================================================== --%>

        <div class="create-question-header">

            <div>

                <h1>
                    <asp:Literal
                        ID="litPageTitle"
                        runat="server"
                        Text="Create New Question">
                    </asp:Literal>
                </h1>

                <p>
                    <asp:Literal
                        ID="litPageSubtitle"
                        runat="server"
                        Text="Add an SPM standardized question to your institutional bank">
                    </asp:Literal>
                </p>

            </div>


            <a href="<%= ResolveUrl("~/Lecturer/QuestionBank.aspx") %>"
               class="cq-cancel-button"
               onclick="return confirmCancelQuestion();">

                Cancel

            </a>

        </div>



        <%-- =====================================================
             SUCCESS MESSAGE
             ===================================================== --%>

        <asp:Panel
            ID="pnlQuestionSuccess"
            runat="server"
            Visible="false"
            CssClass="cq-save-message">

            <i class="bi bi-check-circle-fill"
               aria-hidden="true"></i>

            <asp:Label
                ID="lblQuestionSuccess"
                runat="server">
            </asp:Label>

        </asp:Panel>



        <%-- =====================================================
             GENERAL ERROR
             Used for unexpected server/database errors only
             ===================================================== --%>

        <asp:Panel
            ID="pnlQuestionError"
            runat="server"
            Visible="false"
            CssClass="cq-error-message">

            <i class="bi bi-exclamation-circle-fill"
               aria-hidden="true"></i>

            <asp:Label
                ID="lblQuestionError"
                runat="server">
            </asp:Label>

        </asp:Panel>



        <%-- =====================================================
             EDIT IMAGE STATE
             ===================================================== --%>

        <asp:HiddenField
            ID="hfExistingMediaPath"
            runat="server"
            ClientIDMode="Static" />

        <asp:HiddenField
            ID="hfExistingMediaUrl"
            runat="server"
            ClientIDMode="Static" />

        <asp:HiddenField
            ID="hfRemoveExistingImage"
            runat="server"
            ClientIDMode="Static"
            Value="false" />



        <%-- =====================================================
             QUESTION FORM
             ===================================================== --%>

        <section class="create-question-card">


            <%-- =================================================
                 BASIC DETAILS
                 ================================================= --%>

            <div class="cq-details-grid">


                <%-- =============================================
                     SUBJECT
                     ============================================= --%>

                <div class="cq-field">

                    <label for="ddlSubject">

                        Subject

                        <span class="required-mark">
                            *
                        </span>

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


                    <asp:RequiredFieldValidator
                        ID="valSubject"
                        runat="server"
                        ControlToValidate="ddlSubject"
                        InitialValue=""
                        ErrorMessage="Please select a subject."
                        CssClass="cq-validation-error"
                        Display="Dynamic"
                        SetFocusOnError="true"
                        ValidationGroup="QuestionValidation">
                    </asp:RequiredFieldValidator>

                </div>



                <%-- =============================================
                     QUESTION TYPE
                     ============================================= --%>

                <div class="cq-field">

                    <label for="ddlQuestionType">

                        Question Type

                        <span class="required-mark">
                            *
                        </span>

                    </label>


                    <div class="cq-select-wrapper">

                        <asp:DropDownList
                            ID="ddlQuestionType"
                            runat="server"
                            ClientIDMode="Static"
                            CssClass="cq-select">

                            <asp:ListItem
                                Text="Select Question Type"
                                Value="">
                            </asp:ListItem>

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


                    <asp:RequiredFieldValidator
                        ID="valQuestionType"
                        runat="server"
                        ControlToValidate="ddlQuestionType"
                        InitialValue=""
                        ErrorMessage="Please select a question type."
                        CssClass="cq-validation-error"
                        Display="Dynamic"
                        SetFocusOnError="true"
                        ValidationGroup="QuestionValidation">
                    </asp:RequiredFieldValidator>

                </div>



                <%-- =============================================
                     DIFFICULTY
                     ============================================= --%>

                <div class="cq-field">

                    <label for="ddlDifficulty">

                        Difficulty Level

                        <span class="required-mark">
                            *
                        </span>

                    </label>


                    <div class="cq-select-wrapper">

                        <asp:DropDownList
                            ID="ddlDifficulty"
                            runat="server"
                            ClientIDMode="Static"
                            CssClass="cq-select">

                            <asp:ListItem
                                Text="Select Difficulty"
                                Value="">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Easy"
                                Value="Easy">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Medium"
                                Value="Medium">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Hard"
                                Value="Hard">
                            </asp:ListItem>

                        </asp:DropDownList>

                        <i class="bi bi-caret-down-fill cq-select-arrow"
                           aria-hidden="true"></i>

                    </div>


                    <asp:RequiredFieldValidator
                        ID="valDifficulty"
                        runat="server"
                        ControlToValidate="ddlDifficulty"
                        InitialValue=""
                        ErrorMessage="Please select a difficulty level."
                        CssClass="cq-validation-error"
                        Display="Dynamic"
                        SetFocusOnError="true"
                        ValidationGroup="QuestionValidation">
                    </asp:RequiredFieldValidator>

                </div>

            </div>



            <%-- =================================================
                 QUESTION PROMPT
                 ================================================= --%>

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


                <asp:RequiredFieldValidator
                    ID="valQuestion"
                    runat="server"
                    ControlToValidate="txtQuestion"
                    ErrorMessage="Question prompt is required."
                    CssClass="cq-validation-error"
                    Display="Dynamic"
                    SetFocusOnError="true"
                    ValidationGroup="QuestionValidation">
                </asp:RequiredFieldValidator>

            </div>



            <%-- =================================================
                 IMAGE / STIMULUS
                 ================================================= --%>

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
                                PNG, JPG, JPEG or WebP up to 5MB
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
                                    CssClass="cq-hidden-file"
                                    accept=".png,.jpg,.jpeg,.webp" />


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


                <asp:CustomValidator
                    ID="valImage"
                    runat="server"
                    ErrorMessage=""
                    CssClass="cq-validation-error"
                    Display="Dynamic"
                    OnServerValidate="ValidateImage"
                    ValidationGroup="QuestionValidation">
                </asp:CustomValidator>

            </div>



            <%-- =================================================
                 MCQ SECTION
                 ================================================= --%>

            <div id="mcqSection"
                 class="cq-answer-section"
                 hidden>


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



                <%-- =============================================
                     OPTION A
                     ============================================= --%>

                <div class="cq-option-validation-group">

                    <div class="cq-option-row">

                        <asp:RadioButton
                            ID="correctA"
                            runat="server"
                            ClientIDMode="Static"
                            GroupName="CorrectOption" />

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


                    <asp:CustomValidator
                        ID="valOptionA"
                        runat="server"
                        ControlToValidate="txtOptionA"
                        ValidateEmptyText="true"
                        ErrorMessage="Option A is required."
                        CssClass="cq-validation-error cq-option-validation"
                        Display="Dynamic"
                        ClientValidationFunction="validateMcqOption"
                        OnServerValidate="ValidateMcqOption"
                        ValidationGroup="QuestionValidation">
                    </asp:CustomValidator>

                </div>



                <%-- =============================================
                     OPTION B
                     ============================================= --%>

                <div class="cq-option-validation-group">

                    <div class="cq-option-row">

                        <asp:RadioButton
                            ID="correctB"
                            runat="server"
                            ClientIDMode="Static"
                            GroupName="CorrectOption" />

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


                    <asp:CustomValidator
                        ID="valOptionB"
                        runat="server"
                        ControlToValidate="txtOptionB"
                        ValidateEmptyText="true"
                        ErrorMessage="Option B is required."
                        CssClass="cq-validation-error cq-option-validation"
                        Display="Dynamic"
                        ClientValidationFunction="validateMcqOption"
                        OnServerValidate="ValidateMcqOption"
                        ValidationGroup="QuestionValidation">
                    </asp:CustomValidator>

                </div>



                <%-- =============================================
                     OPTION C
                     ============================================= --%>

                <div class="cq-option-validation-group">

                    <div class="cq-option-row">

                        <asp:RadioButton
                            ID="correctC"
                            runat="server"
                            ClientIDMode="Static"
                            GroupName="CorrectOption" />

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


                    <asp:CustomValidator
                        ID="valOptionC"
                        runat="server"
                        ControlToValidate="txtOptionC"
                        ValidateEmptyText="true"
                        ErrorMessage="Option C is required."
                        CssClass="cq-validation-error cq-option-validation"
                        Display="Dynamic"
                        ClientValidationFunction="validateMcqOption"
                        OnServerValidate="ValidateMcqOption"
                        ValidationGroup="QuestionValidation">
                    </asp:CustomValidator>

                </div>



                <%-- =============================================
                     OPTION D
                     ============================================= --%>

                <div class="cq-option-validation-group">

                    <div class="cq-option-row">

                        <asp:RadioButton
                            ID="correctD"
                            runat="server"
                            ClientIDMode="Static"
                            GroupName="CorrectOption" />

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


                    <asp:CustomValidator
                        ID="valOptionD"
                        runat="server"
                        ControlToValidate="txtOptionD"
                        ValidateEmptyText="true"
                        ErrorMessage="Option D is required."
                        CssClass="cq-validation-error cq-option-validation"
                        Display="Dynamic"
                        ClientValidationFunction="validateMcqOption"
                        OnServerValidate="ValidateMcqOption"
                        ValidationGroup="QuestionValidation">
                    </asp:CustomValidator>

                </div>



                <%-- =============================================
                     CORRECT OPTION VALIDATION
                     ============================================= --%>

                <asp:CustomValidator
                    ID="valCorrectOption"
                    runat="server"
                    ErrorMessage="Please select the correct answer."
                    CssClass="cq-validation-error"
                    Display="Dynamic"
                    ClientValidationFunction="validateCorrectOption"
                    OnServerValidate="ValidateCorrectOption"
                    ValidationGroup="QuestionValidation">
                </asp:CustomValidator>

            </div>



            <%-- =================================================
                 SUBJECTIVE SECTION
                 ================================================= --%>

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


                    <asp:CustomValidator
                        ID="valModelAnswer"
                        runat="server"
                        ControlToValidate="txtModelAnswer"
                        ValidateEmptyText="true"
                        ErrorMessage="Model answer is required for a subjective question."
                        CssClass="cq-validation-error"
                        Display="Dynamic"
                        ClientValidationFunction="validateModelAnswer"
                        OnServerValidate="ValidateModelAnswer"
                        ValidationGroup="QuestionValidation">
                    </asp:CustomValidator>

                </div>


                <p class="cq-model-note">
                    Note: Marks are allocated when this question is added to a quiz.
                </p>

            </div>



            <%-- =================================================
                 OPTIONAL SUPPORT
                 ================================================= --%>

            <div class="cq-support-grid">


                <%-- =============================================
                     HINT
                     ============================================= --%>

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



                <%-- =============================================
                     EXPLANATION
                     ============================================= --%>

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



            <%-- =================================================
                 FORM ACTIONS
                 ================================================= --%>

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
                    CausesValidation="true"
                    ValidationGroup="QuestionValidation"
                    OnClick="btnSaveQuestion_Click" />

            </div>

        </section>

    </div>



    <%-- =========================================================
         QUESTION PREVIEW MODAL
         ========================================================= --%>

    <div id="questionPreviewModal"
         class="question-preview-modal"
         hidden>


        <div class="question-preview-overlay"
             data-preview-close>
        </div>


        <div class="question-preview-dialog"
             role="dialog"
             aria-modal="true"
             aria-labelledby="previewTitle">


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



            <div class="question-preview-body">


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



                <h3 id="previewQuestionText"
                    class="preview-question-text">
                </h3>



                <div id="previewImageArea"
                     class="preview-image-area"
                     hidden>

                    <img id="previewImage"
                         src=""
                         alt="Question stimulus preview" />

                </div>



                <div id="previewMcqAnswers"
                     class="preview-answer-list">


                    <div class="preview-answer"
                         data-preview-option="A">

                        <span>A</span>

                        <p id="previewOptionA"></p>

                    </div>


                    <div class="preview-answer"
                         data-preview-option="B">

                        <span>B</span>

                        <p id="previewOptionB"></p>

                    </div>


                    <div class="preview-answer"
                         data-preview-option="C">

                        <span>C</span>

                        <p id="previewOptionC"></p>

                    </div>


                    <div class="preview-answer"
                         data-preview-option="D">

                        <span>D</span>

                        <p id="previewOptionD"></p>

                    </div>

                </div>



                <div id="previewSubjectiveAnswer"
                     class="preview-subjective"
                     hidden>

                    <label>
                        Model Answer / Scoring Guide
                    </label>

                    <p id="previewModelAnswer"></p>

                </div>

            </div>

        </div>

    </div>


</asp:Content>


<asp:Content
    ID="CreateQuestionScripts"
    ContentPlaceHolderID="PortalScripts"
    runat="server">


    <%-- Shared Preview --%>

    <script src="<%= ResolveUrl("~/Scripts/QuestionPreview.js") %>?v=3">
    </script>



    <script>


        /* =========================================================
           CONDITIONAL VALIDATION
           Must be globally accessible to ASP.NET validators
           ========================================================= */

        window.validateMcqOption =
            function (source, args) {

                const questionType =
                    document.getElementById(
                        "ddlQuestionType"
                    );


                if (
                    !questionType ||
                    questionType.value !== "MCQ"
                ) {
                    args.IsValid =
                        true;

                    return;
                }


                args.IsValid =
                    args.Value.trim().length > 0;
            };


        window.validateModelAnswer =
            function (source, args) {

                const questionType =
                    document.getElementById(
                        "ddlQuestionType"
                    );


                if (
                    !questionType ||
                    questionType.value !== "Subjective"
                ) {
                    args.IsValid =
                        true;

                    return;
                }


                args.IsValid =
                    args.Value.trim().length > 0;
            };


        window.validateCorrectOption =
            function (source, args) {

                const questionType =
                    document.getElementById(
                        "ddlQuestionType"
                    );


                if (
                    !questionType ||
                    questionType.value !== "MCQ"
                ) {
                    args.IsValid =
                        true;

                    return;
                }


                const selected =
                    document.querySelector(
                        "#correctA:checked, " +
                        "#correctB:checked, " +
                        "#correctC:checked, " +
                        "#correctD:checked"
                    );


                args.IsValid =
                    selected !== null;
            };



        /* =========================================================
           PAGE BEHAVIOUR
           ========================================================= */

        (function () {


            /* =====================================================
               ELEMENTS
               ===================================================== */

            const subject =
                document.getElementById(
                    "ddlSubject"
                );


            const questionType =
                document.getElementById(
                    "ddlQuestionType"
                );


            const difficulty =
                document.getElementById(
                    "ddlDifficulty"
                );


            const questionText =
                document.getElementById(
                    "txtQuestion"
                );


            const optionA =
                document.getElementById(
                    "txtOptionA"
                );


            const optionB =
                document.getElementById(
                    "txtOptionB"
                );


            const optionC =
                document.getElementById(
                    "txtOptionC"
                );


            const optionD =
                document.getElementById(
                    "txtOptionD"
                );


            const correctA =
                document.getElementById(
                    "correctA"
                );


            const correctB =
                document.getElementById(
                    "correctB"
                );


            const correctC =
                document.getElementById(
                    "correctC"
                );


            const correctD =
                document.getElementById(
                    "correctD"
                );


            const modelAnswer =
                document.getElementById(
                    "txtModelAnswer"
                );


            const imageInput =
                document.getElementById(
                    "questionImage"
                );


            const removeImageButton =
                document.getElementById(
                    "removeImageButton"
                );


            const selectedFileName =
                document.getElementById(
                    "selectedFileName"
                );


            const existingMediaUrl =
                document.getElementById(
                    "hfExistingMediaUrl"
                );


            const removeExistingImage =
                document.getElementById(
                    "hfRemoveExistingImage"
                );


            const previewButton =
                document.getElementById(
                    "btnPreviewQuestion"
                );


            const mcqSection =
                document.getElementById(
                    "mcqSection"
                );


            const subjectiveSection =
                document.getElementById(
                    "subjectiveSection"
                );



            /* =====================================================
               QUESTION TYPE SWITCHING
               ===================================================== */

            function updateQuestionType() {

                if (
                    !questionType ||
                    !mcqSection ||
                    !subjectiveSection
                ) {
                    return;
                }


                const isMCQ =
                    questionType.value ===
                    "MCQ";


                const isSubjective =
                    questionType.value ===
                    "Subjective";


                mcqSection.hidden =
                    !isMCQ;


                subjectiveSection.hidden =
                    !isSubjective;
            }


            if (questionType) {

                questionType.addEventListener(
                    "change",
                    updateQuestionType
                );

            }



            /* =====================================================
               NEW IMAGE SELECTED
               ===================================================== */

            if (imageInput) {

                imageInput.addEventListener(
                    "change",
                    function () {

                        if (
                            imageInput.files.length > 0
                        ) {

                            if (selectedFileName) {

                                selectedFileName.textContent =
                                    imageInput.files[0].name;

                            }


                            if (removeExistingImage) {

                                removeExistingImage.value =
                                    "false";

                            }

                        }

                    }
                );

            }



            /* =====================================================
               REMOVE IMAGE
               ===================================================== */

            if (removeImageButton) {

                removeImageButton.addEventListener(
                    "click",
                    function() {

                        if (imageInput) {

                            imageInput.value =
                                "";

                        }


                        if (selectedFileName) {

                            selectedFileName.textContent =
                                "";

                        }


                        if (removeExistingImage) {

                            removeExistingImage.value =
                                "true";

                        }


                        window.markQuestionDirty();

                    }
                );

            }



            /* =====================================================
               GET CORRECT OPTION
               ===================================================== */

            function getCorrectOption() {

                if (
                    correctA &&
                    correctA.checked
                ) {
                    return "A";
                }


                if (
                    correctB &&
                    correctB.checked
                ) {
                    return "B";
                }


                if (
                    correctC &&
                    correctC.checked
                ) {
                    return "C";
                }


                if (
                    correctD &&
                    correctD.checked
                ) {
                    return "D";
                }


                return "";
            }



            /* =====================================================
               PREVIEW QUESTION
               ===================================================== */

            if (previewButton) {

                previewButton.addEventListener(
                    "click",
                    function () {

                        let imageUrl =
                            "";


                        /* -----------------------------------------
                           Newly selected image
                           ----------------------------------------- */

                        if (
                            imageInput &&
                            imageInput.files.length > 0
                        ) {

                            imageUrl =
                                URL.createObjectURL(
                                    imageInput.files[0]
                                );

                        }


                        /* -----------------------------------------
                           Existing image in Edit mode
                           ----------------------------------------- */

                        else if (
                            existingMediaUrl &&
                            existingMediaUrl.value &&
                            removeExistingImage &&
                            removeExistingImage.value !==
                            "true"
                        ) {

                            imageUrl =
                                existingMediaUrl.value;

                        }


                        const selectedType =
                            questionType
                                ? questionType.value
                                : "";


                        QuestionPreview.open({

                            subject:
                                subject &&
                                    subject.selectedIndex >= 0
                                    ? subject.options[
                                        subject.selectedIndex
                                    ].text
                                    : "",


                            difficulty:
                                difficulty &&
                                    difficulty.selectedIndex >= 0
                                    ? difficulty.options[
                                        difficulty.selectedIndex
                                    ].text
                                    : "",


                            type:
                                selectedType === "MCQ"
                                    ? "Single Choice"
                                    : selectedType === "Subjective"
                                        ? "Subjective"
                                        : "Question",


                            questionText:
                                questionText
                                    ? questionText.value.trim()
                                    : "",


                            options: [

                                optionA
                                    ? optionA.value.trim()
                                    : "",

                                optionB
                                    ? optionB.value.trim()
                                    : "",

                                optionC
                                    ? optionC.value.trim()
                                    : "",

                                optionD
                                    ? optionD.value.trim()
                                    : ""

                            ],


                            correctOption:
                                getCorrectOption(),


                            modelAnswer:
                                modelAnswer
                                    ? modelAnswer.value.trim()
                                    : "",


                            imageUrl:
                                imageUrl

                        });

                    }
                );

            }

            /* =========================================================
               UNSAVED QUESTION CHANGES
               ========================================================= */

            const questionDirtyField =
                document.getElementById(
                    "hfQuestionDirty"
                );


            window.markQuestionDirty =
                function() {

                    if (questionDirtyField) {

                        questionDirtyField.value =
                            "true";

                    }

                };


            window.hasUnsavedQuestionChanges =
                function() {

                    return (
                        questionDirtyField &&
                        questionDirtyField.value === "true"
                    );

                };


            window.confirmCancelQuestion =
                function() {

                    if (
                        !window.hasUnsavedQuestionChanges()
                    ) {

                        return true;

                    }


                    return window.confirm(

                        "You have unsaved changes to this question.\n\n" +

                        "If you leave now, these changes will be lost.\n\n" +

                        "Do you want to continue?"

                    );

                };

            /* =========================================================
               TRACK QUESTION FORM CHANGES
               ========================================================= */

            document.addEventListener(
                "input",
                function(event) {

                    if (
                        event.target.matches(

                            "#txtQuestion, " +

                            "#txtOptionA, " +
                            "#txtOptionB, " +
                            "#txtOptionC, " +
                            "#txtOptionD, " +

                            "#txtModelAnswer, " +

                            "#txtHint, " +
                            "#txtExplanation"

                        )
                    ) {

                        window.markQuestionDirty();

                    }

                }
            );


            document.addEventListener(
                "change",
                function(event) {

                    if (
                        event.target.matches(

                            "#ddlSubject, " +

                            "#ddlQuestionType, " +

                            "#ddlDifficulty, " +

                            "#correctA, " +
                            "#correctB, " +
                            "#correctC, " +
                            "#correctD, " +

                            "#questionImage"

                        )
                    ) {

                        window.markQuestionDirty();

                    }

                }
            );



            /* =====================================================
               INITIAL STATE
               ===================================================== */

            updateQuestionType();


        })();


    </script>


</asp:Content>