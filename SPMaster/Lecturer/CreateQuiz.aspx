<%@ Page Title="Quiz Editor" Language="C#" AutoEventWireup="true" CodeBehind="CreateQuiz.aspx.cs" Inherits="SPMaster.Lecturer.CreateQuiz" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <title>Quiz Editor - SPMaster</title>

    <%-- BOOTSTRAP ICONS --%>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.13.1/font/bootstrap-icons.min.css" />

    <%-- PAGE CSS --%>
    <link href="<%= ResolveUrl("~/Content/LecturerCreateQuiz.css") %>?v=5" rel="stylesheet" type="text/css" />

    <%-- VALIDATION STYLING --%>
    <style>
        .quiz-validation-error {
            display: block;
            margin-top: 6px;
            color: #ff334b;
            font-family: "Segoe UI", Arial, sans-serif;
            font-size: 13px;
            font-weight: 400;
            line-height: 1.35;
        }

        .allocation-validation-error {
            margin: 0 0 14px;
        }
    </style>
</head>

<body class="create-quiz-body">

<form id="form1" runat="server">

    <main class="create-quiz-standalone">

        <div class="create-quiz-page">

            <%-- =================================================
                 UNSAVED CHANGES
                 ================================================= --%>

            <asp:HiddenField ID="hfQuizDirty" runat="server" ClientIDMode="Static" Value="false" />


            <%-- =================================================
                 PAGE HEADER
                 ================================================= --%>

            <section class="create-quiz-header">

                <div>
                    <h1>Quiz Editor</h1>
                    <p>Define quiz parameters and curate questions from your repository</p>
                </div>

                <div class="create-quiz-header-actions">

                    <%-- CANCEL --%>
                    <a href="<%= ResolveUrl("~/Lecturer/Quizzes.aspx") %>" class="create-quiz-cancel-button" onclick="return window.confirmCancelQuiz();">
                        Cancel
                    </a>

                    <%-- SAVE DRAFT --%>
                    <asp:Button ID="btnSaveDraft" runat="server" CssClass="create-quiz-cancel-button" Text="Save Draft" CausesValidation="true" ValidationGroup="QuizValidation" OnClick="btnSaveDraft_Click" />

                    <%-- PREVIEW AND PUBLISH --%>
                    <asp:LinkButton ID="btnPreviewPublish" runat="server" CssClass="create-quiz-publish-button" CausesValidation="true" ValidationGroup="QuizValidation" OnClick="btnPreviewPublish_Click">
                        Preview &amp; Publish
                        <i class="bi bi-arrow-right" aria-hidden="true"></i>
                    </asp:LinkButton>

                </div>

            </section>


            <%-- =================================================
                 SAVE SUCCESS MESSAGE
                 ================================================= --%>

            <asp:Panel ID="pnlSaveSuccess" runat="server" Visible="false" CssClass="quiz-save-message">
                <i class="bi bi-check-circle-fill" aria-hidden="true"></i>
                <asp:Label ID="lblSaveSuccess" runat="server"></asp:Label>
            </asp:Panel>


            <%-- =================================================
                 1. GENERAL INFORMATION
                 ================================================= --%>

            <section class="create-quiz-card">

                <div class="create-quiz-section-heading">
                    <h2>1. General Information</h2>
                </div>

                <div class="quiz-general-grid">

                    <%-- =========================================
                         QUIZ TITLE
                         ========================================= --%>

                    <div class="quiz-field quiz-field-wide">

                        <label for="txtQuizTitle">
                            Quiz Title
                            <span class="required-mark">*</span>
                        </label>

                        <asp:TextBox ID="txtQuizTitle" runat="server" ClientIDMode="Static" CssClass="quiz-input" placeholder="Enter quiz title"></asp:TextBox>

                        <asp:RequiredFieldValidator ID="valQuizTitle" runat="server" ClientIDMode="Static" ControlToValidate="txtQuizTitle" ErrorMessage="Quiz title is required." CssClass="quiz-validation-error" Display="Dynamic" SetFocusOnError="true" ValidationGroup="QuizValidation"></asp:RequiredFieldValidator>

                    </div>


                    <%-- =========================================
                         SUBJECT
                         ========================================= --%>

                    <div class="quiz-field">

                        <label for="ddlQuizSubject">
                            Subject
                            <span class="required-mark">*</span>
                        </label>

                        <div class="quiz-select-wrapper">

                            <asp:DropDownList ID="ddlQuizSubject" runat="server" ClientIDMode="Static" CssClass="quiz-select" AutoPostBack="true" CausesValidation="false" onchange="if (!window.confirmSubjectChange(this)) { return false; }" OnSelectedIndexChanged="ddlQuizSubject_SelectedIndexChanged"></asp:DropDownList>

                            <i class="bi bi-caret-down-fill quiz-select-arrow" aria-hidden="true"></i>

                        </div>

                        <asp:RequiredFieldValidator ID="valQuizSubject" runat="server" ClientIDMode="Static" ControlToValidate="ddlQuizSubject" InitialValue="" ErrorMessage="Please select a subject." CssClass="quiz-validation-error" Display="Dynamic" SetFocusOnError="true" ValidationGroup="QuizValidation"></asp:RequiredFieldValidator>

                    </div>


                    <%-- =========================================
                         FORM LEVEL
                         ========================================= --%>

                    <div class="quiz-field">

                        <label for="ddlFormLevel">
                            Form Level
                            <span class="required-mark">*</span>
                        </label>

                        <div class="quiz-select-wrapper">

                            <asp:DropDownList ID="ddlFormLevel" runat="server" ClientIDMode="Static" CssClass="quiz-select"></asp:DropDownList>

                            <i class="bi bi-caret-down-fill quiz-select-arrow" aria-hidden="true"></i>

                        </div>

                        <asp:RequiredFieldValidator ID="valFormLevel" runat="server" ClientIDMode="Static" ControlToValidate="ddlFormLevel" InitialValue="" ErrorMessage="Please select a form level." CssClass="quiz-validation-error" Display="Dynamic" SetFocusOnError="true" ValidationGroup="QuizValidation"></asp:RequiredFieldValidator>

                    </div>


                    <%-- =========================================
                         TIME LIMIT
                         ========================================= --%>

                    <div class="quiz-field">

                        <label for="txtTimeLimit">
                            Time Limit (Minutes)
                            <span class="required-mark">*</span>
                        </label>

                        <asp:TextBox ID="txtTimeLimit" runat="server" ClientIDMode="Static" CssClass="quiz-input" TextMode="Number" min="1" placeholder="e.g. 30"></asp:TextBox>

                        <asp:RequiredFieldValidator ID="valTimeLimitRequired" runat="server" ClientIDMode="Static" ControlToValidate="txtTimeLimit" ErrorMessage="Time limit is required." CssClass="quiz-validation-error" Display="Dynamic" SetFocusOnError="true" ValidationGroup="QuizValidation"></asp:RequiredFieldValidator>

                        <asp:RangeValidator ID="valTimeLimitRange" runat="server" ClientIDMode="Static" ControlToValidate="txtTimeLimit" Type="Integer" MinimumValue="1" MaximumValue="1440" ErrorMessage="Time limit must be between 1 and 1440 minutes." CssClass="quiz-validation-error" Display="Dynamic" SetFocusOnError="true" ValidationGroup="QuizValidation"></asp:RangeValidator>

                    </div>


                    <%-- =========================================
                         PASS PERCENTAGE
                         ========================================= --%>

                    <div class="quiz-field">

                        <label for="txtPassPercentage">
                            Pass Percentage
                            <span class="required-mark">*</span>
                        </label>

                        <div class="quiz-percentage-input">

                            <asp:TextBox ID="txtPassPercentage" runat="server" ClientIDMode="Static" CssClass="quiz-input" TextMode="Number" min="0" max="100" placeholder="50"></asp:TextBox>

                            <span>%</span>

                        </div>

                        <asp:RequiredFieldValidator ID="valPassPercentageRequired" runat="server" ClientIDMode="Static" ControlToValidate="txtPassPercentage" ErrorMessage="Pass percentage is required." CssClass="quiz-validation-error" Display="Dynamic" SetFocusOnError="true" ValidationGroup="QuizValidation"></asp:RequiredFieldValidator>

                        <asp:RangeValidator ID="valPassPercentageRange" runat="server" ClientIDMode="Static" ControlToValidate="txtPassPercentage" Type="Double" MinimumValue="0" MaximumValue="100" ErrorMessage="Pass percentage must be between 0 and 100." CssClass="quiz-validation-error" Display="Dynamic" SetFocusOnError="true" ValidationGroup="QuizValidation"></asp:RangeValidator>

                    </div>


                    <%-- =========================================
                         INSTRUCTIONS
                         ========================================= --%>

                    <div class="quiz-field quiz-field-instructions">

                        <label for="txtQuizInstructions">
                            Instructions for Students
                        </label>

                        <asp:TextBox ID="txtQuizInstructions" runat="server" ClientIDMode="Static" CssClass="quiz-input" placeholder="Enter instructions for students"></asp:TextBox>

                    </div>

                </div>

            </section>


            <%-- =================================================
                 2. QUESTIONS ALLOCATION
                 ================================================= --%>

            <section class="create-quiz-card questions-allocation-card">

                <%-- =============================================
                     SECTION HEADER
                     ============================================= --%>

                <div class="question-allocation-header">

                    <div>
                        <h2>2. Questions Allocation</h2>
                        <p>Reorder items, designate mark weightage, and manage structure</p>
                    </div>

                    <div class="question-allocation-actions">

                        <span id="quizSummaryPill" class="quiz-summary-pill">
                            <asp:Literal ID="litQuizSummary" runat="server"></asp:Literal>
                        </span>

                        <button type="button" id="btnOpenQuestionBank" class="add-from-bank-button">
                            <i class="bi bi-plus-circle" aria-hidden="true"></i>
                            Add from Bank
                        </button>

                    </div>

                </div>


                <%-- =============================================
                     QUESTION-LEVEL VALIDATION
                     ============================================= --%>

                <asp:Label ID="lblQuestionValidation" runat="server" Visible="false" CssClass="quiz-validation-error allocation-validation-error"></asp:Label>


                <%-- =============================================
                     ALLOCATED QUESTIONS
                     ============================================= --%>

                <asp:Repeater ID="rptAllocatedQuestions" runat="server" OnItemCommand="rptAllocatedQuestions_ItemCommand">

                    <ItemTemplate>

                        <article class="allocated-question-row">

                            <asp:HiddenField ID="hfAllocatedQuestionId" runat="server" Value='<%# Eval("QuestionId") %>' />


                            <%-- QUESTION ORDER --%>
                            <div class="question-order">
                                <%#: Eval("QuestionOrder") %>
                            </div>


                            <%-- QUESTION INFORMATION --%>
                            <div class="allocated-question-content">

                                <div class="allocated-question-tags">

                                    <span class='allocated-type-tag <%# GetQuestionTypeCss(Eval("QuestionType")) %>'>
                                        <%#: GetQuestionTypeDisplay(Eval("QuestionType")) %>
                                    </span>

                                    <span class="allocated-question-source">
                                        <%#: Eval("SubjectName") %> • <%#: Eval("Difficulty") %>
                                    </span>

                                </div>

                                <h3><%#: Eval("QuestionText") %></h3>

                            </div>


                            <%-- QUESTION CONTROLS --%>
                            <div class="allocated-question-controls">

                                <label>Marks:</label>

                                <asp:TextBox ID="txtMarks" runat="server" CssClass="question-marks-input" TextMode="Number" min="1" Text='<%# Eval("MaxMarks") %>'></asp:TextBox>

                                <asp:LinkButton runat="server" CssClass="remove-question-button" CommandName="RemoveQuestion" CommandArgument='<%# Eval("QuestionId") %>' CausesValidation="false" OnClientClick="window.markQuizDirty();" aria-label="Remove question">
                                    <i class="bi bi-trash3" aria-hidden="true"></i>
                                </asp:LinkButton>

                            </div>

                        </article>

                    </ItemTemplate>

                </asp:Repeater>


                <%-- =============================================
                     EMPTY ALLOCATION
                     ============================================= --%>

                <asp:Panel ID="pnlNoAllocatedQuestions" runat="server" Visible="false">

                    <div class="empty-questions-state">
                        <h3>No questions added yet.</h3>
                        <p>Use Add from Bank to choose questions for this quiz.</p>
                    </div>

                </asp:Panel>

            </section>

        </div>


        <%-- =====================================================
             CHOOSE QUESTIONS FROM BANK MODAL
             ===================================================== --%>

        <div id="questionBankModal" class="question-bank-modal" hidden>

            <%-- OVERLAY --%>
            <div class="question-bank-modal-overlay" data-bank-close></div>


            <%-- MODAL DIALOG --%>
            <div class="question-bank-modal-dialog" role="dialog" aria-modal="true" aria-labelledby="questionBankModalTitle">

                <%-- =========================================
                     MODAL HEADER
                     ========================================= --%>

                <div class="question-bank-modal-header">

                    <div>

                        <h2 id="questionBankModalTitle">
                            Choose Questions from Bank
                        </h2>

                        <p>
                            Matching
                            <asp:Literal ID="litBankSubject" runat="server"></asp:Literal>
                            questions
                        </p>

                    </div>

                    <button type="button" class="question-bank-modal-close" data-bank-close aria-label="Close">
                        <i class="bi bi-x-lg" aria-hidden="true"></i>
                    </button>

                </div>


                <%-- =========================================
                     QUESTION LIST
                     ========================================= --%>

                <div class="bank-question-list">

                    <asp:Repeater ID="rptBankQuestions" runat="server">

                        <ItemTemplate>

                            <div class="bank-question-item">

                                <asp:HiddenField ID="hfBankQuestionId" runat="server" Value='<%# Eval("QuestionId") %>' />

                                <asp:CheckBox ID="chkSelectQuestion" runat="server" CssClass="bank-question-checkbox" />

                                <div>

                                    <div class="bank-question-meta">
                                        <span><%#: GetQuestionTypeDisplay(Eval("QuestionType")) %></span>
                                        <span>•</span>
                                        <span><%#: Eval("Difficulty") %></span>
                                        <span>•</span>
                                        <span><%#: Eval("CreatorName") %></span>
                                    </div>

                                    <p><%#: Eval("QuestionText") %></p>

                                </div>

                            </div>

                        </ItemTemplate>

                    </asp:Repeater>


                    <%-- NO AVAILABLE QUESTIONS --%>
                    <asp:Panel ID="pnlNoBankQuestions" runat="server" Visible="false">
                        <div class="bank-question-item">
                            No available questions found for this subject.
                        </div>
                    </asp:Panel>

                </div>


                <%-- =========================================
                     MODAL FOOTER
                     ========================================= --%>

                <div class="question-bank-modal-footer">

                    <span>Duplicate checking enabled</span>

                    <asp:Button ID="btnAddToQuiz" runat="server" CssClass="add-selected-questions-button" Text="Add to Quiz" CausesValidation="false" OnClientClick="window.markQuizDirty();" OnClick="btnAddToQuiz_Click" />

                </div>

            </div>

        </div>

    </main>


    <%-- =========================================================
         PAGE JAVASCRIPT
         ========================================================= --%>

    <script>

        /* =========================================================
           DIRTY / UNSAVED CHANGES
           ========================================================= */

        const quizDirtyField = document.getElementById("hfQuizDirty");

        window.markQuizDirty = function () {
            if (quizDirtyField) {
                quizDirtyField.value = "true";
            }
        };

        window.hasUnsavedChanges = function () {
            return quizDirtyField && quizDirtyField.value === "true";
        };


        /* =========================================================
           CANCEL CONFIRMATION
           ========================================================= */

        window.confirmCancelQuiz = function () {
            if (!window.hasUnsavedChanges()) {
                return true;
            }

            return window.confirm(
                "You have unsaved changes to this quiz.\n\n" +
                "If you leave now, these changes will be lost.\n\n" +
                "Do you want to continue?"
            );
        };


        /* =========================================================
           TRACK GENERAL FORM CHANGES
           ========================================================= */

        document.addEventListener("input", function (event) {
            if (
                event.target.matches(
                    "#txtQuizTitle, " +
                    "#txtTimeLimit, " +
                    "#txtPassPercentage, " +
                    "#txtQuizInstructions, " +
                    ".question-marks-input"
                )
            ) {
                window.markQuizDirty();
            }
        });

        document.addEventListener("change", function (event) {
            if (event.target.matches("#ddlFormLevel")) {
                window.markQuizDirty();
            }
        });


        /* =========================================================
           SUBJECT CHANGE CONFIRMATION
           ========================================================= */

        const subjectDropdown = document.getElementById("ddlQuizSubject");
        const subjectValidator = document.getElementById("valQuizSubject");

        let previousSubjectValue = subjectDropdown
            ? subjectDropdown.value
            : "";

        window.confirmSubjectChange = function (dropdown) {
            const newValue = dropdown.value;


            /* -----------------------------------------------------
               NOTHING ACTUALLY CHANGED
               ----------------------------------------------------- */

            if (newValue === previousSubjectValue) {
                return true;
            }


            /* -----------------------------------------------------
               CHECK ALLOCATED QUESTIONS
               ----------------------------------------------------- */

            const allocatedQuestions = document.querySelectorAll(
                ".allocated-question-row"
            );

            if (allocatedQuestions.length > 0) {
                const confirmed = window.confirm(
                    "Changing the subject may remove questions " +
                    "that do not match the new subject.\n\n" +
                    "Do you want to continue?"
                );

                if (!confirmed) {
                    dropdown.value = previousSubjectValue;
                    return false;
                }
            }

            previousSubjectValue = newValue;

            if (subjectValidator && newValue) {
                subjectValidator.style.display = "none";
            }

            window.markQuizDirty();

            return true;
        };


        /* =========================================================
           LIVE MARKS SUMMARY
           ========================================================= */

        const quizSummary = document.getElementById("quizSummaryPill");

        function updateQuizSummary() {
            const markInputs = document.querySelectorAll(
                ".question-marks-input"
            );

            let totalMarks = 0;

            markInputs.forEach(function (input) {
                const marks = parseInt(
                    input.value,
                    10
                );

                if (!isNaN(marks) && marks >= 1) {
                    totalMarks += marks;
                }
            });

            if (quizSummary) {
                quizSummary.textContent =
                    markInputs.length +
                    " Questions • " +
                    totalMarks +
                    " Total Marks";
            }
        }

        document.addEventListener("input", function (event) {
            if (event.target.matches(".question-marks-input")) {
                updateQuizSummary();
            }
        });

        updateQuizSummary();


        /* =========================================================
           QUESTION BANK MODAL
           ========================================================= */

        (function () {
            const openButton = document.getElementById("btnOpenQuestionBank");
            const modal = document.getElementById("questionBankModal");

            if (!openButton || !modal) {
                return;
            }


            /* =====================================================
               OPEN QUESTION BANK
               ===================================================== */

            openButton.addEventListener("click", function () {

                /* -------------------------------------------------
                   SUBJECT MUST BE SELECTED FIRST
                   ------------------------------------------------- */

                if (!subjectDropdown || !subjectDropdown.value) {
                    if (subjectValidator) {
                        subjectValidator.style.display = "block";
                    }

                    if (subjectDropdown) {
                        subjectDropdown.focus();
                    }

                    return;
                }


                /* -------------------------------------------------
                   OPEN MODAL
                   ------------------------------------------------- */

                modal.hidden = false;
                document.body.style.overflow = "hidden";

                const closeButton = modal.querySelector(
                    ".question-bank-modal-close"
                );

                if (closeButton) {
                    closeButton.focus();
                }
            });


            /* =====================================================
               CLOSE MODAL
               ===================================================== */

            function closeQuestionBankModal() {
                modal.hidden = true;
                document.body.style.overflow = "";

                if (openButton) {
                    openButton.focus();
                }
            }

            document.addEventListener("click", function (event) {
                if (event.target.closest("[data-bank-close]")) {
                    closeQuestionBankModal();
                }
            });


            /* =====================================================
               ESCAPE KEY
               ===================================================== */

            document.addEventListener("keydown", function (event) {
                if (event.key === "Escape" && !modal.hidden) {
                    closeQuestionBankModal();
                }
            });

        })();

    </script>

</form>

</body>

</html>