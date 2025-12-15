// =========================================================
// 1. GLOBAL SCOPE VARIABLES & FUNCTIONS
// =========================================================

// Store the pure product price (Total - Old Delivery) globally to avoid DOM read issues
window.currentBasePrice = 0;

window.openAdvanceModal = function (element) {
    // --- 1. EXTRACT DATA ---
    const btn = element;
    const orderRef = btn.getAttribute('data-order-ref');
    const custId = btn.getAttribute('data-cust-id');

    // Helper: Safely parse "1,250.00" or "1250" to float
    const parseVal = (val) => {
        if (!val) return 0;
        // Remove commas if present
        const clean = val.toString().replace(/,/g, '');
        return parseFloat(clean) || 0;

        // Inside window.openAdvanceModal...

        // Reset Dropdown
        const typeSelect = document.getElementById('adv_paymentType');
        if (typeSelect) typeSelect.value = "Advance";

        // Immediately hide COD because we just set it to "Advance"
        if (window.toggleCOD) window.toggleCOD();

        // ✅ NEW: Initialize the correct placeholder
        if (window.updateNotePlaceholder) window.updateNotePlaceholder();

        // Clear Validation Styles...
        
    };

    // Note: 'data-net' from DB includes the OLD delivery charge
    const totalPayable = parseVal(btn.getAttribute('data-net'));
    const alreadyPaid = parseVal(btn.getAttribute('data-paid'));
    const storedDelivery = parseVal(btn.getAttribute('data-delivery'));

    // ✅ NEW: Read Product Price & Discount
    const productPrice = parseVal(btn.getAttribute('data-product-price'));
    const discount = parseVal(btn.getAttribute('data-discount'));

    // --- 2. CALCULATE BASE PRICE (CRITICAL STEP) ---
    // This is the price of items only (e.g., 1673 - 123 = 1550)
    // Logic kept unchanged as requested
    window.currentBasePrice = totalPayable - storedDelivery;

    console.log("Modal Debug:", {
        totalFromDb: totalPayable,
        deliveryFromDb: storedDelivery,
        calculatedBase: window.currentBasePrice
    });

    // --- 3. POPULATE INPUTS ---
    document.getElementById('adv_orderRef').value = orderRef;
    document.getElementById('adv_customerId').value = custId;

    // ✅ NEW: Populate Visual Fields
    const productInput = document.getElementById('adv_productPrice');
    const discountInput = document.getElementById('adv_discount');

    if (productInput) productInput.value = productPrice.toLocaleString('en-BD', { minimumFractionDigits: 2 });
    if (discountInput) discountInput.value = discount.toLocaleString('en-BD', { minimumFractionDigits: 2 });

    // Set Editable Delivery Field
    document.getElementById('adv_delivery').value = storedDelivery;

    // Set Read-Only Total Field (Initially matches DB)
    document.getElementById('adv_netAmount').value = totalPayable;

    // Set History
    document.getElementById('adv_alreadyPaid').value = alreadyPaid;
    document.getElementById('adv_displayPaid').textContent = alreadyPaid.toLocaleString('en-BD', { minimumFractionDigits: 2 });

    // Reset User Fields
    document.getElementById('adv_paidAmount').value = "";
    document.getElementById('adv_note').value = "";

    // Reset Dropdown
    // Reset Dropdown
    const typeSelect = document.getElementById('adv_paymentType');
    if (typeSelect) typeSelect.value = "Advance";

    // ✅ NEW: Immediately hide COD because we just set it to "Advance"
    if (window.toggleCOD) window.toggleCOD();

    // Clear Validation Styles
    const paidInput = document.getElementById('adv_paidAmount');
    const submitBtn = document.getElementById('adv_submitBtn');
    if (paidInput) paidInput.classList.remove('is-invalid');
    if (submitBtn) submitBtn.disabled = false;

    // --- 4. TRIGGER INITIAL CALCULATION ---
    // This ensures "Current Due" is correct based on the inputs immediately
    if (typeof window.calculateTotals === 'function') {
        window.calculateTotals();
    }

    // --- 5. SHOW MODAL ---
    const modalEl = document.getElementById('advanceModal');
    // Ensure modal is in body (prevents z-index/overlay issues)
    if (modalEl.parentElement !== document.body) {
        document.body.appendChild(modalEl);
    }
    const modal = new bootstrap.Modal(modalEl);
    modal.show();
};
// =========================================================
// NEW FUNCTION: Update Note Placeholder based on inputs
// =========================================================
window.updateNotePlaceholder = function () {
    const typeSelect = document.getElementById('adv_paymentType');
    const methodSelect = document.getElementById('adv_paymentMethod');
    const noteInput = document.getElementById('adv_note');

    if (!typeSelect || !methodSelect || !noteInput) return;

    // Get current values
    const paymentType = typeSelect.value; // "Advance" or "Sale"

    // Get the text of the selected method (safe way to handle DB-driven values)
    const selectedOption = methodSelect.options[methodSelect.selectedIndex];
    const methodText = selectedOption ? selectedOption.text.trim().toLowerCase() : "";

    // LOGIC:
    // If Type is "Sale" AND Method is "Cash on Delivery" -> Show Courier/Receipt placeholder
    // Otherwise (Advance, Bkash, Nagad, etc.) -> Show Transaction ID placeholder
    if (paymentType === 'Sale' && methodText.includes('cash on delivery')) {
        noteInput.placeholder = "Money Receipt No, Courier ID, or Optional Note...";
    } else {
        noteInput.placeholder = "Transaction ID / Sender Number (Required)";
    }
};
// --- B. LISTENERS ---

// 1. Payment Method Change (Add this new listener)
const methodSelect = document.getElementById('adv_paymentMethod');
if (methodSelect) {
    methodSelect.addEventListener('change', function () {
        if (window.updateNotePlaceholder) window.updateNotePlaceholder();
    });
}

// 2. Payment Type Change (Update existing listener)
const typeSelect = document.getElementById('adv_paymentType');
if (typeSelect) {
    typeSelect.addEventListener('change', function () {
        // Existing Logic
        if (window.toggleCOD) window.toggleCOD();

        // ✅ NEW: Update placeholder when type switches between Sale/Advance
        if (window.updateNotePlaceholder) window.updateNotePlaceholder();

        // Existing Calculation Logic
        const { currentDue } = window.calculateTotals();
        const payInput = document.getElementById('adv_paidAmount');

        if (this.value === 'Sale') {
            payInput.value = currentDue > 0 ? currentDue : 0;
        } else {
            payInput.value = "";
        }
        window.calculateTotals();
    });
}
// =========================================================
// 2. DOM LOADED EVENTS
// =========================================================
document.addEventListener('DOMContentLoaded', function () {

    // --- A. GLOBAL CALCULATION LOGIC ---
    // Defined globally so openAdvanceModal can call it
    window.calculateTotals = function () {
        // Re-select elements to ensure valid references
        const netInput = document.getElementById('adv_netAmount');
        const deliveryInput = document.getElementById('adv_delivery');
        const alreadyPaidInput = document.getElementById('adv_alreadyPaid');
        const payInput = document.getElementById('adv_paidAmount');
        const submitBtn = document.getElementById('adv_submitBtn');

        const displayCurrentDue = document.getElementById('adv_displayCurrentDue');
        const displayBalance = document.getElementById('adv_dueAmount');

        // 1. Get Live Values
        const newDelivery = parseFloat(deliveryInput.value) || 0;
        const alreadyPaid = parseFloat(alreadyPaidInput.value) || 0;
        const payingNow = parseFloat(payInput.value) || 0;

        // 2. Use Global Base Price (Product Cost)
        // If undefined, fallback to 0
        const base = window.currentBasePrice || 0;

        // 3. Calculate New Total (Base + Editable Delivery)
        const newTotalPayable = base + newDelivery;

        // 4. Update the Read-Only Total Field
        if (netInput) netInput.value = newTotalPayable; // Update UI

        // 5. Calculate Current Due
        const currentDue = newTotalPayable - alreadyPaid;

        // 6. Update "Current Due" Display
        if (displayCurrentDue) {
            if (currentDue <= 0) {
                displayCurrentDue.innerHTML = '<span class="text-success"><i class="fas fa-check"></i> Fully Paid</span>';
            } else {
                displayCurrentDue.textContent = currentDue.toFixed(2);
            }
        }

        // 7. Calculate Balance After Payment
        const balanceAfter = currentDue - payingNow;

        // 8. Update Balance Display
        if (displayBalance) {
            displayBalance.textContent = balanceAfter.toFixed(2);
            if (balanceAfter < 0) {
                // Overpayment
                displayBalance.parentElement.className = "text-success fw-bold fs-5";
            } else {
                // Remaining Due
                displayBalance.parentElement.className = "text-danger fw-bold fs-5";
            }
        }

        // 9. Validation (Prevent Paying > Due)
        if (payingNow > (currentDue + 0.5) && currentDue > 0) {
            if (payInput) payInput.classList.add('is-invalid');
            if (submitBtn) submitBtn.disabled = true;
        } else {
            if (payInput) payInput.classList.remove('is-invalid');
            if (submitBtn) submitBtn.disabled = false;
        }

        return { currentDue, payingNow };
    };
    // ... existing listeners ...

    

    // =========================================================
// NEW FUNCTION: Toggle "Cash On Delivery" visibility
// =========================================================
    window.toggleCOD = function () {
        const typeSelect = document.getElementById('adv_paymentType');
        const methodSelect = document.getElementById('adv_paymentMethod');

        if (!typeSelect || !methodSelect) return;

        // Check if "Advance" is currently selected
        const isAdvance = typeSelect.value === 'Advance';

        // Loop through all Payment Method options
        Array.from(methodSelect.options).forEach(option => {
            // defined strictly by the text shown in the dropdown
            const methodText = option.text.trim().toLowerCase();

            if (methodText === "cash on delivery") {
                if (isAdvance) {
                    // HIDE IT
                    option.style.display = "none"; // Hides from view
                    option.disabled = true;        // Prevents selection via keyboard/script

                    // If it was already selected, switch to the first available valid option
                    if (methodSelect.value === option.value) {
                        // Find the first option that isn't disabled
                        const validOption = Array.from(methodSelect.options).find(o => !o.disabled);
                        if (validOption) methodSelect.value = validOption.value;
                    }
                } else {
                    // SHOW IT
                    option.style.display = "";
                    option.disabled = false;
                }
            }
        });
    };

    // --- B. LISTENERS ---

    // 1. Delivery Change -> Updates Total & Due
    const deliveryInput = document.getElementById('adv_delivery');
    if (deliveryInput) {
        deliveryInput.addEventListener('input', window.calculateTotals);
    }

    // 2. Amount Change -> Updates Balance & Validation
    const payInput = document.getElementById('adv_paidAmount');
    const typeSelect = document.getElementById('adv_paymentType');

    if (payInput) {
        payInput.addEventListener('input', function () {
            const { currentDue, payingNow } = window.calculateTotals();

            // Auto-switch dropdown
            if (typeSelect && payingNow <= currentDue) {
                if (payingNow >= currentDue && currentDue > 0) typeSelect.value = "Sale";
                else typeSelect.value = "Advance";
            }
        });
    }

    // 3. Dropdown Change
    if (typeSelect) {
        typeSelect.addEventListener('change', function () {
            // ✅ NEW: Run the toggle logic whenever this changes
            if (window.toggleCOD) window.toggleCOD();

            const { currentDue } = window.calculateTotals();
            if (this.value === 'Sale') {
                payInput.value = currentDue > 0 ? currentDue : 0;
            } else {
                payInput.value = "";
            }
            window.calculateTotals();
        });
    }

    // --- C. SUBMIT FORM ---
    const advanceForm = document.getElementById('advanceForm');
    if (advanceForm) {
        advanceForm.addEventListener('submit', function (e) {
            e.preventDefault();

            // Grab the FINAL delivery charge from input
            const deliveryVal = parseFloat(document.getElementById('adv_delivery').value) || 0;

            const payload = {
                CustomerId: parseInt(document.getElementById('adv_customerId').value),
                PaymentMethodId: parseInt(document.getElementById('adv_paymentMethod').value),
                PaymentType: document.getElementById('adv_paymentType').value,
                Amount: parseFloat(document.getElementById('adv_paidAmount').value),
                TransactionReference: document.getElementById('adv_orderRef').value,
                Notes: document.getElementById('adv_note').value,
                // Send new delivery to update Order Header
                DeliveryCharge: deliveryVal
            };

            fetch('/order/add-payment', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            }).then(r => r.json()).then(data => {
                if (data.success) {
                    alert("Payment Added & Order Updated Successfully!");
                    location.reload();
                } else {
                    alert("Error: " + data.message);
                }
            }).catch(err => alert("Network Error"));
        });
    }
// =========================================================
// NEW FUNCTION: Update Note Placeholder based on Method
// =========================================================
    // --- D. TOGGLE CONFIRMATION (Existing Logic) ---
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
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

            fetch('/SalesOrder/ToggleConfirmation', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded', 'RequestVerificationToken': token },
                body: formData.toString()
            }).then(r => r.json()).then(data => {
                if (data.success && statusBadge) {
                    statusBadge.textContent = data.newStatus;
                    statusBadge.className = data.newStatus === 'Confirmed' ? 'badge bg-success text-white' : 'badge bg-warning text-dark';
                } else if (!data.success) {
                    checkbox.checked = !isConfirmed;
                    alert("Action failed: " + data.message);
                }
            }).catch(err => {
                checkbox.checked = !isConfirmed;
                alert("Network error.");
            });
        });
    });
});
// =========================================================
// 3. CANCEL ORDER LOGIC in order actions js (bottom)
// =========================================================
window.cancelOrder = function (orderId, orderDisplayId) {
    if (!confirm(`Are you sure you want to CANCEL Order #${orderDisplayId}? This action cannot be undone.`)) {
        return;
    }

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
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
                    cancelBtn.style.display = 'none'; // Or cancelBtn.remove();
                }

                // 3. Disable the Toggle Switch (optional but good UX)
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