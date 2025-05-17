/**
 * Cart Service
 * This file contains functions for managing the shopping cart
 */

// Import the base API service
// Note: In a real application, you would use a module bundler like Webpack
// For simplicity, we'll assume the api.js file is loaded before this file in the HTML

// Key for storing the cart in localStorage (for non-authenticated users)
const CART_STORAGE_KEY = 'shopping_cart';

/**
 * Get the current cart from localStorage or API
 * @returns {Promise<Array>} A promise that resolves to the cart items
 */
async function getCart() {
  // If user is logged in, get cart from API
  if (authService.isLoggedIn()) {
    try {
      const response = await api.get('GioHang');

      // Log the response for debugging
      console.log('Cart API response:', response);

      // Handle different response formats
      if (Array.isArray(response)) {
        return response;
      } else if (response && typeof response === 'object') {
        // If response is an object with a data property that's an array
        if (response.data && Array.isArray(response.data)) {
          return response.data;
        }
        // If response has isSuccess and data properties (common API pattern)
        if (response.isSuccess && response.data) {
          return Array.isArray(response.data) ? response.data : [];
        }
      }

      // If we can't determine the format, return an empty array
      console.warn('Unexpected cart response format:', response);
      return [];
    } catch (error) {
      console.error('Error fetching cart from API:', error);
      return [];
    }
  } else {
    // Otherwise, get cart from localStorage
    const cartData = localStorage.getItem(CART_STORAGE_KEY);
    return cartData ? JSON.parse(cartData) : [];
  }
}

/**
 * Add an item to the cart
 * @param {Object} product - The product to add
 * @param {number} quantity - The quantity to add
 * @param {Object} options - Product options (size, color)
 * @returns {Promise<Array>} A promise that resolves to the updated cart
 */
async function addToCart(product, quantity = 1, options = {}) {
  // If user is logged in, add to API cart
  if (authService.isLoggedIn()) {
    try {
      console.log('Adding item to backend cart:', product.id_SanPham, quantity, options);

      const cartItem = {
        id_SanPham: product.id_SanPham,
        soLuong: quantity,
        kichThuoc: options.size || 'M',
        mauSac: options.color || 'Black'
      };

      // Try to use PUT instead of POST if the API doesn't support POST
      try {
        // First try to get the current cart to check if the item exists
        const currentCart = await getCart();
        const existingItem = Array.isArray(currentCart) ?
          currentCart.find(item => 
            item.id_SanPham === product.id_SanPham && 
            item.kichThuoc === cartItem.kichThuoc && 
            item.mauSac === cartItem.mauSac
          ) : null;

        let response;
        if (existingItem) {
          // If item exists, update it with PUT
          const updatedQuantity = existingItem.soLuong + quantity;
          response = await api.put(`GioHang/${product.id_SanPham}`, {
            ...cartItem,
            soLuong: updatedQuantity
          });
          console.log('Updated existing item in cart:', response);
        } else {
          // If item doesn't exist, try POST first
          try {
            response = await api.post('GioHang', cartItem);
            console.log('Added new item to cart with POST:', response);
          } catch (postError) {
            if (postError.status === 405) {
              // If POST is not allowed, try PUT
              response = await api.put(`GioHang/${product.id_SanPham}`, cartItem);
              console.log('Added new item to cart with PUT:', response);
            } else {
              throw postError;
            }
          }
        }
      } catch (apiError) {
        console.error('API error when adding to cart:', apiError);
        // Fall back to localStorage if API fails
        return addToLocalStorageCart(product, quantity, options);
      }

      // Get updated cart from backend
      const updatedCart = await getCart();

      // Dispatch a custom event to notify listeners that the cart has been updated
      window.dispatchEvent(new CustomEvent('cart-updated', { detail: updatedCart }));

      return updatedCart;
    } catch (error) {
      console.error('Error adding item to API cart:', error);
      // Fall back to localStorage if API fails
      return addToLocalStorageCart(product, quantity, options);
    }
  } else {
    // Otherwise, add to localStorage cart
    return addToLocalStorageCart(product, quantity, options);
  }
}

/**
 * Helper function to add an item to the localStorage cart
 * @param {Object} product - The product to add
 * @param {number} quantity - The quantity to add
 * @param {Object} options - Product options (size, color)
 * @returns {Promise<Array>} A promise that resolves to the updated cart
 */
async function addToLocalStorageCart(product, quantity = 1, options = {}) {
  const cart = await getCart();

  // Check if product already exists in cart with same options
  const existingItemIndex = cart.findIndex(item => 
    item.id_SanPham === product.id_SanPham && 
    item.kichThuoc === (options.size || 'M') && 
    item.mauSac === (options.color || 'Black')
  );

  if (existingItemIndex >= 0) {
    // Update quantity if product already in cart
    cart[existingItemIndex].soLuong += quantity;
  } else {
    // Add new item to cart
    cart.push({
      id_SanPham: product.id_SanPham,
      tenSanPham: product.tenSanPham,
      giaBan: product.giaBan,
      giaKhuyenMai: product.giaKhuyenMai,
      coKhuyenMai: product.coKhuyenMai,
      hinhAnh: product.hinhAnh,
      soLuong: quantity,
      kichThuoc: options.size || 'M',
      mauSac: options.color || 'Black'
    });
  }

  // Save updated cart to localStorage
  localStorage.setItem(CART_STORAGE_KEY, JSON.stringify(cart));

  // Dispatch a custom event to notify listeners that the cart has been updated
  window.dispatchEvent(new CustomEvent('cart-updated', { detail: cart }));

  return cart;
}

/**
 * Update the quantity of an item in the cart
 * @param {number} productId - The product ID
 * @param {number} quantity - The new quantity
 * @param {Object} options - Product options (size, color)
 * @returns {Promise<Array>} A promise that resolves to the updated cart
 */
async function updateCartItemQuantity(productId, quantity, options = {}) {
  // If user is logged in, update API cart
  if (authService.isLoggedIn()) {
    try {
      const cartItem = {
        id_SanPham: productId,
        soLuong: quantity,
        kichThuoc: options.size || 'M',
        mauSac: options.color || 'Black'
      };

      await api.put(`GioHang/${productId}`, cartItem);
      return await getCart();
    } catch (error) {
      console.error('Error updating item in API cart:', error);
      throw error;
    }
  } else {
    // Otherwise, update localStorage cart
    const cart = await getCart();

    // Find the item in the cart
    const itemIndex = cart.findIndex(item => 
      item.id_SanPham === productId && 
      item.kichThuoc === (options.size || 'M') && 
      item.mauSac === (options.color || 'Black')
    );

    if (itemIndex >= 0) {
      // Update quantity
      cart[itemIndex].soLuong = quantity;

      // Remove item if quantity is 0
      if (quantity <= 0) {
        cart.splice(itemIndex, 1);
      }

      // Save updated cart to localStorage
      localStorage.setItem(CART_STORAGE_KEY, JSON.stringify(cart));

      // Dispatch a custom event to notify listeners that the cart has been updated
      window.dispatchEvent(new CustomEvent('cart-updated', { detail: cart }));
    }

    return cart;
  }
}

/**
 * Remove an item from the cart
 * @param {number} productId - The product ID
 * @param {Object} options - Product options (size, color)
 * @returns {Promise<Array>} A promise that resolves to the updated cart
 */
async function removeFromCart(productId, options = {}) {
  // If user is logged in, remove from API cart
  if (authService.isLoggedIn()) {
    try {
      const cartItem = {
        id_SanPham: productId,
        kichThuoc: options.size || 'M',
        mauSac: options.color || 'Black'
      };

      await api.delete(`GioHang/${productId}`, { data: cartItem });
      return await getCart();
    } catch (error) {
      console.error('Error removing item from API cart:', error);
      throw error;
    }
  } else {
    // Otherwise, remove from localStorage cart
    const cart = await getCart();

    // Filter out the item to remove
    const updatedCart = cart.filter(item => 
      !(item.id_SanPham === productId && 
        item.kichThuoc === (options.size || 'M') && 
        item.mauSac === (options.color || 'Black'))
    );

    // Save updated cart to localStorage
    localStorage.setItem(CART_STORAGE_KEY, JSON.stringify(updatedCart));

    // Dispatch a custom event to notify listeners that the cart has been updated
    window.dispatchEvent(new CustomEvent('cart-updated', { detail: updatedCart }));

    return updatedCart;
  }
}

/**
 * Clear the cart
 * @returns {Promise<Array>} A promise that resolves to an empty cart
 */
async function clearCart() {
  // If user is logged in, clear API cart
  if (authService.isLoggedIn()) {
    try {
      await api.delete('GioHang/clear');
      return [];
    } catch (error) {
      console.error('Error clearing API cart:', error);
      throw error;
    }
  } else {
    // Otherwise, clear localStorage cart
    localStorage.removeItem(CART_STORAGE_KEY);

    // Dispatch a custom event to notify listeners that the cart has been updated
    window.dispatchEvent(new CustomEvent('cart-updated', { detail: [] }));

    return [];
  }
}

// Export the cart service functions
const cartService = {
  getCart,
  addToCart,
  updateCartItemQuantity,
  removeFromCart,
  clearCart
};

// Make cartService available globally
window.cartService = cartService;
