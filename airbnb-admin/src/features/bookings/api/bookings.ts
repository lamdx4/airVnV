import { api } from "@/lib/api";
import type { ApiResponse, PaginatedResponse } from "@/types/api";
import type { Booking, BookingDetail, BookingListParams } from "../types";

export const bookingsApi = {
  getAll: (params?: BookingListParams) =>
    api.get<ApiResponse<PaginatedResponse<Booking>>>("/bookings/admin", { params }),

  getById: (id: string) =>
    api.get<ApiResponse<BookingDetail>>(`/bookings/admin/${id}`),

  cancel: (id: string, reason: string) =>
    api.patch<ApiResponse<boolean>>(`/bookings/admin/${id}/cancel`, { reason }),

  confirm: (id: string) =>
    api.patch<ApiResponse<boolean>>(`/bookings/admin/${id}/confirm`),
};
