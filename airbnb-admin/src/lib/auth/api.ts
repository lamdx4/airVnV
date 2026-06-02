import { api } from "@/lib/api";
import type { ApiResponse } from "@/types/api";

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  user: {
    id: string;
    email: string;
    fullName: string;
    role: string;
    avatarUrl?: string;
  };
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
}

export const authApi = {
  login: (data: LoginRequest) =>
    api.post<ApiResponse<LoginResponse>>("/admin/login", data),

  logout: () => api.post<ApiResponse<null>>("/admin/logout"),

  refreshToken: (data: RefreshTokenRequest) =>
    api.post<ApiResponse<RefreshTokenResponse>>("/admin/refresh-token", data),

  getProfile: () => api.get<ApiResponse<LoginResponse["user"]>>("/admin/profile"),
};
