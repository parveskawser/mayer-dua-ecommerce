document.addEventListener('DOMContentLoaded', function () {
    var detailsModal = document.getElementById('detailsModal');

    if (detailsModal) {
        document.body.appendChild(detailsModal);
    }

    if (detailsModal) {
        detailsModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            if (!button) return;

            function setText(id, val) {
                var el = document.getElementById(id);
                if (el) {
                    el.textContent = val ? val : '';
                }
            }

            // --- Fill Data ---
            setText('m-id', button.getAttribute('data-id'));
            setText('m-status', button.getAttribute('data-status'));
            setText('m-type', button.getAttribute('data-type'));

            // ✅ CUSTOMER: ID + (Name)
            var custId = button.getAttribute('data-cust-id');
            var custName = button.getAttribute('data-cust-name');
            var custDisplay = custId;
            if (custName && custName !== 'Unknown') {
                custDisplay += ' (' + custName + ')';
            }
            setText('m-cust-id', custDisplay);

            // ✅ ADDRESS: ID + (Full Address)
            var addrId = button.getAttribute('data-addr-id');
            var fullAddr = button.getAttribute('data-full-addr');
            var addrDisplay = addrId;
            if (fullAddr) {
                addrDisplay += ' (' + fullAddr + ')';
            }
            setText('m-addr-id', addrDisplay);

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

            // Status Badge Logic
            var status = button.getAttribute('data-status');
            var statusEl = document.getElementById('m-status');

            if (statusEl) {
                statusEl.classList.remove('bg-success', 'bg-warning', 'bg-secondary-subtle', 'text-white', 'text-dark');
                if (status === 'Confirmed') {
                    statusEl.classList.add('bg-success', 'text-white');
                } else if (status === 'Pending') {
                    statusEl.classList.add('bg-warning', 'text-dark');
                } else {
                    statusEl.classList.add('bg-secondary-subtle', 'text-dark');
                }
            }
        });
    }
});