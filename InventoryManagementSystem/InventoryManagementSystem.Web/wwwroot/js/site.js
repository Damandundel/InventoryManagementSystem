const searchInput = document.getElementById('liveSearch');
const results = document.getElementById('liveSearchResults');
if (searchInput && results) {
    searchInput.addEventListener('input', async () => {
        const term = searchInput.value.trim();
        results.innerHTML = '';
        if (term.length < 2) return;
        const response = await fetch(`/api/products/search?term=${encodeURIComponent(term)}`);
        if (!response.ok) return;
        const products = await response.json();
        results.innerHTML = products.map(p => `
            <a class="list-group-item list-group-item-action" href="/Products/Details/${p.id}">
                <strong>${p.name}</strong> <span class="text-muted">${p.sku}</span><br />
                ${p.categoryName} | Qty: ${p.quantity}
            </a>`).join('');
    });
}
