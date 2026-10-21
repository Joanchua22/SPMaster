<%@ Page Title="Quiz Details" Language="C#" AutoEventWireup="true" CodeBehind="ViewQuiz.aspx.cs" Inherits="SPMaster.Student.ViewQuiz" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Quiz Details - SPMaster</title>

    <%-- BOOTSTRAP ICONS --%>

    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.13.1/font/bootstrap-icons.min.css" />

    <%-- PAGE CSS --%>

    <link href="<%= ResolveUrl("~/Content/StudentViewQuiz.css") %>?v=3" rel="stylesheet" type="text/css" />
</head>


<body class="student-view-quiz-body">

    <form id="form1" runat="server">

        <main class="student-view-quiz-page">

            <%-- =================================================
                 QUIZ DETAILS CARD
                 ================================================= --%>

            <section class="student-quiz-details-card">

                <%-- =============================================
                     TOP TAG
                     ============================================= --%>

                <div class="student-quiz-details-top">

                    <span class="student-quiz-subject-tag">
                        <asp:Literal ID="litQuizContext" runat="server"></asp:Literal>
                    </span>

                </div>



                <%-- =============================================
                     QUIZ TITLE
                     ============================================= --%>

                <div class="student-quiz-title-section">

                    <h1><asp:Literal ID="litQuizTitle" runat="server"></asp:Literal></h1>
                    <p>Designed by
                        <strong>
                            <asp:Literal ID="litCreatorName" runat="server"></asp:Literal>
                        </strong>
                    </p>

                </div>



                <%-- =============================================
                     SUMMARY
                     ============================================= --%>

                <section class="student-quiz-summary">

                    <%-- QUESTIONS --%>

                    <div class="student-summary-item">

                        <span class="summary-label">Questions</span>
                        <strong>
                            <asp:Literal ID="litTotalQuestions" runat="server"></asp:Literal>
                        </strong>

                    </div>



                    <%-- TOTAL MARKS --%>

                    <div class="student-summary-item">

                        <span class="summary-label">Total Marks</span>
                        <strong>
                            <asp:Literal ID="litTotalMarks" runat="server"></asp:Literal>
                        </strong>

                    </div>



                    <%-- DURATION --%>

                    <div class="student-summary-item">

                        <span class="summary-label">Duration</span>
                        <strong>
                            <asp:Literal ID="litDuration" runat="server"></asp:Literal>
                        </strong>

                    </div>



                    <%-- FORMAT --%>

                    <div class="student-summary-item">

                        <span class="summary-label">Format</span>
                        <strong>
                            <asp:Literal ID="litFormat" runat="server"></asp:Literal>
                        </strong>

                    </div>

                </section>



                <%-- =============================================
                     QUIZ INSTRUCTIONS FROM DATABASE
                     ============================================= --%>

                <asp:Panel ID="pnlQuizInstructions" runat="server" Visible="false" CssClass="student-quiz-instructions">

                    <h2>Exam Instructions</h2>
                    <p class="student-quiz-description">
                        <asp:Literal ID="litQuizInstructions" runat="server"></asp:Literal>
                    </p>

                    <ul>
                        <asp:PlaceHolder ID="phMcqInstruction" runat="server" Visible="false">

                            <li>
                                <asp:Literal ID="litMcqInstruction" runat="server"></asp:Literal>
                            </li>

                            <li>
                                Objective questions will be graded automatically upon submission.
                            </li>

                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phSubjectiveInstruction" runat="server" Visible="false">

                            <li>
                                <asp:Literal ID="litSubjectiveInstruction" runat="server"></asp:Literal>
                            </li>

                            <li>
                                Subjective answers will be sent to the lecturer for marking.
                            </li>

                        </asp:PlaceHolder>


                        <li>
                            Your progress will be saved while the quiz is in progress.
                        </li>

                    </ul>

                </asp:Panel>



                <%-- =============================================
                     TIMER NOTICE
                     ============================================= --%>

                <div class="student-quiz-timer-notice">

                    <i class="bi bi-stopwatch" aria-hidden="true"></i>

                    <span>The
                        <strong>
                            <asp:Literal ID="litTimerMinutes" runat="server"></asp:Literal>
                        </strong>
                        timer will begin as soon as you click Start Quiz. Ensure a stable internet connection.
                    </span>

                </div>



                <%-- =============================================
                     ACTION BUTTONS
                     ============================================= --%>

                <div class="student-quiz-details-actions">

                    <%-- CANCEL --%>

                    <a href="<%= ResolveUrl("~/Student/Quizzes.aspx") %>" class="student-cancel-button">
                        Cancel
                    </a>



                    <%-- START QUIZ --%>

                    <asp:HyperLink ID="lnkStartQuiz" runat="server" CssClass="student-start-quiz-button">
                        Start Quiz
                        <i class="bi bi-play-fill" aria-hidden="true"></i>
                    </asp:HyperLink>

                </div>

            </section>

        </main>

    </form>

</body>

</html>