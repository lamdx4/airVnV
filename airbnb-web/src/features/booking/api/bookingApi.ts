import { apiClient } from '@/utils/apiClient';
import { BookingDto, CreateBookingRequest, CreateBookingResponse } from '../types';

interface ApiResponse<T> {
  data: T;
  message: string | null;
  success: boolean;
  errorCode: string | null;
}

export const bookingApi = {
  // Guest calls this to create a booking
  createBooking: async (req: CreateBookingRequest): Promise<CreateBookingResponse> => {
    const response = await apiClient.post<ApiResponse<CreateBookingResponse>>('/bookingservice/api/bookings', req);
    return response.data.data;
  },

  // Guest fetches their own bookings
  getGuestBookings: async (): Promise<BookingDto[]> => {
    const response = await apiClient.get<ApiResponse<BookingDto[]>>('/bookingservice/api/bookings/guest');
    return response.data.data || [];
  },

  // Host fetches bookings for all their properties
  getHostBookings: async (): Promise<BookingDto[]> => {
    const response = await apiClient.get<ApiResponse<BookingDto[]>>('/bookingservice/api/bookings/host');
    return response.data.data || [];
  },

  // Host approves a pending booking
  approveBooking: async (bookingId: string): Promise<boolean> => {
    const response = await apiClient.post<ApiResponse<boolean>>(`/bookingservice/api/bookings/${bookingId}/approve`);
    return response.data.data;
  },

  // Host rejects a pending booking
  rejectBooking: async (bookingId: string): Promise<boolean> => {
    const response = await apiClient.post<ApiResponse<boolean>>(`/bookingservice/api/bookings/${bookingId}/reject`);
    return response.data.data;
  },

  // Host or Guest cancels a booking
  cancelBooking: async (bookingId: string): Promise<boolean> => {
    const response = await apiClient.post<ApiResponse<boolean>>(`/bookingservice/api/bookings/${bookingId}/cancel`);
    return response.data.data;
  }
};
