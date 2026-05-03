import { api } from '@/lib/api';
import type { ProfileResponse, UpdateProfileRequest, UpdateProfileResponse } from '../types';

export const getProfile = async (): Promise<ProfileResponse> => {
  return await api.get('/api/users/me');
};

export const updateProfile = async (data: UpdateProfileRequest): Promise<UpdateProfileResponse> => {
  return await api.put('/api/users/profile', data);
};
