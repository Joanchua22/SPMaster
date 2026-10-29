<%@ Page Title="Grade Submission"
    Language="C#"
    AutoEventWireup="true"
    CodeBehind="GradeDetails.aspx.cs"
    Inherits="SPMaster.Lecturer.GradeDetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <meta charset="utf-8" />

    <meta name="viewport"
          content="width=device-width, initial-scale=1.0" />

    <title>Grade Submission - SPMaster</title>

    <link rel="stylesheet"
          href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.13.1/font/bootstrap-icons.min.css" />

    <link href="<%= ResolveUrl("~/Content/Portal.css") %>?v=1"
          rel="stylesheet"
          type="text/css" />

    <link href="<%= ResolveUrl("~/Content/Grading.css") %>?v=1"
          rel="stylesheet"
          type="text/css" />

</head>


<body class="portal-body">

    <form id="form1"
          runat="server"
          class="portal-wrapper">

        <main class="portal-main">

            <div class="grading-page">

                <%-- =================================================
                     TOP BAR
                     ================================================= --%>

                <section class="grading-detail-header">

                    <div class="grading-detail-heading">

                        <asp:HyperLink
                            ID="lnkBackToQueue"
                            runat="server"
                            NavigateUrl="~/Lecturer/Grading.aspx"
                            CssClass="grading-back-button">

                            <i class="bi bi-arrow-left"></i>

                        </asp:HyperLink>

                        <div>

                            <span class="grading-context" style="font-size:20px;">
                                Quiz Details:
                                <asp:Label
                                    ID="lblDetailContext"
                                    runat="server">
                                </asp:Label>

                            </span>

                            <h2>

                                Student:
                                <asp:Label
                                    ID="lblStudentName"
                                    runat="server">
                                </asp:Label>

                            </h2>

                        </div>

                    </div>

                    <div class="attempt-badge">

                        <asp:Label
                            ID="lblAttemptBadge"
                            runat="server">
                        </asp:Label>

                    </div>

                </section>


                <%-- =================================================
                     DETAIL MESSAGE
                     ================================================= --%>

                <asp:Panel
                    ID="pnlDetailMessage"
                    runat="server"
                    Visible="false"
                    CssClass="grading-detail-message">

                    <asp:Label
                        ID="lblDetailMessage"
                        runat="server">
                    </asp:Label>

                </asp:Panel>


                <%-- =================================================
                     OBJECTIVE SUMMARY
                     ================================================= --%>

                <section class="objective-summary-card">

                    <div>

                        <h2>
                            1. Objective Questions (Auto-Marked)
                        </h2>

                        <p>
                            Single-choice questions are checked
                            automatically according to the answer key.
                        </p>

                    </div>

                    <div class="objective-score">

                        <asp:Label
                            ID="lblObjectiveScore"
                            runat="server"
                            Text="0 / 0 Marks">
                        </asp:Label>

                    </div>

                </section>


                <%-- =================================================
                     NO SUBJECTIVE QUESTIONS
                     ================================================= --%>

                <asp:Panel
                    ID="pnlNoSubjective"
                    runat="server"
                    Visible="false"
                    CssClass="grading-empty">

                    <div class="grading-empty-icon">

                        <i class="bi bi-journal-check"></i>

                    </div>

                    <h2>
                        No subjective questions
                    </h2>

                    <p>
                        This submission does not contain subjective
                        questions requiring manual grading.
                    </p>

                </asp:Panel>


                <%-- =================================================
                     SUBJECTIVE QUESTIONS
                     ================================================= --%>

                <asp:Repeater
                    ID="rptSubjectiveQuestions"
                    runat="server">

                    <ItemTemplate>

                        <article class="subjective-grading-card">

                            <asp:HiddenField
                                ID="hfAnswerId"
                                runat="server"
                                Value='<%# Eval("AnswerId") %>' />

                            <asp:HiddenField
                                ID="hfQuizQuestionId"
                                runat="server"
                                Value='<%# Eval("QuizQuestionId") %>' />

                            <asp:HiddenField
                                ID="hfMaxMarks"
                                runat="server"
                                Value='<%# Eval("MaxMarks") %>' />


                            <div class="subjective-card-header">

                                <h2>
                                    <%# Eval("QuestionHeading") %>
                                </h2>

                                <span class="max-mark-badge">

                                    Max:
                                    <%# Eval("MaxMarksText") %>

                                </span>

                            </div>


                            <%-- =====================================
                                 QUESTION
                                 ===================================== --%>

                            <div class="grading-answer-box question-box">

                                <span class="grading-box-label">
                                    Question Prompt:
                                </span>

                                <p>
                                    <%# Eval("QuestionText") %>
                                </p>


                                <asp:PlaceHolder
                                    ID="phQuestionImage"
                                    runat="server"
                                    Visible='<%# Convert.ToBoolean(Eval("HasQuestionImage")) %>'>

                                    <asp:Image
                                        ID="imgQuestion"
                                        runat="server"
                                        ImageUrl='<%# Eval("QuestionImageUrl") %>'
                                        AlternateText="Question image"
                                        style="display:block;max-width:100%;height:auto;margin-top:12px;border-radius:12px;" />

                                </asp:PlaceHolder>

                            </div>


                            <%-- =====================================
                                 STUDENT ANSWER
                                 ===================================== --%>

                            <div class="grading-answer-box student-answer-box">

                                <span class="grading-box-label">
                                    Student's Submitted Response:
                                </span>

                                <p>
                                    <%# Eval("StudentAnswerText") %>
                                </p>

                            </div>


                            <%-- =====================================
                                 MODEL ANSWER
                                 ===================================== --%>

                            <div class="grading-answer-box model-answer-box">

                                <span class="grading-box-label">

                                    <i class="bi bi-caret-down-fill"></i>

                                    Lecturer Reference &amp; Model Answer

                                </span>

                                <p>
                                    <%# Eval("ModelAnswerText") %>
                                </p>

                            </div>


                            <%-- =====================================
                                 GRADING INPUT
                                 ===================================== --%>

                            <div class="grading-input-grid">

                                <div class="grading-field grading-mark-field">

                                    <label>

                                        Awarded Marks

                                        <span class="required-mark">
                                            *
                                        </span>

                                    </label>

                                    <div class="grading-mark-input-wrapper">

                                        <asp:TextBox
                                            ID="txtAwardedMarks"
                                            runat="server"
                                            CssClass="grading-mark-input"
                                            Text='<%# Eval("MarksText") %>'
                                            MaxLength="8">
                                        </asp:TextBox>

                                        <span>

                                            /
                                            <%# Eval("MaxMarksText") %>

                                        </span>

                                    </div>

                                    <asp:Label
                                        ID="lblMarkError"
                                        runat="server"
                                        Visible="false"
                                        CssClass="grading-field-error">
                                    </asp:Label>

                                </div>


                                <div class="grading-field">

                                    <label>
                                        Lecturer Feedback Note
                                    </label>

                                    <asp:TextBox
                                        ID="txtLecturerFeedback"
                                        runat="server"
                                        CssClass="grading-feedback-input"
                                        Text='<%# Eval("LecturerFeedback") %>'
                                        TextMode="MultiLine"
                                        Rows="3"
                                        MaxLength="1000"
                                        placeholder="Enter feedback for the student...">
                                    </asp:TextBox>

                                </div>

                            </div>

                        </article>

                    </ItemTemplate>

                </asp:Repeater>


                <%-- =================================================
                     BOTTOM SUMMARY
                     ================================================= --%>

                <asp:Panel
                    ID="pnlGradingFooter"
                    runat="server"
                    Visible="false"
                    CssClass="grading-footer-card">

                    <div class="provisional-score">

                        <div class="provisional-icon">

                            <i class="bi bi-check-circle"></i>

                        </div>

                        <div>

                            <span>
                                Total Score
                            </span>

                            <strong>

                                <asp:Label
                                    ID="lblProvisionalScore"
                                    runat="server"
                                    Text="0 / 0 Total">
                                </asp:Label>

                            </strong>

                        </div>

                    </div>


                    <div class="grading-footer-actions">

                        <asp:Button
                            ID="btnFinalize"
                            runat="server"
                            Text="Finalise & Publish Result"
                            CssClass="grading-publish-button"
                            CausesValidation="false"
                            OnClick="btnFinalize_Click" />

                    </div>

                </asp:Panel>

            </div>

        </main>

    </form>

</body>

</html>