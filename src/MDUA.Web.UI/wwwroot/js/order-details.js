//change


document.addEventListener('DOMContentLoaded', function () {
    var detailsModal = document.getElementById('detailsModal');

    // Move the modal HTML out of the nested layout and append it directly 
    // to the body. This ensures the backdrop (dark overlay) appears *behind* the modal.
    if (detailsModal) {
        document.body.appendChild(detailsModal);
    }

    if (detailsModal) {
        detailsModal.addEventListener('show.bs.modal', function (event) {
            // Button that triggered the modal
            var button = event.relatedTarget;
            if (!button) return;

            // Helper function to set text content
            function setText(id, val) {
                var el = document.getElementById(id);
                if (el) {
                    el.textContent = val ? val : '';
                }
            }

            // --- Fill Data ---
            // General
            setText('m-id', button.getAttribute('data-id'));
            setText('m-status', button.getAttribute('data-status'));
            setText('m-type', button.getAttribute('data-type'));
            setText('m-cust-id', button.getAttribute('data-cust-id'));
            setText('m-addr-id', button.getAttribute('data-addr-id'));

            // Financials
            setText('m-total', button.getAttribute('data-total'));
            setText('m-discount', button.getAttribute('data-discount'));
            setText('m-net', button.getAttribute('data-net'));

            // Technical
            setText('m-ip', button.getAttribute('data-ip'));
            setText('m-session', button.getAttribute('data-session'));

            // Audit
            setText('m-created-by', button.getAttribute('data-created-by'));
            setText('m-created-at', button.getAttribute('data-created-at'));
            setText('m-updated-by', button.getAttribute('data-updated-by'));
            setText('m-updated-at', button.getAttribute('data-updated-at'));

            // ✅ NEW: Apply classes directly to the status element (not parent)
            var status = button.getAttribute('data-status');
            var statusEl = document.getElementById('m-status');

            if (statusEl) {
                // Remove all previous status classes
                statusEl.classList.remove('bg-success', 'bg-warning', 'bg-secondary-subtle', 'text-white', 'text-dark');

                // Add appropriate class based on status
                if (status === 'Confirmed') {
                    statusEl.classList.add('bg-success');
                } else if (status === 'Pending') {
                    statusEl.classList.add('bg-warning');
                } else {
                    statusEl.classList.add('bg-secondary-subtle');
                }
            }
        });
    }
});