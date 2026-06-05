import { useQuery } from "@tanstack/react-query";

import { reportsApi } from "../api/reports";
import type { ReportSummary, RevenueBreakdown, TopProperty, UserGrowthPoint } from "../types";

const QUERY_KEYS = {
  SUMMARY: (from: string, to: string) => ["admin", "reports", "summary", from, to] as const,
  REVENUE_BREAKDOWN: (from: string, to: string, groupBy: string) =>
    ["admin", "reports", "revenue-breakdown", from, to, groupBy] as const,
  TOP_PROPERTIES: (from: string, to: string, limit: number) =>
    ["admin", "reports", "top-properties", from, to, limit] as const,
  USER_GROWTH: (from: string, to: string, groupBy: string) =>
    ["admin", "reports", "user-growth", from, to, groupBy] as const,
} as const;

export function useReportSummary(from: string, to: string) {
  return useQuery({
    queryKey: QUERY_KEYS.SUMMARY(from, to),
    queryFn: async () => {
      const response = await reportsApi.getSummary(from, to);
      return response.data as unknown as ReportSummary;
    },
  });
}

export function useRevenueBreakdown(from: string, to: string, groupBy: "day" | "week" | "month" = "day") {
  return useQuery({
    queryKey: QUERY_KEYS.REVENUE_BREAKDOWN(from, to, groupBy),
    queryFn: async () => {
      const response = await reportsApi.getRevenueBreakdown(from, to, groupBy);
      return response.data as unknown as RevenueBreakdown[];
    },
  });
}

export function useTopProperties(from: string, to: string, limit = 10) {
  return useQuery({
    queryKey: QUERY_KEYS.TOP_PROPERTIES(from, to, limit),
    queryFn: async () => {
      const response = await reportsApi.getTopProperties(from, to, limit);
      return response.data as unknown as TopProperty[];
    },
  });
}

export function useUserGrowth(from: string, to: string, groupBy: "day" | "week" | "month" = "day") {
  return useQuery({
    queryKey: QUERY_KEYS.USER_GROWTH(from, to, groupBy),
    queryFn: async () => {
      const response = await reportsApi.getUserGrowth(from, to, groupBy);
      return response.data as unknown as UserGrowthPoint[];
    },
  });
}
