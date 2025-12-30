// wwwroot/js/combo.js

// ==================================================
// 1. ORDER API HELPERS
// ==================================================

window.OrderAPI = {
    // Check Customer Phone
    checkCustomer: async function (phone) {
        try {
            const response = await fetch(`/order/check-customer?phone=${phone}`);
            if (!response.ok) return { found: false };
            return await response.json();
        } catch (e) {
            console.error("API Error:", e);
            return { found: false };
        }
    },

    // Check Postal Code
    checkPostalCode: async function (code) {
        try {
            const response = await fetch(`/order/check-postal-code?code=${code}`);
            if (!response.ok) return { found: false };
            return await response.json();
        } catch (e) {
            return { found: false };
        }
    },

    // Location Cascading
    getDivisions: async function () {
        try {
            const response = await fetch('/order/get-divisions');
            return await response.json();
        } catch (e) { return []; }
    },

    getDistricts: async function (div) {
        try {
            const response = await fetch(`/order/get-districts?division=${div}`);
            return await response.json();
        } catch (e) { return []; }
    },

    getThanas: async function (dist) {
        try {
            const response = await fetch(`/order/get-thanas?district=${dist}`);
            return await response.json();
        } catch (e) { return []; }
    },

    getSubOffices: async function (thana) {
        try {
            const response = await fetch(`/order/get-suboffices?thana=${thana}`);
            return await response.json();
        } catch (e) { return []; }
    }
};
// --- INITIALIZE COUNTRY CODE INPUT ---
const input = document.querySelector("#customerPhone");
const iti = window.intlTelInput(input, {
    initialCountry: "bd",             // Default to Bangladesh (+880)
    separateDialCode: true,           // Shows flag & +880 in dropdown
    preferredCountries: ["bd", "us", "gb"], // Optional: Top countries
    utilsScript: "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.19/js/utils.js"
});
$(document).ready(function () {

    // SAFETY CHECK: If we are on the Admin page (which doesn't have #order-form),
    // stop here so we don't cause errors.
    if ($('#order-form').length === 0) {
        return;
    }

    // ==================================================
    // 2. PAYMENT METHOD UI LOGIC (SIMPLIFIED)
    // ==================================================

    // 1. Handle clicking a Payment Method Card
    $('.payment-option').on('click', function () {
        // Visual Selection
        $('.payment-option').removeClass('selected');
        $(this).addClass('selected');

        // Check the hidden radio button so form submission works
        $(this).find('input[type="radio"]').prop('checked', true);

        handlePaymentUI();
    });

    // Core Logic: Show/Hide Input fields based on the selected card's mode
    function handlePaymentUI() {
        const $selected = $('.payment-option.selected');
        if ($selected.length === 0) return;

        // Get Mode directly from the card (Manual vs Gateway)
        const mode = $selected.data('mode');
        const instruction = $selected.find('.manual-instruction-text').val();
        const $detailsArea = $('#payment-details-area');
        const $trxInput = $('#trx-id-input');

        if (mode === 'Manual') {
            // Case: Manual (Send Money)
            $detailsArea.slideDown(200);
            $('#instruction-text').text(instruction || "Please follow the instructions.");
            $trxInput.prop('required', true); // Make TrxID mandatory
        } else {
            // Case: Gateway (Secure Pay) or COD
            $detailsArea.slideUp(200);
            $trxInput.prop('required', false).val(''); // Clear and un-require
        }

        updateSubmitButtonText();
    }

    // Update Button Text based on selection
    function updateSubmitButtonText() {
        const $selected = $('.payment-option.selected');
        const methodCode = $selected.data('payment'); // e.g. 'cod', 'bkash'
        const mode = $selected.data('mode');          // 'Manual', 'Gateway'
        const totalAmount = $('#receipt-grand-total').text();
        const $btn = $('#final-submit-btn');

        if (methodCode === 'cod') {
            $btn.text('Confirm Order (Cash on Delivery)');
        }
        else if (mode === 'Manual') {
            $btn.text('Verify & Confirm Order');
        }
        else {
            // Gateway / Direct
            $btn.text(`Pay ${totalAmount} Now`);
        }
    }

    // Initialize on page load (to handle default selection)
    handlePaymentUI();

    // Hook into the existing total update function
    const originalUpdateTotals = updateTotals;
    updateTotals = function () {
        originalUpdateTotals();
        updateSubmitButtonText();
    };

    // --- UPDATE SUBMIT HANDLER DATA ---
    // Make sure your Submit Handler (Section 9) captures the correct mode
    // Add this inside $('#order-form').submit(function(e) {...}) just before $.ajax
    /*
        const $selectedCard = $('.payment-option.selected');
        formData.PaymentMode = $selectedCard.data('mode') || 'Gateway'; // Default to Gateway if undefined (like COD)
        
        if (formData.PaymentMode === 'Manual') {
            formData.TransactionReference = $('#trx-id-input').val();
        } else {
            formData.TransactionReference = '';
        }
    */

    // ==================================================
    // 3. GLOBAL VARIABLES & STATE
    // ==================================================

    const baseInfo = window.baseProductInfo || { price: 0, image: "/images/default-product.jpg" };

    let currentVariantPrice = baseInfo.price;
    let maxAvailableStock = 0; // Will be set when variant is selected
    let selectedAttributes = {};

    let isCheckingEmail = false;
    let isEmailAutofilled = false;
    let currentCustomerEmail = null;

    const delivery = (typeof deliveryCharges !== "undefined") ? deliveryCharges : { dhaka: 0, outside: 0 };
    const baseProductImageUrl = baseInfo.image;

    $('#display-price').text('Tk. ' + currentVariantPrice.toLocaleString());
    $('#summary-subtotal').text('Tk. ' + currentVariantPrice.toLocaleString());

    // Helper: Debounce Function for Performance (Anti-Spam)
    function debounce(func, wait) {
        let timeout;
        return function () {
            const context = this, args = arguments;
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(context, args), wait);
        };
    }

    // ==================================================
    // 4. AUTO-SELECT FIRST VARIANT IF ONLY ONE EXISTS
    // ==================================================
    const variants = (window.productVariants || []).filter(v => v);
    if (variants.length === 1) {
        const singleVariant = variants[0];
        applyVariantData(singleVariant);
        $('.variant-chip').addClass('selected');

        // Populate selectedAttributes for the single variant
        if (singleVariant.attributes) {
            selectedAttributes = { ...singleVariant.attributes };
        }
    }

    // ==================================================
    // 5. DYNAMIC ATTRIBUTE SELECTION LOGIC (WITH CASCADING)
    // ==================================================

    // 1. Updated Click Listener (Force String Type)
    $(document).on('click', '.variant-chip', function () {
        let $el = $(this);
        let attributeName = $el.data('attribute');

        // ✅ FIX 3: Use .attr() to get the raw string value (prevents "40" becoming number 40)
        let attributeValue = $el.attr('data-value');

        if ($el.hasClass('selected')) {
            $el.removeClass('selected');
            delete selectedAttributes[attributeName];
        } else {
            $(`.variant-chip[data-attribute='${attributeName}']`).removeClass('selected');
            $el.addClass('selected');
            selectedAttributes[attributeName] = attributeValue;
        }

        updateAttributeAvailability();
        findAndApplyVariant();
    });

    // 2. Updated Match Logic (Safety Checks & Loose Equality)
    function findAndApplyVariant() {
        $('#selected-variant-id').val('');

        // ✅ FIX 4: Filter out undefined slots just in case
        const safeVariants = (window.productVariants || []).filter(v => v);

        const matchedVariant = safeVariants.find(v => {
            if (!v.attributes) return false;

            const variantKeys = Object.keys(v.attributes);
            const selectedKeys = Object.keys(selectedAttributes);

            // A. Count Check
            if (variantKeys.length !== selectedKeys.length) return false;

            // B. Value Check (String Comparison)
            for (let key of selectedKeys) {
                // Ensure the variant has the key
                if (!v.attributes.hasOwnProperty(key)) return false;

                // Compare as Strings to fix Type Mismatch
                if (String(v.attributes[key]).trim() !== String(selectedAttributes[key]).trim()) {
                    return false;
                }
            }
            return true;
        });

        if (matchedVariant) {
            if (Object.keys(selectedAttributes).length > 0) {
                applyVariantData(matchedVariant);
            } else {
                resetToDefault();
            }
        } else {
            handleNoMatch();
        }
    }    // --- NEW CASCADING LOGIC FUNCTION ---
    function updateAttributeAvailability() {
        $('.variant-chip').each(function () {
            const $chip = $(this);
            const chipAttribute = $chip.data('attribute');

            // CHANGE: Use .attr() to ensure String comparison
            const chipValue = $chip.attr('data-value');

            // Skip if this specific chip is already selected
            if ($chip.hasClass('selected')) {
                $chip.removeClass('disabled');
                return;
            }

            // Create a "Test Scenario"
            const testSelection = { ...selectedAttributes };

            // Allow switching values within the same attribute group
            delete testSelection[chipAttribute];
            testSelection[chipAttribute] = chipValue;

            // Check if ANY variant matches this test scenario
            const isCompatible = variants.some(v => {
                for (const [key, val] of Object.entries(testSelection)) {
                    // CHANGE: Convert both sides to String() before comparing
                    if (!v.attributes || String(v.attributes[key]) !== String(val)) {
                        return false;
                    }
                }
                return true;
            });

            // Apply visual state
            if (isCompatible) {
                $chip.removeClass('disabled');
            } else {
                $chip.addClass('disabled');
            }
        });
    }
    // Call once on load to initialize states
    updateAttributeAvailability();

    function findAndApplyVariant() {
        $('#selected-variant-id').val('');

        const matchedVariant = variants.find(v => {
            // ✅ FIX: Safety check for undefined variants
            if (!v || !v.attributes) return false;

            const variantKeys = Object.keys(v.attributes);
            const selectedKeys = Object.keys(selectedAttributes);

            // 1. STRICT COUNT CHECK
            if (variantKeys.length !== selectedKeys.length) return false;

            // 2. STRICT VALUE CHECK
            for (let key of selectedKeys) {
                // Convert both sides to String to match "100" (JSON) with "100" (HTML)
                if (!v.attributes.hasOwnProperty(key) || String(v.attributes[key]) !== String(selectedAttributes[key])) {
                    return false;
                }
            }
            return true;
        });

        if (matchedVariant) {
            if (Object.keys(selectedAttributes).length > 0) {
                applyVariantData(matchedVariant);
            } else {
                resetToDefault();
            }
        } else {
            handleNoMatch();
        }
    } function applyVariantData(variant) {
        $('#selected-variant-id').val(variant.id);
        currentVariantPrice = variant.price;
        $('#display-price').text('Tk. ' + currentVariantPrice.toLocaleString());

        // Handle stock properly
        maxAvailableStock = variant.stock;

        let currentQty = parseInt($('#quantity').val()) || 1;
        if (maxAvailableStock > 0 && currentQty > maxAvailableStock) {
            $('#quantity').val(maxAvailableStock);
        }
        updateStockMessage(maxAvailableStock);

        // 1. Determine the image URL
        let newImg = variant.image && variant.image.length > 1 ? variant.image : baseProductImageUrl;

        // 2. Fix Slash logic (Don't add slash if image is missing)
        if (newImg && newImg.length > 0 && !newImg.startsWith("/") && !newImg.startsWith("http")) {
            newImg = "/" + newImg;
        }

        // 3. Update Image Src AND Force Visibility
        // We use .show() to undo the 'display:none' set by the onerror event
        $('#order-variant-image').attr('src', newImg).show();
        $('#mobile-order-variant-image').attr('src', newImg).show();

        $('.variant-chips-container').css('border', 'none');
        updateTotals();

        if (!isCheckingEmail && !$('#email-status').is(':visible')) {
            $('.submit-btn').prop('disabled', false);
        }
    }

    function resetToDefault() {
        currentVariantPrice = baseInfo.price;
        $('#display-price').text('Tk. ' + currentVariantPrice.toLocaleString());

        $('#order-variant-image').attr('src', baseProductImageUrl);
        $('#mobile-order-variant-image').attr('src', baseProductImageUrl);

        $('#selected-variant-id').val('');
        maxAvailableStock = 0;
        selectedAttributes = {};
        $('.variant-chip').removeClass('selected');

        updateAttributeAvailability();
        $('#variant-info').hide();
        updateStockMessage(0);
        updateTotals();
    }

    function handleNoMatch() {
        $('#stock-message').text("This combination is currently unavailable.").addClass('text-danger show');
        $('#order-variant-image').attr('src', baseProductImageUrl);
        $('#mobile-order-variant-image').attr('src', baseProductImageUrl);
        $('#selected-variant-id').val('');
        maxAvailableStock = 0;
    }

    function updateStockMessage(stock) {
        const el = $('#stock-message');
        const parent = $('#variant-info');

        el.removeClass('stock-high stock-medium stock-low text-danger show text-success');

        if (Object.keys(selectedAttributes).length === 0 && variants.length > 1) {
            parent.hide();
            return;
        }

        parent.show();

        if (stock <= 0) {
            el.text('Out of Stock').addClass('text-danger show');
            $('.submit-btn').prop('disabled', true).text('Out of Stock');
        } else {
            el.text(`Current Stock: ${stock} items available`).addClass('text-success show').css({
                'font-weight': 'bold',
                'color': '#10b981',
                'font-size': '0.95rem'
            });

            if (!isCheckingEmail) {
                $('.submit-btn').prop('disabled', false).text('Confirm Order');
            }
        }
    }

    function showStockError(msg) {
        const el = $('#stock-error-message');
        el.text(msg).addClass('show');
        setTimeout(() => { el.removeClass('show'); }, 3000);
    }

    function clearStockError() {
        $('#stock-error-message').text('').removeClass('show');
    }

    // ==================================================
    // 6. EMAIL & PHONE CHECK (INLINE REAL-TIME VALIDATION)
    // ==================================================

    $('#customerEmail').on('input', function () {
        isEmailAutofilled = false;
        $('#email-status').hide();
        $('.submit-btn').prop('disabled', false).text('Confirm Order');
    });

    $('#customerEmail').on('blur', function () {
        const email = $(this).val().trim();
        const $msg = $('#email-status');

        $msg.hide().removeClass('text-danger').removeClass('text-success');

        // 1. Empty Check
        if (!email) {
            $('.submit-btn').prop('disabled', false).text('Confirm Order');
            return;
        }

        // 2. Autofill match (Case insensitive safety)
        if (isEmailAutofilled && currentCustomerEmail && email.toLowerCase() === currentCustomerEmail.toLowerCase()) {
            $msg.text("✓ Using your registered email").css('color', 'green').show();
            $('.submit-btn').prop('disabled', false).text('Confirm Order');
            return;
        }

        // 3. Format Validation
        if (!email.includes('@')) {
            $msg.text("⚠ Please enter a valid email address").css('color', 'orange').show();
            $('.submit-btn').prop('disabled', false).text('Confirm Order');
            return;
        }

        isCheckingEmail = true;
        $msg.text("⏳ Checking email...").css('color', 'blue').show();
        $('.submit-btn').prop('disabled', true).text('Verifying...');

        // ✅ CAPTURE PHONE NUMBER TO SEND
        // Use iti.getNumber() if available for best formatting, otherwise raw value
        var phoneVal = (typeof iti !== 'undefined') ? iti.getNumber() : $('#customerPhone').val();

        // ✅ SEND PHONE IN REQUEST
        $.get('/order/check-email', { email: email, phone: phoneVal }, function (res) {
            isCheckingEmail = false;

            if (res.exists) {
                // If backend returns true, it effectively means "Conflict with ANOTHER user"
                // because we handled the "Same User" case in the C# controller.

                $msg.text("⚠ This email is already registered with a different phone number.").css('color', 'red').show();
                $('.submit-btn').prop('disabled', true).text('Fix Email');
            } else {
                // Email is available OR belongs to this user -> OK
                $msg.text("✓ Email available").css('color', 'green').show();
                $('.submit-btn').prop('disabled', false).text('Confirm Order');
            }
        }).fail(function () {
            isCheckingEmail = false;
            // In case of network error, don't block the user
            $msg.text("⚠ Could not verify email, but you can proceed.").css('color', 'orange').show();
            $('.submit-btn').prop('disabled', false).text('Confirm Order');
        });
    });
    // Handle Phone Input with Debounce

    // 1. INPUT EVENT: Handles Auto-Discovery & Immediate "Too Long" checks
    // ==================================================
    // RESTRICT PHONE INPUT (Numbers and + only)
    // ==================================================
    $('#customerPhone').on('input', function () {
        var val = $(this).val();

        // Allow 0-9 and the + sign. Remove everything else.
        // regex: /[^0-9+]/g means "replace any character that is NOT a digit OR a plus sign"
        if (/[^0-9+]/.test(val)) {
            $(this).val(val.replace(/[^0-9+]/g, ''));
        }
    });
    $('#customerPhone').on('input', debounce(function () {
        let fullPhone = iti.getNumber();
        const isValid = iti.isValidNumber();
        const errorCode = iti.getValidationError();

        if (isValid) {
            // CASE A: Number is Valid -> Trigger Auto-Check
            $('#phone-status').text("⏳ Checking...").css('color', 'blue');

            // Use encodeURIComponent to handle the '+' sign correctly
            window.OrderAPI.checkCustomer(fullPhone).then(function (data) {
                if (data.found) {
                    $('#phone-status').text("✓ Welcome back! Info loaded.").css('color', 'green');
                    if (data.name) $('#customerName').val(data.name);

                    // Email autofill logic
                    if (data.email) {
                        const $emailField = $('#customerEmail');
                        const currentEmailValue = $emailField.val().trim();
                        if (!currentEmailValue || currentEmailValue.includes('@guest.local')) {
                            isEmailAutofilled = true;
                            currentCustomerEmail = data.email;
                            $emailField.val(data.email);
                            $('#email-status').text("✓ Using your registered email").css('color', 'green').show();
                            $('.submit-btn').prop('disabled', false).text('Confirm Order');
                        }
                    }
                } else {
                    $('#phone-status').text("New Customer").css('color', '#666');
                }
            }).catch(function () {
                $('#phone-status').text("⚠ Could not verify").css('color', 'orange');
            });

        } else {
            // CASE B: Number is Invalid (While Typing)

            // 1. If Too Long, tell them immediately
            if (errorCode === 3) {
                $('#phone-status').text("⚠ Number too long").css('color', 'red');
            }
            // 2. If Too Short (2) or just Invalid (1), DO NOT complain yet. 
            //    Wait for them to finish typing (handled in 'blur').
            else {
                $('#phone-status').text("");
            }

            // Reset autofill flags since phone is invalid
            isEmailAutofilled = false;
            currentCustomerEmail = null;
        }
    }, 500));

    // 2. BLUR EVENT: Handles "Too Short" errors when user leaves the field
    $('#customerPhone').on('blur', function () {
        let fullPhone = iti.getNumber();
        const isValid = iti.isValidNumber();
        const errorCode = iti.getValidationError();

        // Only show error if input is not empty but invalid
        if (!isValid && fullPhone.length > 0) {
            if (errorCode === 2) $('#phone-status').text("⚠ Number too short").css('color', 'red');
            else if (errorCode === 3) $('#phone-status').text("⚠ Number too long").css('color', 'red');
            else $('#phone-status').text("⚠ Invalid number").css('color', 'red');
        }
    });
    // ==================================================
    // 7. LOCATION & TOTALS (ROBUST CASCADING)
    // ==================================================

    function resetSelect(selector, defaultText) {
        $(selector).empty()
            .append(`<option value="">${defaultText}</option>`)
            .prop('disabled', true);
    }

    function enableSelect(selector, data, defaultText) {
        let $el = $(selector);
        $el.empty().append(`<option value="">${defaultText}</option>`);

        if (!data || data.length === 0) {
            $el.append('<option value="" disabled>No options available</option>');
            $el.prop('disabled', false);
            return;
        }

        data.forEach(item => {
            let val = item.name || item.Name || item;
            let text = item.name || item.Name || item;
            let code = item.code || item.Code || "";
            $el.append(`<option value="${val}" data-code="${code}">${text}</option>`);
        });

        $el.prop('disabled', false);
    }

    function populateDivisions() {
        $.get('/order/get-divisions', function (data) {
            enableSelect('#division-select', data, 'Select Division');
        }).fail(function () {
            $('#division-select').append('<option>Error loading data</option>');
        });
    }

    populateDivisions();

    // NEW: Load all districts immediately
    function populateAllDistricts() {
        // We assume your controller can return ALL districts if no division is passed
        // OR you might need to create a specific endpoint like '/order/get-all-districts'
        // For now, we try calling get-districts without parameters.
        $.get('/order/get-districts', function (data) {
            enableSelect('#district-select', data, 'Select District');
        }).fail(function () {
            $('#district-select').empty().append('<option>Error loading districts</option>');
        });
    }

    // Call this instead of populateDivisions
    populateAllDistricts();

    // REMOVED: $('#division-select').change(...) logic is no longer needed 
    // because we don't have a visible division dropdown.

    $('#division-select').change(function () {
        let division = $(this).val();

        resetSelect('#district-select', 'Loading...');
        resetSelect('#thana-select', 'Select District first');
        resetSelect('#suboffice-select', 'Select Thana first');

        if (division) {
            $.get('/order/get-districts', { division: division }, function (data) {
                enableSelect('#district-select', data, 'Select District');
            }).fail(function () {
                resetSelect('#district-select', 'Error loading districts');
                $('#district-select').prop('disabled', false);
            });
        } else {
            resetSelect('#district-select', 'Select Division first');
        }
    });

    $('#district-select').change(function () {
        let district = $(this).val();

        // NEW: Try to find the division for this district from the option data
        // (Assuming your API returns division info, if not, this part is optional but good for data integrity)
        // If your API data doesn't have 'division' in the option dataset, the backend usually figures it out from City anyway.
        // $('#hidden-division').val("..."); 

        resetSelect('#thana-select', 'Loading...');
        resetSelect('#suboffice-select', 'Select Thana first');

        // Delivery Charge Logic (This remains exactly the same)
        let charge = delivery.outside;
        if (district && (district.toLowerCase().includes('dhaka') || district.trim() === 'Dhaka')) {
            charge = delivery.dhaka;
        }
        $('#receipt-delivery').text('Tk. ' + charge).data('cost', charge);
        updateTotals();

        if (district) {
            $.get('/order/get-thanas', { district: district }, function (data) {
                enableSelect('#thana-select', data, 'Select Thana');
            }).fail(function () {
                resetSelect('#thana-select', 'Error loading thanas');
                $('#thana-select').prop('disabled', false);
            });
        } else {
            resetSelect('#thana-select', 'Select District first');
        }
    });
    $('#thana-select').change(function () {
        let thana = $(this).val();

        resetSelect('#suboffice-select', 'Loading...');

        if (thana) {
            $.get('/order/get-suboffices', { thana: thana }, function (data) {
                enableSelect('#suboffice-select', data, 'Select Sub-Office');
            }).fail(function () {
                resetSelect('#suboffice-select', 'Error loading sub-offices');
                $('#suboffice-select').prop('disabled', false);
            });
        } else {
            resetSelect('#suboffice-select', 'Select Thana first');
        }
    });

    $('#suboffice-select').change(function () {
        let code = $(this).find(':selected').data('code');
        if (code && $('input[name="PostalCode"]').val() != code) {
            $('input[name="PostalCode"]').val(code).css('border-color', '#2ecc71');
        }
    });

    function updateTotals() {
        let qty = parseInt($('#quantity').val()) || 1;
        let subtotal = currentVariantPrice * qty;
        let deliveryCost = parseFloat($('#receipt-delivery').data('cost')) || 0;
        let total = subtotal + deliveryCost;

        $('#summary-qty').text(qty);
        $('#summary-subtotal').text('Tk. ' + subtotal.toLocaleString());
        $('#receipt-grand-total').text('Tk. ' + total.toLocaleString());
    }

    // ==================================================
    // 8. QUANTITY CONTROLS
    // ==================================================

    $('#increase').click(function (e) {
        e.preventDefault();
        clearStockError();

        let qty = parseInt($('#quantity').val()) || 1;
        let variantId = $('#selected-variant-id').val();

        // Case 1: No variants exist (simple product)
        if (variants.length === 0) {
            if (qty < 99) {
                $('#quantity').val(qty + 1);
                $('#summary-qty').text(qty + 1);
                updateTotals();
            } else {
                showStockError('Maximum quantity: 99 items');
            }
            return;
        }

        // Case 2: Variants exist but none selected
        if (!variantId) {
            showStockError('⚠️ Please select product options first');
            $('.variant-chips-container').css({
                'border': '2px solid #ff6b6b',
                'padding': '10px',
                'border-radius': '8px'
            });
            setTimeout(() => {
                $('.variant-chips-container').css('border', 'none');
            }, 2000);
            return;
        }

        // Case 3: Variant selected - check stock
        if (maxAvailableStock > 0) {
            if (qty < maxAvailableStock) {
                $('#quantity').val(qty + 1);
                $('#summary-qty').text(qty + 1);
                updateTotals();
            } else {
                showStockError(`Maximum available: ${maxAvailableStock} items`);
                $('#quantity').val(maxAvailableStock);
            }
        } else {
            showStockError('This item is currently out of stock');
        }
    });

    $('#decrease').click(function (e) {
        e.preventDefault();
        clearStockError();

        let qty = parseInt($('#quantity').val()) || 1;

        if (qty > 1) {
            $('#quantity').val(qty - 1);
            $('#summary-qty').text(qty - 1);
            updateTotals();
        } else {
            showStockError('Minimum quantity is 1');
        }
    });

    $('#quantity').on('input change', function () {
        let qty = parseInt($(this).val()) || 1;

        $(this).val(qty);

        if (qty < 1) {
            $(this).val(1);
            showStockError('Minimum quantity is 1');
            return;
        }

        if (maxAvailableStock > 0 && qty > maxAvailableStock) {
            $(this).val(maxAvailableStock);
            showStockError(`Maximum available: ${maxAvailableStock} items`);
            return;
        }

        if (qty > 99) {
            $(this).val(99);
            showStockError('Maximum quantity: 99 items');
            return;
        }

        $('#summary-qty').text(qty);
        updateTotals();
    });

    $('#quantity').on('keypress', function (e) {
        if (e.which < 48 || e.which > 57) {
            e.preventDefault();
        }
    });

    // ==================================================
    // 9. SUBMIT ORDER (MERGED VALIDATION & PAYMENTS)
    // ==================================================

    // ==================================================
    // 9. SUBMIT ORDER (MERGED VALIDATION & PAYMENTS)
    // ==================================================

    // ==================================================
    // 9. SUBMIT ORDER (MERGED VALIDATION & PAYMENTS)
    // ==================================================

$('#order-form').submit(function (e) {
    e.preventDefault();
    
    // ============================================================
    // 1. EXISTING VALIDATIONS (UNTOUCHED)
    // ============================================================
    
    // Capture form reference to use inside SweetAlert later
    var $form = $(this); 

    if (!iti.isValidNumber()) {
        Swal.fire('Error', 'Please enter a valid phone number for the selected country.', 'error');
        return;
    }
    // 1. Reset previous error styles
    $('.input-error').removeClass('input-error');
    $('.variant-chips-container').css({ 'border': 'none', 'padding': '0' });

    // 2. CHECK: Is Email check pending?
    if (isCheckingEmail) {
        Swal.fire({
            title: 'Please wait',
            text: 'Validating email address...',
            icon: 'info',
            timer: 1500,
            showConfirmButton: false
        });
        return;
    }

    // 3. CHECK: Email Error Visible
    const $emailStatus = $('#email-status');
    if ($emailStatus.is(':visible') && $emailStatus.css('color') === 'rgb(255, 0, 0)') {
        $('html, body').animate({ scrollTop: $('#customerEmail').offset().top - 120 }, 300);
        $('#customerEmail').focus();
        return;
    }

    // 4. CHECK: Variant Selected
    let isValid = true;
    let firstErrorField = null;

    if (!$('#selected-variant-id').val()) {
        isValid = false;
        const $variantContainer = $('.variant-chips-container');
        $variantContainer.css({
            'border': '2px solid #dc3545',
            'padding': '5px',
            'border-radius': '5px'
        });
        firstErrorField = $(".variant-selection-area");
    }

    // 5. CHECK: General Required Fields
    $form.find('input[required], select[required], textarea[required]')
        .filter(':visible')
        .each(function () {
            if ($(this).prop('disabled')) return;
            if (!$(this).val() || $(this).val().trim() === "") {
                isValid = false;
                $(this).addClass('input-error');
                if (!firstErrorField) firstErrorField = $(this);
            }
        });

    // 6. IF INVALID: Scroll to error
    if (!isValid || firstErrorField) {
        if (firstErrorField && firstErrorField.length) {
            const elementTop = firstErrorField[0].getBoundingClientRect().top + window.scrollY;
            $('html, body').animate({ scrollTop: elementTop - 120 }, 600);
        }
        return; // STOP HERE IF INVALID
    }

    // 7. CHECK: Stock Limits
    let requestedQty = parseInt($('#quantity').val());
    if (maxAvailableStock > 0 && requestedQty > maxAvailableStock) {
        $('#stock-error-message').text(`ERROR: Requested quantity exceeds limit.`);
        return;
    }

    // --- PAYMENT METHOD & MODE LOGIC ---
    const $selectedCard = $('.payment-option.selected');
    const correctMethodCode = $selectedCard.data('payment');
    const mode = $selectedCard.data('mode');

    // 8. Validation: Manual Mode MUST have a TrxID
    if (mode === 'Manual') {
        const trxId = $('#trx-id-input').val().trim();
        if (!trxId) {
            Swal.fire('Required', 'Please enter the Transaction ID (TrxID).', 'warning');
            $('#trx-id-input').focus().addClass('input-error');
            return; // Stop submission
        }
    }

    // ============================================================
    // ✅ NEW STEP: CONFIRMATION ALERT
    // ============================================================
    Swal.fire({
        title: 'Confirm Order?',
        text: "Are you sure you want to place this order?",
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#2ebf91', // Matches your theme
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, Place Order',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            
            // ============================================================
            // 🧱 EXECUTE YOUR EXISTING SUBMISSION LOGIC HERE
            // ============================================================

            // --- PREPARE DATA ---
            let formData = {};
            // Note: Use $form instead of $(this) because inside .then(), 'this' changes context
            $form.serializeArray().forEach(item => formData[item.name] = item.value);

            // ✅ ROBUST DELIVERY CHARGE CAPTURE
            let deliveryCost = parseFloat($('#receipt-delivery').data('cost'));

            if (isNaN(deliveryCost) || deliveryCost === 0) {
                const dist = $('#district-select').val();
                if (dist && (dist.toLowerCase().includes('dhaka') || dist.trim() === 'Dhaka')) {
                    deliveryCost = delivery.dhaka;
                } else {
                    deliveryCost = delivery.outside;
                }
            }

            formData.DeliveryCharge = deliveryCost; 

            // Apply Payment Values
            formData.PaymentMethod = correctMethodCode;
            formData.CustomerPhone = iti.getNumber();
            formData.PaymentMode = mode;

            if (mode === 'Manual') {
                formData.TransactionReference = $('#trx-id-input').val();
            }

            // Ensure numeric types
            formData.TargetCompanyId = 1;
            formData.ProductVariantId = parseInt(formData.ProductVariantId);
            formData.OrderQuantity = parseInt(formData.OrderQuantity);
            
            // Disable Button
            let $btn = $('#final-submit-btn');
            $btn.prop('disabled', true);

            // Update text based on payment
            if (formData.PaymentMode === 'Gateway') {
                $btn.text('Redirecting...');
            } else {
                $btn.text('Processing...');
            }

            // 
            // SEND TO SERVER
            $.ajax({
                url: '/order/place',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(formData),
                success: function (res) {
                    if (res.success) {
                        // 1. HANDLE COD / MANUAL PAYMENT
                        if (formData.PaymentMethod === 'cod' || formData.PaymentMode === 'Manual') {
                            showOrderSuccessAlert(
                                res.orderId || 'PENDING',
                                formData.CustomerName,
                                formData.CustomerPhone
                            );
                        }
                        // 2. HANDLE ONLINE PAYMENT GATEWAY
                        else {
                            if (res.paymentUrl) {
                                window.location.href = res.paymentUrl;
                            } else {
                                Swal.fire({
                                    title: 'Payment Pending',
                                    text: 'Order created. Redirecting to payment...',
                                    icon: 'info',
                                    showConfirmButton: false,
                                    timer: 2000
                                }).then(() => {
                                    window.location.reload();
                                });
                            }
                        }
                    }
                    // 3. SERVER RETURNED FALSE
                    else {
                        Swal.fire({
                            title: 'Order Failed',
                            text: res.message || "Failed to place order.",
                            icon: 'error',
                            confirmButtonText: 'Try Again'
                        });
                        $btn.prop('disabled', false);
                        updateSubmitButtonText();
                    }
                },
                error: function (xhr) {
                    let errorMessage = "Network Error.";
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMessage = xhr.responseJSON.message;
                    } else if (xhr.responseText) {
                        errorMessage = xhr.responseText.substring(0, 200);
                    }

                    Swal.fire({
                        title: 'Server Error',
                        text: errorMessage,
                        icon: 'error',
                        confirmButtonText: 'Close'
                    });
                    $btn.prop('disabled', false);
                    updateSubmitButtonText();
                }
            });
        }
    });
});    // ==================================================
    // 10. IMAGE GALLERY SLIDER
    // ==================================================
    let currentSlide = 0;
    window.changeSlide = function (dir) {
        const slides = document.querySelectorAll(".slide");
        if (slides.length === 0) return;

        slides[currentSlide].classList.remove("active");
        currentSlide = (currentSlide + dir + slides.length) % slides.length;
        slides[currentSlide].classList.add("active");
    };

    const slides = document.querySelectorAll(".slide");
    if (slides.length > 0) slides[0].classList.add("active");

});

// ==================================================
// 11. POSTAL CODE AUTOFILL (AUTOMATIC)
// ==================================================
$('input[name="PostalCode"]').on('input keyup blur', function () {
    let code = $(this).val().trim();

    if (code.length === 4) {
        let $input = $(this);
        $input.css('border-color', '#3498db');

        $.get('/order/check-postal-code', { code: code }, function (data) {
            if (data.found) {
                $input.css('border-color', '#2ecc71');

                // 1. Set Hidden Division
                $('#hidden-division').val(data.division);

                // 2. Set District Directly (No timeout needed anymore)
                let $distSelect = $('#district-select');
                $distSelect.val(data.district).trigger('change');

                // 3. Load Thana and Suboffice (Keep timeout slightly to allow Thana API to load after District change triggers)
                setTimeout(() => {
                    if (data.thana) {
                        let $thanaSelect = $('#thana-select');
                        // Force load the specific thana option immediately
                        $thanaSelect.empty()
                            .append(`<option value="${data.thana}" selected>${data.thana}</option>`)
                            .prop('disabled', false);

                        // Trigger change to load suboffices
                        $thanaSelect.trigger('change');
                    }

                    // 4. Load SubOffice
                    setTimeout(() => {
                        if (data.subOffice) {
                            let $subSelect = $('#suboffice-select');
                            $subSelect.empty()
                                .append(`<option value="${data.subOffice}" selected>${data.subOffice}</option>`);

                            $subSelect.find(':selected').data('code', code);
                            $subSelect.prop('disabled', false);
                        }
                    }, 300); // Small delay for SubOffice API

                }, 500); // Delay for Thana API (triggered by district change)

            } else {
                $input.css('border-color', '#e74c3c');
            }
        });
    }
});
/* =========================================================
    (Customer Chat Logic)
   ========================================================= */

$(document).ready(function () {
    // --- Shared Variables ---
    let chatConnection = null;
    let chatSessionId = localStorage.getItem("chatSessionId");
    let chatUserName = localStorage.getItem("chatUserName");
    let sessionTimestamp = localStorage.getItem("chatSessionTimestamp");

    const ONE_HOUR = 60 * 60 * 1000;

    // ==================================================
    // 1. SESSION MANAGEMENT
    // ==================================================
    function checkSessionExpiry() {
        const now = new Date().getTime();
        if (sessionTimestamp && (now - sessionTimestamp > ONE_HOUR)) {
            console.log("⚠️ Session expired. Starting fresh.");
            localStorage.removeItem("chatSessionId");
            localStorage.removeItem("chatUserName");
            localStorage.removeItem("chatSessionTimestamp");
            chatSessionId = null;
            chatUserName = null;
        }
    }

    checkSessionExpiry();

    if (!chatSessionId) {
        chatSessionId = generateUUID();
        localStorage.setItem("chatSessionId", chatSessionId);
        localStorage.setItem("chatSessionTimestamp", new Date().getTime());
    }

    function generateUUID() {
        var d = new Date().getTime();
        var d2 = ((typeof performance !== 'undefined') && performance.now && (performance.now() * 1000)) || 0;
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16;
            if (d > 0) {
                r = (d + r) % 16 | 0;
                d = Math.floor(d / 16);
            } else {
                r = (d2 + r) % 16 | 0;
                d2 = Math.floor(d2 / 16);
            }
            return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });
    }

    // ==================================================
    // 2. LOAD HISTORY
    // ==================================================
    function loadChatHistory() {
        $.get('/chat/guest-history?sessionGuid=' + chatSessionId, function (messages) {
            if (messages && messages.length > 0) {
                messages.forEach(function (m) {
                    let type = m.isFromAdmin ? 'incoming' : 'outgoing';
                    let senderName = m.isFromAdmin ? (m.senderName || "Support") : "You";
                    appendCustomerMessage(senderName, m.messageText, type);
                });

                if (!chatUserName) {
                    showChatInterface();
                }
            }
        });
    }

    loadChatHistory();

    // ==================================================
    // 3. SIGNALR CONNECTION
    // ==================================================
    function initSignalR() {
        if (chatConnection) return;

        chatConnection = new signalR.HubConnectionBuilder()
            .withUrl("/supportHub?sessionId=" + chatSessionId)
            .withAutomaticReconnect()
            .build();

        // ✅ LISTENER 1: Admin Reply (AI or Human)
        chatConnection.on("ReceiveReply", function (adminName, message) {
            localStorage.setItem("chatSessionTimestamp", new Date().getTime());

            // Play sound
            var audio = document.getElementById("chat-notification-sound");
            if (audio) {
                audio.play().catch(e => console.log("Audio blocked"));
            }

            // Display message
            appendCustomerMessage(adminName, message, 'incoming');

            // Badge logic if chat is closed
            if (!$('#live-chat-box').is(':visible')) {
                $('#support-widget-btn').addClass('active');
            }
        });

        // ✅ LISTENER 2: System Messages
        chatConnection.on("ReceiveSystemMessage", function (message) {
            const html = `<div class="msg-system">${message}</div>`;
            $('#chat-messages-list').append(html);
            scrollToBottom();
        });

        chatConnection.start()
            .then(() => console.log("✅ Customer Chat Connected"))
            .catch(err => console.error(err));
    }

    initSignalR();

    // ==================================================
    // 4. SEND MESSAGE LOGIC
    // ==================================================
    function sendCustomerMessage() {
        const msg = $('#chat-input-field').val().trim();
        const currentName = chatUserName || "Guest";

        // ✅ Capture the current page's Product ID
        let contextProductId = null;
        if (typeof window.baseProductInfo !== 'undefined' && window.baseProductInfo.id) {
            contextProductId = window.baseProductInfo.id;
        }

        // 🔍 DEBUG: Log what we're about to send
        console.log('[CHAT DEBUG] Sending message:', msg);
        console.log('[CHAT DEBUG] Product ID:', contextProductId);
        console.log('[CHAT DEBUG] baseProductInfo:', window.baseProductInfo);

        if (msg) {
            localStorage.setItem("chatSessionTimestamp", new Date().getTime());

            // 1. Show Local
            appendCustomerMessage("You", msg, 'outgoing');
            $('#chat-input-field').val('');

            // 2. Build message data
            const messageData = {
                SessionGuid: chatSessionId,
                SenderName: currentName,
                MessageText: msg,
                ContextProductId: contextProductId // ✅ Sends the specific Product ID
            };

            // 🔍 DEBUG: Log the actual payload
            console.log('[CHAT DEBUG] Full payload:', JSON.stringify(messageData, null, 2));

            // 3. Send to HTTP Endpoint
            $.ajax({
                url: '/chat/send',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(messageData),
                success: function (response) {
                    console.log('[CHAT DEBUG] ✅ Message sent successfully', response);
                },
                error: function (xhr, status, error) {
                    console.error('[CHAT DEBUG] ❌ Error sending message:', error);
                    console.error('[CHAT DEBUG] Response:', xhr.responseText);
                    appendCustomerMessage("System",
                        "⚠️ Failed to send message. Please check your connection.",
                        'incoming');
                }
            });
        }
    }


    // ==================================================
    // 5. UI HELPERS
    // ==================================================
    function appendCustomerMessage(sender, text, type) {
        const container = $('#chat-messages-list');
        let senderHtml = type === 'incoming' ? `<div class="msg-sender-name">${sender}</div>` : '';

        const html = `
            <div class="msg-${type}">
                ${senderHtml}
                <div class="msg-bubble">${text}</div>
            </div>`;

        container.append(html);
        scrollToBottom();
    }

    function scrollToBottom() {
        const body = document.getElementById("chat-body");
        if (body) body.scrollTop = body.scrollHeight;
    }

    function setUserName(name) {
        chatUserName = name;
        localStorage.setItem("chatUserName", name);
        showChatInterface();
        localStorage.setItem("chatSessionTimestamp", new Date().getTime());
    }

    function checkChatState() {
        if (chatUserName) {
            showChatInterface();
            setTimeout(() => $('#chat-input-field').focus(), 300);
        } else {
            $('#chat-name-screen').css('display', 'flex');
            $('#chat-messages-list').hide();
            $('#chat-footer').css('display', 'none');
        }
    }

    function showChatInterface() {
        $('#chat-name-screen').hide();
        $('#chat-messages-list').css('display', 'flex');
        $('#chat-footer').css('display', 'flex');
    }

    // ==================================================
    // 6. EVENT BINDINGS
    // ==================================================
    $('#chat-send-btn').click(sendCustomerMessage);

    $('#chat-input-field').keypress(function (e) {
        if (e.which == 13) sendCustomerMessage();
    });

    $('#support-widget-btn').click(function () {
        $(this).toggleClass('active');
        const menu = $('#support-options');
        const icon = $(this).find('i');

        if (menu.hasClass('show')) {
            menu.removeClass('show');
            $('#live-chat-box').fadeOut();
            icon.removeClass('fa-times').addClass('fa-headset');
        } else {
            menu.addClass('show');
            $('#live-chat-box').hide();
            icon.removeClass('fa-headset').addClass('fa-times');
        }
    });

    $('#btn-open-live-chat').click(function () {
        $('#support-options').removeClass('show');
        const mainBtn = $('#support-widget-btn');
        mainBtn.addClass('active');
        mainBtn.find('i').removeClass('fa-headset').addClass('fa-times');
        $('#live-chat-box').fadeIn().css('display', 'flex');
        checkChatState();
    });

    $('#chat-close-btn').click(function () {
        $('#live-chat-box').fadeOut();
        const mainBtn = $('#support-widget-btn');
        mainBtn.removeClass('active');
        mainBtn.find('i').removeClass('fa-times').addClass('fa-headset');
    });

    $('#chat-start-btn').click(function () {
        const name = $('#chat-guest-name').val().trim();
        if (name) setUserName(name);
        else $('#chat-guest-name').css('border-color', 'red');
    });

    $('#chat-skip-btn').click(function () {
        setUserName("Guest");
    });

    // ==================================================
    // 7. SCROLL TO TOP WITH PROGRESS RING
    // ==================================================
    var progressPath = document.querySelector('.progress-wrap path');

    if (progressPath) {
        var pathLength = progressPath.getTotalLength();

        progressPath.style.transition = progressPath.style.WebkitTransition = 'none';
        progressPath.style.strokeDasharray = pathLength + ' ' + pathLength;
        progressPath.style.strokeDashoffset = pathLength;
        progressPath.getBoundingClientRect();
        progressPath.style.transition = progressPath.style.WebkitTransition = 'stroke-dashoffset 10ms linear';

        var updateProgress = function () {
            var scroll = $(window).scrollTop();
            var height = $(document).height() - $(window).height();
            var progress = pathLength - (scroll * pathLength / height);
            progressPath.style.strokeDashoffset = progress;
        }

        updateProgress();
        $(window).scroll(updateProgress);

        var offset = 50;

        $(window).on('scroll', function () {
            if ($(this).scrollTop() > offset) {
                $('.progress-wrap').addClass('active-progress');
            } else {
                $('.progress-wrap').removeClass('active-progress');
            }
        });

        $('.progress-wrap').on('click', function (event) {
            event.preventDefault();
            $('html, body').animate({ scrollTop: 0 }, 550);
            return false;
        });
    }
});


function showOrderSuccessAlert(orderId, customerName, customerPhone) {
    Swal.fire({
        title: 'Order Placed Successfully! 🎉',
        icon: 'success',
        // HTML Content for the "Receipt" look
        html: `
            <div style="text-align: left; margin-top: 10px;">
                <p style="font-size: 1.1em; color: #333;">Dear <b>${customerName}</b>,</p>
                <p style="color: #666;">Thank you for your order! We have received your request.</p>
                
                <div style="background: #f0fdf4; padding: 15px; border-radius: 8px; border: 2px dashed #2ebf91; margin: 15px 0; text-align: center;">
                    <p style="margin: 0; font-size: 0.85em; text-transform: uppercase; letter-spacing: 1px; color: #555;">Order Tracking ID</p>
                    <h2 style="margin: 5px 0 0 0; color: #2ebf91; font-family: monospace; font-size: 24px;">${orderId}</h2>
                </div>

                <p style="font-size: 0.9em; color: #666; display: flex; align-items: center; gap: 8px;">
                    <i class="fas fa-phone-alt" style="color: #2ebf91;"></i> 
                    We will contact you at <b>${customerPhone}</b> shortly.
                </p>
            </div>
        `,
        // Button Styling
        confirmButtonText: '<i class="fas fa-search-location"></i> Track Order',
        confirmButtonColor: '#2ebf91', // Your Theme Green
        showCancelButton: true,
        cancelButtonText: 'Close',
        cancelButtonColor: '#6c757d',
        allowOutsideClick: false,
        allowEscapeKey: false,
        customClass: {
            popup: 'animated fadeInDown' // Optional animation
        }
    }).then((result) => {
        if (result.isConfirmed) {
            // 1. Click the "Track My Order" button in your header to open the modal
            $('#open-status-modal').click();

            // 2. Pre-fill the input inside that modal (if input has id="track-order-id")
            setTimeout(() => {
                $('#track-order-id').val(orderId);
            }, 300);
        } else {
            // Reload if they click Close
            window.location.reload();
        }
    });
}