/**
 * Cart Page JavaScript
 * This file contains the logic for the shopping cart page
 */

// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function() {
  // Initialize the cart page
  initCartPage();
});

/**
 * Initialize the cart page
 */
async function initCartPage() {
  try {
    // Show loading state
    const cartContainer = document.querySelector('.cart-items-container');
    if (cartContainer) {
      const loadingOverlay = utils.showLoadingOverlay(cartContainer, 'Loading cart...');

      // Fetch cart items
      const cart = await cartService.getCart();

      // Render cart items
      await renderCart(cart);

      // Hide loading state
      utils.hideLoadingOverlay(loadingOverlay);
    }
  } catch (error) {
    console.error('Error loading cart:', error);
    utils.showNotification('Failed to load cart. Please try again later.', 'error');
  }

  // Initialize event listeners
  initEventListeners();
}

/**
 * Render the cart
 * @param {Array|Object} cartData - The cart items or response object
 */
async function renderCart(cartData) {
  const cartContainer = document.querySelector('.cart-items-container');
  const cartSummaryContainer = document.querySelector('.cart-summary');
  const emptyCartMessage = document.querySelector('.empty-cart-message');
  const checkoutButton = document.querySelector('.checkout-button');

  if (!cartContainer) return;

  // Clear cart container
  cartContainer.innerHTML = '';

  // Ensure we have an array of cart items
  let cartItems = [];
  if (Array.isArray(cartData)) {
    cartItems = cartData;
  } else if (cartData && typeof cartData === 'object') {
    // Handle case where API returns { data: [...] }
    if (cartData.data && Array.isArray(cartData.data)) {
      cartItems = cartData.data;
    } else if (cartData.isSuccess && cartData.data) {
      cartItems = Array.isArray(cartData.data) ? cartData.data : [];
    }
  }

  console.log('Rendering cart items:', cartItems);

  if (cartItems.length === 0) {
    // Show empty cart message
    if (emptyCartMessage) {
      emptyCartMessage.style.display = 'block';
    }

    // Hide cart summary
    if (cartSummaryContainer) {
      cartSummaryContainer.style.display = 'none';
    }

    // Disable checkout button
    if (checkoutButton) {
      checkoutButton.disabled = true;
    }

    return;
  }

  // Hide empty cart message
  if (emptyCartMessage) {
    emptyCartMessage.style.display = 'none';
  }

  // Show cart summary
  if (cartSummaryContainer) {
    cartSummaryContainer.style.display = 'block';
  }

  // Enable checkout button
  if (checkoutButton) {
    checkoutButton.disabled = false;
  }

  // Get product details for each cart item
  const cartItemsWithDetails = await Promise.all(
    cartItems.map(async (item) => {
      try {
        // If item already has details, use them
        if (item.tenSanPham && item.giaBan) {
          return item;
        }

        // Otherwise, fetch product details
        const product = await sanPhamService.getSanPhamById(item.id_SanPham);
        return {
          ...item,
          tenSanPham: product.tenSanPham,
          giaBan: product.giaBan,
          giaKhuyenMai: product.giaKhuyenMai,
          coKhuyenMai: product.coKhuyenMai,
          hinhAnh: product.hinhAnh,
          soLuongTon: product.soLuongTon
        };
      } catch (error) {
        console.error(`Error fetching details for product ${item.id_SanPham}:`, error);
        return {
          ...item,
          tenSanPham: 'Product not available',
          giaBan: 0,
          hinhAnh: 'assets/img/product/placeholder.webp',
          soLuongTon: 0,
          error: true
        };
      }
    })
  );

  // Render each cart item
  cartItemsWithDetails.forEach(item => {
    const cartItemElement = createCartItemElement(item);
    cartContainer.appendChild(cartItemElement);
  });

  // Update cart summary
  updateCartSummary(cartItemsWithDetails);
}

/**
 * Create a cart item element
 * @param {Object} item - The cart item
 * @returns {HTMLElement} The cart item element
 */
function createCartItemElement(item) {
  const cartItem = document.createElement('div');
  cartItem.className = 'cart-item mb-4 pb-4 border-bottom';
  cartItem.dataset.productId = item.id_SanPham;
  cartItem.dataset.size = item.kichThuoc || 'M';
  cartItem.dataset.color = item.mauSac || 'Black';

  // Calculate item price
  const itemPrice = item.coKhuyenMai ? item.giaKhuyenMai : item.giaBan;
  const itemTotal = itemPrice * item.soLuong;

  // Create cart item HTML
  cartItem.innerHTML = `
    <div class="row align-items-center">
      <div class="col-md-6">
        <div class="d-flex align-items-center">
          <div class="product-image me-3" style="width: 100px; height: 100px;">
            <img src="${item.hinhAnh || 'assets/img/product/placeholder.webp'}" alt="${item.tenSanPham}" class="img-fluid">
          </div>
          <div class="product-details">
            <h5 class="product-title mb-2">${item.tenSanPham}</h5>
            <div class="product-meta mb-2">
              <span class="product-color me-2">Color: ${item.mauSac || 'Black'}</span>
              <span class="product-size">Size: ${item.kichThuoc || 'M'}</span>
            </div>
            <button type="button" class="remove-item btn btn-sm text-danger p-0">
              <i class="bi bi-trash me-1"></i> Remove
            </button>
          </div>
        </div>
      </div>
      <div class="col-md-2 text-center">
        <div class="price-tag">
          ${item.coKhuyenMai ?
            `<div><span class="text-decoration-line-through text-muted">${utils.formatPrice(item.giaBan)}</span></div>
             <div><span class="fw-bold">${utils.formatPrice(item.giaKhuyenMai)}</span></div>` :
            `<div><span class="fw-bold">${utils.formatPrice(item.giaBan)}</span></div>`}
        </div>
      </div>
      <div class="col-md-2 text-center">
        <div class="quantity-selector d-flex justify-content-center">
          <button type="button" class="btn btn-outline-secondary btn-sm decrease">-</button>
          <input type="text" class="form-control form-control-sm text-center mx-2" style="max-width: 50px;" value="${item.soLuong}" min="1" max="${item.soLuongTon}" ${item.error ? 'disabled' : ''}>
          <button type="button" class="btn btn-outline-secondary btn-sm increase" ${item.error ? 'disabled' : ''}>+</button>
        </div>
      </div>
      <div class="col-md-2 text-center">
        <div class="item-total">
          <span class="fw-bold">${utils.formatPrice(itemTotal)}</span>
        </div>
      </div>
    </div>
    ${item.error ? '<div class="alert alert-warning mt-2">This product is no longer available.</div>' : ''}
    ${!item.error && item.soLuong > item.soLuongTon ?
      `<div class="alert alert-warning mt-2">Only ${item.soLuongTon} items available in stock.</div>` : ''}
  `;

  return cartItem;
}

/**
 * Update the cart summary
 * @param {Array} cartItems - The cart items
 */
function updateCartSummary(cartItems) {
  const subtotalElement = document.querySelector('.cart-subtotal .amount');
  const shippingElement = document.querySelector('.cart-shipping .amount');
  const totalElement = document.querySelector('.cart-total .amount');

  if (!subtotalElement || !totalElement) return;

  // Calculate subtotal
  const subtotal = cartItems.reduce((total, item) => {
    const itemPrice = item.coKhuyenMai ? item.giaKhuyenMai : item.giaBan;
    return total + (itemPrice * item.soLuong);
  }, 0);

  // Calculate shipping (free if subtotal > 50, otherwise $5)
  const shipping = subtotal > 50 ? 0 : 5;

  // Calculate total
  const total = subtotal + shipping;

  // Update summary elements
  subtotalElement.textContent = utils.formatPrice(subtotal);

  if (shippingElement) {
    shippingElement.textContent = shipping === 0 ? 'Free' : utils.formatPrice(shipping);
  }

  totalElement.textContent = utils.formatPrice(total);
}

/**
 * Initialize event listeners
 */
function initEventListeners() {
  // Quantity buttons
  document.addEventListener('click', async function(e) {
    // Decrease quantity button
    if (e.target.classList.contains('decrease')) {
      const quantityInput = e.target.nextElementSibling;
      const currentValue = parseInt(quantityInput.value);

      if (currentValue > 1) {
        quantityInput.value = currentValue - 1;
        await updateCartItemQuantity(e.target.closest('.cart-item'));
      }
    }

    // Increase quantity button
    if (e.target.classList.contains('increase')) {
      const quantityInput = e.target.previousElementSibling;
      const currentValue = parseInt(quantityInput.value);
      const maxValue = parseInt(quantityInput.getAttribute('max'));

      if (currentValue < maxValue) {
        quantityInput.value = currentValue + 1;
        await updateCartItemQuantity(e.target.closest('.cart-item'));
      }
    }

    // Remove item button
    if (e.target.classList.contains('remove-item') || e.target.closest('.remove-item')) {
      const cartItem = e.target.closest('.cart-item');
      await removeCartItem(cartItem);
    }
  });

  // Quantity input change
  document.addEventListener('change', async function(e) {
    if (e.target.classList.contains('quantity-input')) {
      await updateCartItemQuantity(e.target.closest('.cart-item'));
    }
  });

  // Clear cart button
  const clearCartButton = document.querySelector('.clear-cart-button');
  if (clearCartButton) {
    clearCartButton.addEventListener('click', async function() {
      try {
        await cartService.clearCart();
        await renderCart([]);
        utils.showNotification('Cart cleared successfully', 'success');
      } catch (error) {
        console.error('Error clearing cart:', error);
        utils.showNotification('Failed to clear cart', 'error');
      }
    });
  }

  // Continue shopping button
  const continueShoppingButton = document.querySelector('.continue-shopping-button');
  if (continueShoppingButton) {
    continueShoppingButton.addEventListener('click', function() {
      window.location.href = 'index.html';
    });
  }

  // Checkout button
  const checkoutButton = document.querySelector('.checkout-button');
  if (checkoutButton) {
    checkoutButton.addEventListener('click', function() {
      window.location.href = 'checkout.html';
    });
  }
}

/**
 * Update cart item quantity
 * @param {HTMLElement} cartItem - The cart item element
 */
async function updateCartItemQuantity(cartItem) {
  if (!cartItem) return;

  const productId = parseInt(cartItem.dataset.productId);
  const quantityInput = cartItem.querySelector('.quantity-input');
  const itemTotalElement = cartItem.querySelector('.item-total');
  const size = cartItem.dataset.size || 'M';
  const color = cartItem.dataset.color || 'Black';

  if (!productId || !quantityInput || !itemTotalElement) return;

  const quantity = parseInt(quantityInput.value);

  // Validate quantity
  if (isNaN(quantity) || quantity < 1) {
    quantityInput.value = 1;
    return;
  }

  try {
    // Update cart item quantity
    await cartService.updateCartItemQuantity(productId, quantity, { size, color });

    // Get updated cart
    const cart = await cartService.getCart();

    // Render updated cart
    await renderCart(cart);

    // Show success message
    utils.showNotification('Cart updated successfully', 'success');
  } catch (error) {
    console.error('Error updating cart item quantity:', error);
    utils.showNotification('Failed to update cart', 'error');
  }
}

/**
 * Remove cart item
 * @param {HTMLElement} cartItem - The cart item element
 */
async function removeCartItem(cartItem) {
  if (!cartItem) return;

  const productId = parseInt(cartItem.dataset.productId);
  const size = cartItem.dataset.size || 'M';
  const color = cartItem.dataset.color || 'Black';

  if (!productId) return;

  try {
    // Remove cart item
    await cartService.removeFromCart(productId, { size, color });

    // Get updated cart
    const cart = await cartService.getCart();

    // Render updated cart
    await renderCart(cart);

    // Show success message
    utils.showNotification('Item removed from cart', 'success');
  } catch (error) {
    console.error('Error removing cart item:', error);
    utils.showNotification('Failed to remove item from cart', 'error');
  }
}
