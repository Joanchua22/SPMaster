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
                "added"
            )
        ) {
            url.searchParams.delete(
                "added"
            );

            changed = true;
        }


        if (
            url.searchParams.has(
                "updated"
            )
        ) {
            url.searchParams.delete(
                "updated"
            );

            changed = true;
        }


        if (
            url.searchParams.has(
                "statuschanged"
            )
        ) {
            url.searchParams.delete(
                "statuschanged"
            );

            changed = true;
        }


        if (
            url.searchParams.has(
                "deleted"
            )
        ) {
            url.searchParams.delete(
                "deleted"
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