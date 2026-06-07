import { api } from "@/lib/api";

import type {
  AdminPayoutDetail,
  AdminPayoutItem,
  PayoutListParams,
} from "../types";

interface BackendPagedPayouts {
  items: AdminPayoutItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export const payoutsApi = {
  getAll: (params?: PayoutListParams) =>
    api.get<BackendPagedPayouts>("/admin/payouts", { params }),

  getById: (id: string) => api.get<AdminPayoutDetail>(`/admin/payouts/${id}`),

  approve: (id: string) => api.post(`/admin/payouts/${id}/approve`, {}),

  markCompleted: (id: string) =>
    api.post(`/admin/payouts/${id}/mark-completed`, {}),
};
