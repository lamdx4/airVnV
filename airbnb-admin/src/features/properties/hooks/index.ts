import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

import { propertiesApi } from "../api/properties";
import type { PropertyListParams, Property } from "../types";
import type { PaginatedResponse } from "@/types/api";

const QUERY_KEYS = {
  ALL: ["admin", "properties"] as const,
  LIST: (params?: PropertyListParams) => ["admin", "properties", "list", params] as const,
  DETAIL: (id: string) => ["admin", "properties", "detail", id] as const,
} as const;

export function useProperties(params?: PropertyListParams) {
  return useQuery({
    queryKey: QUERY_KEYS.LIST(params),
    queryFn: async () => {
      const response = await propertiesApi.getAll(params);
      const raw = response.data;
      return {
        items: raw.items,
        totalItems: raw.totalCount,
        page: raw.pageNumber,
        pageSize: raw.pageSize,
        totalPages: raw.totalPages,
      } satisfies PaginatedResponse<Property>;
    },
  });
}

export function useProperty(id: string) {
  return useQuery({
    queryKey: QUERY_KEYS.DETAIL(id),
    queryFn: async () => {
      const response = await propertiesApi.getById(id);
      return response.data;
    },
    enabled: !!id,
  });
}

export function useApproveProperty() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: string) => {
      const response = await propertiesApi.approve(id);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useRejectProperty() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ id, reason }: { id: string; reason: string }) => {
      const response = await propertiesApi.reject(id, reason);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useSuspendProperty() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ id, reason }: { id: string; reason?: string }) => {
      const response = await propertiesApi.suspend(id, reason ?? "Admin suspended");
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useReinstateProperty() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: string) => {
      const response = await propertiesApi.reinstate(id);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useAdminDeleteProperty() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: string) => {
      await propertiesApi.adminDelete(id);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}
