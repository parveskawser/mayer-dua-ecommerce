$(document).ready(function () {

    // ==========================================
    //  1. CONFIGURATION & HELPERS
    // ==========================================

    let attrIndex = $("#addProductModal #attributes-container .attribute-row").length;

    // Helper: Cartesian Product for Variants
    function cartesian(arr) {
        return arr.reduce((a, b) => a.flatMap(d => b.map(e => d.concat([e]))), [[]]);
    }

    // Helper: Update Dropdown Availability
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
    }

    // ==========================================
    //  2. ATTRIBUTE MANAGEMENT
    // ==========================================

    // ➕ Add new attribute row
    $("#addProductModal").on("click", "#add-attribute", function () {
        // Grab options from the first select to replicate
        let firstSelect = $("#addProductModal #attributes-container .attribute-row:first-child select");
        let optionsHtml = firstSelect.length ? firstSelect.html() : '<option value="">-- Select Attribute --</option>';

        let row = `
            <div class="attribute-row mb-2" data-attr-index="${attrIndex}">
                <select name="Attributes[${attrIndex}].AttributeId" 
                        class="attribute-select form-control-modal form-select form-select-sm">
                    ${optionsHtml}
                </select>
                <div class="attribute-values-container mt-2"></div>
                <button type="button" 
                    class="btn btn-sm btn-outline-danger remove-attribute mt-1">x</button>
            </div>
        `;
        $("#addProductModal #attributes-container").append(row);
        attrIndex++;
        updateAttributeDropdowns();
    });

    // ❌ Remove attribute row
    $("#addProductModal").on("click", ".remove-attribute", function () {
        $(this).closest(".attribute-row").remove();
        updateAttributeDropdowns();
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
                    <div class="form-check">
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

    function generateVariants() {
        let variantsContainer = $("#addProductModal #variants-container");
        variantsContainer.html(""); // Clear existing

        let selectedPerAttribute = [];
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
        // Also trigger slug logic (handled below)
    });

    // Remove single variant
    $("#addProductModal").on("click", ".remove-variant", function () {
        $(this).closest(".variant-row").remove();

        let remainingRows = $("#addProductModal #variants-container .variant-row");
        remainingRows.each(function (newIndex) {
            let $row = $(this);
            $row.find("input").each(function () {
                let $input = $(this);
                let oldName = $input.attr("name");
                if (oldName) {
                    let newName = oldName.replace(/^(Variants\[)\d+/, `$1${newIndex}`);
                    $input.attr("name", newName);
                }
            });
        });

        if (remainingRows.length === 0) {
            $("#addProductModal #variants-container .variant-header-row").remove();
        }
    });

    // Modal Reset Logic
    $('#addProductModal').on('hidden.bs.modal', function () {
        let $form = $(this).find('form');
        $form[0].reset();
        $form.find('.attribute-row:not(:first)').remove();
        $form.find('.attribute-values-container').html('');
        $form.find('#variants-container').html('');
        updateAttributeDropdowns();

        // Reset Slug UI
        $('#slugInput').removeClass('is-valid is-invalid');
        $('#slug-error').hide();
        $('#btn-save-product').prop('disabled', false).text('Create Product');
    });

    // Prevent Form Submit on Enter key
    $("#addProductModal").on("keydown", "input", function (e) {
        if (e.key === "Enter" || e.keyCode === 13) {
            e.preventDefault();
        }
    });

    // Prevent submitting empty attribute values
    $('form').on('submit', function (e) {
        // Find all attribute dropdowns
        const selects = document.querySelectorAll('select[name^="Attributes"]');
        selects.forEach(select => {
            if (!select.value) select.disabled = true; // Disable empty ones so they don't send
        });
    });

    // ==========================================
    //  4. SLUG & NAME LOGIC (NEW VALIDATION)
    // ==========================================

    const $nameInput = $('input[name="ProductName"]');
    const $slugInput = $('#slugInput');
    const $slugError = $('#slug-error');
    const $submitBtn = $('#btn-save-product');
    let slugTimer;
    let isSlugManuallyEdited = false;

    function checkSlugAvailability(slug) {
        if (!slug) return;

        // Visual feedback (optional class you can add to CSS)
        $slugInput.addClass('loading-slug');

        $.get('/product/check-slug', { slug: slug })
            .done(function (data) {
                $slugInput.removeClass('loading-slug');

                if (data.exists) {
                    // ❌ Slug Taken
                    $slugInput.addClass('is-invalid').removeClass('is-valid');
                    $slugError.show();
                    $submitBtn.prop('disabled', true);
                    $submitBtn.text('Fix Slug Error');
                } else {
                    // ✅ Slug Available
                    $slugInput.removeClass('is-invalid').addClass('is-valid');
                    $slugError.hide();
                    $submitBtn.prop('disabled', false);
                    $submitBtn.text('Create Product');
                }
            })
            .fail(function () {
                // Determine if 400/500 error or simple network fail
                $slugInput.removeClass('loading-slug');
            });
    }

    $slugInput.on('input', function () {
        const val = $(this).val().trim();
        if (val !== '') {
            isSlugManuallyEdited = true;
        }

        // Reset UI while typing
        $slugInput.removeClass('is-valid is-invalid');
        $slugError.hide();
        $submitBtn.prop('disabled', false);

        clearTimeout(slugTimer);
        if (val) {
            slugTimer = setTimeout(() => checkSlugAvailability(val), 500);
        }
    });

    $nameInput.on('input', function () {
        // Run variant generation
        generateVariants();

        if (!isSlugManuallyEdited) {
            const name = $(this).val();
            const slug = name.toLowerCase()
                .replace(/[^a-z0-9\s-]/g, '')
                .trim()
                .replace(/\s+/g, '-')
                .replace(/-+/g, '-');

            $slugInput.val(slug);

            clearTimeout(slugTimer);
            if (slug) {
                slugTimer = setTimeout(() => checkSlugAvailability(slug), 500);
            }
        }
    });

});