"use client";

import {
  CartesianGrid,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
  Bar,
  BarChart,
} from "recharts";
import { Users, Building2, Clock, AlertTriangle } from "lucide-react";

import { StatCard } from "@/components/common/stat-card";
import { ErrorDisplay } from "@/components/common/error-display";

import {
  useNewListings,
  usePendingBacklog,
  useReportSummary,
  useStatusFunnel,
  useUserGrowth,
} from "../hooks";
import type { GroupBy } from "../types";
import { ChartCard } from "./chart-card";
import { SummaryViewSkeleton } from "./skeletons";

interface OverviewViewProps {
  from: string;
  to: string;
  groupBy: GroupBy;
}

export function OverviewView({ from, to, groupBy }: OverviewViewProps) {
  const summary = useReportSummary(from, to);
  const growth = useUserGrowth(from, to, groupBy);
  const newListings = useNewListings(from, to, groupBy);
  const funnel = useStatusFunnel();
  const backlog = usePendingBacklog();

  if (summary.isLoading) return <SummaryViewSkeleton />;
  if (summary.isError || !summary.data) {
    return <ErrorDisplay message="Failed to load overview." onRetry={() => summary.refetch()} />;
  }

  const s = summary.data;
  const published = funnel.data?.published ?? 0;
  const totalProperties = funnel.data
    ? funnel.data.draft +
      funnel.data.pendingReview +
      funnel.data.published +
      funnel.data.suspended +
      funnel.data.rejected +
      funnel.data.archived
    : 0;

  return (
    <div className="space-y-6">
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard title="Total Users" value={s.totalUsers.toLocaleString()} icon={Users} />
        <StatCard
          title="New Users (range)"
          value={s.newUsers.toLocaleString()}
          icon={Users}
        />
        <StatCard
          title="Published Listings"
          value={`${published.toLocaleString()} / ${totalProperties.toLocaleString()}`}
          icon={Building2}
        />
        <StatCard
          title="Pending Review"
          value={(backlog.data?.pendingCount ?? 0).toLocaleString()}
          icon={Clock}
          description={
            backlog.data && backlog.data.pendingCount > 0
              ? `avg wait ${backlog.data.averageWaitDays.toFixed(1)}d`
              : undefined
          }
        />
      </div>

      {backlog.data && backlog.data.overdueCount > 0 && (
        <div className="flex items-start gap-3 rounded-[14px] border border-[#fde2e1] bg-[#fff6f6] p-4">
          <AlertTriangle className="mt-0.5 h-5 w-5 flex-shrink-0 text-[#c13515]" />
          <div className="text-sm text-[#222222]">
            <span className="font-semibold">{backlog.data.overdueCount}</span> listing(s) have been
            pending review for more than {backlog.data.overdueThresholdDays} days.
          </div>
        </div>
      )}

      <div className="grid gap-4 lg:grid-cols-2">
        <ChartCard title="User Growth" subtitle="New + cumulative users over time">
          {growth.isLoading ? (
            <div className="h-[260px] animate-pulse rounded-[8px] bg-[#f7f7f7]" />
          ) : !growth.data || growth.data.length === 0 ? (
            <EmptyChart />
          ) : (
            <ResponsiveContainer width="100%" height={260}>
              <LineChart data={growth.data} margin={{ left: -10, right: 8, top: 8, bottom: 0 }}>
                <CartesianGrid stroke="#f0f0f0" strokeDasharray="3 3" />
                <XAxis dataKey="label" tick={{ fontSize: 11, fill: "#6a6a6a" }} tickLine={false} axisLine={false} />
                <YAxis tick={{ fontSize: 11, fill: "#6a6a6a" }} tickLine={false} axisLine={false} />
                <Tooltip {...tooltipProps} />
                <Line type="monotone" dataKey="newUsers" name="New users" stroke="#ff385c" strokeWidth={2} dot={false} />
                <Line type="monotone" dataKey="totalUsers" name="Total users" stroke="#222222" strokeWidth={2} dot={false} />
              </LineChart>
            </ResponsiveContainer>
          )}
        </ChartCard>

        <ChartCard title="New Listings" subtitle="Listings created in the selected range">
          {newListings.isLoading ? (
            <div className="h-[260px] animate-pulse rounded-[8px] bg-[#f7f7f7]" />
          ) : !newListings.data || newListings.data.length === 0 ? (
            <EmptyChart />
          ) : (
            <ResponsiveContainer width="100%" height={260}>
              <BarChart data={newListings.data} margin={{ left: -10, right: 8, top: 8, bottom: 0 }}>
                <CartesianGrid stroke="#f0f0f0" strokeDasharray="3 3" />
                <XAxis dataKey="label" tick={{ fontSize: 11, fill: "#6a6a6a" }} tickLine={false} axisLine={false} />
                <YAxis tick={{ fontSize: 11, fill: "#6a6a6a" }} tickLine={false} axisLine={false} />
                <Tooltip {...tooltipProps} />
                <Bar dataKey="newListings" name="New listings" fill="#ff385c" radius={[6, 6, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          )}
        </ChartCard>
      </div>
    </div>
  );
}

function EmptyChart() {
  return (
    <div className="flex h-[260px] items-center justify-center text-sm text-[#6a6a6a]">
      No data in the selected range
    </div>
  );
}

const tooltipProps = {
  contentStyle: {
    borderRadius: 8,
    border: "1px solid #dddddd",
    fontSize: 12,
  },
  labelStyle: { color: "#222222", fontWeight: 600 },
};
