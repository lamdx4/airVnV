import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import type { PaginatedResponse } from "@/types/api";

import { payoutsApi } from "../api/payouts";
import type { AdminPayoutItem, PayoutListParams } from "../types";

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
      const raw = response.data;
      return {
        items: raw.items,
        totalItems: raw.totalCount,
        page: raw.page,
        pageSize: raw.pageSize,
        totalPages: raw.totalPages,
      } satisfies PaginatedResponse<AdminPayoutItem>;
    },
  });
}

export function usePayout(id: string) {
  return useQuery({
    queryKey: QUERY_KEYS.DETAIL(id),
    queryFn: async () => (await payoutsApi.getById(id)).data,
    enabled: Boolean(id),
  });
}

export function useApprovePayout() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (id: string) => payoutsApi.approve(id),
    onSuccess: (_, id) => {
      qc.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
      qc.invalidateQueries({ queryKey: QUERY_KEYS.DETAIL(id) });
    },
  });
}

export function useMarkPayoutCompleted() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (id: string) => payoutsApi.markCompleted(id),
    onSuccess: (_, id) => {
      qc.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
      qc.invalidateQueries({ queryKey: QUERY_KEYS.DETAIL(id) });
    },
  });
}
