import { api } from "@/lib/api";
import type { ApiResponse } from "@/types/api";

export interface AdminProfile {
  userId: string;
  email: string;
  fullName: string;
  avatarUrl?: string;
  phoneNumber?: string;
  bio?: string;
  role: string;
}

export interface UpdateProfileRequest {
  fullName?: string;
  phone?: string;
  avatarUrl?: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface SystemSetting {
  key: string;
  value: string;
  description: string;
  updatedAt: string;
}

export const settingsApi = {
  getProfile: () => api.get<ApiResponse<AdminProfile>>("/users/me"),

  updateProfile: (data: UpdateProfileRequest) =>
    api.put<ApiResponse<AdminProfile>>("/users/me", data),

  changePassword: (data: ChangePasswordRequest) =>
    api.post<ApiResponse<null>>("/account/change-password", data),

  getSystemSettings: () =>
    api.get<ApiResponse<SystemSetting[]>>("/admin/settings"),

  updateSystemSetting: (key: string, value: string) =>
    api.patch<ApiResponse<SystemSetting>>(`/admin/settings/${key}`, { value }),
};
