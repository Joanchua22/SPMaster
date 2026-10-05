<%@ Page Title="Question Bank" Language="C#" MasterPageFile="~/Master/Portal.Master" AutoEventWireup="true" CodeBehind="QuestionBank.aspx.cs" Inherits="SPMaster.Lecturer.QuestionBank" %>

<asp:Content ID="QuestionBankHead" ContentPlaceHolderID="PortalHead" runat="server">
    <link href="<%= ResolveUrl("~/Content/QuestionBank.css") %>?v=3" rel="stylesheet" type="text/css" />
    <link href="<%= ResolveUrl("~/Content/QuestionPreview.css") %>?v=1" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="QuestionBankContent" ContentPlaceHolderID="PortalContent" runat="server">

    <div class="question-bank-page">



        <%-- =========================
             PAGE HEADER
             ========================= --%>

        <section class="question-bank-header">

            <div>
                <h1>Question Bank</h1>
                <p> Browse, create, and organize SPM curriculum questions</p>
            </div>

            <a href="<%= ResolveUrl("~/Lecturer/CreateQuestion.aspx") %>" class="add-question-button">
                <i class="bi bi-plus-circle" aria-hidden="true"></i>
                Add New Question
            </a>

        </section>



        <%-- =========================
             SEARCH AND FILTERS
             ========================= --%>

        <section class="question-filter-card">

            <%-- SEARCH --%>

            <div class="question-search">

                <i class="bi bi-search search-icon" aria-hidden="true"></i>
                <asp:TextBox ID="txtSearch" runat="server" AutoPostBack="true" OnTextChanged="FilterChanged" placeholder="Search by question text or keyword..." aria-label="Search questions"></asp:TextBox>

            </div>


            <%-- SUBJECT --%>

            <div class="question-filter-wrapper">

                <asp:DropDownList ID="ddlSubject" runat="server" CssClass="question-filter" AutoPostBack="true" OnSelectedIndexChanged="FilterChanged" aria-label="Filter by subject"></asp:DropDownList>
                <i class="bi bi-caret-down-fill filter-arrow" aria-hidden="true"></i>

            </div>


            <%-- TYPE --%>

            <div class="question-filter-wrapper">

                <asp:DropDownList ID="ddlQuestionType" runat="server" CssClass="question-filter" AutoPostBack="true" OnSelectedIndexChanged="FilterChanged" aria-label="Filter by question type">

                    <asp:ListItem Text="All Types" Value=""></asp:ListItem>
                    <asp:ListItem Text="MCQ" Value="MCQ"></asp:ListItem>
                    <asp:ListItem Text="Subjective" Value="Subjective"> </asp:ListItem>

                </asp:DropDownList>

                <i class="bi bi-caret-down-fill filter-arrow" aria-hidden="true"></i>

            </div>


            <%-- DIFFICULTY --%>

            <div class="question-filter-wrapper">

                <asp:DropDownList ID="ddlDifficulty" runat="server" CssClass="question-filter" AutoPostBack="true" OnSelectedIndexChanged="FilterChanged" aria-label="Filter by difficulty">

                    <asp:ListItem Text="All Difficulties" Value=""></asp:ListItem>
                    <asp:ListItem Text="Easy" Value="Easy"></asp:ListItem>
                    <asp:ListItem Text="Medium" Value="Medium"></asp:ListItem>
                    <asp:ListItem Text="Hard" Value="Hard"></asp:ListItem>

                </asp:DropDownList>

                <i class="bi bi-caret-down-fill filter-arrow" aria-hidden="true"></i>

            </div>

        </section>



        <%-- =========================
             QUESTION BANK TABS
             ========================= --%>

        <div class="question-tabs">

            <asp:LinkButton ID="btnMyQuestions" runat="server" CssClass="question-tab active" OnClick="btnMyQuestions_Click"></asp:LinkButton>
            <asp:LinkButton ID="btnSharedQuestions" runat="server" CssClass="question-tab" OnClick="btnSharedQuestions_Click"></asp:LinkButton>

        </div>



        <%-- =========================
             QUESTION LIST
             ========================= --%>

        <section class="question-list">

            <asp:Repeater ID="rptQuestions" runat="server" OnItemCommand="rptQuestions_ItemCommand">
                <ItemTemplate>
                    <article class="question-card">
                        <div class="question-card-top">


                            <%-- QUESTION TAGS --%>

                            <div class="question-tags">

                                <span class='question-tag subject-tag <%# GetSubjectCss(Eval("SubjectName")) %>'><%#: Eval("SubjectName") %></span>
                                <span class='question-tag <%# GetDifficultyCss(Eval("Difficulty")) %>'> <%#: Eval("Difficulty") %> </span>
                                <span class="question-tag type-tag"> <%#: GetQuestionTypeDisplay(Eval("QuestionType")) %></span>


                                <%-- IMAGE TAG --%>

                                <asp:PlaceHolder runat="server" Visible='<%# HasValue(Eval("MediaPath")) %>'>

                                    <span class="question-tag media-tag">
                                        <i class="bi bi-image"aria-hidden="true"></i>
                                        Image attached
                                    </span>

                                </asp:PlaceHolder>


                                <span class="question-owner"> <%#: GetOwnerText(Eval("IsOwner"),Eval("CreatorName")) %></span>

                            </div>



                            <%-- ACTION BUTTONS --%>

                            <div class="question-actions">

                                <%-- CURRENT LECTURER'S QUESTION --%>

                                <asp:PlaceHolder runat="server" Visible='<%# Convert.ToBoolean(Eval("IsOwner")) %>'>

                                    <asp:LinkButton runat="server" CssClass="question-action" CommandName="EditQuestion" CommandArgument='<%# Eval("QuestionId") %>'> Edit</asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="question-action" CommandName="ArchiveQuestion" CommandArgument='<%# Eval("QuestionId") %>' OnClientClick="return confirm('Archive this question?');">Archive</asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="question-action question-action-danger" CommandName="DeleteQuestion" CommandArgument='<%# Eval("QuestionId") %>' OnClientClick="return confirm('Permanently delete this question?');">Delete</asp:LinkButton>

                                </asp:PlaceHolder>


                                <%-- OTHER LECTURER'S QUESTION --%>

                                <asp:PlaceHolder runat="server" Visible='<%# !Convert.ToBoolean(Eval("IsOwner")) %>'>

                                    <button type="button"class="question-action preview-question-button"

                                            data-preview-question

                                            data-subject='<%# AttributeEncode(Eval("SubjectName")) %>'

                                            data-difficulty='<%# AttributeEncode(Eval("Difficulty")) %>'

                                            data-type='<%# AttributeEncode(Eval("QuestionType")) %>'

                                            data-question='<%# AttributeEncode(Eval("QuestionText")) %>'

                                            data-option-a='<%# AttributeEncode(Eval("OptionA")) %>'

                                            data-option-b='<%# AttributeEncode(Eval("OptionB")) %>'

                                            data-option-c='<%# AttributeEncode(Eval("OptionC")) %>'

                                            data-option-d='<%# AttributeEncode(Eval("OptionD")) %>'

                                            data-correct-option='<%# AttributeEncode(Eval("CorrectOption")) %>'

                                            data-model-answer='<%# AttributeEncode(Eval("ModelAnswer")) %>'

                                            data-image-url='<%# AttributeEncode(GetMediaUrl(Eval("MediaPath"))) %>'>

                                        Preview

                                    </button>

                                    <asp:LinkButton runat="server" CssClass="question-action question-action-primary" CommandName="AddToQuiz" CommandArgument='<%# Eval("QuestionId") %>'>Add to My Quiz</asp:LinkButton>

                                </asp:PlaceHolder>

                            </div>

                        </div>



                        <%-- QUESTION TEXT --%>

                        <h2 class="question-text"><%#: Eval("QuestionText") %></h2>

                        <%-- QUESTION META --%>

                        <div class="question-meta">


                            <%-- MCQ --%>

                            <asp:PlaceHolder runat="server" Visible='<%# IsMcq(Eval("QuestionType")) %>'>
                                <span> <%#: FormatCorrectAnswer(Eval("CorrectOption"),Eval("CorrectAnswerText")) %></span>
                            </asp:PlaceHolder>


                            <%-- SUBJECTIVE --%>

                            <asp:PlaceHolder runat="server" Visible='<%# !IsMcq(Eval("QuestionType")) %>'>
                                <span><%#: GetModelAnswerStatus(Eval("ModelAnswer")) %></span>
                            </asp:PlaceHolder>

                            <span class="meta-dot">•</span>


                            <span>Used in <%#: Eval("UsedInQuizzes") %> Quiz(es)</span>

                        </div>

                    </article>

                </ItemTemplate>

            </asp:Repeater>



            <%-- NO RESULTS --%>

            <asp:Panel ID="pnlNoQuestions" runat="server" Visible="false">

                <div class="question-card">
                    <h2 class="question-text">No questions found.</h2>

                    <div class="question-meta">
                        Try changing your search or filter.
                    </div>

                </div>

            </asp:Panel>

        </section>
    
    </div>



    <%-- =========================================================
         QUESTION PREVIEW MODAL
         Reused with QuestionPreview.css / QuestionPreview.js
         ========================================================= --%>

    <div id="questionPreviewModal" class="question-preview-modal" hidden>


        <%-- OVERLAY --%>
        <div id="previewOverlay" class="question-preview-overlay" data-preview-close>
        </div>


        <%-- MODAL DIALOG --%>

        <div class="question-preview-dialog" role="dialog" aria-modal="true" aria-labelledby="previewTitle">


            <%-- HEADER --%>

            <div class="question-preview-header">

                <div>
                    <span class="preview-label">Question Preview</span>
                    <h2 id="previewTitle">Student View</h2>
                </div>

                <button type="button" id="btnClosePreview" class="preview-close-button" data-preview-close aria-label="Close preview">
                    <i class="bi bi-x-lg" aria-hidden="true"></i>
                </button>

            </div>



            <%-- BODY --%>

            <div class="question-preview-body">


                <%-- TAGS --%>

                <div class="preview-tags">

                    <span id="previewSubject" class="preview-tag"></span>
                    <span id="previewDifficulty" class="preview-tag"></span>
                    <span id="previewType" class="preview-tag"></span>

                </div>


                <%-- QUESTION TEXT --%>

                <h3 id="previewQuestionText" class="preview-question-text"></h3>


                <%-- OPTIONAL IMAGE --%>

                <div id="previewImageArea" class="preview-image-area" hidden>
                    <img id="previewImage" src="" alt="Question stimulus preview" />
                </div>


                <%-- MCQ ANSWERS --%>

                <div id="previewMcqAnswers" class="preview-answer-list">

                    <div class="preview-answer" data-preview-option="A">

                        <span> A </span>

                        <p id="previewOptionA">
                        </p>

                    </div>


                    <div class="preview-answer"
                         data-preview-option="B">

                        <span>
                            B
                        </span>

                        <p id="previewOptionB">
                        </p>

                    </div>


                    <div class="preview-answer"
                         data-preview-option="C">

                        <span>
                            C
                        </span>

                        <p id="previewOptionC">
                        </p>

                    </div>


                    <div class="preview-answer"
                         data-preview-option="D">

                        <span>
                            D
                        </span>

                        <p id="previewOptionD">
                        </p>

                    </div>

                </div>



                <%-- SUBJECTIVE MODEL ANSWER --%>

                <div id="previewSubjectiveAnswer"
                     class="preview-subjective"
                     hidden>

                    <label>
                        Model Answer / Scoring Guide
                    </label>

                    <p id="previewModelAnswer">
                    </p>

                </div>

            </div>

        </div>

    </div>

</asp:Content>



<asp:Content ID="QuestionBankScripts" ContentPlaceHolderID="PortalScripts" runat="server">

    <script src="<%= ResolveUrl("~/Scripts/QuestionPreview.js") %>?v=1">
    </script>

</asp:Content>