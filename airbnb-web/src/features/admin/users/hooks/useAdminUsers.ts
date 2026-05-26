import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminUsersApi } from '../api/adminUsers';
import type { SuspendUserRequest, KYCApproveRequest, KYCRejectRequest } from '../types';
import { toast } from 'sonner';

// Query keys
export const userKeys = {
  all: ['admin', 'users'] as const,
  list: (filters: GetUsersParams) => [...userKeys.all, 'list', filters] as const,
  detail: (userId: string) => [...userKeys.all, 'detail', userId] as const,
  kyc: (userId: string) => [...userKeys.all, 'kyc', userId] as const,
};

export interface GetUsersParams {
  page?: number;
  pageSize?: number;
  search?: string;
  role?: string;
  status?: string;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

export function useUsers(params: GetUsersParams = {}) {
  return useQuery({
    queryKey: userKeys.list(params),
    queryFn: () => adminUsersApi.getUsers(params),
    staleTime: 30_000,
  });
}

export function useUserDetail(userId: string) {
  return useQuery({
    queryKey: userKeys.detail(userId),
    queryFn: () => adminUsersApi.getUserById(userId),
    enabled: !!userId,
  });
}

// Mutations

export function useSuspendUser() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ userId, data }: { userId: string; data: SuspendUserRequest }) =>
      adminUsersApi.suspendUser(userId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: userKeys.all });
      toast.success('User suspended successfully');
    },
    onError: (error: { message?: string }) => {
      toast.error(error.message || 'Failed to suspend user');
    },
  });
}

export function useUnsuspendUser() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (userId: string) => adminUsersApi.unsuspendUser(userId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: userKeys.all });
      toast.success('User unsuspended successfully');
    },
    onError: (error: { message?: string }) => {
      toast.error(error.message || 'Failed to unsuspend user');
    },
  });
}

export function useApproveKYC() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ userId, data }: { userId: string; data?: KYCApproveRequest }) =>
      adminUsersApi.approveKYC(userId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: userKeys.all });
      queryClient.invalidateQueries({ queryKey: userKeys.all });
      toast.success('KYC approved. User is now verified.');
    },
    onError: (error: { message?: string }) => {
      toast.error(error.message || 'Failed to approve KYC');
    },
  });
}

export function useRejectKYC() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ userId, data }: { userId: string; data: KYCRejectRequest }) =>
      adminUsersApi.rejectKYC(userId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: userKeys.all });
      queryClient.invalidateQueries({ queryKey: userKeys.all });
      toast.success('KYC rejected');
    },
    onError: (error: { message?: string }) => {
      toast.error(error.message || 'Failed to reject KYC');
    },
  });
}