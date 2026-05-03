import { api } from '@/lib/api';
import type { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse, VerifyEmailRequest, VerifyEmailResponse, GoogleAuthRequest, GoogleAuthResponse } from '../types';

export const loginUser = async (data: LoginRequest): Promise<LoginResponse> => {
  return await api.post('/api/users/login', data);
};

export const registerUser = async (data: RegisterRequest): Promise<RegisterResponse> => {
  return await api.post('/api/users/register', data);
};

export const verifyEmail = async (data: VerifyEmailRequest): Promise<VerifyEmailResponse> => {
  return await api.post('/api/users/verify-email', data);
};

export const googleAuthUser = async (data: GoogleAuthRequest): Promise<GoogleAuthResponse> => {
  return await api.post('/api/users/google-auth', data);
};
