function closeSuccessPopup() {

    var popup =
        document.querySelector(
            ".success-popup"
        );

    if (popup) {
        popup.style.display = "none";
    }
}


window.addEventListener(
    "load",
    function () {

        var popup =
            document.querySelector(
                ".success-popup"
            );


        var url =
            new URL(
                window.location.href
            );


        var changed =
            false;


        if (
            url.searchParams.has(
                "responded"
            )
        ) {
            url.searchParams.delete(
                "responded"
            );

            changed = true;
        }


        if (
            url.searchParams.has(
                "statusupdated"
            )
        ) {
            url.searchParams.delete(
                "statusupdated"
            );

            changed = true;
        }


        if (changed) {

            window.history.replaceState(
                {},
                document.title,
                url.pathname +
                url.search +
                url.hash
            );

        }


        if (popup) {

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

    }
);