<%@ Page Title="Start Quiz"
    Language="C#"
    AutoEventWireup="true"
    CodeBehind="StartQuiz.aspx.cs"
    Inherits="SPMaster.Student.StartQuiz" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <meta charset="utf-8" />

    <meta name="viewport"
          content="width=device-width, initial-scale=1.0" />

    <title>Quiz - SPMaster</title>


    <%-- BOOTSTRAP ICONS --%>

    <link rel="stylesheet"
          href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.13.1/font/bootstrap-icons.min.css" />


    <%-- PAGE CSS --%>

    <link href="<%= ResolveUrl("~/Content/StudentStartQuiz.css") %>?v=5"
          rel="stylesheet"
          type="text/css" />

</head>


<body class="student-start-quiz-body">

<form id="form1"
      runat="server">


    <%-- =====================================================
         HIDDEN CONTROLS
         ===================================================== --%>

    <asp:HiddenField
        ID="hfRemainingSeconds"
        runat="server"
        ClientIDMode="Static" />

    <asp:HiddenField
        ID="hfAttemptId"
        runat="server"
        ClientIDMode="Static" />

    <asp:HiddenField
        ID="hfQuizQuestionId"
        runat="server"
        ClientIDMode="Static" />


    <asp:HiddenField
        ID="hfGoQuestionIndex"
        runat="server"
        ClientIDMode="Static" />


    <asp:Button
        ID="btnGoQuestionServer"
        runat="server"
        ClientIDMode="Static"
        Style="display:none;"
        CausesValidation="false"
        OnClick="btnGoQuestionServer_Click" />


    <asp:Button
        ID="btnTimeUp"
        runat="server"
        ClientIDMode="Static"
        Style="display:none;"
        CausesValidation="false"
        OnClick="btnTimeUp_Click" />



    <%-- =====================================================
         NORMAL QUIZ INTERFACE
         ===================================================== --%>

    <asp:Panel
        ID="pnlQuizInterface"
        runat="server"
        Visible="true">


        <main class="student-start-quiz-page">


            <%-- =================================================
                 QUIZ HEADER
                 ================================================= --%>

            <section class="start-quiz-header">


                <%-- LEFT --%>

                <div class="start-quiz-header-left">

                    <asp:LinkButton
                        ID="btnExitQuiz"
                        runat="server"
                        CssClass="quiz-close-button"
                        CausesValidation="false"
                        OnClientClick="return confirmExitQuiz();"
                        OnClick="btnExitQuiz_Click"
                        aria-label="Exit quiz">

                        <i class="bi bi-x-lg"
                           aria-hidden="true"></i>

                    </asp:LinkButton>


                    <div class="start-quiz-title">

                        <strong>

                            <asp:Literal
                                ID="litQuizTitle"
                                runat="server">
                            </asp:Literal>

                        </strong>


                        <span>

                            <asp:Literal
                                ID="litCurrentQuestion"
                                runat="server">
                            </asp:Literal>

                        </span>

                    </div>

                </div>



                <%-- TIMER --%>

                <div class="quiz-timer">

                    <i class="bi bi-stopwatch"
                       aria-hidden="true"></i>

                    <span id="quizTimer">
                        00:00
                    </span>

                </div>



                <%-- SUBMIT QUIZ --%>

                <button type="button"
                        class="submit-quiz-button"
                        onclick="openSubmitModal();">

                    Submit Quiz

                </button>

            </section>



            <%-- =================================================
                 QUESTION NUMBER NAVIGATION
                 ================================================= --%>

            <section class="question-number-navigation">

                <asp:Repeater
                    ID="rptQuestionNavigation"
                    runat="server">

                    <ItemTemplate>

                        <button type="button"
                                class='<%# GetQuestionNavigationCss(
                                    Eval("QuestionIndex"),
                                    Eval("IsAnswered")
                                ) %>'
                                onclick='goToQuestion(<%# Eval("QuestionIndex") %>);'>

                            <%#: Eval("QuestionNumber") %>

                        </button>

                    </ItemTemplate>

                </asp:Repeater>

            </section>



            <%-- =================================================
                 QUESTION CARD
                 ================================================= --%>

            <section class="student-question-card">


                <%-- =============================================
                     QUESTION HEADER
                     ============================================= --%>

                <div class="student-question-top">

                    <span class="question-type-badge">

                        <asp:Literal
                            ID="litQuestionType"
                            runat="server">
                        </asp:Literal>

                    </span>


                    <span class="question-marks-badge">

                        <asp:Literal
                            ID="litQuestionMarks"
                            runat="server">
                        </asp:Literal>

                    </span>

                </div>



                <%-- =============================================
                     QUESTION TEXT
                     ============================================= --%>

                <h1 class="student-question-title">

                    <asp:Literal
                        ID="litQuestionText"
                        runat="server">
                    </asp:Literal>

                </h1>


                <p class="student-question-instruction">

                    <asp:Literal
                        ID="litQuestionInstruction"
                        runat="server">
                    </asp:Literal>

                </p>



                <%-- =============================================
                     OPTIONAL QUESTION IMAGE
                     ============================================= --%>

                <asp:Panel
                    ID="pnlQuestionImage"
                    runat="server"
                    Visible="false"
                    CssClass="student-question-image">

                    <asp:Image
                        ID="imgQuestion"
                        runat="server"
                        AlternateText="Question image" />

                </asp:Panel>



                <%-- =================================================
                     OBJECTIVE / MCQ
                     ================================================= --%>

                <asp:PlaceHolder
                    ID="phObjective"
                    runat="server">

                    <section class="objective-question-section">

                        <asp:Repeater
                            ID="rptOptions"
                            runat="server">

                            <ItemTemplate>

                                <label class="student-answer-option">

                                    <input type="radio"
                                           name="objectiveAnswer"
                                           value='<%# Eval("OptionId") %>'
                                           <%# GetOptionChecked(
                                               Eval("OptionId")
                                           ) %> />

                                    <span class="answer-option-letter">

                                        <%#: Eval("OptionLetter") %>

                                    </span>


                                    <span class="answer-option-text">

                                        <%#: Eval("OptionText") %>

                                    </span>

                                </label>

                            </ItemTemplate>

                        </asp:Repeater>

                    </section>

                </asp:PlaceHolder>



                <%-- =================================================
                     SUBJECTIVE
                     ================================================= --%>

                <asp:PlaceHolder
                    ID="phSubjective"
                    runat="server">

                    <section class="subjective-question-section">


                        <label for="txtSubjectiveAnswer"
                               class="subjective-answer-label">

                            Your Answer:

                        </label>


                        <asp:TextBox
                            ID="txtSubjectiveAnswer"
                            runat="server"
                            ClientIDMode="Static"
                            TextMode="MultiLine"
                            MaxLength="5000"
                            CssClass="subjective-answer-textarea"
                            placeholder="Write your answer clearly here...">
                        </asp:TextBox>


                        <div class="subjective-answer-footer">

                            <span>

                                Maximum 5000 characters.
                                Your lecturer will evaluate your answer
                                based on the allocated marks.

                            </span>


                            <span id="subjectiveCharacterCount">
                                0 / 5000
                            </span>

                        </div>


                    </section>

                </asp:PlaceHolder>


                <div class="answer-auto-save-row">
                    <span id="autoSaveStatus"
                          class="answer-auto-save-status">
                        Auto-save ready
                    </span>
                </div>


                <%-- =================================================
                     QUESTION FOOTER
                     ================================================= --%>

                <div class="student-question-footer">


                    <%-- CLEAR ANSWER --%>

                    <asp:LinkButton
                        ID="btnClearAnswer"
                        runat="server"
                        CausesValidation="false"
                        CssClass="clear-answer-button"
                        OnClick="btnClearAnswer_Click">

                        <i class="bi bi-eraser"
                           aria-hidden="true"></i>

                        Clear answer

                    </asp:LinkButton>



                    <%-- PREVIOUS / NEXT --%>

                    <div class="question-navigation-actions">

                        <asp:Button
                            ID="btnPreviousQuestion"
                            runat="server"
                            CausesValidation="false"
                            CssClass="question-nav-button question-nav-light"
                            Text="Previous"
                            OnClick="btnPreviousQuestion_Click" />


                        <asp:Button
                            ID="btnNextQuestion"
                            runat="server"
                            CausesValidation="false"
                            CssClass="question-nav-button question-nav-primary"
                            Text="Next"
                            OnClick="btnNextQuestion_Click" />

                    </div>


                </div>


            </section>


        </main>


    </asp:Panel>



    <%-- =====================================================
         EXPIRED SESSION
         ===================================================== --%>

    <asp:Panel
        ID="pnlExpiredAttempt"
        runat="server"
        Visible="false"
        CssClass="expired-attempt-layer">


        <%-- DARK OVERLAY --%>

        <div class="expired-attempt-overlay">
        </div>



        <%-- MODAL --%>

        <div class="expired-attempt-dialog">


            <div class="expired-attempt-icon">

                <i class="bi bi-clock-history"
                   aria-hidden="true"></i>

            </div>


            <h2>
                Session Expired
            </h2>


            <p>

                The time limit for this quiz attempt has already
                expired. You can start the quiz again with a new
                attempt, or return to My Attempts.

            </p>



            <div class="expired-attempt-actions">


                <%-- BACK --%>

                <asp:Button
                    ID="btnLeaveExpiredAttempt"
                    runat="server"
                    CausesValidation="false"
                    CssClass="expired-attempt-leave"
                    Text="Back to My Attempts"
                    OnClick="btnLeaveExpiredAttempt_Click" />


                <%-- REDO --%>

                <asp:Button
                    ID="btnRedoExpiredAttempt"
                    runat="server"
                    CausesValidation="false"
                    CssClass="expired-attempt-redo"
                    Text="Redo Quiz"
                    OnClick="btnRedoExpiredAttempt_Click" />


            </div>


        </div>


    </asp:Panel>



    <%-- =====================================================
         SUBMIT CONFIRMATION MODAL
         ===================================================== --%>

    <div id="submitQuizModal"
         class="quiz-submit-modal"
         hidden>


        <div class="quiz-submit-overlay"
             onclick="closeSubmitModal();">
        </div>


        <div class="quiz-submit-dialog">


            <div class="submit-modal-icon">

                <i class="bi bi-check2-circle"
                   aria-hidden="true"></i>

            </div>


            <h2>
                Submit Quiz?
            </h2>


            <p>

                Once submitted, you will not be able
                to change your answers.

            </p>


            <div class="submit-modal-actions">


                <button type="button"
                        class="submit-modal-cancel"
                        onclick="closeSubmitModal();">

                    Continue Quiz

                </button>


                <asp:Button
                    ID="btnConfirmSubmit"
                    runat="server"
                    CausesValidation="false"
                    CssClass="submit-modal-confirm"
                    Text="Submit Quiz"
                    OnClick="btnConfirmSubmit_Click" />


            </div>


        </div>


    </div>

    <%-- =====================================================
         TIME UP / AUTO SUBMITTED MODAL
         ===================================================== --%>

    <asp:Panel
        ID="pnlTimeUpSubmitted"
        runat="server"
        Visible="false"
        CssClass="time-up-layer">


        <div class="time-up-overlay">
        </div>


        <div class="time-up-dialog">


            <div class="time-up-icon">

                <i class="bi bi-clock-history"
                   aria-hidden="true"></i>

            </div>


            <h2>
                Time's Up!
            </h2>


            <p>
                Your quiz time has ended and your answers
                have been submitted automatically.
            </p>


            <asp:Button
                ID="btnTimeUpContinue"
                runat="server"
                CausesValidation="false"
                CssClass="time-up-continue-button"
                Text="View My Attempts"
                OnClick="btnTimeUpContinue_Click" />


        </div>


    </asp:Panel>


</form>



<script>

    /* =========================================================
       GO TO QUESTION
       ========================================================= */

    function goToQuestion(index) {

        const indexField =
            document.getElementById(
                "hfGoQuestionIndex"
            );


        const goButton =
            document.getElementById(
                "btnGoQuestionServer"
            );


        if (
            !indexField ||
            !goButton
        ) {
            return;
        }


        indexField.value =
            index;


        goButton.click();
    }



    /* =========================================================
       EXIT QUIZ
       ========================================================= */

    function confirmExitQuiz() {

        return confirm(
            "Are you sure you want to leave this quiz? "
            + "Your current answer will be saved and "
            + "you can continue later."
        );
    }



    /* =========================================================
       SUBMIT MODAL
       ========================================================= */

    function openSubmitModal() {

        const modal =
            document.getElementById(
                "submitQuizModal"
            );


        if (!modal) {
            return;
        }


        modal.hidden =
            false;


        document.body.classList.add(
            "modal-open"
        );
    }


    function closeSubmitModal() {

        const modal =
            document.getElementById(
                "submitQuizModal"
            );


        if (!modal) {
            return;
        }


        modal.hidden =
            true;


        document.body.classList.remove(
            "modal-open"
        );
    }



    /* =========================================================
       SUBJECTIVE CHARACTER COUNT
       ========================================================= */

    const subjectiveBox =
        document.getElementById(
            "txtSubjectiveAnswer"
        );


    const characterCount =
        document.getElementById(
            "subjectiveCharacterCount"
        );


    function updateCharacterCount() {

        if (
            !subjectiveBox ||
            !characterCount
        ) {
            return;
        }


        characterCount.textContent =
            subjectiveBox.value.length
            + " / 5000";
    }


    if (subjectiveBox) {

        subjectiveBox.addEventListener(
            "input",
            updateCharacterCount
        );


        updateCharacterCount();
}

    /* =========================================================
       ANSWER AUTO-SAVE API
       ========================================================= */

    let answerSaveTimer =
        null;


    let answerSaveInProgress =
        false;


    let answerSavePending =
        false;



    function setAutoSaveStatus(
        message,
        state
    ) {

        const status =
            document.getElementById(
                "autoSaveStatus"
            );


        if (!status) {
            return;
        }


        status.textContent =
            message;


        status.className =
            "answer-auto-save-status";


        if (state) {

            status.classList.add(
                state
            );
        }
    }



    /* ---------------------------------------------------------
       SAVE CURRENT ANSWER
       --------------------------------------------------------- */

    async function saveAnswerWithApi() {

        // If another save is currently running,
        // queue one more save using the newest answer.

        if (answerSaveInProgress) {

            answerSavePending =
                true;

            return;
        }


        const attemptField =
            document.getElementById(
                "hfAttemptId"
            );


        const quizQuestionField =
            document.getElementById(
                "hfQuizQuestionId"
            );


        if (
            !attemptField ||
            !attemptField.value ||
            !quizQuestionField ||
            !quizQuestionField.value
        ) {
            return;
        }


        answerSaveInProgress =
            true;


        setAutoSaveStatus(
            "Saving...",
            "is-saving"
        );


        try {

            // -----------------------------------------------------
            // CURRENT MCQ OPTION
            // -----------------------------------------------------

            const selectedOption =
                document.querySelector(
                    'input[name="objectiveAnswer"]:checked'
                );


            // -----------------------------------------------------
            // REQUEST BODY
            // -----------------------------------------------------

            const formData =
                new URLSearchParams();


            formData.append(
                "attemptId",
                attemptField.value
            );


            formData.append(
                "quizQuestionId",
                quizQuestionField.value
            );


            if (selectedOption) {

                formData.append(
                    "selectedOptionId",
                    selectedOption.value
                );
            }


            if (subjectiveBox) {

                formData.append(
                    "answerText",
                    subjectiveBox.value
                );
            }


            // -----------------------------------------------------
            // CALL API
            // -----------------------------------------------------

            const response =
                await fetch(
                    '<%= ResolveUrl("~/Api/SaveQuizAnswer.ashx") %>',
                    {
                        method: "POST",

                        credentials:
                            "same-origin",

                        cache:
                            "no-store",

                        headers:
                        {
                            "Content-Type":
                                "application/x-www-form-urlencoded; charset=UTF-8"
                        },

                        body:
                            formData.toString()
                    }
                );


            const data =
                await response.json();


            // -----------------------------------------------------
            // EXPIRED ATTEMPT
            // -----------------------------------------------------

            if (
                response.status === 409
            ) {

                setAutoSaveStatus(
                    "Quiz session expired",
                    "is-error"
                );


                if (
                    typeof syncTimerWithServer ===
                    "function"
                ) {
                    syncTimerWithServer();
                }


                return;
            }


            // -----------------------------------------------------
            // FAILED
            // -----------------------------------------------------

            if (
                !response.ok ||
                !data.success
            ) {

                setAutoSaveStatus(
                    "Auto-save failed",
                    "is-error"
                );


                return;
            }


            // -----------------------------------------------------
            // SUCCESS
            // -----------------------------------------------------

            setAutoSaveStatus(
                "Answer Saved",
                "is-saved"
            );

        }
        catch (error) {

            console.error(
                "Answer auto-save failed:",
                error
            );


            setAutoSaveStatus(
                "Auto-save failed",
                "is-error"
            );
        }
        finally {

            answerSaveInProgress =
                false;


            // If the student changed the answer while
            // the previous save was running, save again.

            if (answerSavePending) {

                answerSavePending =
                    false;


                saveAnswerWithApi();
            }
        }
    }



    /* ---------------------------------------------------------
       MCQ AUTO-SAVE
       Save immediately after selecting an option.
       --------------------------------------------------------- */

    document.addEventListener(
        "change",
        function(event) {

            if (
                event.target.matches(
                    'input[name="objectiveAnswer"]'
                )
            ) {

                saveAnswerWithApi();
            }
        }
    );



    /* ---------------------------------------------------------
       SUBJECTIVE AUTO-SAVE
       Wait 1 second after student stops typing.
       --------------------------------------------------------- */

    if (subjectiveBox) {

        subjectiveBox.addEventListener(
            "input",
            function() {

                setAutoSaveStatus(
                    "",
                    ""
                );


                if (answerSaveTimer) {

                    clearTimeout(
                        answerSaveTimer
                    );
                }


                answerSaveTimer =
                    setTimeout(
                        saveAnswerWithApi,
                        1000
                    );
            }
            );
        }


    /* =========================================================
       TIMER
       ========================================================= */

    const timerElement =
        document.getElementById(
            "quizTimer"
        );


    const remainingSecondsField =
        document.getElementById(
            "hfRemainingSeconds"
        );


    let remainingSeconds =
        remainingSecondsField

            ? parseInt(
                remainingSecondsField.value
                || "0"
            )

            : 0;


    let timerSubmitted =
        false;


    let quizTimerInterval =
        null;


    let timerSyncInterval =
        null;



    // ---------------------------------------------------------
    // DISPLAY TIMER
    // ---------------------------------------------------------

    function renderTimer() {

        // Expired session page does not render
        // the normal quiz timer.
        if (!timerElement) {
            return;
        }


        if (
            remainingSeconds < 0
        ) {
            remainingSeconds =
                0;
        }


        const minutes =
            Math.floor(
                remainingSeconds / 60
            );


        const seconds =
            remainingSeconds % 60;


        timerElement.textContent =
            String(minutes)
                .padStart(2, "0")
            + ":"
            + String(seconds)
                .padStart(2, "0");
    }



    // ---------------------------------------------------------
    // TIMER TICK
    // ---------------------------------------------------------

    function timerTick() {

        if (!timerElement) {
            return;
        }


        if (
            remainingSeconds <= 0
        ) {
            remainingSeconds =
                0;


            renderTimer();


            submitTimeUp();


            return;
        }


        remainingSeconds--;


        renderTimer();


        // Submit immediately when 00:00 is reached
        if (
            remainingSeconds <= 0
        ) {
            remainingSeconds =
                0;


            renderTimer();


            submitTimeUp();
        }
    }



    // ---------------------------------------------------------
    // AUTOMATIC SUBMISSION
    // ---------------------------------------------------------

    function submitTimeUp() {

        if (timerSubmitted) {
            return;
        }


        timerSubmitted =
            true;


        if (quizTimerInterval) {

            clearInterval(
                quizTimerInterval
            );
        }


        if (timerSyncInterval) {

            clearInterval(
                timerSyncInterval
            );
        }


        const timeUpButton =
            document.getElementById(
                "btnTimeUp"
            );


        if (timeUpButton) {

            timeUpButton.click();
            }
        }


    /* =========================================================
       TIMER API SYNCHRONISATION
       ========================================================= */


    async function syncTimerWithServer() {

        const attemptField =
            document.getElementById(
                "hfAttemptId"
            );


        if (
            !attemptField ||
            !attemptField.value ||
            timerSubmitted
        ) {
            return;
        }


        try {

            const response =
                await fetch(
                '<%= ResolveUrl("~/Api/QuizAttemptStatus.ashx") %>'
                + "?attemptId="
                + encodeURIComponent(
                    attemptField.value
                ),
                {
                    method: "GET",
                    credentials: "same-origin",
                    cache: "no-store"
                }
            );


        if (!response.ok) {
            return;
        }


        const data =
            await response.json();


        if (!data.success) {
            return;
        }


        // ---------------------------------------------------------
        // ATTEMPT IS NO LONGER ACTIVE
        // ---------------------------------------------------------

        if (
            data.status !== "InProgress"
        ) {
            return;
        }


        // ---------------------------------------------------------
        // SERVER SAYS TIME HAS EXPIRED
        // ---------------------------------------------------------

        if (
            data.expired ||
            data.remainingSeconds <= 0
        ) {
            remainingSeconds =
                0;


            renderTimer();


            submitTimeUp();


            return;
        }


        // ---------------------------------------------------------
        // SYNCHRONISE LOCAL TIMER
        // ---------------------------------------------------------

        remainingSeconds =
            data.remainingSeconds;


        renderTimer();

        }
        catch (error) {

            console.error(
                "Timer synchronisation failed:",
                error
            );

        }
    }



    /* =========================================================
       START TIMER ONLY WHEN NORMAL QUIZ EXISTS
       ========================================================= */

    if (timerElement) {

        // Show initial timer
        renderTimer();


        // Normal local countdown every 1 second
        quizTimerInterval =
            setInterval(
                timerTick,
                1000
            );


        // Check server immediately
        syncTimerWithServer();


        // Synchronise with server every 30 seconds
        timerSyncInterval =
            setInterval(
                syncTimerWithServer,
                30000
                );
    }

</script>


</body>

</html>