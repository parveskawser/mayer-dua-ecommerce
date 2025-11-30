$(document).ready(function () {

    // Helper to open modal and load content
    function openCustomerDataModal(title, url) { // Removed customerId argument
        // Show loading state
        $('#dataModalTitle').text(title);
        $('#dataModalBody').html('<div class="loading-spinner text-center py-4"><div class="spinner-border text-primary"></div></div>');

        // Open the generic modal
        $('#dataModal').modal('show');

        // 🛑 FIX: Ensure the backdrop is removed from the previous state
        // This is a common fix for persistent dark screens on repeated modal calls
        $('.modal-backdrop').hide();
        $('#dataModal').css('z-index', 1050); // Ensure modal is on top

        // Fetch data
        $.get(url, function (html) {
            $('#dataModalBody').html(html);
        }).fail(function (xhr) {
            console.error("AJAX Load Failed:", xhr.responseText);
            $('#dataModalBody').html('<div class="alert alert-danger">Failed to load data. Check server logs for exact error.</div>');
        });
    }

    // 1. Orders Button Handler (AJAX + Modal)
    $(document).on('click', '.js-manage-orders', function (e) {
        let customerId = $(e.currentTarget).data('id');
        let customerName = $(e.currentTarget).data('name');

        let url = `/customer/get-orders-partial/${customerId}`;

        // CRITICAL FIX: To prevent the backdrop issue in this shared modal:
        // 1. Hide the modal immediately (to clear the previous backdrop)
        $('#dataModal').modal('hide');
        // 2. Open it after a very short delay to allow the framework to clean up the DOM
        setTimeout(() => {
            openCustomerDataModal(
                `Orders for ${customerName}`,
                url
            );
        }, 10); // 10ms delay is usually enough
    });

    // 2. Addresses Button Handler (AJAX + Modal)
    $(document).on('click', '.js-manage-addresses', function (e) {
        let customerId = $(e.currentTarget).data('id');
        let customerName = $(e.currentTarget).data('name');

        let url = `/customer/get-addresses-partial/${customerId}`;

        // CRITICAL FIX: Repeat the hide-show cycle
        $('#dataModal').modal('hide');
        setTimeout(() => {
            openCustomerDataModal(
                `Addresses for ${customerName}`,
                url
            );
        }, 10);
    });

    $(document).on('click', '.js-view-details', function (e) {
        let customerId = $(e.currentTarget).data('id');
        let customerName = $(e.currentTarget).data('name');

        // This URL must point to your C# action that returns the Customer Details Partial View
        let url = `/customer/get-details-partial/${customerId}`;

        // CRITICAL FIX: Repeat the hide-show cycle
        $('#dataModal').modal('hide');
        setTimeout(() => {
            openCustomerDataModal(
                `Details for ${customerName}`,
                url
            );
        }, 10);
    });
});