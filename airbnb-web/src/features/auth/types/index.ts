export type UserRole = 0 | 1; // 0 = Guest, 1 = Host

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
