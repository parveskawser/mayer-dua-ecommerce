$(document).ready(function () {
    loadAttributes();
});

// --- ATTRIBUTE FUNCTIONS ---

function loadAttributes() {
    $.get('/Attribute/GetAllAttributes', function (response) {
        const tbody = $('#attributesTableBody');
        tbody.empty();

// Inside loadAttributes() function, replace the row generation part:

        response.data.forEach(attr => {
            // New Badge Styles
            const isVariant = attr.isVariantAffecting ?
                '<span class="badge badge-pill-soft bg-soft-primary">Variant</span>' :
                '<span class="badge badge-pill-soft bg-soft-secondary">Specification</span>';

            const row = `
        <tr>
            <td class="ps-4 fw-semibold">${attr.name}</td>
            <td>${attr.displayOrder}</td>
            <td>${isVariant}</td>
            <td class="text-end pe-4">
                <button class="btn btn-action text-primary" onclick="openValuesModal(${attr.id}, '${attr.name}')" title="Manage Values">
                    <i class="fas fa-list"></i>
                </button>
                <button class="btn btn-action text-info" onclick="editAttribute(${attr.id})" title="Edit">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-action text-danger" onclick="deleteAttribute(${attr.id})" title="Delete">
                    <i class="fas fa-trash-alt"></i>
                </button>
            </td>
        </tr>
    `;
            tbody.append(row);
        });    });
}

function openAttributeModal(id) {
    $('#attrId').val(id);
    $('#attrName').val('');
    $('#attrOrder').val(0);
    $('#attrVariant').prop('checked', false);

    // Logic: If ID is 0 (New), Hide Order. Else Show.
    if (id === 0) {
        $('#attrModalTitle').text('New Attribute');
        $('#divAttrOrder').hide(); // <--- HIDE WHEN CREATING
    } else {
        $('#attrModalTitle').text('Edit Attribute');
        $('#divAttrOrder').show();
    }

    var modal = new bootstrap.Modal(document.getElementById('attributeModal'));
    modal.show();
}
function editAttribute(id) {
    $.get(`/Attribute/GetAttribute?id=${id}`, function (data) {
        if(data) {
            $('#attrId').val(data.id);
            $('#attrName').val(data.name);
            $('#attrOrder').val(data.displayOrder);
            $('#attrVariant').prop('checked', data.isVariantAffecting);

            $('#attrModalTitle').text('Edit Attribute');
            $('#divAttrOrder').show(); // <--- SHOW WHEN EDITING

            var modal = new bootstrap.Modal(document.getElementById('attributeModal'));
            modal.show();
        }
    });
}
function saveAttribute() {
    const model = {
        Id: parseInt($('#attrId').val()),
        Name: $('#attrName').val(),
        DisplayOrder: parseInt($('#attrOrder').val()),
        IsVariantAffecting: $('#attrVariant').is(':checked')
    };

    $.ajax({
        url: '/Attribute/SaveAttribute',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(model),
        success: function (res) {
            if (res.success) {
                bootstrap.Modal.getInstance(document.getElementById('attributeModal')).hide();
                Swal.fire('Success', res.message, 'success');
                loadAttributes();
            } else {
                Swal.fire('Error', res.message, 'error');
            }
        }
    });
}

function deleteAttribute(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post('/Attribute/DeleteAttribute', { id: id }, function (res) {
                if (res.success) {
                    Swal.fire('Deleted!', res.message, 'success');
                    loadAttributes();
                } else {
                    Swal.fire('Error', res.message, 'error');
                }
            });
        }
    });
}

// --- ATTRIBUTE VALUE FUNCTIONS ---

function openValuesModal(attributeId, attributeName) {
    $('#valAttributeId').val(attributeId);
    $('#currentAttrName').text(attributeName);
    $('#valName').val('');
   // $('#valOrder').val(0);

    loadValues(attributeId);

    var modal = new bootstrap.Modal(document.getElementById('valuesModal'));
    modal.show();
}

function loadValues(attributeId) {
    $.get(`/Attribute/GetValuesByAttribute?attributeId=${attributeId}`, function (response) {
        const tbody = $('#valuesTableBody');
        tbody.empty();

        response.data.forEach(val => {
            const row = `
                <tr>
                    <td>${val.value}</td>
                    <td>${val.displayOrder}</td>
                    <td class="text-end">
                        <button class="btn btn-sm btn-link text-danger" onclick="deleteValue(${val.id})">
                            <i class="fas fa-times"></i>
                        </button>
                    </td>
                </tr>
            `;
            tbody.append(row);
        });
    });
}

function saveValue() {
    const model = {
        Id: 0,
        AttributeId: parseInt($('#valAttributeId').val()),
        Value: $('#valName').val(),
        DisplayOrder: 0 // <--- SET DEFAULT TO 0 (Since input is removed)
    };

    $.ajax({
        url: '/Attribute/SaveValue',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(model),
        success: function (res) {
            if (res.success) {
                $('#valName').val(''); // Clear only the name input
                loadValues(model.AttributeId);
            } else {
                Swal.fire('Error', res.message, 'error');
            }
        }
    });
}
function deleteValue(id) {
    // Simple confirmation for values
    if(!confirm("Delete this value?")) return;

    $.post('/Attribute/DeleteValue', { id: id }, function (res) {
        if (res.success) {
            const attrId = $('#valAttributeId').val();
            loadValues(attrId);
        } else {
            Swal.fire('Error', res.message, 'error');
        }
    });
}