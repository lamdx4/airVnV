import { api } from '@/lib/api';
import type { SessionResponse, ChangePasswordRequest } from '../types';

export const getSessions = async (): Promise<SessionResponse[]> => {
  const response = await api.get<SessionResponse[]>('/api/account/sessions') as any;
  return response;
};

export const revokeSession = async (sessionId: string): Promise<boolean> => {
  const response = await api.post<boolean>('/api/account/sessions/revoke', { sessionId }) as any;
  return response;
};

export const changePassword = async (data: ChangePasswordRequest): Promise<boolean> => {
  const response = await api.post<boolean>('/api/account/change-password', data) as any;
  return response;
};
