import { useQuery } from "@tanstack/react-query";

import { dashboardApi } from "../api/dashboard";

const QUERY_KEYS = {
  STATS: ["admin", "dashboard", "stats"],
  REVENUE_CHART: (days: number) => ["admin", "dashboard", "revenue-chart", days],
  RECENT_ACTIVITY: (limit: number) => ["admin", "dashboard", "recent-activity", limit],
} as const;

export function useDashboardStats() {
  return useQuery({
    queryKey: QUERY_KEYS.STATS,
    queryFn: () => dashboardApi.getStats(),
  });
}

export function useRevenueChart(days = 30) {
  return useQuery({
    queryKey: QUERY_KEYS.REVENUE_CHART(days),
    queryFn: () => dashboardApi.getRevenueChart(days),
  });
}

export function useRecentActivity(limit = 10) {
  return useQuery({
    queryKey: QUERY_KEYS.RECENT_ACTIVITY(limit),
    queryFn: () => dashboardApi.getRecentActivity(limit),
  });
}
