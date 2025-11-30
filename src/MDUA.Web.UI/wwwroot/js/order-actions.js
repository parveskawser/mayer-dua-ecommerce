document.addEventListener('DOMContentLoaded', function () {
    const toggles = document.querySelectorAll('.confirm-toggle');

    toggles.forEach(toggle => {
        toggle.addEventListener('change', function () {
            const checkbox = this;
            const orderId = checkbox.getAttribute('data-id');
            const isConfirmed = checkbox.checked;

            // UI References
            const statusBadge = document.getElementById(`status-badge-${orderId}`);
            const label = checkbox.nextElementSibling; // The <label> tag containing "Yes"/"No"

            // 1. Optimistic UI Update (Update text immediately)
            label.textContent = isConfirmed ? "Yes" : "No";

            // 2. Prepare Data
            const formData = new URLSearchParams();
            formData.append('id', orderId);
            formData.append('isConfirmed', isConfirmed);

            // Get Anti-Forgery Token from the page
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

            // 3. Send Request
            fetch('/SalesOrder/ToggleConfirmation', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': token
                },
                body: formData.toString()
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        // 4. Success: Update Status Badge Text & Color
                        if (statusBadge) {
                            statusBadge.textContent = data.newStatus;

                            // Update badge color logic
                            if (data.newStatus === 'Confirmed') {
                                statusBadge.className = 'badge bg-success text-white';
                            } else if (data.newStatus === 'Pending') {
                                statusBadge.className = 'badge bg-warning text-dark';
                            } else {
                                statusBadge.className = 'badge bg-secondary-subtle text-dark';
                            }
                        }

                        // Update the toggle switch label color (optional/Bootstrap standard)
                        // The badge surrounding the "Yes/No" is handled by the View's razor logic on load,
                        // but since we are using a Switch, the color is handled by browser/bootstrap CSS.
                    } else {
                        // 5. Logic Error: Revert UI
                        revertUI(checkbox, label, !isConfirmed, data.message);
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    // 6. Network Error: Revert UI
                    revertUI(checkbox, label, !isConfirmed, "Connection failed.");
                });
        });
    });

    // Helper to revert changes if something fails
    function revertUI(checkbox, label, originalState, message) {
        checkbox.checked = originalState;
        label.textContent = originalState ? "Yes" : "No";
        alert("Action failed: " + message);
    }
});