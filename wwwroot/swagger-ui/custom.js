(function () {
    function addNavigationLink() {
        const topbarWrapper = document.querySelector('.topbar-wrapper');
        if (topbarWrapper) {
            // Remove existing link if any
            const existingLink = document.querySelector('.custom-nav-link');
            if (existingLink) {
                existingLink.remove();
            }

            const link = document.createElement('a');
            link.href = '/StoredProcedure';
            link.className = 'custom-nav-link';
            link.innerHTML = '← Stored Procedures UI';
            link.style.marginLeft = '20px';
            topbarWrapper.appendChild(link);
        }
    }

    // Try multiple times as Swagger UI might load dynamically
    window.addEventListener("load", function () {
        addNavigationLink();
        setTimeout(addNavigationLink, 100);
        setTimeout(addNavigationLink, 500);
        setTimeout(addNavigationLink, 1000);
    });
})();