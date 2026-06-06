import { api } from "@/lib/api";

import type { DashboardStats, RevenueChartPoint, RecentActivity } from "../types";

export const dashboardApi = {
  getStats: () =>
    api.get<DashboardStats>("/admin/dashboard/stats"),

  getRevenueChart: (days = 30) =>
    api.get<RevenueChartPoint[]>("/admin/dashboard/revenue-chart", {
      params: { days },
    }),

  getRecentActivity: (limit = 10) =>
    api.get<RecentActivity[]>("/admin/dashboard/recent-activity", {
      params: { limit },
    }),
};
