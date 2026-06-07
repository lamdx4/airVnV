import { api } from "@/lib/api";

import type {
  AdminPaymentDetail,
  AdminPaymentItem,
  PaymentListParams,
} from "../types";

interface BackendPagedPayments {
  items: AdminPaymentItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface RefundRequest {
  amount: number;
  reason: string;
  ticketId?: string;
}

export interface RefundResponse {
  refundId: string;
  paymentId: string;
  refundedNow: number;
  totalRefunded: number;
  isFullRefund: boolean;
}

export const paymentsApi = {
  getAll: (params?: PaymentListParams) =>
    api.get<BackendPagedPayments>("/admin/payments", { params }),

  getById: (id: string) => api.get<AdminPaymentDetail>(`/admin/payments/${id}`),

  refund: (id: string, body: RefundRequest) =>
    api.post<RefundResponse>(`/admin/payments/${id}/refund`, body),
};
