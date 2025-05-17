/**
 * Account Page JavaScript
 * This file contains the logic for the account page
 */

// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function() {
  // Initialize the account page
  initAccountPage();
});

/**
 * Initialize the account page
 */
function initAccountPage() {
  // Check if user is logged in
  if (!authService.isLoggedIn()) {
    // Redirect to login page
    window.location.href = 'login-register.html';
    return;
  }

  // Get current user data
  const currentUser = authService.getCurrentUser();
  
  // Load account content
  loadAccountContent(currentUser);
  
  // Initialize tab handling
  initTabHandling();
}

/**
 * Load account content based on user data
 * @param {Object} user - The user data
 */
function loadAccountContent(user) {
  const accountContent = document.getElementById('account-content');
  if (!accountContent) return;

  // Create account page structure
  const content = `
    <div class="row">
      <!-- Account Sidebar -->
      <div class="col-lg-3 mb-4">
        <div class="account-sidebar">
          <div class="user-info text-center mb-4">
            <div class="user-avatar mb-3">
              <span class="avatar-text">${getInitials(user.fullName || user.userName || 'User')}</span>
            </div>
            <h5>${user.fullName || user.userName || 'User'}</h5>
            <p class="text-muted">${user.email || user.userName}</p>
          </div>
          
          <div class="nav flex-column nav-pills" id="account-tabs" role="tablist">
            <button class="nav-link active" id="profile-tab" data-bs-toggle="pill" data-bs-target="#profile" type="button" role="tab">
              <i class="bi bi-person-circle me-2"></i> Thông tin cá nhân
            </button>
            <button class="nav-link" id="orders-tab" data-bs-toggle="pill" data-bs-target="#orders" type="button" role="tab">
              <i class="bi bi-bag-check me-2"></i> Đơn hàng của tôi
            </button>
            <button class="nav-link" id="wishlist-tab" data-bs-toggle="pill" data-bs-target="#wishlist" type="button" role="tab">
              <i class="bi bi-heart me-2"></i> Sản phẩm yêu thích
            </button>
            <button class="nav-link" id="addresses-tab" data-bs-toggle="pill" data-bs-target="#addresses" type="button" role="tab">
              <i class="bi bi-geo-alt me-2"></i> Địa chỉ
            </button>
            <button class="nav-link" id="settings-tab" data-bs-toggle="pill" data-bs-target="#settings" type="button" role="tab">
              <i class="bi bi-gear me-2"></i> Cài đặt tài khoản
            </button>
            <button class="nav-link text-danger" onclick="handleLogout()">
              <i class="bi bi-box-arrow-right me-2"></i> Đăng xuất
            </button>
          </div>
        </div>
      </div>
      
      <!-- Account Content -->
      <div class="col-lg-9">
        <div class="tab-content" id="account-tab-content">
          <!-- Profile Tab -->
          <div class="tab-pane fade show active" id="profile" role="tabpanel" aria-labelledby="profile-tab">
            <div class="card">
              <div class="card-header d-flex justify-content-between align-items-center">
                <h5 class="mb-0">Thông tin cá nhân</h5>
                <button class="btn btn-sm btn-primary" id="edit-profile-btn">
                  <i class="bi bi-pencil-square me-1"></i> Chỉnh sửa
                </button>
              </div>
              <div class="card-body">
                <div id="profile-view">
                  <div class="row mb-3">
                    <div class="col-md-4 fw-bold">Họ và tên:</div>
                    <div class="col-md-8" id="profile-fullname">${user.fullName || 'Chưa cập nhật'}</div>
                  </div>
                  <div class="row mb-3">
                    <div class="col-md-4 fw-bold">Email:</div>
                    <div class="col-md-8">${user.email || user.userName}</div>
                  </div>
                  <div class="row mb-3">
                    <div class="col-md-4 fw-bold">Số điện thoại:</div>
                    <div class="col-md-8" id="profile-phone">${user.phoneNumber || 'Chưa cập nhật'}</div>
                  </div>
                  <div class="row mb-3">
                    <div class="col-md-4 fw-bold">Địa chỉ:</div>
                    <div class="col-md-8" id="profile-address">${user.address || 'Chưa cập nhật'}</div>
                  </div>
                  <div class="row mb-3">
                    <div class="col-md-4 fw-bold">Ngày tham gia:</div>
                    <div class="col-md-8">${formatDate(user.createdAt || new Date())}</div>
                  </div>
                </div>
                
                <div id="profile-edit" style="display: none;">
                  <form id="profile-edit-form">
                    <div class="alert alert-danger" id="profile-edit-error" style="display: none;"></div>
                    <div class="alert alert-success" id="profile-edit-success" style="display: none;"></div>
                    
                    <div class="mb-3">
                      <label for="edit-fullname" class="form-label">Họ và tên</label>
                      <input type="text" class="form-control" id="edit-fullname" value="${user.fullName || ''}">
                    </div>
                    <div class="mb-3">
                      <label for="edit-email" class="form-label">Email</label>
                      <input type="email" class="form-control" id="edit-email" value="${user.email || user.userName}" disabled>
                      <div class="form-text">Email không thể thay đổi.</div>
                    </div>
                    <div class="mb-3">
                      <label for="edit-phone" class="form-label">Số điện thoại</label>
                      <input type="tel" class="form-control" id="edit-phone" value="${user.phoneNumber || ''}">
                    </div>
                    <div class="mb-3">
                      <label for="edit-address" class="form-label">Địa chỉ</label>
                      <textarea class="form-control" id="edit-address" rows="3">${user.address || ''}</textarea>
                    </div>
                    <div class="d-flex gap-2">
                      <button type="submit" class="btn btn-primary">Lưu thay đổi</button>
                      <button type="button" class="btn btn-outline-secondary" id="cancel-edit-btn">Hủy</button>
                    </div>
                  </form>
                </div>
              </div>
            </div>
          </div>
          
          <!-- Orders Tab -->
          <div class="tab-pane fade" id="orders" role="tabpanel" aria-labelledby="orders-tab">
            <div class="card">
              <div class="card-header">
                <h5 class="mb-0">Đơn hàng của tôi</h5>
              </div>
              <div class="card-body">
                <div id="orders-list">
                  <div class="text-center py-5">
                    <i class="bi bi-bag-check fs-1 text-muted"></i>
                    <p class="mt-3">Bạn chưa có đơn hàng nào.</p>
                    <a href="products.html" class="btn btn-primary mt-2">Mua sắm ngay</a>
                  </div>
                </div>
              </div>
            </div>
          </div>
          
          <!-- Wishlist Tab -->
          <div class="tab-pane fade" id="wishlist" role="tabpanel" aria-labelledby="wishlist-tab">
            <div class="card">
              <div class="card-header">
                <h5 class="mb-0">Sản phẩm yêu thích</h5>
              </div>
              <div class="card-body">
                <div id="wishlist-items">
                  <div class="text-center py-5">
                    <i class="bi bi-heart fs-1 text-muted"></i>
                    <p class="mt-3">Danh sách yêu thích của bạn đang trống.</p>
                    <a href="products.html" class="btn btn-primary mt-2">Khám phá sản phẩm</a>
                  </div>
                </div>
              </div>
            </div>
          </div>
          
          <!-- Addresses Tab -->
          <div class="tab-pane fade" id="addresses" role="tabpanel" aria-labelledby="addresses-tab">
            <div class="card">
              <div class="card-header d-flex justify-content-between align-items-center">
                <h5 class="mb-0">Địa chỉ của tôi</h5>
                <button class="btn btn-sm btn-primary">
                  <i class="bi bi-plus-lg me-1"></i> Thêm địa chỉ mới
                </button>
              </div>
              <div class="card-body">
                <div id="addresses-list">
                  <div class="text-center py-5">
                    <i class="bi bi-geo-alt fs-1 text-muted"></i>
                    <p class="mt-3">Bạn chưa có địa chỉ nào.</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
          
          <!-- Settings Tab -->
          <div class="tab-pane fade" id="settings" role="tabpanel" aria-labelledby="settings-tab">
            <div class="card mb-4">
              <div class="card-header">
                <h5 class="mb-0">Đổi mật khẩu</h5>
              </div>
              <div class="card-body">
                <form id="change-password-form">
                  <div class="alert alert-danger" id="password-change-error" style="display: none;"></div>
                  <div class="alert alert-success" id="password-change-success" style="display: none;"></div>
                  
                  <div class="mb-3">
                    <label for="current-password" class="form-label">Mật khẩu hiện tại</label>
                    <input type="password" class="form-control" id="current-password" required>
                  </div>
                  <div class="mb-3">
                    <label for="new-password" class="form-label">Mật khẩu mới</label>
                    <input type="password" class="form-control" id="new-password" required>
                    <div class="form-text">Mật khẩu phải có ít nhất 6 ký tự.</div>
                  </div>
                  <div class="mb-3">
                    <label for="confirm-password" class="form-label">Xác nhận mật khẩu mới</label>
                    <input type="password" class="form-control" id="confirm-password" required>
                  </div>
                  <button type="submit" class="btn btn-primary">Đổi mật khẩu</button>
                </form>
              </div>
            </div>
            
            <div class="card">
              <div class="card-header">
                <h5 class="mb-0">Cài đặt thông báo</h5>
              </div>
              <div class="card-body">
                <form id="notification-settings-form">
                  <div class="form-check form-switch mb-3">
                    <input class="form-check-input" type="checkbox" id="email-notifications" checked>
                    <label class="form-check-label" for="email-notifications">Nhận thông báo qua email</label>
                  </div>
                  <div class="form-check form-switch mb-3">
                    <input class="form-check-input" type="checkbox" id="order-updates">
                    <label class="form-check-label" for="order-updates">Cập nhật trạng thái đơn hàng</label>
                  </div>
                  <div class="form-check form-switch mb-3">
                    <input class="form-check-input" type="checkbox" id="promotional-emails" checked>
                    <label class="form-check-label" for="promotional-emails">Thông tin khuyến mãi và ưu đãi</label>
                  </div>
                  <button type="submit" class="btn btn-primary">Lưu cài đặt</button>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `;
  
  accountContent.innerHTML = content;
  
  // Initialize event listeners for profile editing
  initProfileEditListeners();
  
  // Initialize event listeners for password change
  initPasswordChangeListeners();
  
  // Initialize event listeners for notification settings
  initNotificationSettingsListeners();
}

/**
 * Initialize tab handling based on URL parameters
 */
function initTabHandling() {
  // Check URL parameters for tab selection
  const urlParams = new URLSearchParams(window.location.search);
  const tabParam = urlParams.get('tab');
  
  if (tabParam) {
    // Find the corresponding tab button
    const tabButton = document.querySelector(`#${tabParam}-tab`);
    if (tabButton) {
      // Activate the tab
      tabButton.click();
    }
  }
}

/**
 * Initialize event listeners for profile editing
 */
function initProfileEditListeners() {
  const editProfileBtn = document.getElementById('edit-profile-btn');
  const cancelEditBtn = document.getElementById('cancel-edit-btn');
  const profileEditForm = document.getElementById('profile-edit-form');
  
  if (editProfileBtn) {
    editProfileBtn.addEventListener('click', function() {
      document.getElementById('profile-view').style.display = 'none';
      document.getElementById('profile-edit').style.display = 'block';
    });
  }
  
  if (cancelEditBtn) {
    cancelEditBtn.addEventListener('click', function() {
      document.getElementById('profile-edit').style.display = 'none';
      document.getElementById('profile-view').style.display = 'block';
    });
  }
  
  if (profileEditForm) {
    profileEditForm.addEventListener('submit', handleProfileUpdate);
  }
}

/**
 * Handle profile update form submission
 * @param {Event} event - The form submission event
 */
async function handleProfileUpdate(event) {
  event.preventDefault();
  
  const fullNameInput = document.getElementById('edit-fullname');
  const phoneInput = document.getElementById('edit-phone');
  const addressInput = document.getElementById('edit-address');
  
  const errorElement = document.getElementById('profile-edit-error');
  const successElement = document.getElementById('profile-edit-success');
  
  // Hide previous messages
  errorElement.style.display = 'none';
  successElement.style.display = 'none';
  
  // Get form data
  const userData = {
    fullName: fullNameInput.value.trim(),
    phoneNumber: phoneInput.value.trim(),
    address: addressInput.value.trim()
  };
  
  try {
    // Call API to update user profile
    // This is a placeholder - you'll need to implement the actual API call
    // const response = await api.put('User/update-profile', userData);
    
    // For now, we'll just update the UI
    const currentUser = authService.getCurrentUser();
    currentUser.fullName = userData.fullName;
    currentUser.phoneNumber = userData.phoneNumber;
    currentUser.address = userData.address;
    
    // Update localStorage
    localStorage.setItem('user_data', JSON.stringify(currentUser));
    
    // Update profile view
    document.getElementById('profile-fullname').textContent = userData.fullName || 'Chưa cập nhật';
    document.getElementById('profile-phone').textContent = userData.phoneNumber || 'Chưa cập nhật';
    document.getElementById('profile-address').textContent = userData.address || 'Chưa cập nhật';
    
    // Show success message
    successElement.textContent = 'Cập nhật thông tin thành công!';
    successElement.style.display = 'block';
    
    // Switch back to view mode after a delay
    setTimeout(() => {
      document.getElementById('profile-edit').style.display = 'none';
      document.getElementById('profile-view').style.display = 'block';
    }, 2000);
    
  } catch (error) {
    // Show error message
    errorElement.textContent = error.message || 'Có lỗi xảy ra khi cập nhật thông tin.';
    errorElement.style.display = 'block';
  }
}

/**
 * Initialize event listeners for password change
 */
function initPasswordChangeListeners() {
  const changePasswordForm = document.getElementById('change-password-form');
  
  if (changePasswordForm) {
    changePasswordForm.addEventListener('submit', handlePasswordChange);
  }
}

/**
 * Handle password change form submission
 * @param {Event} event - The form submission event
 */
async function handlePasswordChange(event) {
  event.preventDefault();
  
  const currentPasswordInput = document.getElementById('current-password');
  const newPasswordInput = document.getElementById('new-password');
  const confirmPasswordInput = document.getElementById('confirm-password');
  
  const errorElement = document.getElementById('password-change-error');
  const successElement = document.getElementById('password-change-success');
  
  // Hide previous messages
  errorElement.style.display = 'none';
  successElement.style.display = 'none';
  
  // Validate passwords
  if (newPasswordInput.value.length < 6) {
    errorElement.textContent = 'Mật khẩu mới phải có ít nhất 6 ký tự.';
    errorElement.style.display = 'block';
    return;
  }
  
  if (newPasswordInput.value !== confirmPasswordInput.value) {
    errorElement.textContent = 'Mật khẩu xác nhận không khớp.';
    errorElement.style.display = 'block';
    return;
  }
  
  // Get form data
  const passwordData = {
    currentPassword: currentPasswordInput.value,
    newPassword: newPasswordInput.value,
    confirmPassword: confirmPasswordInput.value
  };
  
  try {
    // Call API to change password
    // This is a placeholder - you'll need to implement the actual API call
    // const response = await api.post('User/change-password', passwordData);
    
    // Show success message
    successElement.textContent = 'Đổi mật khẩu thành công!';
    successElement.style.display = 'block';
    
    // Reset form
    changePasswordForm.reset();
    
  } catch (error) {
    // Show error message
    errorElement.textContent = error.message || 'Có lỗi xảy ra khi đổi mật khẩu.';
    errorElement.style.display = 'block';
  }
}

/**
 * Initialize event listeners for notification settings
 */
function initNotificationSettingsListeners() {
  const notificationSettingsForm = document.getElementById('notification-settings-form');
  
  if (notificationSettingsForm) {
    notificationSettingsForm.addEventListener('submit', function(event) {
      event.preventDefault();
      
      // This is a placeholder - you'll need to implement the actual API call
      // For now, just show an alert
      alert('Cài đặt thông báo đã được lưu!');
    });
  }
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
 * Format date to locale string
 * @param {string|Date} date - The date to format
 * @returns {string} The formatted date
 */
function formatDate(date) {
  if (!date) return '';
  
  const dateObj = new Date(date);
  return dateObj.toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
}
