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

function previewProfilePhoto(input) {

    var profileImage =
        document.querySelector(
            ".profile-avatar-image"
        );

    var profileInitials =
        document.querySelector(
            ".profile-avatar-initials"
        );


    if (
        !input
        ||
        !input.files
        ||
        input.files.length === 0
    ) {
        return;
    }


    var file =
        input.files[0];


    var allowedTypes =
        [
            "image/jpeg",
            "image/png",
            "image/webp"
        ];


    if (
        allowedTypes.indexOf(
            file.type
        ) === -1
    ) {
        alert(
            "Please select a JPG, PNG or WEBP image."
        );

        input.value =
            "";

        return;
    }


    if (
        file.size >
        2 * 1024 * 1024
    ) {
        alert(
            "Profile image must be smaller than 2 MB."
        );

        input.value =
            "";

        return;
    }


    var reader =
        new FileReader();


    reader.onload =
        function (event) {

            profileImage.src =
                event.target.result;


            profileImage.style.display =
                "block";


            profileInitials.style.display =
                "none";
        };


    reader.readAsDataURL(
        file
    );
}

window.addEventListener(
    "load",
    function () {

        autoHideSuccessPopup();

    }
);