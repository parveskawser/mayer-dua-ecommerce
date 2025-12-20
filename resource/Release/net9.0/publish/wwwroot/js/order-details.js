document.addEventListener('DOMContentLoaded', function () {
    var detailsModal = document.getElementById('detailsModal');

    if (detailsModal) {
        document.body.appendChild(detailsModal);
    }

    if (detailsModal) {
        detailsModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            if (!button) return;

            // 1. Standard Text Helper
            function setText(id, val) {
                var el = document.getElementById(id);
                if (el) {
                    el.textContent = val ? val : '';
                }
            }

            // 2. ✅ NEW: UTC Date Helper (Converts UTC string to Local Time)
            function setUtcText(id, utcVal) {
                var el = document.getElementById(id);
                if (el) {
                    if (!utcVal) {
                        el.textContent = '';
                        return;
                    }

                    // Create Date object (Browser automatically handles Timezone)
                    var date = new Date(utcVal);

                    if (!isNaN(date.getTime())) {
                        // Format: "Dec 17, 2025, 10:30 PM"
                        el.textContent = date.toLocaleDateString(undefined, {
                            day: 'numeric', month: 'short', year: 'numeric'
                        }) + ", " + date.toLocaleTimeString(undefined, {
                            hour: '2-digit', minute: '2-digit'
                        });
                    } else {
                        // Fallback if parsing fails
                        el.textContent = utcVal;
                    }
                }
            }

            // --- Fill Data ---
            setText('m-id', button.getAttribute('data-id'));
            setText('m-status', button.getAttribute('data-status'));
            setText('m-type', button.getAttribute('data-type'));

            // Customer Logic
            var custId = button.getAttribute('data-cust-id');
            var custName = button.getAttribute('data-cust-name');
            var custPhone = button.getAttribute('data-cust-phone');
            var custDisplay = custId;
            if (custName && custName !== 'Unknown') {
                custDisplay += ' (' + custName + ')';
            }
            setText('m-cust-id', custDisplay);
            setText('m-cust-phone', custPhone);

            // Address Logic
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
            setText('m-updated-by', button.getAttribute('data-updated-by'));

            // ✅ USE THE NEW DATE HELPER HERE
            setText(
                'm-created-at',
                formatUtcToLocal(button.getAttribute('data-created-at'))
            );

            setText(
                'm-updated-at',
                formatUtcToLocal(button.getAttribute('data-updated-at'))
            );


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
function formatUtcToLocal(utcString) {
    if (!utcString) return 'N/A';

    const date = new Date(utcString);
    if (isNaN(date)) return 'N/A';

    return date.toLocaleString(undefined, {
        year: 'numeric',
        month: 'short',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
    });
}
