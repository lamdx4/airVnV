import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

import { payoutsApi } from "../api/payouts";
import type { Payout, PayoutDetail, PayoutListParams } from "../types";
import type { PaginatedResponse } from "@/types/api";

const QUERY_KEYS = {
  ALL: ["admin", "payouts"] as const,
  LIST: (params?: PayoutListParams) => ["admin", "payouts", "list", params] as const,
  DETAIL: (id: string) => ["admin", "payouts", "detail", id] as const,
} as const;

export function usePayouts(params?: PayoutListParams) {
  return useQuery({
    queryKey: QUERY_KEYS.LIST(params),
    queryFn: async () => {
      const response = await payoutsApi.getAll(params);
      const raw = response.data as unknown as PaginatedResponse<Payout>;
      return raw;
    },
  });
}

export function usePayout(id: string) {
  return useQuery({
    queryKey: QUERY_KEYS.DETAIL(id),
    queryFn: async () => {
      const response = await payoutsApi.getById(id);
      return response.data as unknown as PayoutDetail;
    },
    enabled: !!id,
  });
}

export function useGeneratePayouts() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async () => {
      const response = await payoutsApi.generate();
      return response.data as unknown as { payoutsGenerated: number; bookingsProcessed: number };
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useApprovePayout() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: string) => {
      const response = await payoutsApi.approve(id);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useExecutePayout() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: string) => {
      const response = await payoutsApi.execute(id);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useCancelPayout() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: string) => {
      const response = await payoutsApi.cancel(id);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useRetryPayout() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id: string) => {
      const response = await payoutsApi.retry(id);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}
