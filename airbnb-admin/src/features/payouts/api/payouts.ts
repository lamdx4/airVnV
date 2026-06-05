import { api } from "@/lib/api";
import type { ApiResponse, PaginatedResponse } from "@/types/api";
import type { Payout, PayoutDetail, PayoutListParams } from "../types";

export const payoutsApi = {
  getAll: (params?: PayoutListParams) =>
    api.get<ApiResponse<PaginatedResponse<Payout>>>("/payouts/admin", { params }),

  getById: (id: string) =>
    api.get<ApiResponse<PayoutDetail>>(`/payouts/admin/${id}`),

  generate: () =>
    api.post<ApiResponse<{ payoutsGenerated: number; bookingsProcessed: number }>>("/payouts/admin/generate"),

  approve: (id: string) =>
    api.patch<ApiResponse<{ payoutId: string; newStatus: number }>>(`/payouts/admin/${id}/approve`),

  execute: (id: string) =>
    api.patch<ApiResponse<{ payoutId: string; newStatus: number }>>(`/payouts/admin/${id}/execute`),

  cancel: (id: string) =>
    api.patch<ApiResponse<{ payoutId: string; newStatus: number }>>(`/payouts/admin/${id}/cancel`),

  retry: (id: string) =>
    api.patch<ApiResponse<{ payoutId: string; newStatus: number }>>(`/payouts/admin/${id}/retry`),
};
