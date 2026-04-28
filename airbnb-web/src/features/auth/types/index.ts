export type UserRole = 0 | 1; // 0 = Guest, 1 = Host

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
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
  id: string;
  fullName: string;
  email: string;
  role: UserRole;
}
