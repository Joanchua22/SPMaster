(function () {

    "use strict";


    document.addEventListener(
        "DOMContentLoaded",
        function () {

            var modal =
                document.getElementById(
                    "hostSessionModal"
                );


            var openButton =
                document.getElementById(
                    "btnOpenSessionModal"
                );


            var closeButton =
                document.getElementById(
                    "btnCloseSessionModal"
                );


            var cancelButton =
                document.getElementById(
                    "btnCancelSession"
                );


            var overlay =
                document.getElementById(
                    "sessionModalOverlay"
                );


            var keepOpenField =
                document.getElementById(
                    "hfOpenSessionModal"
                );



            // =============================================
            // OPEN MODAL
            // =============================================

            function openModal() {

                if (!modal) {
                    return;
                }


                modal.hidden =
                    false;


                document.body.classList.add(
                    "session-modal-open"
                );
            }



            // =============================================
            // CLOSE MODAL
            // =============================================

            function closeModal() {

                if (!modal) {
                    return;
                }


                modal.hidden =
                    true;


                document.body.classList.remove(
                    "session-modal-open"
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



            // =============================================
            // ESC CLOSE
            // =============================================

            document.addEventListener(
                "keydown",
                function (event) {

                    if (
                        event.key === "Escape"
                    ) {
                        closeModal();
                    }
                }
            );



            // =============================================
            // REOPEN AFTER SERVER POSTBACK
            //
            // Used when:
            // - Regenerate is clicked
            // - validation fails
            // - database error occurs
            // =============================================

            if (
                keepOpenField
                &&
                keepOpenField.value === "1"
            ) {
                openModal();
            }



            // =============================================
            // COPY SESSION CODE
            // =============================================

            var copyButtons =
                document.querySelectorAll(
                    ".copy-code-button"
                );


            copyButtons.forEach(
                function (button) {

                    button.addEventListener(
                        "click",
                        function () {

                            var code =
                                button.getAttribute(
                                    "data-code"
                                );


                            if (!code) {
                                return;
                            }


                            if (
                                navigator.clipboard
                                &&
                                navigator.clipboard.writeText
                            ) {

                                navigator.clipboard
                                    .writeText(code)
                                    .then(
                                        function () {

                                            showCopied(
                                                button
                                            );
                                        }
                                    );
                            }
                            else {

                                fallbackCopy(
                                    code
                                );


                                showCopied(
                                    button
                                );
                            }
                        }
                    );
                }
            );



            function showCopied(
                button) {

                var original =
                    button.innerHTML;


                button.innerHTML =
                    '<i class="bi bi-check2"></i> Copied';


                setTimeout(
                    function () {

                        button.innerHTML =
                            original;
                    },
                    1500
                );
            }



            function fallbackCopy(
                text) {

                var textarea =
                    document.createElement(
                        "textarea"
                    );


                textarea.value =
                    text;


                document.body.appendChild(
                    textarea
                );


                textarea.select();


                document.execCommand(
                    "copy"
                );


                document.body.removeChild(
                    textarea
                );
            }

        }
    );

})();