/**
 * SanPham (Product) API Service
 * This file contains functions for interacting with the SanPham API endpoints
 */

// Import the base API service
// Note: In a real application, you would use a module bundler like Webpack
// For simplicity, we'll assume the api.js file is loaded before this file in the HTML

/**
 * Get all products
 * @returns {Promise<Array>} A promise that resolves to an array of products
 */
async function getAllSanPhams() {
  try {
    return await api.get('SanPham');
  } catch (error) {
    console.error('Error fetching all products:', error);
    throw error;
  }
}

/**
 * Get a product by ID
 * @param {number} id - The product ID
 * @returns {Promise<Object>} A promise that resolves to the product object
 */
async function getSanPhamById(id) {
  try {
    return await api.get(`SanPham/${id}`);
  } catch (error) {
    console.error(`Error fetching product with ID ${id}:`, error);
    throw error;
  }
}

/**
 * Get products by category ID
 * @param {number} danhMucId - The category ID
 * @returns {Promise<Array>} A promise that resolves to an array of products in the category
 */
async function getSanPhamsByDanhMuc(danhMucId) {
  try {
    return await api.get(`SanPham/DanhMuc/${danhMucId}`);
  } catch (error) {
    console.error(`Error fetching products in category ${danhMucId}:`, error);
    throw error;
  }
}

/**
 * Search for products by keyword
 * @param {string} keyword - The search keyword
 * @returns {Promise<Array>} A promise that resolves to an array of matching products
 */
async function searchSanPhams(keyword) {
  try {
    console.log(`Searching for products with keyword: ${keyword}`);
    const result = await api.get(`SanPham/search?keyword=${encodeURIComponent(keyword)}`);
    console.log('Search results:', result);
    return result;
  } catch (error) {
    console.error(`Error searching for products with keyword "${keyword}":`, error);
    throw error;
  }
}

/**
 * Create a new product (requires authentication)
 * @param {Object} sanPhamData - The product data
 * @returns {Promise<Object>} A promise that resolves to the created product
 */
async function createSanPham(sanPhamData) {
  try {
    return await api.post('SanPham', sanPhamData);
  } catch (error) {
    console.error('Error creating product:', error);
    throw error;
  }
}

/**
 * Update a product (requires authentication)
 * @param {number} id - The product ID
 * @param {Object} sanPhamData - The updated product data
 * @returns {Promise<Object>} A promise that resolves to the updated product
 */
async function updateSanPham(id, sanPhamData) {
  try {
    return await api.put(`SanPham/${id}`, sanPhamData);
  } catch (error) {
    console.error(`Error updating product with ID ${id}:`, error);
    throw error;
  }
}

/**
 * Delete a product (requires authentication)
 * @param {number} id - The product ID
 * @returns {Promise<Object>} A promise that resolves to the response data
 */
async function deleteSanPham(id) {
  try {
    return await api.delete(`SanPham/${id}`);
  } catch (error) {
    console.error(`Error deleting product with ID ${id}:`, error);
    throw error;
  }
}

// Export the SanPham service functions
const sanPhamService = {
  getAllSanPhams,
  getSanPhamById,
  getSanPhamsByDanhMuc,
  searchSanPhams,
  createSanPham,
  updateSanPham,
  deleteSanPham
};

// Make sanPhamService available globally
window.sanPhamService = sanPhamService;
