import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

export const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor tự động đính kèm Token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('airbnb_access_token');
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Interceptor xử lý lỗi global
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Logout user hoặc redirect về trang Login
      localStorage.removeItem('airbnb_access_token');
      localStorage.removeItem('airbnb_refresh_token');
      localStorage.removeItem('airbnb_user_id');
    }
    return Promise.reject(error);
  }
);
