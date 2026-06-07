export const PayoutStatus = {
  PENDING: "Pending",
  APPROVED: "Approved",
  PROCESSING: "Processing",
  COMPLETED: "Completed",
  FAILED: "Failed",
  CANCELLED: "Cancelled",
} as const;

export type PayoutStatusValue = (typeof PayoutStatus)[keyof typeof PayoutStatus];

export interface AdminPayoutItem {
  id: string;
  hostId: string;
  hostName: string | null;
  hostEmail: string | null;
  hostAvatarUrl: string | null;
  totalEarnings: number;
  platformFee: number;
  payoutAmount: number;
  currency: string;
  status: PayoutStatusValue;
  itemCount: number;
  createdAt: string;
  approvedAt: string | null;
  completedAt: string | null;
}

export interface PayoutLineItem {
  id: string;
  bookingId: string;
  paymentId: string;
  bookingTotal: number;
  serviceFee: number;
  hostEarning: number;
  checkIn: string; // yyyy-MM-dd
  checkOut: string;
  propertyTitle: string;
  guestName: string;
}

export interface AdminPayoutDetail extends AdminPayoutItem {
  approvedBy: string | null;
  items: PayoutLineItem[];
}

export interface PayoutListParams {
  page?: number;
  pageSize?: number;
  status?: PayoutStatusValue;
  hostId?: string;
  currency?: string;
}
