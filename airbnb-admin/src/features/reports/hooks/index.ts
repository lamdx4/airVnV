import { useQuery } from "@tanstack/react-query";

import { reportsApi } from "../api/reports";
import type { GroupBy } from "../types";

const QUERY_KEYS = {
  SUMMARY: (from: string, to: string) => ["admin", "reports", "summary", from, to] as const,
  USER_GROWTH: (from: string, to: string, groupBy: GroupBy) =>
    ["admin", "reports", "user-growth", from, to, groupBy] as const,
  USER_ACTIVITY: () => ["admin", "reports", "user-activity"] as const,
  STATUS_FUNNEL: () => ["admin", "reports", "status-funnel"] as const,
  PENDING_BACKLOG: () => ["admin", "reports", "pending-backlog"] as const,
  NEW_LISTINGS: (from: string, to: string, groupBy: GroupBy) =>
    ["admin", "reports", "new-listings", from, to, groupBy] as const,
  TYPE_DISTRIBUTION: () => ["admin", "reports", "type-distribution"] as const,
  PRICE_DISTRIBUTION: () => ["admin", "reports", "price-distribution"] as const,
} as const;

export function useReportSummary(from: string, to: string) {
  return useQuery({
    queryKey: QUERY_KEYS.SUMMARY(from, to),
    queryFn: async () => (await reportsApi.getSummary(from, to)).data,
  });
}

export function useUserGrowth(from: string, to: string, groupBy: GroupBy = "day") {
  return useQuery({
    queryKey: QUERY_KEYS.USER_GROWTH(from, to, groupBy),
    queryFn: async () => (await reportsApi.getUserGrowth(from, to, groupBy)).data,
  });
}

export function useUserActivity() {
  return useQuery({
    queryKey: QUERY_KEYS.USER_ACTIVITY(),
    queryFn: async () => (await reportsApi.getUserActivity()).data,
  });
}

export function useStatusFunnel() {
  return useQuery({
    queryKey: QUERY_KEYS.STATUS_FUNNEL(),
    queryFn: async () => (await reportsApi.getStatusFunnel()).data,
  });
}

export function usePendingBacklog() {
  return useQuery({
    queryKey: QUERY_KEYS.PENDING_BACKLOG(),
    queryFn: async () => (await reportsApi.getPendingBacklog()).data,
  });
}

export function useNewListings(from: string, to: string, groupBy: GroupBy = "day") {
  return useQuery({
    queryKey: QUERY_KEYS.NEW_LISTINGS(from, to, groupBy),
    queryFn: async () => (await reportsApi.getNewListings(from, to, groupBy)).data,
  });
}

export function useTypeDistribution() {
  return useQuery({
    queryKey: QUERY_KEYS.TYPE_DISTRIBUTION(),
    queryFn: async () => (await reportsApi.getTypeDistribution()).data,
  });
}

export function usePriceDistribution() {
  return useQuery({
    queryKey: QUERY_KEYS.PRICE_DISTRIBUTION(),
    queryFn: async () => (await reportsApi.getPriceDistribution()).data,
  });
}
