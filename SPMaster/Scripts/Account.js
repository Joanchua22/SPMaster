function settingsTogglePassword(inputId, buttonElement) {

    var input =
        document.getElementById(inputId);

    var icon =
        buttonElement.querySelector("i");


    if (!input) {
        return;
    }


    var currentType =
        input.getAttribute("type");


    if (currentType === "password") {

        input.setAttribute(
            "type",
            "text"
        );


        if (icon) {
            icon.className =
                "bi bi-eye-slash";
        }

    }
    else {

        input.setAttribute(
            "type",
            "password"
        );


        if (icon) {
            icon.className =
                "bi bi-eye";
        }

    }

}


function autoHideSuccessPopup() {

    var popup =
        document.querySelector(
            ".success-popup"
        );


    if (!popup) {
        return;
    }


    setTimeout(
        function () {

            popup.style.opacity =
                "0";


            setTimeout(
                function () {

                    popup.style.display =
                        "none";

                },
                300
            );

        },
        3000
    );

}


window.addEventListener(
    "load",
    function () {

        autoHideSuccessPopup();

    }
);