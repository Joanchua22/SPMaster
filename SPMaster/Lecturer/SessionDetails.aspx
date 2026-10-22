<%@ Page Title="Session Details"
    Language="C#"
    MasterPageFile="~/Master/Portal.Master"
    AutoEventWireup="true"
    CodeBehind="SessionDetails.aspx.cs"
    Inherits="SPMaster.Lecturer.SessionDetails" %>


<asp:Content
    ID="SessionDetailsHead"
    ContentPlaceHolderID="PortalHead"
    runat="server">

    <link href="<%= ResolveUrl("~/Content/SessionDetails.css") %>?v=1"
          rel="stylesheet"
          type="text/css" />

</asp:Content>


<asp:Content
    ID="SessionDetailsContent"
    ContentPlaceHolderID="PortalContent"
    runat="server">

    <main class="session-details-page">


        <%-- =====================================================
             BACK
             ===================================================== --%>

        <a href="<%= ResolveUrl("~/Lecturer/Sessions.aspx") %>"
           class="session-details-back">

            <i class="bi bi-arrow-left"></i>

            Back to Sessions

        </a>



        <%-- =====================================================
             HEADER
             ===================================================== --%>

        <section class="session-details-header">

            <div class="session-details-header-left">

                <div class="session-details-title-row">

                    <h1>

                        <asp:Label
                            ID="lblSessionTitle"
                            runat="server">
                        </asp:Label>

                    </h1>


                    <asp:Label
                        ID="lblSessionStatus"
                        runat="server">
                    </asp:Label>

                </div>


                <p class="session-details-quiz">

                    <asp:Label
                        ID="lblQuizTitle"
                        runat="server">
                    </asp:Label>

                </p>


                <div class="session-details-meta">

                    <span>

                        <i class="bi bi-book"></i>

                        <asp:Label
                            ID="lblSubjectName"
                            runat="server">
                        </asp:Label>

                    </span>


                    <span>

                        <i class="bi bi-calendar3"></i>

                        <asp:Label
                            ID="lblCreatedAt"
                            runat="server">
                        </asp:Label>

                    </span>


                    <span>

                        <i class="bi bi-clock"></i>

                        <asp:Label
                            ID="lblTimeLimit"
                            runat="server">
                        </asp:Label>

                        min

                    </span>

                </div>

            </div>


            <div class="session-details-header-actions">

                <asp:Button
                    ID="btnRefresh"
                    runat="server"
                    Text="Refresh"
                    CssClass="session-refresh-button"
                    CausesValidation="false"
                    OnClick="btnRefresh_Click" />


                <asp:LinkButton
                    ID="btnCloseSession"
                    runat="server"
                    CssClass="session-close-button"
                    CausesValidation="false"
                    OnClick="btnCloseSession_Click"
                    OnClientClick="return confirm('Close this session? Students will no longer be able to join.');">

                    Close Session

                </asp:LinkButton>

            </div>

        </section>



        <%-- =====================================================
             JOIN CODE
             ===================================================== --%>

        <asp:Panel
            ID="pnlJoinCode"
            runat="server"
            CssClass="session-code-card">

            <div>

                <span class="session-code-label">
                    Session Join Code
                </span>


                <strong>

                    <asp:Label
                        ID="lblJoinCode"
                        runat="server">
                    </asp:Label>

                </strong>


                <p>
                    Students can enter this code from their dashboard
                    to join the live quiz session.
                </p>

            </div>


            <div class="session-live-indicator">

                <span></span>

                Live

            </div>

        </asp:Panel>



        <%-- =====================================================
             CLOSED MESSAGE
             ===================================================== --%>

        <asp:Panel
            ID="pnlClosedSession"
            runat="server"
            Visible="false"
            CssClass="session-closed-message">

            <i class="bi bi-lock"></i>

            This session is closed. Students can no longer join.

        </asp:Panel>



        <%-- =====================================================
             STATISTICS
             ===================================================== --%>

        <section class="session-details-stats">


            <%-- TOTAL ATTEMPTS --%>

            <article class="session-detail-stat-card">

                <span class="session-stat-icon stat-purple">

                    <i class="bi bi-people"></i>

                </span>


                <div>

                    <span class="session-stat-label">
                        Total Attempts
                    </span>


                    <strong>

                        <asp:Label
                            ID="lblTotalAttempts"
                            runat="server"
                            Text="0">
                        </asp:Label>

                    </strong>

                </div>

            </article>



            <%-- SUBMITTED --%>

            <article class="session-detail-stat-card">

                <span class="session-stat-icon stat-blue">

                    <i class="bi bi-send-check"></i>

                </span>


                <div>

                    <span class="session-stat-label">
                        Submitted
                    </span>


                    <strong>

                        <asp:Label
                            ID="lblSubmitted"
                            runat="server"
                            Text="0">
                        </asp:Label>

                    </strong>

                </div>

            </article>



            <%-- FULLY GRADED --%>

            <article class="session-detail-stat-card">

                <span class="session-stat-icon stat-green">

                    <i class="bi bi-check2-circle"></i>

                </span>


                <div>

                    <span class="session-stat-label">
                        Fully Graded
                    </span>


                    <strong>

                        <asp:Label
                            ID="lblFullyGraded"
                            runat="server"
                            Text="0">
                        </asp:Label>

                    </strong>

                </div>

            </article>



            <%-- PENDING --%>

            <article class="session-detail-stat-card">

                <span class="session-stat-icon stat-yellow">

                    <i class="bi bi-hourglass-split"></i>

                </span>


                <div>

                    <span class="session-stat-label">
                        Awaiting Review
                    </span>


                    <strong>

                        <asp:Label
                            ID="lblPendingReview"
                            runat="server"
                            Text="0">
                        </asp:Label>

                    </strong>

                </div>

            </article>



            <%-- AVERAGE --%>

            <article class="session-detail-stat-card">

                <span class="session-stat-icon stat-purple">

                    <i class="bi bi-graph-up"></i>

                </span>


                <div>

                    <span class="session-stat-label">
                        Average Score
                    </span>


                    <strong>

                        <asp:Label
                            ID="lblAverageScore"
                            runat="server"
                            Text="—">
                        </asp:Label>

                    </strong>

                </div>

            </article>

        </section>



        <%-- =====================================================
             PARTICIPANTS
             ===================================================== --%>

        <section class="session-participants-card">


            <div class="session-participants-header">

                <div>

                    <h2>
                        Student Attempts
                    </h2>

                    <p>
                        Monitor students who joined this session
                        and review their grading status.
                    </p>

                </div>


                <asp:Label
                    ID="lblParticipantCount"
                    runat="server"
                    CssClass="participant-count">
                </asp:Label>

            </div>



            <%-- NO PARTICIPANTS --%>

            <asp:Panel
                ID="pnlNoParticipants"
                runat="server"
                Visible="false"
                CssClass="session-no-participants">

                <i class="bi bi-people"></i>

                <strong>
                    No students have joined yet
                </strong>

                <span>
                    Student attempts will appear here after they
                    join using the session code.
                </span>

            </asp:Panel>



            <%-- TABLE --%>

            <asp:Panel
                ID="pnlParticipants"
                runat="server">

                <div class="session-table-wrapper">

                    <table class="session-participant-table">

                        <thead>

                            <tr>

                                <th>
                                    Student
                                </th>

                                <th>
                                    Started
                                </th>

                                <th>
                                    Submitted
                                </th>

                                <th>
                                    Status
                                </th>

                                <th>
                                    Score
                                </th>

                                <th>
                                    Action
                                </th>

                            </tr>

                        </thead>


                        <tbody>

                            <asp:Repeater
                                ID="rptParticipants"
                                runat="server">

                                <ItemTemplate>

                                    <tr>


                                        <%-- STUDENT --%>

                                        <td>

                                            <div class="participant-student">

                                                <span class="participant-avatar">

                                                    <%#
                                                        Eval("Initials")
                                                    %>

                                                </span>


                                                <div>

                                                    <strong>

                                                        <%#
                                                            Eval("StudentName")
                                                        %>

                                                    </strong>


                                                    <span>

                                                        <%#
                                                            Eval("StudentEmail")
                                                        %>

                                                    </span>

                                                </div>

                                            </div>

                                        </td>



                                        <%-- STARTED --%>

                                        <td>

                                            <%#
                                                Convert.ToDateTime(
                                                    Eval("StartedAt")
                                                ).ToString(
                                                    "dd MMM, hh:mm tt"
                                                )
                                            %>

                                        </td>



                                        <%-- SUBMITTED --%>

                                        <td>

                                            <%#
                                                Eval("SubmittedAt")
                                                    == DBNull.Value
                                                ? "—"
                                                : Convert.ToDateTime(
                                                    Eval("SubmittedAt")
                                                ).ToString(
                                                    "dd MMM, hh:mm tt"
                                                )
                                            %>

                                        </td>



                                        <%-- STATUS --%>

                                        <td>

                                            <span class='<%#
                                                Eval("StatusCss")
                                            %>'>

                                                <%#
                                                    Eval("StatusText")
                                                %>

                                            </span>

                                        </td>



                                        <%-- SCORE --%>

                                        <td>

                                            <strong class="participant-score">

                                                <%#
                                                    Eval("ScoreText")
                                                %>

                                            </strong>

                                        </td>



                                        <%-- ACTION --%>

                                        <td>

                                            <asp:Panel
                                                runat="server"
                                                Visible='<%#
                                                    Convert.ToBoolean(
                                                        Eval("HasAction")
                                                    )
                                                %>'>

                                                <a href='<%#
                                                        Eval("ActionUrl")
                                                    %>'
                                                   class='<%#
                                                        Eval("ActionCss")
                                                    %>'>

                                                    <%#
                                                        Eval("ActionText")
                                                    %>

                                                </a>

                                            </asp:Panel>


                                            <asp:Panel
                                                runat="server"
                                                Visible='<%#
                                                    !Convert.ToBoolean(
                                                        Eval("HasAction")
                                                    )
                                                %>'>

                                                <span class="participant-no-action">
                                                    —
                                                </span>

                                            </asp:Panel>

                                        </td>

                                    </tr>

                                </ItemTemplate>

                            </asp:Repeater>

                        </tbody>

                    </table>

                </div>

            </asp:Panel>

        </section>


    </main>

</asp:Content>