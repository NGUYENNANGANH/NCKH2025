import axios from 'axios';

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Product API
export const productApi = {
  getProducts: async () => {
    const response = await api.get('/products');
    return response.data;
  },
  
  getProductById: async (id: number) => {
    const response = await api.get(`/products/${id}`);
    return response.data;
  },
};

// Category API
export const categoryApi = {
  getCategories: async () => {
    const response = await api.get('/categories');
    return response.data;
  },
};

// Cart API
export const cartApi = {
  getCart: async () => {
    const response = await api.get('/cart');
    return response.data;
  },
  
  addToCart: async (productId: number, quantity: number) => {
    const response = await api.post('/cart', { productId, quantity });
    return response.data;
  },
  
  updateCartItem: async (itemId: number, quantity: number) => {
    const response = await api.put(`/cart/${itemId}`, { quantity });
    return response.data;
  },
  
  removeFromCart: async (itemId: number) => {
    const response = await api.delete(`/cart/${itemId}`);
    return response.data;
  },
};

// User API
export const userApi = {
  login: async (email: string, password: string) => {
    const response = await api.post('/auth/login', { email, password });
    return response.data;
  },
  
  register: async (userData: any) => {
    const response = await api.post('/auth/register', userData);
    return response.data;
  },
  
  getProfile: async () => {
    const response = await api.get('/user/profile');
    return response.data;
  },
};

export default api; 