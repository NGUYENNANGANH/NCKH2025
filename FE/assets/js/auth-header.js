/**
 * Authentication Header JavaScript
 * This file contains the logic for updating the header based on authentication state
 */

// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function() {
  // Initialize the authentication header
  initAuthHeader();
});

/**
 * Initialize the authentication header
 */
function initAuthHeader() {
  // Check if authService is available
  if (typeof authService === 'undefined') {
    console.error('Auth service not available');
    return;
  }

  // Update header based on authentication state
  updateHeaderAuthState();
}

/**
 * Update header based on authentication state
 */
function updateHeaderAuthState() {
  const isLoggedIn = authService.isLoggedIn();
  const currentUser = authService.getCurrentUser();

  // Get header elements
  const accountDropdown = document.querySelector('.account-dropdown .dropdown-menu');
  const accountButton = document.querySelector('.account-dropdown .header-action-btn');

  if (!accountDropdown) return;

  if (isLoggedIn && currentUser) {
    // User is logged in, update dropdown content
    updateLoggedInDropdown(accountDropdown, currentUser);

    // Add logged-in class to account button
    if (accountButton) {
      accountButton.classList.add('logged-in');

      // Add user initials to button if available
      const userName = currentUser.fullName || currentUser.userName || '';
      if (userName) {
        const initials = getInitials(userName);
        accountButton.innerHTML = `<span class="user-initials">${initials}</span>`;
      }
    }
  } else {
    // User is not logged in, ensure dropdown shows login/register options
    updateLoggedOutDropdown(accountDropdown);

    // Remove logged-in class from account button
    if (accountButton) {
      accountButton.classList.remove('logged-in');
      accountButton.innerHTML = '<i class="bi bi-person"></i>';
    }
  }
}

/**
 * Update dropdown for logged in users
 * @param {HTMLElement} dropdown - The dropdown element
 * @param {Object} user - The user data
 */
function updateLoggedInDropdown(dropdown, user) {
  // Create dropdown header
  const headerContent = `
    <div class="dropdown-header">
      <h6>Welcome, ${user.fullName || user.userName || 'User'}</h6>
      <p class="mb-0">${user.email || user.userName}</p>
    </div>
  `;

  // Create dropdown body
  const bodyContent = `
    <div class="dropdown-body">
      <a class="dropdown-item d-flex align-items-center" href="account.html">
        <i class="bi bi-person-circle me-2"></i>
        <span>My Profile</span>
      </a>
      <a class="dropdown-item d-flex align-items-center" href="account.html?tab=orders">
        <i class="bi bi-bag-check me-2"></i>
        <span>My Orders</span>
      </a>
      <a class="dropdown-item d-flex align-items-center" href="account.html?tab=wishlist">
        <i class="bi bi-heart me-2"></i>
        <span>My Wishlist</span>
      </a>
      <a class="dropdown-item d-flex align-items-center" href="account.html?tab=settings">
        <i class="bi bi-gear me-2"></i>
        <span>Settings</span>
      </a>
    </div>
  `;

  // Create dropdown footer
  const footerContent = `
    <div class="dropdown-footer">
      <button onclick="handleLogout()" class="btn btn-outline-danger w-100">
        <i class="bi bi-box-arrow-right me-2"></i>Logout
      </button>
    </div>
  `;

  // Update dropdown content
  dropdown.innerHTML = headerContent + bodyContent + footerContent;
}

/**
 * Update dropdown for logged out users
 * @param {HTMLElement} dropdown - The dropdown element
 */
function updateLoggedOutDropdown(dropdown) {
  // Create dropdown header
  const headerContent = `
    <div class="dropdown-header">
      <h6>Welcome to <span class="sitename">eStore</span></h6>
      <p class="mb-0">Access account &amp; manage orders</p>
    </div>
  `;

  // Create dropdown body (empty for logged out users)
  const bodyContent = `
    <div class="dropdown-body">
      <div class="text-center py-3">
        <p class="mb-0">Please sign in to access your account</p>
      </div>
    </div>
  `;

  // Create dropdown footer
  const footerContent = `
    <div class="dropdown-footer">
      <a href="login-register.html" class="btn btn-primary w-100 mb-2">Sign In</a>
      <a href="login-register.html?tab=register" class="btn btn-outline-primary w-100">Register</a>
    </div>
  `;

  // Update dropdown content
  dropdown.innerHTML = headerContent + bodyContent + footerContent;
}

/**
 * Get initials from a name
 * @param {string} name - The full name
 * @returns {string} The initials
 */
function getInitials(name) {
  if (!name) return 'U';

  const nameParts = name.split(' ').filter(part => part.length > 0);
  if (nameParts.length === 0) return 'U';

  if (nameParts.length === 1) {
    return nameParts[0].charAt(0).toUpperCase();
  }

  return (nameParts[0].charAt(0) + nameParts[nameParts.length - 1].charAt(0)).toUpperCase();
}

/**
 * Handle logout button click
 */
function handleLogout() {
  // Call logout function from authService
  authService.logout();

  // Redirect to home page
  window.location.href = 'index.html';
}

// Add CSS for user initials in account button
const style = document.createElement('style');
style.textContent = `
  .header-action-btn.logged-in {
    background-color: var(--accent-color);
    color: white;
    width: 32px;
    height: 32px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 14px;
    font-weight: 500;
  }

  .user-initials {
    line-height: 1;
  }
`;
document.head.appendChild(style);
