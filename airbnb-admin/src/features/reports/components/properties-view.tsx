"use client";

import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Legend,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import { AlertTriangle, Clock } from "lucide-react";

import { StatCard } from "@/components/common/stat-card";
import { ErrorDisplay } from "@/components/common/error-display";

import {
  usePendingBacklog,
  usePriceDistribution,
  useStatusFunnel,
  useTypeDistribution,
} from "../hooks";
import { ChartCard } from "./chart-card";
import { SummaryViewSkeleton } from "./skeletons";

const TYPE_COLORS = ["#ff385c", "#222222", "#ffb400", "#3a8b56", "#0078d4", "#8b5cf6"];

export function PropertiesView() {
  const funnel = useStatusFunnel();
  const backlog = usePendingBacklog();
  const types = useTypeDistribution();
  const prices = usePriceDistribution();

  if (funnel.isLoading) return <SummaryViewSkeleton />;
  if (funnel.isError || !funnel.data) {
    return <ErrorDisplay message="Failed to load property reports." onRetry={() => funnel.refetch()} />;
  }

  const f = funnel.data;
  const total = f.draft + f.pendingReview + f.published + f.suspended + f.rejected + f.archived;

  const statusData = [
    { name: "Draft", value: f.draft, color: "#929292" },
    { name: "Pending", value: f.pendingReview, color: "#ffb400" },
    { name: "Published", value: f.published, color: "#3a8b56" },
    { name: "Suspended", value: f.suspended, color: "#c13515" },
    { name: "Rejected", value: f.rejected, color: "#c13515" },
    { name: "Archived", value: f.archived, color: "#dddddd" },
  ];

  return (
    <div className="space-y-6">
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard title="Total Listings" value={total.toLocaleString()} />
        <StatCard title="Published" value={f.published.toLocaleString()} />
        <StatCard
          title="Pending Review"
          value={(backlog.data?.pendingCount ?? f.pendingReview).toLocaleString()}
          icon={Clock}
          description={
            backlog.data && backlog.data.pendingCount > 0
              ? `avg ${backlog.data.averageWaitDays.toFixed(1)}d • max ${backlog.data.maxWaitDays.toFixed(1)}d`
              : undefined
          }
        />
        <StatCard
          title="Overdue (>3d)"
          value={(backlog.data?.overdueCount ?? 0).toLocaleString()}
          icon={AlertTriangle}
        />
      </div>

      <div className="grid gap-4 lg:grid-cols-2">
        <ChartCard title="Status Funnel" subtitle="Distribution across moderation states">
          <ResponsiveContainer width="100%" height={280}>
            <BarChart
              data={statusData}
              layout="vertical"
              margin={{ left: 12, right: 16, top: 8, bottom: 0 }}
            >
              <CartesianGrid stroke="#f0f0f0" strokeDasharray="3 3" horizontal={false} />
              <XAxis type="number" tick={{ fontSize: 11, fill: "#6a6a6a" }} tickLine={false} axisLine={false} />
              <YAxis
                type="category"
                dataKey="name"
                tick={{ fontSize: 12, fill: "#222222" }}
                tickLine={false}
                axisLine={false}
                width={80}
              />
              <Tooltip {...tooltipProps} />
              <Bar dataKey="value" name="Listings" radius={[0, 6, 6, 0]}>
                {statusData.map((d, i) => (
                  <Cell key={i} fill={d.color} />
                ))}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </ChartCard>

        <ChartCard title="Property Type" subtitle="Listings grouped by category">
          {types.isLoading || !types.data ? (
            <div className="h-[280px] animate-pulse rounded-[8px] bg-[#f7f7f7]" />
          ) : (
            <ResponsiveContainer width="100%" height={280}>
              <PieChart>
                <Pie
                  data={types.data.filter((t) => t.count > 0)}
                  dataKey="count"
                  nameKey="type"
                  innerRadius={60}
                  outerRadius={100}
                  paddingAngle={2}
                >
                  {types.data.map((_, i) => (
                    <Cell key={i} fill={TYPE_COLORS[i % TYPE_COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip {...tooltipProps} />
                <Legend iconType="circle" wrapperStyle={{ fontSize: 12 }} />
              </PieChart>
            </ResponsiveContainer>
          )}
        </ChartCard>
      </div>

      <ChartCard
        title="Price Distribution"
        subtitle="Base price per night, published listings only"
        action={
          prices.data && prices.data.total > 0 ? (
            <div className="flex gap-4 text-xs text-[#6a6a6a]">
              <span>
                avg <span className="font-semibold text-[#222222]">${prices.data.average.toFixed(0)}</span>
              </span>
              <span>
                median <span className="font-semibold text-[#222222]">${prices.data.median.toFixed(0)}</span>
              </span>
              <span>
                p90 <span className="font-semibold text-[#222222]">${prices.data.p90.toFixed(0)}</span>
              </span>
            </div>
          ) : null
        }
      >
        {prices.isLoading || !prices.data ? (
          <div className="h-[260px] animate-pulse rounded-[8px] bg-[#f7f7f7]" />
        ) : prices.data.total === 0 ? (
          <div className="flex h-[260px] items-center justify-center text-sm text-[#6a6a6a]">
            No published listings
          </div>
        ) : (
          <ResponsiveContainer width="100%" height={260}>
            <BarChart data={prices.data.buckets} margin={{ left: -10, right: 8, top: 8, bottom: 0 }}>
              <CartesianGrid stroke="#f0f0f0" strokeDasharray="3 3" />
              <XAxis dataKey="label" tick={{ fontSize: 11, fill: "#6a6a6a" }} tickLine={false} axisLine={false} />
              <YAxis tick={{ fontSize: 11, fill: "#6a6a6a" }} tickLine={false} axisLine={false} />
              <Tooltip {...tooltipProps} />
              <Bar dataKey="count" name="Listings" fill="#ff385c" radius={[6, 6, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        )}
      </ChartCard>
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
