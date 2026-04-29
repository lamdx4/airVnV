import { api } from '@/lib/api';
import type { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse, VerifyEmailRequest, VerifyEmailResponse, GoogleAuthRequest, GoogleAuthResponse } from '../types';

export const loginUser = async (data: LoginRequest): Promise<LoginResponse> => {
  const res = await api.post<LoginResponse>('/api/users/login', data);
  return res.data;
};

export const registerUser = async (data: RegisterRequest): Promise<RegisterResponse> => {
  const res = await api.post<RegisterResponse>('/api/users/register', data);
  return res.data;
};

export const verifyEmail = async (data: VerifyEmailRequest): Promise<VerifyEmailResponse> => {
  const res = await api.post<VerifyEmailResponse>('/api/users/verify-email', data);
  return res.data;
};

export const googleAuthUser = async (data: GoogleAuthRequest): Promise<GoogleAuthResponse> => {
  const res = await api.post<GoogleAuthResponse>('/api/users/google-auth', data);
  return res.data;
};
