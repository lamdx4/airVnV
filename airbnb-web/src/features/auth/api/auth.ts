import { api } from '@/lib/api';
import type { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse } from '../types';

export const loginUser = async (data: LoginRequest): Promise<LoginResponse> => {
  const res = await api.post<LoginResponse>('/api/users/login', data);
  return res.data;
};

export const registerUser = async (data: RegisterRequest): Promise<RegisterResponse> => {
  const res = await api.post<RegisterResponse>('/api/users/register', data);
  return res.data;
};
