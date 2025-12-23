document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById('deliverySearch');
    const statusFilter = document.getElementById('statusFilter');

    // Select the main table body
    const tableBody = document.querySelector('.table > tbody');

    function filterTable() {
        if (!tableBody) return;

        const searchTerm = searchInput.value.toLowerCase().trim();
        const statusTerm = statusFilter.value;

        // Use children to get direct rows (ignoring nested tables)
        const rows = tableBody.children;

        for (let i = 0; i < rows.length; i++) {
            const row = rows[i];

            // 1. Skip Detail rows during the main loop
            // We handle them when processing their "parent" row
            if (row.querySelector('td[colspan]')) {
                continue;
            }

            // Safety check for row length
            if (row.cells.length < 5) continue;

            // --- SEARCH MATCHING ---
            const orderRef = row.cells[1].innerText.toLowerCase();
            const customer = row.cells[2].innerText.toLowerCase();
            const tracking = row.cells[3].innerText.toLowerCase();

            const matchesSearch =
                orderRef.includes(searchTerm) ||
                customer.includes(searchTerm) ||
                tracking.includes(searchTerm);

            // --- FILTER MATCHING ---
            const statusSelect = row.cells[4].querySelector('select');
            const currentStatus = statusSelect ? statusSelect.value : row.cells[4].innerText.trim();
            const matchesFilter = statusTerm === "" || currentStatus === statusTerm;

            // --- VISIBILITY LOGIC ---
            // Identify the next row (which contains the collapsible items)
            const detailRow = row.nextElementSibling;
            const isDetailRow = detailRow && detailRow.querySelector('td[colspan]');

            if (matchesSearch && matchesFilter) {
                // SHOW: Match found
                row.style.display = "";

                // CRITICAL FIX: We must also un-hide the detail row wrapper
                // (Bootstrap handles the inner div, but we control the row)
                if (isDetailRow) {
                    detailRow.style.display = "";
                }
            } else {
                // HIDE: No match
                row.style.display = "none";

                // Force hide the detail row so it doesn't hang around
                if (isDetailRow) {
                    detailRow.style.display = "none";
                }
            }
        }
    }
    
    
 
    if (searchInput) searchInput.addEventListener('keyup', filterTable);
    if (statusFilter) statusFilter.addEventListener('change', filterTable);
});