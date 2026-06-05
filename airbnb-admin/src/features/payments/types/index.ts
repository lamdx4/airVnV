export const PaymentStatusEnum = {
  0: "Pending",
  1: "Success",
  2: "Failed",
  3: "Expired",
  4: "Refunded",
  5: "PartiallyRefunded",
} as const;

export const PaymentStatus = {
  PENDING: 0,
  SUCCESS: 1,
  FAILED: 2,
  EXPIRED: 3,
  REFUNDED: 4,
  PARTIALLY_REFUNDED: 5,
} as const;

export type PaymentStatusValue = (typeof PaymentStatus)[keyof typeof PaymentStatus];

export const PaymentStatusLabel: Record<number, string> = {
  [PaymentStatus.PENDING]: "Pending",
  [PaymentStatus.SUCCESS]: "Success",
  [PaymentStatus.FAILED]: "Failed",
  [PaymentStatus.EXPIRED]: "Expired",
  [PaymentStatus.REFUNDED]: "Refunded",
  [PaymentStatus.PARTIALLY_REFUNDED]: "Partially Refunded",
};

export interface Payment {
  id: string;
  bookingId: string;
  amount: number;
  currency: string;
  status: PaymentStatusValue;
  transactionId?: string;
  provider: string;
  guestName?: string;
  hostName?: string;
  createdAt: string;
  expiresAt?: string;
}

export interface PaymentDetail extends Payment {
  paymentUrl?: string;
  refunds: RefundRecord[];
}

export interface RefundRecord {
  id: string;
  amount: number;
  reason: string;
  isFullRefund: boolean;
  performedBy: string;
  createdAt: string;
}

export interface PaymentListParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: PaymentStatusValue;
  fromDate?: string;
  toDate?: string;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}

export interface RefundRequest {
  amount?: number;
  reason: string;
  ticketId?: string;
}
