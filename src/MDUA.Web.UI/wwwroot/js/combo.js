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

$(document).ready(function () {

    // SAFETY CHECK: If we are on the Admin page (which doesn't have #order-form),
    // stop here so we don't cause errors.
    if ($('#order-form').length === 0) {
        return;
    }

    // ==================================================
    // 2. PAYMENT METHOD UI LOGIC
    // ==================================================

    // Handle payment option clicks
    $('.payment-option').on('click', function () {
        // Remove selected class from all options
        $('.payment-option').removeClass('selected');

        // Add selected class to clicked option
        $(this).addClass('selected');

        // Check the radio button
        $(this).find('input[type="radio"]').prop('checked', true);

        // Update button text based on payment method
        updateSubmitButtonText();
    });

    // Handle radio button changes (for keyboard navigation)
    $('input[name="paymentMethod"]').on('change', function () {
        $('.payment-option').removeClass('selected');
        $(this).closest('.payment-option').addClass('selected');
        updateSubmitButtonText();
    });

    // Function to update submit button text
    function updateSubmitButtonText() {
        const selectedPayment = $('input[name="paymentMethod"]:checked').val();
        const $btn = $('#final-submit-btn');
        const totalAmount = $('#receipt-grand-total').text();

        if (selectedPayment === 'cod') {
            $btn.text('Confirm Order');
        } else {
            $btn.text(`Proceed and Pay ${totalAmount}`);
        }
    }

    // Initialize button text on page load
    updateSubmitButtonText();

    // Update button text when total changes
    const originalUpdateTotals = updateTotals;
    updateTotals = function () {
        originalUpdateTotals();
        updateSubmitButtonText();
    };

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
    const variants = window.productVariants || [];

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

    $(document).on('click', '.variant-chip', function () {
        let $el = $(this);
        let attributeName = $el.data('attribute');
        let attributeValue = $el.data('value');

        // 1. Toggle Selection
        if ($el.hasClass('selected')) {
            $el.removeClass('selected');
            delete selectedAttributes[attributeName];
        } else {
            // Remove 'selected' from other chips in the same row
            $(`.variant-chip[data-attribute='${attributeName}']`).removeClass('selected');
            $el.addClass('selected');
            selectedAttributes[attributeName] = attributeValue;
        }

        // 2. Update Availability of OTHER options based on this new selection
        updateAttributeAvailability();

        // 3. Try to find the matching variant
        findAndApplyVariant();
    });

    // --- NEW CASCADING LOGIC FUNCTION ---
    function updateAttributeAvailability() {
        $('.variant-chip').each(function () {
            const $chip = $(this);
            const chipAttribute = $chip.data('attribute');
            const chipValue = $chip.data('value');

            // Skip if this specific chip is already selected (always keep selected items enabled)
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
                    if (!v.attributes || v.attributes[key] !== val) {
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
            const variantKeys = Object.keys(v.attributes);
            const selectedKeys = Object.keys(selectedAttributes);

            // 1. STRICT COUNT CHECK
            if (variantKeys.length !== selectedKeys.length) return false;

            // 2. STRICT VALUE CHECK
            for (let key of selectedKeys) {
                if (!v.attributes.hasOwnProperty(key) || v.attributes[key] !== selectedAttributes[key]) {
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

    function applyVariantData(variant) {
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

        let newImg = variant.image && variant.image.length > 1 ? variant.image : baseProductImageUrl;
        if (!newImg.startsWith("/") && !newImg.startsWith("http")) newImg = "/" + newImg;

        // Update BOTH Desktop and Mobile images
        $('#order-variant-image').attr('src', newImg);
        $('#mobile-order-variant-image').attr('src', newImg);

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

        // 1. If empty, reset button and exit
        if (!email) {
            $('.submit-btn').prop('disabled', false).text('Confirm Order');
            return;
        }

        // 2. If it matches the autofilled email exactly, it's valid
        if (isEmailAutofilled && email === currentCustomerEmail) {
            $msg.text("✓ Using your registered email").css('color', 'green').show();
            $('.submit-btn').prop('disabled', false).text('Confirm Order');
            return;
        }

        // 3. Basic validation
        if (!email.includes('@')) {
            $msg.text("⚠ Please enter a valid email address").css('color', 'orange').show();
            $('.submit-btn').prop('disabled', false).text('Confirm Order');
            return;
        }

        isCheckingEmail = true;
        $msg.text("⏳ Checking email...").css('color', 'blue').show();
        $('.submit-btn').prop('disabled', true).text('Verifying...');

        $.get('/order/check-email', { email: email }, function (res) {
            isCheckingEmail = false;

            if (res.exists) {
                // LOGIC UPDATE:
                // Since the backend blocks an email if it belongs to a different phone,
                // we must treat `exists` as an ERROR unless it matches `currentCustomerEmail`.

                if (currentCustomerEmail && email === currentCustomerEmail) {
                    // It exists and matches the current phone number -> OK
                    $msg.text("✓ Using your registered email").css('color', 'green').show();
                    $('.submit-btn').prop('disabled', false).text('Confirm Order');
                } else {
                    // It exists but does NOT match the phone number (or no phone entered) -> ERROR
                    $msg.text("⚠ This email is already registered with a different phone number.").css('color', 'red').show();
                    $('.submit-btn').prop('disabled', true).text('Fix Email');
                }
            } else {
                // Email is new -> OK
                $msg.text("✓ Email available").css('color', 'green').show();
                $('.submit-btn').prop('disabled', false).text('Confirm Order');
            }
        }).fail(function () {
            isCheckingEmail = false;
            $msg.text("⚠ Could not verify email, but you can proceed.").css('color', 'orange').show();
            $('.submit-btn').prop('disabled', false).text('Confirm Order');
        });
    });

    // Handle Phone Input with Debounce
    $('#customerPhone').on('input', debounce(function () {
        let phone = $(this).val();

        if (phone.length >= 11) {
            $('#phone-status').text("⏳ Checking...").css('color', 'blue');

            window.OrderAPI.checkCustomer(phone).then(function (data) {
                if (data.found) {
                    $('#phone-status').text("✓ Welcome back! Info loaded.").css('color', 'green');

                    if (data.name) $('#customerName').val(data.name);

                    if (data.email) {
                        const $emailField = $('#customerEmail');
                        const currentEmailValue = $emailField.val().trim();

                        // Only autofill if email is empty or has a placeholder value
                        if (!currentEmailValue || currentEmailValue.includes('@guest.local')) {
                            isEmailAutofilled = true;
                            currentCustomerEmail = data.email;
                            $emailField.val(data.email);
                            $('#email-status').text("✓ Using your registered email").css('color', 'green').show();
                            $('.submit-btn').prop('disabled', false).text('Confirm Order');
                        } else {
                            // Even if we don't overwrite, we need to know the correct email for this phone
                            currentCustomerEmail = data.email;
                        }
                    } else {
                        // Phone exists but has no email (or generated one)
                        currentCustomerEmail = null;
                    }
                } else {
                    $('#phone-status').text("New Customer").css('color', '#666');
                    isEmailAutofilled = false;
                    currentCustomerEmail = null;
                }
            }).fail(function () {
                $('#phone-status').text("⚠ Could not verify phone").css('color', 'orange');
            });
        } else {
            $('#phone-status').text("");
            isEmailAutofilled = false;
            currentCustomerEmail = null;
        }
    }, 500));

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

        resetSelect('#thana-select', 'Loading...');
        resetSelect('#suboffice-select', 'Select Thana first');

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

    $('#order-form').submit(function (e) {
        e.preventDefault();

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

        // 3. CHECK: Email Error Visible (New Check)
        // If the email field shows an error message (red text), do not proceed.
        const $emailStatus = $('#email-status');
        if ($emailStatus.is(':visible') && $emailStatus.css('color') === 'rgb(255, 0, 0)') { // Checks for red color
            $('html, body').animate({ scrollTop: $('#customerEmail').offset().top - 120 }, 300);
            $('#customerEmail').focus();
            return;
        }

        let isValid = true;
        let firstErrorField = null;

        // 4. CHECK: Variant Selected
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
        $(this).find('input[required], select[required], textarea[required]')
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

        // ==========================
        // PROCEED WITH AJAX SUBMIT
        // ==========================
        let formData = {};
        $(this).serializeArray().forEach(function (item) {
            formData[item.name] = item.value;
        });

        // Add Payment Method (From your new logic)
        formData.PaymentMethod = $('input[name="paymentMethod"]:checked').val() || 'cod';

        // Ensure numeric types
        formData.TargetCompanyId = 1;
        formData.ProductVariantId = parseInt(formData.ProductVariantId);
        formData.OrderQuantity = parseInt(formData.OrderQuantity);

        let $btn = $('#final-submit-btn');
        $btn.prop('disabled', true);

        // Update text based on payment
        if (formData.PaymentMethod === 'cod') {
            $btn.text('Processing...');
        } else {
            $btn.text('Redirecting...');
        }

        $.ajax({
            url: '/order/place',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (res) {
                if (res.success) {
                    // Success Logic
                    if (formData.PaymentMethod === 'cod') {
                        // COD Success
                        Swal.fire({
                            title: 'Success!',
                            text: "Order Placed Successfully! Order ID: " + (res.orderId || 'Check DB'),
                            icon: 'success',
                            confirmButtonText: 'OK',
                            confirmButtonColor: '#2563eb'
                        }).then((result) => {
                            location.reload();
                        });
                    } else {
                        // Online Payment Redirect
                        if (res.paymentUrl) {
                            window.location.href = res.paymentUrl;
                        } else {
                            Swal.fire({
                                title: 'Payment Pending',
                                text: 'Order created. Redirecting to payment...',
                                icon: 'info',
                                confirmButtonText: 'OK'
                            }).then(() => {
                                location.reload();
                            });
                        }
                    }
                } else {
                    // Server returned false
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
$('input[name="PostalCode"]').on('input keyup blur', function () {
    let code = $(this).val().trim();

    if (code.length === 4) {
        let $input = $(this);
        $input.css('border-color', '#3498db');

        $.get('/order/check-postal-code', { code: code }, function (data) {
            if (data.found) {
                $input.css('border-color', '#2ecc71');

                let $divSelect = $('#division-select');
                $divSelect.val(data.division).trigger('change');

                setTimeout(() => {
                    let $distSelect = $('#district-select');
                    $distSelect.val(data.district).trigger('change');
                }, 500);

                setTimeout(() => {
                    if (data.thana) {
                        let $thanaSelect = $('#thana-select');
                        $thanaSelect.empty()
                            .append(`<option value="${data.thana}" selected>${data.thana}</option>`)
                            .prop('disabled', false);
                    }

                    if (data.subOffice) {
                        let $subSelect = $('#suboffice-select');
                        $subSelect.empty()
                            .append(`<option value="${data.subOffice}" selected>${data.subOffice}</option>`);

                        $subSelect.find(':selected').data('code', code);
                        $subSelect.prop('disabled', false);
                    }
                }, 800);

            } else {
                $input.css('border-color', '#e74c3c');
            }
        });
    }
});