//******************new*************************

// wwwroot/js/combo.js

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
//******************newEND*************************




$(document).ready(function () {
    //******************new*************************

    // SAFETY CHECK: If we are on the Admin page (which doesn't have #order-form),
    // stop here so we don't cause errors.
    if ($('#order-form').length === 0) {
        return;
    }

    //******************newEND*************************

    // ==================================================
    // 0. GLOBAL VARIABLES & STATE
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
    // AUTO-SELECT FIRST VARIANT IF ONLY ONE EXISTS
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
    // 1. DYNAMIC ATTRIBUTE SELECTION LOGIC (WITH TOGGLE)
    // ==================================================

    $(document).on('click', '.variant-chip', function () {
        let $el = $(this);
        let attributeName = $el.data('attribute');
        let attributeValue = $el.data('value');

        if ($el.hasClass('selected')) {
            $el.removeClass('selected');
            delete selectedAttributes[attributeName];
        } else {
            $(`.variant-chip[data-attribute='${attributeName}']`).removeClass('selected');
            $el.addClass('selected');
            selectedAttributes[attributeName] = attributeValue;
        }

        findAndApplyVariant();
    });
    function findAndApplyVariant() {
        $('#selected-variant-id').val('');

        const matchedVariant = variants.find(v => {
            for (let key in selectedAttributes) {
                // Use strict equality check logic or robust string comparison
                if (!v.attributes[key] || String(v.attributes[key]) !== String(selectedAttributes[key])) {
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

        // ✅ CRITICAL FIX: Handle stock properly
        maxAvailableStock = variant.stock;

        let currentQty = parseInt($('#quantity').val()) || 1;
        if (maxAvailableStock > 0 && currentQty > maxAvailableStock) {
            $('#quantity').val(maxAvailableStock);
        }
        updateStockMessage(maxAvailableStock);

        let newImg = variant.image && variant.image.length > 1 ? variant.image : baseProductImageUrl;
        if (!newImg.startsWith("/") && !newImg.startsWith("http")) newImg = "/" + newImg;

        $('#order-variant-image').attr('src', newImg);

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
        $('#selected-variant-id').val('');
        maxAvailableStock = 0;
        // ✅ ADD THIS: Hide the stock info on reset
        $('#variant-info').hide();
        updateStockMessage(0); // This calls the function above, but the hide() line above ensures it stays hidden
        updateTotals();
    }
    function handleNoMatch() {
        $('#stock-message').text("This combination is currently unavailable.").addClass('text-danger show');
        $('#order-variant-image').attr('src', baseProductImageUrl);
        $('#selected-variant-id').val('');
        maxAvailableStock = 0;
    }

    function updateStockMessage(stock) {
        const el = $('#stock-message');
        const parent = $('#variant-info'); // Get the parent container

        el.removeClass('stock-high stock-medium stock-low text-danger show text-success');

        // If no attributes selected yet, keep hidden
        if (Object.keys(selectedAttributes).length === 0 && variants.length > 1) {
            parent.hide();
            return;
        }

        parent.show(); // Reveal the container

        if (stock <= 0) {
            el.text('Out of Stock').addClass('text-danger show');
            // Optional: Disable submit button if out of stock
            $('.submit-btn').prop('disabled', true).text('Out of Stock');
        } else {
            // ✅ CHANGED: Always show specific stock count
            el.text(`Current Stock: ${stock} items available`).addClass('text-success show').css({
                'font-weight': 'bold',
                'color': '#10b981', // Green color
                'font-size': '0.95rem'
            });

            // Re-enable button if it was disabled by out-of-stock
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
    // 2. EMAIL & PHONE CHECK (INLINE REAL-TIME VALIDATION)
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
                // --- CHANGED LOGIC HERE ---
                // Previous logic: Blocked the user (Red text, Disabled button)
                // New logic: Welcomes the user (Green/Blue text, ENABLED button)

                if (email === currentCustomerEmail) {
                    $msg.text("✓ Using your registered email").css('color', 'green').show();
                } else {
                    // Determine if this is a "Welcome Back" scenario
                    $msg.text("✓ Account found! We will link this order to you.").css('color', 'blue').show();
                }

                // ALWAYS enable the button if the email is valid, even if it exists
                $('.submit-btn').prop('disabled', false).text('Confirm Order');
            } else {
                // Email is new - also valid
                $msg.text("✓ Email available").css('color', 'green').show();
                $('.submit-btn').prop('disabled', false).text('Confirm Order');
            }
        })
            .fail(function () {
                isCheckingEmail = false;
                // In case of server error, usually safer to let them try to submit
                $msg.text("⚠ Could not verify email, but you can proceed.").css('color', 'orange').show();
                $('.submit-btn').prop('disabled', false).text('Confirm Order');
            });
    });

    // ✅ FIXED: Using Debounce (500ms) to prevent server spam on typing
    $('#customerPhone').on('input', debounce(function () {
        let phone = $(this).val();

        if (phone.length >= 11) {
            $('#phone-status').text("⏳ Checking...").css('color', 'blue');


            //******************************new*************** */

            window.OrderAPI.checkCustomer(phone).then(function (data) {
                //******************************newEND*************** */

                if (data.found) {
                    $('#phone-status').text("✓ Welcome back! Info loaded.").css('color', 'green');

                    // 1. FILL NAME
                    if (data.name) $('#customerName').val(data.name);

                    // 2. FILL EMAIL (With logic for @guest.local)
                    if (data.email) {
                        const $emailField = $('#customerEmail');
                        const currentEmailValue = $emailField.val().trim();

                        if (!currentEmailValue || currentEmailValue.includes('@guest.local')) {
                            isEmailAutofilled = true;
                            currentCustomerEmail = data.email;
                            $emailField.val(data.email);
                            $('#email-status').text("✓ Using your registered email").css('color', 'green').show();
                            $('.submit-btn').prop('disabled', false).text('Confirm Order');
                        } else {
                            currentCustomerEmail = data.email;
                        }
                    }

                    // ❌ REMOVED: Address Autofill Logic
                    // The block that filled Street, PostalCode, Division, etc. is deleted.

                } else {
                    $('#phone-status').text("New Customer").css('color', '#666');
                    isEmailAutofilled = false;
                    currentCustomerEmail = null;
                }
            })
                .fail(function () {
                    $('#phone-status').text("⚠ Could not verify phone").css('color', 'orange');
                });
        } else {
            $('#phone-status').text("");
            isEmailAutofilled = false;
            currentCustomerEmail = null;
        }
    }, 500)); // 500ms delay
    // ==================================================
    // 3. LOCATION & TOTALS (ROBUST CASCADING)
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
    // ✅ FIXED QUANTITY CONTROLS
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
            // Stock limit exists
            if (qty < maxAvailableStock) {
                $('#quantity').val(qty + 1);
                $('#summary-qty').text(qty + 1);
                updateTotals();
            } else {
                showStockError(`Maximum available: ${maxAvailableStock} items`);
                $('#quantity').val(maxAvailableStock);
            }
        } else {
            // Stock is 0 or undefined - treat as out of stock
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

    // Manual input validation
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
    // 4. SUBMIT ORDER
    // ==================================================

    $('#order-form').submit(function (e) {
        e.preventDefault();

        // 1. Reset previous error styles
        $('.input-error').removeClass('input-error');
        $('.variant-chips-container').css({ 'border': 'none', 'padding': '0' });

        // 2. CHECK: Is Email check pending?
        if (isCheckingEmail) {
            // Replaced alert with SweetAlert
            Swal.fire({
                title: 'Please wait',
                text: 'Validating email address...',
                icon: 'info',
                timer: 1500,
                showConfirmButton: false
            });
            return;
        }

        let isValid = true;
        let firstErrorField = null;

        // 3. CHECK: Variant Selected (Check this FIRST as it is visually at the top)
        if (!$('#selected-variant-id').val()) {
            isValid = false;

            // Highlight the container
            const $variantContainer = $('.variant-chips-container');
            $variantContainer.css({
                'border': '2px solid #dc3545',
                'padding': '5px',
                'border-radius': '5px'
            });

            // Set as first error
            firstErrorField = $(".variant-selection-area");
        }

        // 4. CHECK: General Required Fields (Name, Phone, Address)
        $(this).find('input[required], select[required], textarea[required]')
            .filter(':visible')
            .each(function () {

                // Skip disabled fields
                if ($(this).prop('disabled')) return;

                if (!$(this).val() || $(this).val().trim() === "") {
                    isValid = false;
                    $(this).addClass('input-error');

                    // Only set firstErrorField if we haven't found one yet (e.g., Variant)
                    if (!firstErrorField) {
                        firstErrorField = $(this);
                    }
                }
            });

        // 5. IF INVALID: Scroll to the first problem
        if (!isValid || firstErrorField) {
            if (firstErrorField && firstErrorField.length) {

                const elementTop = firstErrorField[0].getBoundingClientRect().top + window.scrollY;

                $('html, body').animate({
                    scrollTop: elementTop - 120
                }, 600);

                // focus if it's a typing field
                if (firstErrorField.is('input, select, textarea')) {
                    firstErrorField.focus();
                }
            }
            return; // Stop execution
        }

        // 6. CHECK: Stock Limits
        let requestedQty = parseInt($('#quantity').val());
        if (maxAvailableStock > 0 && requestedQty > maxAvailableStock) {
            $('#stock-error-message').text(`ERROR: Requested quantity (${requestedQty}) exceeds stock limit (${maxAvailableStock}).`);
            $('html, body').animate({ scrollTop: $('#quantity').offset().top - 150 }, 400);
            return;
        }

        // 7. CHECK: Email Error Visible
        const $emailStatus = $('#email-status');
        if ($emailStatus.is(':visible') && $emailStatus.text().includes('✗')) {
            $('html, body').animate({ scrollTop: $('#customerEmail').offset().top - 120 }, 300);
            $('#customerEmail').focus();
            return;
        }

        // ==========================
        // PROCEED WITH AJAX SUBMIT
        // ==========================
        let formData = {};
        $(this).serializeArray().forEach(function (item) {
            formData[item.name] = item.value;
        });

        // Ensure numeric types
        formData.TargetCompanyId = 1;
        formData.ProductVariantId = parseInt(formData.ProductVariantId);
        formData.OrderQuantity = parseInt(formData.OrderQuantity);

        let $btn = $('.submit-btn');
        $btn.prop('disabled', true).text('Processing...');

        $.ajax({
            url: '/order/place',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (res) {
                if (res.success) {
                    // ✅ Fixed: Using SweetAlert2 for Success
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
                    // ✅ Fixed: Using SweetAlert2 for Error
                    Swal.fire({
                        title: 'Order Failed',
                        text: res.message || "Failed to place order.",
                        icon: 'error',
                        confirmButtonText: 'Try Again'
                    });
                    $btn.prop('disabled', false).text('Confirm Order');
                }
            },
            error: function (xhr) {
                let errorMessage = "Network Error.";
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage = xhr.responseJSON.message;
                } else if (xhr.responseText) {
                    errorMessage = xhr.responseText.substring(0, 200);
                }

                // ✅ Fixed: Using SweetAlert2 for Server Error
                Swal.fire({
                    title: 'Server Error',
                    text: errorMessage,
                    icon: 'error',
                    confirmButtonText: 'Close'
                });
                $btn.prop('disabled', false).text('Confirm Order');
            }
        });
    });

    // ==================================================
    // 5. IMAGE GALLERY SLIDER
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
// 6. POSTAL CODE AUTOFILL (AUTOMATIC)
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