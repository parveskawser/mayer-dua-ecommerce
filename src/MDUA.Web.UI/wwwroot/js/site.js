﻿document.addEventListener('DOMContentLoaded', function () {

    // Global Date Converter
    function convertUtcDates() {
        $(".utc-date").each(function () {
            var $this = $(this);
            if ($this.data("converted")) return;

            var utcTime = $this.data("utc");
            if (utcTime) {
                var localDate = new Date(utcTime);
                if (!isNaN(localDate.getTime())) {
                    var formatted = localDate.toLocaleDateString(undefined, {
                        day: 'numeric', month: 'short', year: 'numeric'
                    }) + ", " + localDate.toLocaleTimeString(undefined, {
                        hour: '2-digit', minute: '2-digit'
                    });

                    $this.text(formatted);
                    $this.data("converted", true);
                }
            }
        });
    }

    // Run on page load
    convertUtcDates();

    // Re-run whenever an AJAX request completes
    if (typeof $ !== 'undefined') {
        $(document).ajaxComplete(function () {
            convertUtcDates();
        });
    }
});

// ✅ MOVE FUNCTION OUTSIDE (Make it Global)
// Now HTML buttons can find it!
function addToCart(productId) {
    console.log("Adding product:", productId);

    // 1. Send Request
    fetch('/Cart/Add?productId=' + productId, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        }
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // 2. Update Badge Number Animation
                const badge = document.getElementById('cart-badge');
                if (badge) {
                    badge.innerText = data.count;
                    // Optional: Little "bump" animation
                    badge.style.transform = "translate(-50%, -50%) scale(1.3)";
                    setTimeout(() => badge.style.transform = "translate(-50%, -50%) scale(1)", 200);
                }

                // 3. Show Success Message
                if (typeof Swal !== 'undefined') {
                    const Toast = Swal.mixin({
                        toast: true,
                        position: 'top-end',
                        showConfirmButton: false,
                        timer: 2000,
                        timerProgressBar: false,
                        didOpen: (toast) => {
                            toast.addEventListener('mouseenter', Swal.stopTimer)
                            toast.addEventListener('mouseleave', Swal.resumeTimer)
                        }
                    });

                    Toast.fire({
                        icon: 'success',
                        title: 'Item added to cart'
                    });
                } else {
                    alert("Item added to cart!");
                }
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Failed to add item to cart.');
        });
}