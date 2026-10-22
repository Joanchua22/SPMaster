(function () {

    "use strict";


    document.addEventListener(
        "DOMContentLoaded",
        function () {

            var modal =
                document.getElementById(
                    "feedbackSubmitModal"
                );

            var openButton =
                document.getElementById(
                    "btnOpenFeedbackModal"
                );

            var closeButton =
                document.getElementById(
                    "btnCloseFeedbackModal"
                );

            var cancelButton =
                document.getElementById(
                    "btnCancelFeedback"
                );

            var overlay =
                document.getElementById(
                    "feedbackModalOverlay"
                );

            var reopenField =
                document.getElementById(
                    "hfOpenFeedbackModal"
                );


            function openModal() {

                if (!modal) {
                    return;
                }

                modal.hidden = false;

                document.body.classList.add(
                    "feedback-modal-open"
                );
            }


            function closeModal() {

                if (!modal) {
                    return;
                }

                modal.hidden = true;

                document.body.classList.remove(
                    "feedback-modal-open"
                );
            }


            if (openButton) {

                openButton.addEventListener(
                    "click",
                    openModal
                );
            }


            if (closeButton) {

                closeButton.addEventListener(
                    "click",
                    closeModal
                );
            }


            if (cancelButton) {

                cancelButton.addEventListener(
                    "click",
                    closeModal
                );
            }


            if (overlay) {

                overlay.addEventListener(
                    "click",
                    closeModal
                );
            }


            document.addEventListener(
                "keydown",
                function (event) {

                    if (event.key === "Escape") {
                        closeModal();
                    }
                }
            );


            // Reopen after server validation/error
            if (
                reopenField &&
                reopenField.value === "1"
            ) {
                openModal();
            }

        }
    );

})();