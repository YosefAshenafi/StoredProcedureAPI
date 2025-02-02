function refreshList() {
    window.location.reload();
}

function handleCheckboxChange(checkbox) {
    const hiddenInput = document.getElementById('hidden_' + checkbox.id.replace('checkbox_', ''));
    hiddenInput.value = checkbox.checked;
    checkbox.form.submit();
}

function showNotification(message) {
    const toastContainer = document.querySelector('.toast-container');
    const toast = document.createElement('div');
    toast.className = 'toast show';
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');
    
    toast.innerHTML = `
        <div class="toast-header bg-success text-white">
            <i class="bi bi-check-circle me-2"></i>
            <strong class="me-auto">Success</strong>
            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
        <div class="toast-body">
            ${message}
        </div>
    `;
    
    toastContainer.appendChild(toast);
    
    setTimeout(() => {
        toast.remove();
    }, 3000);
}

document.addEventListener('DOMContentLoaded', function() {
    const notificationData = document.querySelector('[data-notification]');
    if (notificationData && notificationData.dataset.notification) {
        try {
            const data = JSON.parse(notificationData.dataset.notification);
            showNotification(`API "${data.procedureName}" is now ${data.visibility ? 'visible' : 'hidden'}`);
        } catch (error) {
            console.error('Error parsing notification data:', error);
        }
    }
    
    const searchInput = document.getElementById('searchInput');
    
    searchInput.addEventListener('input', function() {
        const searchTerm = this.value.toLowerCase();
        const spRows = document.querySelectorAll('.sp-row');
        
        spRows.forEach(row => {
            const spName = row.querySelector('.sp-name').textContent.toLowerCase();
            const nextRow = row.nextElementSibling;
            
            if (spName.includes(searchTerm)) {
                row.style.display = '';
                if (nextRow) nextRow.style.display = '';
            } else {
                row.style.display = 'none';
                if (nextRow) nextRow.style.display = 'none';
            }
        });
    });

    const spRows = document.querySelectorAll('.sp-row');
    spRows.forEach(row => {
        const link = row.querySelector('a[data-bs-toggle="collapse"]');
        const procedureName = link.textContent.trim().replace(/\s+/g, '');
        
        link.addEventListener('click', async function() {
            const isExpanded = this.getAttribute('aria-expanded') === 'true';
            if (isExpanded) {
                spRows.forEach(r => r.classList.remove('selected'));
                row.classList.add('selected');
                
                const spinners = row.nextElementSibling.querySelectorAll('.loading-spinner');
                spinners.forEach(spinner => spinner.classList.remove('d-none'));
                
                try {
                    const response = await fetch(`/api/dynamicApi/procedures/${procedureName}/details`);
                    const collapseElement = row.nextElementSibling.querySelector('.card-body');
                    
                    if (response.status === 404) {
                        collapseElement.innerHTML = `
                            <div class="code-block-container">
                                <div class="code-block">
                                    <div class="json-editor">
                                        <pre class="mb-0 text-warning"><code>⚠️ This API is currently private. Please enable visibility or contact your administrator to make it accessible.</code></pre>
                                    </div>
                                </div>
                            </div>`;
                        return;
                    }
                    
                    collapseElement.innerHTML = `
                        <div class="row g-4">
                            <div class="col-md-6">
                                <div class="code-block-container">
                                    <div class="code-block">
                                        <div class="d-flex justify-content-between align-items-center mb-2">
                                            <h6 class="mb-0">
                                                <i class="bi bi-arrow-right-square me-2"></i>Details
                                            </h6>
                                        </div>
                                        <div class="json-editor">
                                            <div class="loading-spinner d-none">
                                                <div class="spinner-border text-light" role="status">
                                                    <span class="visually-hidden">Loading...</span>
                                                </div>
                                            </div>
                                            <pre class="mb-0"><code id="parameters_${procedureName.replace(/\./g, '_')}"></code></pre>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="code-block-container">
                                    <div class="code-block">
                                        <div class="d-flex justify-content-between align-items-center mb-2">
                                            <h6 class="mb-0">
                                                <i class="bi bi-arrow-right-square me-2"></i>Sample Request
                                            </h6>
                                        </div>
                                        <div class="json-editor">
                                            <div class="loading-spinner d-none">
                                                <div class="spinner-border text-light" role="status">
                                                    <span class="visually-hidden">Loading...</span>
                                                </div>
                                            </div>
                                            <pre class="mb-0"><code id="response_${procedureName.replace(/\./g, '_')}"></code></pre>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    
                    const data = await response.json();
                    const parametersElement = document.getElementById(`parameters_${procedureName.replace(/\./g, '_')}`);
                    const responseElement = document.getElementById(`response_${procedureName.replace(/\./g, '_')}`);
                    
                    // Format and display parameters
                    if (parametersElement && data.parameters) {
                        const formattedParams = {};
                        data.parameters.forEach(p => {
                            formattedParams[p.name] = {
                                type: p.dataType,
                                mode: p.mode
                            };
                        });
                        parametersElement.textContent = JSON.stringify(formattedParams, null, 2);
                    }

                    // Format and display sample response
                    if (responseElement && data.parameters) {
                        const sampleRequest = {};
                        data.parameters.forEach(p => {
                            if (p.mode.toUpperCase() === 'IN') {
                                sampleRequest[p.name] = getSampleValue(p.dataType);
                            }
                        });
                        responseElement.textContent = JSON.stringify(sampleRequest, null, 2);
                    }
                } catch (error) {
                    console.error('Error fetching procedure details:', error);
                    const parametersElement = document.getElementById(`parameters_${procedureName.replace(/\./g, '_')}`);
                    const responseElement = document.getElementById(`response_${procedureName.replace(/\./g, '_')}`);
                    if (parametersElement) {
                        parametersElement.textContent = error.message || 'An unexpected error occurred';
                    }
                    if (responseElement) {
                        responseElement.textContent = error.message || 'An unexpected error occurred';
                    }
                } finally {
                    spinners.forEach(spinner => spinner.classList.add('d-none'));
                }
            } else {
                row.classList.remove('selected');
            }
        });
    });
});

function getSampleValue(dataType) {
    switch (dataType.toLowerCase()) {
        case 'varchar':
        case 'char':
        case 'text':
            return "sample_text";
        case 'int':
        case 'bigint':
        case 'smallint':
        case 'tinyint':
            return 0;
        case 'decimal':
        case 'float':
        case 'double':
            return 0.0;
        case 'datetime':
        case 'timestamp':
        case 'date':
            return "2024-01-01";
        case 'bit':
        case 'boolean':
            return false;
        default:
            return "";
    }
}