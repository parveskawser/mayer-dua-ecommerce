//change

$(document).ready(function () {

    // Move modal to body to fix z-index issues
    var dataModal = document.getElementById('dataModal');
    if (dataModal && !dataModal.hasAttribute('data-moved')) {
        document.body.appendChild(dataModal);
        dataModal.setAttribute('data-moved', 'true');
    }

    // Helper to open modal and load content
    function openCustomerDataModal(title, url) {
        // Show loading state
        $('#dataModalTitle').text(title);
        $('#dataModalBody').html('<div class="loading-spinner text-center py-4"><div class="spinner-border text-primary"></div></div>');

        // Open the generic modal with backdrop
        var modalEl = document.getElementById('dataModal');
        var modal = new bootstrap.Modal(modalEl, {
            backdrop: true,
            keyboard: true,
            focus: true
        });

        modal.show();

        // Fetch data
        $.get(url, function (html) {
            $('#dataModalBody').html(html);
        }).fail(function (xhr) {
            console.error("AJAX Load Failed:", xhr.responseText);
            $('#dataModalBody').html('<div class="alert alert-danger m-3">Failed to load data. Please try again.</div>');
        });
    }

    // 1. View Details Button Handler
    $(document).on('click', '.js-view-details', function (e) {
        e.preventDefault();
        let customerId = $(this).data('id');
        let customerName = $(this).data('name');
        let url = `/customer/get-details-partial/${customerId}`;

        openCustomerDataModal(`Details for ${customerName}`, url);
    });

    // 2. Orders Button Handler
    $(document).on('click', '.js-manage-orders', function (e) {
        e.preventDefault();
        let customerId = $(this).data('id');
        let customerName = $(this).data('name');
        let url = `/customer/get-orders-partial/${customerId}`;

        openCustomerDataModal(`Orders for ${customerName}`, url);
    });

    // 3. Addresses Button Handler
    $(document).on('click', '.js-manage-addresses', function (e) {
        e.preventDefault();
        let customerId = $(this).data('id');
        let customerName = $(this).data('name');
        let url = `/customer/get-addresses-partial/${customerId}`;

        openCustomerDataModal(`Addresses for ${customerName}`, url);
    });

    // 4. When modal is shown, ensure proper layering
    $('#dataModal').on('shown.bs.modal', function () {
        // Force correct z-index
        $(this).css('z-index', '1050');
        $('.modal-backdrop').css('z-index', '1040');
    });

    // 5. Clean up when modal closes
    $('#dataModal').on('hidden.bs.modal', function () {
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open');
        $('body').css('overflow', '');
        $('body').css('padding-right', '');
    });
});