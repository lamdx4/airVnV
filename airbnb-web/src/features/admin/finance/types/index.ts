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
