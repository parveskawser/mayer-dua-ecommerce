document.addEventListener('DOMContentLoaded', function () {
    // Global Date Converter
    // Finds any element with class 'utc-date' and converts its 'data-utc' value to local time
    function convertUtcDates() {
        $(".utc-date").each(function () {
            var $this = $(this);
            // Skip if already processed (optional optimization)
            if ($this.data("converted")) return;

            var utcTime = $this.data("utc");
            if (utcTime) {
                var localDate = new Date(utcTime);
                if (!isNaN(localDate.getTime())) {
                    // Customize format as needed
                    var formatted = localDate.toLocaleDateString(undefined, {
                        day: 'numeric', month: 'short', year: 'numeric'
                    }) + ", " + localDate.toLocaleTimeString(undefined, {
                        hour: '2-digit', minute: '2-digit'
                    });

                    $this.text(formatted);
                    $this.data("converted", true); // Mark as done
                }
            }
        });
    }

    // Run on page load
    convertUtcDates();

    // Re-run whenever an AJAX request completes (for Search results, Partials, etc.)
    $(document).ajaxComplete(function () {
        convertUtcDates();
    });
});