import { api } from "@/lib/api";

import type {
  GroupBy,
  NewListingPoint,
  PendingBacklog,
  PriceDistribution,
  PropertyStatusFunnel,
  ReportSummary,
  RevenueOverview,
  RevenuePoint,
  TypeCount,
  UserActivityReport,
  UserGrowthPoint,
} from "../types";

export const reportsApi = {
  getSummary: (from: string, to: string) =>
    api.get<ReportSummary>("/admin/reports/summary", { params: { from, to } }),

  getUserGrowth: (from: string, to: string, groupBy: GroupBy = "day") =>
    api.get<UserGrowthPoint[]>("/admin/reports/user-growth", {
      params: { from, to, groupBy },
    }),

  getUserActivity: () =>
    api.get<UserActivityReport>("/admin/reports/user-activity"),

  getStatusFunnel: () =>
    api.get<PropertyStatusFunnel>("/properties/admin/reports/status-funnel"),

  getPendingBacklog: () =>
    api.get<PendingBacklog>("/properties/admin/reports/pending-backlog"),

  getNewListings: (from: string, to: string, groupBy: GroupBy = "day") =>
    api.get<NewListingPoint[]>("/properties/admin/reports/new-listings", {
      params: { from, to, groupBy },
    }),

  getTypeDistribution: () =>
    api.get<TypeCount[]>("/properties/admin/reports/type-distribution"),

  getPriceDistribution: () =>
    api.get<PriceDistribution>("/properties/admin/reports/price-distribution"),

  getRevenueOverview: (from: string, to: string) =>
    api.get<RevenueOverview>("/admin/payments/reports/revenue-overview", {
      params: { from, to },
    }),

  getRevenueSeries: (from: string, to: string, groupBy: GroupBy = "day", currency?: string) =>
    api.get<RevenuePoint[]>("/admin/payments/reports/revenue-series", {
      params: { from, to, groupBy, currency },
    }),
};
