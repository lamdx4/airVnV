export type UserRole = 'User' | 'Moderator' | 'Admin';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  fullName: string;
  email: string;
  role: UserRole;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  role: UserRole;
}

export interface RegisterResponse {
  message: string;
  otpCode?: string;
}

export interface VerifyEmailRequest {
  email: string;
  otpCode: string;
}

export interface VerifyEmailResponse {
  accessToken: string;
  refreshToken: string;
  fullName: string;
  email: string;
  role: UserRole;
}
