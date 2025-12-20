$(document).ready(function () {

    // Expand / collapse action drawer
    $(document).on("click", ".js-expand-actions", function () {
        const id = $(this).data("product-id");
        const drawer = $("#drawer-" + id);
        const $thisButton = $(this);

        // 1. Reset all other buttons
        $(".js-expand-actions").not($thisButton).removeClass("btn-primary text-white").addClass("btn-manage");
        $(".js-expand-actions").not($thisButton).find("span").html("&#9660;"); // Reset arrow down

        // 2. Close other drawers
        $(".action-drawer-row").not(drawer).addClass("d-none");

        // 3. Toggle this drawer
        drawer.toggleClass("d-none");

        // 4. Toggle Button Style
        if (drawer.hasClass("d-none")) {
            // Closed state
            $thisButton.removeClass("btn-primary text-white").addClass("btn-manage");
            $thisButton.find("span").html("&#9660;"); // Arrow Down
        } else {
            // Open state
            $thisButton.removeClass("btn-manage").addClass("btn-primary text-white");
            $thisButton.find("span").html("&#9650;"); // Arrow Up
        }
    });

    // ============================================================
    // 1. GLOBAL CONFIGURATION & VARIABLES
    // ============================================================

    let token = $('input[name="__RequestVerificationToken"]').val();
    let modalBody = $('#modal-variants-content');
    const urls = window.productConfig ? window.productConfig.urls : {};

    // AJAX INTERCEPTOR
    (function ($) {
        var originalAjax = $.ajax;
        $.ajax = function (options) {
            var originalSuccess = options.success;
            options.success = function (data, textStatus, xhr) {
                if (data && data.success === false && data.redirectUrl) {
                    window.location.href = data.redirectUrl;
                    return;
                }
                if (originalSuccess) {
                    originalSuccess(data, textStatus, xhr);
                }
            };
            return originalAjax(options);
        };
    })(jQuery);

    // ============================================================
    // 2. HELPER FUNCTIONS
    // ============================================================

    // 🟢 NEW HELPER: Update Variant Image Count Badge in Real-Time
    // Helper: Update Main Grid Price without Reload
    function updateMainGridPrice(productId) {
        let $row = $(`tr[data-product-row-id="${productId}"]`);
        let $cell = $row.find('.price-cell');

        if ($row.length === 0) return;

        // ✅ FIX: Added timestamp { _: new Date().getTime() } to prevent caching
        $.get(urls.getUpdatedPrice, { productId: productId, _: new Date().getTime() }, function (data) {
            if (data.success) {
                let html = '';

                // Server calculates 'hasDiscount' based on current Date+Time
                if (data.hasDiscount) {
                    // Applied CSS classes for red/strikethrough styling
                    html = `<span class="original-price">${data.originalPrice}</span>
                        <span class="discounted-price">${data.sellingPrice}</span>`;
                } else {
                    html = `<span class="fw-bold text-dark">${data.originalPrice}</span>`;
                }

                $cell.html(html);

                // Optional: visual flash to indicate update
                $cell.hide().fadeIn(300);
            }
        });
    }


    // 🟢 HELPER: Update the badge count on the background table row
    function updateVariantImageCount(variantId, change) {
        let $row = $('tr[data-variant-id="' + variantId + '"]');
        let $btn = $row.find('.js-manage-var-images');
        let $badge = $btn.find('.badge');

        // Calculate new count
        let currentCount = $badge.length > 0 ? parseInt($badge.text()) : 0;
        let newCount = currentCount + change;
        if (newCount < 0) newCount = 0;

        // Update DOM
        if (newCount === 0) {
            $badge.remove();
        } else {
            if ($badge.length === 0) {
                // Add new badge structure
                $btn.append(`<span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger" style="font-size: 0.6rem; padding: 0.25em 0.4em;">${newCount}<span class="visually-hidden">images</span></span>`);
            } else {
                // Update existing badge number
                $badge.html(`${newCount} <span class="visually-hidden">images</span>`);
            }
        }
    }
    // Helper: Prevent selecting the same attribute twice across rows
    function updateDropdownAvailability() {
        let selectedValues = [];
        $('.attr-name-select').each(function () {
            let v = $(this).val();
            if (v) selectedValues.push(v);
        });

        $('.attr-name-select').each(function () {
            let myVal = $(this).val();
            $(this).find('option').each(function () {
                let optVal = $(this).val();
                if (optVal && selectedValues.includes(optVal) && optVal !== myVal) {
                    $(this).prop('disabled', true);
                } else {
                    $(this).prop('disabled', false);
                }
            });
        });
    }

    // Helper: Add a New Attribute Row
    function addAttributeRow() {
        let index = $('#dynamic-attributes-container .dynamic-attr-row').length;
        let optionsHtml = $('#attribute-template').html();

        if (!optionsHtml || optionsHtml.trim() === "") {
            console.error("Attribute Template is empty. Ensure Facade loads Attributes.");
            optionsHtml = '<option value="">Error: No Attributes Found</option>';
        }

        let rowHtml = `
            <div class="dynamic-attr-row mb-2 d-flex gap-2 align-items-center">
                <select class="form-control form-control-sm attr-name-select" style="width: 45%;">
                    ${optionsHtml}
                </select>
                
                <select class="form-control form-control-sm attr-value-select" style="width: 45%;" disabled>
                    <option value="">Select Value</option>
                </select>
                
                <input type="hidden" name="AttributeValueIds[${index}]" class="final-value-id" />

                <button type="button" class="btn btn-sm btn-outline-danger remove-attr-row" style="width: 10%;">&times;</button>
            </div>
        `;

        $('#dynamic-attributes-container').append(rowHtml);
        updateDropdownAvailability();
    }

    // ============================================================
    // 3. DYNAMIC ATTRIBUTE HANDLERS (Add/Remove Rows)
    // ============================================================

    // HANDLER: Click "+ Add Attribute"
    $(document).on('click', '#btn-add-dynamic-attr', function () {
        addAttributeRow();
    });

    // HANDLER: Remove Row
    $(document).on('click', '.remove-attr-row', function () {
        $(this).closest('.dynamic-attr-row').remove();
        $('#dynamic-attributes-container .dynamic-attr-row').each(function (idx) {
            $(this).find('.final-value-id').attr('name', `AttributeValueIds[${idx}]`);
        });
        updateDropdownAvailability();
    });

    // HANDLER: Attribute Name Changed -> Load Values
    $(document).on('change', '.attr-name-select', function () {
        let $select = $(this);
        let $row = $select.closest('.dynamic-attr-row');
        let $valSelect = $row.find('.attr-value-select');
        let attrId = $select.val();

        updateDropdownAvailability();

        if (attrId) {
            $valSelect.html('<option>Loading...</option>').prop('disabled', true);

            $.get(urls.getAttributeValues, { attributeId: attrId }, function (data) {
                let html = '<option value="">Select Value</option>';
                if (data && data.length > 0) {
                    data.forEach(v => {
                        html += `<option value="${v.id}">${v.value}</option>`;
                    });
                } else {
                    html = '<option value="">No values found</option>';
                }
                $valSelect.html(html).prop('disabled', false);
            });
        } else {
            $valSelect.html('<option value="">Select Value</option>').prop('disabled', true);
        }
    });

    // HANDLER: Attribute Value Changed -> Update Hidden Input
    $(document).on('change', '.attr-value-select', function () {
        let val = $(this).val();
        $(this).siblings('.final-value-id').val(val);
    });

    // ============================================================
    // 4. ADD NEW VARIANT LOGIC (Submit)
    // ============================================================

    modalBody.on('click', '#btn-save-new-variant', function () {
        let $form = $('#form-add-single-variant');
        let productName = $('#modal-product-name').text().trim();
        let parts = [productName];
        let isValid = true;

        if ($('.dynamic-attr-row').length === 0) {
            alert("Please add at least one attribute.");
            return;
        }

        $('.attr-value-select').each(function () {
            if (!$(this).val()) {
                isValid = false;
                $(this).addClass('is-invalid');
            } else {
                $(this).removeClass('is-invalid');
                parts.push($(this).find('option:selected').text());
            }
        });

        if (!isValid) {
            alert("Please select values for all attributes.");
            return;
        }

        let proposedName = parts.join(' - ');
        let isDuplicate = false;
        $('.variants-table-modal tbody tr').each(function () {
            let existingName = $(this).find('td:first').text().trim();
            if (existingName === proposedName) {
                isDuplicate = true;
                return false;
            }
        });

        if (isDuplicate) {
            $('#duplicate-variant-name').text(proposedName);
            $('#duplicateVariantModal').modal('show');
            return;
        }

        $('#new-variant-name').val(proposedName);
        let $btn = $(this);
        $btn.prop('disabled', true).text('Saving...');

        $.ajax({
            url: urls.addSingleVariant,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: $form.serialize(),
            success: function (data) {
                if (data.success) {
                    let productId = $form.find('input[name="ProductId"]').val();
                    $.get(urls.getVariantsPartial, { productId: productId }, function (html) {
                        $('#modal-variants-content').html(html);
                        addAttributeRow();
                    });
                } else {
                    alert('Error: ' + data.message);
                    $btn.prop('disabled', false).text('Save New Variant');
                }
            },
            error: function () {
                $btn.prop('disabled', false).text('Save New Variant');
            }
        });
    });

    // ============================================================
    // 5. MAIN PAGE HANDLERS (FIXED WITH EVENT DELEGATION)
    // ============================================================

    // 1. View Variants Button
    $(document).on('click', '.btn-view-variants', function () {
        var productId = $(this).data('product-id');
        var productName = $(this).data('product-name');

        $('#modal-product-name').text(productName);
        $('#modal-variants-content').html('<div class="loading-spinner"></div>');
        $('#productVariantsModal').modal('show');

        $.get(urls.getVariantsPartial, { productId: productId }, function (data) {
            $('#modal-variants-content').html(data);
            addAttributeRow();
        }).fail(function () {
            $('#modal-variants-content').html('<div class="error-message">Failed to load variants.</div>');
        });
    });

    // 2. Toggle Status
    $(document).on('click', '.js-toggle-status', function () {
        let $button = $(this);
        let productId = $button.data('product-id');
        $button.prop('disabled', true);

        $.ajax({
            url: urls.toggleStatus,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { productId: productId },
            success: function (data) {
                if (data.success) {
                    $button.text(data.newIsActive ? "Active" : "Inactive");
                    $button.removeClass("bg-success-subtle text-success bg-danger-subtle text-danger");
                    $button.addClass(data.newIsActive ? "bg-success-subtle text-success" : "bg-danger-subtle text-danger");
                } else {
                    alert(data.message);
                }
            },
            complete: function () {
                $button.prop('disabled', false);
            }
        });
    });

    // 3. View Details
    $(document).on('click', '.js-view-details', function () {
        let $button = $(this);
        let productId = $button.data('product-id');
        let productName = $button.data('product-name');

        $('#productDetailsModalLabel').text("Details for " + productName);
        $('#modal-details-content').html('<div class="loading-spinner"></div>');
        $('#productDetailsModal').modal('show');

        $.get(urls.getProductDetailsPartial, { productId: productId }, function (data) {
            $('#modal-details-content').html(data);
        }).fail(function () {
            $('#modal-details-content').html('<div class="error-message">Failed to load details.</div>');
        });
    });

    // 4. Edit Product (Main)
    $(document).on('click', '.js-edit-product', function () {
        let $button = $(this);
        let productId = $button.data('product-id');
        let productName = $button.data('product-name');

        $('#editProductModalLabel').text("Edit: " + productName);
        $('#modal-edit-content').html('<div class="loading-spinner"></div>');
        $('#editProductModal').modal('show');

        $.get(urls.getEditPartial, { productId: productId }, function (data) {
            $('#modal-edit-content').html(data);
        }).fail(function () {
            $('#modal-edit-content').html('<div class="error-message">Failed to load form.</div>');
        });
    });

    // 5. Delete Product
    $(document).on('click', '.js-delete-product', function () {
        let productId = $(this).data('product-id');
        let productName = $(this).data('product-name');

        $('#modal-delete-product-name').text(productName);
        $('#confirm-delete-button').data('product-id', productId);
        $('#deleteProductModal').modal('show');
    });

    // Checkbox Logic for Delete
    let deleteCheckbox = document.getElementById('confirm-delete-checkbox');
    if (deleteCheckbox) {
        deleteCheckbox.addEventListener('change', function () {
            document.getElementById('confirm-delete-button').disabled = !this.checked;
        });
    }
    $('#deleteProductModal').on('hidden.bs.modal', function () {
        if (deleteCheckbox) deleteCheckbox.checked = false;
        document.getElementById('confirm-delete-button').disabled = true;
    });

    // Execute Delete Product
    $('#confirm-delete-button').on('click', function () {
        let $button = $(this);
        let productId = $button.data('product-id');
        $button.prop('disabled', true).text('Deleting...');

        $.ajax({
            url: urls.deleteConfirmed,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { productId: productId },
            success: function (data) {
                if (data.success) {
                    $('#deleteProductModal').modal('hide');
                    const $targetRows = $(`tr[data-product-row-id="${productId}"], #drawer-${productId}`);
                    $targetRows.fadeOut(300, function () {
                        $(this).remove();
                    });
                    window.Toast.fire({
                        icon: 'success',
                        title: 'Product deleted successfully!'
                    });
                } else {
                    window.Toast.fire({
                        icon: 'error',
                        title: data.message || 'Failed to delete product.'
                    });
                }
            },
            error: function () {
                window.Toast.fire({
                    icon: 'error',
                    title: 'Something went wrong.'
                });
            }
        });
    });
    // ============================================================
    // 6. MANAGE VARIANTS LIST (Inline Edit & Delete)
    // ============================================================

    // 1. Delete Variant (Opens Confirm Modal)
    modalBody.on('click', '.js-delete-variant', function () {
        let $button = $(this);
        let $row = $button.closest('tr');
        let variantId = $row.data('variant-id');
        let variantName = $row.find('td:first').text().trim();

        $('#modal-delete-variant-name').text(variantName);
        $('#confirm-delete-variant-button').data('variant-id', variantId);

        $('#deleteVariantModal').modal('show');
    });

    // 2. Execute Delete Variant
    $('#confirm-delete-variant-button').on('click', function () {
        let $button = $(this);
        let variantId = $button.data('variant-id');
        $button.prop('disabled', true).text('Deleting...');

        $.ajax({
            url: urls.deleteVariant,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { variantId: variantId },
            success: function (data) {
                if (data.success) {
                    $('#deleteVariantModal').modal('hide');
                    $('tr[data-variant-id="' + variantId + '"]').fadeOut(300, function () {
                        $(this).remove();
                        if ($('#modal-variants-content tbody tr').length === 0) {
                            $('#modal-variants-content').html('<div class="empty-state"><p>No variants found.</p></div>');
                        }
                    });
                } else { alert('Error: ' + data.message); }
            },
            complete: function () { $button.prop('disabled', false).text('Delete Variant'); }
        });
    });

    // 3. Inline Edit Variant
    modalBody.on('click', '.js-edit-variant', function () {
        let $button = $(this);
        let $row = $button.closest('tr');
        let variantId = $row.data('variant-id');
        let isEditMode = $button.hasClass('text-warning');

        if (isEditMode) {
            // --- ENTER EDIT MODE ---
            $row.find('td[data-col="price"] .display-value').hide();
            $row.find('td[data-col="price"] .edit-input').show();
            $row.find('td[data-col="sku"] .display-value').hide();
            $row.find('td[data-col="sku"] .edit-input').show();

            // Show [+ Add Attr] button
            let nameCell = $row.find('td:first');
            if (nameCell.find('.btn-add-attr-inline').length === 0) {
                nameCell.append(`
                    <button type="button" class="btn btn-xs btn-outline-primary ms-2 btn-add-attr-inline" 
                        title="Add missing attribute" style="padding: 0px 4px; font-size: 10px;">
                        + Add Attr
                    </button>
                `);
            }
            nameCell.find('.btn-add-attr-inline').show();

            $button.removeClass('text-warning').addClass('text-success')
                .html('<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="20 6 9 17 4 12"></polyline></svg>');
        } else {
            // --- SAVE MODE ---
            let newPrice = $row.find('td[data-col="price"] .edit-input').val();
            let newSku = $row.find('td[data-col="sku"] .edit-input').val();

            $button.prop('disabled', true).text('Saving...');

            $.ajax({
                url: urls.updateVariantPrice,
                type: 'POST',
                headers: { 'RequestVerificationToken': token },
                data: {
                    variantId: variantId,
                    newPrice: newPrice,
                    newSku: newSku
                },
                success: function (data) {
                    if (data.success) {
                        $row.find('td[data-col="price"] .display-value').text('Tk. ' + parseFloat(newPrice).toFixed(2));
                        $row.find('td[data-col="sku"] .display-value').text(newSku ? newSku : "N/A");

                        $row.find('.display-value').show();
                        $row.find('.edit-input').hide();
                        $row.find('.btn-add-attr-inline').hide();

                        $button.removeClass('text-success').addClass('text-warning')
                            .html('<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path></svg>');
                    } else {
                        alert('Error: ' + data.message);
                    }
                },
                complete: function () { $button.prop('disabled', false); }
            });
        }
    });

    // 4. HANDLER: Click "[+ Add Attr]"
    modalBody.on('click', '.btn-add-attr-inline', function (e) {
        e.stopPropagation();
        let $row = $(this).closest('tr');
        let variantId = $row.data('variant-id');
        let productId = $('input[name="ProductId"]').val();

        $('#target-variant-id').val(variantId);
        $('#missing-attr-select').html('<option>Loading...</option>');
        $('#missing-value-select').html('<option>Select Value</option>').prop('disabled', true);
        $('#addVariantAttributeModal').modal('show');

        $.get(urls.getMissingAttributes, { productId: productId, variantId: variantId }, function (data) {
            let html = '<option value="">Select Attribute</option>';
            if (data && data.length > 0) {
                data.forEach(a => { html += `<option value="${a.id}">${a.name}</option>`; });
            } else {
                html = '<option value="">No missing attributes!</option>';
            }
            $('#missing-attr-select').html(html);
        });
    });

    // 5. HANDLER: Missing Attribute Selected
    $('#missing-attr-select').on('change', function () {
        let attrId = $(this).val();
        let $valSelect = $('#missing-value-select');

        if (attrId) {
            $valSelect.html('<option>Loading...</option>').prop('disabled', true);
            $.get(urls.getAttributeValues, { attributeId: attrId }, function (data) {
                let html = '<option value="">Select Value</option>';
                data.forEach(v => { html += `<option value="${v.id}">${v.value}</option>`; });
                $valSelect.html(html).prop('disabled', false);
            });
        }
    });

    // 6. HANDLER: Confirm Add Attribute
    $('#btn-confirm-add-attr').on('click', function () {
        let variantId = $('#target-variant-id').val();
        let valId = $('#missing-value-select').val();
        let productId = $('input[name="ProductId"]').val();

        if (!valId) { alert("Select a value"); return; }

        $(this).prop('disabled', true).text('Adding...');

        $.ajax({
            url: urls.addAttributeToVariant,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { variantId: variantId, attributeValueId: valId },
            success: function (data) {
                if (data.success) {
                    $('#addVariantAttributeModal').modal('hide');
                    $.get(urls.getVariantsPartial, { productId: productId }, function (html) {
                        $('#modal-variants-content').html(html);
                        addAttributeRow();
                    });
                } else {
                    alert("Failed");
                }
            },
            complete: function () {
                $('#btn-confirm-add-attr').prop('disabled', false).text('Add & Update');
            }
        });
    });

    // ============================================================
    // 7. DISCOUNT MODAL LOGIC
    // ============================================================

    // 1. Open Modal (Discounts) - FIXED DELEGATION
    $(document).on('click', '.js-manage-discounts', function () {
        let productId = $(this).data('product-id');
        let name = $(this).data('product-name');

        $('#modal-discount-product-name').text(name);
        $('#modal-discounts-content').html('<div class="loading-spinner"></div>');
        $('#productDiscountsModal').modal('show');

        $.get(urls.getDiscounts, { productId: productId }, function (html) {
            $('#modal-discounts-content').html(html);
        });
    });

    // 2. Add Discount
    // 2. Add Discount
    $(document).on('click', '#btn-save-discount', function () {
        let $form = $('#form-add-discount');
        let productId = $form.find('input[name="ProductId"]').val();
        let $btn = $(this);

        // ✅ REAL-WORLD FIX: Convert User's Local Time to UTC before sending
        // This ensures if you are in UTC+7 and pick 12:51 AM, the server receives the correct UTC time.

        let rawFrom = $form.find('input[name="EffectiveFrom"]').val();
        let rawTo = $form.find('input[name="EffectiveTo"]').val();

        // Helper to convert local datetime input to UTC ISO string
        function toUtc(dateStr) {
            if (!dateStr) return null;
            return new Date(dateStr).toISOString();
        }

        let formData = {
            ProductId: productId,
            DiscountType: $form.find('select[name="DiscountType"]').val(),
            DiscountValue: $form.find('input[name="DiscountValue"]').val(),
            MinQuantity: $form.find('input[name="MinQuantity"]').val(),
            // Send UTC time
            EffectiveFrom: toUtc(rawFrom),
            EffectiveTo: toUtc(rawTo),
            IsActive: $form.find('select[name="IsActive"]').val()
        };

        $btn.prop('disabled', true).text('Adding...');

        $.ajax({
            url: urls.addDiscount,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: formData, // ✅ Send the UTC data object instead of .serialize()
            success: function (data) {
                if (data.success) {
                    $.get(urls.getDiscounts, { productId: productId }, function (html) {
                        $('#modal-discounts-content').html(html);
                    });
                    updateMainGridPrice(productId);
                    // Clear inputs
                    $form.find('input[name="EffectiveFrom"], input[name="EffectiveTo"], input[name="DiscountValue"]').val('');
                } else {
                    alert("Error: " + data.message);
                }
            },
            complete: function () {
                $btn.prop('disabled', false).text('Add Discount');
            }
        });
    });
    // 3. Delete Discount
    // 3. Delete Discount (Swal Replacement)
    $(document).on('click', '.js-delete-discount', function () {
        let $row = $(this).closest('tr');
        let discountId = $row.data('id');
        let type = $row.find('td[data-col="type"] .display-value').text();
        let value = $row.find('td[data-col="value"] .display-value').text();

        // Get ProductId to refresh the grid later
        let productId = $('#form-add-discount input[name="ProductId"]').val();

        Swal.fire({
            title: 'Delete Discount?',
            html: `Are you sure you want to delete: <br/><b>${type} : ${value}</b>?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#dc3545', // Danger Red
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                // Show loading state
                Swal.showLoading();

                $.ajax({
                    url: urls.deleteDiscount,
                    type: 'POST',
                    headers: { 'RequestVerificationToken': token },
                    data: { id: discountId },
                    success: function (data) {
                        if (data.success) {
                            // Show success toast
                            window.Toast.fire({ icon: 'success', title: 'Discount deleted' });

                            // Remove row with animation
                            $row.fadeOut(300, function () {
                                $(this).remove();
                                if ($('.discounts-table tbody tr').length === 0) {
                                    $('.discounts-table tbody').html('<tr><td colspan="7" class="text-center text-muted">No discounts found.</td></tr>');
                                }
                            });

                            // Update main grid price
                            updateMainGridPrice(productId);
                        } else {
                            window.Toast.fire({ icon: 'error', title: data.message || "Error deleting" });
                        }
                    },
                    error: function () {
                        window.Toast.fire({ icon: 'error', title: "Server Error" });
                    }
                });
            }
        });
    });
    // 3b. Confirm Delete Discount
    $(document).on('click', '#confirm-delete-discount-button', function () {
        let $btn = $(this);
        let discountId = $btn.data('discount-id');

        $btn.prop('disabled', true).text('Deleting...');

        $.ajax({
            url: urls.deleteDiscount,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { id: discountId },
            success: function (data) {
                if (data.success) {
                    $('#deleteDiscountModal').modal('hide');
                    let $row = $(`.discounts-table tr[data-id="${discountId}"]`);
                    $row.fadeOut(300, function () {
                        $(this).remove();
                        if ($('.discounts-table tbody tr').length === 0) {
                            $('.discounts-table tbody').html('<tr><td colspan="7" class="text-center text-muted">No discounts found.</td></tr>');
                        }
                    });
                    let productId = $('#form-add-discount input[name="ProductId"]').val();
                    updateMainGridPrice(productId);
                } else {
                    alert("Error: " + data.message);
                }
            },
            complete: function () {
                $btn.prop('disabled', false).text('Delete Discount');
            }
        });
    });

    // 4. Inline Edit Discount
    // 4. Inline Edit Discount
    $(document).on('click', '.js-edit-discount', function () {
        let $btn = $(this);
        let $row = $btn.closest('tr');
        let isEdit = $btn.text().trim() === 'Edit';

        if (isEdit) {
            $row.find('.display-value').hide();
            $row.find('.edit-input').show();
            $btn.text('Save').removeClass('btn-warning').addClass('btn-success');
        } else {
            let id = $row.data('id');
            let productId = $('#form-add-discount input[name="ProductId"]').val();

            // ✅ Helper: Convert to UTC before sending
            function toUtc(dateStr) {
                if (!dateStr) return null;
                return new Date(dateStr).toISOString();
            }

            let rawFrom = $row.find('td[data-col="from"] input').val();
            let rawTo = $row.find('td[data-col="to"] input').val();

            let discount = {
                Id: id,
                ProductId: productId,
                DiscountType: $row.find('td[data-col="type"] select').val(),
                DiscountValue: $row.find('td[data-col="value"] input').val(),
                MinQuantity: $row.find('td[data-col="qty"] input').val(),
                // ✅ FIX: Convert the edited time (Dhaka) to UTC before sending
                EffectiveFrom: toUtc(rawFrom),
                EffectiveTo: toUtc(rawTo),
                IsActive: $row.find('td[data-col="active"] select').val(),
                CreatedBy: "",
                CreatedAt: new Date().toISOString()
            };

            $btn.prop('disabled', true).text('Saving...');

            $.ajax({
                url: urls.updateDiscount,
                type: 'POST',
                headers: { 'RequestVerificationToken': token },
                data: discount,
                success: function (data) {
                    if (data.success) {
                        $.get(urls.getDiscounts, { productId: productId }, function (html) {
                            $('#modal-discounts-content').html(html);
                        });
                        updateMainGridPrice(productId);
                    } else { alert("Error saving: " + data.message); $btn.prop('disabled', false).text('Save'); }
                },
                error: function () { alert("Server Error"); $btn.prop('disabled', false).text('Save'); }
            });
        }
    });
    // ============================================================
    // 10. MANAGE VIDEOS (Video Logic)
    // ============================================================

    function getEmbedUrl(url) {
        if (!url) return null;
        if (url.includes("facebook.com/plugins/video.php") || url.includes("player.vimeo.com/video/")) return url;

        const ytMatch = url.match(/(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?|shorts)\/|.*[?&]v=)|youtu\.be\/)([^"&?\/\s]{11})/i);
        if (ytMatch && ytMatch[1]) return `https://www.youtube.com/embed/${ytMatch[1]}`;

        const vimeoMatch = url.match(/(?:vimeo\.com\/|player\.vimeo\.com\/video\/)(\d+)/i);
        if (vimeoMatch && vimeoMatch[1]) return `https://player.vimeo.com/video/${vimeoMatch[1]}`;

        if (url.includes("facebook.com") || url.includes("fb.watch")) {
            return `https://www.facebook.com/plugins/video.php?href=${encodeURIComponent(url)}&show_text=false&width=560`;
        }
        return null;
    }

    // A. Open Modal
    $(document).on('click', '.js-manage-videos', function () {
        const productId = $(this).data('product-id');
        const productName = $(this).data('product-name');
        $('#modal-video-product-name').text(productName);
        $('#productVideosModal').modal('show');
        loadVideos(productId);
    });

    // Helper Function to Load Videos
    function loadVideos(productId) {
        const container = $('#modal-videos-content');
        container.html('<div class="text-center py-4"><div class="spinner-border text-primary"></div></div>');
        $.get(window.productConfig.urls.getVideosPartial, { productId: productId })
            .done(function (html) { container.html(html); })
            .fail(function () { container.html('<div class="text-danger text-center">Failed to load videos.</div>'); });
    }

    // B. Live Preview
    $(document).on('input paste', '#video-url-input', function () {
        const url = $(this).val().trim();
        const embedUrl = getEmbedUrl(url);
        const $preview = $('#video-preview-container');
        const $error = $('#video-url-error');
        const $btn = $('#btn-save-video');

        $preview.css({ 'height': '200px', 'width': '100%', 'background': '#000' });

        if (url.length === 0) {
            $preview.hide().empty();
            $error.hide();
            $btn.prop('disabled', false);
            return;
        }

        if (embedUrl) {
            $preview.html(`<iframe src="${embedUrl}" width="100%" height="100%" frameborder="0" allowfullscreen></iframe>`).show();
            $error.hide();
            $btn.prop('disabled', false);
        } else if (url.includes("share") || url.includes("fb.watch")) {
            $preview.hide().empty();
            $error.removeClass("text-danger").addClass("text-warning")
                .html('<i class="fas fa-info-circle"></i> Share links: Preview unavailable, but valid after saving.').show();
            $btn.prop('disabled', false);
        } else {
            $preview.hide().empty();
            $error.removeClass("text-warning").addClass("text-danger")
                .text("Invalid video URL. Supported: YouTube, Vimeo, Facebook.").show();
            $btn.prop('disabled', true);
        }
    });

    // C. Add Video (Submit Form)
    $(document).on('submit', '#form-add-video', function (e) {
        e.preventDefault();
        const urlInput = $('#video-url-input').val().trim();
        if (!getEmbedUrl(urlInput)) {
            $('#video-url-error').text("Invalid Video URL. Supported: YouTube, Vimeo, Facebook.").show();
            return false;
        }

        const form = $(this);
        const btn = form.find('button[type="submit"]');
        const productId = form.find('input[name="ProductId"]').val();

        btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Saving...');

        $.post(window.productConfig.urls.addVideo, form.serialize())
            .done(function (res) {
                if (res.success) {
                    window.Toast.fire({ icon: 'success', title: 'Video saved successfully' });
                    loadVideos(productId);
                } else {
                    window.Toast.fire({ icon: 'error', title: res.message });
                    btn.prop('disabled', false).html('<i class="fas fa-save me-1"></i> Save Video');
                }
            })
            .fail(function () {
                window.Toast.fire({ icon: 'error', title: 'Server Error' });
                btn.prop('disabled', false).html('<i class="fas fa-save me-1"></i> Save Video');
            });
    });

    // D. Delete Video
    $(document).on('click', '.js-delete-video', function () {
        const videoId = $(this).data('video-id');
        const productId = $('#form-add-video input[name="ProductId"]').val();

        Swal.fire({
            title: 'Delete this video?',
            text: "This action cannot be undone.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#dc3545',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                Swal.showLoading();
                $.ajax({
                    url: window.productConfig.urls.deleteVideo,
                    type: 'POST',
                    headers: { 'RequestVerificationToken': token },
                    data: { id: videoId },
                    success: function (res) {
                        if (res.success) {
                            window.Toast.fire({ icon: 'success', title: 'Video deleted' });
                            loadVideos(productId);
                        } else {
                            window.Toast.fire({ icon: 'error', title: res.message });
                        }
                    },
                    error: function () {
                        window.Toast.fire({ icon: 'error', title: 'Server Error' });
                    }
                });
            }
        });
    });

    // E. Set Primary Video
    $(document).on('click', '.js-set-primary-video', function () {
        const btn = $(this);
        const originalHtml = btn.html();
        btn.prop('disabled', true).html('...');

        const videoId = btn.data('video-id');
        const productId = btn.data('product-id');

        $.ajax({
            url: window.productConfig.urls.setPrimaryVideo,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { videoId: videoId, productId: productId },
            success: function (res) {
                if (res.success) {
                    window.Toast.fire({ icon: 'success', title: 'Primary video updated' });
                    loadVideos(productId);
                } else {
                    window.Toast.fire({ icon: 'error', title: res.message });
                    btn.prop('disabled', false).html(originalHtml);
                }
            },
            error: function () {
                window.Toast.fire({ icon: 'error', title: 'Server Error' });
                btn.prop('disabled', false).html(originalHtml);
            }
        });
    });

    // ============================================================
    // 8. PRODUCT IMAGE LOGIC (Cropper.js)
    // ============================================================

    let cropper;

    // 1. Open Image Modal
    $('.js-manage-images').on('click', function () {
        let productId = $(this).data('product-id');
        $('#modal-images-content').data('product-id', productId);

        $.get(urls.getImages, { productId: productId }, function (html) {
            $('#modal-images-content').html(html);
        });
        $('#productImagesModal').modal('show');
    });

    // 2. Handle File Selection
    $(document).on('change', '#upload-image-input', function (e) {
        let files = e.target.files;
        if (files && files.length > 0) {
            let file = files[0];
            let url = URL.createObjectURL(file);

            $('#cropper-wrapper, #cropper-controls').show();
            $('#upload-image-input').closest('.mb-3').hide();

            let img = document.getElementById('image-to-crop');
            img.src = url;

            if (cropper) cropper.destroy();

            img.onload = function () {
                cropper = new Cropper(img, {
                    aspectRatio: 1,
                    viewMode: 1,
                    autoCropArea: 1,
                });
            };
        }
    });

    // 3. Close Cropper
    $(document).on('click', '#btn-close-cropper', function () {
        if (cropper) {
            cropper.destroy();
            cropper = null;
        }
        $('#cropper-wrapper, #cropper-controls').hide();
        $('#upload-image-input').val('').closest('.mb-3').show();
    });

    // 4. Confirm Upload
    $(document).on('click', '#btn-confirm-upload', function () {
        if (!cropper) return;
        let $btn = $(this);
        $btn.prop('disabled', true).text('Uploading...');

        cropper.getCroppedCanvas({ width: 800, height: 800 }).toBlob((blob) => {
            let formData = new FormData();
            let productId = $('#modal-images-content').data('product-id');

            formData.append('file', blob, 'image.jpg');
            formData.append('productId', productId);

            $.ajax({
                url: urls.uploadImage,
                method: 'POST',
                headers: { 'RequestVerificationToken': token },
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    if (response.success) {
                        $.get(urls.getImages, { productId: productId }, function (html) {
                            $('#modal-images-content').html(html);
                            if (cropper) { cropper.destroy(); cropper = null; }
                            $('#cropper-wrapper, #cropper-controls').hide();
                            $('#upload-image-input').val('').closest('.mb-3').show();
                        });
                    } else {
                        alert("Upload failed: " + response.message);
                    }
                },
                complete: function () {
                    $btn.prop('disabled', false).html('<i class="fas fa-check"></i> Save Image');
                }
            });
        });
    });

    // 5. Delete Image
    $(document).on('click', '.js-delete-image', function () {
        let id = $(this).data('id');
        $('#btn-confirm-del-prod-img').data('id', id);
        $('#deleteProductImageModal').modal('show');
    });

    // 6. Confirm Delete Image
    $(document).on('click', '#btn-confirm-del-prod-img', function () {
        let $btn = $(this);
        let id = $btn.data('id');
        let productId = $('#modal-images-content').data('product-id');

        $btn.prop('disabled', true).text('Deleting...');

        $.ajax({
            url: urls.deleteImage,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { id: id },
            success: function (data) {
                if (data.success) {
                    $('#deleteProductImageModal').modal('hide');
                    $.get(urls.getImages, { productId: productId }, function (html) {
                        $('#modal-images-content').html(html);
                    });
                } else { alert("Error deleting"); }
            },
            complete: function () { $btn.prop('disabled', false).text('Delete'); }
        });
    });

    // 7. Zoom Handlers
    $(document).on('click', '.js-prod-zoom-in', function () { if (cropper) cropper.zoom(0.1); });
    $(document).on('click', '.js-prod-zoom-out', function () { if (cropper) cropper.zoom(-0.1); });

    // ============================================================
    // 9. VARIANT IMAGE LOGIC (Cropper.js) - 🟢 WITH REAL-TIME UPDATE
    // ============================================================

    let varCropper;

    // 1. Open Variant Image Modal
    $(document).on('click', '.js-manage-var-images', function () {
        let $row = $(this).closest('tr');
        let variantId = $row.data('variant-id');

        // Store variantId on the container
        $('#modal-var-images-content').data('variant-id', variantId);
        $('#modal-var-images-content').html('<div class="loading-spinner"></div>');

        $.get(urls.getVariantImages, { variantId: variantId }, function (html) {
            $('#modal-var-images-content').html(html);
        });
        $('#variantImagesModal').modal('show');
    });

    // 2. Handle File Selection
    $(document).on('change', '#upload-variant-image-input', function (e) {
        let file = e.target.files[0];
        if (file) {
            let url = URL.createObjectURL(file);
            $('#var-cropper-wrapper, #var-cropper-controls').show();
            let img = document.getElementById('var-image-to-crop');
            img.src = url;
            if (varCropper) varCropper.destroy();
            varCropper = new Cropper(img, { aspectRatio: 1, viewMode: 1, autoCropArea: 1 });
            $(this).val('');
        }
    });

    // 3. Upload Variant Image
    $(document).on('click', '#btn-confirm-var-upload', function () {
        if (!varCropper) return;
        let $btn = $(this);
        $btn.prop('disabled', true).text('Uploading...');
        let variantId = $('#modal-var-images-content').data('variant-id');

        varCropper.getCroppedCanvas({ width: 600, height: 600 }).toBlob((blob) => {
            let formData = new FormData();
            formData.append('file', blob, 'var-img.jpg');
            formData.append('variantId', variantId);

            $.ajax({
                url: urls.uploadVariantImage,
                type: 'POST',
                headers: { 'RequestVerificationToken': token },
                data: formData,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        // A. Refresh Modal Content
                        $.get(urls.getVariantImages, { variantId: variantId }, function (html) {
                            $('#modal-var-images-content').html(html);
                            if (varCropper) { varCropper.destroy(); varCropper = null; }
                            $('#var-cropper-wrapper').hide();
                        });

                        // B. 🟢 UPDATE REAL-TIME COUNT (Background Table)
                        updateVariantImageCount(variantId, 1);
                    } else { alert("Upload failed"); }
                },
                complete: function () { $btn.prop('disabled', false).text('Save'); }
            });
        });
    });

    // 4. Delete Variant Image
    $(document).on('click', '.js-del-var-img', function () {
        let id = $(this).data('id');
        $('#btn-confirm-del-var-img').data('id', id);
        $('#deleteVariantImageModal').modal('show');
    });

    $(document).on('click', '#btn-confirm-del-var-img', function () {
        let $btn = $(this);
        let id = $btn.data('id');
        let variantId = $('#modal-var-images-content').data('variant-id');

        $btn.prop('disabled', true).text('Deleting...');

        $.ajax({
            url: urls.deleteVariantImage,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { id: id },
            success: function () {
                // A. Hide modal and refresh grid
                $('#deleteVariantImageModal').modal('hide');
                $.get(urls.getVariantImages, { variantId: variantId }, function (html) {
                    $('#modal-var-images-content').html(html);
                });

                // B. 🟢 UPDATE REAL-TIME COUNT (Background Table)
                updateVariantImageCount(variantId, -1);
            },
            complete: function () { $btn.prop('disabled', false).text('Delete'); }
        });
    });

    // 5. Variant Zoom & Close
    $(document).on('click', '.js-var-zoom-in', function () { if (varCropper) varCropper.zoom(0.1); });
    $(document).on('click', '.js-var-zoom-out', function () { if (varCropper) varCropper.zoom(-0.1); });
    $(document).on('click', '#btn-close-var-cropper', function () {
        if (varCropper) { varCropper.destroy(); varCropper = null; }
        $('#var-cropper-wrapper').hide();
        $('#var-cropper-controls').hide();
        $('#upload-variant-image-input').val('');
    });

    // 6. Set Primary Image
    $(document).on('change', '.js-set-primary', function () {
        let imageId = $(this).data('id');
        let productId = $(this).data('product-id');

        $.ajax({
            url: '/product/set-primary-image',
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { imageId: imageId, productId: productId },
            success: function (res) {
                if (res.success) {
                    $.get(urls.getImages, { productId: productId }, function (html) {
                        $('#modal-images-content').html(html);
                    });
                }
            }
        });
    });

    // 7. Update Sort Order (Product)
    $(document).on('change', '.js-update-order', function () {
        let imageId = $(this).data('id');
        let newOrder = $(this).val();
        let productId = $('#modal-images-content').data('product-id');

        $.ajax({
            url: '/product/update-image-order',
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { imageId: imageId, sortOrder: newOrder },
            success: function (res) {
                if (res.success) {
                    $.get(urls.getImages, { productId: productId }, function (html) {
                        $('#modal-images-content').html(html);
                    });
                }
            }
        });
    });

    // 8. Update Sort Order (Variant)
    $(document).on('change', '.js-update-var-order', function () {
        let imageId = $(this).data('id');
        let newOrder = $(this).val();
        let variantId = $(this).data('variant-id');

        $.ajax({
            url: '/product/update-variant-image-order',
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { imageId: imageId, displayOrder: newOrder },
            success: function (res) {
                if (res.success) {
                    $.get(urls.getVariantImages, { variantId: variantId }, function (html) {
                        $('#modal-var-images-content').html(html);
                    });
                }
            }
        });
    });

}); // End of Document Ready