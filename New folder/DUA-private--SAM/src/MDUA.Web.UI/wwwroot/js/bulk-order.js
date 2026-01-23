document.addEventListener('DOMContentLoaded', function () {
    console.log("✅ Bulk Order Script Loaded");

    // Used to refresh the modal after an action (Receive/Reject)
    let currentOpenedOrderId = null;
    let detailsModalInstance = null;

    // =========================================================
    // 1. OPEN MODAL LOGIC
    // =========================================================
    document.body.addEventListener('click', function (e) {
        const btn = e.target.closest('.js-btn-details');
        if (btn) {
            e.preventDefault();
            const orderId = btn.getAttribute('data-order-id');
            openDetailsModal(orderId);
        }
    });

    function openDetailsModal(id) {
        console.log(`🔍 Opening details for Order ID: ${id}`);
        currentOpenedOrderId = id;

        const modalEl = document.getElementById('detailsModal');
        const modalBody = document.getElementById('modalContentPlaceholder');

        if (!modalEl || !modalBody) return;

        // Ensure Modal is in Body (Z-Index fix)
        if (modalEl.parentElement !== document.body) {
            document.body.appendChild(modalEl);
        }

        if (!detailsModalInstance) {
            detailsModalInstance = new bootstrap.Modal(modalEl, { backdrop: 'static', keyboard: false });
        }

        // Loading State
        modalBody.innerHTML = `
            <div class="d-flex flex-column align-items-center justify-content-center py-5">
                <div class="spinner-border text-primary" role="status"></div>
                <span class="mt-2 text-muted">Loading details...</span>
            </div>`;

        detailsModalInstance.show();

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

    // =========================================================
    // 2. BULK RECEIVE ACTIONS (Inside Modal)
    // =========================================================

    // A. Show Bulk Form (Receive All)
    document.body.addEventListener('click', function (e) {
        if (e.target.closest('#btnShowReceiveAll')) {
            e.preventDefault();
            const listContainer = document.getElementById('originalListContainer');
            const bulkHeader = document.getElementById('bulkActionsHeader');
            const bulkForm = document.getElementById('bulkReceiveContainer');

            if (listContainer && bulkForm) {
                listContainer.classList.add('d-none');
                if (bulkHeader) bulkHeader.classList.add('d-none');
                bulkForm.classList.remove('d-none');
                recalcBulkTotals();
            }
        }
    });

    // B. Cancel Bulk Form
    document.body.addEventListener('click', function (e) {
        if (e.target.id === 'btnCancelBulk') {
            e.preventDefault();
            document.getElementById('bulkReceiveContainer').classList.add('d-none');
            document.getElementById('originalListContainer').classList.remove('d-none');
            const header = document.getElementById('bulkActionsHeader');
            if (header) header.classList.remove('d-none');
        }
    });

    // C. Live Totals Calculation
    document.body.addEventListener('input', function (e) {
        if (e.target.matches('.bulk-qty, .bulk-price')) {
            recalcBulkTotals();
        }
    });

    function recalcBulkTotals() {
        let total = 0;
        document.querySelectorAll('.bulk-item-row').forEach(row => {
            const qty = parseFloat(row.querySelector('.bulk-qty').value) || 0;
            const price = parseFloat(row.querySelector('.bulk-price').value) || 0;
            const rowTotal = qty * price;
            row.querySelector('.bulk-row-total').innerText = rowTotal.toFixed(2);
            total += rowTotal;
        });
        const display = document.getElementById('bulkTotalDisplay');
        if (display) display.innerText = total.toFixed(2);
    }

    // D. Submit Bulk Receive (Receive All)
    document.body.addEventListener('submit', function (e) {
        if (e.target.id === 'bulkReceiveForm') {
            e.preventDefault();

            const invoice = document.getElementById('bulkInvoice').value;
            const vendorIdVal = document.getElementById('bulkVendorId').value;

            if (!invoice) { alert("Invoice Number is required"); return; }
            if (!vendorIdVal || vendorIdVal == "0") { alert("Vendor ID missing."); return; }

            const items = [];
            let grandTotal = 0;

            document.querySelectorAll('.bulk-item-row').forEach(row => {
                const qty = parseFloat(row.querySelector('.bulk-qty').value) || 0;
                const price = parseFloat(row.querySelector('.bulk-price').value) || 0;

                if (qty > 0) {
                    items.push({
                        PoRequestId: parseInt(row.getAttribute('data-poid')),
                        ProductVariantId: parseInt(row.getAttribute('data-variantid')),
                        Quantity: qty,
                        Price: price
                    });
                    grandTotal += (qty * price);
                }
            });

            if (items.length === 0) { alert("No items selected."); return; }

            const payload = {
                InvoiceNo: invoice,
                Remarks: document.getElementById('bulkRemarks').value,
                PaymentMethodId: document.getElementById('bulkPaymentMethod').value ? parseInt(document.getElementById('bulkPaymentMethod').value) : null,
                TotalPaid: grandTotal,
                VendorId: parseInt(vendorIdVal),
                Items: items
            };

            const btn = e.target.querySelector('button[type="submit"]');
            const oldText = btn.innerText;
            btn.disabled = true;
            btn.innerText = "Processing...";

            fetch('/purchase/receive-bulk-stock', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        if (currentOpenedOrderId) openDetailsModal(currentOpenedOrderId);
                    } else {
                        alert(data.message);
                        btn.disabled = false;
                        btn.innerText = oldText;
                    }
                })
                .catch(err => {
                    console.error(err);
                    alert("Server Error");
                    btn.disabled = false;
                    btn.innerText = oldText;
                });
        }
    });

    // E. Reject All Remaining
    document.body.addEventListener('click', function (e) {
        if (e.target.closest('#btnRejectAll')) {
            e.preventDefault();
            if (!confirm("Are you sure you want to REJECT ALL remaining items?")) return;
            if (!currentOpenedOrderId) return;

            const btn = e.target.closest('#btnRejectAll');
            btn.disabled = true;
            btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';

            fetch('/purchase/reject-bulk-remaining', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ BulkOrderId: parseInt(currentOpenedOrderId) })
            })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        openDetailsModal(currentOpenedOrderId);
                    } else {
                        alert(data.message);
                        btn.disabled = false;
                        btn.innerHTML = '<i class="bi bi-x-circle"></i> Reject Remaining';
                    }
                });
        }
    });

    // =========================================================
    // 3. INDIVIDUAL ITEM ACTIONS (Receive/Reject Single)
    // =========================================================

    // A. Show Single Receive Row
    document.body.addEventListener('click', function (e) {
        const btn = e.target.closest('.js-btn-receive-toggle');
        if (btn) {
            e.preventDefault();
            const targetId = btn.getAttribute('data-target');
            const formRow = document.getElementById(targetId);
            const btnGroup = btn.parentElement;

            if (formRow) {
                formRow.classList.remove('d-none');
                btnGroup.classList.add('d-none');
            }
        }
    });

    // B. Cancel Single Receive
    document.body.addEventListener('click', function (e) {
        const btn = e.target.closest('.js-btn-cancel-receive');
        if (btn) {
            e.preventDefault();
            const targetId = btn.getAttribute('data-target');
            const formRow = document.getElementById(targetId);
            const poId = targetId.split('-')[1];
            const btnGroup = document.getElementById(`btn-group-${poId}`);

            if (formRow) formRow.classList.add('d-none');
            if (btnGroup) btnGroup.classList.remove('d-none');
        }
    });

    // C. Confirm Single Receive
    document.body.addEventListener('click', function (e) {
        const btn = e.target.closest('.js-btn-confirm-receive');
        if (btn) {
            e.preventDefault();
            const row = btn.closest('tr');

            const qtyVal = parseInt(row.querySelector('.js-input-qty').value);
            const priceVal = parseFloat(row.querySelector('.js-input-price').value);
            const invoice = row.querySelector('.js-input-invoice').value;
            const remarks = row.querySelector('.js-input-remarks').value;

            if (!qtyVal || qtyVal <= 0) { alert("Invalid Quantity"); return; }
            if (isNaN(priceVal) || priceVal < 0) { alert("Invalid Price"); return; }

            const payload = {
                ProductVariantId: parseInt(btn.getAttribute('data-variantid')),
                Quantity: qtyVal,
                BuyingPrice: priceVal,
                InvoiceNo: invoice,
                Remarks: remarks
            };

            const oldText = btn.innerText;
            btn.disabled = true;
            btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';

            fetch('/purchase/receive-stock', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        if (currentOpenedOrderId) openDetailsModal(currentOpenedOrderId);
                    } else {
                        alert(data.message);
                        btn.disabled = false;
                        btn.innerText = oldText;
                    }
                })
                .catch(err => {
                    console.error(err);
                    alert("Server Error");
                    btn.disabled = false;
                    btn.innerText = oldText;
                });
        }
    });

    // D. Reject Single Item
    document.body.addEventListener('click', function (e) {
        const btn = e.target.closest('.js-btn-reject');
        if (btn) {
            e.preventDefault();
            if (!confirm("Reject this item?")) return;

            const poId = parseInt(btn.getAttribute('data-poid'));
            btn.disabled = true;

            fetch('/purchase/reject-item', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ PoRequestId: poId })
            })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        if (currentOpenedOrderId) openDetailsModal(currentOpenedOrderId);
                    } else {
                        alert(data.message);
                        btn.disabled = false;
                    }
                });
        }
    });
});