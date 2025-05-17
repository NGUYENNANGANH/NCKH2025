/**
 * User API Service
 * This file contains functions for user-related operations
 */

/**
 * Get the current user's profile
 * @returns {Promise<Object>} A promise that resolves to the user profile data
 */
async function getUserProfile() {
  try {
    return await api.get('User/profile');
  } catch (error) {
    console.error('Failed to get user profile:', error);
    throw error;
  }
}

/**
 * Update the user's profile
 * @param {Object} userData - The user data to update
 * @returns {Promise<Object>} A promise that resolves to the updated user data
 */
async function updateUserProfile(userData) {
  try {
    return await api.put('User/update-profile', userData);
  } catch (error) {
    console.error('Failed to update user profile:', error);
    throw error;
  }
}

/**
 * Change the user's password
 * @param {Object} passwordData - The password data (currentPassword, newPassword, confirmPassword)
 * @returns {Promise<Object>} A promise that resolves to the response data
 */
async function changePassword(passwordData) {
  try {
    return await api.post('User/change-password', passwordData);
  } catch (error) {
    console.error('Failed to change password:', error);
    throw error;
  }
}

/**
 * Get the user's orders
 * @returns {Promise<Array>} A promise that resolves to the user's orders
 */
async function getUserOrders() {
  try {
    return await api.get('Order/user-orders');
  } catch (error) {
    console.error('Failed to get user orders:', error);
    throw error;
  }
}

/**
 * Get the user's wishlist
 * @returns {Promise<Array>} A promise that resolves to the user's wishlist
 */
async function getUserWishlist() {
  try {
    return await api.get('Wishlist');
  } catch (error) {
    console.error('Failed to get user wishlist:', error);
    throw error;
  }
}

/**
 * Add a product to the user's wishlist
 * @param {number} productId - The ID of the product to add
 * @returns {Promise<Object>} A promise that resolves to the response data
 */
async function addToWishlist(productId) {
  try {
    return await api.post('Wishlist/add', { productId });
  } catch (error) {
    console.error('Failed to add to wishlist:', error);
    throw error;
  }
}

/**
 * Remove a product from the user's wishlist
 * @param {number} productId - The ID of the product to remove
 * @returns {Promise<Object>} A promise that resolves to the response data
 */
async function removeFromWishlist(productId) {
  try {
    return await api.delete(`Wishlist/remove/${productId}`);
  } catch (error) {
    console.error('Failed to remove from wishlist:', error);
    throw error;
  }
}

/**
 * Get the user's addresses
 * @returns {Promise<Array>} A promise that resolves to the user's addresses
 */
async function getUserAddresses() {
  try {
    return await api.get('User/addresses');
  } catch (error) {
    console.error('Failed to get user addresses:', error);
    throw error;
  }
}

/**
 * Add a new address for the user
 * @param {Object} addressData - The address data
 * @returns {Promise<Object>} A promise that resolves to the response data
 */
async function addUserAddress(addressData) {
  try {
    return await api.post('User/add-address', addressData);
  } catch (error) {
    console.error('Failed to add user address:', error);
    throw error;
  }
}

/**
 * Update an existing address
 * @param {number} addressId - The ID of the address to update
 * @param {Object} addressData - The updated address data
 * @returns {Promise<Object>} A promise that resolves to the response data
 */
async function updateUserAddress(addressId, addressData) {
  try {
    return await api.put(`User/update-address/${addressId}`, addressData);
  } catch (error) {
    console.error('Failed to update user address:', error);
    throw error;
  }
}

/**
 * Delete an address
 * @param {number} addressId - The ID of the address to delete
 * @returns {Promise<Object>} A promise that resolves to the response data
 */
async function deleteUserAddress(addressId) {
  try {
    return await api.delete(`User/delete-address/${addressId}`);
  } catch (error) {
    console.error('Failed to delete user address:', error);
    throw error;
  }
}

/**
 * Update notification settings
 * @param {Object} settings - The notification settings
 * @returns {Promise<Object>} A promise that resolves to the response data
 */
async function updateNotificationSettings(settings) {
  try {
    return await api.put('User/notification-settings', settings);
  } catch (error) {
    console.error('Failed to update notification settings:', error);
    throw error;
  }
}

// Export the user service functions
const userService = {
  getUserProfile,
  updateUserProfile,
  changePassword,
  getUserOrders,
  getUserWishlist,
  addToWishlist,
  removeFromWishlist,
  getUserAddresses,
  addUserAddress,
  updateUserAddress,
  deleteUserAddress,
  updateNotificationSettings
};

// Make userService available globally
window.userService = userService;
