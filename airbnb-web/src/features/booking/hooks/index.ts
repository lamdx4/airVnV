import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { bookingApi } from '../api/bookingApi';
import { BookingDto, CreateBookingRequest } from '../types';

const QUERY_KEYS = {
  GUEST_BOOKINGS: ['guest_bookings'],
  HOST_BOOKINGS: ['host_bookings']
};

export const useCreateBooking = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (req: CreateBookingRequest) => bookingApi.createBooking(req),
    onSuccess: () => {
      // Invalidate guest bookings so they see their new trip
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.GUEST_BOOKINGS });
    }
  });
};

export const useGuestBookings = () => {
  return useQuery({
    queryKey: QUERY_KEYS.GUEST_BOOKINGS,
    queryFn: () => bookingApi.getGuestBookings()
  });
};

export const useHostBookings = () => {
  return useQuery({
    queryKey: QUERY_KEYS.HOST_BOOKINGS,
    queryFn: () => bookingApi.getHostBookings()
  });
};

export const useApproveBooking = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (bookingId: string) => bookingApi.approveBooking(bookingId),
    onSuccess: (_, bookingId) => {
      // Optimistic or simple invalidation. Here we invalidate host bookings.
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.HOST_BOOKINGS });
    }
  });
};

export const useRejectBooking = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (bookingId: string) => bookingApi.rejectBooking(bookingId),
    onSuccess: (_, bookingId) => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.HOST_BOOKINGS });
    }
  });
};

export const useCancelBooking = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (bookingId: string) => bookingApi.cancelBooking(bookingId),
    onSuccess: () => {
      // Could be either host or guest cancelling, safe to invalidate both
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.GUEST_BOOKINGS });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.HOST_BOOKINGS });
    }
  });
};
