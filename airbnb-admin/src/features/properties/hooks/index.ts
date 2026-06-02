import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

import { propertiesApi } from "../api/properties";
import type { PropertyListParams } from "../types";

const QUERY_KEYS = {
  ALL: ["admin", "properties"] as const,
  LIST: (params?: PropertyListParams) => ["admin", "properties", "list", params] as const,
  DETAIL: (id: string) => ["admin", "properties", "detail", id] as const,
} as const;

export function useProperties(params?: PropertyListParams) {
  return useQuery({
    queryKey: QUERY_KEYS.LIST(params),
    queryFn: () => propertiesApi.getAll(params),
  });
}

export function useProperty(id: string) {
  return useQuery({
    queryKey: QUERY_KEYS.DETAIL(id),
    queryFn: () => propertiesApi.getById(id),
    enabled: !!id,
  });
}

export function useApproveProperty() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => propertiesApi.approve(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useRejectProperty() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, reason }: { id: string; reason: string }) =>
      propertiesApi.reject(id, reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useDeactivateProperty() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => propertiesApi.deactivate(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useDeleteProperty() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => propertiesApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}
