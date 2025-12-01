// wwwroot/js/admin-order.js

document.addEventListener('DOMContentLoaded', function () {
    console.log("Admin Order Script Loaded");

    // ==========================================
    // 1. UI ELEMENTS
    // ==========================================
    const productSelect = document.getElementById('productSelect');
    const variantSelect = document.getElementById('variantSelect');
    const qtyInput = document.getElementById('orderQty');
    const stockInfo = document.getElementById('stockInfo');

    // Display Elements
    const displayPrice = document.getElementById('displayPrice');
    const displaySubTotal = document.getElementById('displaySubTotal');
    const displayDiscount = document.getElementById('displayDiscount');
    const displayDelivery = document.getElementById('displayDelivery');
    const displayNet = document.getElementById('displayNet');

    // Mode & Location Elements
    const modeDelivery = document.getElementById('modeDelivery');
    const modeStore = document.getElementById('modeStore');
    const addressContainer = document.getElementById('address-container');

    const divisionSelect = document.getElementById('division-select');
    const districtSelect = document.getElementById('district-select');
    const thanaSelect = document.getElementById('thana-select');
    const subOfficeSelect = document.getElementById('suboffice-select');
    const postalInput = document.getElementById('postalCode');
    const postalStatus = document.getElementById('postalStatus');

    // ==========================================
    // 2. TOGGLE MODE (In-Store Logic)
    // ==========================================
    function toggleMode() {
        const isStore = modeStore && modeStore.checked;

        // 1. Show/Hide Address
        if (addressContainer) {
            addressContainer.style.display = isStore ? 'none' : 'block';
        }

        // 2. Recalculate Totals (to update Delivery Charge)
        calcTotal();
    }

    if (modeDelivery) modeDelivery.addEventListener('change', toggleMode);
    if (modeStore) modeStore.addEventListener('change', toggleMode);

    // ==========================================
    // 3. CALCULATION LOGIC (With Crash Fix)
    // ==========================================
    function calcTotal() {
        // --- CRITICAL SAFETY CHECK ---
        // If the variant dropdown is disabled or empty, we cannot calculate price.
        // We set defaults and EXIT immediately to prevent crashes.
        if (variantSelect.disabled || variantSelect.selectedIndex === -1) {
            if (displayPrice) displayPrice.textContent = "0.00";
            if (displaySubTotal) displaySubTotal.textContent = "0.00";
            if (displayDiscount) displayDiscount.textContent = "0.00";
            if (displayDelivery) displayDelivery.textContent = "0.00";
            if (displayNet) displayNet.textContent = "0.00";
            return;
        }

        // 1. Get Selected Option
        const option = variantSelect.options[variantSelect.selectedIndex];

        // 2. Get Data Attributes (Safe Parsing)
        const price = parseFloat(option.getAttribute('data-price')) || 0;
        const qty = parseInt(qtyInput.value) || 1;

        const discType = option.getAttribute('data-disc-type') || "None";
        const discVal = parseFloat(option.getAttribute('data-disc-val')) || 0;

        // 3. Calculate Basic
        const subTotal = price * qty;

        // 4. Calculate Discount
        let discountAmount = 0;
        if (discType === "Flat") {
            discountAmount = discVal * qty;
        } else if (discType === "Percentage") {
            discountAmount = subTotal * (discVal / 100);
        }

        // 5. Calculate Delivery
        let deliveryCharge = 0;
        if (modeStore && modeStore.checked) {
            deliveryCharge = 0; // In-Store is always 0
        } else {
            // Home Delivery logic
            const division = (divisionSelect && divisionSelect.value) ? divisionSelect.value.toLowerCase() : "";

            if (division === "") {
                deliveryCharge = 0; // No division selected yet
            } else if (division.includes("dhaka")) {
                deliveryCharge = 50;
            } else {
                deliveryCharge = 100;
            }
        }

        // 6. Net Total
        const grandTotal = (subTotal - discountAmount) + deliveryCharge;

        // 7. Update UI
        displayPrice.textContent = price.toFixed(2);
        displaySubTotal.textContent = subTotal.toFixed(2);
        displayDiscount.textContent = discountAmount.toFixed(2);
        if (displayDelivery) displayDelivery.textContent = deliveryCharge.toFixed(2);
        displayNet.textContent = grandTotal.toFixed(2);
    }

    // ==========================================
    // 4. POPULATE PRODUCTS (From View Data)
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
    // 5. EVENT LISTENERS
    // ==========================================

    // Product Change -> Load Variants
    productSelect.addEventListener('change', function () {
        // Reset Logic
        variantSelect.innerHTML = '<option value="" data-price="0">Select Variant...</option>';
        variantSelect.disabled = true;
        if (stockInfo) stockInfo.textContent = "";

        // Recalculate (will set to 0.00)
        calcTotal();

        const pId = this.value;
        if (pId && productMap[pId]) {
            productMap[pId].variants.forEach(v => {
                // Map Properties (Case insensitive fallback)
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

            // Enable and allow user to pick
            variantSelect.disabled = false;
        }
    });

    // Variant Change -> Update Price & Stock Info
    variantSelect.addEventListener('change', function () {
        const option = this.options[this.selectedIndex];

        // Stock Display
        const stock = option.getAttribute('data-stock');
        if (stockInfo) {
            if (stock) {
                const stockInt = parseInt(stock);
                if (stockInt >= 999) {
                    stockInfo.textContent = "In Stock";
                    stockInfo.className = "form-text text-success";
                } else {
                    stockInfo.textContent = `Available Stock: ${stock}`;
                    stockInfo.className = stockInt < 5 ? "form-text text-danger fw-bold" : "form-text text-success";
                }
            } else {
                stockInfo.textContent = "";
            }
        }

        // Recalculate Price
        calcTotal();
    });

    qtyInput.addEventListener('input', calcTotal);
    if (divisionSelect) divisionSelect.addEventListener('change', calcTotal);

    // ==========================================
    // 6. SHARED API HELPERS
    // ==========================================
    async function loadAdminDropdown(apiPromise, targetSelect, selectedVal = null) {
        targetSelect.innerHTML = '<option value="">Loading...</option>';
        targetSelect.disabled = true;

        try {
            const data = await apiPromise;
            targetSelect.innerHTML = '<option value="">Select...</option>';

            if (Array.isArray(data)) {
                data.forEach(item => {
                    let val = item.name || item;
                    let opt = new Option(val, val);
                    if (item.code || item.postalCode) {
                        opt.setAttribute('data-code', item.code || item.postalCode);
                    }
                    targetSelect.add(opt);
                });
                targetSelect.disabled = false;
                if (selectedVal) targetSelect.value = selectedVal;
            }
        } catch (e) {
            targetSelect.innerHTML = '<option value="">Error</option>';
        }
    }

    // ==========================================
    // 7. LOAD DIVISIONS & SETUP CASCADING
    // ==========================================
    if (window.OrderAPI) {
        loadAdminDropdown(window.OrderAPI.getDivisions(), divisionSelect);
    } else {
        console.error("OrderAPI missing. combo.js must be loaded first.");
    }

    divisionSelect.addEventListener('change', () => {
        loadAdminDropdown(window.OrderAPI.getDistricts(divisionSelect.value), districtSelect);
        // Reset children
        thanaSelect.innerHTML = '<option value="">Select...</option>'; thanaSelect.disabled = true;
        subOfficeSelect.innerHTML = '<option value="">Select...</option>'; subOfficeSelect.disabled = true;
        // Update Delivery Charge if Division changes (Dhaka vs Outside)
        calcTotal();
    });

    districtSelect.addEventListener('change', () => {
        loadAdminDropdown(window.OrderAPI.getThanas(districtSelect.value), thanaSelect);
    });

    thanaSelect.addEventListener('change', () => {
        loadAdminDropdown(window.OrderAPI.getSubOffices(thanaSelect.value), subOfficeSelect);
    });

    subOfficeSelect.addEventListener('change', function () {
        const option = this.options[this.selectedIndex];
        const autoCode = option.getAttribute('data-code');
        if (autoCode && postalInput) {
            postalInput.value = autoCode;
            if (postalStatus) {
                postalStatus.textContent = "Code auto-filled";
                postalStatus.className = "text-success small";
            }
        }
    });

    // ==========================================
    // 8. CUSTOMER CHECK
    // ==========================================
    const custAddress = document.getElementById('custAddress');
    const phoneInput = document.getElementById('custPhone');
    const btnCheckCust = document.getElementById('btnCheckCust');

    if (btnCheckCust) {
        btnCheckCust.addEventListener('click', () => {
            const phone = phoneInput.value.trim();
            if (phone.length < 10) return;

            window.OrderAPI.checkCustomer(phone).then(data => {
                if (data.found) {
                    document.getElementById('custName').value = data.name || '';
                    const emailEl = document.getElementById('custEmail');
                    if (emailEl) emailEl.value = data.email || '';

                    const custStatus = document.getElementById('custStatus');
                    if (custStatus) {
                        custStatus.textContent = "Customer found!";
                        custStatus.className = "text-success small";
                    }

                    // Autofill address ONLY if we are in Delivery Mode
                    if (data.addressData && modeDelivery.checked && custAddress) {
                        custAddress.value = data.addressData.street || '';
                    }
                } else {
                    const custStatus = document.getElementById('custStatus');
                    if (custStatus) {
                        custStatus.textContent = "New Customer";
                        custStatus.className = "text-primary small";
                    }
                    document.getElementById('custName').value = '';
                    if (custAddress) custAddress.value = '';
                }
            });
        });
    }

    // ==========================================
    // 9. SUBMIT ORDER
    // ==========================================
    const btnPlaceOrder = document.getElementById('btnPlaceOrder');
    if (btnPlaceOrder) {
        btnPlaceOrder.addEventListener('click', function () {
            const btn = this;
            const msg = document.getElementById('orderMsg');

            // Check current mode
            const isStoreSale = modeStore && modeStore.checked;

            // --- VALIDATION ---
            if (!phoneInput.value || !document.getElementById('custName').value || !variantSelect.value) {
                alert("Please fill in Phone, Name, Product, and Variant.");
                return;
            }

            // Only validate Address/Division if we are doing HOME DELIVERY
            if (!isStoreSale) {
                if (!custAddress.value || !divisionSelect.value) {
                    alert("Address and Division are required for Home Delivery.");
                    return;
                }
            }

            // --- PREPARE PAYLOAD ---
            // If In-Store, we send dummy address data to satisfy the backend Controller
            const payload = {
                CustomerPhone: phoneInput.value,
                CustomerName: document.getElementById('custName').value,
                CustomerEmail: document.getElementById('custEmail') ? document.getElementById('custEmail').value : '',

                // Logic: If Store Sale, send "Counter Sale" string. If Delivery, send real input.
                Street: isStoreSale ? "Counter Sale - In Store" : custAddress.value,
                Divison: isStoreSale ? "Dhaka" : divisionSelect.value,
                City: isStoreSale ? "Dhaka" : districtSelect.value,
                Thana: isStoreSale ? "" : thanaSelect.value,
                SubOffice: isStoreSale ? "" : subOfficeSelect.value,
                PostalCode: isStoreSale ? "1000" : (postalInput.value || "0000"),

                ProductVariantId: parseInt(variantSelect.value),
                OrderQuantity: parseInt(qtyInput.value),
                TargetCompanyId: parseInt(document.getElementById('targetCompanyId').value) || 0,
                Confirmed: document.getElementById('confirmImmediately').checked
            };

            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

            btn.disabled = true;
            btn.textContent = "Processing...";
            if (msg) msg.textContent = "";

            // --- SEND REQUEST ---
            fetch('/order/place-direct', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify(payload)
            })
                .then(r => r.json())
                .then(data => {
                    if (data.success) {
                        let finalId = data.orderId;
                        // Handle capitalization differences (OrderId vs orderId)
                        if (typeof data.orderId === 'object' && data.orderId !== null) {
                            finalId = data.orderId.OrderId || data.orderId.orderId;
                        }
                        alert("Success! Order Created: " + finalId);
                        window.location.reload();
                    } else {
                        if (msg) {
                            msg.textContent = "Error: " + data.message;
                            msg.className = "mt-3 text-center small text-danger";
                        }
                        btn.disabled = false;
                        btn.textContent = "Create Direct Order";
                    }
                })
                .catch(err => {
                    console.error(err);
                    if (msg) {
                        msg.textContent = "Network Error";
                        msg.className = "mt-3 text-center small text-danger";
                    }
                    btn.disabled = false;
                    btn.textContent = "Create Direct Order";
                });
        });
    }

    // Initialize the Toggle State on Load
    toggleMode();
});