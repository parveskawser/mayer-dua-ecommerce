document.addEventListener('DOMContentLoaded', function () {

    // 1. Initialize Modals
    const reqModalEl = document.getElementById('requestModal');
    const infoModalEl = document.getElementById('infoModal');
    const receiveModalEl = document.getElementById('receiveModal');

    // Move modals to body to prevent stacking issues
    [reqModalEl, infoModalEl, receiveModalEl].forEach(el => {
        if (el && el.parentNode !== document.body) document.body.appendChild(el);
    });

    const requestModal = new bootstrap.Modal(reqModalEl);
    const infoModal = new bootstrap.Modal(infoModalEl);
    const receiveModal = new bootstrap.Modal(receiveModalEl);

    // ============================================================
    // 2. EVENT LISTENERS
    // ============================================================

    // A. REQUEST STOCK (Standard)
    document.querySelectorAll('.btn-request').forEach(btn => {
        btn.addEventListener('click', function () {
            document.getElementById('reqVariantId').value = this.dataset.variantId;
            document.getElementById('reqProductName').value = this.dataset.name;
            document.getElementById('reqQty').value = this.dataset.suggested;
            requestModal.show();
        });
    });

    // B. VIEW INFO (Pending Request)
    document.querySelectorAll('.btn-req-info').forEach(btn => {
        btn.addEventListener('click', function () {
            const variantId = this.dataset.variantId;
            const body = document.getElementById('infoModalBody');

            body.innerHTML = '<div class="text-center text-muted py-5"><div class="spinner-border text-info"></div><div class="mt-2">Loading details...</div></div>';
            infoModal.show();

            // Fetch Info
            fetch(`/purchase/get-pending-info?variantId=${variantId}`)
                .then(r => r.json())
                .then(res => {
                    if (res.success && res.data) {
                        const d = res.data;
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
        });
    });

    // C. RECEIVE STOCK
    document.querySelectorAll('.btn-receive').forEach(btn => {
        btn.addEventListener('click', function () {
            const variantId = this.dataset.variantId;
            const name = this.dataset.name;

            document.getElementById('recVariantId').value = variantId;
            document.getElementById('recProductName').textContent = name;

            // Clear previous inputs
            document.getElementById('recQty').value = "";
            document.getElementById('recPrice').value = "";
            document.getElementById('recInvoice').value = "";
            document.getElementById('recRemarks').value = "";
            document.getElementById('recReqQty').value = "Loading...";

            receiveModal.show();

            // Fetch requested qty to help the user
            fetch(`/purchase/get-pending-info?variantId=${variantId}`)
                .then(r => r.json())
                .then(res => {
                    if (res.success && res.data) {
                        const reqQty = res.data.quantity;
                        document.getElementById('recReqQty').value = reqQty;
                        // Auto-fill received qty for convenience
                        document.getElementById('recQty').value = reqQty;
                    } else {
                        document.getElementById('recReqQty').value = "N/A";
                    }
                });
        });
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

    // Helper: Generic Submit Function
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
                    if (typeof Swal !== 'undefined') {
                        Swal.fire('Success!', data.message || 'Operation Successful', 'success').then(() => location.reload());
                    } else {
                        alert(data.message || "Success!");
                        location.reload();
                    }
                    modalInstance.hide();
                } else {
                    throw new Error(data.message || "Unknown Error");
                }
            })
            .catch(err => {
                if (typeof Swal !== 'undefined') Swal.fire('Error', err.message, 'error');
                else alert("Error: " + err.message);
                btn.disabled = false;
                btn.textContent = originalText;
            });
    }
});