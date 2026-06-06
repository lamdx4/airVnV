import { api } from "@/lib/api";

import type { ReportSummary, RevenueBreakdown, TopProperty, UserGrowthPoint } from "../types";

export const reportsApi = {
  getSummary: (from: string, to: string) =>
    api.get<ReportSummary>("/admin/reports/summary", {
      params: { from, to },
    }),

  getRevenueBreakdown: (from: string, to: string, groupBy: "day" | "week" | "month" = "day") =>
    api.get<RevenueBreakdown[]>("/admin/reports/revenue-breakdown", {
      params: { from, to, groupBy },
    }),

  getTopProperties: (from: string, to: string, limit = 10) =>
    api.get<TopProperty[]>("/admin/reports/top-properties", {
      params: { from, to, limit },
    }),

  getUserGrowth: (from: string, to: string, groupBy: "day" | "week" | "month" = "day") =>
    api.get<UserGrowthPoint[]>("/admin/reports/user-growth", {
      params: { from, to, groupBy },
    }),
};
