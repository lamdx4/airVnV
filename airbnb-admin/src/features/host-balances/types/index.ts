export const BalanceEntryType = {
  PAYMENT_RECEIVED: "PaymentReceived",
  BOOKING_CHECKED_OUT: "BookingCheckedOut",
  PAYOUT_APPROVED: "PayoutApproved",
  REFUND: "Refund",
  ADJUSTMENT: "Adjustment",
} as const;

export type BalanceEntryTypeValue =
  (typeof BalanceEntryType)[keyof typeof BalanceEntryType];

export interface HostBalanceItem {
  id: string;
  hostId: string;
  hostName: string | null;
  hostEmail: string | null;
  hostAvatarUrl: string | null;
  currency: string;
  pendingBalance: number;
  availableBalance: number;
  totalHeld: number;
  updatedAt: string;
}

export interface BalanceEntry {
  id: string;
  type: BalanceEntryTypeValue;
  pendingDelta: number;
  availableDelta: number;
  paymentId: string | null;
  payoutId: string | null;
  bookingId: string | null;
  note: string | null;
  createdAt: string;
}

export interface HostBalanceDetailResponse {
  id: string;
  hostId: string;
  hostName: string | null;
  hostEmail: string | null;
  hostAvatarUrl: string | null;
  currency: string;
  pendingBalance: number;
  availableBalance: number;
  updatedAt: string;
  entries: BalanceEntry[];
}

export interface HostBalancesListResponse {
  page: {
    items: HostBalanceItem[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
  };
  totalEscrowVnd: number;
  totalEscrowUsd: number;
}

export interface HostBalanceListParams {
  page?: number;
  pageSize?: number;
  currency?: string;
  hostId?: string;
}
