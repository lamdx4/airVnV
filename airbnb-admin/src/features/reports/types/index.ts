export interface ReportSummary {
  totalRevenue: number;
  totalBookings: number;
  averageBookingValue: number;
  occupancyRate: number;
  newUsers: number;
  newProperties: number;
  totalUsers: number;
  activeUsers: number;
  suspendedUsers: number;
  bannedUsers: number;
  userCount: number;
  adminCount: number;
}

export interface UserGrowthPoint {
  label: string;
  newUsers: number;
  totalUsers: number;
}

export interface UserActivityReport {
  activeLast7Days: number;
  activeLast30Days: number;
  activeLast90Days: number;
  inactiveOver90Days: number;
  neverLoggedIn: number;
}

export interface PropertyStatusFunnel {
  draft: number;
  pendingReview: number;
  published: number;
  suspended: number;
  rejected: number;
  archived: number;
}

export interface PendingBacklog {
  pendingCount: number;
  averageWaitDays: number;
  maxWaitDays: number;
  overdueCount: number;
  overdueThresholdDays: number;
}

export interface NewListingPoint {
  label: string;
  newListings: number;
}

export interface TypeCount {
  type: string;
  count: number;
}

export interface PriceBucket {
  label: string;
  min: number;
  max: number | null;
  count: number;
}

export interface PriceDistribution {
  buckets: PriceBucket[];
  median: number;
  p90: number;
  average: number;
  total: number;
}

export type GroupBy = "day" | "week" | "month";
