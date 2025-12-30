// --- ROW SELECTION LOGIC ---
document.addEventListener("DOMContentLoaded", function () {
    // 1. Handle "Check All" in table header
    const checkAll = document.getElementById('checkAllRows');
    if (checkAll) {
        checkAll.addEventListener('change', function () {
            document.querySelectorAll('.row-checkbox').forEach(cb => cb.checked = this.checked);
            updateSelectedCount();
        });
    }

    // 2. Handle individual row click (update count)
    document.querySelectorAll('.row-checkbox').forEach(cb => {
        cb.addEventListener('change', updateSelectedCount);
    });
});

function updateSelectedCount() {
    const count = document.querySelectorAll('.row-checkbox:checked').length;
    const badge = document.getElementById('selectedCountDisplay');
    if (badge) badge.innerText = count;

    // Auto-select "Selected Rows" in dropdown if > 0
    const scopeSelect = document.getElementById('exportScope');
    if (count > 0 && scopeSelect) scopeSelect.value = 'selected';
}

// --- SUBMIT EXPORT ---
function submitExport() {
    // 1. Collect Options
    const form = document.getElementById('exportForm');
    const formData = new FormData(form);
    const scope = formData.get('scope'); // all, filtered, selected

    // 2. Collect IDs if "Selected" scope is chosen
    let selectedIds = [];
    if (scope === 'selected') {
        document.querySelectorAll('.row-checkbox:checked').forEach(cb => selectedIds.push(cb.value));
        if (selectedIds.length === 0) {
            Swal.fire('Warning', 'No rows selected!', 'warning');
            return;
        }
    }

    // 3. Collect Current Filters (from the main page URL or hidden form)
    // We get the current URL search params to preserve filters (Date, Status, Search, etc.)
    const currentParams = new URLSearchParams(window.location.search);

    // 4. Build Payload
    const payload = {
        format: formData.get('format'),
        columns: [],
        scope: scope,
        selectedIds: selectedIds,
        // Pass existing filters
        status: currentParams.get('status') || 'all',
        dateRange: currentParams.get('dateRange') || 'all',
        search: currentParams.get('search') || '',
        // ... add other filters like minAmount, maxAmount etc ...
        entityType: 'Order' // ✅ Validates reuse for other lists
    };

    // Collect checked columns
    document.querySelectorAll('.col-check:checked').forEach(c => payload.columns.push(c.value));

    // 5. Send Request (Use a hidden form POST to trigger download)
    downloadFile('/Report/ExportData', payload);
}

function downloadFile(url, payload) {
    // Create a temporary form to POST data and trigger browser download
    var form = document.createElement("form");
    form.method = "POST";
    form.action = url;
    form.target = "_blank"; // Open in new tab or download

    // Add JSON payload as a hidden field
    var input = document.createElement("input");
    input.name = "jsonPayload";
    input.value = JSON.stringify(payload);
    form.appendChild(input);

    document.body.appendChild(form);
    form.submit();
    document.body.removeChild(form);

    // Close modal
    var modalEl = document.getElementById('exportModal');
    var modal = bootstrap.Modal.getInstance(modalEl);
    modal.hide();
}