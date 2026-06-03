"use client";

import {
  Building2,
  CalendarCheck,
  Users,
  DollarSign,
  ClipboardCheck,
  CalendarRange,
} from "lucide-react";

import { StatCard } from "@/components/common/stat-card";
import type { DashboardStats } from "../types";

interface StatCardsGridProps {
  stats: DashboardStats;
}

const statCards = [
  {
    key: "totalProperties" as const,
    title: "Total Properties",
    icon: Building2,
    format: (v: number) => v.toLocaleString(),
  },
  {
    key: "totalBookings" as const,
    title: "Total Bookings",
    icon: CalendarCheck,
    format: (v: number) => v.toLocaleString(),
  },
  {
    key: "totalUsers" as const,
    title: "Total Users",
    icon: Users,
    format: (v: number) => v.toLocaleString(),
  },
  {
    key: "totalRevenue" as const,
    title: "Total Revenue",
    icon: DollarSign,
    format: (v: number) =>
      new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", notation: "compact" }).format(v),
  },
  {
    key: "pendingReviews" as const,
    title: "Pending Reviews",
    icon: ClipboardCheck,
    format: (v: number) => v.toLocaleString(),
  },
  {
    key: "activeBookings" as const,
    title: "Active Bookings",
    icon: CalendarRange,
    format: (v: number) => v.toLocaleString(),
  },
];

export function StatCardsGrid({ stats }: StatCardsGridProps) {
  return (
    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      {statCards.map(({ key, title, icon: Icon, format }) => (
        <StatCard
          key={key}
          title={title}
          value={format(stats[key])}
          icon={Icon}
        />
      ))}
    </div>
  );
}
