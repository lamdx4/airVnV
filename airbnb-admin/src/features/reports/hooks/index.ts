import { useQuery } from "@tanstack/react-query";

import { reportsApi } from "../api/reports";

const QUERY_KEYS = {
  SUMMARY: (from: string, to: string) => ["admin", "reports", "summary", from, to] as const,
  REVENUE_BREAKDOWN: (from: string, to: string, groupBy: string) =>
    ["admin", "reports", "revenue-breakdown", from, to, groupBy] as const,
  TOP_PROPERTIES: (from: string, to: string, limit: number) =>
    ["admin", "reports", "top-properties", from, to, limit] as const,
  USER_GROWTH: (from: string, to: string) =>
    ["admin", "reports", "user-growth", from, to] as const,
} as const;

export function useReportSummary(from: string, to: string) {
  return useQuery({
    queryKey: QUERY_KEYS.SUMMARY(from, to),
    queryFn: () => reportsApi.getSummary(from, to),
  });
}

export function useRevenueBreakdown(from: string, to: string, groupBy: "day" | "week" | "month" = "day") {
  return useQuery({
    queryKey: QUERY_KEYS.REVENUE_BREAKDOWN(from, to, groupBy),
    queryFn: () => reportsApi.getRevenueBreakdown(from, to, groupBy),
  });
}

export function useTopProperties(from: string, to: string, limit = 10) {
  return useQuery({
    queryKey: QUERY_KEYS.TOP_PROPERTIES(from, to, limit),
    queryFn: () => reportsApi.getTopProperties(from, to, limit),
  });
}

export function useUserGrowth(from: string, to: string) {
  return useQuery({
    queryKey: QUERY_KEYS.USER_GROWTH(from, to),
    queryFn: () => reportsApi.getUserGrowth(from, to),
  });
}
