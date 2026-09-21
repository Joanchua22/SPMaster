(function () {
    "use strict";
    const toggle = document.getElementById("sidebarToggle");
    const sidebar = document.getElementById("dashboardSidebar");
    if (!toggle || !sidebar) return;
    const mobile = window.matchMedia("(max-width: 1000px)");

    function setOpen(open, restoreFocus) {
        sidebar.classList.toggle("is-open", open);
        toggle.setAttribute("aria-expanded", String(open));
        toggle.setAttribute("aria-label", open ? "Close navigation" : "Open navigation");
        toggle.textContent = open ? "✕" : "☰";
        if (restoreFocus) toggle.focus();
    }
    toggle.addEventListener("click", function () {
        setOpen(toggle.getAttribute("aria-expanded") !== "true", false);
    });
    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape" && sidebar.classList.contains("is-open")) setOpen(false, true);
    });
    document.addEventListener("click", function (event) {
        if (mobile.matches && !sidebar.contains(event.target) && !toggle.contains(event.target)) {
            setOpen(false, sidebar.contains(document.activeElement));
        }
    });
    sidebar.querySelectorAll("a").forEach(function (link) {
        link.addEventListener("click", function () { if (mobile.matches) setOpen(false, true); });
    });
    mobile.addEventListener("change", function () {
        const sidebarFocused = sidebar.contains(document.activeElement);
        if (!mobile.matches && document.activeElement === toggle) sidebar.querySelector("a").focus();
        setOpen(false, mobile.matches && sidebarFocused);
    });
})();
