document.addEventListener('DOMContentLoaded', function () {

    console.log("Admin Order Script Loaded");

    // ==========================================
    // UI ELEMENTS
    // ==========================================
    const productSelect = document.getElementById('productSelect');
    const variantSelect = document.getElementById('variantSelect');
    const qtyInput = document.getElementById('orderQty');

    // Calculation Display Elements
    const displayPrice = document.getElementById('displayPrice');
    const displaySubTotal = document.getElementById('displaySubTotal');
    const displayDiscount = document.getElementById('displayDiscount');
    const displayDelivery = document.getElementById('displayDelivery');
    const displayNet = document.getElementById('displayNet');

    const stockInfo = document.getElementById('stockInfo');

    // Mode & Location Elements
    const modeDelivery = document.getElementById('modeDelivery');
    const modeStore = document.getElementById('modeStore');
    const divisionSelect = document.getElementById('division-select');

    // Location Dropdowns
    const districtSelect = document.getElementById('district-select');
    const thanaSelect = document.getElementById('thana-select');
    const subOfficeSelect = document.getElementById('suboffice-select');
    const postalInput = document.getElementById('postalCode');
    const postalStatus = document.getElementById('postalStatus');

    // Validation State
    let isPostalValid = false;

    // ==========================================
    // 1. LOAD DATA & POPULATE PRODUCTS
    // ==========================================
    const allVariants = (typeof window.rawVariants !== 'undefined' && Array.isArray(window.rawVariants))
        ? window.rawVariants
        : [];

    const productMap = {};
    allVariants.forEach(v => {
        const pId = v.ProductId || v.productId;
        const pName = v.ProductName || v.productName;

        if (pId) {
            if (!productMap[pId]) {
                productMap[pId] = { name: pName, variants: [] };
                let opt = new Option(pName, pId);
                productSelect.add(opt);
            }
            productMap[pId].variants.push(v);
        }
    });

    // ==========================================
    // 2. CALCULATION LOGIC
    // ==========================================
    function calcTotal() {
        const option = variantSelect.options[variantSelect.selectedIndex];

        const price = parseFloat(option.getAttribute('data-price')) || 0;
        const qty = parseInt(qtyInput.value) || 1;

        const discType = option.getAttribute('data-disc-type') || "None";
        const discVal = parseFloat(option.getAttribute('data-disc-val')) || 0;

        const subTotal = price * qty;

        let discountAmount = 0;
        if (discType === "Flat") {
            discountAmount = discVal * qty;
        } else if (discType === "Percentage") {
            discountAmount = subTotal * (discVal / 100);
        }

        let deliveryCharge = 0;
        if (modeStore.checked) {
            deliveryCharge = 0;
        } else {
            const division = divisionSelect.value ? divisionSelect.value.toLowerCase() : "";
            if (division.includes("dhaka")) {
                deliveryCharge = 50;
            } else if (division !== "") {
                deliveryCharge = 100;
            } else {
                deliveryCharge = 0;
            }
        }

        const grandTotal = (subTotal - discountAmount) + deliveryCharge;

        displayPrice.textContent = price.toFixed(2);
        displaySubTotal.textContent = subTotal.toFixed(2);
        displayDiscount.textContent = discountAmount.toFixed(2);

        if (displayDelivery) displayDelivery.textContent = deliveryCharge.toFixed(2);

        displayNet.textContent = grandTotal.toFixed(2);
    }

    // ==========================================
    // 3. EVENT LISTENERS (Product & Variant)
    // ==========================================

    productSelect.addEventListener('change', function () {
        variantSelect.innerHTML = '<option value="" data-price="0">Select Variant...</option>';
        variantSelect.disabled = true;
        stockInfo.textContent = "";

        displayPrice.textContent = "0.00";
        displaySubTotal.textContent = "0.00";
        displayDiscount.textContent = "0.00";
        if (displayDelivery) displayDelivery.textContent = "0.00";
        displayNet.textContent = "0.00";

        const pId = this.value;
        if (pId && productMap[pId]) {
            productMap[pId].variants.forEach(v => {
                const vId = v.VariantId || v.variantId;
                const vName = v.VariantName || v.variantName;
                const vPrice = v.Price || v.price || 0;
                const vStock = v.Stock || v.stock || 0;

                const dType = v.DiscountType || v.discountType || "None";
                const dVal = v.DiscountValue || v.discountValue || 0;

                let opt = new Option(`${vName} (Tk. ${vPrice})`, vId);
                opt.setAttribute('data-price', vPrice);
                opt.setAttribute('data-stock', vStock);

                opt.setAttribute('data-disc-type', dType);
                opt.setAttribute('data-disc-val', dVal);

                variantSelect.add(opt);
            });
            variantSelect.disabled = false;
        }
    });

    variantSelect.addEventListener('change', function () {
        const option = this.options[this.selectedIndex];
        const stock = option.getAttribute('data-stock');

        if (stock) {
            if (parseInt(stock) === 999) {
                stockInfo.textContent = "In Stock";
                stockInfo.className = "form-text text-success";
            } else {
                stockInfo.textContent = `Available Stock: ${stock}`;
                stockInfo.className = parseInt(stock) < 5 ? "form-text text-danger fw-bold" : "form-text text-success";
            }
        } else {
            stockInfo.textContent = "";
        }
        calcTotal();
    });

    qtyInput.addEventListener('input', calcTotal);

    if (divisionSelect) divisionSelect.addEventListener('change', calcTotal);
    if (modeDelivery) modeDelivery.addEventListener('change', calcTotal);
    if (modeStore) modeStore.addEventListener('change', calcTotal);


    // ==========================================
    // 4. SHARED LOGIC INTEGRATION (Refactored)
    // ==========================================

    // Helper to use the OrderAPI promises from combo.js
    function loadAdminDropdown(apiPromise, targetSelect, selectedVal = null) {
        targetSelect.innerHTML = '<option value="">Loading...</option>';
        targetSelect.disabled = true;

        return apiPromise.then(data => {
            targetSelect.innerHTML = '<option value="">Select...</option>';

            if (Array.isArray(data)) {
                data.forEach(item => {
                    let val = item.name || item;
                    let opt = new Option(val, val);
                    // Handle code for autofill if present
                    if (item.code || item.postalCode || item.PostalCode) {
                        opt.setAttribute('data-code', item.code || item.postalCode || item.PostalCode);
                    }
                    targetSelect.add(opt);
                });
                targetSelect.disabled = false;
                if (selectedVal) targetSelect.value = selectedVal;
            }
        }).catch(() => {
            targetSelect.innerHTML = '<option value="">Error</option>';
        });
    }

    function invalidatePostal() {
        isPostalValid = false;
        postalStatus.textContent = "Location changed. Verify Postal Code.";
        postalStatus.className = "text-warning small";
    }

    // --- Init: Load Divisions using Shared API ---
    if (window.OrderAPI && window.OrderAPI.getDivisions) {
        window.OrderAPI.getDivisions().then(data => {
            if (Array.isArray(data)) data.forEach(d => { divisionSelect.add(new Option(d, d)); });
        });
    } else {
        console.error("OrderAPI missing. Ensure combo.js is loaded first.");
    }

    // --- Location Cascading Listeners ---
    divisionSelect.addEventListener('change', () => {
        invalidatePostal();
        loadAdminDropdown(window.OrderAPI.getDistricts(divisionSelect.value), districtSelect);
        thanaSelect.innerHTML = '<option value="">Select...</option>'; thanaSelect.disabled = true;
        subOfficeSelect.innerHTML = '<option value="">Select...</option>'; subOfficeSelect.disabled = true;
    });

    districtSelect.addEventListener('change', () => {
        invalidatePostal();
        loadAdminDropdown(window.OrderAPI.getThanas(districtSelect.value), thanaSelect);
    });

    thanaSelect.addEventListener('change', () => {
        invalidatePostal();
        loadAdminDropdown(window.OrderAPI.getSubOffices(thanaSelect.value), subOfficeSelect);
    });

    // --- Sub-Office Autofill ---
    subOfficeSelect.addEventListener('change', function () {
        const option = this.options[this.selectedIndex];
        const autoCode = option.getAttribute('data-code');
        if (autoCode) {
            postalInput.value = autoCode;
            postalInput.dispatchEvent(new Event('blur')); // Trigger validation
        } else {
            invalidatePostal();
            postalStatus.textContent = "Enter Postal Code manually.";
        }
    });

    // --- Postal Code Validation (Using Shared API) ---
    postalInput.addEventListener('blur', function () {
        const code = this.value.trim();
        if (code.length < 4) { isPostalValid = false; return; }

        postalStatus.textContent = "Checking...";
        postalStatus.className = "text-muted small";

        window.OrderAPI.checkPostalCode(code).then(async data => {
            if (data.found) {
                isPostalValid = true;
                postalStatus.textContent = "Location found!";
                postalStatus.className = "text-success small";

                const div = data.division || data.DivisionEn;
                const dist = data.district || data.DistrictEn;
                const th = data.thana || data.ThanaEn;
                const sub = data.subOffice || data.SubOfficeEn;

                // Auto-fill logic
                if (div && divisionSelect.value !== div) {
                    divisionSelect.value = div;
                    calcTotal();
                    await loadAdminDropdown(window.OrderAPI.getDistricts(div), districtSelect, dist);
                    await loadAdminDropdown(window.OrderAPI.getThanas(dist), thanaSelect, th);
                    await loadAdminDropdown(window.OrderAPI.getSubOffices(th), subOfficeSelect, sub);
                } else if (div && divisionSelect.value === div) {
                    if (dist && districtSelect.value !== dist) await loadAdminDropdown(window.OrderAPI.getDistricts(div), districtSelect, dist);
                    if (th && thanaSelect.value !== th) await loadAdminDropdown(window.OrderAPI.getThanas(dist), thanaSelect, th);
                    if (sub && subOfficeSelect.value !== sub) await loadAdminDropdown(window.OrderAPI.getSubOffices(th), subOfficeSelect, sub);
                }
            } else {
                isPostalValid = false;
                postalStatus.textContent = "Invalid Postal Code!";
                postalStatus.className = "text-danger fw-bold small";
            }
        });
    });

    // --- Customer Check (Using Shared API) ---
    const addressContainer = document.getElementById('address-container');
    const custAddress = document.getElementById('custAddress');
    const phoneInput = document.getElementById('custPhone');

    document.getElementById('btnCheckCust').addEventListener('click', () => {
        const phone = phoneInput.value.trim();
        if (phone.length < 10) return;

        window.OrderAPI.checkCustomer(phone).then(data => {
            if (data.found) {
                document.getElementById('custName').value = data.name || '';
                const emailEl = document.getElementById('custEmail');
                if (emailEl) emailEl.value = data.email || '';

                document.getElementById('custStatus').textContent = "Customer found!";
                document.getElementById('custStatus').className = "text-success small";

                if (data.addressData && modeDelivery.checked) {
                    custAddress.value = data.addressData.street || '';
                    if (data.addressData.postalCode) {
                        postalInput.value = data.addressData.postalCode;
                        setTimeout(() => { postalInput.dispatchEvent(new Event('blur')); }, 100);
                    } else if (data.addressData.divison) {
                        divisionSelect.value = data.addressData.divison;
                        calcTotal();
                        if (data.addressData.city) {
                            loadAdminDropdown(window.OrderAPI.getDistricts(data.addressData.divison), districtSelect, data.addressData.city);
                        }
                    }
                }
            } else {
                document.getElementById('custStatus').textContent = "New Customer";
                document.getElementById('custStatus').className = "text-primary small";
                document.getElementById('custName').value = '';
                custAddress.value = '';
            }
        });
    });

    // --- Mode Toggle ---
    function toggleMode() {
        addressContainer.style.display = modeStore.checked ? 'none' : 'block';
    }
    modeDelivery.addEventListener('change', toggleMode);
    modeStore.addEventListener('change', toggleMode);

    // ==========================================
    // 5. SUBMIT ORDER
    // ==========================================
    document.getElementById('btnPlaceOrder').addEventListener('click', function () {
        const btn = this;
        const msg = document.getElementById('orderMsg');
        const isStoreSale = modeStore.checked;

        if (!phoneInput.value || !document.getElementById('custName').value || !variantSelect.value) {
            alert("Please fill in Phone, Name, Product, and Variant.");
            return;
        }

        if (!isStoreSale) {
            if (!custAddress.value || !divisionSelect.value) {
                alert("Address and Division are required for Home Delivery.");
                return;
            }
            if (!isPostalValid) {
                alert("Please enter a valid Postal Code to continue.");
                postalInput.focus();
                return;
            }
        }

        const payload = {
            CustomerPhone: phoneInput.value,
            CustomerName: document.getElementById('custName').value,
            CustomerEmail: document.getElementById('custEmail') ? document.getElementById('custEmail').value : '',
            Street: isStoreSale ? "Counter Sale - In Store" : custAddress.value,
            Divison: isStoreSale ? "Dhaka" : divisionSelect.value,
            City: isStoreSale ? "Dhaka" : districtSelect.value
            Thana: isStoreSale ? "" : thanaSelect.value,
            SubOffice: isStoreSale ? "" : subOfficeSelect.value,
            PostalCode: isStoreSale ? "1000" : postalInput.value,
            ProductVariantId: parseInt(variantSelect.value),
            OrderQuantity: parseInt(qtyInput.value),
            TargetCompanyId: parseInt(document.getElementById('targetCompanyId').value) || 0,
            Confirmed: document.getElementById('confirmImmediately').checked
        };

        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
        btn.disabled = true;
        btn.textContent = "Processing...";
        msg.textContent = "";

        fetch('/order/place-direct', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
            body: JSON.stringify(payload)
        })
            .then(r => r.json()).then(data => {
                if (data.success) {
                    let finalAmount = data.netAmount;
                    let finalId = data.orderId;
                    if (typeof data.orderId === 'object' && data.orderId !== null) {
                        finalId = data.orderId.OrderId || data.orderId.orderId;
                    }

                    const modalHtml = `
            <div class="modal fade" id="orderSuccessModal" tabindex="-1" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
              <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content shadow-lg border-0">
                  <div class="modal-header bg-success text-white border-0">
                    <h5 class="modal-title fw-bold">✅ Order Placed!</h5>
                  </div>
                  <div class="modal-body text-center p-4">
                    <div class="display-1 mb-3">📦</div>
                    <h3 class="fw-bold text-dark mb-1">${finalId}</h3>
                    <p class="text-muted">has been successfully created.</p>
                  </div>
                  <div class="modal-footer justify-content-center border-0 pb-4">
                    <button type="button" class="btn btn-outline-secondary" onclick="window.location.reload()">Create Another</button>
                    <button type="button" class="btn btn-primary px-4" onclick="window.location.href='/Order/AllOrders'">Go to Orders</button>
                  </div>
                </div>
              </div>
            </div>`;

                    const existingModal = document.getElementById('orderSuccessModal');
                    if (existingModal) existingModal.remove();

                    document.body.insertAdjacentHTML('beforeend', modalHtml);
                    const modalEl = document.getElementById('orderSuccessModal');
                    const modal = new bootstrap.Modal(modalEl);
                    modal.show();

                } else {
                    msg.textContent = "Error: " + data.message;
                    msg.className = "mt-3 text-center small text-danger";
                    btn.disabled = false;
                    btn.textContent = "Create Direct Order";
                }
            }).catch(err => {
                msg.textContent = "Network Error";
                msg.className = "mt-3 text-center small text-danger";
                btn.disabled = false;
                btn.textContent = "Create Direct Order";
            });
    });
});