// Variable to store cropper instance
let cropper;

$(document).ready(function () {

    // 1. OPEN SEO MODAL
    $(document).on('click', '.js-manage-seo', function () {
        const productId = $(this).data('product-id');
        const productName = $(this).data('product-name');

        $('#modal-seo-product-name').text(productName);
        $('#modal-seo-content').html('<div class="text-center py-4"><div class="spinner-border text-primary"></div></div>');
        $('#productSEOModal').modal('show');

        // Load Partial
        $.get('/product/get-seo', { productId: productId })
            .done(function (html) {
                $('#modal-seo-content').html(html);
            })
            .fail(function () {
                $('#modal-seo-content').html('<div class="text-danger text-center">Failed to load SEO configuration.</div>');
            });
    });

    // 2. TRIGGER CROPPER ON FILE SELECT
    $(document).on('change', '#seo-image-upload', function (e) {
        const files = e.target.files;
        if (files && files.length > 0) {
            const file = files[0];
            const url = URL.createObjectURL(file);

            // Set image source in cropper modal
            const image = document.getElementById('cropper-image');
            image.src = url;

            // Show Cropper Modal
            $('#cropperModal').modal('show');

            // Initialize Cropper when modal is shown
            $('#cropperModal').one('shown.bs.modal', function () {
                if (cropper) {
                    cropper.destroy();
                }
                cropper = new Cropper(image, {
                    aspectRatio: 1200 / 630, // Default Recommended
                    viewMode: 1,
                    autoCropArea: 1,
                });
            }).one('hidden.bs.modal', function () {
                // Cleanup when closed
                if (cropper) {
                    cropper.destroy();
                    cropper = null;
                }
                // Clear input if cancelled so same file can be selected again
                $('#seo-image-upload').val('');
            });
        }
    });

    // 3. SWITCH ASPECT RATIO
    window.setAspectRatio = function (ratio) {
        if (cropper) {
            cropper.setAspectRatio(ratio);

            // UI Toggle Logic
            const btns = document.querySelectorAll('.btn-group .btn');
            btns.forEach(b => b.classList.remove('active'));
            event.target.classList.add('active');
        }
    };

    // 4. CROP & UPLOAD
    $(document).on('click', '#btn-crop-upload', function () {
        if (!cropper) return;

        const canvas = cropper.getCroppedCanvas({
            width: 1200, // Optimize resize target
            height: 630,
            fillColor: '#fff',
        });

        canvas.toBlob(function (blob) {
            const formData = new FormData();
            const productId = $('#form-product-seo input[name="ProductId"]').val();

            // Create a filename
            const fileName = "seo_image.jpg";
            formData.append('file', blob, fileName);
            formData.append('productId', productId);

            const token = $('input[name="__RequestVerificationToken"]').val();
            const $btn = $('#btn-crop-upload');

            $btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Uploading...');

            $.ajax({
                url: '/product/upload-seo-image',
                type: 'POST',
                headers: { 'RequestVerificationToken': token },
                data: formData,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        // Update Hidden Input & Preview
                        $('#hidden-og-image-path').val(res.filePath);
                        $('#seo-image-preview').attr('src', res.filePath);
                        $('#seo-image-preview-container').fadeIn();

                        // Close Cropper Modal
                        $('#cropperModal').modal('hide');
                    } else {
                        alert('Upload failed: ' + res.message);
                    }
                },
                error: function () {
                    alert('Server error during upload.');
                },
                complete: function () {
                    $btn.prop('disabled', false).html('<i class="fas fa-check me-1"></i> Crop & Upload');
                }
            });
        });
    });

    // 5. REMOVE IMAGE
    $(document).on('click', '.btn-remove-image', function () {
        if (confirm("Are you sure you want to remove the social image?")) {
            // Clear preview
            $('#seo-image-preview-container').fadeOut();
            $('#seo-image-preview').attr('src', '');

            // Clear hidden value (DB will update on Save)
            $('#hidden-og-image-path').val('');

            // Clear file input
            $('#seo-image-upload').val('');
        }
    });

    // 6. SAVE FORM
    $(document).on('click', '#btn-save-seo', function (e) {
        e.preventDefault();
        const $form = $('#form-product-seo');
        const $btn = $(this);

        $btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Saving...');

        $.ajax({
            url: '/product/save-seo',
            type: 'POST',
            data: $form.serialize(),
            success: function (res) {
                if (res.success) {
                    $('#productSEOModal').modal('hide');
                    if (window.Toast) {
                        window.Toast.fire({ icon: 'success', title: 'SEO settings saved!' });
                    } else {
                        alert('Saved successfully!');
                    }
                } else {
                    alert('Error: ' + res.message);
                    $btn.prop('disabled', false).html('<i class="fas fa-save me-1"></i> Save SEO');
                }
            },
            error: function () {
                alert('Server error.');
                $btn.prop('disabled', false).html('<i class="fas fa-save me-1"></i> Save SEO');
            }
        });
    });
});