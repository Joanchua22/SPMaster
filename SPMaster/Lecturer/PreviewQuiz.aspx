<%@ Page Title="Quiz Preview" Language="C#" AutoEventWireup="true" CodeBehind="PreviewQuiz.aspx.cs" Inherits="SPMaster.Lecturer.PreviewQuiz" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <title>Quiz Preview - SPMaster</title>

    <%-- BOOTSTRAP ICONS --%>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.13.1/font/bootstrap-icons.min.css" />

    <%-- PAGE CSS --%>
    <link href="<%= ResolveUrl("~/Content/LecturerPreviewQuiz.css") %>?v=5" rel="stylesheet" type="text/css" />
</head>

<body class="quiz-preview-body">

<form id="form1" runat="server">

    <main class="quiz-preview-page">

        <%-- =====================================================
             PAGE HEADER
             ===================================================== --%>

        <section class="quiz-preview-header">

            <%-- LEFT SIDE --%>
            <div class="quiz-preview-heading">

                <span class="quiz-preview-context">
                    <asp:Literal ID="litQuizContext" runat="server"></asp:Literal>
                </span>

                <h1>
                    <asp:Literal ID="litQuizTitle" runat="server"></asp:Literal>
                </h1>

            </div>


            <%-- RIGHT SIDE ACTIONS --%>
            <div class="quiz-preview-header-actions">

                <asp:HyperLink ID="lnkBack" runat="server" CssClass="quiz-preview-back-text-button">
                    Back
                </asp:HyperLink>

                <asp:Button ID="btnPublishQuiz" runat="server" CssClass="publish-quiz-button" Text="Publish Quiz Now" OnClientClick="return confirm('Are you sure you want to publish this quiz? Once published, it will become available for use.');" OnClick="btnPublishQuiz_Click" />

            </div>

        </section>


        <%-- =====================================================
             PAGE MESSAGE
             ===================================================== --%>

        <asp:Panel ID="pnlPreviewMessage" runat="server" Visible="false" CssClass="preview-message preview-message-error">

            <i class="bi bi-exclamation-circle-fill" aria-hidden="true"></i>

            <asp:Label ID="lblPreviewMessage" runat="server"></asp:Label>

        </asp:Panel>


        <%-- =====================================================
             QUIZ SUMMARY
             ===================================================== --%>

        <section class="quiz-preview-summary">

            <%-- TOTAL QUESTIONS --%>
            <div class="quiz-summary-item">
                <strong>Total Questions:</strong>
                <span>
                    <asp:Literal ID="litTotalQuestions" runat="server"></asp:Literal>
                </span>
            </div>

            <span class="quiz-summary-dot">•</span>


            <%-- TOTAL MARKS --%>
            <div class="quiz-summary-item">
                <strong>Total Marks:</strong>
                <span>
                    <asp:Literal ID="litTotalMarks" runat="server"></asp:Literal>
                </span>
            </div>

            <span class="quiz-summary-dot">•</span>


            <%-- TIME LIMIT --%>
            <div class="quiz-summary-item">
                <strong>Time Limit:</strong>
                <span>
                    <asp:Literal ID="litTimeLimit" runat="server"></asp:Literal>
                </span>
            </div>

            <span class="quiz-summary-dot">•</span>


            <%-- PASS PERCENTAGE --%>
            <div class="quiz-summary-item">
                <strong>Pass Percentage:</strong>
                <span>
                    <asp:Literal ID="litPassPercentage" runat="server"></asp:Literal>
                </span>
            </div>

        </section>


        <%-- =====================================================
             QUIZ INSTRUCTIONS
             ===================================================== --%>

        <asp:Panel ID="pnlQuizInstructions" runat="server" Visible="false" CssClass="quiz-preview-instructions">

            <strong>Instructions:</strong>

            <asp:Label ID="lblQuizInstructions" runat="server"></asp:Label>

        </asp:Panel>


        <%-- =====================================================
             QUESTION LIST
             ===================================================== --%>

        <section class="quiz-preview-question-list">

            <asp:Repeater ID="rptPreviewQuestions" runat="server">

                <ItemTemplate>

                    <article class="quiz-preview-question-card">

                        <%-- =========================================
                             QUESTION HEADER
                             ========================================= --%>

                        <div class="preview-question-header">

                            <span class="preview-question-number">
                                Question <%# Eval("QuestionOrder") %> of <%# Eval("TotalQuestions") %> (<%# GetQuestionTypeDisplay(Eval("QuestionType")) %>)
                            </span>

                            <span class="preview-question-marks">
                                <%# Convert.ToDecimal(Eval("MaxMarks")).ToString("0.##") %> <%# Convert.ToDecimal(Eval("MaxMarks")) == 1 ? "Mark" : "Marks" %>
                            </span>

                        </div>


                        <%-- =========================================
                             QUESTION TEXT
                             ========================================= --%>

                        <h2 class="preview-question-title">
                            <%#: Eval("QuestionText") %>
                        </h2>


                        <%-- =========================================
                             OPTIONAL QUESTION IMAGE
                             ========================================= --%>

                        <asp:PlaceHolder runat="server" Visible='<%# HasValue(Eval("MediaPath")) %>'>

                            <div class="preview-question-image-wrapper">
                                <img src='<%# GetMediaUrl(Eval("MediaPath")) %>' class="preview-question-image" alt="Question stimulus" />
                            </div>

                        </asp:PlaceHolder>


                        <%-- =========================================
                             MCQ OPTIONS
                             ========================================= --%>

                        <asp:PlaceHolder runat="server" Visible='<%# IsMcq(Eval("QuestionType")) %>'>

                            <div class="preview-option-grid">

                                <%-- OPTION A --%>
                                <div class="preview-option">
                                    <span class="preview-option-letter">A.</span>
                                    <span><%#: Eval("OptionA") %></span>
                                </div>

                                <%-- OPTION B --%>
                                <div class="preview-option">
                                    <span class="preview-option-letter">B.</span>
                                    <span><%#: Eval("OptionB") %></span>
                                </div>

                                <%-- OPTION C --%>
                                <div class="preview-option">
                                    <span class="preview-option-letter">C.</span>
                                    <span><%#: Eval("OptionC") %></span>
                                </div>

                                <%-- OPTION D --%>
                                <div class="preview-option">
                                    <span class="preview-option-letter">D.</span>
                                    <span><%#: Eval("OptionD") %></span>
                                </div>

                            </div>

                        </asp:PlaceHolder>


                        <%-- =========================================
                             SUBJECTIVE STUDENT ANSWER AREA
                             ========================================= --%>

                        <asp:PlaceHolder runat="server" Visible='<%# !IsMcq(Eval("QuestionType")) %>'>

                            <div class="preview-subjective-answer-area">
                                <span>Student written answer area</span>
                            </div>

                        </asp:PlaceHolder>

                    </article>

                </ItemTemplate>

            </asp:Repeater>


            <%-- =================================================
                 NO QUESTIONS
                 ================================================= --%>

            <asp:Panel ID="pnlNoPreviewQuestions" runat="server" Visible="false" CssClass="quiz-preview-empty">

                <i class="bi bi-question-circle" aria-hidden="true"></i>

                <h3>No questions found.</h3>

                <p>
                    Return to the Quiz Editor and add at least one question.
                </p>

            </asp:Panel>

        </section>

    </main>

</form>

</body>

</html>