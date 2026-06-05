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

export interface PlatformFeeConfig {
  id: string;
  feePercentage: number;
  description?: string;
  changedBy: string;
  previousValue?: number;
  createdAt: string;
}

export interface PlatformFeeHistoryItem {
  id: string;
  feePercentage: number;
  description?: string;
  isActive: boolean;
  changedBy: string;
  previousValue?: number;
  createdAt: string;
}

export interface CreatePlatformFeeRequest {
  feePercentage: number;
  description?: string;
}
