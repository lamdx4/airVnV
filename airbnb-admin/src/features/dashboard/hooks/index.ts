import { useQuery } from "@tanstack/react-query";

import { dashboardApi } from "../api/dashboard";
import type { DashboardStats, RevenueChartPoint, RecentActivity } from "../types";

const QUERY_KEYS = {
  STATS: ["admin", "dashboard", "stats"],
  REVENUE_CHART: (days: number) => ["admin", "dashboard", "revenue-chart", days],
  RECENT_ACTIVITY: (limit: number) => ["admin", "dashboard", "recent-activity", limit],
} as const;

export function useDashboardStats() {
  return useQuery({
    queryKey: QUERY_KEYS.STATS,
    queryFn: async () => {
      const response = await dashboardApi.getStats();
      return response.data as unknown as DashboardStats;
    },
  });
}

export function useRevenueChart(days = 30) {
  return useQuery({
    queryKey: QUERY_KEYS.REVENUE_CHART(days),
    queryFn: async () => {
      const response = await dashboardApi.getRevenueChart(days);
      return response.data as unknown as RevenueChartPoint[];
    },
  });
}

export function useRecentActivity(limit = 10) {
  return useQuery({
    queryKey: QUERY_KEYS.RECENT_ACTIVITY(limit),
    queryFn: async () => {
      const response = await dashboardApi.getRecentActivity(limit);
      return response.data as unknown as RecentActivity[];
    },
  });
}
