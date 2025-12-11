//change

document.addEventListener('DOMContentLoaded', function () {

    console.log("Admin Order Script Loaded");

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

                // ✅ FIX: Check Stock Status
                let label = `${vName} (Tk. ${vPrice})`;
                let isOutOfStock = vStock <= 0;

                if (isOutOfStock) {
                    label += " ❌";
                } else if (vStock < 10 && vStock !== 999) {
                    // Optional: Show stock count in dropdown if low
                    label += ` (${vStock} left)`;
                }

                let opt = new Option(label, vId);
                opt.setAttribute('data-price', vPrice);
                opt.setAttribute('data-stock', vStock);
                opt.setAttribute('data-disc-type', dType);
                opt.setAttribute('data-disc-val', dVal);

                // ✅ FIX: Disable and Color Red if Out of Stock
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
            // We don't need to check <= 0 here because they are disabled in the dropdown,
            // but good to handle just in case.
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
    if (divisionSelect) divisionSelect.addEventListener('change', calcTotal);
    if (modeDelivery) modeDelivery.addEventListener('change', calcTotal);
    if (modeStore) modeStore.addEventListener('change', calcTotal);

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

        let delivery = 0;
        if (!modeStore.checked) {
            const div = divisionSelect.value ? divisionSelect.value.toLowerCase() : "";
            if (div.includes("dhaka")) delivery = 50;
            else if (div !== "") delivery = 100;
        }

        const net = (subTotal - discount) + delivery;

        displayPrice.textContent = price.toFixed(2);
        displaySubTotal.textContent = subTotal.toFixed(2);
        displayDiscount.textContent = discount.toFixed(2);
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
                calcTotal();
            });
    });

    districtSelect.addEventListener('change', () => {
        loadFromApi(window.OrderAPI.getThanas(districtSelect.value), thanaSelect);
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
                    calcTotal();
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
    const phoneInput = document.getElementById('custPhone');
    const addressContainer = document.getElementById('address-container');

    // Email Check Logic (Visual Only)
    const custEmail = document.getElementById('custEmail');
    const emailStatus = document.getElementById('custEmailStatus'); // Assuming you created this element in view or JS
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
        calcTotal();
    }
    modeDelivery.addEventListener('change', toggleMode);
    modeStore.addEventListener('change', toggleMode);

    document.getElementById('btnCheckCust').addEventListener('click', () => {
        const phone = phoneInput.value.trim();
        if (phone.length < 10) return;

        window.OrderAPI.checkCustomer(phone).then(data => {
            if (data.found) {
                document.getElementById('custName').value = data.name || '';

                // Handle Email
                if (custEmail) {
                    custEmail.value = data.email || '';
                    currentLoadedEmail = data.email || '';
                }

                document.getElementById('custStatus').textContent = "Customer found!";
                document.getElementById('custStatus').className = "text-success small";

                if (data.addressData && modeDelivery.checked) {
                    custAddress.value = data.addressData.street || '';

                    // Waterfall Chain for Address
                    if (data.addressData.postalCode) {
                        postalInput.value = data.addressData.postalCode;
                        postalInput.dispatchEvent(new Event('blur'));
                    }
                    else if (data.addressData.divison) {
                        const div = data.addressData.divison;
                        const dist = data.addressData.city;
                        const th = data.addressData.thana;
                        const sub = data.addressData.subOffice;

                        divisionSelect.value = div;
                        calcTotal();

                        if (dist) {
                            loadFromApi(window.OrderAPI.getDistricts(div), districtSelect, dist)
                                .then(() => {
                                    if (th) {
                                        return loadFromApi(window.OrderAPI.getThanas(dist), thanaSelect, th);
                                    }
                                })
                                .then(() => {
                                    if (sub) {
                                        return loadFromApi(window.OrderAPI.getSubOffices(th), subOfficeSelect, sub);
                                    }
                                });
                        }
                    }
                }
            } else {
                document.getElementById('custStatus').textContent = "New Customer";
                document.getElementById('custStatus').className = "text-primary small";
                document.getElementById('custName').value = '';
                if (custEmail) { custEmail.value = ''; currentLoadedEmail = ''; }
                custAddress.value = '';
            }
        });
    });

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
            CustomerPhone: phoneInput.value,
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

                const deliveryCharge = parseFloat(displayDelivery.textContent) || 0;
                const grandTotal = (parseFloat(finalNet) || 0) + deliveryCharge;

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
<button type="button" 
        class="btn btn-primary px-4" 
        onclick="window.location.href='/order/all'">
    View Orders List
</button>                        </div>
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