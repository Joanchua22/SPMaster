<%@ Page Title="Feedback"
    Language="C#"
    MasterPageFile="~/Master/Portal.Master"
    AutoEventWireup="true"
    CodeBehind="Feedback.aspx.cs"
    Inherits="SPMaster.Feedback" %>


<asp:Content
    ID="FeedbackHead"
    ContentPlaceHolderID="PortalHead"
    runat="server">

    <link href="<%= ResolveUrl("~/Content/Feedback.css") %>?v=1"
          rel="stylesheet"
          type="text/css" />

</asp:Content>


<asp:Content
    ID="FeedbackContent"
    ContentPlaceHolderID="PortalContent"
    runat="server">

    <main class="feedback-user-page">


        <%-- =====================================================
             PAGE HEADER
             ===================================================== --%>

        <section class="feedback-user-header">

            <div>

                <h1>
                    Feedback &amp; Support
                </h1>

                <p>
                    Submit questions regarding SPM quizzes,
                    system issues or suggestions.
                </p>

            </div>


            <button type="button"
                    id="btnOpenFeedbackModal"
                    class="feedback-submit-open">

                <i class="bi bi-plus-circle"></i>

                Submit Feedback

            </button>

        </section>


        <%-- =====================================================
             PAGE MESSAGE
             ===================================================== --%>

        <asp:Panel
            ID="pnlFeedbackMessage"
            runat="server"
            Visible="false"
            CssClass="feedback-page-message">

            <asp:Label
                ID="lblFeedbackMessage"
                runat="server">
            </asp:Label>

        </asp:Panel>


        <%-- =====================================================
             NO FEEDBACK
             ===================================================== --%>

        <asp:Panel
            ID="pnlNoFeedback"
            runat="server"
            Visible="false"
            CssClass="feedback-empty">

            <i class="bi bi-chat-left-text"></i>

            <h2>
                No feedback yet
            </h2>

            <p>
                Submit feedback if you need help
                or notice an issue.
            </p>

        </asp:Panel>


        <%-- =====================================================
             FEEDBACK LIST
             ===================================================== --%>

        <section class="feedback-user-list">

            <asp:Repeater
                ID="rptFeedback"
                runat="server">

                <ItemTemplate>

                    <article class="feedback-user-card">


                        <%-- TOP ROW --%>

                        <div class="feedback-card-top">

                            <div class="feedback-card-meta">

                                <span class="feedback-type-badge">

                                    <%#
                                        Eval("QuizId") == DBNull.Value
                                        ? "General Feedback"
                                        : "Quiz Feedback"
                                    %>

                                </span>


                                <span class="feedback-ticket">

                                    Ticket #FB-<%# Eval("FeedbackId") %>

                                    &bull;

                                    <%#
                                        Convert.ToDateTime(
                                            Eval("CreatedAt")
                                        ).ToString(
                                            "dd MMM yyyy"
                                        )
                                    %>

                                </span>

                            </div>


                            <span class='<%#
                                Eval("Status")
                                    .ToString()
                                    .Equals(
                                        "Resolved",
                                        StringComparison.OrdinalIgnoreCase
                                    )
                                ? "feedback-status feedback-status-resolved"
                                : "feedback-status feedback-status-open"
                            %>'>

                                <%# Eval("Status") %>

                            </span>

                        </div>


                        <%-- QUIZ TITLE --%>

                        <h2 class="feedback-card-title">

                            <%#
                                Eval("QuizTitle") == DBNull.Value
                                ? "General Feedback"
                                : Eval("QuizTitle")
                            %>

                        </h2>


                        <%-- USER MESSAGE --%>

                        <p class="feedback-card-message">

                            <%# Eval("Message") %>

                        </p>


                        <%-- =====================================
                             NO ADMIN REPLY YET
                             ===================================== --%>

                        <asp:Panel
                            runat="server"
                            Visible='<%#
                                string.IsNullOrWhiteSpace(
                                    Convert.ToString(
                                        Eval("AdminReply")
                                    )
                                )
                            %>'
                            CssClass="feedback-no-reply">

                            <i class="bi bi-clock"></i>

                            No admin response yet.
                            Your feedback is under review.

                        </asp:Panel>


                        <%-- =====================================
                             ADMIN REPLY
                             ===================================== --%>

                        <asp:Panel
                            runat="server"
                            Visible='<%#
                                !string.IsNullOrWhiteSpace(
                                    Convert.ToString(
                                        Eval("AdminReply")
                                    )
                                )
                            %>'
                            CssClass="feedback-admin-reply">

                            <div class="feedback-admin-reply-header">

                                <div class="feedback-admin-name">

                                    <span class="feedback-admin-icon">

                                        <i class="bi bi-headset"></i>

                                    </span>


                                    <strong>

                                        <%#
                                            string.IsNullOrWhiteSpace(
                                                Convert.ToString(
                                                    Eval("RepliedByName")
                                                )
                                            )
                                            ? "SPMaster Admin Team"
                                            : Eval("RepliedByName")
                                        %>

                                    </strong>

                                </div>


                                <span class="feedback-reply-date">

                                    <%#
                                        Eval("RepliedAt") == DBNull.Value
                                        ? ""
                                        : Convert.ToDateTime(
                                            Eval("RepliedAt")
                                        ).ToString(
                                            "dd MMM yyyy, hh:mm tt"
                                        )
                                    %>

                                </span>

                            </div>


                            <p>

                                <%# Eval("AdminReply") %>

                            </p>

                        </asp:Panel>


                    </article>

                </ItemTemplate>

            </asp:Repeater>

        </section>


        <%-- =====================================================
             SUBMIT FEEDBACK MODAL
             ===================================================== --%>

        <div id="feedbackSubmitModal"
             class="feedback-modal"
             hidden>


            <div id="feedbackModalOverlay"
                 class="feedback-modal-overlay">
            </div>


            <div class="feedback-modal-dialog">


                <%-- =============================================
                     HEADER
                     ============================================= --%>

                <div class="feedback-modal-header">

                    <div>

                        <h2>
                            Submit Inquiry / Feedback
                        </h2>

                        <p>
                            Tell us about the issue you encountered.
                        </p>

                    </div>


                    <button type="button"
                            id="btnCloseFeedbackModal"
                            class="feedback-modal-close">

                        <i class="bi bi-x-lg"></i>

                    </button>

                </div>


                <%-- =============================================
                     SERVER ERROR
                     ============================================= --%>

                <asp:Panel
                    ID="pnlSubmitError"
                    runat="server"
                    Visible="false"
                    CssClass="feedback-submit-error">

                    <i class="bi bi-exclamation-circle"></i>

                    <asp:Label
                        ID="lblSubmitError"
                        runat="server">
                    </asp:Label>

                </asp:Panel>
                <%-- =============================================
     SEARCH QUIZ
     ============================================= --%>

<div class="feedback-form-group">

    <label for="<%= txtQuizSearch.ClientID %>">
        Search Quiz
    </label>


    <div class="feedback-quiz-search">

        <asp:TextBox
            ID="txtQuizSearch"
            runat="server"
            CssClass="feedback-input"
            placeholder="Search quiz title...">
        </asp:TextBox>


        <asp:Button
            ID="btnSearchQuiz"
            runat="server"
            Text="Search"
            CssClass="feedback-search-button"
            CausesValidation="false"
            OnClick="btnSearchQuiz_Click" />


        <asp:Button
            ID="btnClearQuizSearch"
            runat="server"
            Text="Clear"
            CssClass="feedback-clear-button"
            CausesValidation="false"
            OnClick="btnClearQuizSearch_Click" />

    </div>

</div>


<%-- =============================================
     RELATED QUIZ
     ============================================= --%>

<div class="feedback-form-group">

    <label for="<%= ddlRelatedQuiz.ClientID %>">

        Related Quiz

        <span class="feedback-required">
            *
        </span>

    </label>


    <asp:DropDownList
        ID="ddlRelatedQuiz"
        runat="server"
        CssClass="feedback-dropdown">
    </asp:DropDownList>


    <asp:RequiredFieldValidator
        ID="rfvRelatedQuiz"
        runat="server"
        ControlToValidate="ddlRelatedQuiz"
        InitialValue=""
        ErrorMessage="Please select a related quiz."
        ValidationGroup="SubmitFeedback"
        Display="Dynamic"
        CssClass="feedback-field-error">
    </asp:RequiredFieldValidator>

</div>
                <%-- =============================================
                     MESSAGE
                     ============================================= --%>

                <div class="feedback-form-group">

                    <label for="<%= txtFeedbackMessage.ClientID %>">

                        Message / Description

                        <span class="feedback-required">
                            *
                        </span>

                    </label>


                    <asp:TextBox
                        ID="txtFeedbackMessage"
                        runat="server"
                        TextMode="MultiLine"
                        Rows="6"
                        MaxLength="2000"
                        CssClass="feedback-textarea"
                        placeholder="Describe the issue or feedback clearly...">
                    </asp:TextBox>


                    <asp:RequiredFieldValidator
                        ID="rfvFeedbackMessage"
                        runat="server"
                        ControlToValidate="txtFeedbackMessage"
                        ErrorMessage="Please enter your feedback."
                        ValidationGroup="SubmitFeedback"
                        Display="Dynamic"
                        CssClass="feedback-field-error">
                    </asp:RequiredFieldValidator>

                </div>


                <%-- =============================================
                     ACTION BUTTONS
                     ============================================= --%>

                <div class="feedback-modal-actions">

                    <button type="button"
                            id="btnCancelFeedback"
                            class="feedback-cancel-button">

                        Cancel

                    </button>


                    <asp:Button
                        ID="btnSubmitFeedback"
                        runat="server"
                        Text="Submit Ticket"
                        CssClass="feedback-submit-button"
                        ValidationGroup="SubmitFeedback"
                        CausesValidation="true"
                        OnClick="btnSubmitFeedback_Click" />

                </div>


                <%-- Used to reopen popup after server validation --%>

                <asp:HiddenField
                    ID="hfOpenFeedbackModal"
                    runat="server"
                    ClientIDMode="Static"
                    Value="0" />


            </div>

        </div>


    </main>

</asp:Content>


<asp:Content
    ID="FeedbackScripts"
    ContentPlaceHolderID="PortalScripts"
    runat="server">

    <script src="<%= ResolveUrl("~/Scripts/Feedback.js") %>?v=1">
    </script>

</asp:Content>