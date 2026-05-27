export type BookingStatus = 'Pending' | 'AwaitingApproval' | 'Confirmed' | 'Cancelled' | 'Completed';

// Represents the payload sent to the API
export interface CreateBookingRequest {
  propertyId: string;
  checkIn: string; // YYYY-MM-DD
  checkOut: string; // YYYY-MM-DD
  guestCount: number;
}

// Represents the response from the Create API
export interface CreateBookingResponse {
  bookingId: string;
  status: BookingStatus;
}

// Represents a Booking record retrieved from the backend API
export interface BookingDto {
  id: string;
  propertyId: string;
  hostId: string;
  guestId?: string; // included in some endpoints
  checkIn: string; // YYYY-MM-DD
  checkOut: string; // YYYY-MM-DD
  guestCount: number;
  nightCount: number;
  totalPrice: number;
  currencyCode: string;
  status: BookingStatus;
}
