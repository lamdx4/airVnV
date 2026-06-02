import { api } from "@/lib/api";
import type { ApiResponse } from "@/types/api";

export interface ReportSummary {
  totalRevenue: number;
  totalBookings: number;
  averageBookingValue: number;
  occupancyRate: number;
  newUsers: number;
  newProperties: number;
}

export interface RevenueBreakdown {
  period: string;
  revenue: number;
  bookings: number;
  cancellations: number;
}

export interface TopProperty {
  id: string;
  title: string;
  revenue: number;
  bookings: number;
  occupancyRate: number;
}

export interface UserGrowthPoint {
  date: string;
  guests: number;
  hosts: number;
}

export const reportsApi = {
  getSummary: (from: string, to: string) =>
    api.get<ApiResponse<ReportSummary>>("/admin/reports/summary", {
      params: { from, to },
    }),

  getRevenueBreakdown: (from: string, to: string, groupBy: "day" | "week" | "month" = "day") =>
    api.get<ApiResponse<RevenueBreakdown[]>>("/admin/reports/revenue-breakdown", {
      params: { from, to, groupBy },
    }),

  getTopProperties: (from: string, to: string, limit = 10) =>
    api.get<ApiResponse<TopProperty[]>>("/admin/reports/top-properties", {
      params: { from, to, limit },
    }),

  getUserGrowth: (from: string, to: string) =>
    api.get<ApiResponse<UserGrowthPoint[]>>("/admin/reports/user-growth", {
      params: { from, to },
    }),
};
