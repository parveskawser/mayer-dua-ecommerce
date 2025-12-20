document.addEventListener('DOMContentLoaded', function () {
    console.log("✅ Bulk Order Script Loaded");

    // Used to refresh the modal after an action (Receive/Reject)
    let currentOpenedOrderId = null;

    // =========================================================
    // 1. MODAL LOGIC (Z-Index Fix & Data Fetching)
    // =========================================================

    let detailsModalInstance = null;

    // Event Delegation: Listen for clicks on the body
    document.body.addEventListener('click', function (e) {
        // Check if the clicked element has the class 'js-btn-details'
        const btn = e.target.closest('.js-btn-details');

        if (btn) {
            e.preventDefault();
            const orderId = btn.getAttribute('data-order-id');
            openDetailsModal(orderId);
        }
    });

    function openDetailsModal(id) {
        console.log(`🔍 Opening details for Order ID: ${id}`);

        // Save ID for refreshing later
        currentOpenedOrderId = id;

        const modalEl = document.getElementById('detailsModal');
        const modalBody = document.getElementById('modalContentPlaceholder');

        if (!modalEl || !modalBody) {
            console.error("❌ Modal elements not found!");
            return;
        }

        if (modalEl.parentElement !== document.body) {
            document.body.appendChild(modalEl);
        }

        if (detailsModalInstance) {
            detailsModalInstance.dispose();
            detailsModalInstance = null;
        }

        detailsModalInstance = new bootstrap.Modal(modalEl, {
            backdrop: 'static',
            keyboard: false
        });

        // Set Loading UI
        modalBody.innerHTML = `
            <div class="d-flex flex-column align-items-center justify-content-center py-5">
                <div class="spinner-border text-primary" role="status"></div>
                <span class="mt-2 text-muted">Loading details...</span>
            </div>`;

        // Show Modal
        detailsModalInstance.show();

        // AJAX Fetch
        fetch(`/Purchase/GetBulkOrderDetails?id=${id}`)
            .then(res => {
                if (!res.ok) throw new Error("Network response was not ok");
                return res.text();
            })
            .then(html => {
                modalBody.innerHTML = html;
            })
            .catch(err => {
                console.error(err);
                modalBody.innerHTML = `<div class="alert alert-danger m-3">Error loading data.</div>`;
            });
    }

    // Cleanup backdrops on page reload/navigation
    window.addEventListener('beforeunload', function () {
        const backdrops = document.querySelectorAll('.modal-backdrop');
        backdrops.forEach(b => b.remove());
    });

    // =========================================================
    // 2. FORM SUBMISSION & CHECKBOX LOGIC (Existing Code)
    // =========================================================

    const form = document.getElementById('bulkOrderForm');

    if (form) {
        form.addEventListener('submit', function (e) {
            console.log("--- FORM SUBMISSION STARTED ---");
            const checkedVariants = document.querySelectorAll('.variant-checkbox:checked');

            if (checkedVariants.length === 0) {
                alert("Please select at least one item.");
                e.preventDefault();
            }
        });
    }

    // Parent Checkbox (Select All)
    const prodCheckboxes = document.querySelectorAll('.product-checkbox');
    prodCheckboxes.forEach(parentCb => {
        parentCb.addEventListener('change', function () {
            const parentId = this.id;
            const children = document.querySelectorAll(`.variant-checkbox[data-parent="${parentId}"]`);
            children.forEach(child => {
                child.checked = this.checked;
            });
        });
    });

    // Child Checkbox Logic
    const variantCheckboxes = document.querySelectorAll('.variant-checkbox');
    variantCheckboxes.forEach(childCb => {
        childCb.addEventListener('change', function () {
            const parentId = this.dataset.parent;
            const parentCb = document.getElementById(parentId);
            const siblings = document.querySelectorAll(`.variant-checkbox[data-parent="${parentId}"]`);

            const allChecked = Array.from(siblings).every(cb => cb.checked);
            const someChecked = Array.from(siblings).some(cb => cb.checked);

            parentCb.checked = allChecked;
            parentCb.indeterminate = someChecked && !allChecked;
        });
    });

    // =========================================================
    // 3. IN-MODAL RECEIVE / REJECT LOGIC (New Features)
    // =========================================================

    // A. Toggle "Receive" Form Visibility
    document.body.addEventListener('click', function (e) {
        const btn = e.target.closest('.js-btn-receive-toggle');
        if (btn) {
            e.preventDefault();
            const targetId = btn.getAttribute('data-target'); // e.g., "form-102"
            const formRow = document.getElementById(targetId);
            const btnGroup = btn.parentElement; // The div holding the buttons

            if (formRow) {
                formRow.classList.remove('d-none'); // Show input row
                btnGroup.classList.add('d-none');   // Hide action buttons temporarily
            }
        }
    });

    // B. Cancel Receive Action
    // =========================================================
    // B. Cancel Receive Action (Fix)
    // =========================================================
    document.body.addEventListener('click', function (e) {
        // Look for the class 'js-btn-cancel-receive'
        const btn = e.target.closest('.js-btn-cancel-receive');

        if (btn) {
            e.preventDefault();
            console.log("Cancel button clicked");

            // 1. Get the target ID (e.g., "form-105")
            const targetId = btn.getAttribute('data-target');

            // 2. Find the Form Row (The row to hide)
            const formRow = document.getElementById(targetId);

            // 3. Find the Button Group (The buttons to show again)
            // Extract ID: "form-105" -> "105"
            const poId = targetId.split('-')[1];
            const btnGroup = document.getElementById(`btn-group-${poId}`);

            // 4. Toggle Classes
            if (formRow) {
                formRow.classList.add('d-none'); // Hide the input row
            } else {
                console.error("Form row not found:", targetId);
            }

            if (btnGroup) {
                btnGroup.classList.remove('d-none'); // Show the original buttons
            } else {
                console.error("Button group not found: btn-group-" + poId);
            }
        }
    });
    // C. Confirm Receive (AJAX POST)
    document.body.addEventListener('click', function (e) {
        const btn = e.target.closest('.js-btn-confirm-receive');
        if (btn) {
            e.preventDefault();

            // 1. Gather Data from the row
            const row = btn.closest('tr');
            const variantId = parseInt(btn.getAttribute('data-variantid'));
            const poId = parseInt(btn.getAttribute('data-poid'));
            const maxQty = parseInt(btn.getAttribute('data-max-qty'));

            // ✅ CHANGED: Get quantity from the new input field
            const qtyInput = row.querySelector('.js-input-qty');
            const qtyVal = parseInt(qtyInput.value);

            const priceVal = row.querySelector('.js-input-price').value;
            const invoice = row.querySelector('.js-input-invoice').value;
            const remarks = row.querySelector('.js-input-remarks').value;

            // 2. Validate
            if (!qtyVal || qtyVal <= 0) {
                alert("Please enter a valid Quantity.");
                return;
            }

            if (qtyVal > maxQty) {
                alert(`You cannot receive more than the ordered quantity (${maxQty}).`);
                return;
            }

            if (!priceVal || parseFloat(priceVal) < 0) {
                alert("Please enter a valid Buying Price.");
                return;
            }

            const payload = {
                ProductVariantId: variantId,
                Quantity: qtyVal, // Send the user-input quantity
                BuyingPrice: parseFloat(priceVal),
                InvoiceNo: invoice,
                Remarks: remarks
            };

            // 3. UI Feedback
            const originalText = btn.innerText;
            btn.disabled = true;
            btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';

            // 4. Send Request
            fetch('/purchase/receive-stock', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        if (currentOpenedOrderId) {
                            openDetailsModal(currentOpenedOrderId);
                        }
                    } else {
                        alert(data.message);
                        btn.disabled = false;
                        btn.innerText = originalText;
                    }
                })
                .catch(err => {
                    console.error(err);
                    alert("An error occurred while connecting to the server.");
                    btn.disabled = false;
                    btn.innerText = originalText;
                });
        }
    });
    // D. Reject Item Logic
    document.body.addEventListener('click', function (e) {
        const btn = e.target.closest('.js-btn-reject');
        if (btn) {
            e.preventDefault();

            if (!confirm("Are you sure you want to REJECT this item? This cannot be undone.")) {
                return;
            }

            const poId = parseInt(btn.getAttribute('data-poid'));
            const originalHtml = btn.innerHTML;

            btn.disabled = true;
            btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';

            fetch('/purchase/reject-item', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ PoRequestId: poId })
            })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        // Success: Refresh the modal content
                        if (currentOpenedOrderId) {
                            openDetailsModal(currentOpenedOrderId);
                        }
                    } else {
                        alert(data.message);
                        btn.disabled = false;
                        btn.innerHTML = originalHtml;
                    }
                })
                .catch(err => {
                    console.error(err);
                    alert("Error rejecting item.");
                    btn.disabled = false;
                    btn.innerHTML = originalHtml;
                });
        }
    });

});