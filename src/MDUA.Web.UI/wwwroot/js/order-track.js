// Wait for the document to be fully loaded
$(document).ready(function () {

    const modal = $('#order-status-modal');
    const openBtn = $('#open-status-modal');
    const closeBtn = modal.find('.close-btn');
    const lookupForm = $('#status-lookup-form');
    const resultArea = $('#status-result');
    const loading = $('#status-loading');

    // --- Modal Control ---
    openBtn.on('click', function () {
        modal.css('display', 'block');
        $('#order-id-input').val('');
        resultArea.empty();
        loading.hide();
    });

    closeBtn.on('click', function () {
        modal.css('display', 'none');
    });

    $(window).on('click', function (event) {
        if ($(event.target).is(modal)) {
            modal.css('display', 'none');
        }
    });


    // --- Order Lookup Logic ---
    lookupForm.on('submit', function (e) {
        e.preventDefault();
        const orderId = $('#order-id-input').val().trim();

        if (orderId === "") {
            resultArea.html('<p style="color: red;">Please enter a valid Online Order ID.</p>');
            return;
        }

        resultArea.empty();
        loading.show();

        // 🚨 IMPORTANT: API Endpoint Call
        $.ajax({
            url: '/Order/GetOrderStatus?orderId=' + encodeURIComponent(orderId),
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                loading.hide();

                if (data && data.orderFound && data.lineItems && data.lineItems.length > 0) {

                    // --- Access all required fields directly from the data object ---
                    const discountAmount = parseFloat(data.discountAmount || 0);
                    const deliveryCharge = parseFloat(data.deliveryCharge || 0);
                    const netAmount = parseFloat(data.netAmount || 0);

                    // Calculate Sub Total
                    const totalAmount = data.lineItems.reduce((sum, item) => sum + item.lineTotal, 0);

                    // --- Build Line Items Table ---
                    let lineItemHtml = data.lineItems.map(item => `
            <tr>
                <td>${item.productName || 'Product Variant'}</td>
                <td class="qty">${item.qty}</td>
                <td class="price">Tk. ${parseFloat(item.price).toFixed(2)}</td>
                <td class="total">Tk. ${parseFloat(item.lineTotal).toFixed(2)}</td>
            </tr>
        `).join('');

                    // --- Build the Receipt HTML ---
                    resultArea.html(`
            <div class="receipt-container">
                <p class="receipt-title">Order Details</p>
                
                <div class="receipt-header-details">
                    <p><strong>Order ID:</strong> ${data.orderId}</p>
                    <p><strong>Status:</strong> <span class="status-badge status-${data.status.toLowerCase()}">${data.status}</span></p>
                    <p><strong>Order Date:</strong> ${data.formattedOrderDate || 'N/A'}</p>
                    <p><strong>Estimated Delivery:</strong> ${data.estimatedDelivery}</p>
                    <p><strong>Customer:</strong> ${data.customerName || 'N/A'}</p>
                    <p><strong>Ship To:</strong> ${data.customerAddress || 'N/A'}</p>
                </div>
                
                <table class="receipt-summary-table">
                    <tr><th>Sub Total (Items)</th><td>Tk. ${totalAmount.toFixed(2)}</td></tr>
                    <tr><th>Discount</th><td class="discount-amount">(-) Tk. ${discountAmount.toFixed(2)}</td></tr>
                    <tr><th>Net Amount</th><td>Tk. ${netAmount.toFixed(2)}</td></tr>
                    <tr><th>Delivery Charge</th><td>(+) Tk. ${deliveryCharge.toFixed(2)}</td></tr>
                    <tr class="grand-total-row">
                        <th>Grand Total (COD)</th>
                        <td>Tk. ${parseFloat(data.grandTotal).toFixed(2)}</td>
                    </tr>
                </table>
                
                <p class="thank-you-message">Thank you for placing your order!</p>
            </div>
        `);
                }
                else {
                    // Render not found result
                    resultArea.html('<p style="color: red; font-weight: bold; text-align: center;">Order Not Found 😔</p><p style="text-align: center;">Please double-check the Order ID.</p>');
                }
            },
            error: function (xhr) {
                loading.hide();
                // Check if the response contains a JSON error message from the C# controller
                let errorMessage = "Could not connect to the tracking service.";
                try {
                    const errorJson = JSON.parse(xhr.responseText);
                    errorMessage = errorJson.message || errorMessage;
                } catch (e) {
                    // Ignore if response is not JSON
                    if (xhr.status === 500) {
                        errorMessage = "Internal Server Error: Data Processing Failed.";
                    }
                }
                resultArea.html(`<p style="color: red; font-weight: bold; text-align: center;">Error: ${errorMessage}</p>`);
            }
        });
    });
});