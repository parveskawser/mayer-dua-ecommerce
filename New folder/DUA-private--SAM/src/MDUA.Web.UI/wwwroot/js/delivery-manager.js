document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById('deliverySearch');
    const statusFilter = document.getElementById('statusFilter');

    const tableBody = document.querySelector('.table > tbody');

    function filterTable() {
        if (!tableBody) return;

        const searchTerm = (searchInput?.value || "").toLowerCase().trim();
        const statusTerm = statusFilter?.value || "";

        const rows = tableBody.children;

        for (let i = 0; i < rows.length; i++) {
            const row = rows[i];

            if (row.querySelector('td[colspan]')) continue;
            if (row.cells.length < 5) continue;

            const orderRef = row.cells[1].innerText.toLowerCase();
            const customer = row.cells[2].innerText.toLowerCase();
            const tracking = row.cells[3].innerText.toLowerCase();

            const matchesSearch =
                orderRef.includes(searchTerm) ||
                customer.includes(searchTerm) ||
                tracking.includes(searchTerm);

            const statusSelect = row.cells[4].querySelector('select');
            const currentStatus = statusSelect ? statusSelect.value : row.cells[4].innerText.trim();
            const matchesFilter = statusTerm === "" || currentStatus === statusTerm;

            const detailRow = row.nextElementSibling;
            const isDetailRow = detailRow && detailRow.querySelector('td[colspan]');

            if (matchesSearch && matchesFilter) {
                row.style.display = "";
                if (isDetailRow) detailRow.style.display = "";
            } else {
                row.style.display = "none";
                if (isDetailRow) detailRow.style.display = "none";
            }
        }
    }

    if (searchInput) searchInput.addEventListener('keyup', filterTable);
    if (statusFilter) statusFilter.addEventListener('change', filterTable);

    // ============================================================
    // Shipment Modal Wiring
    // ============================================================
    window.__shipmentState = {
        currentDeliveryId: null,
        currentSelectEl: null,
        bootstrapData: null
    };

    const modalEl = document.getElementById('shipmentModal');
    const modal = modalEl ? new bootstrap.Modal(modalEl) : null;

    const smError = document.getElementById('smError');
    const smDeliveryId = document.getElementById('smDeliveryId');
    const smCompanyCarrierId = document.getElementById('smCompanyCarrierId');
    const smWeightGrams = document.getElementById('smWeightGrams');
    const smOrderNumber = document.getElementById('smOrderNumber');
    const smRecipientName = document.getElementById('smRecipientName');
    const smRecipientPhone = document.getElementById('smRecipientPhone');
    const smDeliveryType = document.getElementById('smDeliveryType');
    const smItemQty = document.getElementById('smItemQty');
    const smAmountToCollect = document.getElementById('smAmountToCollect');
    const smRecipientAddress = document.getElementById('smRecipientAddress');
    const smSpecialInstruction = document.getElementById('smSpecialInstruction');
    const smItemsBody = document.getElementById('smItemsBody');
    const smSubmitBtn = document.getElementById('smSubmitBtn');
    const smTitle = document.getElementById('shipmentModalTitle');

    function showError(msg) {
        if (!smError) return;
        smError.textContent = msg || "Something went wrong.";
        smError.classList.remove("d-none");
    }

    function clearError() {
        if (!smError) return;
        smError.textContent = "";
        smError.classList.add("d-none");
    }

    function setSubmitting(isSubmitting) {
        if (!smSubmitBtn) return;
        smSubmitBtn.disabled = !!isSubmitting;
        smSubmitBtn.textContent = isSubmitting ? "Submitting..." : "Submit Order";
    }

    function escapeHtml(str) {
        if (str === null || str === undefined) return "";
        return String(str)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    function populateModal(data) {
        clearError();

        const modalDto = data.modal;
        const carriers = data.carriers || [];

        window.__shipmentState.bootstrapData = data;

        smDeliveryId.value = modalDto.deliveryId || modalDto.DeliveryId;
        smOrderNumber.value = modalDto.orderNumber || modalDto.OrderNumber || "";
        smRecipientName.value = modalDto.recipientName || modalDto.RecipientName || "";
        smRecipientPhone.value = modalDto.recipientPhone || modalDto.RecipientPhone || "";
        smRecipientAddress.value = modalDto.recipientAddress || modalDto.RecipientAddress || "";
        smAmountToCollect.value = (modalDto.amountToCollect ?? modalDto.AmountToCollect ?? 0);
        smItemQty.value = (modalDto.itemQuantity ?? modalDto.ItemQuantity ?? 0);
        smWeightGrams.value = (modalDto.packageWeightGrams ?? modalDto.PackageWeightGrams ?? "");

        // Special instruction is UI-only unless you wire it to backend later
        smSpecialInstruction.value = "";

        // Carriers dropdown
        smCompanyCarrierId.innerHTML = "";
        for (let i = 0; i < carriers.length; i++) {
            const c = carriers[i];
            const opt = document.createElement("option");
            opt.value = c.companyCarrierId || c.CompanyCarrierId;
            opt.textContent = c.carrierName || c.CarrierName;
            opt.setAttribute("data-requires-api", (c.requiresApi || c.RequiresApi) ? "1" : "0");
            smCompanyCarrierId.appendChild(opt);
        }

        const selected = modalDto.selectedCompanyCarrierId || modalDto.SelectedCompanyCarrierId;
        if (selected) smCompanyCarrierId.value = String(selected);

        // Title changes with carrier selection
        function syncTitle() {
            const sel = smCompanyCarrierId.options[smCompanyCarrierId.selectedIndex];
            const name = sel ? sel.textContent : "Courier";
            if (smTitle) smTitle.textContent = name + " Order";

            const requiresApi = sel && sel.getAttribute("data-requires-api") === "1";
            if (smSubmitBtn) smSubmitBtn.textContent = requiresApi ? "Submit Order" : "Mark as Shipped";
        }
        smCompanyCarrierId.onchange = syncTitle;
        syncTitle();

        // Items table
        const items = modalDto.items || modalDto.Items || [];
        if (!items || items.length === 0) {
            smItemsBody.innerHTML = `<tr><td colspan="2" class="text-muted text-center">No items</td></tr>`;
        } else {
            let html = "";
            for (let i = 0; i < items.length; i++) {
                const it = items[i];
                const name = it.productName || it.ProductName || "Item";
                const variant = it.variantName || it.VariantName || "";
                const qty = it.quantity || it.Quantity || 0;

                html += `<tr>
                    <td>${escapeHtml(name)}${variant ? `<div class="small text-muted">${escapeHtml(variant)}</div>` : ""}</td>
                    <td class="text-center">${qty}</td>
                </tr>`;
            }
            smItemsBody.innerHTML = html;
        }
    }

    async function fetchModalBootstrapData(deliveryId) {
        const url = window.shipmentEndpoints.modalBootstrapUrl + "?deliveryId=" + encodeURIComponent(deliveryId);
        const res = await fetch(url, { method: "GET", headers: { "Accept": "application/json" } });
        return await res.json();
    }

    // Called from cshtml inline onchange
    window.onDeliveryStatusChanged = async function (deliveryId, selectEl) {
        const newStatus = selectEl.value;
        const prevStatus = selectEl.getAttribute("data-prev") || "Pending";

        // Shipped -> open modal, do NOT update status yet
        if (newStatus === "Shipped") {
            // revert UI immediately to previous until successful shipment
            selectEl.value = prevStatus;

            window.__shipmentState.currentDeliveryId = deliveryId;
            window.__shipmentState.currentSelectEl = selectEl;

            try {
                const data = await fetchModalBootstrapData(deliveryId);
                if (!data.success) {
                    alert("Failed to load shipment modal: " + (data.message || "Unknown error"));
                    return;
                }

                populateModal(data);
                modal.show();
            } catch (e) {
                alert("Server error while loading shipment modal.");
            }

            return;
        }

        // Non-shipped status -> normal update
        $.post(window.shipmentEndpoints.updateStatusUrl, { deliveryId: deliveryId, status: newStatus })
            .done(function (response) {
                if (!response.success) {
                    alert("Failed: " + response.message);
                    selectEl.value = prevStatus;
                    return;
                }
                selectEl.setAttribute("data-prev", newStatus);
                filterTable();
            })
            .fail(function () {
                alert("Server error.");
                selectEl.value = prevStatus;
            });
    };

    if (smSubmitBtn) {
        smSubmitBtn.addEventListener("click", function () {
            clearError();
            setSubmitting(true);

            const deliveryId = parseInt(smDeliveryId.value, 10);
            const companyCarrierId = parseInt(smCompanyCarrierId.value, 10);

            $.post(window.shipmentEndpoints.shipUrl, {
                deliveryId: deliveryId,
                companyCarrierId: companyCarrierId
            })
                .done(function (resp) {
                    if (!resp.success) {
                        showError(resp.message || "Shipment failed.");
                        setSubmitting(false);
                        return;
                    }

                    // Update UI: set dropdown to shipped + store prev
                    const sel = window.__shipmentState.currentSelectEl;
                    if (sel) {
                        sel.value = "Shipped";
                        sel.setAttribute("data-prev", "Shipped");
                    }

                    // Update tracking number cell
                    const trk = document.getElementById("trk-" + deliveryId);
                    if (trk && resp.trackingNumber) trk.textContent = resp.trackingNumber;

                    setSubmitting(false);
                    modal.hide();
                    filterTable();
                })
                .fail(function () {
                    showError("Server error while submitting shipment.");
                    setSubmitting(false);
                });
        });
    }

    if (modalEl) {
        modalEl.addEventListener('hidden.bs.modal', function () {
            clearError();
            setSubmitting(false);
        });
    }
});
