function loginTogglePassword(passwordId) {

    var password =
        document.getElementById(passwordId);

    var eyeIcon =
        document.getElementById("eyeIcon");


    if (!password) {
        return;
    }


    var currentType =
        password.getAttribute("type");


    if (currentType === "password") {

        password.setAttribute(
            "type",
            "text"
        );


        if (eyeIcon) {
            eyeIcon.textContent =
                "⊘";
        }

    }
    else {

        password.setAttribute(
            "type",
            "password"
        );


        if (eyeIcon) {
            eyeIcon.textContent =
                "◉";
        }

    }

}