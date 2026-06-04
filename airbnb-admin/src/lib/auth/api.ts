import { api } from "@/lib/api";
import type { ApiResponse } from "@/types/api";

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  fullName: string;
  email: string;
  role: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
}

export interface AdminProfile {
  userId: string;
  email: string;
  fullName: string;
  role: string;
  avatarUrl?: string;
  phoneNumber?: string;
  bio?: string;
}

export const authApi = {
  login: (data: LoginRequest) =>
    api.post<ApiResponse<LoginResponse>>("/users/admin/login", data),

  logout: () => api.post<ApiResponse<null>>("/account/sessions/revoke"),

  refreshToken: (data: RefreshTokenRequest) =>
    api.post<ApiResponse<RefreshTokenResponse>>("/users/refresh-token", data),

  getProfile: () => api.get<ApiResponse<AdminProfile>>("/users/me"),
};
