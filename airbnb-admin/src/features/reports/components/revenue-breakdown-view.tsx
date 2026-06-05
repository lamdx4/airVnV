"use client";

import { useState } from "react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  Legend,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { ErrorDisplay } from "@/components/common/error-display";
import { cn } from "@/lib/utils";

import { useRevenueBreakdown } from "../hooks";
import { RevenueBreakdownSkeleton } from "./skeletons";

interface RevenueBreakdownViewProps {
  from: string;
  to: string;
}

type GroupBy = "day" | "week" | "month";

const GROUP_OPTIONS: { label: string; value: GroupBy }[] = [
  { label: "Day", value: "day" },
  { label: "Week", value: "week" },
  { label: "Month", value: "month" },
];

function formatCurrency(value: number) {
  if (value >= 1_000_000) return `$${(value / 1_000_000).toFixed(1)}M`;
  if (value >= 1_000) return `$${(value / 1_000).toFixed(1)}K`;
  return `$${value}`;
}

export function RevenueBreakdownView({ from, to }: RevenueBreakdownViewProps) {
  const [groupBy, setGroupBy] = useState<GroupBy>("day");
  const { data, isLoading, isError, refetch } = useRevenueBreakdown(from, to, groupBy);

  if (isLoading) return <RevenueBreakdownSkeleton />;
  if (isError) {
    return (
      <Card>
        <CardContent className="flex h-[300px] flex-col items-center justify-center gap-3">
          <ErrorDisplay message="Failed to load revenue breakdown." onRetry={() => refetch()} />
        </CardContent>
      </Card>
    );
  }

  const chartData = data ?? [];

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle>Revenue Breakdown</CardTitle>
        <div className="flex gap-1">
          {GROUP_OPTIONS.map((opt) => (
            <Button
              key={opt.value}
              variant={groupBy === opt.value ? "default" : "ghost"}
              size="sm"
              className={cn("h-7 px-3 text-xs", groupBy === opt.value && "bg-[#222222] text-white")}
              onClick={() => setGroupBy(opt.value)}
            >
              {opt.label}
            </Button>
          ))}
        </div>
      </CardHeader>
      <CardContent>
        {chartData.length === 0 ? (
          <div className="flex h-[300px] items-center justify-center">
            <p className="text-sm text-[#6a6a6a]">No revenue activity in the selected period.</p>
          </div>
        ) : (
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={chartData} margin={{ top: 5, right: 10, left: 0, bottom: 5 }}>
              <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#dddddd" />
              <XAxis
                dataKey="period"
                tick={{ fontSize: 12, fill: "#717171" }}
                axisLine={false}
                tickLine={false}
              />
              <YAxis
                yAxisId="revenue"
                tickFormatter={formatCurrency}
                tick={{ fontSize: 12, fill: "#717171" }}
                axisLine={false}
                tickLine={false}
              />
              <YAxis
                yAxisId="count"
                orientation="right"
                tick={{ fontSize: 12, fill: "#717171" }}
                axisLine={false}
                tickLine={false}
                allowDecimals={false}
              />
              <Tooltip
                formatter={(value, name) => {
                  if (name === "revenue") return [formatCurrency(Number(value) || 0), "Revenue"];
                  if (name === "bookings") return [value, "Bookings"];
                  if (name === "cancellations") return [value, "Cancellations"];
                  return [value, name];
                }}
              />
              <Legend />
              <Bar yAxisId="revenue" dataKey="revenue" fill="#ff385c" name="Revenue" />
              <Bar yAxisId="count" dataKey="bookings" fill="#222222" name="Bookings" />
              <Bar yAxisId="count" dataKey="cancellations" fill="#c13515" name="Cancellations" />
            </BarChart>
          </ResponsiveContainer>
        )}
      </CardContent>
    </Card>
  );
}
