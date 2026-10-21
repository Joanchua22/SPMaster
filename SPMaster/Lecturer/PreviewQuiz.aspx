<%@ Page Title="Quiz Preview" Language="C#" MasterPageFile="~/Master/Portal.Master" AutoEventWireup="true" CodeBehind="PreviewQuiz.aspx.cs" Inherits="SPMaster.Lecturer.PreviewQuiz" %>

<asp:Content ID="PreviewQuizHead" ContentPlaceHolderID="PortalHead" runat="server">
    <link href="<%= ResolveUrl("~/Content/LecturerPreviewQuiz.css") %>?v=2" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="PreviewQuizContent" ContentPlaceHolderID="PortalContent" runat="server">

    <div class="quiz-preview-page">

        <%-- =====================================================
             PAGE HEADER
             ===================================================== --%>

        <section class="quiz-preview-header">

            <%-- LEFT SIDE --%>

            <div class="quiz-preview-heading">
                <span class="quiz-preview-context">
                    <asp:Literal ID="litQuizContext" runat="server"></asp:Literal>
                </span>
                <h1> <asp:Literal ID="litQuizTitle" runat="server"></asp:Literal></h1>
            </div>



            <%-- RIGHT SIDE ACTIONS --%>

            <div class="quiz-preview-header-actions">

                <%-- BACK BUTTON --%>

                <asp:HyperLink ID="lnkBack" runat="server" CssClass="quiz-preview-back-text-button">Back</asp:HyperLink>

                <%-- PUBLISH BUTTON --%>

                <asp:Button ID="btnPublishQuiz" runat="server" CssClass="publish-quiz-button" Text="Publish Quiz Now" OnClientClick="return confirm('Are you sure you want to publish this quiz? Once published, it will become available for use.');" OnClick="btnPublishQuiz_Click" />

            </div>

        </section>



        <%-- =====================================================
             PAGE MESSAGE
             ===================================================== --%>

        <asp:Panel ID="pnlPreviewMessage" runat="server" Visible="false" CssClass="preview-message preview-message-error">

            <i class="bi bi-exclamation-circle-fill" aria-hidden="true"></i>

            <asp:Label ID="lblPreviewMessage"runat="server"></asp:Label>

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



            <%-- PASS PERCENTAGE --%>

            <span class="quiz-summary-dot">•</span>

            <div class="quiz-summary-item">

                <strong> Pass Percentage: </strong>
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
                                <%# Eval("MaxMarks") %> <%# Convert.ToDecimal(Eval("MaxMarks")) == 1 ? "Mark" : "Marks" %>
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

                        <asp:PlaceHolder runat="server" Visible='<%# HasValue( Eval("MediaPath")) %>'>

                            <div class="preview-question-image-wrapper">
                                <img src='<%# GetMediaUrl(Eval("MediaPath")) %>' class="preview-question-image" alt="Question stimulus" />
                            </div>

                        </asp:PlaceHolder>



                        <%-- =========================================
                             MCQ OPTIONS
                             ========================================= --%>

                        <asp:PlaceHolder runat="server" Visible='<%# IsMcq( Eval("QuestionType")) %>'>

                            <div class="preview-option-grid">

                                <%-- OPTION A --%>

                                <div class="preview-option">
                                    <span class="preview-option-letter"> A.</span>
                                    <span> <%#: Eval("OptionA") %> </span>
                                </div>


                                <%-- OPTION B --%>

                                <div class="preview-option">
                                    <span class="preview-option-letter"> B. </span>
                                    <span><%#: Eval("OptionB") %> </span>
                                </div>


                                <%-- OPTION C --%>

                                <div class="preview-option">
                                    <span class="preview-option-letter"> C. </span>
                                    <span><%#: Eval("OptionC") %></span>
                                </div>

                                <%-- OPTION D --%>

                                <div class="preview-option">
                                    <span class="preview-option-letter"> D. </span>
                                    <span> <%#: Eval("OptionD") %> </span>
                                </div>

                            </div>

                        </asp:PlaceHolder>



                        <%-- =========================================
                             SUBJECTIVE STUDENT ANSWER AREA
                             ========================================= --%>

                        <asp:PlaceHolder runat="server" Visible='<%# !IsMcq( Eval("QuestionType")) %>'>

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
                <h3> No questions found.</h3>
                <p> Return to the Quiz Editor and add at least one question.</p>

            </asp:Panel>

        </section>

    </div>

</asp:Content>



<asp:Content ID="PreviewQuizScripts" ContentPlaceHolderID="PortalScripts" runat="server"></asp:Content>