document.addEventListener('DOMContentLoaded', function () {

    // ============================================================
    // 1. INITIALIZATION & SETUP (SAFE MODE)
    // ============================================================

    // We use 'let' so these variables exist even if the elements are missing
    let requestModal = null;
    let infoModal = null;
    let receiveModal = null;

    // Helper to safely initialize a modal
    function initSafeModal(id) {
        const el = document.getElementById(id);
        if (el) {
            // Move to body to prevent z-index issues
            if (el.parentNode !== document.body) document.body.appendChild(el);
            return new bootstrap.Modal(el);
        }
        return null;
    }

    // Initialize only if they exist (Fixes crash on Bulk Order page)
    requestModal = initSafeModal('requestModal');
    infoModal = initSafeModal('infoModal');
    receiveModal = initSafeModal('receiveModal');


    // ============================================================
    // 2. EVENT DELEGATION (Inventory Page Actions)
    // ============================================================

    document.addEventListener('click', function (e) {

        // --- A. REQUEST STOCK BUTTON ---
        const reqBtn = e.target.closest('.btn-request');
        if (reqBtn && requestModal) { // Check if modal exists
            document.getElementById('reqVariantId').value = reqBtn.dataset.variantId;
            document.getElementById('reqProductName').value = reqBtn.dataset.name;
            document.getElementById('reqQty').value = reqBtn.dataset.suggested;
            requestModal.show();
            return;
        }

        // --- B. RECEIVE STOCK BUTTON ---
        const recBtn = e.target.closest('.btn-receive');
        if (recBtn && receiveModal) {
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

            // Fetch info
            fetch(`/purchase/get-pending-info?variantId=${variantId}`)
                .then(r => r.json())
                .then(res => {
                    const reqQtyField = document.getElementById('recReqQty');
                    if (reqQtyField) {
                        if (res.success && res.data) {
                            reqQtyField.value = res.data.quantity;
                            document.getElementById('recQty').value = res.data.quantity;
                        } else {
                            reqQtyField.value = "N/A";
                        }
                    }
                });
            return;
        }

        // --- C. INFO BUTTON ---
        const infoBtn = e.target.closest('.btn-req-info');
        if (infoBtn && infoModal) {
            const variantId = infoBtn.dataset.variantId;
            const body = document.getElementById('infoModalBody');

            if (body) {
                body.innerHTML = '<div class="text-center text-muted py-5"><div class="spinner-border text-info"></div><div class="mt-2">Loading details...</div></div>';
                infoModal.show();

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
            }
            return;
        }
    });

    // ============================================================
    // 3. SUBMIT ACTIONS (Inventory Page)
    // ============================================================

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
                    if (modalInstance) modalInstance.hide(); // Hide only if modal exists

                    if (payload.ProductVariantId) {
                        refreshVariantRow(payload.ProductVariantId);
                    }

                    if (typeof Swal !== 'undefined') {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: data.message || 'Success',
                            timer: 2000,
                            showConfirmButton: false
                        });
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

    function refreshVariantRow(variantId) {
        const row = document.getElementById(`row-${variantId}`);
        if (!row) return;

        row.style.transition = "opacity 0.3s";
        row.style.opacity = '0.4';

        fetch(`/purchase/get-variant-row?variantId=${variantId}`)
            .then(r => r.text())
            .then(html => {
                row.outerHTML = html;
                const newRow = document.getElementById(`row-${variantId}`);
                if (newRow) {
                    newRow.style.backgroundColor = "#e8f5e9";
                    setTimeout(() => newRow.style.backgroundColor = "", 1000);
                }
                updateGroupTotal(variantId);
            })
            .catch(err => console.error("Auto-refresh failed:", err));
    }

    function updateGroupTotal(variantId) {
        const row = document.getElementById(`row-${variantId}`);
        if (!row) return;
        const collapseDiv = row.closest('.collapse');
        if (!collapseDiv) return;

        let newTotal = 0;
        const stockSpans = collapseDiv.querySelectorAll('[id^="stock-"]');
        stockSpans.forEach(span => {
            const val = parseInt(span.textContent.trim()) || 0;
            newTotal += val;
        });

        const parentBtn = document.querySelector(`button[data-bs-target="#${collapseDiv.id}"]`);
        if (parentBtn) {
            const parentRow = parentBtn.closest('tr') || parentBtn.closest('.card-header'); // Handle both Table and Accordion layouts
            if (parentRow) {
                // Try to find the total cell (Table layout) or badge (Accordion layout)
                const totalCell = parentRow.querySelector('.group-total-stock') || parentRow.querySelector('b');
                if (totalCell) {
                    totalCell.style.transition = "color 0.3s";
                    totalCell.style.color = "#198754";
                    totalCell.textContent = newTotal;
                    setTimeout(() => totalCell.style.color = "", 1000);
                }
            }
        }
    }

    // ============================================================
    // 5. BULK ORDER PAGE LOGIC
    // ============================================================

    // --- A. SEARCH FUNCTIONALITY (FIXED) ---
    // --- A. SEARCH FUNCTIONALITY (ENHANCED) ---
    const searchInput = document.getElementById('productSearch');
    if (searchInput) {
        searchInput.addEventListener('keyup', function (e) {
            const term = e.target.value.toLowerCase().trim();
            const cards = document.querySelectorAll('.product-card');

            cards.forEach(card => {
                const productName = (card.dataset.searchTerm || "").toLowerCase();
                const variantRows = card.querySelectorAll('.variant-row');

                // 1. Check if Product Name matches
                const isProductMatch = productName.includes(term);

                let hasVisibleVariant = false;

                // 2. Loop through variants to check and filter them
                variantRows.forEach(row => {
                    // If product matches, always show the variant (reset previous filters)
                    if (isProductMatch) {
                        row.classList.remove('d-none');
                        hasVisibleVariant = true;
                    } else {
                        // If product doesn't match, check if the variant row text matches
                        // We use .textContent to search name, stock, remarks, etc.
                        const variantText = row.textContent.toLowerCase();

                        if (variantText.includes(term)) {
                            row.classList.remove('d-none');
                            hasVisibleVariant = true;
                        } else {
                            row.classList.add('d-none');
                        }
                    }
                });

                // 3. Show Card if Product matches OR at least one variant matches
                if (isProductMatch || hasVisibleVariant) {
                    card.classList.remove('d-none');
                } else {
                    card.classList.add('d-none');
                }
            });
        });
    }

    // --- B. CHECKBOX INTERACTION ---
    document.body.addEventListener('change', function (e) {
        if (e.target.classList.contains('variant-checkbox')) {
            const checkbox = e.target;
            const row = checkbox.closest('tr');
            const inputs = row.querySelectorAll('.input-vendor, .input-qty, .input-remarks');

            if (checkbox.checked) {
                row.classList.add('table-primary');
                inputs.forEach(input => input.disabled = false);
            } else {
                row.classList.remove('table-primary');
                inputs.forEach(input => input.disabled = true);
            }
        }
    });

    // --- C. BULK SUBMIT ACTION ---
    const btnSubmitBulk = document.getElementById('btnSubmitBulk');
    if (btnSubmitBulk) {
        btnSubmitBulk.addEventListener('click', function () {

            const selectedCheckboxes = document.querySelectorAll('.variant-checkbox:checked');

            if (selectedCheckboxes.length === 0) {
                alert("Please select at least one item to order.");
                return;
            }

            const payload = [];
            let hasError = false;

            selectedCheckboxes.forEach(chk => {
                const row = chk.closest('tr');
                const variantId = chk.dataset.variantId;
                const vendorId = row.querySelector('.input-vendor').value;
                const qty = row.querySelector('.input-qty').value;
                const remarks = row.querySelector('.input-remarks').value;

                if (!vendorId || !qty || qty <= 0) {
                    hasError = true;
                    row.classList.add('table-danger');
                    setTimeout(() => row.classList.remove('table-danger'), 2000);
                } else {
                    payload.push({
                        ProductVariantId: parseInt(variantId),
                        VendorId: parseInt(vendorId),
                        Quantity: parseInt(qty),
                        Remarks: remarks
                    });
                }
            });

            if (hasError) {
                alert("Please select a Vendor and valid Quantity for all checked items.");
                return;
            }

            const originalText = btnSubmitBulk.innerHTML;
            btnSubmitBulk.disabled = true;
            btnSubmitBulk.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Processing...';

            fetch('/purchase/bulk-create', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            })
                .then(r => r.json())
                .then(data => {
                    if (data.success) {
                        if (typeof Swal !== 'undefined') {
                            Swal.fire({
                                icon: 'success', title: 'Order Placed!', text: data.message, confirmButtonText: 'OK'
                            }).then(() => window.location.reload());
                        } else {
                            alert(data.message);
                            window.location.reload();
                        }
                    } else {
                        alert("Error: " + data.message);
                    }
                })
                .catch(err => alert("System Error: " + err.message))
                .finally(() => {
                    btnSubmitBulk.disabled = false;
                    btnSubmitBulk.innerHTML = originalText;
                });
        });
    }

});