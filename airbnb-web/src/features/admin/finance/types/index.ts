export interface PayoutItem {
  payoutId: string;
  hostId: string;
  hostName: string;
  amount: number;
  currency: string;
  status: string;
  bookingCount: number;
  createdAt: string;
  processedAt: string | null;
}

export interface PayoutResponse {
  items: PayoutItem[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface DashboardFinance {
  totalPayIn: number;
  totalPayOut: number;
  platformRevenue: number;
  pendingPayouts: number;
  pendingPayoutCount: number;
  averageTransactionAmount: number;
  dailyStats: DailyFinanceStat[];
}

export interface DailyFinanceStat {
  date: string;
  payIn: number;
  payOut: number;
  revenue: number;
}

// Transaction History Types (UC-C1)
export interface TransactionItem {
  paymentId: string;
  bookingId: string;
  transactionId: string | null;
  amount: number;
  currency: string;
  status: string;
  type: 'PayIn' | 'PayOut';
  platformFee: number;
  netAmount: number;
  createdAt: string;
  processedAt: string | null;
}

export interface TransactionSummary {
  totalPayIn: number;
  totalPayOut: number;
  totalTransactions: number;
  successTransactions: number;
  failedTransactions: number;
  pendingTransactions: number;
}

export interface TransactionResponse {
  items: TransactionItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  summary: TransactionSummary;
}

// Platform Fee Config (UC-C3)
export interface PlatformFeeConfig {
  hostFeePercent: number;
  guestFeePercent: number;
  defaultPlatformFeePercent: number;
  lastUpdatedBy: string;
  lastUpdatedAt: string;
}
