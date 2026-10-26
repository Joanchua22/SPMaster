<%@ Page Title="Quiz Result" Language="C#" AutoEventWireup="true" CodeBehind="QuizResult.aspx.cs" Inherits="SPMaster.Student.QuizResult" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <title>Quiz Result - SPMaster</title>

    <%-- BOOTSTRAP ICONS --%>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.13.1/font/bootstrap-icons.min.css" />

    <%-- PAGE CSS --%>
    <link href="<%= ResolveUrl("~/Content/StudentQuizResult.css") %>?v=7" rel="stylesheet" type="text/css" />
</head>

<body class="student-result-body">

<form id="form1" runat="server">

    <main class="student-result-page">

        <%-- =====================================================
             RESULT SUMMARY
             ===================================================== --%>

        <section class="result-summary-card">

            <div class="result-summary-info">

                <%-- GRADED --%>
                <asp:Panel ID="pnlGradedBadge" runat="server" Visible="false" CssClass="result-status-badge result-status-graded">
                    Quiz Completed &amp; Graded
                </asp:Panel>

                <%-- PENDING GRADING --%>
                <asp:Panel ID="pnlPendingBadge" runat="server" Visible="false" CssClass="result-status-badge result-status-pending">
                    Pending Subjective Grading
                </asp:Panel>

                <%-- QUIZ TITLE --%>
                <h1>
                    <asp:Literal ID="litQuizTitle" runat="server"></asp:Literal>
                </h1>

                <%-- COMPLETION INFORMATION --%>
                <p class="result-completion-info">
                    Completed on
                    <asp:Literal ID="litCompletedDate" runat="server"></asp:Literal>
                    <span>•</span>
                    Time spent:
                    <asp:Literal ID="litTimeSpent" runat="server"></asp:Literal>
                </p>

            </div>

            <%-- GRADED SCORE --%>
            <asp:Panel ID="pnlGradedScore" runat="server" Visible="false" CssClass="result-score-card">

                <span class="result-score-label">
                    Final Score
                </span>

                <strong class="result-score">
                    <asp:Literal ID="litFinalScore" runat="server"></asp:Literal>
                </strong>

                <span class="result-score-status result-score-status-graded">
                    <asp:Literal ID="litFinalPercentage" runat="server"></asp:Literal>
                </span>

            </asp:Panel>

            <%-- PENDING SCORE --%>
            <asp:Panel ID="pnlPendingScore" runat="server" Visible="false" CssClass="result-score-card">

                <span class="result-score-label">
                    Current Score
                </span>

                <strong class="result-score">
                    <asp:Literal ID="litCurrentScore" runat="server"></asp:Literal>
                </strong>

                <span class="result-score-status result-score-status-pending">
                    <asp:Literal ID="litPendingCount" runat="server"></asp:Literal>
                </span>

            </asp:Panel>

        </section>


        <%-- =====================================================
             QUESTION BREAKDOWN
             ===================================================== --%>

        <h2 class="result-breakdown-title">
            Question Breakdown
        </h2>


        <%-- =====================================================
             QUESTIONS
             ===================================================== --%>

        <asp:Repeater ID="rptResultQuestions" runat="server">

            <ItemTemplate>

                <article class="result-question-card">

                    <%-- =========================================
                         QUESTION HEADER
                         ========================================= --%>

                    <div class="result-question-header">

                        <div class="result-question-heading">

                            <span class='<%# GetQuestionIconCss(Eval("QuestionType"), Eval("DisplayMarksAwarded"), Eval("MaxMarks")) %>'>
                                <i class='<%# GetQuestionIcon(Eval("QuestionType"), Eval("DisplayMarksAwarded"), Eval("MaxMarks")) %>' aria-hidden="true"></i>
                            </span>

                            <span>
                                Question <%#: Eval("QuestionOrder") %> • <%#: GetQuestionTypeText(Eval("QuestionType")) %>
                            </span>

                        </div>

                        <span class='<%# GetQuestionMarksCss(Eval("QuestionType"), Eval("DisplayMarksAwarded")) %>'>
                            <%#: GetQuestionMarksText(Eval("QuestionType"), Eval("DisplayMarksAwarded"), Eval("MaxMarks")) %>
                        </span>

                    </div>


                    <%-- =========================================
                         QUESTION TEXT
                         ========================================= --%>

                    <p class="result-question-text">
                        <%#: Eval("QuestionText") %>
                    </p>


                    <%-- =========================================
                         OBJECTIVE / MCQ RESULT
                         ========================================= --%>

                    <asp:Panel ID="pnlObjectiveResult" runat="server" Visible='<%# IsMcq(Eval("QuestionType")) %>'>

                        <%-- STUDENT ANSWER --%>
                        <div class='<%# GetObjectiveAnswerCss(Eval("SelectedOptionId"), Eval("SelectedOptionIsCorrect")) %>'>

                            <strong>Your Answer:</strong>

                            <span>
                                <%#: GetSelectedAnswerText(Eval("SelectedOptionText")) %>
                            </span>

                            <span>
                                <%#: GetObjectiveResultText(Eval("SelectedOptionId"), Eval("SelectedOptionIsCorrect")) %>
                            </span>

                        </div>


                        <%-- CORRECT ANSWER --%>
                        <asp:Panel ID="pnlCorrectAnswer" runat="server" Visible='<%# ShowCorrectAnswer(Eval("SelectedOptionId"), Eval("SelectedOptionIsCorrect")) %>' CssClass="result-correct-answer-box">
                            <strong>Correct Answer:</strong>
                            <%#: Eval("CorrectOptionText") %>
                        </asp:Panel>


                        <%-- EXPLANATION --%>
                        <asp:Panel ID="pnlExplanation" runat="server" Visible='<%# HasText(Eval("Explanation")) %>' CssClass="result-explanation-box">
                            <strong>Explanation:</strong>
                            <%#: Eval("Explanation") %>
                        </asp:Panel>

                    </asp:Panel>


                    <%-- =========================================
                         SUBJECTIVE RESULT
                         ========================================= --%>

                    <asp:Panel ID="pnlSubjectiveResult" runat="server" Visible='<%# !IsMcq(Eval("QuestionType")) %>'>

                        <%-- STUDENT SUBMITTED ANSWER --%>
                        <div class="result-submitted-answer">

                            <strong>Your Submitted Answer:</strong>

                            <div class="result-submitted-answer-content">
                                <%#: GetSubjectiveAnswer(Eval("AnswerText")) %>
                            </div>

    </div>


                        <%-- MODEL ANSWER / SCORING GUIDE --%>
                        <asp:Panel ID="pnlModelAnswer" runat="server" Visible='<%# HasText(Eval("ModelAnswer")) %>' CssClass="result-inline-info result-inline-model">
                            <span class="result-inline-heading">
                                <strong>Model Answer / Scoring Guide</strong>
                            </span>
                            <span class="result-inline-colon">:</span>
                            <span class="result-inline-text"><%#: Eval("ModelAnswer") %></span>
                        </asp:Panel>


                        <%-- POST-SUBMISSION EXPLANATION --%>
                        <asp:Panel ID="pnlSubjectiveExplanation" runat="server" Visible='<%# HasText(Eval("Explanation")) %>' CssClass="result-inline-info result-inline-explanation">
                            <span class="result-inline-heading">
                                <strong>Explanation</strong>
                            </span>
                            <span class="result-inline-colon">:</span>
                            <span class="result-inline-text"><%#: Eval("Explanation") %></span>
                        </asp:Panel>


                        <%-- LECTURER FEEDBACK --%>
                        <asp:Panel ID="pnlLecturerFeedback" runat="server" Visible='<%# HasText(Eval("LecturerFeedback")) %>' CssClass="result-inline-info result-inline-feedback">
                            <span class="result-inline-heading">
                                <strong>Lecturer Feedback</strong>
                            </span>
                            <span class="result-inline-colon">:</span>
                            <span class="result-inline-text"><%#: Eval("LecturerFeedback") %></span>
                        </asp:Panel>

                    </asp:Panel>

                </article>

            </ItemTemplate>

        </asp:Repeater>

        <%-- =====================================================
             ACTION BUTTONS
             ===================================================== --%>

        <div class="result-page-actions">

            <asp:HyperLink ID="lnkTryAgain" runat="server" CssClass="result-retry-button">
                Try Again (New Attempt)
            </asp:HyperLink>

            <a href="<%= ResolveUrl("~/Student/Quizzes.aspx?tab=attempts") %>" class="result-done-button">
                Done Reviewing
            </a>

        </div>

    </main>

</form>

</body>

</html>