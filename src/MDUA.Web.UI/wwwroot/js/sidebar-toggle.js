document.addEventListener("DOMContentLoaded", function () {
    const toggleBtn = document.getElementById("sidebar-toggle-btn");
    const sidebar = document.getElementById("admin-sidebar");

    if (toggleBtn && sidebar) {
        toggleBtn.addEventListener("click", function () {
            // 'toggled' class handles both states via CSS:
            // On Desktop: Collapses sidebar (width 0)
            // On Mobile: Slides sidebar in (transform 0)
            sidebar.classList.toggle("toggled");
        });
    }

    // Close sidebar when clicking outside on mobile
    document.addEventListener("click", function (event) {
        // Only apply close logic on mobile screens
        if (window.innerWidth < 768) {
            const isClickInside = sidebar.contains(event.target) || toggleBtn.contains(event.target);

            // If clicked outside AND sidebar is currently open (toggled), close it
            if (!isClickInside && sidebar.classList.contains("toggled")) {
                sidebar.classList.remove("toggled");
            }
        }
    });
});