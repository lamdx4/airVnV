"use client";

import { ErrorDisplay } from "@/components/common/error-display";
import { useDashboardStats } from "../hooks";
import { StatCardsGrid } from "./stat-cards-grid";
import { RevenueChart } from "./revenue-chart";
import { RecentActivityFeed } from "./recent-activity-feed";
import { StatCardSkeleton } from "./skeletons";

export function DashboardView() {
  const { data: stats, isLoading, isError, refetch } = useDashboardStats();

  return (
    <div className="space-y-6">
      {isLoading && (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {Array.from({ length: 6 }).map((_, i) => (
            <StatCardSkeleton key={i} />
          ))}
        </div>
      )}
      {isError && (
        <ErrorDisplay message="Failed to load dashboard stats." onRetry={() => refetch()} />
      )}
      {stats && <StatCardsGrid stats={stats} />}

      <RevenueChart />

      <RecentActivityFeed />
    </div>
  );
}
