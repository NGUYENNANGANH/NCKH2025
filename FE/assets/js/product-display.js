/**
 * Product Display JavaScript
 * This file contains functions for displaying products from the API
 */

// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function() {
  // Initialize the product display
  initProductDisplay();
});

/**
 * Initialize the product display
 */
async function initProductDisplay() {
  // Get the product container
  const productContainer = document.querySelector('.product-container');

  if (!productContainer) {
    console.error('Product container not found');
    return;
  }

  try {
    // Show loading state
    productContainer.innerHTML = '<div class="col-12 text-center"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>';

    // Fetch products from the API
    const products = await sanPhamService.getAllSanPhams();

    // Clear the loading state
    productContainer.innerHTML = '';

    // Display the products
    displayProducts(productContainer, products);
  } catch (error) {
    console.error('Error fetching products:', error);
    productContainer.innerHTML = `<div class="col-12 text-center"><div class="alert alert-danger">Error loading products: ${error.message}</div></div>`;
  }
}

/**
 * Display products in the container
 * @param {HTMLElement} container - The container element
 * @param {Array} products - The products to display
 */
function displayProducts(container, products) {
  if (!products || products.length === 0) {
    container.innerHTML = '<div class="col-12 text-center"><div class="alert alert-info">No products found</div></div>';
    return;
  }

  // Clear the container
  container.innerHTML = '';

  // Create a row for the products
  const row = document.createElement('div');
  row.className = 'row';
  container.appendChild(row);

  // Loop through the products and create HTML for each product
  products.forEach((product, index) => {
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

    // Add the product item to the row
    row.appendChild(productItem);
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

/**
 * Add a product to the cart
 * @param {number} productId - The product ID
 */
function addToCart(productId) {
  // Check if cartService is available
  if (typeof cartService !== 'undefined') {
    // Get the product
    sanPhamService.getSanPhamById(productId)
      .then(product => {
        // Add the product to the cart
        return cartService.addToCart(product, 1);
      })
      .then(() => {
        // Show success message
        showNotification('Product added to cart successfully', 'success');
      })
      .catch(error => {
        console.error('Error adding product to cart:', error);
        showNotification('Error adding product to cart', 'error');
      });
  } else {
    console.error('Cart service not available');
    showNotification('Cart service not available', 'error');
  }
}

/**
 * Add a product to the wishlist
 * @param {number} productId - The product ID
 */
function addToWishlist(productId) {
  // Implement wishlist functionality here
  console.log('Add to wishlist:', productId);
  showNotification('Product added to wishlist', 'success');
}

/**
 * Add a product to the compare list
 * @param {number} productId - The product ID
 */
function addToCompare(productId) {
  // Implement compare functionality here
  console.log('Add to compare:', productId);
  showNotification('Product added to compare list', 'success');
}

/**
 * Show a notification message
 * @param {string} message - The message to show
 * @param {string} type - The message type (success, error, warning, info)
 */
function showNotification(message, type = 'info') {
  // Check if utils is available
  if (typeof utils !== 'undefined' && typeof utils.showNotification === 'function') {
    utils.showNotification(message, type);
  } else {
    // Fallback notification
    alert(message);
  }
}
