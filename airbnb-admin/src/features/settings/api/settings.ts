import { api } from "@/lib/api";
import type { ApiResponse } from "@/types/api";

export interface AdminProfile {
  id: string;
  email: string;
  fullName: string;
  avatarUrl?: string;
  phone?: string;
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
  getProfile: () => api.get<ApiResponse<AdminProfile>>("/admin/profile"),

  updateProfile: (data: UpdateProfileRequest) =>
    api.patch<ApiResponse<AdminProfile>>("/admin/profile", data),

  changePassword: (data: ChangePasswordRequest) =>
    api.post<ApiResponse<null>>("/admin/change-password", data),

  getSystemSettings: () =>
    api.get<ApiResponse<SystemSetting[]>>("/admin/settings"),

  updateSystemSetting: (key: string, value: string) =>
    api.patch<ApiResponse<SystemSetting>>(`/admin/settings/${key}`, { value }),
};
