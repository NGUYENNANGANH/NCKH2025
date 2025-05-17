/**
 * Base API service for handling HTTP requests to the backend
 * This file contains common functionality for all API calls
 */

// Base API URL - change this to match your backend URL
let API_BASE_URL = 'http://localhost:5140/api';

// Default request headers
const DEFAULT_HEADERS = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
};

/**
 * Get the authentication token from local storage
 * @returns {string|null} The authentication token or null if not found
 */
function getAuthToken() {
  return localStorage.getItem('auth_token');
}

/**
 * Add authentication header to request headers if token exists
 * @param {Object} headers - The headers object to add the auth header to
 * @returns {Object} The headers object with auth header if token exists
 */
function addAuthHeader(headers = {}) {
  const token = getAuthToken();
  if (token) {
    return {
      ...headers,
      'Authorization': `Bearer ${token}`
    };
  }
  return headers;
}

/**
 * Handle API response
 * @param {Response} response - The fetch response object
 * @returns {Promise} A promise that resolves to the response data
 * @throws {Error} If the response is not ok
 */
async function handleResponse(response) {
  const contentType = response.headers.get('content-type');
  const isJson = contentType && contentType.includes('application/json');

  let data;
  try {
    data = isJson ? await response.json() : await response.text();
  } catch (e) {
    console.error('Error parsing response:', e);
    throw new Error('Could not parse server response');
  }

  // Trả về dữ liệu ngay cả khi status code là 400 nếu có isSuccess
  if (isJson && typeof data === 'object') {
    // Nếu API trả về isSuccess = false, đây là lỗi nghiệp vụ
    if (data.isSuccess === false && data.message) {
      const error = new Error(data.message);
      error.response = data;
      error.status = response.status;
      throw error;
    }

    // Nếu API trả về isSuccess = true, đây là thành công
    if (data.isSuccess === true) {
      return data;
    }
  }

  // Xử lý các trường hợp lỗi HTTP thông thường
  if (!response.ok) {
    // If the server returned an error with details, use that message
    let errorMessage = 'An error occurred';

    if (isJson && typeof data === 'object') {
      if (data.error) {
        errorMessage = data.error;
      } else if (data.message) {
        errorMessage = data.message;
      } else if (data.errors) {
        // Handle validation errors
        if (typeof data.errors === 'object') {
          const errorMessages = [];
          for (const key in data.errors) {
            if (Array.isArray(data.errors[key])) {
              errorMessages.push(...data.errors[key]);
            } else {
              errorMessages.push(data.errors[key]);
            }
          }
          errorMessage = errorMessages.join(', ');
        } else {
          errorMessage = data.errors;
        }
      } else if (data.title) {
        errorMessage = data.title;
      }
    }

    // Thêm thông tin lỗi vào đối tượng Error
    const error = new Error(errorMessage);
    error.response = data;
    error.status = response.status;
    throw error;
  }

  return data;
}

/**
 * Make a GET request to the API
 * @param {string} endpoint - The API endpoint to call
 * @param {Object} options - Additional fetch options
 * @returns {Promise} A promise that resolves to the response data
 */
async function get(endpoint, options = {}) {
  const url = `${API_BASE_URL}/${endpoint}`;
  const headers = addAuthHeader(DEFAULT_HEADERS);

  try {
    const response = await fetch(url, {
      method: 'GET',
      headers,
      ...options
    });

    return await handleResponse(response);
  } catch (error) {
    console.error(`GET request to ${endpoint} failed:`, error);
    throw error;
  }
}

/**
 * Make a POST request to the API
 * @param {string} endpoint - The API endpoint to call
 * @param {Object} data - The data to send in the request body
 * @param {Object} options - Additional fetch options
 * @returns {Promise} A promise that resolves to the response data
 */
async function post(endpoint, data, options = {}) {
  const url = `${API_BASE_URL}/${endpoint}`;
  const headers = addAuthHeader(DEFAULT_HEADERS);

  try {
    // Log request details for debugging
    console.log(`Sending POST request to ${url} with data:`, data);

    const response = await fetch(url, {
      method: 'POST',
      headers,
      body: JSON.stringify(data),
      ...options
    });

    // Log response status for debugging
    console.log(`Response status: ${response.status}`);

    // Clone response to log its content
    const clonedResponse = response.clone();
    try {
      const responseText = await clonedResponse.text();
      console.log(`Response body: ${responseText}`);
    } catch (e) {
      console.log('Could not log response body:', e);
    }

    return await handleResponse(response);
  } catch (error) {
    console.error(`POST request to ${endpoint} failed:`, error);
    throw error;
  }
}

/**
 * Make a PUT request to the API
 * @param {string} endpoint - The API endpoint to call
 * @param {Object} data - The data to send in the request body
 * @param {Object} options - Additional fetch options
 * @returns {Promise} A promise that resolves to the response data
 */
async function put(endpoint, data, options = {}) {
  const url = `${API_BASE_URL}/${endpoint}`;
  const headers = addAuthHeader(DEFAULT_HEADERS);

  try {
    const response = await fetch(url, {
      method: 'PUT',
      headers,
      body: JSON.stringify(data),
      ...options
    });

    return await handleResponse(response);
  } catch (error) {
    console.error(`PUT request to ${endpoint} failed:`, error);
    throw error;
  }
}

/**
 * Make a DELETE request to the API
 * @param {string} endpoint - The API endpoint to call
 * @param {Object} options - Additional fetch options
 * @returns {Promise} A promise that resolves to the response data
 */
async function del(endpoint, options = {}) {
  const url = `${API_BASE_URL}/${endpoint}`;
  const headers = addAuthHeader(DEFAULT_HEADERS);

  try {
    const response = await fetch(url, {
      method: 'DELETE',
      headers,
      ...options
    });

    return await handleResponse(response);
  } catch (error) {
    console.error(`DELETE request to ${endpoint} failed:`, error);
    throw error;
  }
}

// Export the API functions
const api = {
  get,
  post,
  put,
  delete: del,
  getAuthToken,
  baseUrl: API_BASE_URL,
  setBaseUrl: (url) => {
    API_BASE_URL = url;
  }
};

// Make api available globally
window.api = api;
