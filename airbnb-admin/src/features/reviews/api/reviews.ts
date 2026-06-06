import { api } from "@/lib/api";
import type { PaginatedResponse } from "@/types/api";
import type { Review, ReviewListParams } from "../types";

export const reviewsApi = {
  getAll: (params?: ReviewListParams) =>
    api.get<PaginatedResponse<Review>>("/admin/reviews", { params }),

  approve: (id: string) =>
    api.patch<Review>(`/admin/reviews/${id}/approve`),

  reject: (id: string, reason: string) =>
    api.patch<Review>(`/admin/reviews/${id}/reject`, { reason }),

  delete: (id: string) =>
    api.delete<null>(`/admin/reviews/${id}`),
};
