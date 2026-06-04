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
