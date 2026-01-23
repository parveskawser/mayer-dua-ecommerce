let homepageCropper;
let homepageCropperRatio = 1200 / 630;

function setHomepageSeoRatio(ratio) {
    if (!homepageCropper) return;
    homepageCropper.setAspectRatio(ratio);
}

function updateHomepageSeoPreview(url) {
    document.getElementById('homepage-og-image').value = url || '';
    const preview = document.getElementById('homepage-og-preview');
    const img = document.getElementById('homepage-og-preview-img');

    if (url) {
        img.src = url;
        preview.style.display = 'block';
    } else {
        img.src = '';
        preview.style.display = 'none';
    }
}

function saveHomepageSeo() {
    const ogWidth = parseInt(document.getElementById('homepage-og-width').value || '0', 10);
    const ogHeight = parseInt(document.getElementById('homepage-og-height').value || '0', 10);

    window.homepageSeo.metaTitle = document.getElementById('homepage-meta-title').value;
    window.homepageSeo.metaDescription = document.getElementById('homepage-meta-description').value;
    window.homepageSeo.customHeaderTags = document.getElementById('homepage-custom-tags').value;
    window.homepageSeo.ogImage = document.getElementById('homepage-og-image').value;
    window.homepageSeo.ogImageWidth = Number.isNaN(ogWidth) ? 1200 : ogWidth;
    window.homepageSeo.ogImageHeight = Number.isNaN(ogHeight) ? 630 : ogHeight;

    Swal.fire({
        title: 'Saving SEO...',
        allowOutsideClick: false,
        didOpen: () => { Swal.showLoading(); }
    });

    fetch('/settings/save-homepage-seo', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            MetaTitle: window.homepageSeo.metaTitle,
            MetaDescription: window.homepageSeo.metaDescription,
            OGImage: window.homepageSeo.ogImage,
            CustomHeaderTags: window.homepageSeo.customHeaderTags,
            OgImageWidth: window.homepageSeo.ogImageWidth,
            OgImageHeight: window.homepageSeo.ogImageHeight
        })
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Saved!',
                    text: 'Homepage SEO settings updated.',
                    timer: 1500,
                    showConfirmButton: false
                });
            } else {
                Swal.fire('Save Failed', data.message || 'Could not save SEO.', 'error');
            }
        })
        .catch(() => Swal.fire('Network Error', 'Could not connect to the server.', 'error'));
}

function removeHomepageOgImage() {
    window.homepageSeo.ogImage = '';
    updateHomepageSeoPreview('');
}

$(document).ready(function () {
    const fileInput = $('#homepage-og-upload');

    $(document).on('click', '[data-seo-ratio]', function (event) {
        const ratioValue = event.currentTarget.getAttribute('data-seo-ratio');
        homepageCropperRatio = ratioValue === 'free' ? NaN : 1200 / 630;

        document.querySelectorAll('[data-seo-ratio]').forEach(btn => btn.classList.remove('active'));
        event.currentTarget.classList.add('active');

        if (homepageCropper) {
            setHomepageSeoRatio(homepageCropperRatio);
        }
    });

    fileInput.on('change', function (e) {
        const files = e.target.files;
        if (!files || files.length === 0) return;

        const file = files[0];
        const url = URL.createObjectURL(file);
        const image = document.getElementById('homepage-cropper-image');
        image.src = url;

        $('#homepageCropperModal').modal('show');

        $('#homepageCropperModal').one('shown.bs.modal', function () {
            if (homepageCropper) {
                homepageCropper.destroy();
            }
            homepageCropper = new Cropper(image, {
                aspectRatio: homepageCropperRatio,
                viewMode: 1,
                autoCropArea: 1
            });
        }).one('hidden.bs.modal', function () {
            if (homepageCropper) {
                homepageCropper.destroy();
                homepageCropper = null;
            }
            fileInput.val('');
        });
    });

    $('#homepage-crop-upload').on('click', function () {
        if (!homepageCropper) return;

        const width = parseInt(document.getElementById('homepage-crop-width').value || '1200', 10);
        const height = parseInt(document.getElementById('homepage-crop-height').value || '630', 10);

        const canvas = homepageCropper.getCroppedCanvas({
            width: Number.isNaN(width) ? 1200 : width,
            height: Number.isNaN(height) ? 630 : height,
            fillColor: '#fff'
        });

        canvas.toBlob(function (blob) {
            const formData = new FormData();
            formData.append('file', blob, 'homepage_seo.jpg');
            formData.append('previousUrl', window.homepageSeo.ogImage || '');

            const $btn = $('#homepage-crop-upload');
            $btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Uploading...');

            $.ajax({
                url: '/settings/upload-homepage-seo-image',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function (res) {
                    if (res.success) {
                        window.homepageSeo.ogImage = res.url;
                        document.getElementById('homepage-og-width').value = Number.isNaN(width) ? 1200 : width;
                        document.getElementById('homepage-og-height').value = Number.isNaN(height) ? 630 : height;
                        updateHomepageSeoPreview(res.url);
                        $('#homepageCropperModal').modal('hide');
                    } else {
                        Swal.fire('Upload Failed', res.message || 'Could not upload image.', 'error');
                    }
                },
                error: function () {
                    Swal.fire('Upload Failed', 'Server error during upload.', 'error');
                },
                complete: function () {
                    $btn.prop('disabled', false).html('<i class="fas fa-check me-1"></i> Crop & Upload');
                }
            });
        });
    });
});
