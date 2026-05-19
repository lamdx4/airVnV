export interface SessionResponse {
  id: string;
  userAgent: string | null;
  ipAddress: string | null;
  loginAt: string;
  expiresAt: string;
  isCurrent: boolean;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}
