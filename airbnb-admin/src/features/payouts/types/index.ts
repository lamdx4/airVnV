export const PayoutStatus = {
  PENDING: 0,
  APPROVED: 1,
  PROCESSING: 2,
  COMPLETED: 3,
  FAILED: 4,
  CANCELLED: 5,
} as const;

export type PayoutStatusValue = (typeof PayoutStatus)[keyof typeof PayoutStatus];

export const PayoutStatusLabel: Record<number, string> = {
  [PayoutStatus.PENDING]: "Pending",
  [PayoutStatus.APPROVED]: "Approved",
  [PayoutStatus.PROCESSING]: "Processing",
  [PayoutStatus.COMPLETED]: "Completed",
  [PayoutStatus.FAILED]: "Failed",
  [PayoutStatus.CANCELLED]: "Cancelled",
};

export interface Payout {
  id: string;
  hostId: string;
  totalEarnings: number;
  platformFee: number;
  payoutAmount: number;
  currency: string;
  status: PayoutStatusValue;
  itemCount: number;
  approvedAt?: string;
  completedAt?: string;
  createdAt: string;
}

export interface PayoutItem {
  id: string;
  bookingId: string;
  paymentId: string;
  bookingTotal: number;
  serviceFee: number;
  hostEarning: number;
  checkIn: string;
  checkOut: string;
  propertyTitle: string;
  guestName: string;
}

export interface PayoutDetail extends Payout {
  approvedBy?: string;
  items: PayoutItem[];
}

export interface PayoutListParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: PayoutStatusValue;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}
