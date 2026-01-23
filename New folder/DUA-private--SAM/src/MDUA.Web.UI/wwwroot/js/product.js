// wwwroot/js/product.js

$(document).ready(function () {

    // ==========================================
    //  1. CONFIGURATION & HELPERS
    // ==========================================

    let attrIndex = $("#addProductModal #attributes-container .attribute-row").length;

    // Helper: Cartesian Product for Variants
    function cartesian(arr) {
        return arr.reduce((a, b) => a.flatMap(d => b.map(e => d.concat([e]))), [[]]);
    }

    // Helper: Update Dropdown Availability & Visuals
    function updateAttributeDropdowns() {
        let selectedIds = [];

        // Get currently selected IDs
        $("#addProductModal .attribute-select").each(function () {
            let val = $(this).val();
            if (val) selectedIds.push(val);
        });

        // Disable already selected options in other dropdowns
        $("#addProductModal .attribute-select").each(function () {
            let currentVal = $(this).val();
            $(this).find("option").each(function () {
                let optionVal = $(this).val();
                if (!optionVal) return; // Skip default option

                if (selectedIds.includes(optionVal) && optionVal !== currentVal) {
                    $(this).prop('disabled', true);
                } else {
                    $(this).prop('disabled', false);
                }
            });
        });

        // Ensure one primary is always checked if attributes exist
        if ($(".attribute-row").length > 0 && $(".is-primary-radio:checked").length === 0) {
            $(".attribute-row:first .is-primary-radio").prop("checked", true);
        }
    }

    // ==========================================
    //  2. ATTRIBUTE MANAGEMENT
    // ==========================================

    // ➕ Add new attribute row
    $("#addProductModal").on("click", "#add-attribute", function () {
        // Grab options from the first select to replicate
        let firstSelect = $("#addProductModal #attributes-container .attribute-row:first-child select");
        let optionsHtml = firstSelect.length ? firstSelect.html() : '<option value="">-- Select Attribute --</option>';

        // Determine if this should be checked (if it's the first one)
        let isFirst = $("#attributes-container .attribute-row").length === 0;
        let checkedAttr = isFirst ? "checked" : "";

        let row = `
            <div class="attribute-row mb-2 p-2 border rounded bg-white" data-attr-index="${attrIndex}">
                <div class="d-flex justify-content-between align-items-center mb-2">
                    
                    <div class="form-check" title="This attribute controls the product image (e.g. Color)">
                        <input class="form-check-input is-primary-radio" type="radio" name="primaryAttributeGroup" id="primary_${attrIndex}" ${checkedAttr}>
                        <label class="form-check-label small fw-bold text-primary" for="primary_${attrIndex}">
                            Primary Visual
                        </label>
                    </div>

                    <button type="button" class="btn btn-sm btn-outline-danger remove-attribute border-0" style="padding: 0 5px;">&times;</button>
                </div>

                <select name="Attributes[${attrIndex}].AttributeId" 
                        class="attribute-select form-control-modal form-select form-select-sm">
                    ${optionsHtml}
                </select>
                <div class="attribute-values-container mt-2"></div>
            </div>
        `;
        $("#addProductModal #attributes-container").append(row);
        attrIndex++;
        updateAttributeDropdowns();
    });

    // ❌ Remove attribute row
    $("#addProductModal").on("click", ".remove-attribute", function () {
        $(this).closest(".attribute-row").remove();
        updateAttributeDropdowns(); // Will auto-select new primary if needed
        generateVariants();
    });

    // 🔄 Load attribute values via AJAX
    $("#addProductModal").on("change", ".attribute-select", function () {
        let $select = $(this);
        let row = $select.closest(".attribute-row");
        let container = row.find(".attribute-values-container");
        let attributeId = $select.val();

        container.empty();

        if (!attributeId) {
            updateAttributeDropdowns();
            return;
        }

        container.html('<div class="text-muted small fst-italic">Loading values...</div>');

        // Use config URL or fallback
        let url = (window.productConfig && window.productConfig.urls)
            ? window.productConfig.urls.getAttributeValues
            : "/Product/GetAttributeValues";

        $.ajax({
            url: url,
            type: "GET",
            data: { attributeId: attributeId },
            success: function (data) {
                container.empty();
                if (!data || data.length === 0) {
                    container.html('<span class="text-muted small">No values found.</span>');
                    return;
                }

                data.forEach(v => {
                    let valId = v.id || v.Id;
                    let valName = v.value || v.Value || v.name || v.Name;

                    container.append(`
                    <div class="form-check form-check-inline">
                        <input type="checkbox" 
                               class="form-check-input attribute-value-checkbox" 
                               id="attr_val_${valId}_${Date.now()}"
                               value="${valId}" 
                               data-attrname="${valName}" />
                        <label class="form-check-label" for="attr_val_${valId}_${Date.now()}">
                            ${valName}
                        </label>
                    </div>
                `);
                });

                updateAttributeDropdowns();
            },
            error: function (xhr, status, error) {
                console.error("Error loading attributes:", error);
                container.html('<span class="text-danger small">Error loading data.</span>');
            }
        });
    });

    // ==========================================
    //  3. VARIANT GENERATION
    // ==========================================

    // Refresh variants when checkbox changes
    $("#addProductModal").on("change", ".attribute-value-checkbox", generateVariants);
    // Refresh variants when Primary Radio changes (to update visuals if we add logic later)
    $("#addProductModal").on("change", ".is-primary-radio", function() {
        // Optional: Highlight the primary column in variant table if we wanted
    });

    function generateVariants() {
        let variantsContainer = $("#addProductModal #variants-container");
        variantsContainer.html(""); // Clear existing

        let selectedPerAttribute = [];

        // Iterate rows to build the arrays
        $("#addProductModal .attribute-row").each(function () {
            let checked = $(this).find(".attribute-value-checkbox:checked");
            if (checked.length > 0) {
                let values = [];
                checked.each(function () {
                    values.push({
                        id: $(this).val(),
                        label: $(this).data("attrname")
                    });
                });
                selectedPerAttribute.push(values);
            }
        });

        if (selectedPerAttribute.length === 0) return;

        let combos = cartesian(selectedPerAttribute);
        let productName = $("input[name='ProductName']").val() || "";
        let basePrice = $("#addProductModal input[name='BasePrice']").val() || 0;

        // Header Row
        if (combos.length > 0) {
            variantsContainer.append(`
                <div class="variant-header-row mb-1 d-flex">
                    <span class="w-50" style="font-weight: bold;">Variant Name</span>
                    <span class="w-50" style="font-weight: bold;">Variant Price</span>
                </div>
            `);
        }

        // Create Rows
        combos.forEach((combo, idx) => {
            let label = productName + " - " + combo.map(v => v.label).join(" - ");
            let hiddenInputs = combo.map((v, i) =>
                `<input type="hidden" name="Variants[${idx}].AttributeValueIds[${i}]" value="${v.id}" />`
            ).join("");

            variantsContainer.append(`
                <div class="variant-row mb-2 d-flex align-items-center">
                    ${hiddenInputs}
                    <input type="hidden" name="Variants[${idx}].VariantName" value="${label}" />
                    
                    <span class="w-50 small">${label}</span>
                    
                    <div class="w-50 d-flex align-items-center">
                        <span class="me-1 small">Tk.</span> 
                        <input type="number" 
                               name="Variants[${idx}].VariantPrice"
                               class="form-control form-control-sm form-control-modal" style="width: 100px;" 
                               value="${basePrice}"
                               required />
                        
                        <button type="button" class="btn btn-sm btn-outline-danger remove-variant ms-2" 
                                style="line-height: 1; padding: 0.25rem 0.5rem;">&times;</button>
                    </div>
                </div>
            `);
        });
    }

    // Refresh variants when Product Name changes
    $("#addProductModal input[name='ProductName']").on("input", function () {
        generateVariants();
    });

    // Remove single variant
    $("#addProductModal").on("click", ".remove-variant", function () {
        $(this).closest(".variant-row").remove();
        // Re-indexing handled on submit usually, but good to keep clean
    });

    // Modal Reset Logic
    $('#addProductModal').on('hidden.bs.modal', function () {
        let $form = $(this).find('form');
        $form[0].reset();
        $form.find('.attribute-row').remove(); // Remove all rows
        $form.find('.attribute-values-container').html('');
        $form.find('#variants-container').html('');
        // Add one empty default row back? Optional.
        updateAttributeDropdowns();

        // Reset Slug UI
        $('#slugInput').removeClass('is-valid is-invalid');
        $('#slug-error').hide();
        $('#btn-save-product').prop('disabled', false).text('Create Product');
    });

    // ==========================================
    //  4. SLUG & NAME LOGIC
    // ==========================================
    const $nameInput = $('input[name="ProductName"]');
    const $slugInput = $('#slugInput');
    const $slugError = $('#slug-error');
    const $submitBtn = $('#btn-save-product');
    let slugTimer;
    let isSlugManuallyEdited = false;

    function checkSlugAvailability(slug) {
        if (!slug) return;
        $slugInput.addClass('loading-slug');
        $.get('/product/check-slug', { slug: slug })
            .done(function (data) {
                $slugInput.removeClass('loading-slug');
                if (data.exists) {
                    $slugInput.addClass('is-invalid').removeClass('is-valid');
                    $slugError.show();
                    $submitBtn.prop('disabled', true).text('Fix Slug Error');
                } else {
                    $slugInput.removeClass('is-invalid').addClass('is-valid');
                    $slugError.hide();
                    $submitBtn.prop('disabled', false).text('Create Product');
                }
            });
    }

    $slugInput.on('input', function () {
        const val = $(this).val().trim();
        if (val !== '') isSlugManuallyEdited = true;
        $slugInput.removeClass('is-valid is-invalid');
        $slugError.hide();
        $submitBtn.prop('disabled', false);
        clearTimeout(slugTimer);
        if (val) slugTimer = setTimeout(() => checkSlugAvailability(val), 500);
    });

    $nameInput.on('input', function () {
        generateVariants();
        if (!isSlugManuallyEdited) {
            const name = $(this).val();
            const slug = name.toLowerCase().replace(/[^a-z0-9\s-]/g, '').trim().replace(/\s+/g, '-').replace(/-+/g, '-');
            $slugInput.val(slug);
            clearTimeout(slugTimer);
            if (slug) slugTimer = setTimeout(() => checkSlugAvailability(slug), 500);
        }
    });

    // ==========================================
    //  5. FORM SUBMISSION (CRITICAL RE-ORDERING)
    // ==========================================
    $('form').on('submit', function (e) {

        // A. Prevent submitting empty attribute selects
        const selects = document.querySelectorAll('select[name^="Attributes"]');
        selects.forEach(select => {
            if (!select.value) select.disabled = true;
        });

        // B. PRIMARY ATTRIBUTE RE-ORDERING
        // 1. Find the row that contains the checked radio button
        var $primaryRow = $('.is-primary-radio:checked').closest('.attribute-row');

        // 2. If found, move it to the top of the container
        if ($primaryRow.length > 0) {
            var $container = $('#attributes-container');
            // Detach and prepend to make it the first child in the DOM
            $primaryRow.detach().prependTo($container);
        }

        // 3. Re-index all attribute rows so the Server sees Primary as Index 0
        $('#attributes-container .attribute-row').each(function(index) {
            // Update Select Name
            $(this).find('select.attribute-select').attr('name', `Attributes[${index}].AttributeId`);

            // Note: We don't strictly need to re-index the checkbox values inside because 
            // the Variants are generated separately. But the "Attributes" list sent to server 
            // will now have the Primary one at Index 0.
        });

        // 4. Proceed with submission...
    });

});