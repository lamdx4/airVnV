export const BookingStatus = {
  PENDING: "Pending",
  CONFIRMED: "Confirmed",
  CHECKED_IN: "CheckedIn",
  CHECKED_OUT: "CheckedOut",
  CANCELLED: "Cancelled",
  REFUNDED: "Refunded",
} as const;

export type BookingStatusValue = (typeof BookingStatus)[keyof typeof BookingStatus];

export interface Booking {
  id: string;
  propertyId: string;
  propertyTitle: string;
  guestId: string;
  guestName: string;
  hostId: string;
  hostName: string;
  checkIn: string;
  checkOut: string;
  guests: number;
  totalPrice: number;
  status: BookingStatusValue;
  createdAt: string;
}

export interface BookingListParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: BookingStatusValue;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}
