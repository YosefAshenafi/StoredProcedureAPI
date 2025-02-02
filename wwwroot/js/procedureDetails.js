function showProcedureDetails(procedureName) {
    fetch(`/api/procedures/${procedureName}/details`)
        .then(response => response.json())
        .then(data => {
            const detailsHtml = `
                <div class="card mb-4">
                    <div class="card-header">
                        <h5 class="mb-0">Parameters</h5>
                    </div>
                    <div class="card-body">
                        <table class="table">
                            <thead>
                                <tr>
                                    <th>Name</th>
                                    <th>Type</th>
                                    <th>Mode</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${data.parameters.map(param => `
                                    <tr>
                                        <td>${param.name}</td>
                                        <td>${param.dataType}</td>
                                        <td>${param.mode}</td>
                                    </tr>
                                `).join('')}
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="card">
                    <div class="card-header">
                        <h5 class="mb-0">Sample Response</h5>
                    </div>
                    <div class="card-body">
                        <pre class="response-preview">${JSON.stringify(data.sampleResponse, null, 2)}</pre>
                    </div>
                </div>`;

            document.getElementById('procedureDetails').innerHTML = detailsHtml;
        })
        .catch(error => {
            console.error('Error:', error);
            document.getElementById('procedureDetails').innerHTML = 
                '<div class="alert alert-danger">Error loading procedure details</div>';
        });
}