<%@ Page Title="Performance" Language="C#" MasterPageFile="~/Master/Portal.Master" AutoEventWireup="true" CodeBehind="Performance.aspx.cs" Inherits="SPMaster.Lecturer.Performance" %>

<asp:Content ID="PerformanceHead" ContentPlaceHolderID="PortalHead" runat="server">
    <link href="<%= ResolveUrl("~/Content/LecturerPerformance.css") %>?v=2" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="PerformanceContent" ContentPlaceHolderID="PortalContent" runat="server">

    <main class="lecturer-performance-page">

        <%-- =====================================================
             PAGE HEADER
             ===================================================== --%>

        <section class="performance-page-header">

            <div class="performance-heading">
                <h1>Cohort Performance Analytics</h1>
                <p>Grading distributions and question-level performance insights</p>
            </div>

            <div class="performance-header-actions">

                <%-- =============================================
                     QUIZ SELECTOR
                     ============================================= --%>

                <div class="performance-quiz-select-wrapper">
                    <asp:DropDownList ID="ddlQuiz" runat="server" CssClass="performance-quiz-select" AutoPostBack="true" OnSelectedIndexChanged="ddlQuiz_SelectedIndexChanged"></asp:DropDownList>
                    <i class="bi bi-chevron-down performance-quiz-arrow" aria-hidden="true"></i>
                </div>

            </div>

        </section>



        <%-- =====================================================
             NO QUIZZES
             ===================================================== --%>

        <asp:Panel ID="pnlNoQuizzes" runat="server" Visible="false" CssClass="performance-empty-state">

            <i class="bi bi-bar-chart" aria-hidden="true"></i>
            <h2>No quiz performance available</h2>
            <p>Publish a quiz first. Performance analytics will appear after students submit their attempts.</p>

        </asp:Panel>



        <%-- =====================================================
             PERFORMANCE CONTENT
             ===================================================== --%>

        <asp:Panel ID="pnlPerformance" runat="server" Visible="false">

            <%-- =================================================
                 OVERVIEW STATISTICS
                 ================================================= --%>

            <section class="performance-stat-grid">


                <%-- =============================================
                     TOTAL ATTEMPTS
                     ============================================= --%>

                <article class="performance-stat-card">

                    <span class="performance-stat-label">Total Attempts</span>

                    <strong class="performance-stat-value">
                        <asp:Literal ID="litTotalAttempts" runat="server"></asp:Literal>
                    </strong>

                    <span class="performance-stat-description">
                        <asp:Literal ID="litUniqueStudents" runat="server"></asp:Literal>
                    </span>

                </article>



                <%-- =============================================
                     GRADED SUBMISSIONS
                     ============================================= --%>

                <article class="performance-stat-card">

                    <span class="performance-stat-label">Graded Submissions</span>

                    <strong class="performance-stat-value performance-stat-purple">
                        <asp:Literal ID="litGradedSubmissions" runat="server"></asp:Literal>
                    </strong>

                    <span class="performance-stat-description performance-stat-warning">
                        <asp:Literal ID="litPendingSubmissions" runat="server"></asp:Literal>
                    </span>

                </article>



                <%-- =============================================
                     AVERAGE SCORE
                     ============================================= --%>

                <article class="performance-stat-card">

                    <span class="performance-stat-label">Average Graded Score</span>

                    <strong class="performance-stat-value">
                        <asp:Literal ID="litAverageScore" runat="server"></asp:Literal>
                    </strong>

                    <span class="performance-stat-description">
                        <asp:Literal ID="litAverageDescription" runat="server"></asp:Literal>
                    </span>

                </article>



                <%-- =============================================
                     MEDIAN COMPLETION TIME
                     ============================================= --%>

                <article class="performance-stat-card">

                    <span class="performance-stat-label">Median Completion Time</span>

                    <strong class="performance-stat-value">
                        <asp:Literal ID="litMedianCompletion" runat="server"></asp:Literal>
                    </strong>

                    <span class="performance-stat-description">
                        <asp:Literal ID="litAllocatedTime" runat="server"></asp:Literal>
                    </span>

                </article>

            </section>



            <%-- =================================================
                 ANALYTICS
                 ================================================= --%>

            <section class="performance-analytics-grid">

                <%-- =============================================
                     GRADE BAND DISTRIBUTION
                     ============================================= --%>

                <article class="performance-panel grade-distribution-panel">

                    <h2>Grade Band Distribution</h2>

                    <%-- =========================================
                         GRADE A
                         ========================================= --%>

                    <div class="grade-band-item">

                        <div class="grade-band-heading">
                            <span>Grade A (75% - 100%)</span>
                            <span>
                                <asp:Literal ID="litGradeA" runat="server"></asp:Literal>
                            </span>
                        </div>

                        <div class="grade-band-track">
                            <div id="barGradeA" runat="server" class="grade-band-fill grade-a"></div>
                        </div>

                    </div>



                    <%-- =========================================
                         GRADE B
                         ========================================= --%>

                    <div class="grade-band-item">

                        <div class="grade-band-heading">
                            <span>Grade B (60% - 74%)</span>
                            <span>
                                <asp:Literal ID="litGradeB" runat="server"></asp:Literal>
                            </span>
                        </div>

                        <div class="grade-band-track">
                            <div id="barGradeB" runat="server" class="grade-band-fill grade-b"></div>
                        </div>

                    </div>



                    <%-- =========================================
                         GRADE C
                         ========================================= --%>

                    <div class="grade-band-item">

                        <div class="grade-band-heading">
                            <span>Grade C (40% - 59%)</span>
                            <span>
                                <asp:Literal ID="litGradeC" runat="server"></asp:Literal>
                            </span>
                        </div>

                        <div class="grade-band-track">
                            <div id="barGradeC" runat="server" class="grade-band-fill grade-c"></div>
                        </div>

                    </div>



                    <%-- =========================================
                         GRADE D / E
                         ========================================= --%>

                    <div class="grade-band-item">

                        <div class="grade-band-heading">
                            <span>Grade D/E (&lt; 40%)</span>
                            <span>
                                <asp:Literal ID="litGradeDE" runat="server"></asp:Literal>
                            </span>
                        </div>

                        <div class="grade-band-track">
                            <div id="barGradeDE" runat="server" class="grade-band-fill grade-de"></div>
                        </div>

                    </div>


                </article>



                <%-- =============================================
                     QUESTION PERFORMANCE
                     ============================================= --%>

                <article class="performance-panel question-analysis-panel">


                    <div class="performance-panel-heading">

                        <div>
                            <h2>Question Performance Analysis</h2>
                            <p>Review the performance of every question in the selected quiz.</p>
                        </div>

                        <span class="question-count">
                            <asp:Literal ID="litQuestionCount" runat="server"></asp:Literal>
                        </span>

                    </div>


                    <div class="question-performance-scroll">

                        <div class="question-performance-table">

                            <%-- =================================
                                 HEADER
                                 ================================= --%>

                            <div class="question-performance-row question-performance-header">
                                <span>Question Item</span>
                                <span>Type</span>
                                <span>Correct / Avg Rate</span>
                            </div>



                            <%-- =================================
                                 DATABASE QUESTIONS
                                 ================================= --%>

                            <asp:Repeater ID="rptQuestionPerformance" runat="server">

                                <ItemTemplate>

                                    <div class="question-performance-row">
                                        <span class="question-item-name"title='<%# AttributeEncode(Eval("QuestionText")) %>'><%# GetQuestionDisplay(Eval("QuestionOrder"),Eval("QuestionText")) %></span>
                                        <span class="question-type"><%# GetQuestionTypeDisplay(Eval("QuestionType")) %></span>
                                        <span class='<%# GetRateCss(Eval("PerformanceRate")) %>'><%# GetRateText(Eval("QuestionType"),Eval("PerformanceRate")) %></span>
                                    </div>

                                </ItemTemplate>

                            </asp:Repeater>



                            <%-- =================================
                                 NO QUESTION DATA
                                 ================================= --%>

                            <asp:Panel ID="pnlNoQuestionPerformance" runat="server" Visible="false" CssClass="question-performance-empty">
                                No question performance data is available yet.
                            </asp:Panel>

                        </div>

                    </div>

                </article>

            </section>

        </asp:Panel>

    </main>

</asp:Content>


<asp:Content ID="PerformanceScripts" ContentPlaceHolderID="PortalScripts" runat="server"></asp:Content>