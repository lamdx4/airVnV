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
  async (error) => {
    const originalRequest = error.config;

    // Nếu lỗi 401 và chưa thử retry
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      const refreshToken = localStorage.getItem('airbnb_refresh_token');

      if (refreshToken) {
        try {
          // Gọi API refresh token (dùng axios gốc để tránh interceptor hiện tại)
          const response = await axios.post(`${API_URL}/api/users/refresh-token`, {
            refreshToken
          });

          const { success, data } = response.data;

          if (success && data.accessToken) {
            // Lưu token mới
            localStorage.setItem('airbnb_access_token', data.accessToken);
            localStorage.setItem('airbnb_refresh_token', data.refreshToken);

            // Cập nhật header và thực hiện lại request cũ
            originalRequest.headers.Authorization = `Bearer ${data.accessToken}`;
            return api(originalRequest);
          }
        } catch (refreshError) {
          // Refresh token cũng lỗi -> xử lý logout
          console.error('Refresh token failed', refreshError);
        }
      }

      // Nếu không có refresh token hoặc refresh thất bại
      localStorage.removeItem('airbnb_access_token');
      localStorage.removeItem('airbnb_refresh_token');
      localStorage.removeItem('airbnb_user_id');

      if (!window.location.pathname.includes('/login')) {
        window.location.href = '/login?expired=true';
      }
    }

    const apiError = error.response?.data as ApiResponse<null>;
    return Promise.reject(apiError || { 
      success: false, 
      message: error.message, 
      errorCode: 'SYSTEM_ERROR' 
    });
  }
);
