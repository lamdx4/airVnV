export const PaymentStatus = {
  PENDING: "Pending",
  SUCCESS: "Success",
  FAILED: "Failed",
  EXPIRED: "Expired",
  REFUNDED: "Refunded",
  PARTIALLY_REFUNDED: "PartiallyRefunded",
} as const;

export type PaymentStatusValue = (typeof PaymentStatus)[keyof typeof PaymentStatus];

export interface AdminPaymentItem {
  id: string;
  bookingId: string;
  amount: number;
  currency: string;
  status: PaymentStatusValue;
  transactionId: string | null;
  createdAt: string;
  expiresAt: string | null;
}

export interface AdminPaymentDetail extends AdminPaymentItem {
  paymentUrl: string | null;
}

export interface PaymentListParams {
  page?: number;
  pageSize?: number;
  status?: PaymentStatusValue;
  search?: string;
  from?: string;
  to?: string;
  sortOrder?: "asc" | "desc";
}
