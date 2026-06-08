import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import type { PaginatedResponse } from "@/types/api";

import { paymentsApi, type RefundRequest } from "../api/payments";
import type { AdminPaymentItem, PaymentListParams } from "../types";

const QUERY_KEYS = {
  ALL: ["admin", "payments"] as const,
  LIST: (params?: PaymentListParams) => ["admin", "payments", "list", params] as const,
  DETAIL: (id: string) => ["admin", "payments", "detail", id] as const,
} as const;

export function usePayments(params?: PaymentListParams) {
  return useQuery({
    queryKey: QUERY_KEYS.LIST(params),
    queryFn: async () => {
      const response = await paymentsApi.getAll(params);
      const raw = response.data;
      return {
        items: raw.items,
        totalItems: raw.totalCount,
        page: raw.page,
        pageSize: raw.pageSize,
        totalPages: raw.totalPages,
      } satisfies PaginatedResponse<AdminPaymentItem>;
    },
  });
}

export function usePayment(id: string) {
  return useQuery({
    queryKey: QUERY_KEYS.DETAIL(id),
    queryFn: async () => (await paymentsApi.getById(id)).data,
    enabled: Boolean(id),
  });
}

export function useRefundPayment(paymentId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (body: RefundRequest) =>
      (await paymentsApi.refund(paymentId, body)).data,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.DETAIL(paymentId) });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}
