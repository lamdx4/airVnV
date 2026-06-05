import { api } from "@/lib/api";
import type { ApiResponse, PaginatedResponse } from "@/types/api";
import type {
  Payment,
  PaymentDetail,
  PaymentListParams,
  RefundRequest,
} from "../types";

export const paymentsApi = {
  getAll: (params?: PaymentListParams) =>
    api.get<ApiResponse<PaginatedResponse<Payment>>>("/payments/admin", { params }),

  getById: (id: string) =>
    api.get<ApiResponse<PaymentDetail>>(`/payments/admin/${id}`),

  refund: (id: string, data: RefundRequest) =>
    api.post<ApiResponse<{ refundId: string; refundAmount: number; isFullRefund: boolean; newPaymentStatus: number }>>(`/payments/admin/${id}/refund`, data),
};
