import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

import { paymentsApi } from "../api/payments";
import type { PaymentListParams } from "../types";

const QUERY_KEYS = {
  ALL: ["admin", "payments"] as const,
  LIST: (params?: PaymentListParams) => ["admin", "payments", "list", params] as const,
  DETAIL: (id: string) => ["admin", "payments", "detail", id] as const,
} as const;

export function usePayments(params?: PaymentListParams) {
  return useQuery({
    queryKey: QUERY_KEYS.LIST(params),
    queryFn: () => paymentsApi.getAll(params),
  });
}

export function usePayment(id: string) {
  return useQuery({
    queryKey: QUERY_KEYS.DETAIL(id),
    queryFn: () => paymentsApi.getById(id),
    enabled: !!id,
  });
}

export function useRefundPayment() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, amount, reason }: { id: string; amount?: number; reason?: string }) =>
      paymentsApi.refund(id, amount, reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}
