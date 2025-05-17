/**
 * Utility functions for the frontend
 */

/**
 * Format a price with currency symbol
 * @param {number} price - The price to format
 * @param {string} currency - The currency symbol (default: $)
 * @returns {string} The formatted price
 */
function formatPrice(price, currency = '$') {
  return `${currency}${parseFloat(price).toFixed(2)}`;
}

/**
 * Format a date to a readable string
 * @param {string|Date} date - The date to format
 * @param {Object} options - The Intl.DateTimeFormat options
 * @returns {string} The formatted date
 */
function formatDate(date, options = {}) {
  const defaultOptions = {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  };

  const dateObj = typeof date === 'string' ? new Date(date) : date;
  return new Intl.DateTimeFormat('en-US', { ...defaultOptions, ...options }).format(dateObj);
}

/**
 * Truncate a string to a specified length
 * @param {string} str - The string to truncate
 * @param {number} length - The maximum length
 * @param {string} suffix - The suffix to add if truncated (default: ...)
 * @returns {string} The truncated string
 */
function truncateString(str, length, suffix = '...') {
  if (!str) return '';
  if (str.length <= length) return str;
  return str.substring(0, length).trim() + suffix;
}

/**
 * Show a notification message
 * @param {string} message - The message to show
 * @param {string} type - The message type (success, error, warning, info)
 * @param {number} duration - The duration in milliseconds
 */
function showNotification(message, type = 'info', duration = 3000) {
  // Check if notification container exists, create if not
  let container = document.getElementById('notification-container');
  if (!container) {
    container = document.createElement('div');
    container.id = 'notification-container';
    container.style.position = 'fixed';
    container.style.top = '20px';
    container.style.right = '20px';
    container.style.zIndex = '9999';
    document.body.appendChild(container);
  }

  // Create notification element
  const notification = document.createElement('div');
  notification.className = `notification notification-${type}`;
  notification.innerHTML = `
    <div class="notification-content">
      <i class="bi ${getIconForType(type)}"></i>
      <span>${message}</span>
    </div>
    <button class="notification-close">×</button>
  `;

  // Style the notification
  notification.style.backgroundColor = getColorForType(type);
  notification.style.color = '#fff';
  notification.style.padding = '12px 20px';
  notification.style.borderRadius = '4px';
  notification.style.marginBottom = '10px';
  notification.style.boxShadow = '0 2px 10px rgba(0,0,0,0.1)';
  notification.style.display = 'flex';
  notification.style.justifyContent = 'space-between';
  notification.style.alignItems = 'center';
  notification.style.opacity = '0';
  notification.style.transform = 'translateX(40px)';
  notification.style.transition = 'opacity 0.3s, transform 0.3s';

  // Add close button functionality
  const closeButton = notification.querySelector('.notification-close');
  closeButton.style.background = 'none';
  closeButton.style.border = 'none';
  closeButton.style.color = '#fff';
  closeButton.style.fontSize = '20px';
  closeButton.style.cursor = 'pointer';
  closeButton.style.marginLeft = '10px';

  closeButton.addEventListener('click', () => {
    closeNotification(notification);
  });

  // Add to container
  container.appendChild(notification);

  // Trigger animation
  setTimeout(() => {
    notification.style.opacity = '1';
    notification.style.transform = 'translateX(0)';
  }, 10);

  // Auto-close after duration
  setTimeout(() => {
    closeNotification(notification);
  }, duration);

  // Helper function to close notification with animation
  function closeNotification(element) {
    element.style.opacity = '0';
    element.style.transform = 'translateX(40px)';

    setTimeout(() => {
      element.remove();
    }, 300);
  }

  // Helper function to get icon for notification type
  function getIconForType(type) {
    switch (type) {
      case 'success': return 'bi-check-circle-fill';
      case 'error': return 'bi-x-circle-fill';
      case 'warning': return 'bi-exclamation-triangle-fill';
      case 'info': return 'bi-info-circle-fill';
      default: return 'bi-info-circle-fill';
    }
  }

  // Helper function to get color for notification type
  function getColorForType(type) {
    switch (type) {
      case 'success': return '#28a745';
      case 'error': return '#dc3545';
      case 'warning': return '#ffc107';
      case 'info': return '#17a2b8';
      default: return '#17a2b8';
    }
  }
}

/**
 * Create a loading spinner element
 * @param {string} size - The size of the spinner (sm, md, lg)
 * @param {string} color - The color of the spinner
 * @returns {HTMLElement} The spinner element
 */
function createLoadingSpinner(size = 'md', color = 'primary') {
  const spinner = document.createElement('div');
  spinner.className = `spinner-border text-${color} spinner-border-${size}`;
  spinner.setAttribute('role', 'status');

  const span = document.createElement('span');
  span.className = 'visually-hidden';
  span.textContent = 'Loading...';

  spinner.appendChild(span);
  return spinner;
}

/**
 * Show a loading overlay on an element
 * @param {HTMLElement} element - The element to show the overlay on
 * @param {string} message - The loading message
 * @returns {HTMLElement} The overlay element
 */
function showLoadingOverlay(element, message = 'Loading...') {
  // Create overlay
  const overlay = document.createElement('div');
  overlay.className = 'loading-overlay';
  overlay.style.position = 'absolute';
  overlay.style.top = '0';
  overlay.style.left = '0';
  overlay.style.width = '100%';
  overlay.style.height = '100%';
  overlay.style.backgroundColor = 'rgba(255, 255, 255, 0.8)';
  overlay.style.display = 'flex';
  overlay.style.flexDirection = 'column';
  overlay.style.justifyContent = 'center';
  overlay.style.alignItems = 'center';
  overlay.style.zIndex = '1000';

  // Create spinner
  const spinner = createLoadingSpinner('lg');

  // Create message
  const messageElement = document.createElement('div');
  messageElement.textContent = message;
  messageElement.style.marginTop = '10px';

  // Add to overlay
  overlay.appendChild(spinner);
  overlay.appendChild(messageElement);

  // Make sure the element has position relative for absolute positioning of overlay
  const elementPosition = window.getComputedStyle(element).position;
  if (elementPosition === 'static') {
    element.style.position = 'relative';
  }

  // Add overlay to element
  element.appendChild(overlay);

  return overlay;
}

/**
 * Hide a loading overlay
 * @param {HTMLElement} overlay - The overlay element to hide
 */
function hideLoadingOverlay(overlay) {
  if (overlay && overlay.parentNode) {
    overlay.parentNode.removeChild(overlay);
  }
}

// Export the utility functions
const utils = {
  formatPrice,
  formatDate,
  truncateString,
  showNotification,
  createLoadingSpinner,
  showLoadingOverlay,
  hideLoadingOverlay
};

// Make utils available globally
window.utils = utils;
