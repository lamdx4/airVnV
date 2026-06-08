import { api } from '@/lib/api';
import type { BookingDto, CreateBookingRequest, CreateBookingResponse } from '../types';

export const bookingApi = {
  // Guest calls this to create a booking
  createBooking: (req: CreateBookingRequest): Promise<CreateBookingResponse> => 
    api.post<any, CreateBookingResponse>('/api/bookings', req),

  // Guest fetches their own bookings
  getGuestBookings: (): Promise<BookingDto[]> => 
    api.get<any, BookingDto[]>('/api/bookings/guest'),

  // Host fetches bookings for all their properties
  getHostBookings: (): Promise<BookingDto[]> => 
    api.get<any, BookingDto[]>('/api/bookings/host'),

  // Host approves a pending booking
  approveBooking: (bookingId: string): Promise<boolean> =>
    api.post<any, boolean>(`/api/bookings/${bookingId}/approve`, {}),

  // Host rejects a pending booking
  rejectBooking: (bookingId: string): Promise<boolean> =>
    api.post<any, boolean>(`/api/bookings/${bookingId}/reject`, {}),

  // Host or Guest cancels a booking
  cancelBooking: (bookingId: string): Promise<boolean> =>
    api.post<any, boolean>(`/api/bookings/${bookingId}/cancel`, {})
};
