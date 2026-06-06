import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

import { bookingsApi } from "../api/bookings";
import type { BookingListParams } from "../types";

const QUERY_KEYS = {
  ALL: ["admin", "bookings"] as const,
  LIST: (params?: BookingListParams) => ["admin", "bookings", "list", params] as const,
  DETAIL: (id: string) => ["admin", "bookings", "detail", id] as const,
} as const;

export function useBookings(params?: BookingListParams) {
  return useQuery({
    queryKey: QUERY_KEYS.LIST(params),
    queryFn: async () => {
      const response = await bookingsApi.getAll(params);
      return response.data;
    },
  });
}

export function useBooking(id: string) {
  return useQuery({
    queryKey: QUERY_KEYS.DETAIL(id),
    queryFn: async () => {
      const response = await bookingsApi.getById(id);
      return response.data;
    },
    enabled: !!id,
  });
}

export function useCancelBooking() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, reason }: { id: string; reason: string }) =>
      bookingsApi.cancel(id, reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useConfirmBooking() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => bookingsApi.confirm(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}
