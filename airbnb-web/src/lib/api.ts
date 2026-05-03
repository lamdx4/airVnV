import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  errorCode?: string;
  errors?: string[];
  timestamp: string;
}

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

// Interceptor xử lý phản hồi
api.interceptors.response.use(
  (response) => {
    // Ép kiểu về ApiResponse
    const apiResponse = response.data as ApiResponse<any>;

    // Nếu Backend trả về success: true, ta trả về data thực tế luôn
    if (apiResponse.success) {
      return apiResponse.data;
    }

    // Nếu success: false, ta ném lỗi để đi vào block catch
    return Promise.reject(apiResponse);
  },
  (error) => {
    // Xử lý lỗi hệ thống (Network error, 500, 401...)
    const apiError = error.response?.data as ApiResponse<null>;
    
    if (error.response?.status === 401) {
      localStorage.removeItem('airbnb_access_token');
      localStorage.removeItem('airbnb_refresh_token');
      localStorage.removeItem('airbnb_user_id');
    }

    return Promise.reject(apiError || { 
      success: false, 
      message: error.message, 
      errorCode: 'SYSTEM_ERROR' 
    });
  }
);
