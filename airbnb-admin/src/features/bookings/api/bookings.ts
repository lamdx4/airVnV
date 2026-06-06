import { api } from "@/lib/api";
import type { PaginatedResponse } from "@/types/api";
import type { Booking, BookingDetail, BookingListParams } from "../types";

export const bookingsApi = {
  getAll: (params?: BookingListParams) =>
    api.get<PaginatedResponse<Booking>>("/bookings/admin", { params }),

  getById: (id: string) =>
    api.get<BookingDetail>(`/bookings/admin/${id}`),

  cancel: (id: string, reason: string) =>
    api.patch<boolean>(`/bookings/admin/${id}/cancel`, { reason }),

  confirm: (id: string) =>
    api.patch<boolean>(`/bookings/admin/${id}/confirm`),
};
