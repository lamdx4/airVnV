import { api } from "@/lib/api";
import type { ApiResponse, PaginatedResponse } from "@/types/api";

export const PaymentStatus = {
  PENDING: "Pending",
  COMPLETED: "Completed",
  FAILED: "Failed",
  REFUNDED: "Refunded",
  PARTIALLY_REFUNDED: "PartiallyRefunded",
} as const;

export type PaymentStatusValue = (typeof PaymentStatus)[keyof typeof PaymentStatus];

export interface Payment {
  id: string;
  bookingId: string;
  userId: string;
  userName: string;
  amount: number;
  currency: string;
  provider: string;
  status: PaymentStatusValue;
  method: string;
  transactionId?: string;
  createdAt: string;
}

export interface PaymentListParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: PaymentStatusValue;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}

export const paymentsApi = {
  getAll: (params?: PaymentListParams) =>
    api.get<ApiResponse<PaginatedResponse<Payment>>>("/admin/payments", { params }),

  getById: (id: string) =>
    api.get<ApiResponse<Payment>>(`/admin/payments/${id}`),

  refund: (id: string, amount?: number, reason?: string) =>
    api.post<ApiResponse<Payment>>(`/admin/payments/${id}/refund`, { amount, reason }),
};
