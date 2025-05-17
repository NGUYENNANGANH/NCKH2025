/**
 * Search functionality for products
 * This file handles the search form submission and displaying search results
 */

// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function() {
  // Initialize search functionality
  initSearch();
});

/**
 * Initialize search functionality
 */
function initSearch() {
  // Get the search forms
  const desktopSearchForm = document.getElementById('desktop-search-form');
  const mobileSearchForm = document.getElementById('mobile-search-form');
  const clearSearchButton = document.getElementById('clear-search');
  
  // Add event listeners to the search forms
  if (desktopSearchForm) {
    desktopSearchForm.addEventListener('submit', handleSearchSubmit);
  }
  
  if (mobileSearchForm) {
    mobileSearchForm.addEventListener('submit', handleSearchSubmit);
  }
  
  // Add event listener to the clear search button
  if (clearSearchButton) {
    clearSearchButton.addEventListener('click', clearSearch);
  }
}

/**
 * Handle search form submission
 * @param {Event} event - The form submission event
 */
async function handleSearchSubmit(event) {
  // Prevent the default form submission
  event.preventDefault();
  
  // Get the search input value
  const isDesktop = event.target.id === 'desktop-search-form';
  const searchInput = isDesktop 
    ? document.getElementById('desktop-search-input') 
    : document.getElementById('mobile-search-input');
  
  const keyword = searchInput.value.trim();
  
  // If the keyword is empty, do nothing
  if (!keyword) {
    return;
  }
  
  // Show the search results section
  const searchResultsSection = document.getElementById('search-results');
  const searchResultsContainer = document.getElementById('search-results-container');
  
  if (!searchResultsSection || !searchResultsContainer) {
    console.error('Search results section or container not found');
    return;
  }
  
  // Show the search results section
  searchResultsSection.style.display = 'block';
  
  // Scroll to the search results section
  searchResultsSection.scrollIntoView({ behavior: 'smooth' });
  
  // Show loading state
  searchResultsContainer.innerHTML = '<div class="col-12 text-center"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>';
  
  try {
    // Search for products using the API
    const products = await searchProducts(keyword);
    
    // Display the search results
    displaySearchResults(searchResultsContainer, products, keyword);
  } catch (error) {
    console.error('Error searching for products:', error);
    searchResultsContainer.innerHTML = `<div class="col-12 text-center"><div class="alert alert-danger">Error searching for products: ${error.message}</div></div>`;
  }
}

/**
 * Search for products using the API
 * @param {string} keyword - The search keyword
 * @returns {Promise<Array>} A promise that resolves to an array of matching products
 */
async function searchProducts(keyword) {
  try {
    // Use the sanPhamService to search for products
    return await sanPhamService.searchSanPhams(keyword);
  } catch (error) {
    console.error(`Error searching for products with keyword "${keyword}":`, error);
    throw error;
  }
}

/**
 * Display search results in the container
 * @param {HTMLElement} container - The container element
 * @param {Array} products - The products to display
 * @param {string} keyword - The search keyword
 */
function displaySearchResults(container, products, keyword) {
  // Clear the container
  container.innerHTML = '';
  
  // If no products found, show a message
  if (!products || products.length === 0) {
    container.innerHTML = `<div class="col-12 text-center"><div class="alert alert-info">No products found matching "${keyword}"</div></div>`;
    return;
  }
  
  // Update the section title to show the number of results
  const sectionTitle = document.querySelector('#search-results .section-title p');
  if (sectionTitle) {
    sectionTitle.textContent = `Found ${products.length} product${products.length > 1 ? 's' : ''} matching "${keyword}"`;
  }
  
  // Loop through the products and create HTML for each product
  products.forEach((product) => {
    // Create a product item element
    const productItem = document.createElement('div');
    productItem.className = 'col-md-6 col-lg-3 mb-4';
    
    // Determine product status
    const isSale = product.giaBan > 0 && product.giaKhuyenMai && product.giaKhuyenMai < product.giaBan;
    const isSoldOut = product.soLuongTon <= 0;
    
    // Create the product HTML
    productItem.innerHTML = `
      <div class="card product-card h-100">
        <div class="position-relative">
          <img src="${product.hinhAnh || 'assets/img/product/placeholder.webp'}" 
               class="card-img-top product-img" 
               alt="${product.tenSanPham}"
               onerror="this.src='assets/img/product/placeholder.webp'">
          ${isSale ? 
            `<div class="product-badge sale-badge">
              Sale
            </div>` : ''
          }
          ${isSoldOut ? 
            `<div class="product-badge soldout-badge">
              Sold Out
            </div>` : ''
          }
        </div>
        <div class="card-body d-flex flex-column">
          <h5 class="card-title">
            <a href="product-details.html?id=${product.id_SanPham}" class="text-decoration-none text-dark product-link">${product.tenSanPham}</a>
          </h5>
          
          <div class="product-price mb-4">
            <span class="current-price fw-bold">${formatCurrency(isSale ? product.giaKhuyenMai : product.giaBan)}</span>
            ${isSale ? `<span class="old-price text-decoration-line-through ms-2 text-muted">${formatCurrency(product.giaBan)}</span>` : ''}
          </div>
          
          ${isSoldOut ?
            `<button class="btn btn-secondary w-100 mt-auto" disabled>
              <i class="bi bi-bag-x me-2"></i>Sold Out
            </button>` :
            `<button class="btn btn-primary w-100 mt-auto" onclick="addToCart(${product.id_SanPham})">
              <i class="bi bi-cart me-2"></i>Add to Cart
            </button>`
          }
        </div>
      </div>
    `;
    
    // Make the entire card clickable
    const card = productItem.querySelector('.product-card');
    card.style.cursor = 'pointer';
    card.addEventListener('click', function(e) {
      // Don't navigate if clicking on the Add to Cart button
      if (e.target.closest('.btn')) {
        e.stopPropagation();
        return;
      }
      
      // Navigate to product details page
      window.location.href = `product-details.html?id=${product.id_SanPham}`;
    });
    
    // Add the product item to the container
    container.appendChild(productItem);
  });
}

/**
 * Clear the search results and hide the search results section
 */
function clearSearch() {
  // Hide the search results section
  const searchResultsSection = document.getElementById('search-results');
  if (searchResultsSection) {
    searchResultsSection.style.display = 'none';
  }
  
  // Clear the search input fields
  const desktopSearchInput = document.getElementById('desktop-search-input');
  const mobileSearchInput = document.getElementById('mobile-search-input');
  
  if (desktopSearchInput) {
    desktopSearchInput.value = '';
  }
  
  if (mobileSearchInput) {
    mobileSearchInput.value = '';
  }
  
  // Scroll back to the top of the page
  window.scrollTo({ top: 0, behavior: 'smooth' });
}

/**
 * Format price with currency symbol (VND)
 * @param {number} price - The price to format
 * @returns {string} The formatted price
 */
function formatCurrency(price) {
  return new Intl.NumberFormat('vi-VN', { 
    style: 'currency', 
    currency: 'VND' 
  }).format(price);
}
