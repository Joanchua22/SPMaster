(function () {

    "use strict";


    document.addEventListener(
        "DOMContentLoaded",
        function () {

            // =====================================================
            // ELEMENTS
            // =====================================================

            var modal =
                document.getElementById(
                    "studentJoinModal"
                );


            var openButton =
                document.getElementById(
                    "btnOpenJoinModal"
                );


            var closeButton =
                document.getElementById(
                    "btnCloseJoinModal"
                );


            var cancelButton =
                document.getElementById(
                    "btnCancelJoin"
                );


            var overlay =
                document.getElementById(
                    "studentJoinOverlay"
                );


            var reopenField =
                document.getElementById(
                    "hfOpenJoinModal"
                );


            var joinCodeInput =
                document.querySelector(
                    ".join-code-input"
                );



            // =====================================================
            // OPEN MODAL
            // =====================================================

            function openJoinModal() {

                if (!modal) {
                    return;
                }


                modal.hidden =
                    false;


                document.body.classList.add(
                    "student-join-open"
                );


                if (joinCodeInput) {

                    setTimeout(
                        function () {

                            joinCodeInput.focus();

                        },
                        100
                    );
                }
            }



            // =====================================================
            // CLOSE MODAL
            // =====================================================

            function closeJoinModal() {

                if (!modal) {
                    return;
                }


                modal.hidden =
                    true;


                document.body.classList.remove(
                    "student-join-open"
                );
            }



            // =====================================================
            // OPEN BUTTON
            // =====================================================

            if (openButton) {

                openButton.addEventListener(
                    "click",
                    function () {

                        openJoinModal();

                    }
                );
            }



            // =====================================================
            // CLOSE BUTTON
            // =====================================================

            if (closeButton) {

                closeButton.addEventListener(
                    "click",
                    function () {

                        closeJoinModal();

                    }
                );
            }



            // =====================================================
            // CANCEL BUTTON
            // =====================================================

            if (cancelButton) {

                cancelButton.addEventListener(
                    "click",
                    function () {

                        closeJoinModal();

                    }
                );
            }



            // =====================================================
            // CLICK OUTSIDE
            // =====================================================

            if (overlay) {

                overlay.addEventListener(
                    "click",
                    function () {

                        closeJoinModal();

                    }
                );
            }



            // =====================================================
            // ESCAPE KEY
            // =====================================================

            document.addEventListener(
                "keydown",
                function (event) {

                    if (
                        event.key === "Escape"
                        &&
                        modal
                        &&
                        !modal.hidden
                    ) {
                        closeJoinModal();
                    }
                }
            );



            // =====================================================
            // REOPEN AFTER ASP.NET POSTBACK
            //
            // Used when:
            // - join code is invalid
            // - session is closed
            // - student already completed session
            // - valid session has been found
            // =====================================================

            if (
                reopenField
                &&
                reopenField.value === "1"
            ) {
                openJoinModal();
            }



            // =====================================================
            // JOIN CODE FORMATTING
            //
            // Allows:
            // spm-1234
            // SPM-1234
            //
            // Automatically converts it to uppercase.
            // =====================================================

            if (joinCodeInput) {

                joinCodeInput.addEventListener(
                    "input",
                    function () {

                        var value =
                            joinCodeInput.value
                                .toUpperCase();


                        // Remove spaces
                        value =
                            value.replace(
                                /\s+/g,
                                ""
                            );


                        joinCodeInput.value =
                            value;
                    }
                );
            }

        }
    );

})();