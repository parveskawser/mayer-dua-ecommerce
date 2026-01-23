// wwwroot/js/combo.js

// ==================================================
// 1. ORDER API HELPERS
// ==================================================
window.OrderAPI = {
    checkCustomer: async function (phone) {
        try {
            const response = await fetch(`/order/check-customer?phone=${phone}`);
            if (!response.ok) return { found: false };
            return await response.json();
        } catch (e) { return { found: false }; }
    },
    checkPostalCode: async function (code) {
        try {
            const response = await fetch(`/order/check-postal-code?code=${code}`);
            if (!response.ok) return { found: false };
            return await response.json();
        } catch (e) { return { found: false }; }
    },
    getDivisions: async function () {
        try { const r = await fetch('/order/get-divisions'); return await r.json(); } catch (e) { return []; }
    },
    getDistricts: async function (div) {
        try { const r = await fetch(`/order/get-districts?division=${div}`); return await r.json(); } catch (e) { return []; }
    },
    getThanas: async function (dist) {
        try { const r = await fetch(`/order/get-thanas?district=${dist}`); return await r.json(); } catch (e) { return []; }
    },
    getSubOffices: async function (thana) {
        try { const r = await fetch(`/order/get-suboffices?thana=${thana}`); return await r.json(); } catch (e) { return []; }
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
    // HELPER: Force Scroll to Element (Fixes Sticky Header issues)
    // ✅ MOVED HERE so the Submit Button can find it
    // ==================================================
    function forceScrollTo(element) {
        if (!element || element.length === 0) return;

        // 1. Get the DOM object
        const domNode = element[0];

        // 2. Calculate position: Absolute Top - Header Offset (150px)
        const headerOffset = 150;
        const elementPosition = domNode.getBoundingClientRect().top;
        const offsetPosition = elementPosition + window.pageYOffset - headerOffset;

        // 3. Scroll Manually
        window.scrollTo({
            top: offsetPosition,
            behavior: "smooth"
        });

        // 4. Focus if it's an input (opens keyboard on mobile)
        if (element.is('input, select, textarea')) {
            setTimeout(() => {
                element.focus({ preventScroll: true });
            }, 300);
        }
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
    // Helper to update the Submit Button text dynamically
    function updateSubmitButtonText(grandTotal) {
        const $btn = $('#final-submit-btn');
        const $selectedCard = $('.payment-option.selected');

        if ($selectedCard.length > 0) {
            const methodCode = $selectedCard.data('payment'); // e.g. 'cod', 'bkash'
            const mode = $selectedCard.data('mode');          // 'Manual', 'Gateway'

            if (methodCode === 'cod') {
                $btn.text('Confirm Order (Cash on Delivery)');
            } else if (mode === 'Manual') {
                $btn.text('Verify & Confirm Order');
            } else {
                $btn.text(`Pay Tk. ${Math.floor(grandTotal).toLocaleString()} Now`);
            }
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
    // const variants = (window.productVariants || []).filter(v => v);
    // if (variants.length === 1) {
    //     const singleVariant = variants[0];
    //     applyVariantData(singleVariant);
    //     $('.variant-chip').addClass('selected');
    //
    //     // Populate selectedAttributes for the single variant
    //     if (singleVariant.attributes) {
    //         selectedAttributes = { ...singleVariant.attributes };
    //     }
    // }

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
    // function updateAttributeAvailability() {
    //     $('.variant-chip').each(function () {
    //         const $chip = $(this);
    //         const chipAttribute = $chip.data('attribute');
    //
    //         // CHANGE: Use .attr() to ensure String comparison
    //         const chipValue = $chip.attr('data-value');
    //
    //         // Skip if this specific chip is already selected
    //         if ($chip.hasClass('selected')) {
    //             $chip.removeClass('disabled');
    //             return;
    //         }
    //
    //         // Create a "Test Scenario"
    //         const testSelection = { ...selectedAttributes };
    //
    //         // Allow switching values within the same attribute group
    //         delete testSelection[chipAttribute];
    //         testSelection[chipAttribute] = chipValue;
    //
    //         // Check if ANY variant matches this test scenario
    //         const isCompatible = variants.some(v => {
    //             for (const [key, val] of Object.entries(testSelection)) {
    //                 // CHANGE: Convert both sides to String() before comparing
    //                 if (!v.attributes || String(v.attributes[key]) !== String(val)) {
    //                     return false;
    //                 }
    //             }
    //             return true;
    //         });
    //
    //         // Apply visual state
    //         if (isCompatible) {
    //             $chip.removeClass('disabled');
    //         } else {
    //             $chip.addClass('disabled');
    //         }
    //     });
    // }
    // Call once on load to initialize states
    // updateAttributeAvailability();

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
    }

    // function applyVariantData(variant) {
    //     $('#selected-variant-id').val(variant.id);
    //     currentVariantPrice = variant.price;
    //     $('#display-price').text('Tk. ' + currentVariantPrice.toLocaleString());
    //
    //     // Handle stock properly
    //     maxAvailableStock = variant.stock;
    //
    //     let currentQty = parseInt($('#quantity').val()) || 1;
    //     if (maxAvailableStock > 0 && currentQty > maxAvailableStock) {
    //         $('#quantity').val(maxAvailableStock);
    //     }
    //     updateStockMessage(maxAvailableStock);
    //
    //     // 1. Determine the image URL
    //     let newImg = variant.image && variant.image.length > 1 ? variant.image : baseProductImageUrl;
    //
    //     // 2. Fix Slash logic (Don't add slash if image is missing)
    //     if (newImg && newImg.length > 0 && !newImg.startsWith("/") && !newImg.startsWith("http")) {
    //         newImg = "/" + newImg;
    //     }
    //
    //     // 3. Update Image Src AND Force Visibility
    //     // We use .show() to undo the 'display:none' set by the onerror event
    //     $('#order-variant-image').attr('src', newImg).show();
    //     $('#mobile-order-variant-image').attr('src', newImg).show();
    //
    //     $('.variant-chips-container').css('border', 'none');
    //     updateTotals();
    //
    //     if (!isCheckingEmail && !$('#email-status').is(':visible')) {
    //         $('.submit-btn').prop('disabled', false);
    //     }
    // }

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
        // 3. Format Validation
        if (!email.includes('@')) {
            $msg.text("⚠ Please enter a valid email address").css('color', 'orange').show();
            // Change text to 'Fix Email'
            $('.submit-btn').prop('disabled', false).text('Fix Email');
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
                $msg.text("⚠ This email is already registered with a different phone number.").css('color', 'red').show();

                // ✅ FIX: Keep button ENABLED so user can click it to be redirected
                $('.submit-btn').prop('disabled', false).text('Fix Email');
            } else {
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

    // ==================================================
    // 4. CORE LOGIC: UPDATE RECEIPT & TOTALS
    // ==================================================
    // ==================================================
    // CORE LOGIC: UPDATE RECEIPT & TOTALS
    // ==================================================
    // ==================================================
    // UPDATED TOTALS LOGIC FOR RECEIPT
    // ==================================================

    function updateTotals() {
        let subtotal = 0;
        let receiptHtml = "";

        // 1. Loop through all checked variant checkboxes
        $('.variant-select-chk:checked').each(function () {
            const $card = $(this).closest('.variant-row-card');
            const name = $(this).data('name');
            const price = parseFloat($(this).data('price')) || 0;
            const qty = parseInt($card.find('.row-qty-input').val()) || 1;

            const lineTotal = price * qty;
            subtotal += lineTotal;

            // Add row to the receipt summary
            receiptHtml += `
            <tr>
                <td>${name}</td>
                <td style="text-align: center;">${qty}</td>
                <td style="text-align: right;">Tk. ${lineTotal.toLocaleString()}</td>
            </tr>`;
        });

        // 2. Update the Receipt UI table body
        if (receiptHtml === "") {
            $('#receipt-body').html('<tr><td colspan="3" style="text-align:center; color:#999;">No items selected</td></tr>');
        } else {
            $('#receipt-body').html(receiptHtml);
        }

        // 3. Handle Delivery and Grand Total
        const deliveryCharge = parseFloat($('#receipt-delivery').data('cost')) || 0;
        const grandTotal = subtotal + deliveryCharge;

        // 4. Update the visual totals in the receipt
        $('#receipt-grand-total').text('Tk. ' + grandTotal.toLocaleString());

        // 5. Update the dynamic Submit Button text (if applicable)
        if (typeof updateSubmitButtonText === "function") {
            updateSubmitButtonText(grandTotal);
        }
    }
    // ==================================================
    // EVENT LISTENERS FOR QUANTITY AND SELECTION
    // ==================================================
    //     $(document).ready(function () {
    //         // Checkbox Change
    //         $(document).on('change', '.variant-select-chk', function() {
    //             updateTotals();
    //         });
    //
    //         // Quantity Plus/Minus Buttons
    //         $(document).on('click', '.row-qty-btn', function() {
    //             const $input = $(this).siblings('.row-qty-input');
    //             let val = parseInt($input.val());
    //             const max = parseInt($input.attr('max'));
    //
    //             if ($(this).hasClass('plus')) {
    //                 if (val < max) val++;
    //             } else {
    //                 if (val > 1) val--;
    //             }
    //
    //             $input.val(val);
    //             updateTotals(); // Trigger live update
    //         });
    //     });    
    // Helper: Format Currency
    function formatCurrency(amount) {
        return 'Tk. ' + Math.floor(amount).toLocaleString();
    }
    // ==================================================
    // 8. QUANTITY CONTROLS
    // ==================================================

    // ==================================================
    // 8. QUANTITY CONTROLS
    // ==================================================
    // A. Checkbox Toggle
    // ==================================================
    // 8. QUANTITY & CHECKBOX CONTROLS (Correct Location)
    // ==================================================

    // 
    // ==================================================
    // 8. QUANTITY CONTROLS & REAL-TIME STOCK UPDATE
    // ==================================================

    // 1. Function to recalculate stock text (e.g., 30 - 2 = 28)
    function updateRealTimeStock($row) {
        if (!$row || $row.length === 0) return;

        var $chk = $row.find('.variant-select-chk');
        var $stockSpan = $row.find('.dynamic-stock');
        var $notifyBtn = $row.find('.back-in-stock-btn');
        var $qtyInput = $row.find('.row-qty-input');
        var $qtyBtns = $row.find('.row-qty-btn');

        // Always read the latest stock from the checkbox attribute (this one we update after AJAX refresh)
        var rawStock = $chk.attr('data-stock');
        var maxStock = parseInt(rawStock, 10);
        if (!Number.isFinite(maxStock) || maxStock < 0) maxStock = 0;

        // Keep span's data-max in sync for legacy usage / markup expectations
        if ($stockSpan.length > 0) {
            $stockSpan.data('max', maxStock);
            $stockSpan.attr('data-max', String(maxStock));
        }

        if ($notifyBtn.length > 0) {
            const alreadyRequested = $notifyBtn.data('requested') === true || $notifyBtn.attr('data-requested') === 'true';
            if (maxStock <= 0) {
                $notifyBtn.show();
                if (alreadyRequested) {
                    $notifyBtn.prop('disabled', true).text('Request received');
                } else {
                    $notifyBtn.prop('disabled', false).text('Notify me');
                }
            } else {
                $notifyBtn.hide();
            }
        }

        // If out of stock -> hard disable the whole row selection + qty controls
        if (maxStock <= 0) {
            // Uncheck and disable checkbox (so row click can't re-enable it)
            $chk.prop('checked', false).prop('disabled', true);

            // Disable qty controls & force qty = 1
            $qtyInput.val(1).prop('disabled', true);
            $qtyBtns.prop('disabled', true);

            // Visual + text
            if ($stockSpan.length > 0) {
                $stockSpan.text('(Out of Stock)').css('color', '#dc3545');
            }

            // Optional: add a CSS hook
            $row.addClass('is-out-of-stock').css('opacity', 0.7);

            return;
        }

        // In stock -> enable controls
        $chk.prop('disabled', false);
        $qtyInput.prop('disabled', false);
        $qtyBtns.prop('disabled', false);
        $row.removeClass('is-out-of-stock').css('opacity', 1);

        // Clamp qty to [1..maxStock]
        var currentQty = parseInt($qtyInput.val(), 10);
        if (!Number.isFinite(currentQty) || currentQty < 1) currentQty = 1;
        if (currentQty > maxStock) currentQty = maxStock;
        $qtyInput.val(currentQty);

        // Remaining = Max - Quantity in cart (row)
        var remaining = maxStock - currentQty;
        if (remaining < 0) remaining = 0;

        if ($stockSpan.length > 0) {
            $stockSpan.text('(Stock: ' + remaining + ')');

            // Turn text RED if stock hits 0
            if (remaining === 0) {
                $stockSpan.css('color', '#dc3545');
            } else {
                $stockSpan.css('color', '#777');
            }
        }

        // Keep receipt + totals accurate if something auto-clamped
        // (Don't spam: only update if this row is selected)
        if ($chk.is(':checked')) {
            updateTotals();
        }
    }

    // 2. Event: Checkbox Changed
    $(document).on('change', '.variant-select-chk', function () {
        const $row = $(this).closest('.variant-row-card');
        if ($(this).is(':checked')) {
            $row.css('border-color', '#2ebf91').css('background-color', '#f0fdf4');
        } else {
            $row.css('border-color', '#e0e0e0').css('background-color', '#fff');
        }

        // Keep the displayed stock text + disabled state always correct
        updateRealTimeStock($row);

        updateTotals();
    });

    // 3. Event: Row Click (Selects product)
    $(document).on('click', '.variant-row-card', function (e) {
        // Ignore clicks on inputs/buttons/images
        if ($(e.target).closest('.variant-select-chk, .row-qty-btn, input, img').length) return;

        const $chk = $(this).find('.variant-select-chk');

        // If checkbox is disabled (out of stock), do nothing
        if ($chk.prop('disabled')) return;

        // If stock is 0, keep it disabled
        const stockNow = (function () {
            const n = parseInt($chk.attr('data-stock'), 10);
            return (Number.isFinite(n) && n >= 0) ? n : 0;
        })();
        if (stockNow <= 0) {
            updateRealTimeStock($(this));
            return;
        }

        $chk.prop('checked', !$chk.is(':checked')).trigger('change');
    });

    // 4. Event: User Types Quantity Manually
    $(document).on('input change', '.row-qty-input', function () {
        const $row = $(this).closest('.variant-row-card');
        const $chk = $row.find('.variant-select-chk');

        let val = parseInt($(this).val());
        const maxStock = (function () { const n = parseInt($chk.attr('data-stock'), 10); return (Number.isFinite(n) && n > 0) ? n : 0; })();

        // If out of stock, force disable + stop
        if (maxStock <= 0) {
            updateRealTimeStock($row);
            return;
        }

        if (isNaN(val) || val < 1) $(this).val(1);
        if (val > maxStock) $(this).val(maxStock);

        // ✅ UPDATE STOCK TEXT HERE
        updateRealTimeStock($row);

        if (!$chk.is(':checked')) {
            $chk.prop('checked', true).trigger('change');
        } else {
            updateTotals();
        }
    });

    // 5. Event: Plus / Minus Buttons Clicked
    $(document).off('click', '.row-qty-btn').on('click', '.row-qty-btn', function (e) {
        e.preventDefault();
        e.stopPropagation();

        const $btn = $(this);
        const $row = $btn.closest('.variant-row-card');
        const $input = $row.find('.row-qty-input');
        const $checkbox = $row.find('.variant-select-chk');

        let currentVal = parseInt($input.val()) || 0;
        const maxStock = (function () { const n = parseInt($checkbox.attr('data-stock'), 10); return (Number.isFinite(n) && n > 0) ? n : 0; })();

        // If out of stock, stop
        if (maxStock <= 0) {
            updateRealTimeStock($row);
            return;
        }

        if ($btn.hasClass('plus')) {
            if (currentVal < maxStock) {
                $input.val(currentVal + 1);
            } else {
                if (typeof Swal !== 'undefined') {
                    Swal.fire({ toast: true, icon: 'warning', title: `Max Stock: ${maxStock}`, position: 'top-end', timer: 1500, showConfirmButton: false });
                }
            }
        } else {
            if (currentVal > 1) {
                $input.val(currentVal - 1);
            }
        }

        // ✅ CRITICAL: UPDATE STOCK TEXT HERE IMMEDIATELY
        updateRealTimeStock($row);

        // Auto-select checkbox
        if (!$checkbox.is(':checked')) {
            $checkbox.prop('checked', true).trigger('change');
        } else {
            updateTotals();
        }
    });

    // ==================================================


    // ==================================================
    // 8.1 LIVE STOCK REFRESH (AJAX) + BACK BUTTON FIX
    // ==================================================

    // Uses a lightweight endpoint to fetch latest stocks without reloading the whole page.
    // Backend expected: GET /order/get-variant-stocks?productId=123&variantIds=1,2,3
    // Response can be an array or { items: [...] } where each item contains (variantId/id) and (stock/stockQty/StockQty).
    function refreshAllVariantStocks(options) {
        options = options || {};
        const $all = $('.variant-select-chk');

        if ($all.length === 0) {
            // Nothing to refresh
            return $.Deferred().resolve().promise();
        }

        const ids = $all.map(function () { return parseInt($(this).val(), 10); })
            .get()
            .filter(x => Number.isFinite(x) && x > 0);

        if (ids.length === 0) {
            return $.Deferred().resolve().promise();
        }

        return $.ajax({
            url: '/product/variant-stocks',
            type: 'GET',
            dataType: 'json',
            cache: false,
            data: {
                ids: ids.join(',')
            }
        }).done(function (res) {
            let items = res;

            if (res && Array.isArray(res.items)) items = res.items;
            if (res && Array.isArray(res.data)) items = res.data;

            if (!Array.isArray(items)) return;

            // Normalize to map: variantId -> stock
            const map = {};
            items.forEach(x => {
                if (!x) return;

                const id = parseInt(
                    (x.variantId ?? x.VariantId ?? x.productVariantId ?? x.ProductVariantId ?? x.id ?? x.Id),
                    10
                );
                const stock = parseInt(
                    (x.stock ?? x.Stock ?? x.stockQty ?? x.StockQty ?? x.qty ?? x.Qty),
                    10
                );

                if (Number.isFinite(id) && id > 0) {
                    map[id] = (Number.isFinite(stock) && stock >= 0) ? stock : 0;
                }
            });

            // Apply to DOM
            $('.variant-row-card').each(function () {
                const $row = $(this);
                const $chk = $row.find('.variant-select-chk');
                const id = parseInt($chk.val(), 10);
                if (!Number.isFinite(id) || id <= 0) return;

                if (map.hasOwnProperty(id)) {
                    const newStock = map[id];

                    // Update the source of truth attribute (used by qty handlers)
                    $chk.attr('data-stock', String(newStock));
                    $chk.data('stock', newStock); // keep jQuery cache in sync

                    // Keep span in sync too
                    const $span = $row.find('.dynamic-stock');
                    if ($span.length > 0) {
                        $span.attr('data-max', String(newStock));
                        $span.data('max', newStock);
                    }

                    // Re-apply disable + clamp + stock text
                    updateRealTimeStock($row);

                    // If it became out of stock while selected, ensure it's unselected
                    if (newStock <= 0) {
                        $chk.prop('checked', false).trigger('change');
                    }
                }
            });
        }).fail(function () {
            // Silent fail: don't block ordering UI if refresh endpoint isn't available yet
            if (!options.silent && typeof console !== 'undefined') {
                console.warn('Stock refresh failed (endpoint missing or network issue).');
            }
        });
    }

    // Initialize row states once on load (disables out-of-stock rows immediately)
    $('.variant-row-card').each(function () {
        updateRealTimeStock($(this));
    });

    // Back button / BFCache: refresh stocks when user returns to this page
    window.addEventListener('pageshow', function (evt) {
        // persisted=true means loaded from BFCache (back/forward)
        if (evt && evt.persisted) {
            refreshAllVariantStocks({ silent: true });
        } else {
            // Also refresh on normal pageshow; it's cheap and avoids stale UI
            refreshAllVariantStocks({ silent: true });
        }
    });

    // Optional: keep UI fresh while user stays on the page (no full refresh, only AJAX)
    setInterval(function () {
        refreshAllVariantStocks({ silent: true });
    }, 5000);

    // ==================================================
    // 8.2 BACK IN STOCK REQUEST MODAL
    // ==================================================
    let lastBackInStockButton = null;

    function openBackInStockModal(variantId, variantName) {
        $('#back-in-stock-variant-id').val(variantId);
        $('#back-in-stock-variant-name').text(variantName || 'this item');

        const prefFill = $('#customerPhone').val();
        if (prefFill && !$('#back-in-stock-phone').val()) {
            $('#back-in-stock-phone').val(prefFill);
        }

        $('#back-in-stock-modal').addClass('open');
        $('#back-in-stock-phone').focus();
    }

    function closeBackInStockModal() {
        $('#back-in-stock-modal').removeClass('open');
        $('#back-in-stock-phone').val('');
        $('#back-in-stock-variant-id').val('');
    }

    $(document).on('click', '.back-in-stock-btn', function (e) {
        e.preventDefault();
        e.stopPropagation();

        const $btn = $(this);
        if ($btn.prop('disabled')) {
            return;
        }

        lastBackInStockButton = $btn;
        const variantId = $btn.data('variant-id');
        const variantName = $btn.data('variant-name');
        openBackInStockModal(variantId, variantName);
    });

    $(document).on('click', '#back-in-stock-close, #back-in-stock-cancel', function () {
        closeBackInStockModal();
    });

    $(document).on('submit', '#back-in-stock-request-form', function (e) {
        e.preventDefault();

        const variantId = parseInt($('#back-in-stock-variant-id').val(), 10);
        const phone = $('#back-in-stock-phone').val().trim();

        if (!variantId || !phone) {
            if (typeof Swal !== 'undefined') {
                Swal.fire('Missing info', 'Please enter your mobile number.', 'warning');
            } else {
                alert('Please enter your mobile number.');
            }
            return;
        }

        const token = $('#back-in-stock-token-form input[name="__RequestVerificationToken"]').val();

        const payload = {
            ProductVariantId: variantId,
            ContactNumber: phone
        };

        const $submitBtn = $('#back-in-stock-submit');
        const originalText = $submitBtn.text();
        $submitBtn.prop('disabled', true).text('Submitting...');

        fetch('/product/back-in-stock-request', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(payload)
        }).then(res => res.json())
            .then(res => {
                if (res && res.success) {
                    if (lastBackInStockButton) {
                        lastBackInStockButton
                            .data('requested', true)
                            .attr('data-requested', 'true')
                            .prop('disabled', true)
                            .text(res.alreadyRequested ? 'Request received' : 'Request received');
                    }

                    if (typeof Swal !== 'undefined') {
                        Swal.fire('Success', res.message || 'Request submitted!', 'success');
                    } else {
                        alert(res.message || 'Request submitted!');
                    }

                    closeBackInStockModal();
                } else {
                    const message = res && res.message ? res.message : 'Request failed. Please try again.';
                    if (typeof Swal !== 'undefined') {
                        Swal.fire('Error', message, 'error');
                    } else {
                        alert(message);
                    }
                }
            })
            .catch(() => {
                if (typeof Swal !== 'undefined') {
                    Swal.fire('Error', 'Unable to submit request right now.', 'error');
                } else {
                    alert('Unable to submit request right now.');
                }
            })
            .finally(() => {
                $submitBtn.prop('disabled', false).text(originalText);
            });
    });

    // 9. SUBMIT ORDER (MERGED VALIDATION & PAYMENTS)
    // ==================================================

    // ==================================================
    // 9. SUBMIT ORDER (MERGED VALIDATION & PAYMENTS)
    // ==================================================

    // ==================================================
    // 9. SUBMIT ORDER (MERGED VALIDATION & PAYMENTS)
    // ==================================================

    // ==================================================
    // 6. SUBMIT ORDER
    // ==================================================
    $('#order-form').submit(function (e) {
        e.preventDefault();
        var $form = $(this);

        // 1. Phone Check
        if (typeof iti !== 'undefined' && iti && !iti.isValidNumber()) {
            Swal.fire('Error', 'Please enter a valid phone number.', 'error');
            forceScrollTo($('#customerPhone'));
            return;
        }

        // 2. Gather Items from Checkboxes
        const selectedItems = [];
        $('.variant-select-chk:checked').each(function () {
            const $row = $(this).closest('.variant-row-card');
            selectedItems.push({
                ProductVariantId: parseInt($(this).val()),
                OrderQuantity: parseInt($row.find('.row-qty-input').val()) || 1
            });
        });

        if (selectedItems.length === 0) {
            $('#variant-error-msg').show();
            forceScrollTo($('.variant-list-section'));
            return;
        }

        // 3. Required Fields Validation (Redirects to Error)
        let isValid = true;
        let firstError = null;
        $form.find('input[required], select[required], textarea[required]').each(function () {
            if ($(this).is(':visible') && (!$(this).val() || $(this).val().trim() === "")) {
                isValid = false;
                $(this).css('border', '1px solid red');
                if (!firstError) firstError = $(this);
            } else {
                $(this).css('border', '');
            }
        });

        if (!isValid) {
            if (firstError) forceScrollTo(firstError);
            return;
        }

        // 4. Confirm & Submit
        Swal.fire({
            title: 'Confirm Order?',
            text: "Are you sure you want to place this order?",
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#2ebf91',
            confirmButtonText: 'Yes, Place Order'
        }).then((result) => {
            if (result.isConfirmed) {

                // --- PREPARE DATA ---
                let formData = {};
                $form.serializeArray().forEach(item => formData[item.name] = item.value);
                if (typeof iti !== 'undefined' && iti) formData.CustomerPhone = iti.getNumber();

                // Payment Info
                const $selectedCard = $('.payment-option.selected');
                formData.PaymentMethod = $selectedCard.data('payment');
                formData.PaymentMode = $selectedCard.data('mode');
                if (formData.PaymentMode === 'Manual') formData.TransactionReference = $('#trx-id-input').val();

                // Financials & Items
                formData.DeliveryCharge = parseFloat($('#receipt-delivery').data('cost')) || 0;
                formData.OrderItems = selectedItems; // List of items

                // Legacy Fallback (Send first item for older backends)
                formData.ProductVariantId = selectedItems[0].ProductVariantId;
                formData.OrderQuantity = selectedItems[0].OrderQuantity;

                // Send Request
                let $btn = $('#final-submit-btn');
                $btn.prop('disabled', true).text('Processing...');

                $.ajax({
                    url: '/order/place',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(formData),

                    success: function (res) {
                        console.log('Response:', res);

                        var isSuccess =
                            res &&
                            (
                                res.success === true ||
                                res.Success === true ||
                                res.success === 1 ||
                                res.success === "true"
                            );

                        if (isSuccess) {
                            // --- NEW CODE START ---
                            // 1. Attempt to find the Order ID in the response
                            // Adjust these keys based on what your API actually returns (e.g., res.data.id, res.orderId)
                            var newOrderId = res.orderId || res.OrderId || res.id || res.Id || (res.data ? res.data.id : null);

                            if (newOrderId) {
                                // 2. Set a short-lived cookie (60 seconds) to carry the ID to the next page
                                document.cookie = "CurrentOrderId=" + newOrderId + "; path=/; max-age=60";
                            }
                            // --- NEW CODE END ---

                            refreshAllVariantStocks({ silent: true }).always(function () {
                                window.location.href = '/order/confirmation';
                            });
                        } else {
                            Swal.fire(
                                'Failed',
                                res && res.message ? res.message : 'Order Failed',
                                'error'
                            );
                            $btn.prop('disabled', false);
                            updateTotals();
                        }
                    }
                    ,
                    error: function (xhr) {
                        let msg =
                            xhr.responseJSON && xhr.responseJSON.message
                                ? xhr.responseJSON.message
                                : 'Network Error';

                        Swal.fire('Error', msg, 'error');

                        $btn.prop('disabled', false);
                        updateTotals();
                    }
                });
            }
        });
    });
    // ==================================================
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
// ==================================================
// 11. POSTAL CODE AUTOFILL (AUTOMATIC)
// ==================================================
//$('input[name="PostalCode"]').on('input keyup blur', function () {
//    let code = $(this).val().trim();
//    let $input = $(this);
//    let $status = $('#postal-status'); // The new message container

//    // 1. Reset state while typing or if empty
//    if (code.length < 4) {
//        $input.css('border-color', ''); // Reset border
//        $status.hide(); // Hide message
//        return;
//    }

//    // 2. Trigger check when 4 digits are entered
//    if (code.length === 4) {
//        $input.css('border-color', '#3498db'); // Blue (Loading)
//        $status.text("Checking...").css('color', '#3498db').show();

//        $.get('/order/check-postal-code', { code: code }, function (data) {
//            if (data.found) {
//                // SUCCESS: Found in DB
//                $input.css('border-color', '#2ecc71'); // Green
//                $status.text("✓ Location found").css('color', '#2ecc71').show();

//                // --- Existing Autofill Logic ---
//                $('#hidden-division').val(data.division);
//                let $distSelect = $('#district-select');
//                $distSelect.val(data.district).trigger('change');

//                setTimeout(() => {
//                    if (data.thana) {
//                        let $thanaSelect = $('#thana-select');
//                        $thanaSelect.empty()
//                            .append(`<option value="${data.thana}" selected>${data.thana}</option>`)
//                            .prop('disabled', false)
//                            .trigger('change');
//                    }
//                    setTimeout(() => {
//                        if (data.subOffice) {
//                            let $subSelect = $('#suboffice-select');
//                            $subSelect.empty()
//                                .append(`<option value="${data.subOffice}" selected>${data.subOffice}</option>`);
//                            $subSelect.find(':selected').data('code', code);
//                            $subSelect.prop('disabled', false);
//                        }
//                    }, 300);
//                }, 500);

//            } else {
//                // ERROR: Not found in DB
//                $input.css('border-color', '#e74c3c'); // Red
//                $status.text("⚠ Postal code not found. Please select location manually.")
//                    .css('color', '#e74c3c')
//                    .show();
//            }
//        }).fail(function () {
//            // Handle Network/Server Error
//            $input.css('border-color', '#e74c3c');
//            $status.text("⚠ Error checking code").css('color', '#e74c3c').show();
//        });
//    }
//});

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


    // SAFETY CHECK: If we are on the Admin page...
    if ($('#order-form').length === 0) {
        return;
    }

    // ==================================================
    // HELPER: Force Scroll to Element (Fixes Sticky Header issues)
    // ✅ MOVED HERE so the Submit Button can find it
    // ==================================================
    function forceScrollTo(element) {
        if (!element || element.length === 0) return;

        const domNode = element[0];
        const headerOffset = 150;
        const elementPosition = domNode.getBoundingClientRect().top;
        const offsetPosition = elementPosition + window.pageYOffset - headerOffset;

        window.scrollTo({
            top: offsetPosition,
            behavior: "smooth"
        });

        if (element.is('input, select, textarea')) {
            setTimeout(() => {
                element.focus({ preventScroll: true });
            }, 300);
        }
    }
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
        // ==================================================
        // FIX: Inject Close Button on Mobile if Missing
        // ==================================================
        function ensureChatCloseButton() {
            if ($(window).width() <= 768) {
                var $chatBox = $('#live-chat-box');
                var $closeBtn = $('#chat-close-btn');

                // If chat box exists but button is missing
                if ($chatBox.length > 0 && $closeBtn.length === 0) {
                    // Append the button manually
                    $chatBox.append('<div id="chat-close-btn"><i class="fas fa-times"></i></div>');

                    // Bind the click event to the new button
                    $(document).on('click', '#chat-close-btn', function () {
                        $('#live-chat-box').fadeOut();
                        var mainBtn = $('#support-widget-btn');
                        mainBtn.removeClass('active');
                        mainBtn.find('i').removeClass('fa-times').addClass('fa-headset');
                    });
                }
            }
            // A. When a Checkbox is toggled
            //             $(document).on('change', '.variant-select-chk', function () {
            //                 // Visual styling for the row
            //                 const $row = $(this).closest('.variant-row-card');
            //                 if ($(this).is(':checked')) {
            //                     $row.css('border-color', '#2ebf91').css('background-color', '#f0fdf4');
            //                 } else {
            //                     $row.css('border-color', '#e0e0e0').css('background-color', '#fff');
            //                 }
            //                 updateTotals();
            //             });
            // Row Click (Toggles Checkbox for better UX)
            // $(document).on('input change', '.row-qty-input', function () {
            //     const $row = $(this).closest('.variant-row-card');
            //     const $chk = $row.find('.variant-select-chk');
            //
            //     // Auto-check the box if quantity is touched
            //     if (!$chk.is(':checked')) {
            //         $chk.prop('checked', true).trigger('change');
            //     } else {
            //         updateTotals();
            //     }
            // });
            // Quantity Buttons (+/-)
            // $(document).off('click', '.row-qty-btn').on('click', '.row-qty-btn', function (e) {
            //     e.preventDefault(); e.stopPropagation();
            //
            //     const $btn = $(this);
            //     const $row = $btn.closest('.variant-row-card');
            //     const $input = $row.find('.row-qty-input');
            //     const $checkbox = $row.find('.variant-select-chk');
            //
            //     let currentVal = parseInt($input.val()) || 0;
            //     const maxStock = (function(){ const n = parseInt($checkbox.attr('data-stock'), 10); return (Number.isFinite(n) && n > 0) ? n : 0; })();
            //
            //     if ($btn.hasClass('plus')) {
            //         if (currentVal < maxStock) $input.val(currentVal + 1);
            //         else Swal.fire({ toast: true, icon: 'warning', title: `Max Stock: ${maxStock}`, position: 'top-end', timer: 1500, showConfirmButton: false });
            //     } else {
            //         if (currentVal > 1) $input.val(currentVal - 1);
            //     }
            //
            //     // Auto-select checkbox if quantity changed
            //     if (!$checkbox.is(':checked')) {
            //         $checkbox.prop('checked', true).trigger('change');
            //     } else {
            //         updateTotals();
            //     }
            // });
        }

        // Run on load and on resize
        ensureChatCloseButton();
        $(window).resize(ensureChatCloseButton);
        // HELPER: Force Scroll to Element (Fixes Sticky Header issues)
        function forceScrollTo(element) {
            if (!element || element.length === 0) return;

            // 1. Get the DOM object
            const domNode = element[0];

            // 2. Calculate position: Absolute Top - Header Offset (150px)
            const headerOffset = 150;
            const elementPosition = domNode.getBoundingClientRect().top;
            const offsetPosition = elementPosition + window.pageYOffset - headerOffset;

            // 3. Scroll Manually
            window.scrollTo({
                top: offsetPosition,
                behavior: "smooth"
            });

            // 4. Focus if it's an input (opens keyboard on mobile)
            if (element.is('input, select, textarea')) {
                // slightly delay focus to allow scroll to start
                setTimeout(() => {
                    element.focus({ preventScroll: true });
                }, 300);
            }
        }
        // ==================================================
        // HELPER: Force Scroll to Element (Fixes Sticky Header issues)
        // Place this at the top of $(document).ready
        // ==================================================
        // function forceScrollTo(element) {
        //     if (!element || element.length === 0) return;
        //
        //     // 1. Get the DOM object
        //     const domNode = element[0];
        //
        //     // 2. Calculate position: Absolute Top - Header Offset (150px)
        //     const headerOffset = 150;
        //     const elementPosition = domNode.getBoundingClientRect().top;
        //     const offsetPosition = elementPosition + window.pageYOffset - headerOffset;
        //
        //     // 3. Scroll Manually
        //     window.scrollTo({
        //         top: offsetPosition,
        //         behavior: "smooth"
        //     });
        //
        //     // 4. Focus if it's an input (opens keyboard on mobile)
        //     if (element.is('input, select, textarea')) {
        //         setTimeout(() => {
        //             element.focus({ preventScroll: true });
        //         }, 300);
        //     }
        // }
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
        // ... inside the Swal.fire success alert ...
    }).then((result) => {
        if (result.isConfirmed) {
            // 1. Open Track Modal
            $('#open-status-modal').click();

            // 2. Pre-fill Tracking ID
            setTimeout(() => {
                $('#track-order-id').val(res.orderId || res.OrderId);
            }, 300);

            // 3. RESET FORM & BUTTON (Fixes the stuck "Processing" button)
            $btn.prop('disabled', false);     // Re-enable button
            updateSubmitButtonText();         // Restore "Confirm Order" text
            $('#order-form')[0].reset();      // Clear name, phone, etc.

        } else {
            // If they clicked "Close", we can safely reload the page
            window.location.reload();
        }
    });
}

// image modal

// Open the Modal
// Make functions global by using "window."
window.openImageModal = function (src) {
    var modal = document.getElementById("productImageModal");
    var modalImg = document.getElementById("modalImgDisplay");

    if (modal && modalImg) {
        // CHANGE THIS LINE: Use 'block' instead of 'flex'
        modal.style.display = "block";

        modalImg.src = src;
    }
};

window.closeImageModal = function () {
    var modal = document.getElementById("productImageModal");
    if (modal) {
        modal.style.display = "none";
    }
};

// Close modal if user clicks background (keep this outside functions)
window.onclick = function (event) {
    var modal = document.getElementById("productImageModal");
    if (event.target == modal) {
        modal.style.display = "none";
    }
};
