import { api } from '@/lib/api';
import type {
  UserListItem,
  UserDetail,
  SuspendUserRequest,
  KYCApproveRequest,
  KYCRejectRequest,
  AdminActionResponse,
} from '../types';

// Response types
export interface GetUsersResponse {
  items: UserListItem[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface GetUserDetailResponse extends UserDetail {}

export const adminUsersApi = {
  getUsers: (params: {
    page?: number;
    pageSize?: number;
    search?: string;
    role?: string;
    status?: string;
    sortBy?: string;
    sortOrder?: 'asc' | 'desc';
  }): Promise<GetUsersResponse> => {
    return api.get('/api/admin/users', { params }) as Promise<GetUsersResponse>;
  },

  getUserById: (userId: string): Promise<GetUserDetailResponse> => {
    return api.get(`/api/admin/users/${userId}`) as Promise<GetUserDetailResponse>;
  },

  suspendUser: (userId: string, data: SuspendUserRequest): Promise<AdminActionResponse> => {
    return api.post(`/api/admin/users/${userId}/suspend`, data) as Promise<AdminActionResponse>;
  },

  unsuspendUser: (userId: string): Promise<AdminActionResponse> => {
    return api.post(`/api/admin/users/${userId}/unsuspend`) as Promise<AdminActionResponse>;
  },

  approveKYC: (userId: string, data?: KYCApproveRequest): Promise<AdminActionResponse> => {
    return api.post(`/api/admin/users/${userId}/kyc/approve`, data) as Promise<AdminActionResponse>;
  },

  rejectKYC: (userId: string, data: KYCRejectRequest): Promise<AdminActionResponse> => {
    return api.post(`/api/admin/users/${userId}/kyc/reject`, data) as Promise<AdminActionResponse>;
  },
};