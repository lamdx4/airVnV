import { api } from "@/lib/api";
import type { ApiResponse } from "@/types/api";

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

export const dashboardApi = {
  getStats: () => api.get<ApiResponse<DashboardStats>>("/admin/dashboard/stats"),

  getRevenueChart: (days = 30) =>
    api.get<ApiResponse<RevenueChartPoint[]>>("/admin/dashboard/revenue-chart", {
      params: { days },
    }),

  getRecentActivity: (limit = 10) =>
    api.get<ApiResponse<RecentActivity[]>>("/admin/dashboard/recent-activity", {
      params: { limit },
    }),
};
