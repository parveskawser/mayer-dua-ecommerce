$(document).ready(function () {

    let attrIndex = $("#addProductModal #attributes-container .attribute-row").length;
    // Hides/shows options based on what's already selected
    function updateAttributeDropdowns() {
        let selectedIds = [];

        // 1. Get all currently selected attribute IDs
        $("#addProductModal .attribute-select").each(function () {
            let val = $(this).val();
            if (val) {
                selectedIds.push(val);
            }
        });

        // 2. Loop through all dropdowns
        $("#addProductModal .attribute-select").each(function () {
            let currentVal = $(this).val();

            // 3. Loop through all options in this dropdown
            $(this).find("option").each(function () {
                let optionVal = $(this).val();
                if (!optionVal) return; // Skip the "-- Select --" option

                // 4. ✅ Use .prop('disabled', true/false) instead of hide/show
                if (selectedIds.includes(optionVal) && optionVal !== currentVal) {
                    $(this).prop('disabled', true);
                } else {
                    $(this).prop('disabled', false);
                }
            });
        });
    }
    function cartesian(arr) {
        return arr.reduce((a, b) => a.flatMap(d => b.map(e => d.concat([e]))), [[]]);
    }

    // ➕ Add new attribute row
    $("#addProductModal").on("click", "#add-attribute", function () {
        let firstOptions = $("#addProductModal #attributes-container .attribute-row:first-child select").html();
        let row = `
            <div class="attribute-row mb-2" data-attr-index="${attrIndex}">
                <select name="Attributes[${attrIndex}].AttributeId" 
                        class="attribute-select form-control-modal">
                    ${firstOptions}
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

    // 🔄 Load attribute values
    $("#addProductModal").on("change", ".attribute-select", function () {
        // ... (This function is unchanged)
        let row = $(this).closest(".attribute-row");
        let container = row.find(".attribute-values-container");
        container.html("");
        updateAttributeDropdowns();
        let attributeId = $(this).val();
        if (!attributeId) return;

        $.get("/product/get-attribute-values", { attributeId: attributeId }, function (data) {
            data.forEach(v => {
                container.append(`
                    <label class="d-block">
                        <input type="checkbox" 
                               class="attribute-value-checkbox" 
                               value="${v.id}" 
                               data-attrname="${v.value}" />
                        ${v.value}
                    </label>
                `);
            });
        });
    });

    // 🔁 Refresh variants when clicking attribute value checkbox
    $("#addProductModal").on("change", ".attribute-value-checkbox", generateVariants);

    // 🚀 Generate product variants (THIS FUNCTION IS MODIFIED)
    function generateVariants() {
        let variantsContainer = $("#addProductModal #variants-container");
        variantsContainer.html(""); // Clear existing variants

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

        // ✅ 1. ADDED: Header Row
        if (combos.length > 0) {
            variantsContainer.append(`
                <div class="variant-header-row mb-1 d-flex">
                    <span class="w-50" style="font-weight: bold;">Variant Name</span>
                    <span class="w-50" style="font-weight: bold;">Variant Price</span>
                </div>
            `);
        }

        // Loop to create each variant row
        combos.forEach((combo, idx) => {

            let label = productName + " - " + combo.map(v => v.label).join(" - ");

            let hiddenInputs = combo.map((v, i) =>
                `<input type="hidden" 
                        name="Variants[${idx}].AttributeValueIds[${i}]"
                        value="${v.id}" />`
            ).join("");

            // ✅ MODIFIED: Variant Row HTML
            variantsContainer.append(`
                <div class="variant-row mb-2 d-flex align-items-center">
                    ${hiddenInputs}
                    <input type="hidden" name="Variants[${idx}].VariantName" value="${label}" />
                    
                    <span class="w-50">${label}</span>
                    
                    <div class="w-50 d-flex align-items-center">
                        
                        <span class="me-1">Tk.</span> 
                        
                        <input type="number" 
                               name="Variants[${idx}].VariantPrice"
                               class="form-control-modal" style="width: 100px;" 
                               value="${basePrice}"
                               required />
                        
                        <button type="button" class="btn btn-sm btn-outline-danger remove-variant ms-2" 
                                style="line-height: 1; padding: 0.25rem 0.5rem;">
                                &times;
                        </button>
                    </div>
                </div>
            `);
        });
    }

    // 🔁 Refresh variants when product name is typed
    $("#addProductModal input[name='ProductName']").on("input", generateVariants);

    // ✅ UPDATED: Click handler to remove a variant AND re-index the list
    $("#addProductModal").on("click", ".remove-variant", function () {

        // 1. Remove the row the user clicked on
        $(this).closest(".variant-row").remove();

        // 2. Find all remaining variant rows
        let remainingRows = $("#addProductModal #variants-container .variant-row");

        // 3. Loop through each remaining row to update its index
        remainingRows.each(function (newIndex) {
            let $row = $(this);

            // 4. Find all input fields in this row
            $row.find("input").each(function () {
                let $input = $(this);
                let oldName = $input.attr("name");

                if (oldName) {
                    // 5. Use a regular expression to replace the old index (e.g., "Variants[3]")
                    // with the new sequential index (e.g., "Variants[2]")
                    let newName = oldName.replace(/^(Variants\[)\d+/, `$1${newIndex}`);
                    $input.attr("name", newName);
                }
            });
        });

        // 6. (Optional) If no variant rows are left, remove the header
        if (remainingRows.length === 0) {
            $("#addProductModal #variants-container .variant-header-row").remove();
        }
    });
    // ✅ This handles the "Cancel" or "X" button click
    $('#addProductModal').on('hidden.bs.modal', function () {
        let $form = $(this).find('form');

        // 1. Reset all standard form fields (inputs, selects, textareas)
        $form[0].reset();

        // 2. Remove all dynamically added attribute rows, except the first one
        $form.find('.attribute-row:not(:first)').remove();

        // 3. Clear any loaded values from the first attribute row
        $form.find('.attribute-values-container').html('');

        // 4. Clear all generated variants and the header
        $form.find('#variants-container').html('');

        // 5. Make sure all options are visible in the first dropdown again
        updateAttributeDropdowns();
    });

    // (This is the 'Enter' key fix, it's good to keep)
    $("#addProductModal").on("keydown", "input", function (e) {
        if (e.key === "Enter" || e.keyCode === 13) {
            e.preventDefault();
        }
    });


    const $nameInput = $('input[name="ProductName"]');
    const $slugInput = $('input[name="Slug"]');

    // Track if user has manually edited the slug
    let isSlugManuallyEdited = false;

    $slugInput.on('input', function () {
        if ($(this).val().trim() !== '') {
            isSlugManuallyEdited = true;
        }
    });

    $nameInput.on('input', function () {
        // Only auto-generate if the user hasn't manually typed a custom slug
        if (!isSlugManuallyEdited) {
            const name = $(this).val();

            const slug = name.toLowerCase()
                .replace(/[^a-z0-9\s-]/g, '')  // Remove special chars
                .trim()                        // Remove start/end spaces
                .replace(/\s+/g, '-')          // Replace spaces with -
                .replace(/-+/g, '-');          // Remove duplicate -

            $slugInput.val(slug);
        }
    });
});