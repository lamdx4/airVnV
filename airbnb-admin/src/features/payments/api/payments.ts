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

export const paymentsApi = {
  getAll: (params?: PaymentListParams) =>
    api.get<BackendPagedPayments>("/admin/payments", { params }),

  getById: (id: string) => api.get<AdminPaymentDetail>(`/admin/payments/${id}`),
};
