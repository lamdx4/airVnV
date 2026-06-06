export const BookingStatus = {
  PENDING:      "Pending",
  CONFIRMED:    "Confirmed",
  AWAITING:     "AwaitingApproval",
  CHECKED_IN:   "CheckedIn",
  CHECKED_OUT:  "CheckedOut",
  CANCELLED:    "Cancelled",
  REFUNDED:     "Refunded",
} as const;

export type BookingStatusValue = (typeof BookingStatus)[keyof typeof BookingStatus];

export interface Booking {
  id: string;
  propertyId: string;
  hostId: string;
  guestId: string;
  countryCode: string;
  bookingMode: string;
  checkIn: string;
  checkOut: string;
  guestCount: number;
  nightCount: number;
  totalPrice: number;
  currencyCode: string;
  status: BookingStatusValue;
  createdAt: string;
}

export interface BookingDetail extends Booking {
  basePricePerNight: number;
  cleaningFee: number;
  serviceFee: number;
  taxAmount: number;
}

export interface BookingListParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: BookingStatusValue;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}
