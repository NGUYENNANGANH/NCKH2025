/**
 * Login and Registration Page JavaScript
 * This file contains the logic for the login and registration page
 */

// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function() {
  // Initialize the login and registration page
  initLoginRegisterPage();
});

/**
 * Initialize the login and registration page
 */
function initLoginRegisterPage() {
  // Check if user is already logged in
  if (authService.isLoggedIn()) {
    // Redirect to account page or home page
    const currentUser = authService.getCurrentUser();
    if (currentUser) {
      showLoggedInMessage(currentUser.fullName || currentUser.userName || currentUser.email);
    } else {
      window.location.href = 'index.html';
    }
    return;
  }

  // Check URL parameters for tab selection
  const urlParams = new URLSearchParams(window.location.search);
  const tabParam = urlParams.get('tab');

  if (tabParam === 'register') {
    // Switch to registration tab
    const registerTab = document.querySelector('[data-bs-target="#login-register-registration-form"]');
    if (registerTab) {
      registerTab.click();
    }
  }

  // Initialize form event listeners
  initFormEventListeners();
}

/**
 * Initialize form event listeners
 */
function initFormEventListeners() {
  // Login form
  const loginForm = document.getElementById('login-form');
  if (loginForm) {
    loginForm.addEventListener('submit', handleLoginSubmit);
  }

  // Registration form
  const registerForm = document.getElementById('register-form');
  if (registerForm) {
    registerForm.addEventListener('submit', handleRegisterSubmit);
  }

  // Password visibility toggle
  const passwordToggles = document.querySelectorAll('.password-toggle');
  passwordToggles.forEach(toggle => {
    toggle.addEventListener('click', togglePasswordVisibility);
  });

  // Form validation
  const passwordInputs = document.querySelectorAll('input[type="password"]');
  passwordInputs.forEach(input => {
    input.addEventListener('input', validatePassword);
  });

  // Password confirmation validation
  const confirmPasswordInput = document.getElementById('login-register-reg-confirm-password');
  if (confirmPasswordInput) {
    confirmPasswordInput.addEventListener('input', validatePasswordMatch);
  }
}

/**
 * Handle login form submission
 * @param {Event} event - The form submission event
 */
async function handleLoginSubmit(event) {
  event.preventDefault();

  // Get form data
  const emailInput = document.getElementById('login-register-login-email');
  const passwordInput = document.getElementById('login-register-login-password');
  const rememberMeInput = document.getElementById('login-register-remember-me');

  // Validate form data
  if (!emailInput.value || !passwordInput.value) {
    showFormError('login-form-error', 'Please enter both email and password.');
    return;
  }

  // Show loading state
  const submitButton = event.target.querySelector('button[type="submit"]');
  const originalButtonText = submitButton.innerHTML;
  submitButton.disabled = true;
  submitButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Logging in...';

  try {
    // Call the login API
    const response = await authService.login(emailInput.value, passwordInput.value);

    // Handle successful login
    if (response && (response.isSuccess || response.token)) {
      // If remember me is checked, store the token for longer
      if (rememberMeInput && rememberMeInput.checked) {
        // This would typically be handled by the backend with a longer-lived token
        console.log('Remember me checked');
      }

      // Show success message
      showFormSuccess('login-form-success', response.message || 'Login successful! Redirecting...');

      // Redirect to home page or previous page
      setTimeout(() => {
        window.location.href = 'index.html';
      }, 1500);
    } else {
      // Handle unsuccessful login
      showFormError('login-form-error', response.message || response.title || 'Login failed. Please try again.');
    }
  } catch (error) {
    // Handle login error
    console.error('Login error:', error);
    let errorMessage = 'Login failed. Please check your credentials and try again.';

    if (error.message) {
      errorMessage = error.message;
    }

    // Kiểm tra xem lỗi có phải từ API không
    if (error.response) {
      const response = error.response;
      if (response.title) {
        errorMessage = response.title;
      } else if (response.errors) {
        // Xử lý lỗi validation từ API
        const errors = [];
        for (const key in response.errors) {
          if (Array.isArray(response.errors[key])) {
            errors.push(...response.errors[key]);
          } else {
            errors.push(response.errors[key]);
          }
        }
        errorMessage = errors.join(', ');
      }
    }

    showFormError('login-form-error', errorMessage);
  } finally {
    // Reset button state
    submitButton.disabled = false;
    submitButton.innerHTML = originalButtonText;
  }
}

/**
 * Handle registration form submission
 * @param {Event} event - The form submission event
 */
async function handleRegisterSubmit(event) {
  event.preventDefault();

  // Get form data
  const firstNameInput = document.getElementById('login-register-reg-firstname');
  const lastNameInput = document.getElementById('login-register-reg-lastname');
  const emailInput = document.getElementById('login-register-reg-email');
  const passwordInput = document.getElementById('login-register-reg-password');
  const confirmPasswordInput = document.getElementById('login-register-reg-confirm-password');
  const termsCheckbox = document.getElementById('login-register-terms');

  // Validate form data
  if (!firstNameInput.value || !lastNameInput.value || !emailInput.value || !passwordInput.value || !confirmPasswordInput.value) {
    showFormError('register-form-error', 'Please fill in all required fields.');
    return;
  }

  if (passwordInput.value !== confirmPasswordInput.value) {
    showFormError('register-form-error', 'Passwords do not match.');
    return;
  }

  if (!termsCheckbox.checked) {
    showFormError('register-form-error', 'You must agree to the Terms of Service and Privacy Policy.');
    return;
  }

  // Show loading state
  const submitButton = event.target.querySelector('button[type="submit"]');
  const originalButtonText = submitButton.innerHTML;
  submitButton.disabled = true;
  submitButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Creating account...';

  try {
    // Prepare user data with correct field names for API
    const userData = {
      FullName: `${firstNameInput.value} ${lastNameInput.value}`,
      Email: emailInput.value,
      Password: passwordInput.value,
      ConfirmPassword: confirmPasswordInput.value
    };

    // Call the registration API
    const response = await authService.register(userData);

    // Handle registration response
    if (response && (response.isSuccess || response.token || response.message)) {
      // Show success message
      showFormSuccess('register-form-success', response.message || 'Registration successful! You can now log in.');

      // Switch to login tab after a delay
      setTimeout(() => {
        const loginTab = document.querySelector('[data-bs-target="#login-register-login-form"]');
        if (loginTab) {
          loginTab.click();
        }
      }, 2000);
    } else {
      // Handle unsuccessful registration
      let errorMessage = 'Registration failed. Please try again.';
      if (response && response.title) {
        errorMessage = response.title;
      } else if (response && response.message) {
        errorMessage = response.message;
      } else if (response && response.errors) {
        // Xử lý lỗi validation từ API
        const errors = [];
        for (const key in response.errors) {
          if (Array.isArray(response.errors[key])) {
            errors.push(...response.errors[key]);
          } else {
            errors.push(response.errors[key]);
          }
        }
        errorMessage = errors.join(', ');
      }
      showFormError('register-form-error', errorMessage);
    }
  } catch (error) {
    // Handle registration error
    console.error('Registration error:', error);
    let errorMessage = 'Registration failed. Please try again.';

    if (error.message) {
      errorMessage = error.message;
    }

    // Kiểm tra xem lỗi có phải từ API không
    if (error.response) {
      const response = error.response;
      if (response.title) {
        errorMessage = response.title;
      } else if (response.errors) {
        // Xử lý lỗi validation từ API
        const errors = [];
        for (const key in response.errors) {
          if (Array.isArray(response.errors[key])) {
            errors.push(...response.errors[key]);
          } else {
            errors.push(response.errors[key]);
          }
        }
        errorMessage = errors.join(', ');
      }
    }

    showFormError('register-form-error', errorMessage);
  } finally {
    // Reset button state
    submitButton.disabled = false;
    submitButton.innerHTML = originalButtonText;
  }
}

/**
 * Toggle password visibility
 * @param {Event} event - The click event
 */
function togglePasswordVisibility(event) {
  const button = event.currentTarget;
  const passwordInput = button.previousElementSibling;

  if (passwordInput.type === 'password') {
    passwordInput.type = 'text';
    button.innerHTML = '<i class="bi bi-eye-slash"></i>';
  } else {
    passwordInput.type = 'password';
    button.innerHTML = '<i class="bi bi-eye"></i>';
  }
}

/**
 * Validate password strength
 * @param {Event} event - The input event
 */
function validatePassword(event) {
  const passwordInput = event.target;
  const password = passwordInput.value;

  // Simple password validation
  const isValid = password.length >= 8;

  if (isValid) {
    passwordInput.classList.remove('is-invalid');
    passwordInput.classList.add('is-valid');
  } else {
    passwordInput.classList.remove('is-valid');
    if (password.length > 0) {
      passwordInput.classList.add('is-invalid');
    } else {
      passwordInput.classList.remove('is-invalid');
    }
  }
}

/**
 * Validate password match
 * @param {Event} event - The input event
 */
function validatePasswordMatch(event) {
  const confirmPasswordInput = event.target;
  const passwordInput = document.getElementById('login-register-reg-password');

  if (passwordInput && confirmPasswordInput.value) {
    const isMatch = confirmPasswordInput.value === passwordInput.value;

    if (isMatch) {
      confirmPasswordInput.classList.remove('is-invalid');
      confirmPasswordInput.classList.add('is-valid');
    } else {
      confirmPasswordInput.classList.remove('is-valid');
      confirmPasswordInput.classList.add('is-invalid');
    }
  }
}

/**
 * Show form error message
 * @param {string} elementId - The ID of the error element
 * @param {string} message - The error message
 */
function showFormError(elementId, message) {
  const errorElement = document.getElementById(elementId);
  if (errorElement) {
    errorElement.textContent = message;
    errorElement.style.display = 'block';

    // Hide after 5 seconds
    setTimeout(() => {
      errorElement.style.display = 'none';
    }, 5000);
  }
}

/**
 * Show form success message
 * @param {string} elementId - The ID of the success element
 * @param {string} message - The success message
 */
function showFormSuccess(elementId, message) {
  const successElement = document.getElementById(elementId);
  if (successElement) {
    successElement.textContent = message;
    successElement.style.display = 'block';

    // Hide after 5 seconds
    setTimeout(() => {
      successElement.style.display = 'none';
    }, 5000);
  }
}

/**
 * Show logged in message
 * @param {string} userName - The user's name or email
 */
function showLoggedInMessage(userName) {
  const loginRegisterSection = document.getElementById('login-register');
  if (loginRegisterSection) {
    loginRegisterSection.innerHTML = `
      <div class="container" data-aos="fade-up" data-aos-delay="100">
        <div class="row justify-content-center">
          <div class="col-lg-6 text-center">
            <div class="alert alert-success mb-4">
              <h4 class="alert-heading">You're already logged in!</h4>
              <p>Welcome back, ${userName}!</p>
              <hr>
              <div class="d-flex justify-content-center gap-3">
                <a href="index.html" class="btn btn-primary">Go to Home</a>
                <a href="account.html" class="btn btn-outline-primary">My Account</a>
                <button class="btn btn-outline-danger" onclick="authService.logout()">Logout</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    `;
  }
}

// Thêm console log để debug
console.log('Login/Register script loaded');
