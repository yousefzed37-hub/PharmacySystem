// =====================================================
// PHARMACARE MS — SIDEBAR TOGGLE SCRIPT
// =====================================================
(function () {
    "use strict";

    const sidebar = document.getElementById("sidebar");
    const mainWrapper = document.getElementById("mainWrapper");
    const sidebarToggle = document.getElementById("sidebarToggle");
    const sidebarClose = document.getElementById("sidebarClose");
    const sidebarOverlay = document.getElementById("sidebarOverlay");

    const MOBILE_BREAKPOINT = 992;
    const STORAGE_KEY = "pharmacare_sidebar_collapsed";

    function isMobile() {
        return window.innerWidth < MOBILE_BREAKPOINT;
    }

    // Restore collapsed state on desktop load
    function restoreSidebarState() {
        if (!isMobile()) {
            const collapsed = localStorage.getItem(STORAGE_KEY) === "true";
            if (collapsed) {
                sidebar.classList.add("collapsed");
                mainWrapper.classList.add("expanded");
            }
        }
    }

    // Toggle behaviour differs for desktop (collapse) vs mobile (slide-in)
    function handleToggleClick() {
        if (isMobile()) {
            sidebar.classList.add("mobile-open");
            sidebarOverlay.classList.add("active");
            document.body.style.overflow = "hidden";
        } else {
            sidebar.classList.toggle("collapsed");
            mainWrapper.classList.toggle("expanded");
            localStorage.setItem(STORAGE_KEY, sidebar.classList.contains("collapsed"));
        }
    }

    function closeMobileSidebar() {
        sidebar.classList.remove("mobile-open");
        sidebarOverlay.classList.remove("active");
        document.body.style.overflow = "";
    }

    // Event bindings
    if (sidebarToggle) {
        sidebarToggle.addEventListener("click", handleToggleClick);
    }

    if (sidebarClose) {
        sidebarClose.addEventListener("click", closeMobileSidebar);
    }

    if (sidebarOverlay) {
        sidebarOverlay.addEventListener("click", closeMobileSidebar);
    }

    // Reset inline styles / classes gracefully on resize
    window.addEventListener("resize", function () {
        if (!isMobile()) {
            closeMobileSidebar();
        }
    });

    // Highlight active nav link based on current path (extra polish)
    function setActiveLink() {
        const links = document.querySelectorAll(".sidebar-nav .nav-link");
        const currentPath = window.location.pathname.toLowerCase();

        links.forEach(function (link) {
            link.classList.remove("active");
            const href = link.getAttribute("href");
            if (href && href !== "#" && currentPath.startsWith(href.toLowerCase()) && href.toLowerCase() !== "/") {
                link.classList.add("active");
            }
        });
    }

    document.addEventListener("DOMContentLoaded", function () {
        restoreSidebarState();
        setActiveLink();
    });
})();



document.addEventListener('DOMContentLoaded', function () {
    // تحديد جميع خانات الأرقام والأسعار
    const numberInputs = document.querySelectorAll('input[type="number"]');

    numberInputs.forEach(input => {
        // أول ما تضغط جوه الخانة
        input.addEventListener('focus', function () {
            if (this.value === '0' || this.value === '0.00' || this.value === '0.0') {
                this.value = '';
            }
        });

        // لو خرجت منها وهي فاضية (ترجع 0 لو تحب أو تسيبها فاضية)
        input.addEventListener('blur', function () {
            if (this.value.trim() === '') {
                // تقدر تسيبها فاضية عشان ما تضايقش المستخدم
            }
        });
    });
});