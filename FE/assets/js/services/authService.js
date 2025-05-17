/**
 * Authentication API Service
 * This file contains functions for user authentication
 */

// Import the base API service
// Note: In a real application, you would use a module bundler like Webpack
// For simplicity, we'll assume the api.js file is loaded before this file in the HTML

// Key for storing the auth token in localStorage
const AUTH_TOKEN_KEY = 'auth_token';
// Key for storing the user data in localStorage
const USER_DATA_KEY = 'user_data';

/**
 * Login a user
 * @param {string} email - The user's email
 * @param {string} password - The user's password
 * @returns {Promise<Object>} A promise that resolves to the user data and token
 */
async function login(email, password) {
  try {
    console.log('Attempting login with:', { Email: email, Password: password });

    // Gửi request đăng nhập với đúng format API yêu cầu
    const response = await api.post('Auth/login', {
      Email: email,
      Password: password  // Đảm bảo tên trường là "Password" với chữ P viết hoa
    });

    console.log('Login response:', response);

    // Store the token and user data in localStorage
    if (response.token) {
      localStorage.setItem(AUTH_TOKEN_KEY, response.token);

      // Create user object from response
      const userData = {
        userId: response.userId,
        userName: response.userName || email,
        email: email,
        roles: response.roles || []
      };

      localStorage.setItem(USER_DATA_KEY, JSON.stringify(userData));
    }

    return response;
  } catch (error) {
    console.error('Login failed:', error);
    throw error;
  }
}

/**
 * Register a new user
 * @param {Object} userData - The user registration data
 * @returns {Promise<Object>} A promise that resolves to the registration response
 */
async function register(userData) {
  try {
    console.log('Attempting registration with:', userData);

    // Đảm bảo dữ liệu đăng ký có đúng format API yêu cầu
    const registerData = {
      Email: userData.Email || userData.email,
      Password: userData.Password || userData.password,
      ConfirmPassword: userData.ConfirmPassword || userData.confirmPassword,
      FullName: userData.FullName || userData.fullName
    };

    console.log('Formatted registration data:', registerData);

    const response = await api.post('Auth/register', registerData);
    console.log('Registration response:', response);

    return response;
  } catch (error) {
    console.error('Registration failed:', error);
    throw error;
  }
}

/**
 * Logout the current user
 */
function logout() {
  localStorage.removeItem(AUTH_TOKEN_KEY);
  localStorage.removeItem(USER_DATA_KEY);

  // Redirect to home page or login page
  window.location.href = '/index.html';
}

/**
 * Check if a user is logged in
 * @returns {boolean} True if the user is logged in, false otherwise
 */
function isLoggedIn() {
  return !!localStorage.getItem(AUTH_TOKEN_KEY);
}

/**
 * Get the current user data
 * @returns {Object|null} The user data or null if not logged in
 */
function getCurrentUser() {
  const userData = localStorage.getItem(USER_DATA_KEY);
  return userData ? JSON.parse(userData) : null;
}

/**
 * Check if the current user has a specific role
 * @param {string|Array} roles - The role(s) to check
 * @returns {boolean} True if the user has the role, false otherwise
 */
function hasRole(roles) {
  const user = getCurrentUser();
  if (!user || !user.roles) return false;

  if (Array.isArray(roles)) {
    return roles.some(role => user.roles.includes(role));
  }

  return user.roles.includes(roles);
}

// Export the auth service functions
const authService = {
  login,
  register,
  logout,
  isLoggedIn,
  getCurrentUser,
  hasRole
};

// Make authService available globally
window.authService = authService;
