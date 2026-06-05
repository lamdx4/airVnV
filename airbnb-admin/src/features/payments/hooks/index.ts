import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

import { paymentsApi } from "../api/payments";
import type { Payment, PaymentDetail, PaymentListParams, RefundRequest } from "../types";
import type { PaginatedResponse } from "@/types/api";

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
      const raw = response.data as unknown as PaginatedResponse<Payment>;
      return raw;
    },
  });
}

export function usePayment(id: string) {
  return useQuery({
    queryKey: QUERY_KEYS.DETAIL(id),
    queryFn: async () => {
      const response = await paymentsApi.getById(id);
      return response.data as unknown as PaymentDetail;
    },
    enabled: !!id,
  });
}

export function useRefundPayment() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ id, data }: { id: string; data: RefundRequest }) => {
      const response = await paymentsApi.refund(id, data);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}
