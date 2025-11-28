$(document).ready(function () {

    // ==================================================
    // 0. GLOBAL VARIABLES & STATE
    // ==================================================

    const baseInfo = window.baseProductInfo || { price: 0, image: "/images/default-product.jpg" };

    let currentVariantPrice = baseInfo.price;
    let maxAvailableStock = 0;
    let selectedAttributes = {};

    let isCheckingEmail = false;
    let isEmailAutofilled = false;
    let currentCustomerEmail = null;

    const delivery = (typeof deliveryCharges !== "undefined") ? deliveryCharges : { dhaka: 0, outside: 0 };
    const baseProductImageUrl = baseInfo.image;

    $('#display-price').text('Tk. ' + currentVariantPrice.toLocaleString());
    $('#summary-subtotal').text('Tk. ' + currentVariantPrice.toLocaleString());

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
        const variants = window.productVariants || [];

        const matchedVariant = variants.find(v => {
            for (let key in selectedAttributes) {
                if (!v.attributes[key] || v.attributes[key] != selectedAttributes[key]) {
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
        updateStockMessage(0);
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
        el.removeClass('stock-high stock-medium stock-low text-danger show');

        if (Object.keys(selectedAttributes).length === 0) return;

        if (stock <= 0) {
            el.text('Out of Stock').addClass('text-danger show');
        } else if (stock >= 10) {
            el.text(`In stock: ${stock} items`).addClass('stock-high show');
        } else if (stock >= 3) {
            el.text(`Hurry! Only ${stock} left`).addClass('stock-medium show');
        } else {
            el.text(`Last few! Just ${stock} left`).addClass('stock-low show');
        }
    }

    // ==================================================
    // 2. EMAIL & PHONE CHECK (INLINE REAL-TIME VALIDATION)
    // ==================================================

    // ✅ When user starts typing in email field
    $('#customerEmail').on('input', function () {
        isEmailAutofilled = false;
        $('#email-status').hide();
        $('.submit-btn').prop('disabled', false).text('Confirm Order');
    });

    // ✅ Real-time validation on blur (when user leaves the field)
    $('#customerEmail').on('blur', function () {
        const email = $(this).val().trim();
        const $msg = $('#email-status');

        // Clear previous messages
        $msg.hide().removeClass('text-danger').removeClass('text-success');

        if (!email) {
            $('.submit-btn').prop('disabled', false).text('Confirm Order');
            return;
        }

        // ✅ CRITICAL: If this email was autofilled AND matches current customer, allow it
        if (isEmailAutofilled && email === currentCustomerEmail) {
            $msg.text("✓ Using your registered email").css('color', 'green').show();
            $('.submit-btn').prop('disabled', false).text('Confirm Order');
            return;
        }

        // ✅ Validate email format
        if (!email.includes('@')) {
            $msg.text("⚠ Please enter a valid email address").css('color', 'orange').show();
            $('.submit-btn').prop('disabled', false).text('Confirm Order');
            return;
        }

        // ✅ Check if email exists (AJAX call)
        isCheckingEmail = true;
        $msg.text("⏳ Checking email...").css('color', 'blue').show();
        $('.submit-btn').prop('disabled', true).text('Verifying...');

        $.get('/order/check-email', { email: email }, function (res) {
            isCheckingEmail = false;

            if (res.exists) {
                // ✅ Double-check: Is this the same customer's email?
                if (email === currentCustomerEmail) {
                    $msg.text("✓ Using your registered email").css('color', 'green').show();
                    $('.submit-btn').prop('disabled', false).text('Confirm Order');
                } else {
                    // ❌ SHOW ERROR INLINE (like phone status)
                    $msg.text("✗ Email already exists! Please use another.").css('color', 'red').show();
                    $('.submit-btn').prop('disabled', true).text('Confirm Order');
                }
            } else {
                // ✅ Email is available
                $msg.text("✓ Email available").css('color', 'green').show();
                $('.submit-btn').prop('disabled', false).text('Confirm Order');
            }
        })
            .fail(function () {
                isCheckingEmail = false;
                $msg.text("⚠ Could not verify email. Please try again.").css('color', 'orange').show();
                $('.submit-btn').prop('disabled', false).text('Confirm Order');
            });
    });

    // ✅ PHONE CHECK - WITH PROPER EMAIL AUTOFILL HANDLING
    $('#customerPhone').on('input blur', function () {
        let phone = $(this).val();

        if (phone.length >= 11) {
            $('#phone-status').text("⏳ Checking...").css('color', 'blue');

            $.get('/order/check-customer', { phone: phone }, function (data) {
                if (data.found) {
                    $('#phone-status').text("✓ Welcome back! Info loaded.").css('color', 'green');

                    // Autofill name
                    if (data.name) $('#customerName').val(data.name);

                    // ✅ Autofill email with proper flagging
                    if (data.email) {
                        const $emailField = $('#customerEmail');
                        const currentEmailValue = $emailField.val().trim();

                        // Only autofill if email field is empty OR has the old guest email
                        if (!currentEmailValue || currentEmailValue.includes('@guest.local')) {
                            // Set flags BEFORE setting value
                            isEmailAutofilled = true;
                            currentCustomerEmail = data.email;

                            $emailField.val(data.email);

                            // Show success message inline
                            $('#email-status').text("✓ Using your registered email").css('color', 'green').show();
                            $('.submit-btn').prop('disabled', false).text('Confirm Order');
                        } else {
                            // Email field has a value - just update tracking
                            currentCustomerEmail = data.email;
                        }
                    }

                    // Autofill address
                    if (data.addressData) {
                        const addr = data.addressData;
                        $('textarea[name="Street"]').val(addr.street);
                        $('input[name="PostalCode"]').val(addr.postalCode);
                        $('input[name="ZipCode"]').val(addr.zipCode);
                        $('#division-select').val(addr.divison).trigger('change');

                        // Wait for Districts to load via AJAX (Increased timeout slightly for safety)
                        setTimeout(() => {
                            $('#district-select').val(addr.city).trigger('change');
                        }, 500);
                    }
                } else {
                    // New customer
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
    });

    // ==================================================
    // 3. LOCATION & TOTALS (DYNAMIC CASCADING)
    // ==================================================

    // Helper to reset and disable a dropdown
    // ==================================================
    // 3. LOCATION & TOTALS (ROBUST CASCADING)
    // ==================================================

    // Helper: Reset, Disable, and Show Loading Text
    function resetSelect(selector, defaultText) {
        $(selector).empty()
            .append(`<option value="">${defaultText}</option>`)
            .prop('disabled', true);
    }

    // Helper: Unlock and Populate
    function enableSelect(selector, data, defaultText) {
        let $el = $(selector);
        $el.empty().append(`<option value="">${defaultText}</option>`);

        if (!data || data.length === 0) {
            $el.append('<option value="" disabled>No options available</option>');
            // We unlock it anyway so the user sees "No options" instead of a blocked field
            $el.prop('disabled', false);
            return;
        }

        data.forEach(item => {
            // Handle both simple strings and objects
            let val = item.name || item.Name || item;
            let text = item.name || item.Name || item;
            let code = item.code || item.Code || ""; // For SubOffice

            $el.append(`<option value="${val}" data-code="${code}">${text}</option>`);
        });

        $el.prop('disabled', false);
    }

    // 1. Load Divisions
    function populateDivisions() {
        console.log("Fetching Divisions...");
        $.get('/order/get-divisions', function (data) {
            console.log("Divisions loaded:", data);
            enableSelect('#division-select', data, 'Select Division');
        }).fail(function () {
            console.error("Failed to load Divisions");
            $('#division-select').append('<option>Error loading data</option>');
        });
    }

    populateDivisions();

    // 2. Division Change -> Load Districts
    $('#division-select').change(function () {
        let division = $(this).val();
        console.log("Selected Division:", division);

        // Reset downstream
        resetSelect('#district-select', 'Loading...');
        resetSelect('#thana-select', 'Select District first');
        resetSelect('#suboffice-select', 'Select Thana first');

        if (division) {
            $.get('/order/get-districts', { division: division }, function (data) {
                console.log("Districts loaded:", data);
                enableSelect('#district-select', data, 'Select District');
            })
                .fail(function () {
                    console.error("API Error: /order/get-districts");
                    resetSelect('#district-select', 'Error loading districts');
                    $('#district-select').prop('disabled', false); // Unlock to show error
                });
        } else {
            resetSelect('#district-select', 'Select Division first');
        }
    });

    // 3. District Change -> Load Thanas
    $('#district-select').change(function () {
        let district = $(this).val();
        console.log("Selected District:", district);

        resetSelect('#thana-select', 'Loading...');
        resetSelect('#suboffice-select', 'Select Thana first');

        // Delivery Charge Logic
        let charge = delivery.outside;
        if (district && (district.toLowerCase().includes('dhaka') || district.trim() === 'Dhaka')) {
            charge = delivery.dhaka;
        }
        $('#receipt-delivery').text('Tk. ' + charge).data('cost', charge);
        updateTotals();

        if (district) {
            $.get('/order/get-thanas', { district: district }, function (data) {
                console.log("Thanas loaded:", data);
                enableSelect('#thana-select', data, 'Select Thana');
            })
                .fail(function () {
                    console.error("API Error: /order/get-thanas");
                    resetSelect('#thana-select', 'Error loading thanas');
                    $('#thana-select').prop('disabled', false);
                });
        } else {
            resetSelect('#thana-select', 'Select District first');
        }
    });

    // 4. Thana Change -> Load Sub-Offices
    $('#thana-select').change(function () {
        let thana = $(this).val();
        console.log("Selected Thana:", thana);

        resetSelect('#suboffice-select', 'Loading...');

        if (thana) {
            $.get('/order/get-suboffices', { thana: thana }, function (data) {
                console.log("SubOffices loaded:", data);
                enableSelect('#suboffice-select', data, 'Select Sub-Office');
            })
                .fail(function () {
                    console.error("API Error: /order/get-suboffices");
                    resetSelect('#suboffice-select', 'Error loading sub-offices');
                    $('#suboffice-select').prop('disabled', false);
                });
        } else {
            resetSelect('#suboffice-select', 'Select Thana first');
        }
    });

    // 5. Sub-Office Change -> Autofill Postal Code
    $('#suboffice-select').change(function () {
        let code = $(this).find(':selected').data('code');
        if (code && $('input[name="PostalCode"]').val() != code) {
            $('input[name="PostalCode"]').val(code).css('border-color', '#2ecc71');
        }
    });
    // Totals Calculation
    function updateTotals() {
        let qty = parseInt($('#quantity').val()) || 1;
        let subtotal = currentVariantPrice * qty;
        let deliveryCost = parseFloat($('#receipt-delivery').data('cost')) || 0;
        let total = subtotal + deliveryCost;

        $('#summary-subtotal').text('Tk. ' + subtotal.toLocaleString());
        $('#receipt-grand-total').text('Tk. ' + total.toLocaleString());
    }

    $('#increase').click(function () {
        clearStockError();
        let qty = parseInt($('#quantity').val());

        if (maxAvailableStock > 0 && qty < maxAvailableStock) {
            $('#quantity').val(qty + 1);
        } else if (maxAvailableStock > 0 && qty >= maxAvailableStock) {
            showStockError(`Maximum available quantity reached: ${maxAvailableStock}`);
            $('#quantity').val(maxAvailableStock);
        }
        updateTotals();
    });

    $('#decrease').click(function () {
        clearStockError();
        let qty = parseInt($('#quantity').val());
        if (qty > 1) {
            $('#quantity').val(qty - 1);
            updateTotals();
        }
    });

    // ==================================================
    // 4. SUBMIT ORDER - WITH INLINE ERROR CHECK
    // ==================================================
    $('#order-form').submit(function (e) {
        e.preventDefault();

        // ✅ CHECK 0: Is Email check pending?
        if (isCheckingEmail) {
            alert("⏳ Please wait, checking email...");
            return;
        }

        // ✅ CHECK 1: Variant Selected
        if (!$('#selected-variant-id').val()) {
            alert("⚠️ Warning: The selected combination is not available.\nPlease choose a different variation.");
            $('.variant-chips-container').css({ 'border': '2px solid red', 'padding': '5px', 'border-radius': '5px' });
            $('html, body').animate({ scrollTop: $(".variant-selection-area").offset().top - 100 }, 500);
            return;
        }

        // ✅ CHECK 2: Stock
        let requestedQty = parseInt($('#quantity').val());
        if (maxAvailableStock > 0 && requestedQty > maxAvailableStock) {
            $('#stock-error-message').text(`ERROR: Requested quantity (${requestedQty}) exceeds stock limit (${maxAvailableStock}).`);
            return;
        }

        // ✅ CHECK 3: Email Error Visible (Inline check - NOT alert)
        const $emailStatus = $('#email-status');
        if ($emailStatus.is(':visible') && $emailStatus.text().includes('✗')) {
            // Scroll to email field and focus
            $('html, body').animate({ scrollTop: $('#customerEmail').offset().top - 100 }, 300);
            $('#customerEmail').focus();
            return; // Block submission silently
        }

        // ✅ All checks passed - proceed with submission
        let formData = {};
        $(this).serializeArray().forEach(function (item) {
            formData[item.name] = item.value;
        });

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
                    alert("✅ Order Placed Successfully! Order ID: " + (res.orderId || 'Check DB'));
                    location.reload();
                } else {
                    alert("❌ Error: " + (res.message || "Failed to place order."));
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
                alert("❌ Server Error: " + errorMessage);
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

    // ✅ AUTOMATIC TRIGGER: Run as soon as we have 4 digits
    if (code.length === 4) {

        let $input = $(this);
        // Optional: visual cue that we are checking
        $input.css('border-color', '#3498db');

        $.get('/order/check-postal-code', { code: code }, function (data) {
            if (data.found) {
                $input.css('border-color', '#2ecc71'); // Green border on success

                // 1. Set Division & Trigger Change (loads districts via AJAX)
                let $divSelect = $('#division-select');
                $divSelect.val(data.division).trigger('change');

                // 2. Set District (Wait 500ms for Division AJAX change to finish loading districts)
                // Note: Increased timeout to 500ms to allow server response time
                setTimeout(() => {
                    let $distSelect = $('#district-select');
                    $distSelect.val(data.district).trigger('change');
                }, 500);

                // 3. Set Thana & SubOffice
                // We manually insert these options so they work even if cascading is slow
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
                            .append(`<option value="${data.subOffice}" selected>${data.subOffice}</option>`)
                            // Important: store the code so reverse lookup doesn't break
                            .find(':selected').data('code', code);

                        $subSelect.prop('disabled', false);
                    }
                }, 800); // Wait slightly longer for Thana to ensure it doesn't conflict

            } else {
                // Not found - revert border
                $input.css('border-color', '#e74c3c'); // Red border
            }
        });
    }
});