/* =========================================================
   QUESTION PREVIEW
   Shared by:
   - CreateQuestion.aspx
   - QuestionBank.aspx
   ========================================================= */

window.QuestionPreview = (function () {


    /* =====================================================
       GET PREVIEW ELEMENTS
       ===================================================== */

    function getElements() {

        return {

            modal:
                document.getElementById(
                    "questionPreviewModal"
                ),

            subject:
                document.getElementById(
                    "previewSubject"
                ),

            difficulty:
                document.getElementById(
                    "previewDifficulty"
                ),

            type:
                document.getElementById(
                    "previewType"
                ),

            questionText:
                document.getElementById(
                    "previewQuestionText"
                ),

            imageArea:
                document.getElementById(
                    "previewImageArea"
                ),

            image:
                document.getElementById(
                    "previewImage"
                ),

            mcqAnswers:
                document.getElementById(
                    "previewMcqAnswers"
                ),

            subjectiveAnswer:
                document.getElementById(
                    "previewSubjectiveAnswer"
                ),

            modelAnswer:
                document.getElementById(
                    "previewModelAnswer"
                ),

            optionA:
                document.getElementById(
                    "previewOptionA"
                ),

            optionB:
                document.getElementById(
                    "previewOptionB"
                ),

            optionC:
                document.getElementById(
                    "previewOptionC"
                ),

            optionD:
                document.getElementById(
                    "previewOptionD"
                )

        };

    }



    /* =====================================================
       RESET CORRECT ANSWER HIGHLIGHT
       ===================================================== */

    function resetCorrectAnswer() {

        const previewAnswers =
            document.querySelectorAll(
                ".preview-answer"
            );


        previewAnswers.forEach(
            function (answer) {

                answer.classList.remove(
                    "correct-preview-answer"
                );

            }
        );

    }



    /* =====================================================
       HIGHLIGHT CORRECT ANSWER
       ===================================================== */

    function highlightCorrectAnswer(
        correctOption
    ) {

        resetCorrectAnswer();


        if (!correctOption) {
            return;
        }


        const correctAnswer =
            document.querySelector(
                '.preview-answer[data-preview-option="' +
                correctOption +
                '"]'
            );


        if (correctAnswer) {

            correctAnswer.classList.add(
                "correct-preview-answer"
            );

        }

    }



    /* =====================================================
       DISPLAY IMAGE
       ===================================================== */

    function displayImage(
        elements,
        imageUrl
    ) {

        if (imageUrl) {

            elements.image.src =
                imageUrl;

            elements.imageArea.hidden =
                false;

        } else {

            elements.image.removeAttribute(
                "src"
            );

            elements.imageArea.hidden =
                true;

        }

    }



    /* =====================================================
       DISPLAY MCQ QUESTION
       ===================================================== */

    function displayMCQ(
        elements,
        data
    ) {

        elements.mcqAnswers.hidden =
            false;

        elements.subjectiveAnswer.hidden =
            true;


        const options =
            data.options || [];


        elements.optionA.textContent =
            options[0] ||
            "No answer entered";

        elements.optionB.textContent =
            options[1] ||
            "No answer entered";

        elements.optionC.textContent =
            options[2] ||
            "No answer entered";

        elements.optionD.textContent =
            options[3] ||
            "No answer entered";


        highlightCorrectAnswer(
            data.correctOption
        );

    }



    /* =====================================================
       DISPLAY SUBJECTIVE QUESTION
       ===================================================== */

    function displaySubjective(
        elements,
        data
    ) {

        elements.mcqAnswers.hidden =
            true;

        elements.subjectiveAnswer.hidden =
            false;


        resetCorrectAnswer();


        elements.modelAnswer.textContent =
            data.modelAnswer ||
            "No model answer entered yet.";

    }

    function getSubjectClass(subject) {

        return "preview-subject";
    }


    function getDifficultyClass(difficulty) {

        const value =
            (difficulty || "")
                .toLowerCase();


        switch (value) {

            case "easy":
                return "preview-difficulty-easy";

            case "hard":
                return "preview-difficulty-hard";

            default:
                return "preview-difficulty-medium";
        }
    }


    function getTypeClass() {

        return "preview-type";
    }



    /* =====================================================
       OPEN PREVIEW
       ===================================================== */

    function open(data) {

        const elements =
            getElements();


        if (!elements.modal) {

            console.warn(
                "Question preview modal was not found."
            );

            return;
        }


        /* -------------------------
           Question information
           ------------------------- */

        elements.subject.textContent =
            data.subject ||
            "No Subject";

        elements.difficulty.textContent =
            data.difficulty ||
            "No Difficulty";

        elements.type.textContent =
            data.type ||
            "Question";

        elements.questionText.textContent =
            data.questionText ||
            "No question entered yet.";

        /* =========================================
           TAG COLORS
           ========================================= */

        elements.subject.className =
            "preview-tag " +
            getSubjectClass(
                data.subject
            );


        elements.difficulty.className =
            "preview-tag " +
            getDifficultyClass(
                data.difficulty
            );


        elements.type.className =
            "preview-tag " +
            getTypeClass();


        /* -------------------------
           Image
           ------------------------- */

        displayImage(
            elements,
            data.imageUrl
        );



        /* -------------------------
           Question type
           ------------------------- */

        const isMCQ =
            data.type ===
            "Single Choice (MCQ)" ||
            data.type ===
            "MCQ" ||
            data.type ===
            "Single Choice";


        if (isMCQ) {

            displayMCQ(
                elements,
                data
            );

        } else {

            displaySubjective(
                elements,
                data
            );

        }



        /* -------------------------
           Display modal
           ------------------------- */

        elements.modal.hidden =
            false;


        document.body.style.overflow =
            "hidden";


        const closeButton =
            elements.modal.querySelector(
                ".preview-close-button"
            );


        if (closeButton) {

            closeButton.focus();

        }

    }



    /* =====================================================
       CLOSE PREVIEW
       ===================================================== */

    function close() {

        const elements =
            getElements();


        if (!elements.modal) {
            return;
        }


        elements.modal.hidden =
            true;


        document.body.style.overflow =
            "";


        resetCorrectAnswer();

    }



    /* =====================================================
       RETURN PUBLIC FUNCTIONS
       ===================================================== */

    return {

        open: open,

        close: close

    };

})();



/* =========================================================
   CLOSE BUTTONS / OVERLAY
   ========================================================= */

document.addEventListener(
    "click",
    function (event) {

        const closeElement =
            event.target.closest(
                "[data-preview-close]"
            );


        if (closeElement) {

            QuestionPreview.close();

        }

    }
);



/* =========================================================
   ESCAPE KEY
   ========================================================= */

document.addEventListener(
    "keydown",
    function (event) {

        if (event.key !== "Escape") {
            return;
        }


        const modal =
            document.getElementById(
                "questionPreviewModal"
            );


        if (
            modal &&
            !modal.hidden
        ) {

            QuestionPreview.close();

        }

    }
);



/* =========================================================
   QUESTION BANK PREVIEW BUTTONS
   ========================================================= */

document.addEventListener(
    "click",
    function (event) {

        const button =
            event.target.closest(
                "[data-preview-question]"
            );


        if (!button) {
            return;
        }


        const type =
            button.dataset.type || "";


        let options = [];


        if (
            type === "MCQ" ||
            type === "Single Choice" ||
            type === "Single Choice (MCQ)"
        ) {

            options = [

                button.dataset.optionA || "",

                button.dataset.optionB || "",

                button.dataset.optionC || "",

                button.dataset.optionD || ""

            ];

        }


        QuestionPreview.open({

            subject:
                button.dataset.subject || "",

            difficulty:
                button.dataset.difficulty || "",

            type:
                type === "MCQ"
                    ? "Single Choice"
                    : type,

            questionText:
                button.dataset.question || "",

            options:
                options,

            correctOption:
                button.dataset.correctOption || "",

            modelAnswer:
                button.dataset.modelAnswer || "",

            imageUrl:
                button.dataset.imageUrl || ""

        });

    }
);