import axios from "axios";

import { API_BASE_URL } from "@/config/constants";
import type { ApiResponse } from "@/types/api";

const api = axios.create({
  baseURL: `${API_BASE_URL}/api`,
  headers: { "Content-Type": "application/json" },
});

api.interceptors.request.use((config) => {
  const token =
    typeof window !== "undefined"
      ? localStorage.getItem("admin_access_token")
      : null;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

api.interceptors.response.use(
  (response) => {
    const body = response.data as ApiResponse<unknown>;
    if (body && typeof body === "object" && "success" in body && body.success === true) {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      return { ...response, data: body.data } as any;
    }
    return response;
  },
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      const refreshToken = localStorage.getItem("admin_refresh_token");
      if (!refreshToken) {
        localStorage.removeItem("admin_access_token");
        window.location.href = "/login?expired=true";
        return Promise.reject(error);
      }

      try {
        const { data } = await axios.post<ApiResponse<{ accessToken: string; refreshToken: string }>>(
          `${API_BASE_URL}/api/users/refresh-token`,
          { refreshToken },
        );

        localStorage.setItem("admin_access_token", data.data.accessToken);
        localStorage.setItem("admin_refresh_token", data.data.refreshToken);
        originalRequest.headers.Authorization = `Bearer ${data.data.accessToken}`;

        return api(originalRequest);
      } catch {
        localStorage.removeItem("admin_access_token");
        localStorage.removeItem("admin_refresh_token");
        window.location.href = "/login?expired=true";
      }
    }

    return Promise.reject(error);
  },
);

export { api };
