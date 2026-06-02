export interface DashboardStats {
  totalProperties: number;
  totalBookings: number;
  totalUsers: number;
  totalRevenue: number;
  pendingReviews: number;
  activeBookings: number;
}

export interface RevenueChartPoint {
  date: string;
  revenue: number;
  bookings: number;
}

export interface RecentActivity {
  id: string;
  type: "booking" | "property" | "user" | "payment" | "review";
  description: string;
  timestamp: string;
}
