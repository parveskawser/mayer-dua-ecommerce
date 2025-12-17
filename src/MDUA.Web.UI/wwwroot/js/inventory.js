document.addEventListener('DOMContentLoaded', function () {

    // ============================================================
    // 1. INITIALIZATION & SETUP
    // ============================================================

    // Initialize Modals
    const reqModalEl = document.getElementById('requestModal');
    const infoModalEl = document.getElementById('infoModal');
    const receiveModalEl = document.getElementById('receiveModal');

    // Move modals to body to prevent z-index/backdrop issues
    [reqModalEl, infoModalEl, receiveModalEl].forEach(el => {
        if (el && el.parentNode !== document.body) document.body.appendChild(el);
    });

    const requestModal = new bootstrap.Modal(reqModalEl);
    const infoModal = new bootstrap.Modal(infoModalEl);
    const receiveModal = new bootstrap.Modal(receiveModalEl);

    // ============================================================
    // 2. EVENT DELEGATION (Handles Clicks for Dynamic Rows)
    // ============================================================
    // We attach one listener to the Document. It detects clicks on our buttons.

    document.addEventListener('click', function (e) {

        // --- A. REQUEST STOCK BUTTON ---
        // Checks if the clicked element OR its parent has class .btn-request
        const reqBtn = e.target.closest('.btn-request');
        if (reqBtn) {
            document.getElementById('reqVariantId').value = reqBtn.dataset.variantId;
            document.getElementById('reqProductName').value = reqBtn.dataset.name;
            document.getElementById('reqQty').value = reqBtn.dataset.suggested;
            requestModal.show();
            return; // Stop processing
        }

        // --- B. RECEIVE STOCK BUTTON ---
        const recBtn = e.target.closest('.btn-receive');
        if (recBtn) {
            const variantId = recBtn.dataset.variantId;
            const name = recBtn.dataset.name;

            document.getElementById('recVariantId').value = variantId;
            document.getElementById('recProductName').textContent = name;

            // Reset Form Fields
            document.getElementById('recQty').value = "";
            document.getElementById('recPrice').value = "";
            document.getElementById('recInvoice').value = "";
            document.getElementById('recRemarks').value = "";
            document.getElementById('recReqQty').value = "Loading...";

            receiveModal.show();

            // Fetch info to autofill requested qty
            fetch(`/purchase/get-pending-info?variantId=${variantId}`)
                .then(r => r.json())
                .then(res => {
                    if (res.success && res.data) {
                        const reqQty = res.data.quantity;
                        document.getElementById('recReqQty').value = reqQty;
                        // Convenience: Auto-fill receive qty
                        document.getElementById('recQty').value = reqQty;
                    } else {
                        document.getElementById('recReqQty').value = "N/A";
                    }
                });
            return;
        }

        // --- C. INFO BUTTON ---
        const infoBtn = e.target.closest('.btn-req-info');
        if (infoBtn) {
            const variantId = infoBtn.dataset.variantId;
            const body = document.getElementById('infoModalBody');

            body.innerHTML = '<div class="text-center text-muted py-5"><div class="spinner-border text-info"></div><div class="mt-2">Loading details...</div></div>';
            infoModal.show();

            fetch(`/purchase/get-pending-info?variantId=${variantId}`)
                .then(r => r.json())
                .then(res => {
                    if (res.success && res.data) {
                        const d = res.data;
                        // Render Details
                        body.innerHTML = `
                            <div class="list-group list-group-flush">
                                <div class="list-group-item d-flex justify-content-between align-items-center p-3">
                                    <span class="text-muted">Request ID</span>
                                    <span class="fw-bold">PO #${d.id}</span>
                                </div>
                                <div class="list-group-item d-flex justify-content-between align-items-center p-3">
                                    <span class="text-muted">Vendor</span>
                                    <span class="fw-bold text-dark">${d.vendorName}</span>
                                </div>
                                <div class="list-group-item d-flex justify-content-between align-items-center p-3">
                                    <span class="text-muted">Requested Quantity</span>
                                    <span class="badge bg-warning text-dark fs-6">${d.quantity}</span>
                                </div>
                                <div class="list-group-item d-flex justify-content-between align-items-center p-3">
                                    <span class="text-muted">Request Date</span>
                                    <span>${d.requestDate}</span>
                                </div>
                                <div class="list-group-item p-3">
                                    <span class="text-muted d-block mb-2">Remarks</span>
                                    <div class="bg-light p-2 rounded border text-break text-secondary small">
                                        ${d.remarks || 'No remarks provided.'}
                                    </div>
                                </div>
                            </div>`;
                    } else {
                        body.innerHTML = `<div class="text-center text-danger py-4 p-3">${res.message || 'Data not found'}</div>`;
                    }
                })
                .catch(() => {
                    body.innerHTML = `<div class="text-center text-danger py-4">Failed to load data.</div>`;
                });
            return;
        }
    });

    // ============================================================
    // 3. SUBMIT ACTIONS
    // ============================================================

    // Submit REQUEST
    const btnConfirmReq = document.getElementById('btnConfirmRequest');
    if (btnConfirmReq) {
        btnConfirmReq.addEventListener('click', function () {
            submitData('/purchase/create-request', {
                VendorId: parseInt(document.getElementById('reqVendor').value),
                ProductVariantId: parseInt(document.getElementById('reqVariantId').value),
                Quantity: parseInt(document.getElementById('reqQty').value)
            }, this, requestModal);
        });
    }

    // Submit RECEIVE
    const btnConfirmRec = document.getElementById('btnConfirmReceive');
    if (btnConfirmRec) {
        btnConfirmRec.addEventListener('click', function () {
            const payload = {
                ProductVariantId: parseInt(document.getElementById('recVariantId').value),
                Quantity: parseInt(document.getElementById('recQty').value),
                BuyingPrice: parseFloat(document.getElementById('recPrice').value),
                InvoiceNo: document.getElementById('recInvoice').value,
                Remarks: document.getElementById('recRemarks').value
            };

            // Validation
            if (!payload.ProductVariantId || isNaN(payload.ProductVariantId)) { alert("System Error: Variant ID missing"); return; }
            if (!payload.Quantity || payload.Quantity < 1) { alert("Please enter a valid quantity."); return; }
            if (isNaN(payload.BuyingPrice) || payload.BuyingPrice < 0) { alert("Please enter a valid total cost."); return; }

            submitData('/purchase/receive-stock', payload, this, receiveModal);
        });
    }

    // ============================================================
    // 4. HELPER FUNCTIONS
    // ============================================================

    function submitData(url, payload, btn, modalInstance) {
        const originalText = btn.textContent;
        btn.disabled = true;
        btn.textContent = "Processing...";

        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        const headers = { 'Content-Type': 'application/json' };
        if (token) headers['RequestVerificationToken'] = token;

        fetch(url, { method: 'POST', headers: headers, body: JSON.stringify(payload) })
            .then(async response => {
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) return await response.json();
                throw new Error("Invalid Server Response");
            })
            .then(data => {
                if (data.success) {
                    modalInstance.hide();

                    // ✅ SUCCESS: Update the specific row without reloading
                    if (payload.ProductVariantId) {
                        refreshVariantRow(payload.ProductVariantId);
                    }

                    // Show Toast (Using SweetAlert if available)
                    if (typeof Swal !== 'undefined') {
                        const Toast = Swal.mixin({
                            toast: true,
                            position: 'top-end',
                            showConfirmButton: false,
                            timer: 3000,
                            timerProgressBar: true,
                            didOpen: (toast) => {
                                toast.addEventListener('mouseenter', Swal.stopTimer)
                                toast.addEventListener('mouseleave', Swal.resumeTimer)
                            }
                        });
                        Toast.fire({ icon: 'success', title: data.message || 'Success' });
                    } else {
                        alert(data.message || "Success!");
                    }

                } else {
                    throw new Error(data.message || "Unknown Error");
                }
            })
            .catch(err => {
                if (typeof Swal !== 'undefined') Swal.fire('Error', err.message, 'error');
                else alert("Error: " + err.message);
            })
            .finally(() => {
                btn.disabled = false;
                btn.textContent = originalText;
            });
    }

 

    //  REFRESH FUNCTION
    function refreshVariantRow(variantId) {
        const row = document.getElementById(`row-${variantId}`);
        if (!row) return;

        // Visual feedback
        row.style.transition = "opacity 0.3s";
        row.style.opacity = '0.4';

        fetch(`/purchase/get-variant-row?variantId=${variantId}`)
            .then(r => r.text())
            .then(html => {
                // 1. Replace the HTML
                row.outerHTML = html;

                // 2. Flash effect
                const newRow = document.getElementById(`row-${variantId}`);
                if (newRow) {
                    newRow.style.backgroundColor = "#e8f5e9";
                    setTimeout(() => {
                        newRow.style.backgroundColor = "";
                    }, 1000);
                }

                // 3. ✅ TRIGGER TOTAL UPDATE
                updateGroupTotal(variantId);
            })
            .catch(err => console.error("Auto-refresh failed:", err));
    }
    $('#receiveModal').on('show.bs.modal', function () {
        // 1. Get client local date
        const now = new Date();
        // 2. Format as YYYY-MM-DD manually using local parts
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        const localToday = `${year}-${month}-${day}`;

        // 3. Set value
        $('#recDate').val(localToday);
    });
    // ✅ NEW HELPER: Recalculates the Parent Group's Total Stock
    function updateGroupTotal(variantId) {
        // 1. Find the newly updated row
        const row = document.getElementById(`row-${variantId}`);
        if (!row) return;

        // 2. Find the container (the accordion div)
        const collapseDiv = row.closest('.collapse');
        if (!collapseDiv) return;

        // 3. Sum up all stock values in this group
        // We look for spans with IDs starting with "stock-"
        let newTotal = 0;
        const stockSpans = collapseDiv.querySelectorAll('[id^="stock-"]');
        stockSpans.forEach(span => {
            // Parse integer, default to 0 if NaN
            const val = parseInt(span.textContent.trim()) || 0;
            newTotal += val;
        });

        // 4. Find the Parent Row to update
        // We match the button's target ID to the collapseDiv ID
        const parentBtn = document.querySelector(`button[data-bs-target="#${collapseDiv.id}"]`);
        if (parentBtn) {
            const parentRow = parentBtn.closest('tr');
            const totalCell = parentRow.querySelector('.group-total-stock');

            if (totalCell) {
                // Animate the change
                totalCell.style.transition = "color 0.3s";
                totalCell.style.color = "#198754"; // Flash Green
                totalCell.textContent = newTotal;

                setTimeout(() => {
                    totalCell.style.color = ""; // Reset color
                }, 1000);
            }
        }
    }
});