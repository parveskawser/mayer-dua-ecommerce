$(document).ready(function () {
    // Ensure jQuery is loaded
    console.log("Product Category Script Loaded");

    // Make functions globally accessible for inline onclick events
    window.openModal = openModal;
    window.saveCategory = saveCategory;
    window.toggleStatus = toggleStatus;
});

// ===========================================
// 1. OPEN MODAL (Add / Edit)
// ===========================================
function openModal(id) {
    // Use the attribute route defined in the controller: "product-category/add-edit/{id?}"
    var url = '/product-category/add-edit/' + (id || '');

    // Fallback if ID is 0 or null
    if (id === 0) url = '/product-category/add-edit/';

    $.get(url, function (data) {
        $('#modalContent').html(data);
        $('#categoryModal').modal('show');
    }).fail(function (xhr, status, error) {
        console.error("Error loading modal:", error);
        alert("Failed to load form. Please check console for details.");
    });
}

// ===========================================
// 2. SAVE CATEGORY
// ===========================================
function saveCategory() {
    var form = $('#categoryForm');

    // Basic Client-Side Validation
    if (!form[0].checkValidity()) {
        form[0].reportValidity();
        return;
    }

    // Use the attribute route: "product-category/save"
    $.post('/product-category/save', form.serialize(), function (response) {
        if (response.success) {
            $('#categoryModal').modal('hide');
            // Reload to reflect changes (Global -> Private conversion, etc.)
            window.location.reload();
        } else {
            alert('Error: ' + response.message);
        }
    }).fail(function (xhr, status, error) {
        console.error("Error saving:", error);
        alert("Server error occurred. Try again.");
    });
}

// ===========================================
// 3. TOGGLE STATUS (Active / Inactive)
// ===========================================
function toggleStatus(id, checkbox) {
    var isActive = $(checkbox).is(':checked');
    var originalState = !isActive; // To revert if failed

    // Use the attribute route: "product-category/update-status"
    $.post('/product-category/update-status', { id: id, isActive: isActive }, function (response) {
        if (response.success) {
            console.log("Status updated successfully");

            // RELOAD IS CRITICAL HERE:
            // If you deactivate a Global Category, the backend creates a NEW Private Category ID.
            // The UI must reload to bind the "Toggle" switch to the NEW Private ID.
            window.location.reload();
        } else {
            // Revert checkbox visually if backend failed
            $(checkbox).prop('checked', originalState);
            alert('Error: ' + response.message);
        }
    }).fail(function (xhr, status, error) {
        // Revert checkbox on server error
        $(checkbox).prop('checked', originalState);
        console.error("Error updating status:", error);
        alert("Connection failed. Status not updated.");
    });
}