import { api } from '@/lib/api';
import type { BookingDto, CreateBookingRequest, CreateBookingResponse } from '../types';

export const bookingApi = {
  // Guest calls this to create a booking
  createBooking: (req: CreateBookingRequest): Promise<CreateBookingResponse> => 
    api.post('/bookingservice/api/bookings', req) as any,

  // Guest fetches their own bookings
  getGuestBookings: (): Promise<BookingDto[]> => 
    api.get('/bookingservice/api/bookings/guest') as any,

  // Host fetches bookings for all their properties
  getHostBookings: (): Promise<BookingDto[]> => 
    api.get('/bookingservice/api/bookings/host') as any,

  // Host approves a pending booking
  approveBooking: (bookingId: string): Promise<boolean> => 
    api.post(`/bookingservice/api/bookings/${bookingId}/approve`) as any,

  // Host rejects a pending booking
  rejectBooking: (bookingId: string): Promise<boolean> => 
    api.post(`/bookingservice/api/bookings/${bookingId}/reject`) as any,

  // Host or Guest cancels a booking
  cancelBooking: (bookingId: string): Promise<boolean> => 
    api.post(`/bookingservice/api/bookings/${bookingId}/cancel`) as any
};
