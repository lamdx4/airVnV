export const PaymentStatus = {
  PENDING: "Pending",
  COMPLETED: "Completed",
  FAILED: "Failed",
  REFUNDED: "Refunded",
  PARTIALLY_REFUNDED: "PartiallyRefunded",
} as const;

export type PaymentStatusValue = (typeof PaymentStatus)[keyof typeof PaymentStatus];

export interface Payment {
  id: string;
  bookingId: string;
  userId: string;
  userName: string;
  amount: number;
  currency: string;
  provider: string;
  status: PaymentStatusValue;
  method: string;
  transactionId?: string;
  createdAt: string;
}

export interface PaymentListParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: PaymentStatusValue;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}
