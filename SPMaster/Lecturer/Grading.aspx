<%@ Page Title="Grading"
    Language="C#"
    MasterPageFile="~/Master/Portal.Master"
    AutoEventWireup="true"
    CodeBehind="Grading.aspx.cs"
    Inherits="SPMaster.Lecturer.Grading" %>

<asp:Content
    ID="GradingHead"
    ContentPlaceHolderID="PortalHead"
    runat="server">

    <link href="<%= ResolveUrl("~/Content/Grading.css") %>?v=1"
          rel="stylesheet"
          type="text/css" />

</asp:Content>

<asp:Content
    ID="GradingContent"
    ContentPlaceHolderID="PortalContent"
    runat="server">

    <div class="grading-page">

        <%-- =====================================================
             GENERAL MESSAGE
             ===================================================== --%>

        <asp:Panel
            ID="pnlMessage"
            runat="server"
            Visible="false"
            CssClass="grading-message">

            <asp:Label
                ID="lblMessage"
                runat="server">
            </asp:Label>

        </asp:Panel>

        <%-- =====================================================
             QUEUE + REVIEW VIEW
             ===================================================== --%>

        <asp:Panel
            ID="pnlQueueView"
            runat="server"
            Visible="true">

            <%-- =================================================
                 PAGE HEADER
                 ================================================= --%>

            <section class="grading-header">

                <div>

                    <h1>
                        Grading
                    </h1>

                    <p>
                        Grade pending submissions and review
                        previously published results.
                    </p>

                </div>

                <div class="grading-filter">

                    <asp:DropDownList
                        ID="ddlQuizFilter"
                        runat="server"
                        CssClass="grading-filter-select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlQuizFilter_SelectedIndexChanged">
                    </asp:DropDownList>

                </div>

            </section>

            <%-- =================================================
                 SEARCH
                 ================================================= --%>

            <div class="grading-filter"
                 style="display:flex;gap:10px;margin-bottom:24px;align-items:center;">

                <asp:TextBox
                    ID="txtSearch"
                    runat="server"
                    CssClass="grading-filter-select"
                    placeholder="Search student, quiz or session..."
                    style="min-width:300px;">
                </asp:TextBox>

                <asp:Button
                    ID="btnSearch"
                    runat="server"
                    Text="Search"
                    CssClass="grade-submission-button"
                    CausesValidation="false"
                    OnClick="btnSearch_Click" />

                <asp:Button
                    ID="btnClearSearch"
                    runat="server"
                    Text="Clear"
                    CssClass="grading-save-button"
                    CausesValidation="false"
                    OnClick="btnClearSearch_Click" />

            </div>

            <%-- =================================================
                 PENDING GRADING HEADER
                 ================================================= --%>

            <section class="grading-detail-header"
                     style="margin-bottom:18px;">

                <div class="grading-detail-heading">

                    <div>

                        <h1 style="font-size:22px;">
                            Pending Grading
                        </h1>

                        <span class="grading-context">
                            Subjective submissions waiting for review.
                        </span>

                    </div>

                </div>

            </section>

            <%-- =================================================
                 NO PENDING / NO SEARCH RESULT
                 ================================================= --%>

            <asp:Panel
                ID="pnlNoPending"
                runat="server"
                Visible="false"
                CssClass="grading-empty">

                <div class="grading-empty-icon">

                    <i class="bi bi-check-circle"></i>

                </div>

                <h2>

                    <asp:Label
                        ID="lblNoPendingTitle"
                        runat="server">
                    </asp:Label>

                </h2>

                <p>

                    <asp:Label
                        ID="lblNoPendingDescription"
                        runat="server">
                    </asp:Label>

                </p>

            </asp:Panel>

            <%-- =================================================
                 PENDING GRADING LIST
                 ================================================= --%>

            <asp:Panel
                ID="pnlQueueList"
                runat="server"
                Visible="false"
                CssClass="grading-queue">

                <asp:Repeater
                    ID="rptGradingQueue"
                    runat="server">

                    <ItemTemplate>

                        <article class="grading-queue-card">

                            <%-- STUDENT INFORMATION --%>

                            <div class="grading-student">

                                <div class="grading-avatar">

                                    <%# Eval("StudentInitials") %>

                                </div>

                                <div class="grading-student-info">

                                    <h2>

                                        <%# Eval("StudentName") %>

                                    </h2>

                                    <p>

                                        Quiz:
                                        <strong>

                                            <%# Eval("QuizTitle") %>

                                        </strong>

                                        <asp:PlaceHolder
                                            ID="phSession"
                                            runat="server"
                                            Visible='<%# Convert.ToBoolean(Eval("HasSession")) %>'>

                                            <span class="grading-divider">
                                                •
                                            </span>

                                            Session:
                                            <strong>

                                                <%# Eval("SessionTitle") %>

                                            </strong>

                                        </asp:PlaceHolder>

                                    </p>

                                    <div class="grading-queue-meta">

                                        <span class="grading-subjective-count">

                                            <%# Eval("SubjectiveCountText") %>

                                        </span>

                                        <span>

                                            Submitted
                                            <%# Eval("SubmittedAtText") %>

                                        </span>

                                    </div>

                                </div>

                            </div>

                            <%-- ACTION --%>

                            <div class="grading-queue-action">

                                <asp:HyperLink
                                    ID="lnkGradeSubmission"
                                    runat="server"
                                    CssClass="grade-submission-button"
                                    NavigateUrl='<%# Eval("GradeUrl") %>'>

                                    Grade Submission

                                </asp:HyperLink>

                            </div>

                        </article>

                    </ItemTemplate>

                </asp:Repeater>

            </asp:Panel>

            <%-- =================================================
                 PAST GRADING REVIEWS HEADER
                 ================================================= --%>

            <section class="grading-detail-header"
                     style="margin-top:38px;margin-bottom:18px;">

                <div class="grading-detail-heading">

                    <div>

                        <h1 style="font-size:22px;">
                            Past Grading Reviews
                        </h1>

                        <span class="grading-context">
                            Review results that you have already finalised.
                        </span>

                    </div>

                </div>

            </section>

            <%-- =================================================
                 NO REVIEW HISTORY / NO SEARCH RESULT
                 ================================================= --%>

            <asp:Panel
                ID="pnlNoReviews"
                runat="server"
                Visible="false"
                CssClass="grading-empty">

                <div class="grading-empty-icon">

                    <i class="bi bi-journal-check"></i>

                </div>

                <h2>

                    <asp:Label
                        ID="lblNoReviewsTitle"
                        runat="server">
                    </asp:Label>

                </h2>

                <p>

                    <asp:Label
                        ID="lblNoReviewsDescription"
                        runat="server">
                    </asp:Label>

                </p>

            </asp:Panel>

            <%-- =================================================
                 REVIEW HISTORY LIST
                 ================================================= --%>

            <asp:Panel
                ID="pnlReviewList"
                runat="server"
                Visible="false"
                CssClass="grading-queue">

                <asp:Repeater
                    ID="rptReviewHistory"
                    runat="server">

                    <ItemTemplate>

                        <article class="grading-queue-card">

                            <%-- STUDENT INFORMATION --%>

                            <div class="grading-student">

                                <div class="grading-avatar">

                                    <%# Eval("StudentInitials") %>

                                </div>

                                <div class="grading-student-info">

                                    <h2>

                                        <%# Eval("StudentName") %>

                                    </h2>

                                    <p>

                                        Quiz:
                                        <strong>

                                            <%# Eval("QuizTitle") %>

                                        </strong>

                                        <asp:PlaceHolder
                                            ID="phReviewSession"
                                            runat="server"
                                            Visible='<%# Convert.ToBoolean(Eval("HasSession")) %>'>

                                            <span class="grading-divider">
                                                •
                                            </span>

                                            Session:
                                            <strong>

                                                <%# Eval("SessionTitle") %>

                                            </strong>

                                        </asp:PlaceHolder>

                                    </p>

                                    <div class="grading-queue-meta">

                                        <span class="grading-subjective-count">

                                            <%# Eval("FinalScoreText") %>

                                        </span>

                                        <span>

                                            Graded
                                            <%# Eval("GradedAtText") %>

                                        </span>

                                    </div>

                                </div>

                            </div>

                            <%-- ACTION --%>

                            <div class="grading-queue-action">

                                <asp:HyperLink
                                    ID="lnkReviewGrade"
                                    runat="server"
                                    CssClass="grade-submission-button"
                                    NavigateUrl='<%# Eval("ReviewUrl") %>'>

                                    Review Grade

                                </asp:HyperLink>

                            </div>

                        </article>

                    </ItemTemplate>

                </asp:Repeater>

            </asp:Panel>

        </asp:Panel>

        <%-- =====================================================
             INDIVIDUAL GRADING / REVIEW VIEW
             ===================================================== --%>

        <asp:Panel
            ID="pnlGradingView"
            runat="server"
            Visible="false">

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

                        <span class="grading-context">

                            <asp:Label
                                ID="lblDetailContext"
                                runat="server">
                            </asp:Label>

                        </span>

                        <h1>

                            Grading:

                            <asp:Label
                                ID="lblStudentName"
                                runat="server">
                            </asp:Label>

                        </h1>

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
                CssClass="grading-empty grading-detail-empty">

                <div class="grading-empty-icon">

                    <i class="bi bi-check-circle"></i>

                </div>

                <h2>
                    No subjective grading required
                </h2>

                <p>
                    This attempt does not contain any subjective
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

                        <%-- HIDDEN VALUES --%>

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

                        <%-- QUESTION HEADER --%>

                        <div class="subjective-card-header">

                            <h2>

                                <%# Eval("QuestionHeading") %>

                            </h2>

                            <span class="max-mark-badge">

                                Max:
                                <%# Eval("MaxMarksText") %>

                            </span>

                        </div>

                        <%-- QUESTION PROMPT --%>

                        <div class="grading-answer-box question-box">

                            <span class="grading-box-label">

                                Question Prompt:

                            </span>

                            <p>

                                <%# Eval("QuestionText") %>

                            </p>

                        </div>

                        <%-- STUDENT ANSWER --%>

                        <div class="grading-answer-box student-answer-box">

                            <span class="grading-box-label">

                                Student's Submitted Response:

                            </span>

                            <p>

                                <%# Eval("StudentAnswerText") %>

                            </p>

                        </div>

                        <%-- MODEL ANSWER --%>

                        <div class="grading-answer-box model-answer-box">

                            <span class="grading-box-label">

                                <i class="bi bi-caret-down-fill"></i>

                                Lecturer Reference &amp; Model Answer

                            </span>

                            <p>

                                <%# Eval("ModelAnswerText") %>

                            </p>

                        </div>

                        <%-- MARKS + FEEDBACK --%>

                        <div class="grading-input-grid">

                            <%-- MARKS --%>

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

                            <%-- FEEDBACK --%>

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

        </asp:Panel>

    </div>

</asp:Content>