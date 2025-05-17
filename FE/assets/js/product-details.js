/**
 * Product Details Page JavaScript
 * This file contains the logic for the product details page
 */

// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function() {
  // Initialize the product details page
  initProductDetailsPage();
});

/**
 * Initialize the product details page
 */
async function initProductDetailsPage() {
  // Get product ID from URL
  const productId = getProductIdFromUrl();

  if (!productId) {
    // If no product ID is found, show error message
    showErrorMessage('Product not found. Please try again.');
    return;
  }

  try {
    // Show loading state
    const productSection = document.getElementById('product-details');
    if (productSection) {
      // Add a loading spinner
      const loadingSpinner = document.createElement('div');
      loadingSpinner.className = 'text-center py-5';
      loadingSpinner.innerHTML = '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>';

      // Insert the loading spinner at the beginning of the section
      productSection.querySelector('.container').prepend(loadingSpinner);

      // Fetch product details
      const product = await sanPhamService.getSanPhamById(productId);

      // Remove loading spinner
      loadingSpinner.remove();

      // Render product details
      renderProductDetails(product);

      // Initialize related products if they exist
      if (product.id_DanhMuc) {
        initRelatedProducts(product.id_DanhMuc);
      }
    } else {
      console.error('Product details section not found');
      showErrorMessage('Product details section not found. Please try again later.');
    }
  } catch (error) {
    console.error('Error loading product details:', error);
    showErrorMessage('Failed to load product details. Please try again later.');
  }

  // Initialize event listeners
  initEventListeners();
}

/**
 * Get the product ID from the URL query parameters
 * @returns {number|null} The product ID or null if not found
 */
function getProductIdFromUrl() {
  const urlParams = new URLSearchParams(window.location.search);
  const productId = urlParams.get('id');

  return productId ? parseInt(productId) : null;
}

/**
 * Show an error message on the page
 * @param {string} message - The error message to show
 */
function showErrorMessage(message) {
  const productSection = document.getElementById('product-details');

  if (productSection) {
    const container = productSection.querySelector('.container');
    if (container) {
      // Clear the container
      container.innerHTML = `
        <div class="row">
          <div class="col-12">
            <div class="alert alert-danger text-center my-5">
              <i class="bi bi-exclamation-triangle-fill me-2"></i>
              ${message}
            </div>
            <div class="text-center">
              <a href="index.html" class="btn btn-primary">Return to Home</a>
            </div>
          </div>
        </div>
      `;
    }
  }
}

/**
 * Render product details on the page
 * @param {Object} product - The product data
 */
function renderProductDetails(product) {
  // Update page title
  document.title = `${product.tenSanPham} - HtStore`;

  // Get product container elements
  const productTitle = document.querySelector('.product-title');
  const productPriceContainer = document.querySelector('.product-price-container');
  const productDescription = document.querySelector('.product-short-description');
  const productCategory = document.querySelector('.product-category');
  const productAvailability = document.querySelector('.product-availability');
  const mainProductImage = document.getElementById('main-product-image');
  const thumbnailsWrapper = document.querySelector('.product-thumbnails-slider .swiper-wrapper');

  // Update product details
  if (productTitle) {
    productTitle.textContent = product.tenSanPham;
  }

  // Update price
  if (productPriceContainer) {
    const isSale = product.giaBan > 0 && product.giaKhuyenMai && product.giaKhuyenMai < product.giaBan;
    const discount = isSale ? Math.round((1 - product.giaKhuyenMai / product.giaBan) * 100) : 0;

    if (isSale) {
      productPriceContainer.innerHTML = `
        <span class="current-price">${formatPrice(product.giaKhuyenMai)}</span>
        <span class="original-price">${formatPrice(product.giaBan)}</span>
        <span class="discount-badge">-${discount}%</span>
      `;
    } else {
      productPriceContainer.innerHTML = `
        <span class="current-price">${formatPrice(product.giaBan)}</span>
      `;
    }
  }

  // Update description
  if (productDescription) {
    productDescription.innerHTML = `<p>${product.moTa || 'No description available.'}</p>`;
  }

  // Update category
  if (productCategory) {
    productCategory.textContent = product.tenDanhMuc || 'Uncategorized';
  }

  // Update availability
  if (productAvailability) {
    const isSoldOut = product.soLuongTon <= 0;

    if (isSoldOut) {
      productAvailability.innerHTML = `
        <i class="bi bi-x-circle-fill text-danger"></i>
        <span>Out of Stock</span>
      `;
    } else {
      productAvailability.innerHTML = `
        <i class="bi bi-check-circle-fill text-success"></i>
        <span>In Stock</span>
        <span class="stock-count">(${product.soLuongTon} items left)</span>
      `;
    }
  }

  // Update main product image
  if (mainProductImage) {
    const imageUrl = product.hinhAnh || 'https://placehold.co/600x400/e9ecef/495057?text=No+Image';
    mainProductImage.src = imageUrl;
    mainProductImage.alt = product.tenSanPham;
    mainProductImage.setAttribute('data-zoom', imageUrl);
  }

  // Update thumbnails
  if (thumbnailsWrapper) {
    thumbnailsWrapper.innerHTML = '';

    // Add main image as first thumbnail
    const imageUrl = product.hinhAnh || 'https://placehold.co/600x400/e9ecef/495057?text=No+Image';
    const mainThumbnail = document.createElement('div');
    mainThumbnail.className = 'swiper-slide thumbnail-item active';
    mainThumbnail.setAttribute('data-image', imageUrl);
    mainThumbnail.innerHTML = `<img src="${imageUrl}" alt="${product.tenSanPham}" class="img-fluid">`;
    thumbnailsWrapper.appendChild(mainThumbnail);

    // Add additional images if available
    if (product.hinhAnhPhu) {
      const additionalImages = product.hinhAnhPhu.split(',');

      additionalImages.forEach(imageUrl => {
        if (imageUrl && imageUrl.trim()) {
          const thumbnail = document.createElement('div');
          thumbnail.className = 'swiper-slide thumbnail-item';
          thumbnail.setAttribute('data-image', imageUrl.trim());
          thumbnail.innerHTML = `<img src="${imageUrl.trim()}" alt="${product.tenSanPham}" class="img-fluid">`;
          thumbnailsWrapper.appendChild(thumbnail);
        }
      });
    }
  }

  // Update quantity selector max value
  const quantityInput = document.querySelector('.quantity-input');
  if (quantityInput) {
    quantityInput.setAttribute('max', product.soLuongTon.toString());

    // Disable add to cart button if out of stock
    const addToCartButton = document.querySelector('.add-to-cart-btn');
    const buyNowButton = document.querySelector('.buy-now-btn');

    if (addToCartButton) {
      const isSoldOut = product.soLuongTon <= 0;
      addToCartButton.disabled = isSoldOut;

      if (isSoldOut) {
        addToCartButton.innerHTML = '<i class="bi bi-bag-x"></i> Sold Out';
        addToCartButton.classList.add('btn-secondary');
        addToCartButton.classList.remove('btn-primary');
      } else {
        addToCartButton.innerHTML = '<i class="bi bi-cart-plus"></i> Add to Cart';
        addToCartButton.classList.add('btn-primary');
        addToCartButton.classList.remove('btn-secondary');
      }
    }

    if (buyNowButton) {
      buyNowButton.disabled = product.soLuongTon <= 0;
    }
  }
}

/**
 * Format price with currency symbol
 * @param {number} price - The price to format
 * @returns {string} The formatted price
 */
function formatPrice(price) {
  return '$' + parseFloat(price).toFixed(2);
}

/**
 * Show a notification to the user
 * @param {string} message - The message to display
 * @param {string} type - The type of notification ('success', 'error', 'warning', 'info')
 */
function showNotification(message, type = 'info') {
  // Check if utils service is available
  if (typeof utils !== 'undefined' && utils.showNotification) {
    utils.showNotification(message, type);
    return;
  }

  // Create notification element
  const notification = document.createElement('div');
  notification.className = `notification notification-${type}`;
  notification.innerHTML = `
    <div class="notification-content">
      <i class="bi ${getIconForType(type)}"></i>
      <span>${message}</span>
    </div>
    <button type="button" class="notification-close">
      <i class="bi bi-x"></i>
    </button>
  `;

  // Add styles
  notification.style.position = 'fixed';
  notification.style.top = '20px';
  notification.style.right = '20px';
  notification.style.zIndex = '9999';
  notification.style.minWidth = '300px';
  notification.style.maxWidth = '400px';
  notification.style.padding = '15px';
  notification.style.borderRadius = '5px';
  notification.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.15)';
  notification.style.display = 'flex';
  notification.style.justifyContent = 'space-between';
  notification.style.alignItems = 'center';
  notification.style.animation = 'fadeIn 0.3s ease-out';

  // Set background color based on type
  switch (type) {
    case 'success':
      notification.style.backgroundColor = '#d4edda';
      notification.style.color = '#155724';
      notification.style.borderLeft = '4px solid #28a745';
      break;
    case 'error':
      notification.style.backgroundColor = '#f8d7da';
      notification.style.color = '#721c24';
      notification.style.borderLeft = '4px solid #dc3545';
      break;
    case 'warning':
      notification.style.backgroundColor = '#fff3cd';
      notification.style.color = '#856404';
      notification.style.borderLeft = '4px solid #ffc107';
      break;
    default:
      notification.style.backgroundColor = '#d1ecf1';
      notification.style.color = '#0c5460';
      notification.style.borderLeft = '4px solid #17a2b8';
  }

  // Add to document
  document.body.appendChild(notification);

  // Add close button event listener
  const closeButton = notification.querySelector('.notification-close');
  if (closeButton) {
    closeButton.addEventListener('click', () => {
      notification.style.animation = 'fadeOut 0.3s ease-out';
      setTimeout(() => {
        notification.remove();
      }, 300);
    });
  }

  // Auto remove after 5 seconds
  setTimeout(() => {
    if (document.body.contains(notification)) {
      notification.style.animation = 'fadeOut 0.3s ease-out';
      setTimeout(() => {
        if (document.body.contains(notification)) {
          notification.remove();
        }
      }, 300);
    }
  }, 5000);

  // Add CSS animations if they don't exist
  if (!document.getElementById('notification-styles')) {
    const style = document.createElement('style');
    style.id = 'notification-styles';
    style.textContent = `
      @keyframes fadeIn {
        from { opacity: 0; transform: translateY(-20px); }
        to { opacity: 1; transform: translateY(0); }
      }
      @keyframes fadeOut {
        from { opacity: 1; transform: translateY(0); }
        to { opacity: 0; transform: translateY(-20px); }
      }
    `;
    document.head.appendChild(style);
  }
}

/**
 * Get the appropriate icon for the notification type
 * @param {string} type - The notification type
 * @returns {string} The icon class
 */
function getIconForType(type) {
  switch (type) {
    case 'success':
      return 'bi-check-circle-fill';
    case 'error':
      return 'bi-exclamation-triangle-fill';
    case 'warning':
      return 'bi-exclamation-circle-fill';
    default:
      return 'bi-info-circle-fill';
  }
}

/**
 * Initialize related products
 * @param {number} categoryId - The category ID
 */
async function initRelatedProducts(categoryId) {
  const relatedProductsContainer = document.getElementById('related-products');

  if (relatedProductsContainer) {
    try {
      // Show loading state
      const loadingOverlay = utils.showLoadingOverlay(relatedProductsContainer, 'Loading related products...');

      // Fetch products in the same category
      const products = await sanPhamService.getSanPhamsByDanhMuc(categoryId);

      // Get current product ID
      const currentProductId = getProductIdFromUrl();

      // Filter out the current product and limit to 4 products
      const relatedProducts = products
        .filter(product => product.id_SanPham !== currentProductId)
        .slice(0, 4);

      // Render related products
      renderRelatedProducts(relatedProductsContainer, relatedProducts);

      // Hide loading state
      utils.hideLoadingOverlay(loadingOverlay);
    } catch (error) {
      console.error('Error loading related products:', error);
      relatedProductsContainer.innerHTML = '<p class="text-center">Failed to load related products.</p>';
    }
  }
}

/**
 * Render related products
 * @param {HTMLElement} container - The container element
 * @param {Array} products - The products to render
 */
function renderRelatedProducts(container, products) {
  // Clear container
  container.innerHTML = '';

  if (products.length === 0) {
    container.innerHTML = '<p class="text-center">No related products found.</p>';
    return;
  }

  // Create product cards
  products.forEach(product => {
    const productCard = document.createElement('div');
    productCard.className = 'col-md-3 col-sm-6';

    productCard.innerHTML = `
      <div class="product-card">
        <div class="product-image">
          <img src="${product.hinhAnh || 'assets/img/product/placeholder.webp'}" alt="${product.tenSanPham}" loading="lazy">
          ${product.coKhuyenMai ? `<span class="badge-sale">-${product.phanTramGiam}%</span>` : ''}
        </div>
        <div class="product-info">
          <h5>${product.tenSanPham}</h5>
          <p class="price">
            ${product.coKhuyenMai ?
              `<span class="original-price">${utils.formatPrice(product.giaBan)}</span> ${utils.formatPrice(product.giaKhuyenMai)}` :
              utils.formatPrice(product.giaBan)}
          </p>
          <a href="product-details.html?id=${product.id_SanPham}" class="btn-view">View Product</a>
        </div>
      </div>
    `;

    container.appendChild(productCard);
  });
}

/**
 * Initialize event listeners
 */
function initEventListeners() {
  // Add to cart button
  const addToCartBtn = document.querySelector('.add-to-cart-btn');

  if (addToCartBtn) {
    addToCartBtn.addEventListener('click', async function(e) {
      e.preventDefault();

      const productId = getProductIdFromUrl();
      const quantityInput = document.querySelector('.quantity-input');

      if (!productId || !quantityInput) return;

      const quantity = parseInt(quantityInput.value);

      if (isNaN(quantity) || quantity <= 0) {
        alert('Please enter a valid quantity');
        return;
      }

      try {
        // Disable button to prevent multiple clicks
        addToCartBtn.disabled = true;
        addToCartBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Adding...';

        // Get product details
        const product = await sanPhamService.getSanPhamById(productId);

        // Check if product is in stock
        if (product.soLuongTon <= 0) {
          showNotification('Sorry, this product is out of stock', 'error');
          return;
        }

        // Add to cart
        if (typeof cartService !== 'undefined' && cartService.addToCart) {
          // Check if user is logged in
          const isLoggedIn = authService.isLoggedIn();

          // Add to cart (backend or localStorage)
          await cartService.addToCart(product, quantity);

          // Show success message
          showNotification(`Added ${quantity} ${quantity === 1 ? 'item' : 'items'} to cart`, 'success');

          // Update cart badge count
          if (typeof updateCartBadge === 'function') {
            updateCartBadge();
          }

          // If user is not logged in, show login suggestion
          if (!isLoggedIn) {
            setTimeout(() => {
              if (confirm('Sign in to sync your cart across devices. Would you like to sign in now?')) {
                window.location.href = 'login-register.html?redirect=' + encodeURIComponent(window.location.href);
              }
            }, 1000);
          }
        } else {
          showNotification(`Added ${quantity} ${quantity === 1 ? 'item' : 'items'} to cart (Cart service not available)`, 'warning');
        }
      } catch (error) {
        console.error('Error adding product to cart:', error);
        showNotification('Failed to add product to cart: ' + (error.message || 'Unknown error'), 'error');
      } finally {
        // Re-enable button
        addToCartBtn.disabled = false;
        addToCartBtn.innerHTML = '<i class="bi bi-cart-plus me-2"></i> Add to Cart';
      }
    });
  }

  // Buy now button
  const buyNowBtn = document.querySelector('.buy-now-btn');

  if (buyNowBtn) {
    buyNowBtn.addEventListener('click', async function(e) {
      e.preventDefault();

      const productId = getProductIdFromUrl();
      const quantityInput = document.querySelector('.quantity-input');

      if (!productId || !quantityInput) return;

      const quantity = parseInt(quantityInput.value);

      if (isNaN(quantity) || quantity <= 0) {
        alert('Please enter a valid quantity');
        return;
      }

      try {
        // Get product details
        const product = await sanPhamService.getSanPhamById(productId);

        // Check if product is in stock
        if (product.soLuongTon <= 0) {
          alert('Sorry, this product is out of stock');
          return;
        }

        // Add to cart and redirect to checkout
        if (typeof cartService !== 'undefined' && cartService.addToCart) {
          await cartService.addToCart(product, quantity);
          window.location.href = 'checkout.html';
        } else {
          alert('Buy now functionality is not available');
        }
      } catch (error) {
        console.error('Error processing buy now:', error);
        alert('Failed to process buy now request');
      }
    });
  }

  // Quantity buttons
  const decreaseBtn = document.querySelector('.quantity-btn.decrease');
  const increaseBtn = document.querySelector('.quantity-btn.increase');
  const quantityInput = document.querySelector('.quantity-input');

  if (decreaseBtn && increaseBtn && quantityInput) {
    decreaseBtn.addEventListener('click', function() {
      const currentValue = parseInt(quantityInput.value);
      if (currentValue > 1) {
        quantityInput.value = currentValue - 1;
      }
    });

    increaseBtn.addEventListener('click', function() {
      const currentValue = parseInt(quantityInput.value);
      const maxValue = parseInt(quantityInput.getAttribute('max') || '99');
      if (currentValue < maxValue) {
        quantityInput.value = currentValue + 1;
      }
    });
  }

  // Thumbnail click events
  const thumbnails = document.querySelectorAll('.thumbnail-item');
  const mainImage = document.getElementById('main-product-image');

  if (thumbnails.length > 0 && mainImage) {
    thumbnails.forEach(thumbnail => {
      thumbnail.addEventListener('click', function() {
        // Remove active class from all thumbnails
        thumbnails.forEach(t => t.classList.remove('active'));

        // Add active class to clicked thumbnail
        this.classList.add('active');

        // Update main image
        const imageUrl = this.getAttribute('data-image');
        if (imageUrl) {
          mainImage.src = imageUrl;
          mainImage.setAttribute('data-zoom', imageUrl);

          // Reinitialize zoom if available
          if (typeof Drift !== 'undefined') {
            // Remove existing drift instances
            const existingDrift = document.querySelector('.drift-zoom-pane');
            if (existingDrift) {
              existingDrift.remove();
            }

            // Initialize new drift instance
            new Drift(mainImage, {
              paneContainer: document.querySelector('.image-zoom-container'),
              inlinePane: false
            });
          }
        }
      });
    });
  }

  // Color options
  const colorOptions = document.querySelectorAll('.color-option');
  const selectedColorText = document.querySelector('.product-colors .selected-option');

  if (colorOptions.length > 0 && selectedColorText) {
    colorOptions.forEach(option => {
      option.addEventListener('click', function() {
        // Remove active class from all options
        colorOptions.forEach(o => o.classList.remove('active'));

        // Add active class to clicked option
        this.classList.add('active');

        // Update selected text
        const colorName = this.getAttribute('data-color');
        if (colorName) {
          selectedColorText.textContent = colorName;
        }
      });
    });
  }

  // Size options
  const sizeOptions = document.querySelectorAll('.size-option');
  const selectedSizeText = document.querySelector('.product-sizes .selected-option');

  if (sizeOptions.length > 0 && selectedSizeText) {
    sizeOptions.forEach(option => {
      option.addEventListener('click', function() {
        // Remove active class from all options
        sizeOptions.forEach(o => o.classList.remove('active'));

        // Add active class to clicked option
        this.classList.add('active');

        // Update selected text
        const sizeName = this.getAttribute('data-size');
        if (sizeName) {
          selectedSizeText.textContent = sizeName;
        }
      });
    });
  }
}
