document.addEventListener("DOMContentLoaded", function () {

    const ccBody = document.getElementById("ccBody");
    const ccAlert = document.getElementById("ccAlert");
    const btnRefresh = document.getElementById("btnRefresh");

    // Modal Elements
    const modalEl = document.getElementById("ccModal");
    const modal = modalEl ? new bootstrap.Modal(modalEl) : null;
    const ccModalTitle = document.getElementById("ccModalTitle");
    const ccModalError = document.getElementById("ccModalError");

    // Modal Inputs
    const ccCarrierId = document.getElementById("ccCarrierId");
    const ccCarrierName = document.getElementById("ccCarrierName");
    const ccApiEndpoint = document.getElementById("ccApiEndpoint");
    const ccApiKey = document.getElementById("ccApiKey");
    const ccApiSecret = document.getElementById("ccApiSecret");
    const ccApiUsername = document.getElementById("ccApiUsername");
    const ccApiPassword = document.getElementById("ccApiPassword");
    const ccStoreId = document.getElementById("ccStoreId");
    const ccIsActive = document.getElementById("ccIsActive"); // Modal checkbox (optional use)
    const ccSaveBtn = document.getElementById("ccSaveBtn");

    // Pathao Container
    const ccPathaoFields = document.getElementById("ccPathaoFields");

    let currentRows = [];
    let busy = false;

    // --- Helpers ---

    function showTopError(msg) {
        ccAlert.textContent = msg || "Something went wrong.";
        ccAlert.classList.remove("d-none");
        setTimeout(() => ccAlert.classList.add("d-none"), 5000);
    }

    function clearTopError() {
        ccAlert.textContent = "";
        ccAlert.classList.add("d-none");
    }

    function showModalError(msg) {
        ccModalError.textContent = msg || "Something went wrong.";
        ccModalError.classList.remove("d-none");
    }

    function clearModalError() {
        ccModalError.textContent = "";
        ccModalError.classList.add("d-none");
    }

    function esc(s) {
        if (s === null || s === undefined) return "";
        return String(s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;")
            .replace(/>/g, "&gt;").replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    function normRow(r) {
        return {
            carrierId: r.carrierId ?? r.CompanyCarrierId ?? 0, // Fallback for case sensitivity
            carrierName: r.carrierName ?? r.CarrierName ?? "",
            apiEndpoint: r.apiEndpoint ?? r.ApiEndpoint ?? "",
            requiresApi: (r.requiresApi ?? r.RequiresApi) ? true : false,
            isActive: (r.isActive ?? r.IsActive) ? true : false,
            apiKeyMasked: r.apiKeyMasked ?? r.ApiKeyMasked ?? "—",
            apiSecretMasked: r.apiSecretMasked ?? r.ApiSecretMasked ?? "—"
        };
    }

    function isPathaoName(name) {
        return (name || "").trim().toLowerCase().includes("pathao");
    }

    function setPathaoFieldsVisible(visible) {
        if (!ccPathaoFields) return;

        if (visible) {
            ccPathaoFields.classList.remove("d-none");
        } else {
            ccPathaoFields.classList.add("d-none");
        }

        // Disable inputs when hidden to prevent accidental submission
        if (ccApiUsername) ccApiUsername.disabled = !visible;
        if (ccApiPassword) ccApiPassword.disabled = !visible;
        if (ccStoreId) ccStoreId.disabled = !visible;
    }

    function setRowBusy(carrierId, isBusy) {
        const tr = ccBody.querySelector(`tr[data-carrier-id="${carrierId}"]`);
        if (!tr) return;

        const inputs = tr.querySelectorAll("input, button");
        inputs.forEach(el => el.disabled = isBusy);
        tr.style.opacity = isBusy ? "0.6" : "1";
    }

    async function safeJson(res) {
        try { return await res.json(); }
        catch { return { success: false, message: "Invalid server response." }; }
    }

    // --- Core Logic ---

    async function loadRows() {
        if (busy) return;
        busy = true;

        clearTopError();
        ccBody.innerHTML = `<tr><td colspan="6" class="text-center text-muted py-4">Loading...</td></tr>`;

        try {
            const res = await fetch(window.ccEndpoints.loadUrl, { method: "GET" });
            const json = await safeJson(res);

            if (!json.success) {
                ccBody.innerHTML = `<tr><td colspan="6" class="text-center text-danger py-4">${esc(json.message)}</td></tr>`;
                return;
            }

            currentRows = (json.rows || []).map(normRow);

            if (currentRows.length === 0) {
                ccBody.innerHTML = `<tr><td colspan="6" class="text-center text-muted py-4">No couriers found.</td></tr>`;
                return;
            }

            let html = "";
            for (const r of currentRows) {
                const activeChecked = r.isActive ? "checked" : "";
                const hasKey = (r.apiKeyMasked && r.apiKeyMasked !== "—");
                const hasSecret = (r.apiSecretMasked && r.apiSecretMasked !== "—");

                html += `
                <tr data-carrier-id="${r.carrierId}">
                    <td class="fw-bold">${esc(r.carrierName)}</td>
                    <td class="small text-muted text-truncate" style="max-width: 200px;">
                        ${esc(r.apiEndpoint)}
                    </td>
                    
                    <td class="text-center">
                        <div class="form-check form-switch d-inline-block">
                            <input class="form-check-input cursor-pointer cc-active-toggle" type="checkbox" 
                                   ${activeChecked} data-carrier-id="${r.carrierId}">
                        </div>
                    </td>

                    <td class="text-center h5 mb-0">
                        ${hasKey ? '<i class="fa fa-check-circle text-success" title="Configured"></i>' : '<span class="text-muted small">-</span>'}
                    </td>

                    <td class="text-center h5 mb-0">
                        ${hasSecret ? '<i class="fa fa-check-circle text-success" title="Configured"></i>' : '<span class="text-muted small">-</span>'}
                    </td>

                    <td class="text-end">
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-info" data-action="test" data-carrier-id="${r.carrierId}">
                                <i class="fa fa-plug"></i> Test
                            </button>
                            <button class="btn btn-outline-primary" data-action="edit" data-carrier-id="${r.carrierId}">
                                <i class="fa fa-cog"></i> Config
                            </button>
                        </div>
                    </td>
                </tr>`;
            }

            ccBody.innerHTML = html;
            attachEventListeners();

        } catch (e) {
            ccBody.innerHTML = `<tr><td colspan="6" class="text-center text-danger py-4">Network Error</td></tr>`;
            showTopError(e.message);
        } finally {
            busy = false;
        }
    }

    function attachEventListeners() {
        // Edit Button
        ccBody.querySelectorAll('button[data-action="edit"]').forEach(btn => {
            btn.addEventListener("click", function () {
                const id = parseInt(this.getAttribute("data-carrier-id"), 10);
                openEditModal(id);
            });
        });

        // Test Button
        ccBody.querySelectorAll('button[data-action="test"]').forEach(btn => {
            btn.addEventListener("click", function () {
                const id = parseInt(this.getAttribute("data-carrier-id"), 10);
                testConnection(id);
            });
        });

        // Toggle Switch
        ccBody.querySelectorAll('.cc-active-toggle').forEach(toggle => {
            toggle.addEventListener("change", function () {
                const id = parseInt(this.getAttribute("data-carrier-id"), 10);
                const isActive = this.checked;
                toggleActive(id, isActive);
            });
        });
    }

    // --- Modal Logic ---

    function openEditModal(carrierId) {
        clearModalError();
        const r = currentRows.find(x => x.carrierId === carrierId);
        if (!r) {
            showTopError("Courier data not found.");
            return;
        }

        ccCarrierId.value = carrierId;
        ccCarrierName.value = r.carrierName;
        ccApiEndpoint.value = r.apiEndpoint;
        ccIsActive.checked = !!r.isActive;

        // Clear secrets
        ccApiKey.value = "";
        ccApiSecret.value = "";
        if (ccApiUsername) ccApiUsername.value = "";
        if (ccApiPassword) ccApiPassword.value = "";
        if (ccStoreId) ccStoreId.value = "";

        // Toggle Pathao fields
        setPathaoFieldsVisible(isPathaoName(r.carrierName));

        if (ccModalTitle) ccModalTitle.textContent = `Update: ${r.carrierName}`;
        modal.show();
    }

    async function saveCredential() {
        clearModalError();
        ccSaveBtn.disabled = true;
        ccSaveBtn.textContent = "Saving...";

        const carrierName = (ccCarrierName.value || "").trim();
        const isPathao = isPathaoName(carrierName);

        const payload = {
            carrierId: parseInt(ccCarrierId.value, 10),
            isActive: ccIsActive.checked,
            apiKey: (ccApiKey.value || "").trim(),
            apiSecret: (ccApiSecret.value || "").trim(),

            // Pathao extras
            apiUsername: (isPathao && ccApiUsername) ? ccApiUsername.value.trim() : "",
            apiPassword: (isPathao && ccApiPassword) ? ccApiPassword.value.trim() : "",
            storeId: (isPathao && ccStoreId && ccStoreId.value) ? parseInt(ccStoreId.value, 10) : null
        };

        try {
            const res = await fetch(window.ccEndpoints.saveUrl, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            });
            const json = await safeJson(res);

            if (!json.success) {
                showModalError(json.message || "Save failed.");
            } else {
                modal.hide();
                await loadRows();
            }
        } catch (e) {
            showModalError("Network error during save.");
        } finally {
            ccSaveBtn.disabled = false;
            ccSaveBtn.textContent = "Save Changes";
        }
    }

    // --- Toggle Logic ---

    async function toggleActive(carrierId, isActive) {
        setRowBusy(carrierId, true);

        // We use a FormData approach or specialized endpoint if defined, 
        // but here reusing SaveCredential endpoint logic or a specialized one.
        // Assuming we use the dedicated toggle endpoint defined in CSHTML.

        try {
            const formData = new FormData();
            formData.append("companyCarrierId", carrierId);
            formData.append("isActive", isActive);

            const res = await fetch(window.ccEndpoints.toggleUrl, {
                method: "POST",
                body: formData
            });
            const json = await safeJson(res);

            if (!json.success) {
                // Revert UI
                const toggle = ccBody.querySelector(`.cc-active-toggle[data-carrier-id="${carrierId}"]`);
                if (toggle) toggle.checked = !isActive;
                showTopError(json.message);
            } else {
                // Update local model
                const r = currentRows.find(x => x.carrierId === carrierId);
                if (r) r.isActive = isActive;
            }

        } catch (e) {
            // Revert UI
            const toggle = ccBody.querySelector(`.cc-active-toggle[data-carrier-id="${carrierId}"]`);
            if (toggle) toggle.checked = !isActive;
            showTopError("Network error toggling status.");
        } finally {
            setRowBusy(carrierId, false);
        }
    }

    // --- Test Logic ---

    async function testConnection(carrierId) {
        setRowBusy(carrierId, true);
        const originalBtn = ccBody.querySelector(`button[data-action="test"][data-carrier-id="${carrierId}"]`);
        const originalHtml = originalBtn ? originalBtn.innerHTML : "Test";

        if (originalBtn) originalBtn.innerHTML = `<i class="fa fa-spinner fa-spin"></i>`;

        try {
            const formData = new FormData();
            formData.append("companyCarrierId", carrierId);

            const res = await fetch(window.ccEndpoints.testUrl, {
                method: "POST",
                body: formData
            });
            const json = await safeJson(res);

            if (json.success) {
                alert(`✅ Success: ${json.message}`);
            } else {
                alert(` Failed: ${json.message}`);
            }

        } catch (e) {
            alert("❌ Network error during test.");
        } finally {
            setRowBusy(carrierId, false);
            if (originalBtn) originalBtn.innerHTML = originalHtml;
        }
    }

    // --- Init ---
    if (btnRefresh) btnRefresh.addEventListener("click", loadRows);
    if (ccSaveBtn) ccSaveBtn.addEventListener("click", saveCredential);

    loadRows();
});