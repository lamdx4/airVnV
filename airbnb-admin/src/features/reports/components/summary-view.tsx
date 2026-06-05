"use client";

import { DollarSign, CalendarCheck, TrendingUp, Percent, UserPlus, Building2 } from "lucide-react";

import { StatCard } from "@/components/common/stat-card";
import { ErrorDisplay } from "@/components/common/error-display";

import { useReportSummary } from "../hooks";
import { SummaryViewSkeleton } from "./skeletons";

interface SummaryViewProps {
  from: string;
  to: string;
}

function formatCurrencyCompact(value: number) {
  if (value >= 1_000_000) return `$${(value / 1_000_000).toFixed(1)}M`;
  if (value >= 1_000) return `$${(value / 1_000).toFixed(1)}K`;
  return `$${value}`;
}

function formatCurrency(value: number) {
  return new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", maximumFractionDigits: 0 }).format(value);
}

function formatPercent(value: number) {
  return `${(value * 100).toFixed(1)}%`;
}

export function SummaryView({ from, to }: SummaryViewProps) {
  const { data, isLoading, isError, refetch } = useReportSummary(from, to);

  if (isLoading) return <SummaryViewSkeleton />;
  if (isError) return <ErrorDisplay message="Failed to load report summary." onRetry={() => refetch()} />;
  if (!data) return null;

  return (
    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      <StatCard
        title="Total Revenue"
        value={formatCurrencyCompact(data.totalRevenue)}
        icon={DollarSign}
      />
      <StatCard
        title="Total Bookings"
        value={data.totalBookings.toLocaleString()}
        icon={CalendarCheck}
      />
      <StatCard
        title="Average Booking Value"
        value={formatCurrency(data.averageBookingValue)}
        icon={TrendingUp}
      />
      <StatCard
        title="Occupancy Rate"
        value={formatPercent(data.occupancyRate)}
        icon={Percent}
      />
      <StatCard
        title="New Users"
        value={data.newUsers.toLocaleString()}
        icon={UserPlus}
      />
      <StatCard
        title="New Properties"
        value={data.newProperties.toLocaleString()}
        icon={Building2}
      />
    </div>
  );
}
