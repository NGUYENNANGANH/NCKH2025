/**
 * Best Sellers JavaScript
 * This file contains functions for displaying best selling products from the API
 */

// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function() {
  // Initialize the best sellers display
  initBestSellers();
});

/**
 * Initialize the best sellers display
 */
async function initBestSellers() {
  // Get the best sellers container
  const bestSellersContainer = document.querySelector('#best-sellers .row.gy-4');

  if (!bestSellersContainer) {
    console.error('Best sellers container not found');
    return;
  }

  try {
    // Show loading state
    bestSellersContainer.innerHTML = '<div class="col-12 text-center"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>';

    // Fetch products from the API
    // In a real application, you would have an endpoint for best sellers
    // For now, we'll just use the first 4 products from the general product list
    const products = await sanPhamService.getAllSanPhams();

    // Take the first 4 products (or fewer if there aren't 4)
    const bestSellers = products.slice(0, 4);

    // Clear the loading state
    bestSellersContainer.innerHTML = '';

    // Display the best sellers
    displayBestSellers(bestSellersContainer, bestSellers);
  } catch (error) {
    console.error('Error fetching best sellers:', error);
    bestSellersContainer.innerHTML = `<div class="col-12 text-center"><div class="alert alert-danger">Error loading best sellers: ${error.message}</div></div>`;
  }
}

/**
 * Display best selling products in the container
 * @param {HTMLElement} container - The container element
 * @param {Array} products - The products to display
 */
function displayBestSellers(container, products) {
  if (!products || products.length === 0) {
    container.innerHTML = '<div class="col-12 text-center"><div class="alert alert-info">No best selling products found</div></div>';
    return;
  }

  // Clear the container
  container.innerHTML = '';

  // Loop through the products and create HTML for each product
  products.forEach((product) => {
    // Create a product item element
    const productItem = document.createElement('div');
    productItem.className = 'col-md-6 col-lg-3 mb-4';

    // Determine product status
    const isSale = product.giaBan > 0 && product.giaKhuyenMai && product.giaKhuyenMai < product.giaBan;
    const isSoldOut = product.soLuongTon <= 0;

    // Create the product HTML with image, name, price, and add to cart button
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
