/**
 * Main application JavaScript file
 * This file contains the main logic for the frontend application
 */

// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function() {
  // Initialize the application
  initApp();
});

/**
 * Initialize the application
 */
function initApp() {
  // Update UI based on authentication status
  updateAuthUI();

  // Initialize product listings
  initProductListings();

  // Initialize search functionality
  initSearch();

  // Initialize cart functionality
  initCart();

  // Initialize event listeners
  initEventListeners();
}

/**
 * Update UI elements based on authentication status
 */
function updateAuthUI() {
  const isLoggedIn = authService.isLoggedIn();
  const currentUser = authService.getCurrentUser();

  // Get auth-related elements
  const authLinks = document.querySelectorAll('.auth-links');
  const userProfileElements = document.querySelectorAll('.user-profile');
  const adminLinks = document.querySelectorAll('.admin-link');

  if (isLoggedIn && currentUser) {
    // User is logged in
    authLinks.forEach(el => {
      const signInLink = el.querySelector('.sign-in-link');
      const registerLink = el.querySelector('.register-link');
      const userNameElement = el.querySelector('.user-name');
      const logoutLink = el.querySelector('.logout-link');

      if (signInLink) signInLink.style.display = 'none';
      if (registerLink) registerLink.style.display = 'none';
      if (userNameElement) {
        userNameElement.textContent = currentUser.name || currentUser.email;
        userNameElement.style.display = 'inline-block';
      }
      if (logoutLink) {
        logoutLink.style.display = 'inline-block';
        logoutLink.addEventListener('click', function(e) {
          e.preventDefault();
          authService.logout();
        });
      }
    });

    // Update user profile elements
    userProfileElements.forEach(el => {
      el.style.display = 'block';
    });

    // Show/hide admin links based on user role
    const isAdmin = authService.hasRole(['Admin', 'Manager']);
    adminLinks.forEach(el => {
      el.style.display = isAdmin ? 'block' : 'none';
    });
  } else {
    // User is not logged in
    authLinks.forEach(el => {
      const signInLink = el.querySelector('.sign-in-link');
      const registerLink = el.querySelector('.register-link');
      const userNameElement = el.querySelector('.user-name');
      const logoutLink = el.querySelector('.logout-link');

      if (signInLink) signInLink.style.display = 'inline-block';
      if (registerLink) registerLink.style.display = 'inline-block';
      if (userNameElement) userNameElement.style.display = 'none';
      if (logoutLink) logoutLink.style.display = 'none';
    });

    // Hide user profile elements
    userProfileElements.forEach(el => {
      el.style.display = 'none';
    });

    // Hide admin links
    adminLinks.forEach(el => {
      el.style.display = 'none';
    });
  }
}

/**
 * Initialize product listings
 */
async function initProductListings() {
  // Get product container elements
  const featuredProductsContainer = document.getElementById('featured-products');
  const newArrivalsContainer = document.getElementById('new-arrivals');
  const saleProductsContainer = document.getElementById('sale-products');

  if (featuredProductsContainer) {
    try {
      // Show loading state
      const loadingOverlay = utils.showLoadingOverlay(featuredProductsContainer, 'Loading featured products...');

      // Fetch products
      const products = await sanPhamService.getAllSanPhams();

      // Sort by number of sales to get featured products
      const featuredProducts = products
        .sort((a, b) => b.soLuongDaBan - a.soLuongDaBan)
        .slice(0, 8); // Get top 8 products

      // Render products
      renderProducts(featuredProductsContainer, featuredProducts);

      // Hide loading state
      utils.hideLoadingOverlay(loadingOverlay);
    } catch (error) {
      console.error('Error loading featured products:', error);
      utils.showNotification('Failed to load featured products', 'error');
    }
  }

  if (newArrivalsContainer) {
    try {
      // Show loading state
      const loadingOverlay = utils.showLoadingOverlay(newArrivalsContainer, 'Loading new arrivals...');

      // Fetch products
      const products = await sanPhamService.getAllSanPhams();

      // Sort by creation date to get new arrivals
      const newArrivals = products
        .sort((a, b) => new Date(b.ngayTao) - new Date(a.ngayTao))
        .slice(0, 8); // Get top 8 products

      // Render products
      renderProducts(newArrivalsContainer, newArrivals);

      // Hide loading state
      utils.hideLoadingOverlay(loadingOverlay);
    } catch (error) {
      console.error('Error loading new arrivals:', error);
      utils.showNotification('Failed to load new arrivals', 'error');
    }
  }

  if (saleProductsContainer) {
    try {
      // Show loading state
      const loadingOverlay = utils.showLoadingOverlay(saleProductsContainer, 'Loading sale products...');

      // Fetch products
      const products = await sanPhamService.getAllSanPhams();

      // Filter products with discount
      const saleProducts = products
        .filter(product => product.coKhuyenMai)
        .slice(0, 8); // Get top 8 products

      // Render products
      renderProducts(saleProductsContainer, saleProducts);

      // Hide loading state
      utils.hideLoadingOverlay(loadingOverlay);
    } catch (error) {
      console.error('Error loading sale products:', error);
      utils.showNotification('Failed to load sale products', 'error');
    }
  }
}

/**
 * Render products in a container
 * @param {HTMLElement} container - The container element
 * @param {Array} products - The products to render
 */
function renderProducts(container, products) {
  // Clear container
  container.innerHTML = '';

  // Create product cards
  products.forEach(product => {
    const productCard = createProductCard(product);
    container.appendChild(productCard);
  });
}

/**
 * Create a product card element
 * @param {Object} product - The product data
 * @returns {HTMLElement} The product card element
 */
function createProductCard(product) {
  const card = document.createElement('div');
  card.className = 'product-card';

  // Create product image
  const imageContainer = document.createElement('div');
  imageContainer.className = 'product-image';

  const image = document.createElement('img');
  image.src = product.hinhAnh || 'assets/img/product/placeholder.webp';
  image.alt = product.tenSanPham;
  image.loading = 'lazy';

  // Add sale badge if product is on sale
  if (product.coKhuyenMai) {
    const saleBadge = document.createElement('span');
    saleBadge.className = 'badge-sale';
    saleBadge.textContent = `-${product.phanTramGiam}%`;
    imageContainer.appendChild(saleBadge);
  }

  imageContainer.appendChild(image);

  // Create product info
  const infoContainer = document.createElement('div');
  infoContainer.className = 'product-info';

  const title = document.createElement('h5');
  title.textContent = product.tenSanPham;

  const price = document.createElement('p');
  price.className = 'price';

  if (product.coKhuyenMai) {
    const originalPrice = document.createElement('span');
    originalPrice.className = 'original-price';
    originalPrice.textContent = utils.formatPrice(product.giaBan);

    price.appendChild(originalPrice);
    price.appendChild(document.createTextNode(' ' + utils.formatPrice(product.giaKhuyenMai)));
  } else {
    price.textContent = utils.formatPrice(product.giaBan);
  }

  const viewButton = document.createElement('a');
  viewButton.href = `product-details.html?id=${product.id_SanPham}`;
  viewButton.className = 'btn-view';
  viewButton.textContent = 'View Product';

  infoContainer.appendChild(title);
  infoContainer.appendChild(price);
  infoContainer.appendChild(viewButton);

  // Add quick add to cart button
  const addToCartButton = document.createElement('button');
  addToCartButton.className = 'btn-add-to-cart';
  addToCartButton.innerHTML = '<i class="bi bi-cart-plus"></i>';
  addToCartButton.title = 'Add to Cart';
  addToCartButton.addEventListener('click', async function(e) {
    e.preventDefault();
    e.stopPropagation();

    try {
      await cartService.addToCart(product, 1);
      utils.showNotification(`Added ${product.tenSanPham} to cart`, 'success');
    } catch (error) {
      console.error('Error adding product to cart:', error);
      utils.showNotification('Failed to add product to cart', 'error');
    }
  });

  infoContainer.appendChild(addToCartButton);

  // Assemble card
  card.appendChild(imageContainer);
  card.appendChild(infoContainer);

  return card;
}

/**
 * Initialize search functionality
 */
function initSearch() {
  const searchForms = document.querySelectorAll('.search-form');

  searchForms.forEach(form => {
    form.addEventListener('submit', function(e) {
      e.preventDefault();

      const input = form.querySelector('input[type="text"]');
      if (!input || !input.value.trim()) return;

      const keyword = input.value.trim();
      window.location.href = `search-results.html?keyword=${encodeURIComponent(keyword)}`;
    });
  });
}

/**
 * Initialize cart functionality
 */
async function initCart() {
  // Update cart badge count
  updateCartBadge();

  // Listen for cart updates
  window.addEventListener('cart-updated', function() {
    updateCartBadge();
  });
}

/**
 * Update the cart badge count
 */
async function updateCartBadge() {
  const cartBadges = document.querySelectorAll('.header-action-btn .badge');

  try {
    const cart = await cartService.getCart();

    // Ensure cart is an array before using reduce
    let itemCount = 0;
    if (Array.isArray(cart)) {
      itemCount = cart.reduce((total, item) => total + (item.soLuong || 0), 0);
    } else if (cart && typeof cart === 'object' && cart.data && Array.isArray(cart.data)) {
      // Handle case where API returns { data: [...] } format
      itemCount = cart.data.reduce((total, item) => total + (item.soLuong || 0), 0);
    } else {
      console.warn('Cart is not in expected format:', cart);
    }

    cartBadges.forEach(badge => {
      badge.textContent = itemCount.toString();

      // Show/hide badge based on item count
      if (itemCount > 0) {
        badge.style.display = 'inline-block';
      } else {
        badge.style.display = 'none';
      }
    });
  } catch (error) {
    console.error('Error updating cart badge:', error);
  }
}

// Make updateCartBadge available globally
window.updateCartBadge = updateCartBadge;

/**
 * Initialize event listeners
 */
function initEventListeners() {
  // Add event listeners for login/register forms
  const loginForm = document.getElementById('login-form');
  const registerForm = document.getElementById('register-form');

  if (loginForm) {
    loginForm.addEventListener('submit', handleLogin);
  }

  if (registerForm) {
    registerForm.addEventListener('submit', handleRegister);
  }
}

/**
 * Handle login form submission
 * @param {Event} e - The form submit event
 */
async function handleLogin(e) {
  e.preventDefault();

  const emailInput = document.getElementById('login-email');
  const passwordInput = document.getElementById('login-password');
  const submitButton = document.querySelector('#login-form button[type="submit"]');

  if (!emailInput || !passwordInput) return;

  const email = emailInput.value.trim();
  const password = passwordInput.value;

  if (!email || !password) {
    utils.showNotification('Please enter both email and password', 'warning');
    return;
  }

  try {
    // Disable submit button and show loading state
    submitButton.disabled = true;
    submitButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Logging in...';

    // Attempt login
    const response = await authService.login(email, password);

    // Show success message
    utils.showNotification('Login successful!', 'success');

    // Update UI
    updateAuthUI();

    // Redirect to home page or previous page
    setTimeout(() => {
      window.location.href = 'index.html';
    }, 1000);
  } catch (error) {
    console.error('Login error:', error);
    utils.showNotification(error.message || 'Login failed. Please check your credentials.', 'error');

    // Reset submit button
    submitButton.disabled = false;
    submitButton.textContent = 'Sign In';
  }
}

/**
 * Handle register form submission
 * @param {Event} e - The form submit event
 */
async function handleRegister(e) {
  e.preventDefault();

  const nameInput = document.getElementById('register-name');
  const emailInput = document.getElementById('register-email');
  const passwordInput = document.getElementById('register-password');
  const confirmPasswordInput = document.getElementById('register-confirm-password');
  const submitButton = document.querySelector('#register-form button[type="submit"]');

  if (!nameInput || !emailInput || !passwordInput || !confirmPasswordInput) return;

  const name = nameInput.value.trim();
  const email = emailInput.value.trim();
  const password = passwordInput.value;
  const confirmPassword = confirmPasswordInput.value;

  // Validate inputs
  if (!name || !email || !password || !confirmPassword) {
    utils.showNotification('Please fill in all fields', 'warning');
    return;
  }

  if (password !== confirmPassword) {
    utils.showNotification('Passwords do not match', 'warning');
    return;
  }

  try {
    // Disable submit button and show loading state
    submitButton.disabled = true;
    submitButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Registering...';

    // Attempt registration
    const userData = {
      name,
      email,
      password
    };

    const response = await authService.register(userData);

    // Show success message
    utils.showNotification('Registration successful! Please log in.', 'success');

    // Reset form
    document.getElementById('register-form').reset();

    // Switch to login tab
    const loginTab = document.querySelector('[data-bs-target="#login-tab-pane"]');
    if (loginTab) {
      loginTab.click();
    }
  } catch (error) {
    console.error('Registration error:', error);
    utils.showNotification(error.message || 'Registration failed. Please try again.', 'error');
  } finally {
    // Reset submit button
    submitButton.disabled = false;
    submitButton.textContent = 'Register';
  }
}
