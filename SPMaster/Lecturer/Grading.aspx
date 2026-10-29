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
             PAGE HEADER
             ===================================================== --%>

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


        <%-- =====================================================
             SEARCH
             ===================================================== --%>

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


        <%-- =====================================================
             PENDING GRADING HEADER
             ===================================================== --%>

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


        <%-- =====================================================
             NO PENDING
             ===================================================== --%>

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


        <%-- =====================================================
             PENDING QUEUE
             ===================================================== --%>

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


        <%-- =====================================================
             REVIEW HISTORY HEADER
             ===================================================== --%>

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


        <%-- =====================================================
             NO REVIEWS
             ===================================================== --%>

        <asp:Panel
            ID="pnlNoReviews"
            runat="server"
            Visible="false"
            CssClass="grading-empty">

            <div class="grading-empty-icon">

                <i class="bi bi-journal-check"></i>

            </div>

            <h2>
                No grading history found
            </h2>

            <p>
                Finalised subjective grading will appear here
                for future review.
            </p>

        </asp:Panel>


        <%-- =====================================================
             REVIEW HISTORY
             ===================================================== --%>

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

    </div>

</asp:Content>