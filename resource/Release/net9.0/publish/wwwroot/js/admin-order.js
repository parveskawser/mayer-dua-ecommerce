document.addEventListener('DOMContentLoaded', function () {

    console.log("Admin Order Script Loaded");

    // Helper: Debounce function
    function debounce(func, wait) {
        let timeout;
        return function (...args) {
            const context = this;
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(context, args), wait);
        };
    }

    // ==========================================
    // 1. UI ELEMENTS
    // ==========================================
    const productSelect = document.getElementById('productSelect');
    const variantSelect = document.getElementById('variantSelect');
    const qtyInput = document.getElementById('orderQty');

    // Calculation Display Elements
    const displayPrice = document.getElementById('displayPrice');
    const displaySubTotal = document.getElementById('displaySubTotal');
    const displayDiscount = document.getElementById('displayDiscount');

    // ✅ CHANGED: Reference the Input field for Delivery Charge
    const deliveryInput = document.getElementById('deliveryChargeInput');
    // We keep this purely to update any text display if it still exists, though the input shows the value now
    const displayDelivery = document.getElementById('displayDelivery');

    const displayNet = document.getElementById('displayNet');
    const stockInfo = document.getElementById('stockInfo');

    // Mode & Location Elements
    const modeDelivery = document.getElementById('modeDelivery');
    const modeStore = document.getElementById('modeStore');
    const divisionSelect = document.getElementById('division-select');
    const districtSelect = document.getElementById('district-select');
    const thanaSelect = document.getElementById('thana-select');
    const subOfficeSelect = document.getElementById('suboffice-select');
    const postalInput = document.getElementById('postalCode');
    const postalStatus = document.getElementById('postalStatus');

    let isPostalValid = false;

    // Check if OrderAPI is loaded (from combo.js)
    if (typeof window.OrderAPI === 'undefined') {
        console.error("OrderAPI is missing! Make sure combo.js is loaded before admin-order.js");
        return;
    }

    // ✅ 1. INITIALIZE COUNTRY CODE INPUT
    const phoneInput = document.getElementById('custPhone');
    const iti = window.intlTelInput(phoneInput, {
        initialCountry: "bd",
        separateDialCode: true,
        preferredCountries: ["bd", "us", "gb"],
        utilsScript: "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.19/js/utils.js"
    });

    // ==========================================
    // 2. PRODUCT LOGIC
    // ==========================================
    const allVariants = (typeof window.rawVariants !== 'undefined' && Array.isArray(window.rawVariants)) ? window.rawVariants : [];
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

    productSelect.addEventListener('change', function () {
        variantSelect.innerHTML = '<option value="" data-price="0">Select Variant...</option>';
        variantSelect.disabled = true;
        stockInfo.textContent = "";
        calcTotal();

        const pId = this.value;
        if (pId && productMap[pId]) {
            productMap[pId].variants.forEach(v => {
                const vId = v.VariantId || v.variantId;
                const vName = v.VariantName || v.variantName;
                const vPrice = v.Price || v.price;
                const vStock = parseInt(v.Stock || v.stock || 0);

                // Discount Logic
                const dType = v.DiscountType || v.discountType || "None";
                const dVal = v.DiscountValue || v.discountValue || 0;

                // Check Stock Status
                let label = `${vName} (Tk. ${vPrice})`;
                let isOutOfStock = vStock <= 0;

                if (isOutOfStock) {
                    label += " ❌";
                } else if (vStock < 10 && vStock !== 999) {
                    label += ` (${vStock} left)`;
                }

                let opt = new Option(label, vId);
                opt.setAttribute('data-price', vPrice);
                opt.setAttribute('data-stock', vStock);
                opt.setAttribute('data-disc-type', dType);
                opt.setAttribute('data-disc-val', dVal);

                if (isOutOfStock) {
                    opt.disabled = true;
                    opt.style.color = "#dc3545"; // Red color
                    opt.style.fontWeight = "bold";
                }

                variantSelect.add(opt);
            });
            variantSelect.disabled = false;
        }
    });

    variantSelect.addEventListener('change', function () {
        const option = this.options[this.selectedIndex];
        const stock = parseInt(option.getAttribute('data-stock')) || 0;

        if (stock === 999) {
            stockInfo.textContent = "In Stock";
            stockInfo.className = "form-text text-success";
        } else {
            if (stock <= 0) {
                stockInfo.textContent = "Out of Stock";
                stockInfo.className = "form-text text-danger fw-bold";
            } else {
                stockInfo.textContent = `Available: ${stock}`;
                stockInfo.className = stock < 5 ? "form-text text-danger fw-bold" : "form-text text-success";
            }
        }
        calcTotal();
    });

    qtyInput.addEventListener('input', calcTotal);

    // ✅ NEW: Listen for manual delivery charge edits to recalculate Grand Total
    if (deliveryInput) {
        deliveryInput.addEventListener('input', calcTotal);
        if (!deliveryInput.value) deliveryInput.value = 0;
    }

    // ==========================================
    // 2.5 CALCULATION LOGIC
    // ==========================================

    // ✅ NEW FUNCTION: Calculates suggested delivery fee based on rules and updates the Input
    function autoCalculateDelivery() {
        if (!deliveryInput) return;

        if (modeStore && modeStore.checked) {
            deliveryInput.value = 0;
        } else {
            const config = window.deliveryConfig || { dhaka: 0, outside: 0 };
            const costDhaka = parseFloat(config.dhaka || config.Cost_InsideDhaka || 0);
            const costOutside = parseFloat(config.outside || config.Cost_OutsideDhaka || 0);

            const div = divisionSelect.value ? divisionSelect.value.toLowerCase() : "";
            const dist = districtSelect.value ? districtSelect.value.toLowerCase() : "";

            // Logic: Dhaka Division OR Dhaka District = Inside City
            if (div.includes("dhaka") || dist.includes("dhaka")) {
                deliveryInput.value = costDhaka;
            } else if (div !== "") {
                deliveryInput.value = costOutside;
            } else {
                deliveryInput.value = 0;
            }
        }
        // After setting the auto value, calculate the totals
        calcTotal();
    }

    function calcTotal() {
        // Safety check
        if (variantSelect.selectedIndex < 0) return;

        const option = variantSelect.options[variantSelect.selectedIndex];
        const price = parseFloat(option.getAttribute('data-price')) || 0;
        const qty = parseInt(qtyInput.value) || 1;
        const discType = option.getAttribute('data-disc-type');
        const discVal = parseFloat(option.getAttribute('data-disc-val')) || 0;

        const subTotal = price * qty;
        let discount = 0;
        if (discType === "Flat") discount = discVal * qty;
        else if (discType === "Percentage") discount = subTotal * (discVal / 100);

        // ✅ FIXED: Read from the Input field (allows manual override)
        let delivery = 0;
        if (deliveryInput) {
            delivery = parseFloat(deliveryInput.value) || 0;
        } else if (displayDelivery) {
            // Fallback for old UI logic if input is missing
            delivery = parseFloat(displayDelivery.textContent) || 0;
        }

        const net = (subTotal - discount) + delivery;

        displayPrice.textContent = price.toFixed(2);
        displaySubTotal.textContent = subTotal.toFixed(2);
        displayDiscount.textContent = discount.toFixed(2);

        // Update display text if element exists
        if (displayDelivery) displayDelivery.textContent = delivery.toFixed(2);

        displayNet.textContent = net.toFixed(2);
    }

    // ==========================================
    // 3. LOCATION LOGIC (Using OrderAPI)
    // ==========================================

    // Helper to load data from Promise
    async function loadFromApi(apiPromise, targetSelect, selectedValueToSet = null) {
        targetSelect.innerHTML = '<option value="">Loading...</option>';
        targetSelect.disabled = true;

        try {
            const data = await apiPromise;
            targetSelect.innerHTML = '<option value="">Select...</option>';

            if (Array.isArray(data)) {
                data.forEach(item => {
                    let val = item.name || item;
                    let opt = new Option(val, val);
                    if (item.code || item.postalCode) opt.setAttribute('data-code', item.code || item.postalCode);
                    targetSelect.add(opt);
                });
                targetSelect.disabled = false;
                if (selectedValueToSet) targetSelect.value = selectedValueToSet;
            }
        } catch (e) {
            targetSelect.innerHTML = '<option value="">Error</option>';
        }
    }

    // Init Divisions
    loadFromApi(window.OrderAPI.getDivisions(), divisionSelect);

    // Cascading Logic
    divisionSelect.addEventListener('change', () => {
        loadFromApi(window.OrderAPI.getDistricts(divisionSelect.value), districtSelect)
            .then(() => {
                thanaSelect.innerHTML = '<option value="">Select...</option>'; thanaSelect.disabled = true;
                subOfficeSelect.innerHTML = '<option value="">Select...</option>'; subOfficeSelect.disabled = true;
                // ✅ Changed: Recalculate fee when Division changes
                autoCalculateDelivery();
            });
    });

    districtSelect.addEventListener('change', () => {
        loadFromApi(window.OrderAPI.getThanas(districtSelect.value), thanaSelect);
        // ✅ Changed: Recalculate fee when District changes (e.g. Gazipur -> Dhaka)
        autoCalculateDelivery();
    });

    thanaSelect.addEventListener('change', () => {
        loadFromApi(window.OrderAPI.getSubOffices(thanaSelect.value), subOfficeSelect);
    });

    subOfficeSelect.addEventListener('change', function () {
        const code = this.options[this.selectedIndex].getAttribute('data-code');
        if (code) {
            postalInput.value = code;
            isPostalValid = true;
            postalStatus.textContent = "Auto-filled";
            postalStatus.className = "text-success small";
        }
    });

    // Validation via API
    postalInput.addEventListener('blur', function () {
        const code = this.value.trim();
        if (code.length < 4) { isPostalValid = false; return; }
        postalStatus.textContent = "Checking...";

        window.OrderAPI.checkPostalCode(code).then(async data => {
            if (data.found) {
                isPostalValid = true;
                postalStatus.textContent = "Location found!";
                postalStatus.className = "text-success small";

                const div = data.division || data.DivisionEn;
                const dist = data.district || data.DistrictEn;
                const th = data.thana || data.ThanaEn;
                const sub = data.subOffice || data.SubOfficeEn;

                if (div) {
                    divisionSelect.value = div;

                    // Trigger auto calc for the new location
                    autoCalculateDelivery();

                    await loadFromApi(window.OrderAPI.getDistricts(div), districtSelect, dist);
                    await loadFromApi(window.OrderAPI.getThanas(dist), thanaSelect, th);
                    await loadFromApi(window.OrderAPI.getSubOffices(th), subOfficeSelect, sub);
                }
            } else {
                isPostalValid = false;
                postalStatus.textContent = "Invalid Code";
                postalStatus.className = "text-danger small";
            }
        });
    });

    // ==========================================
    // 4. CUSTOMER AUTOFILL (Using OrderAPI)
    // ==========================================
    const custAddress = document.getElementById('custAddress');
    // NOTE: 'phoneInput' is already defined at the top for intl-tel-input

    const addressContainer = document.getElementById('address-container');

    // Email Check Logic (Visual Only)
    const custEmail = document.getElementById('custEmail');
    const emailStatus = document.getElementById('custEmailStatus');
    let currentLoadedEmail = "";

    if (custEmail) {
        custEmail.addEventListener('blur', function () {
            const email = this.value.trim();
            if (emailStatus) emailStatus.textContent = "";

            if (!email || !email.includes('@')) return;

            if (email === currentLoadedEmail) {
                if (emailStatus) {
                    emailStatus.textContent = "✓ Verified existing email";
                    emailStatus.className = "d-block mt-1 text-success small";
                }
                return;
            }

            fetch(`/order/check-email?email=${encodeURIComponent(email)}`)
                .then(r => r.json())
                .then(data => {
                    if (emailStatus) {
                        if (data.exists) {
                            emailStatus.textContent = "⚠ This email is already registered to another customer!";
                            emailStatus.className = "d-block mt-1 text-danger fw-bold small";
                        } else {
                            emailStatus.textContent = "✓ Email available";
                            emailStatus.className = "d-block mt-1 text-success small";
                        }
                    }
                });
        });
    }

    function toggleMode() {
        addressContainer.style.display = modeStore.checked ? 'none' : 'block';
        autoCalculateDelivery();
    }
    modeDelivery.addEventListener('change', toggleMode);
    modeStore.addEventListener('change', toggleMode);

    // ==========================================
    // 2. PHONE INPUT LOGIC & VALIDATION
    // ==========================================

    const statusEl = document.getElementById('custStatus');

    // A. RESTRICT INPUT: Allow only 0-9 and +
    // This runs immediately (no debounce) to prevent typing letters
    phoneInput.addEventListener('input', function () {
        const val = this.value;
        if (/[^0-9+]/.test(val)) {
            this.value = val.replace(/[^0-9+]/g, '');
        }
    });

    // Handles Auto-Discovery & Immediate "Too Long" checks
    phoneInput.addEventListener('input', debounce(function () {
        const fullPhone = iti.getNumber();
        const isValid = iti.isValidNumber();
        const errorCode = iti.getValidationError();

        // Reset UI defaults
        statusEl.className = "text-muted small";

        if (isValid) {
            // CASE A: Number is Valid -> Trigger Auto-Check
            statusEl.textContent = "⏳ Checking...";
            statusEl.className = "text-primary small";

            // Call API
            window.OrderAPI.checkCustomer(fullPhone).then(data => {
                if (data.found) {
                    // FOUND
                    statusEl.textContent = "✓ Customer found!";
                    statusEl.className = "text-success small fw-bold";

                    document.getElementById('custName').value = data.name || '';
                    if (custEmail) {
                        custEmail.value = data.email || '';
                        currentLoadedEmail = data.email || '';
                    }

                    // Autofill Address (Only if Delivery Mode)
                    if (data.addressData && modeDelivery.checked) {
                        custAddress.value = data.addressData.street || '';

                        // Smart Location Filling
                        if (data.addressData.postalCode) {
                            postalInput.value = data.addressData.postalCode;
                            postalInput.dispatchEvent(new Event('blur'));
                        } else if (data.addressData.divison) {
                            const div = data.addressData.divison;
                            divisionSelect.value = div;
                            autoCalculateDelivery();

                            // Cascading loads
                            if (data.addressData.city) {
                                loadFromApi(window.OrderAPI.getDistricts(div), districtSelect, data.addressData.city)
                                    .then(() => data.addressData.thana ? loadFromApi(window.OrderAPI.getThanas(data.addressData.city), thanaSelect, data.addressData.thana) : null)
                                    .then(() => data.addressData.subOffice ? loadFromApi(window.OrderAPI.getSubOffices(data.addressData.thana), subOfficeSelect, data.addressData.subOffice) : null);
                            }
                        }
                    }
                } else {
                    // NEW CUSTOMER
                    statusEl.textContent = "New Customer";
                    statusEl.className = "text-info small";

                    // Clear fields
                    document.getElementById('custName').value = '';
                    if (custEmail) { custEmail.value = ''; currentLoadedEmail = ''; }
                    custAddress.value = '';
                }
            });
        } else {
            // CASE B: Number is Invalid (While Typing)

            // 1. If Too Long (Error Code 3), tell them immediately
            if (errorCode === 3) {
                statusEl.textContent = "⚠ Number too long";
                statusEl.className = "text-danger small";
            }
            // 2. If Too Short or just Invalid, DO NOT complain yet. 
            //    Wait for them to finish typing (handled in 'blur').
            else {
                statusEl.textContent = "";
            }
        }
    }, 500));

    // C. BLUR VALIDATION
    // Handles "Too Short" or generic errors when user leaves the field
    phoneInput.addEventListener('blur', function () {
        const fullPhone = iti.getNumber();
        const isValid = iti.isValidNumber();
        const errorCode = iti.getValidationError();

        // Only show error if input is not empty but invalid
        if (!isValid && fullPhone.length > 0) {
            if (errorCode === 2) { // TOO_SHORT
                statusEl.textContent = "⚠ Number too short";
                statusEl.className = "text-danger small";
            } else if (errorCode === 3) { // TOO_LONG
                statusEl.textContent = "⚠ Number too long";
                statusEl.className = "text-danger small";
            } else {
                statusEl.textContent = "⚠ Invalid number";
                statusEl.className = "text-danger small";
            }
        }
    });

    // ==========================================
    // 5. SUBMIT ORDER
    // ==========================================
    document.getElementById('btnPlaceOrder').addEventListener('click', function () {
        const btn = this;
        const msg = document.getElementById('orderMsg');
        const isStoreSale = modeStore.checked;

        // ✅ 4. VALIDATE PHONE BEFORE SUBMIT
        if (!iti.isValidNumber()) {
            alert("Please enter a valid phone number for the selected country.");
            phoneInput.focus();
            return;
        }

        if (!document.getElementById('custName').value || !variantSelect.value) {
            alert("Please fill in Name, Product, and Variant.");
            return;
        }

        if (!isStoreSale) {
            if (!custAddress.value || !divisionSelect.value) {
                alert("Address and Division are required for Home Delivery.");
                return;
            }
            if (!isPostalValid) {
                alert("Please verify Postal Code before submitting.");
                return;
            }
        }

        // Check email status before submitting
        if (emailStatus && emailStatus.className.includes('text-danger')) {
            alert("Please use a different email address.");
            custEmail.focus();
            return;
        }

        const payload = {

            // ✅ 5. SEND STANDARDIZED NUMBER
            CustomerPhone: iti.getNumber(),

            CustomerName: document.getElementById('custName').value,
            CustomerEmail: custEmail ? custEmail.value : '',
            Street: isStoreSale ? "Counter Sale - In Store" : custAddress.value,
            Divison: isStoreSale ? "Dhaka" : divisionSelect.value,
            City: isStoreSale ? "Dhaka" : districtSelect.value,
            Thana: isStoreSale ? "" : thanaSelect.value,
            SubOffice: isStoreSale ? "" : subOfficeSelect.value,
            PostalCode: isStoreSale ? "1000" : (postalInput.value || "0000"),

            ProductVariantId: parseInt(variantSelect.value),
            OrderQuantity: parseInt(qtyInput.value),

            // ✅ ADDED: Send the Delivery Charge from the editable input
            DeliveryCharge: parseFloat(deliveryInput ? deliveryInput.value : 0) || 0,

            TargetCompanyId: parseInt(document.getElementById('targetCompanyId')?.value) || 1,
            Confirmed: document.getElementById('confirmImmediately').checked
        };

        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
        btn.disabled = true; btn.textContent = "Processing...";
        if (msg) msg.textContent = "";

        fetch('/order/place-direct', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
            body: JSON.stringify(payload)
        }).then(r => r.json()).then(data => {
            if (data.success) {
                let finalId = data.orderId;
                let finalNet = 0;

                if (typeof data.orderId === 'object' && data.orderId !== null) {
                    finalNet = data.orderId.NetAmount || data.orderId.netAmount;
                    finalId = data.orderId.OrderId || data.orderId.orderId;
                } else if (data.netAmount) {
                    finalNet = data.netAmount;
                }

                const deliveryCharge = parseFloat(deliveryInput ? deliveryInput.value : 0) || 0;

                const grandTotal = parseFloat(displayNet.textContent) || 0;

                const modalHtml = `
                <div class="modal fade" id="successModal" tabindex="-1" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
                  <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content border-0 shadow-lg">
                      <div class="modal-body text-center p-5">
                        <div class="mb-3" style="font-size: 4rem;">🎉</div>
                        <h2 class="fw-bold text-success mb-2">Order Placed!</h2>
                        <p class="text-muted mb-4">The direct order has been created successfully.</p>
                        
                        <div class="card bg-light border-0 p-3 mb-4">
                            <h4 class="fw-bold text-dark m-0">${finalId}</h4>
                            <div class="d-flex justify-content-between mt-3 border-top pt-2">
                                <span class="fw-bold">Grand Total (Inc. Delivery):</span>
                                <span class="fw-bold text-primary fs-5">Tk. ${grandTotal.toFixed(2)}</span>
                            </div>
                        </div>

                        <div class="d-grid gap-2 d-sm-flex justify-content-sm-center">
                            <button type="button" class="btn btn-outline-secondary px-4" onclick="window.location.reload()">Create Another</button>
                            <button type="button" class="btn btn-primary px-4" onclick="window.location.href='/order/all'">View Orders List</button>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>`;

                document.body.insertAdjacentHTML('beforeend', modalHtml);
                const successModal = new bootstrap.Modal(document.getElementById('successModal'));
                successModal.show();

            } else {
                if (msg) {
                    msg.textContent = "Error: " + data.message;
                    msg.className = "text-danger small mt-2";
                }
                btn.disabled = false; btn.textContent = "Create Direct Order";
            }
        }).catch(err => {
            if (msg) {
                msg.textContent = "Network Error";
                msg.className = "text-danger small mt-2";
            }
            btn.disabled = false; btn.textContent = "Create Direct Order";
        });
    });

    // Initialize state
    toggleMode();

});