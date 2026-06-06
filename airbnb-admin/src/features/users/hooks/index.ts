import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

import { usersApi } from "../api/users";
import type { User, UserDetail, KycDocument, UserListParams, UserRoleValue } from "../types";

const QUERY_KEYS = {
  ALL: ["admin", "users"] as const,
  LIST: (params?: UserListParams) => ["admin", "users", "list", params] as const,
  DETAIL: (id: string) => ["admin", "users", "detail", id] as const,
  KYC: (id: string) => ["admin", "users", id, "kyc"] as const,
} as const;

export function useUsers(params?: UserListParams) {
  return useQuery({
    queryKey: QUERY_KEYS.LIST(params),
    queryFn: async () => {
      const response = await usersApi.getAll(params);
      const raw = response.data;
      return {
        items: raw.items as User[],
        totalItems: raw.totalCount,
        page: raw.pageNumber,
        pageSize: raw.pageSize,
        totalPages: raw.totalPages,
      };
    },
  });
}

export function useUser(id: string) {
  return useQuery({
    queryKey: QUERY_KEYS.DETAIL(id),
    queryFn: async () => {
      const response = await usersApi.getById(id);
      return response.data as UserDetail;
    },
    enabled: !!id,
  });
}

export function useSuspendUser() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, reason }: { id: string; reason: string }) =>
      usersApi.suspend(id, reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useBanUser() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, reason }: { id: string; reason: string }) =>
      usersApi.ban(id, reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useActivateUser() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => usersApi.activate(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useUpdateUserRole() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, role }: { id: string; role: UserRoleValue }) =>
      usersApi.updateRole(id, role),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useDeleteUser() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => usersApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useKycDocuments(userId: string) {
  return useQuery({
    queryKey: QUERY_KEYS.KYC(userId),
    queryFn: async () => {
      const response = await usersApi.getKycDocuments(userId);
      return response.data as KycDocument[];
    },
    enabled: !!userId,
  });
}

export function useApproveVerification() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => usersApi.approveVerification(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useRejectVerification() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, reason }: { id: string; reason: string }) =>
      usersApi.rejectVerification(id, reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}
