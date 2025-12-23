$(document).ready(function () {
    var $modal = $('#cropModal');
    var image = document.getElementById('imageToCrop');
    var cropper;
    var croppedBlob = null;

    // 1. Handle File Selection
    $("#logoInput").change(function (e) {
        var files = e.target.files;
        if (files && files.length > 0) {
            var file = files[0];
            var url = URL.createObjectURL(file);
            image.src = url;
            $modal.modal('show');
            $(this).val('');
        }
    });

    // 2. Initialize Cropper
    $modal.on('shown.bs.modal', function () {
        cropper = new Cropper(image, {
            aspectRatio: 1,
            viewMode: 1,
            autoCropArea: 1,
            crop(event) {
                $("#cropWidth").val(Math.round(event.detail.width));
                $("#cropHeight").val(Math.round(event.detail.height));
            },
        });
    }).on('hidden.bs.modal', function () {
        if (cropper) {
            cropper.destroy();
            cropper = null;
        }
    });

    // 3. Handle Ratio Buttons
    window.setRatio = function(ratio) {
        if(cropper) cropper.setAspectRatio(ratio);
        $(".fluffy-btn-option").removeClass("active");
        $(event.target).addClass("active");
    };

    // 4. Manual Dimension Update
    $("#cropWidth, #cropHeight").on('change keyup', function() {
        if(cropper) {
            cropper.setData({
                width: parseFloat($("#cropWidth").val()),
                height: parseFloat($("#cropHeight").val())
            });
        }
    });

    // 5. Apply Crop
// 5. Apply Crop (Optimized)
    $("#btnCropApply").click(function () {
        if (!cropper) return;

        // Force a maximum size (e.g., 500px) to prevent huge uploads
        var canvas = cropper.getCroppedCanvas({
            width: 500,
            height: 500,
            imageSmoothingEnabled: true,
            imageSmoothingQuality: 'high',
        });

        if (canvas) {
            $("#logoPreview").attr("src", canvas.toDataURL());

            // Convert to Blob (Use PNG to keep transparency, or JPEG for smaller size)
            canvas.toBlob(function (blob) {
                croppedBlob = blob;
                $modal.modal('hide');
            }, 'image/png'); // You can change to 'image/jpeg' and add 0.8 quality if needed
        }
    });
    // 6. Submit Form
    $("#companyProfileForm").submit(function (e) {
        e.preventDefault();

        var formData = new FormData(this);
        var $btn = $("#btnSaveProfile");

        if (croppedBlob) {
            formData.set('LogoFile', croppedBlob, 'company-logo.png');
        }

        $btn.prop("disabled", true).html('<span class="spinner-border spinner-border-sm"></span> Saving...');

        $.ajax({
            url: '/Settings/UpdateCompanyProfile',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                $btn.prop("disabled", false).html('<span>💾</span> Save Changes');

                if (response.success) {
                    // Update Sidebar Text
                    $(".sidebar .fw-bold").text(response.newName);

                    // Update Sidebar Logo with timestamp to force refresh
                    var $sidebarImg = $(".sidebar img.rounded-circle");
                    if ($sidebarImg.length > 0) {
                        $sidebarImg.attr("src", response.newLogoUrl + "?t=" + new Date().getTime());
                    } else {
                        // Reload if no image existed previously
                        location.reload();
                        return;
                    }

                    Swal.fire({
                        title: 'Success!',
                        text: 'Company profile updated successfully.',
                        icon: 'success',
                        confirmButtonText: 'Great!',
                        confirmButtonColor: '#42e695',
                        background: '#f0f4f8',
                        customClass: { popup: 'fluffy-card' }
                    });

                } else {
                    Swal.fire({
                        title: 'Error!',
                        text: response.message,
                        icon: 'error',
                        confirmButtonColor: '#ff6b6b'
                    });
                }
            },
            // ✅ UPDATED ERROR HANDLING
            error: function (xhr, status, error) {
                $btn.prop("disabled", false).html('<span>💾</span> Save Changes');

                let errorMessage = "An unexpected error occurred.";

                // Check for "Payload Too Large" (413) or Connection Reset (0)
                if (xhr.status === 413 || xhr.status === 0) {
                    errorMessage = "Upload failed! The image is too large. Max size allowed is 100MB.";
                } else if (xhr.responseText) {
                    // Try to show server error if available
                    errorMessage = xhr.responseText;
                }

                Swal.fire({
                    title: 'Upload Failed',
                    text: errorMessage,
                    icon: 'error',
                    confirmButtonColor: '#ff6b6b',
                    background: '#fff0f0'
                });
            }
        });    });
});