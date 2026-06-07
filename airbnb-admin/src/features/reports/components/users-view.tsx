"use client";

import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip, Legend, Bar, BarChart, CartesianGrid, XAxis, YAxis } from "recharts";

import { ErrorDisplay } from "@/components/common/error-display";

import { useReportSummary, useUserActivity } from "../hooks";
import { ChartCard } from "./chart-card";
import { SummaryViewSkeleton } from "./skeletons";

interface UsersViewProps {
  from: string;
  to: string;
}

const STATUS_COLORS = ["#3a8b56", "#ffb400", "#c13515"];
const ROLE_COLORS = ["#222222", "#ff385c"];

export function UsersView({ from, to }: UsersViewProps) {
  const summary = useReportSummary(from, to);
  const activity = useUserActivity();

  if (summary.isLoading) return <SummaryViewSkeleton />;
  if (summary.isError || !summary.data) {
    return <ErrorDisplay message="Failed to load user reports." onRetry={() => summary.refetch()} />;
  }

  const s = summary.data;
  const statusData = [
    { name: "Active", value: s.activeUsers },
    { name: "Suspended", value: s.suspendedUsers },
    { name: "Banned", value: s.bannedUsers },
  ];
  const roleData = [
    { name: "Users", value: s.userCount },
    { name: "Admins", value: s.adminCount },
  ];

  const activityData = activity.data
    ? [
        { bucket: "Last 7d", count: activity.data.activeLast7Days },
        { bucket: "7–30d", count: activity.data.activeLast30Days },
        { bucket: "30–90d", count: activity.data.activeLast90Days },
        { bucket: "90d+", count: activity.data.inactiveOver90Days },
        { bucket: "Never", count: activity.data.neverLoggedIn },
      ]
    : [];

  return (
    <div className="space-y-6">
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <ChartCard title="Account Status" subtitle="Active / Suspended / Banned">
          <ResponsiveContainer width="100%" height={240}>
            <PieChart>
              <Pie
                data={statusData}
                dataKey="value"
                nameKey="name"
                innerRadius={50}
                outerRadius={80}
                paddingAngle={2}
              >
                {statusData.map((_, i) => (
                  <Cell key={i} fill={STATUS_COLORS[i]} />
                ))}
              </Pie>
              <Tooltip {...tooltipProps} />
              <Legend iconType="circle" wrapperStyle={{ fontSize: 12 }} />
            </PieChart>
          </ResponsiveContainer>
        </ChartCard>

        <ChartCard title="Role Distribution" subtitle="Users vs admins">
          <ResponsiveContainer width="100%" height={240}>
            <PieChart>
              <Pie
                data={roleData}
                dataKey="value"
                nameKey="name"
                innerRadius={50}
                outerRadius={80}
                paddingAngle={2}
              >
                {roleData.map((_, i) => (
                  <Cell key={i} fill={ROLE_COLORS[i]} />
                ))}
              </Pie>
              <Tooltip {...tooltipProps} />
              <Legend iconType="circle" wrapperStyle={{ fontSize: 12 }} />
            </PieChart>
          </ResponsiveContainer>
        </ChartCard>

        <ChartCard title="Headline" subtitle="Snapshot of the user base">
          <dl className="space-y-3 pt-2 text-sm">
            <Row label="Total users" value={s.totalUsers.toLocaleString()} />
            <Row label="New (range)" value={s.newUsers.toLocaleString()} highlight />
            <Row label="Active" value={s.activeUsers.toLocaleString()} />
            <Row label="Suspended" value={s.suspendedUsers.toLocaleString()} />
            <Row label="Banned" value={s.bannedUsers.toLocaleString()} />
          </dl>
        </ChartCard>
      </div>

      <ChartCard title="Login Activity" subtitle="How recently users have signed in">
        {activity.isLoading ? (
          <div className="h-[260px] animate-pulse rounded-[8px] bg-[#f7f7f7]" />
        ) : (
          <ResponsiveContainer width="100%" height={260}>
            <BarChart data={activityData} margin={{ left: -10, right: 8, top: 8, bottom: 0 }}>
              <CartesianGrid stroke="#f0f0f0" strokeDasharray="3 3" />
              <XAxis dataKey="bucket" tick={{ fontSize: 12, fill: "#6a6a6a" }} tickLine={false} axisLine={false} />
              <YAxis tick={{ fontSize: 11, fill: "#6a6a6a" }} tickLine={false} axisLine={false} />
              <Tooltip {...tooltipProps} />
              <Bar dataKey="count" name="Users" fill="#222222" radius={[6, 6, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        )}
      </ChartCard>
    </div>
  );
}

function Row({ label, value, highlight }: { label: string; value: string; highlight?: boolean }) {
  return (
    <div className="flex items-center justify-between">
      <dt className="text-[#6a6a6a]">{label}</dt>
      <dd className={highlight ? "font-semibold text-[#ff385c]" : "font-medium text-[#222222]"}>{value}</dd>
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
