// API Base URL
const API_BASE_URL = 'http://localhost:5140/api';

// Token management
const getToken = () => {
    const token = localStorage.getItem('token');
    if (!token) return null;
    
    // Kiểm tra token có hợp lệ không
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const expiry = payload.exp * 1000; // Convert to milliseconds
        if (Date.now() >= expiry) {
            removeToken();
            return null;
        }
        return token;
    } catch (error) {
        console.error('Invalid token:', error);
        removeToken();
        return null;
    }
};

const setToken = (token) => {
    if (!token) {
        removeToken();
        return;
    }
    localStorage.setItem('token', token);
};

const removeToken = () => {
    localStorage.removeItem('token');
    window.location.href = 'login.html';
};

// Check if user is logged in
const isLoggedIn = () => {
    const token = getToken();
    return !!token;
};

// Redirect to login if not authenticated
const requireAuth = () => {
    if (!isLoggedIn()) {
        window.location.href = 'login.html';
    }
};

// API request helper
const apiRequest = async (endpoint, options = {}) => {
    const token = getToken();
    if (!token && endpoint !== '/Auth/login' && endpoint !== '/Auth/register') {
        window.location.href = 'login.html';
        return;
    }

    const headers = {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` }),
        ...options.headers
    };

    try {
        console.log('Making request to:', `${API_BASE_URL}${endpoint}`);
        console.log('Request options:', { ...options, headers });
        
        const response = await fetch(`${API_BASE_URL}${endpoint}`, {
            ...options,
            headers
        });

        if (response.status === 401) {
            removeToken();
            window.location.href = 'login.html';
            return;
        }

        // Xử lý các response đặc biệt
        if (response.status === 204) {
            return true;
        }

        // Kiểm tra content type của response
        const contentType = response.headers.get('content-type');
        let data;
        
        if (contentType && contentType.includes('application/json')) {
            data = await response.json();
            console.log('Response:', data);
        } else {
            // Nếu không phải JSON, đọc response dưới dạng text
            const text = await response.text();
            console.log('Response text:', text);
            throw new Error('Server trả về dữ liệu không hợp lệ');
        }

        if (!response.ok) {
            let errorMessage = 'Có lỗi xảy ra';
            
            if (data && data.message) {
                errorMessage = data.message;
            } else if (data && data.errors) {
                if (Array.isArray(data.errors)) {
                    errorMessage = data.errors.join(', ');
                } else if (typeof data.errors === 'object') {
                    errorMessage = Object.values(data.errors).flat().join(', ');
                }
            } else if (response.status === 403) {
                errorMessage = 'Bạn không có quyền thực hiện thao tác này';
            }
            
            const error = new Error(errorMessage);
            error.status = response.status;
            throw error;
        }

        return data;
    } catch (error) {
        console.error('API Request Error:', error);
        throw error;
    }
};

// Show alert message
const showAlert = (message, type = 'success') => {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type}`;
    alertDiv.textContent = message;
    
    const container = document.querySelector('.container');
    container.insertBefore(alertDiv, container.firstChild);
    
    setTimeout(() => alertDiv.remove(), 5000);
};

// Format currency
const formatCurrency = (amount) => {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
};

// Format date
const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('vi-VN');
};

// Handle form submission
const handleFormSubmit = async (formId, endpoint, method = 'POST', onSuccess) => {
    const form = document.getElementById(formId);
    if (!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const formData = new FormData(form);
        const data = Object.fromEntries(formData.entries());
        
        try {
            const response = await apiRequest(endpoint, {
                method,
                body: JSON.stringify(data)
            });
            
            if (onSuccess) {
                onSuccess(response);
            }
            
            showAlert('Thao tác thành công!');
        } catch (error) {
            showAlert('Có lỗi xảy ra!', 'error');
        }
    });
};

// Load data into table
const loadTableData = async (endpoint, tableId, columns) => {
    const table = document.getElementById(tableId);
    if (!table) return;

    try {
        const data = await apiRequest(endpoint);
        const tbody = table.querySelector('tbody');
        tbody.innerHTML = '';

        data.forEach(item => {
            const row = document.createElement('tr');
            columns.forEach(column => {
                const cell = document.createElement('td');
                cell.textContent = item[column.key];
                row.appendChild(cell);
            });
            tbody.appendChild(row);
        });
    } catch (error) {
        showAlert('Không thể tải dữ liệu!', 'error');
    }
}; 