// =========================================================
// 1. GLOBAL SCOPE VARIABLES & FUNCTIONS
// =========================================================

// Store the pure product price (Total - Old Delivery)
window.currentBasePrice = 0;

// ---------------------------------------------------------
// OPEN ADVANCE MODAL
// ---------------------------------------------------------
window.openAdvanceModal = function (element) {

    const btn = element;
    const orderRef = btn.getAttribute('data-order-ref');
    const custId = btn.getAttribute('data-cust-id');

    const parseVal = (val) => {
        if (!val) return 0;
        return parseFloat(val.toString().replace(/,/g, '')) || 0;
    };

    const totalPayable = parseVal(btn.getAttribute('data-net'));
    const alreadyPaid = parseVal(btn.getAttribute('data-paid'));
    const storedDelivery = parseVal(btn.getAttribute('data-delivery'));

    const productPrice = parseVal(btn.getAttribute('data-product-price'));
    const discount = parseVal(btn.getAttribute('data-discount'));

    // BASE PRICE (product only)
    window.currentBasePrice = totalPayable - storedDelivery;

    // Populate hidden/meta fields
    document.getElementById('adv_orderRef').value = orderRef;
    document.getElementById('adv_customerId').value = custId;

    // Visual-only fields
    document.getElementById('adv_productPrice').value =
        productPrice.toLocaleString('en-BD', { minimumFractionDigits: 2 });

    document.getElementById('adv_discount').value =
        discount.toLocaleString('en-BD', { minimumFractionDigits: 2 });

    document.getElementById('adv_delivery').value = storedDelivery;
    document.getElementById('adv_netAmount').value = totalPayable;

    document.getElementById('adv_alreadyPaid').value = alreadyPaid;
    document.getElementById('adv_displayPaid').textContent =
        alreadyPaid.toLocaleString('en-BD', { minimumFractionDigits: 2 });

    // Reset user inputs
    document.getElementById('adv_paidAmount').value = "";
    document.getElementById('adv_note').value = "";

    // Reset dropdown
    const typeSelect = document.getElementById('adv_paymentType');
    if (typeSelect) typeSelect.value = "Advance";

    if (window.toggleCOD) window.toggleCOD();
    if (window.updateNotePlaceholder) window.updateNotePlaceholder();
    if (window.updateNoteValidation) window.updateNoteValidation();

    const paidInput = document.getElementById('adv_paidAmount');
    const submitBtn = document.getElementById('adv_submitBtn');

    if (paidInput) paidInput.classList.remove('is-invalid');
    if (submitBtn) submitBtn.disabled = false;

    if (window.calculateTotals) window.calculateTotals();

    const modalEl = document.getElementById('advanceModal');
    if (modalEl.parentElement !== document.body) {
        document.body.appendChild(modalEl);
    }
    new bootstrap.Modal(modalEl).show();
};

// =========================================================
// NOTE PLACEHOLDER
// =========================================================
window.updateNotePlaceholder = function () {
    const typeSelect = document.getElementById('adv_paymentType');
    const methodSelect = document.getElementById('adv_paymentMethod');
    const noteInput = document.getElementById('adv_note');

    if (!typeSelect || !methodSelect || !noteInput) return;

    const paymentType = typeSelect.value;
    const selectedOption = methodSelect.options[methodSelect.selectedIndex];
    const methodText = selectedOption ? selectedOption.text.trim().toLowerCase() : "";

    if (paymentType === 'Sale' && methodText.includes('cash on delivery')) {
        noteInput.placeholder = "Money Receipt No, Courier ID, or Optional Note...";
    } else {
        noteInput.placeholder = "Transaction ID / Sender Number (Required)";
    }
};

// =========================================================
// NOTE VALIDATION (RED STAR + REQUIRED)
// =========================================================
window.updateNoteValidation = function () {
    const typeSelect = document.getElementById('adv_paymentType');
    const methodSelect = document.getElementById('adv_paymentMethod');
    const noteInput = document.getElementById('adv_note');
    const starSpan = document.getElementById('adv_note_star');

    if (!typeSelect || !methodSelect || !noteInput || !starSpan) return;

    const paymentType = typeSelect.value;
    const selectedOption = methodSelect.options[methodSelect.selectedIndex];
    const methodText = selectedOption ? selectedOption.text.trim().toLowerCase() : "";
    const isCOD = methodText.includes('cash on delivery');

    let isRequired = false;

    if (paymentType === 'Advance') isRequired = true;
    else if (paymentType === 'Sale' && !isCOD) isRequired = true;

    if (isRequired) {
        noteInput.setAttribute('required', 'required');
        starSpan.classList.remove('d-none');
    } else {
        noteInput.removeAttribute('required');
        starSpan.classList.add('d-none');
        noteInput.classList.remove('is-invalid');
    }
};

// =========================================================
// DOM READY
// =========================================================
document.addEventListener('DOMContentLoaded', function () {

    // =====================================================
    // CALCULATION LOGIC
    // =====================================================
    window.calculateTotals = function () {

        const netInput = document.getElementById('adv_netAmount');
        const deliveryInput = document.getElementById('adv_delivery');
        const alreadyPaidInput = document.getElementById('adv_alreadyPaid');
        const payInput = document.getElementById('adv_paidAmount');
        const submitBtn = document.getElementById('adv_submitBtn');

        const displayCurrentDue = document.getElementById('adv_displayCurrentDue');
        const displayBalance = document.getElementById('adv_dueAmount');

        const newDelivery = parseFloat(deliveryInput.value) || 0;
        const alreadyPaid = parseFloat(alreadyPaidInput.value) || 0;
        let payingNow = parseFloat(payInput.value) || 0;

        const base = window.currentBasePrice || 0;
        const newTotalPayable = base + newDelivery;

        netInput.value = newTotalPayable;

        const currentDue = newTotalPayable - alreadyPaid;

        // Fully paid lock
        if (currentDue <= 0) {
            displayCurrentDue.innerHTML =
                '<span class="text-success"><i class="fas fa-check"></i> Fully Paid</span>';
            payInput.value = "";
            payInput.disabled = true;
            submitBtn.disabled = true;
            displayBalance.textContent = "0.00";
            return { currentDue: 0, payingNow: 0 };
        }

        payInput.disabled = false;
        displayCurrentDue.textContent = currentDue.toFixed(2);

        // Hard overpayment cap
        if (payingNow > currentDue) {
            payingNow = currentDue;
            payInput.value = currentDue.toFixed(2);
        }

        const balanceAfter = currentDue - payingNow;
        displayBalance.textContent = balanceAfter.toFixed(2);

        submitBtn.disabled = payingNow <= 0 || payingNow > currentDue;

        return { currentDue, payingNow };
    };

    // =====================================================
    // COD TOGGLE
    // =====================================================
    // =====================================================
    // COD TOGGLE (Improved to force switch if COD is selected)
    // =====================================================
    window.toggleCOD = function () {
        const typeSelect = document.getElementById('adv_paymentType');
        const methodSelect = document.getElementById('adv_paymentMethod');

        if (!typeSelect || !methodSelect) return;

        const isAdvance = typeSelect.value === 'Advance';
        let codOption = null;

        // 1. Loop to find COD, hide/disable it
        Array.from(methodSelect.options).forEach(option => {
            const text = option.text.trim().toLowerCase();
            // Use 'includes' to be safe against minor spacing issues
            if (text.includes('cash on delivery')) {
                codOption = option;
                option.style.display = isAdvance ? 'none' : '';
                option.disabled = isAdvance;
            }
        });

        // 2. CRITICAL FIX: If we are in "Advance" mode AND the current selection is COD
        // We must switch to a different option immediately.
        if (isAdvance && codOption && methodSelect.value === codOption.value) {
            // Find the first option that is NOT disabled
            const validOption = Array.from(methodSelect.options).find(o => !o.disabled && o.value !== "");

            if (validOption) {
                methodSelect.value = validOption.value;
                // Trigger change event so placeholders update immediately
                methodSelect.dispatchEvent(new Event('change'));
            }
        }
    };
    // =====================================================
    // LISTENERS
    // =====================================================

    document.getElementById('adv_delivery')
        ?.addEventListener('input', window.calculateTotals);

    const payInput = document.getElementById('adv_paidAmount');
    const typeSelect = document.getElementById('adv_paymentType');

    if (payInput) {
        payInput.addEventListener('input', function () {
            const { currentDue, payingNow } = window.calculateTotals();

            if (typeSelect) {
                if (currentDue > 0 && payingNow === currentDue)
                    typeSelect.value = "Sale";
                else
                    typeSelect.value = "Advance";
            }

            if (window.toggleCOD) window.toggleCOD();
            if (window.updateNoteValidation) window.updateNoteValidation();
        });
    }

    const methodSelect = document.getElementById('adv_paymentMethod');
    if (methodSelect) {
        methodSelect.addEventListener('change', function () {
            if (window.updateNotePlaceholder) window.updateNotePlaceholder();
            if (window.updateNoteValidation) window.updateNoteValidation();
        });
    }

    if (typeSelect) {
        typeSelect.addEventListener('change', function () {
            if (window.toggleCOD) window.toggleCOD();
            if (window.updateNotePlaceholder) window.updateNotePlaceholder();
            if (window.updateNoteValidation) window.updateNoteValidation();

            const { currentDue } = window.calculateTotals();

            if (this.value === 'Sale') {
                payInput.value = currentDue > 0 ? currentDue.toFixed(2) : "0.00";
            } else {
                payInput.value = "";
            }

            window.calculateTotals();
        });
    }

    // =====================================================
    // SUBMIT
    // =====================================================
    document.getElementById('advanceForm')
        ?.addEventListener('submit', function (e) {
            e.preventDefault();

            const amount = parseFloat(document.getElementById('adv_paidAmount').value) || 0;
            const { currentDue } = window.calculateTotals();

            if (amount <= 0 || amount > currentDue) {
                alert("Invalid payment amount.");
                return;
            }

            const payload = {
                CustomerId: parseInt(document.getElementById('adv_customerId').value),
                PaymentMethodId: parseInt(document.getElementById('adv_paymentMethod').value),
                PaymentType: document.getElementById('adv_paymentType').value,
                Amount: amount,
                TransactionReference: document.getElementById('adv_orderRef').value,
                Notes: document.getElementById('adv_note').value,
                DeliveryCharge: parseFloat(document.getElementById('adv_delivery').value) || 0
            };

            fetch('/order/add-payment', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            })
                .then(r => r.json())
                .then(data => {
                    if (!data.success) {
                        alert(data.message);
                        return;
                    }
                    alert("Payment Added & Order Updated Successfully!");
                    location.reload();
                })
                .catch(() => alert("Network Error"));
        });


    // =====================================================
    // TOGGLE CONFIRMATION LOGIC
    // =====================================================
    const toggles = document.querySelectorAll('.confirm-toggle');
    toggles.forEach(toggle => {
        toggle.addEventListener('change', function () {
            const checkbox = this;
            const orderId = checkbox.getAttribute('data-id');
            const isConfirmed = checkbox.checked;
            const statusBadge = document.getElementById(`status-badge-${orderId}`);

            const formData = new URLSearchParams();
            formData.append('id', orderId);
            formData.append('isConfirmed', isConfirmed);

            // Get token safely
            const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
            const token = tokenInput ? tokenInput.value : '';

            fetch('/SalesOrder/ToggleConfirmation', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': token
                },
                body: formData.toString()
            }).then(r => r.json()).then(data => {
                if (data.success && statusBadge) {

                    // ✅ FIX: Check if status is "Draft" and change text to "Pending"
                    const displayStatus = data.newStatus === 'Draft' ? 'Pending' : data.newStatus;

                    statusBadge.textContent = displayStatus;

                    statusBadge.className = data.newStatus === 'Confirmed'
                        ? 'badge bg-success text-white'
                        : 'badge bg-warning text-dark';

                } else if (!data.success) {
                    checkbox.checked = !isConfirmed; // Revert
                    alert("Action failed: " + data.message);
                }
            }).catch(err => {
                checkbox.checked = !isConfirmed; // Revert
                alert("Network error.");
            });
        });
    });
    // =========================================================
    // CANCEL ORDER ACTION
    // =========================================================
    window.cancelOrder = function (orderId, orderDisplayId) {
        if (!confirm(`Are you sure you want to CANCEL Order #${orderDisplayId}? This action cannot be undone.`)) {
            return;
        }

        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        const token = tokenInput ? tokenInput.value : '';

        const formData = new URLSearchParams();
        formData.append('id', orderId);
        formData.append('status', 'Cancelled');

        fetch('/SalesOrder/UpdateStatus', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': token
            },
            body: formData.toString()
        })
            .then(r => r.json())
            .then(data => {
                if (data.success) {
                    // 1. Update Badge to Red immediately
                    const badge = document.getElementById(`status-badge-${orderId}`);
                    if (badge) {
                        badge.textContent = "Cancelled";
                        badge.className = "badge bg-danger text-white";
                    }

                    // 2. Hide the Cancel Button immediately
                    const cancelBtn = document.getElementById(`btn-cancel-${orderId}`);
                    if (cancelBtn) {
                        cancelBtn.style.display = 'none';
                    }

                    // 3. Disable the Toggle Switch
                    const toggle = document.getElementById(`toggle-${orderId}`);
                    if (toggle) {
                        toggle.disabled = true;
                        toggle.checked = false;
                    }

                    alert("Order Cancelled Successfully.");
                } else {
                    alert("Error: " + data.message);
                }
            })
            .catch(err => alert("Network Error: Could not cancel order."));
    };
});
