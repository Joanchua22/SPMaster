<%@ Page Title="Classroom Sessions"
    Language="C#"
    MasterPageFile="~/Master/Portal.Master"
    AutoEventWireup="true"
    CodeBehind="Sessions.aspx.cs"
    Inherits="SPMaster.Lecturer.Sessions" %>


<asp:Content
    ID="SessionsHead"
    ContentPlaceHolderID="PortalHead"
    runat="server">

    <link href="<%= ResolveUrl("~/Content/Sessions.css") %>?v=1"
          rel="stylesheet"
          type="text/css" />

</asp:Content>



<asp:Content
    ID="SessionsContent"
    ContentPlaceHolderID="PortalContent"
    runat="server">

    <div class="sessions-page">


        <%-- =====================================================
             HEADER
             ===================================================== --%>

        <section class="sessions-header">

            <div>
                <h1>Classroom Sessions</h1>

                <p>
                    Host published quizzes, monitor attempts,
                    and review previous sessions.
                </p>
            </div>


            <button type="button"
                    id="btnOpenSessionModal"
                    class="host-session-button">

                <i class="bi bi-plus-circle"></i>

                Host New Session

            </button>

        </section>



        <%-- =====================================================
             MESSAGE
             ===================================================== --%>

        <asp:Panel
            ID="pnlMessage"
            runat="server"
            Visible="false"
            CssClass="session-message">

            <asp:Label
                ID="lblMessage"
                runat="server">
            </asp:Label>

        </asp:Panel>



        <%-- =====================================================
             ACTIVE SESSIONS
             ===================================================== --%>

        <section class="session-group">

            <div class="session-group-title">

                <div>
                    <h2>Active Sessions</h2>

                    <p>
                        Students can currently join these sessions.
                    </p>
                </div>

                <asp:Label
                    ID="lblActiveCount"
                    runat="server"
                    CssClass="session-count">
                </asp:Label>

            </div>



            <asp:Panel
                ID="pnlNoActiveSessions"
                runat="server"
                Visible="false"
                CssClass="session-empty">

                <i class="bi bi-broadcast"></i>

                <strong>No active sessions</strong>

                <span>
                    Host a new session to get started.
                </span>

            </asp:Panel>



            <div class="session-grid">

                <asp:Repeater
                    ID="rptActiveSessions"
                    runat="server"
                    OnItemCommand="rptActiveSessions_ItemCommand">

                    <ItemTemplate>

                        <article class="session-card">

                            <div class="session-card-top">

                                <span class="session-status status-open">
                                    Open
                                </span>


                                <span class="session-date">
                                    <%# Eval(
                                        "CreatedAt",
                                        "{0:dd MMM yyyy}"
                                    ) %>
                                </span>

                            </div>



                            <h3>
                                <%# Eval("SessionTitle") %>
                            </h3>


                            <p class="session-quiz-title">
                                <%# Eval("QuizTitle") %>
                            </p>


                            <span class="session-subject">
                                <%# Eval("SubjectName") %>
                            </span>



                            <%-- JOIN CODE --%>

                            <div class="session-code-box">

                                <span class="code-label">
                                    Class Join Code
                                </span>


                                <div class="code-row">

                                    <strong>
                                        <%# Eval("JoinCode") %>
                                    </strong>


                                    <button type="button"
                                            class="copy-code-button"
                                            data-code='<%# Eval("JoinCode") %>'>

                                        <i class="bi bi-copy"></i>

                                        Copy

                                    </button>

                                </div>

                            </div>



                            <%-- STATISTICS --%>

                            <div class="session-stat-grid">

                                <div class="session-stat">

                                    <strong>
                                        <%# Eval("CompletedAttempts") %>
                                    </strong>

                                    <span>
                                        Completed
                                    </span>

                                </div>


                                <div class="session-stat">

                                    <strong>
                                        <%# Eval("FullyGraded") %>
                                    </strong>

                                    <span>
                                        Graded
                                    </span>

                                </div>


                                <div class="session-stat">

                                    <strong class="pending-number">
                                        <%# Eval("PendingReview") %>
                                    </strong>

                                    <span>
                                        Pending
                                    </span>

                                </div>

                            </div>



                            <div class="session-average">

                                <span>
                                    Average Score
                                </span>

                                <strong>
                                    <%# Eval("AverageScoreText") %>
                                </strong>

                            </div>



                            <%-- ACTIONS --%>

                            <div class="session-card-actions">

                                <a class="session-details-button"
                                   href='<%#
                                        ResolveUrl(
                                            "~/Lecturer/SessionDetails.aspx?id="
                                            + Eval("SessionId")
                                        )
                                   %>'>

                                    Session Details

                                </a>


                                <asp:LinkButton
                                    ID="btnCloseSession"
                                    runat="server"
                                    CssClass="close-session-button"
                                    CommandName="CloseSession"
                                    CommandArgument='<%# Eval("SessionId") %>'
                                    CausesValidation="false"
                                    OnClientClick="return confirm('Close this session? Students will no longer be able to join.');">

                                    Close

                                </asp:LinkButton>

                            </div>

                        </article>

                    </ItemTemplate>

                </asp:Repeater>

            </div>

        </section>



        <%-- =====================================================
             HISTORY
             ===================================================== --%>

        <section class="session-group history-group">

            <div class="session-group-title">

                <div>
                    <h2>Session History</h2>

                    <p>
                        Previous sessions and their results.
                    </p>
                </div>


                <asp:Label
                    ID="lblHistoryCount"
                    runat="server"
                    CssClass="session-count">
                </asp:Label>

            </div>



            <asp:Panel
                ID="pnlNoHistory"
                runat="server"
                Visible="false"
                CssClass="session-empty">

                <i class="bi bi-clock-history"></i>

                <strong>No session history</strong>

                <span>
                    Closed sessions will appear here.
                </span>

            </asp:Panel>



            <div class="session-grid">

                <asp:Repeater
                    ID="rptHistory"
                    runat="server">

                    <ItemTemplate>

                        <article class="session-card closed-card">

                            <div class="session-card-top">

                                <span class="session-status status-closed">
                                    Closed
                                </span>


                                <span class="session-date">
                                    <%# Eval(
                                        "CreatedAt",
                                        "{0:dd MMM yyyy}"
                                    ) %>
                                </span>

                            </div>



                            <h3>
                                <%# Eval("SessionTitle") %>
                            </h3>


                            <p class="session-quiz-title">
                                <%# Eval("QuizTitle") %>
                            </p>


                            <span class="session-subject">
                                <%# Eval("SubjectName") %>
                            </span>



                            <div class="session-code-box closed-code">

                                <span class="code-label">
                                    Join Code
                                </span>


                                <div class="code-row">

                                    <strong>
                                        <%# Eval("JoinCode") %>
                                    </strong>


                                    <span class="archived-label">
                                        Archived
                                    </span>

                                </div>

                            </div>



                            <div class="session-stat-grid">

                                <div class="session-stat">

                                    <strong>
                                        <%# Eval("CompletedAttempts") %>
                                    </strong>

                                    <span>
                                        Completed
                                    </span>

                                </div>


                                <div class="session-stat">

                                    <strong>
                                        <%# Eval("FullyGraded") %>
                                    </strong>

                                    <span>
                                        Graded
                                    </span>

                                </div>


                                <div class="session-stat">

                                    <strong class="pending-number">
                                        <%# Eval("PendingReview") %>
                                    </strong>

                                    <span>
                                        Pending
                                    </span>

                                </div>

                            </div>



                            <div class="session-average">

                                <span>
                                    Average Score
                                </span>

                                <strong>
                                    <%# Eval("AverageScoreText") %>
                                </strong>

                            </div>



                            <a class="final-analytics-button"
                               href='<%#
                                    ResolveUrl(
                                        "~/Lecturer/SessionDetails.aspx?id="
                                        + Eval("SessionId")
                                    )
                               %>'>

                                View Final Analytics

                            </a>

                        </article>

                    </ItemTemplate>

                </asp:Repeater>

            </div>

        </section>



        <%-- =====================================================
             HOST SESSION MODAL
             ===================================================== --%>

        <div id="hostSessionModal"
             class="session-modal"
             hidden>


            <div id="sessionModalOverlay"
                 class="session-modal-overlay">
            </div>



            <div class="session-modal-card">

                <div class="session-modal-header">

                    <div>

                        <h2>
                            Host Live Session
                        </h2>

                        <p>
                            Select a published quiz and create
                            a join code for students.
                        </p>

                    </div>


                    <button type="button"
                            id="btnCloseSessionModal"
                            class="modal-close-button">

                        <i class="bi bi-x-lg"></i>

                    </button>

                </div>

                <asp:ValidationSummary
                    ID="vsHostSession"
                    runat="server"
                    ValidationGroup="HostSession"
                    CssClass="validation-summary"
                    HeaderText="Please correct the following:"
                    DisplayMode="BulletList" />

                <%-- ERROR --%>

                <asp:Panel
                    ID="pnlHostError"
                    runat="server"
                    Visible="false"
                    CssClass="host-error">

                    <asp:Label
                        ID="lblHostError"
                        runat="server">
                    </asp:Label>

                </asp:Panel>



               <%-- QUIZ --%>

                <div class="modal-field">

                    <label for="<%= ddlPublishedQuiz.ClientID %>">

                        Select Published Quiz

                        <span class="required-mark">
                            *
                        </span>

                    </label>


                    <asp:DropDownList
                        ID="ddlPublishedQuiz"
                        runat="server"
                        CssClass="modal-input">
                    </asp:DropDownList>


                    <asp:RequiredFieldValidator
                        ID="rfvQuiz"
                        runat="server"
                        ControlToValidate="ddlPublishedQuiz"
                        InitialValue=""
                        ErrorMessage="Please select a published quiz."
                        CssClass="field-error"
                        ValidationGroup="HostSession"
                        Display="Dynamic"
                        SetFocusOnError="true">
                    </asp:RequiredFieldValidator>

                </div>



                <%-- SESSION TITLE --%>

                  <div class="modal-field">

                      <label for="<%= txtSessionTitle.ClientID %>">

                         Session Title / Class Label

                          <span class="required-mark">
                                *
                            </span>

                        </label>


                        <asp:TextBox
                            ID="txtSessionTitle"
                            runat="server"
                            CssClass="modal-input"
                            MaxLength="150"
                            placeholder="Example: 5 Alpha Morning Drill">
                        </asp:TextBox>


                        <asp:RequiredFieldValidator
                            ID="rfvSessionTitle"
                            runat="server"
                            ControlToValidate="txtSessionTitle"
                            ErrorMessage="Please enter a session title."
                            CssClass="field-error"
                            ValidationGroup="HostSession"
                            Display="Dynamic"
                            SetFocusOnError="true">
                        </asp:RequiredFieldValidator>

                    </div>



                <%-- JOIN CODE --%>

                <div class="generated-code-card">

                    <div>

                        <span>
                            Auto-Generated Join Code
                        </span>


                        <strong>
                            <asp:Label
                                ID="lblGeneratedCode"
                                runat="server">
                            </asp:Label>
                        </strong>

                    </div>


                    <asp:LinkButton
                        ID="btnRegenerateCode"
                        runat="server"
                        CssClass="regenerate-button"
                        CausesValidation="false"
                        OnClick="btnRegenerateCode_Click">

                        <i class="bi bi-arrow-clockwise"></i>

                        Regenerate

                    </asp:LinkButton>

                </div>



                <asp:HiddenField
                    ID="hfGeneratedCode"
                    runat="server" />


                <%--
                    Sessions.js reads this field after a postback.
                    Value 1 = reopen Host Session modal.
                --%>

                <asp:HiddenField
                    ID="hfOpenSessionModal"
                    runat="server"
                    ClientIDMode="Static"
                    Value="0" />



                <div class="session-modal-actions">

                    <button type="button"
                            id="btnCancelSession"
                            class="modal-cancel-button">

                        Cancel

                    </button>


                   <asp:Button
                    ID="btnCreateSession"
                    runat="server"
                    Text="Create & Launch"
                    CssClass="modal-create-button"
                    CausesValidation="true"
                    ValidationGroup="HostSession"
                    OnClick="btnCreateSession_Click" />

                </div>

            </div>

        </div>

    </div>

</asp:Content>



<asp:Content
    ID="SessionsScripts"
    ContentPlaceHolderID="PortalScripts"
    runat="server">

    <script src="<%= ResolveUrl("~/Scripts/Sessions.js") %>?v=1">
    </script>

</asp:Content>