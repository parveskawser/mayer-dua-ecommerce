// =========================================================
// 1. GLOBAL SCOPE FUNCTIONS (Required for inline onclick="")
// =========================================================
window.openAdvanceModal = function (element) {
    const btn = element;

    const orderRef = btn.getAttribute('data-order-ref');
    const custId = btn.getAttribute('data-cust-id');
    const net = parseFloat(btn.getAttribute('data-net')) || 0;
    const division = (btn.getAttribute('data-division') || "").toLowerCase();

    // Set Hidden Inputs
    document.getElementById('adv_orderRef').value = orderRef;
    document.getElementById('adv_customerId').value = custId;
    document.getElementById('adv_netAmount').value = net;

    // Auto-Calculate Delivery
    let delivery = 100;
    if (division.includes('dhaka')) delivery = 50;
    document.getElementById('adv_delivery').value = delivery;

    // Reset User Inputs
    document.getElementById('adv_paidAmount').value = "";
    document.getElementById('adv_note').value = "";

    // Reset Dropdown to Default
    const typeSelect = document.getElementById('adv_paymentType');
    if (typeSelect) typeSelect.value = "Advance";

    // Initial Calculation (Show full due amount)
    const total = net + delivery;
    document.getElementById('adv_dueAmount').textContent = total.toFixed(2);

    // ✅ BACKDROP FIX: Move Modal to Body
    // This ensures it sits on top of all other elements/overlays
    const modalEl = document.getElementById('advanceModal');
    if (modalEl.parentElement !== document.body) {
        document.body.appendChild(modalEl);
    }

    // Show Modal
    const modal = new bootstrap.Modal(modalEl);
    modal.show();
};

// =========================================================
// 2. DOM LOADED EVENTS
// =========================================================
document.addEventListener('DOMContentLoaded', function () {

    // --- A. TOGGLE CONFIRMATION LOGIC ---
    const toggles = document.querySelectorAll('.confirm-toggle');

    toggles.forEach(toggle => {
        toggle.addEventListener('change', function () {
            const checkbox = this;
            const orderId = checkbox.getAttribute('data-id');
            const isConfirmed = checkbox.checked;

            // UI References
            const statusBadge = document.getElementById(`status-badge-${orderId}`);
            // Note: If you don't have a label next to the checkbox in your new table, this line might be null
            const label = checkbox.nextElementSibling;

            // Optimistic UI Update (only if label exists)
            if (label) label.textContent = isConfirmed ? "Yes" : "No";

            const formData = new URLSearchParams();
            formData.append('id', orderId);
            formData.append('isConfirmed', isConfirmed);

            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

            fetch('/SalesOrder/ToggleConfirmation', { // Ensure this matches your Controller Route
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': token
                },
                body: formData.toString()
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        if (statusBadge) {
                            statusBadge.textContent = data.newStatus;
                            // Update badge color
                            if (data.newStatus === 'Confirmed') {
                                statusBadge.className = 'badge bg-success text-white';
                            } else if (data.newStatus === 'Pending') {
                                statusBadge.className = 'badge bg-warning text-dark';
                            } else {
                                statusBadge.className = 'badge bg-secondary-subtle text-dark';
                            }
                        }
                    } else {
                        revertUI(checkbox, label, !isConfirmed, data.message);
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    revertUI(checkbox, label, !isConfirmed, "Connection failed.");
                });
        });
    });

    function revertUI(checkbox, label, originalState, message) {
        checkbox.checked = originalState;
        if (label) label.textContent = originalState ? "Yes" : "No";
        alert("Action failed: " + message);
    }

    // --- B. ADVANCE MODAL LOGIC (Merged & Cleaned) ---
    const paidInput = document.getElementById('adv_paidAmount');

    if (paidInput) {
        paidInput.addEventListener('input', function () {
            const net = parseFloat(document.getElementById('adv_netAmount').value) || 0;
            const delivery = parseFloat(document.getElementById('adv_delivery').value) || 0;
            const total = net + delivery;
            const currentPay = parseFloat(this.value) || 0;

            // 1. Calculate Due
            const due = total - currentPay;
            document.getElementById('adv_dueAmount').textContent = due.toFixed(2);

            // 2. Smart Logic: Switch Payment Type automatically
            const typeSelect = document.getElementById('adv_paymentType');
            if (typeSelect) {
                // If they pay the full amount (or more), assume it's a "Sale" (Final)
                if (currentPay >= total) {
                    typeSelect.value = "Sale";
                } else {
                    typeSelect.value = "Advance";
                }
            }
        });
    }

    // --- C. SUBMIT FORM ---
    const advanceForm = document.getElementById('advanceForm');
    if (advanceForm) {
        advanceForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const payload = {
                CustomerId: parseInt(document.getElementById('adv_customerId').value),
                PaymentMethodId: parseInt(document.getElementById('adv_paymentMethod').value),
                PaymentType: document.getElementById('adv_paymentType').value,
                Amount: parseFloat(document.getElementById('adv_paidAmount').value),
                TransactionReference: document.getElementById('adv_orderRef').value,
                Notes: document.getElementById('adv_note').value
            };

            fetch('/order/add-payment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            })
                .then(r => r.json())
                .then(data => {
                    if (data.success) {
                        alert("Payment Added Successfully!");
                        location.reload();
                    } else {
                        alert("Error: " + data.message);
                    }
                })
                .catch(err => alert("Network Error"));
        });
    }
});