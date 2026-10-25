<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/Master/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="ViewContent.aspx.cs"
    Inherits="SPMaster.Admin.ViewContent" %>


<asp:Content
    ID="ViewContentHead"
    ContentPlaceHolderID="DashboardHead"
    runat="server">

    <link
        href="<%= ResolveUrl("~/Content/ViewContent.css") %>"
        rel="stylesheet" />

</asp:Content>


<asp:Content
    ID="AdminTitle"
    ContentPlaceHolderID="DashboardTitle"
    runat="server">

    View Content

</asp:Content>


<asp:Content
    ID="AdminNavigation"
    ContentPlaceHolderID="DashboardNavigation"
    runat="server">

    <nav class="sidebar-nav"
         aria-label="Admin navigation">

        <a href="<%= ResolveUrl("~/Admin/Dashboard.aspx") %>"
           class="nav-item">

            <span class="nav-icon">
                <i class="bi bi-grid"></i>
            </span>

            <span>Dashboard</span>

        </a>


        <a href="<%= ResolveUrl("~/Admin/UserManagement.aspx") %>"
           class="nav-item">

            <span class="nav-icon">
                <i class="bi bi-people"></i>
            </span>

            <span>User Management</span>

        </a>


        <a href="<%= ResolveUrl("~/Admin/ContentAudit.aspx") %>"
           class="nav-item active"
           aria-current="page">

            <span class="nav-icon">
                <i class="bi bi-file-earmark-text"></i>
            </span>

            <span>Content Audit</span>

        </a>


        <a href="<%= ResolveUrl("~/Admin/FeedbackReports.aspx") %>"
           class="nav-item">

            <span class="nav-icon">
                <i class="bi bi-chat-left-text"></i>
            </span>

            <span>Feedback &amp; Reports</span>

        </a>

    </nav>

</asp:Content>


<asp:Content
    ID="AdminAccountNavigation"
    ContentPlaceHolderID="DashboardAccountNavigation"
    runat="server">

    <nav aria-label="Admin account navigation">

        <a href="<%= ResolveUrl("~/Account/Profile.aspx") %>"
           class="nav-item">

            <span class="nav-icon">
                <i class="bi bi-person-circle"></i>
            </span>

            <span>Profile</span>

        </a>


        <a href="<%= ResolveUrl("~/Account/Settings.aspx") %>"
           class="nav-item">

            <span class="nav-icon">
                <i class="bi bi-gear"></i>
            </span>

            <span>Settings</span>

        </a>

    </nav>

</asp:Content>


<asp:Content
    ID="AdminUser"
    ContentPlaceHolderID="DashboardUser"
    runat="server">
</asp:Content>


<asp:Content
    ID="AdminContent"
    ContentPlaceHolderID="DashboardContent"
    runat="server">

    <section class="view-content-page">


        <%-- =====================================================
             BACK + HEADER
             ===================================================== --%>

        <div class="view-content-header">

            <a
                href="<%= ResolveUrl("~/Admin/ContentAudit.aspx") %>"
                class="view-content-back">

                <i class="bi bi-arrow-left"></i>

                Back to Content Audit

            </a>


            <div>

                <h2>
                    Quiz Details
                </h2>

                <p>
                    Review the published quiz and all questions included in it
                </p>

            </div>

        </div>



        <%-- =====================================================
             ERROR
             ===================================================== --%>

        <asp:Panel
            ID="pnlError"
            runat="server"
            CssClass="view-content-error"
            Visible="false">

            <asp:Label
                ID="lblError"
                runat="server">
            </asp:Label>

        </asp:Panel>



        <%-- =====================================================
             QUIZ CONTENT
             ===================================================== --%>

        <asp:Panel
            ID="pnlQuiz"
            runat="server"
            Visible="false">


            <%-- QUIZ INFORMATION --%>

            <div class="view-content-card">


                <div class="view-content-section">

                    <span class="view-content-label">
                        QUIZ TITLE
                    </span>

                    <div class="view-question-text">

                        <asp:Label
                            ID="lblQuizTitle"
                            runat="server">
                        </asp:Label>

                    </div>

                </div>



                <%-- QUIZ META --%>

                <div class="view-question-meta">

                    <span class="view-meta-item">

                        <i class="bi bi-book"></i>

                        <asp:Label
                            ID="lblSubject"
                            runat="server">
                        </asp:Label>

                    </span>


                    <span class="view-meta-item">

                        <i class="bi bi-person"></i>

                        <asp:Label
                            ID="lblLecturer"
                            runat="server">
                        </asp:Label>

                    </span>


                    <span class="view-meta-item">

                        <i class="bi bi-calendar3"></i>

                        <asp:Label
                            ID="lblCreatedDate"
                            runat="server">
                        </asp:Label>

                    </span>


                    <span class="view-meta-item">

                        <i class="bi bi-list-check"></i>

                        <asp:Label
                            ID="lblQuestionCount"
                            runat="server">
                        </asp:Label>

                    </span>

                </div>



                <%-- QUIZ INFORMATION --%>

                <div class="view-info-grid">


                    <div class="view-info-item">

                        <span class="view-content-label">
                            FORM
                        </span>

                        <div class="view-readonly-field">

                            <asp:Label
                                ID="lblForm"
                                runat="server">
                            </asp:Label>

                        </div>

                    </div>


                    <div class="view-info-item">

                        <span class="view-content-label">
                            TIME LIMIT
                        </span>

                        <div class="view-readonly-field">

                            <asp:Label
                                ID="lblTimeLimit"
                                runat="server">
                            </asp:Label>

                        </div>

                    </div>


                    <div class="view-info-item">

                        <span class="view-content-label">
                            PASS PERCENTAGE
                        </span>

                        <div class="view-readonly-field">

                            <asp:Label
                                ID="lblPassPercentage"
                                runat="server">
                            </asp:Label>

                        </div>

                    </div>


                    <div class="view-info-item">

                        <span class="view-content-label">
                            STATUS
                        </span>

                        <div class="view-readonly-field">

                            <div class="view-readonly-field">

                                <asp:Label
                                    ID="lblQuizStatus"
                                    runat="server">
                                </asp:Label>

                            </div>

                        </div>

                    </div>

                </div>



                <%-- DESCRIPTION --%>

                <asp:Panel
                    ID="pnlDescription"
                    runat="server"
                    CssClass="view-content-section"
                    Visible="false">

                    <span class="view-content-label">
                        DESCRIPTION
                    </span>

                    <div class="view-answer-box">

                        <asp:Label
                            ID="lblDescription"
                            runat="server">
                        </asp:Label>

                    </div>

                </asp:Panel>

            </div>



            <%-- =================================================
                 QUESTIONS
                 ================================================= --%>

            <div class="view-content-card">

                <div class="view-content-section">

                    <span class="view-content-label">
                        QUESTIONS IN THIS QUIZ
                    </span>

                </div>


                <asp:Repeater
                    ID="rptQuestions"
                    runat="server"
                    OnItemDataBound="rptQuestions_ItemDataBound">

                    <ItemTemplate>


                        <div class="view-quiz-question">


                            <%-- QUESTION NUMBER --%>

                            <div class="view-question-heading">

                                <span class="view-question-number">

                                    Question
                                    <%# Eval("QuestionOrder") %>

                                </span>


                                <span class="view-question-type-badge">

                                    <%# GetQuestionTypeDisplay(
                                            Eval("QuestionType").ToString()
                                        ) %>

                                </span>

                            </div>



                            <%-- QUESTION TEXT --%>

                            <div class="view-content-section">

                                <span class="view-content-label">
                                    QUESTION
                                </span>

                                <div class="view-question-text">

                                    <%# Eval("QuestionText") %>

                                </div>

                            </div>



                            <%-- TYPE + DIFFICULTY --%>

                            <div class="view-info-grid">


                                <div class="view-info-item">

                                    <span class="view-content-label">
                                        QUESTION TYPE
                                    </span>

                                    <div class="view-readonly-field">

                                        <%# GetQuestionTypeDisplay(
                                                Eval("QuestionType")
                                                    .ToString()
                                            ) %>

                                    </div>

                                </div>


                                <div class="view-info-item">

                                    <span class="view-content-label">
                                        DIFFICULTY
                                    </span>

                                    <div class="view-readonly-field">

                                        <%# Eval("Difficulty") %>

                                    </div>

                                </div>

                            </div>



                            <%-- QUESTION IMAGE --%>

                            <asp:Panel
                                ID="pnlQuestionMedia"
                                runat="server"
                                CssClass="view-content-section"
                                Visible="false">

                                <span class="view-content-label">
                                    QUESTION IMAGE
                                </span>

                                <div class="view-question-image-wrapper">

                                    <asp:Image
                                        ID="imgQuestion"
                                        runat="server"
                                        CssClass="view-question-image"
                                        AlternateText="Question image" />

                                </div>

                            </asp:Panel>



                            <%-- MCQ OPTIONS --%>

                            <asp:Panel
                                ID="pnlQuestionMcq"
                                runat="server"
                                CssClass="view-content-section"
                                Visible="false">

                                <span class="view-content-label">
                                    ANSWER OPTIONS
                                </span>


                                <div class="view-options-list">

                                    <asp:Repeater
                                        ID="rptOptions"
                                        runat="server">

                                        <ItemTemplate>

                                            <div class='<%#
                                                Convert.ToBoolean(
                                                    Eval("IsCorrect")
                                                )
                                                ? "view-option correct"
                                                : "view-option"
                                            %>'>

                                                <span class="view-option-letter">

                                                    <%#
                                                        Convert.ToChar(
                                                            64 +
                                                            Convert.ToInt32(
                                                                Eval("DisplayOrder")
                                                            )
                                                        )
                                                    %>

                                                </span>


                                                <span class="view-option-text">

                                                    <%# Eval("OptionText") %>

                                                </span>


                                                <asp:PlaceHolder
                                                    ID="phCorrect"
                                                    runat="server"
                                                    Visible='<%#
                                                        Convert.ToBoolean(
                                                            Eval("IsCorrect")
                                                        )
                                                    %>'>

                                                    <span class="view-correct-badge">

                                                        <i class="bi bi-check-circle"></i>

                                                        Correct

                                                    </span>

                                                </asp:PlaceHolder>

                                            </div>

                                        </ItemTemplate>

                                    </asp:Repeater>

                                </div>

                            </asp:Panel>



                            <%-- SUBJECTIVE ANSWER --%>

                            <asp:Panel
                                ID="pnlQuestionSubjective"
                                runat="server"
                                CssClass="view-content-section"
                                Visible="false">

                                <span class="view-content-label">
                                    MODEL ANSWER
                                </span>

                                <div class="view-answer-box">

                                    <asp:Label
                                        ID="lblModelAnswer"
                                        runat="server">
                                    </asp:Label>

                                </div>

                            </asp:Panel>



                            <%-- EXPLANATION --%>

                            <asp:Panel
                                ID="pnlQuestionExplanation"
                                runat="server"
                                CssClass="view-content-section"
                                Visible="false">

                                <span class="view-content-label">
                                    EXPLANATION
                                </span>

                                <div class="view-answer-box">

                                    <asp:Label
                                        ID="lblExplanation"
                                        runat="server">
                                    </asp:Label>

                                </div>

                            </asp:Panel>


                        </div>

                    </ItemTemplate>

                </asp:Repeater>



                <%-- NO QUESTIONS --%>

                <asp:Panel
                    ID="pnlNoQuestions"
                    runat="server"
                    CssClass="view-no-published"
                    Visible="false">

                    No questions are currently linked to this quiz.

                </asp:Panel>

            </div>


        </asp:Panel>

    </section>

</asp:Content>