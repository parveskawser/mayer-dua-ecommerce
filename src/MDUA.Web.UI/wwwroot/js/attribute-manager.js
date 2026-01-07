let currentAttrId = 0;
let isCurrentGlobal = false;

// Selection Logic
function selectAttribute(row, id, name, isGlobal) {
    document.querySelectorAll('.attribute-row').forEach(r => r.classList.remove('selected'));
    row.classList.add('selected');

    currentAttrId = id;
    isCurrentGlobal = isGlobal;
    document.getElementById('valHeader').innerHTML = `<i class="fas fa-list me-2"></i>Values: <b>${name}</b>`;
    document.getElementById('btnAddVal').disabled = false;

    loadValues(id);
}

function loadValues(id) {
    document.getElementById('valuesContainer').innerHTML = '<div class="text-center py-5"><div class="spinner-border text-primary"></div></div>';
    fetch(`/attribute/values-partial/${id}`)
        .then(r => r.text())
        .then(html => {
            document.getElementById('valuesContainer').innerHTML = html;
        })
        .catch(err => {
            document.getElementById('valuesContainer').innerHTML = '<div class="text-danger p-4 text-center">Failed to load values.</div>';
        });
}

// --- Attribute CRUD ---
function openAttributeModal() {
    document.getElementById('attrId').value = 0;
    document.getElementById('attrName').value = '';
    document.getElementById('attrOrder').value = 0;
    document.getElementById('attrActive').checked = true;
    new bootstrap.Modal(document.getElementById('attrModal')).show();
}

function editAttribute(id) {
    fetch(`/attribute/get/${id}`).then(r => r.json()).then(d => {
        document.getElementById('attrId').value = d.id;
        document.getElementById('attrName').value = d.name;
        document.getElementById('attrOrder').value = d.displayOrder;
        document.getElementById('attrActive').checked = d.isActive;
        new bootstrap.Modal(document.getElementById('attrModal')).show();
    });
}

function saveAttribute() {
    const data = {
        Id: document.getElementById('attrId').value,
        Name: document.getElementById('attrName').value,
        DisplayOrder: document.getElementById('attrOrder').value,
        IsActive: document.getElementById('attrActive').checked
    };
    fetch('/attribute/save', {
        method: 'POST', headers: {'Content-Type': 'application/json'}, body: JSON.stringify(data)
    }).then(r => r.json()).then(res => {
        if(res.success) location.reload();
        else Swal.fire('Error', res.message, 'error');
    });
}

function toggleAttrStatus(id, el) {
    fetch(`/attribute/toggle-status?id=${id}&isActive=${el.checked}`, { method: 'POST' })
        .then(r => r.json())
        .then(res => { if(!res.success) { el.checked = !el.checked; Swal.fire('Error', res.message, 'error'); }});
}

function deleteAttribute(id) {
    Swal.fire({
        title: 'Delete Attribute?', text: "This will also delete all its values.", icon: 'warning',
        showCancelButton: true, confirmButtonColor: '#d33', confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(`/attribute/delete/${id}`, { method: 'POST' }).then(r => r.json()).then(res => {
                if(res.success) location.reload();
                else Swal.fire('Error', res.message, 'error');
            });
        }
    });
}

// --- Value CRUD ---
function openValueModal() {
    document.getElementById('valId').value = 0;
    document.getElementById('valParentId').value = currentAttrId;
    document.getElementById('valText').value = '';
    document.getElementById('valOrder').value = 0;
    document.getElementById('valActive').checked = true;
    document.getElementById('cloneAlert').style.display = isCurrentGlobal ? 'block' : 'none';
    new bootstrap.Modal(document.getElementById('valModal')).show();
}

function editValue(id, val, order, active) {
    document.getElementById('valId').value = id;
    document.getElementById('valParentId').value = currentAttrId;
    document.getElementById('valText').value = val;
    document.getElementById('valOrder').value = order;
    document.getElementById('valActive').checked = active;
    document.getElementById('cloneAlert').style.display = 'none'; // Editing implies it's already private
    new bootstrap.Modal(document.getElementById('valModal')).show();
}

function saveValue() {
    const data = {
        Id: document.getElementById('valId').value,
        AttributeId: document.getElementById('valParentId').value,
        Value: document.getElementById('valText').value,
        DisplayOrder: document.getElementById('valOrder').value,
        IsActive: document.getElementById('valActive').checked
    };
    fetch('/attribute/value/save', {
        method: 'POST', headers: {'Content-Type': 'application/json'}, body: JSON.stringify(data)
    }).then(r => r.json()).then(res => {
        if(res.success) {
            bootstrap.Modal.getInstance(document.getElementById('valModal')).hide();
            // Check for Clone
            if(res.attributeId != currentAttrId) {
                Swal.fire('List Cloned', 'Global attribute has been cloned to your company.', 'info')
                    .then(() => location.reload());
            } else {
                loadValues(currentAttrId);
                Swal.fire({ toast: true, position: 'top-end', icon: 'success', title: 'Saved', showConfirmButton: false, timer: 1500 });
            }
        } else Swal.fire('Error', res.message, 'error');
    });
}

function toggleValueStatus(id, el) {
    fetch(`/attribute/value/toggle-status?id=${id}&isActive=${el.checked}`, { method: 'POST' })
        .then(r => r.json())
        .then(res => { if(!res.success) { el.checked = !el.checked; Swal.fire('Error', res.message, 'error'); }});
}

function deleteValue(id) {
    if(!confirm("Delete Value?")) return;
    fetch(`/attribute/value/delete/${id}`, { method: 'POST' }).then(r => r.json()).then(res => {
        if(res.success) loadValues(currentAttrId);
        else Swal.fire('Error', res.message, 'error');
    });
}