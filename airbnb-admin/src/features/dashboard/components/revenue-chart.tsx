"use client";

import { useState } from "react";
import {
  Area,
  CartesianGrid,
  ComposedChart,
  Line,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { useRevenueChart } from "../hooks";
import type { RevenueChartPoint } from "../types";
import { RevenueChartSkeleton } from "./skeletons";

const RANGE_OPTIONS = [
  { label: "7d", days: 7 },
  { label: "14d", days: 14 },
  { label: "30d", days: 30 },
  { label: "90d", days: 90 },
] as const;

function formatCurrencyShort(value: number) {
  if (value >= 1_000_000) return `$${(value / 1_000_000).toFixed(1)}M`;
  if (value >= 1_000) return `$${(value / 1_000).toFixed(1)}K`;
  return `$${value}`;
}

function formatDateLabel(dateStr: string) {
  const d = new Date(dateStr);
  return d.toLocaleDateString("en-US", { month: "short", day: "numeric" });
}

interface ChartTooltipProps {
  active?: boolean;
  payload?: { value: number; dataKey: string }[];
  label?: string;
}

function ChartTooltip({ active, payload, label }: ChartTooltipProps) {
  if (!active || !payload?.length || !label) return null;

  const revenue = payload.find((p) => p.dataKey === "revenue")?.value ?? 0;
  const bookings = payload.find((p) => p.dataKey === "bookings")?.value ?? 0;

  return (
    <div className="rounded-lg border bg-card p-3 shadow-md">
      <p className="mb-1 text-xs font-medium text-muted-foreground">{label}</p>
      <p className="text-sm font-semibold text-ink">
        Revenue: {new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" }).format(revenue)}
      </p>
      <p className="text-sm text-muted-foreground">
        Bookings: {bookings.toLocaleString()}
      </p>
    </div>
  );
}

export function RevenueChart() {
  const [days, setDays] = useState(30);
  const { data, isLoading, isError, refetch } = useRevenueChart(days);

  if (isLoading) return <RevenueChartSkeleton />;

  if (isError) {
    return (
      <Card>
        <CardContent className="flex h-[380px] items-center justify-center">
          <div className="flex flex-col items-center gap-3">
            <p className="text-sm text-muted-foreground">Failed to load chart data.</p>
            <Button variant="outline" size="sm" onClick={() => refetch()}>
              Retry
            </Button>
          </div>
        </CardContent>
      </Card>
    );
  }

  const chartData: RevenueChartPoint[] = data ?? [];

  if (chartData.length === 0) {
    return (
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>Revenue & Bookings</CardTitle>
        </CardHeader>
        <CardContent className="flex h-[300px] items-center justify-center">
          <p className="text-sm text-muted-foreground">No data available for the selected period.</p>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle>Revenue & Bookings</CardTitle>
        <div className="flex gap-1">
          {RANGE_OPTIONS.map((opt) => (
            <Button
              key={opt.days}
              variant={days === opt.days ? "default" : "ghost"}
              size="sm"
              className={cn("h-7 px-3 text-xs", days === opt.days && "bg-ink text-white")}
              onClick={() => setDays(opt.days)}
            >
              {opt.label}
            </Button>
          ))}
        </div>
      </CardHeader>
      <CardContent>
        <ResponsiveContainer width="100%" height={300}>
          <ComposedChart data={chartData} margin={{ top: 5, right: 10, left: 0, bottom: 5 }}>
            <defs>
              <linearGradient id="revenueGradient" x1="0" y1="0" x2="0" y2="1">
                <stop offset="5%" stopColor="#ff385c" stopOpacity={0.3} />
                <stop offset="95%" stopColor="#ff385c" stopOpacity={0} />
              </linearGradient>
            </defs>
            <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#dddddd" />
            <XAxis
              dataKey="date"
              tickFormatter={formatDateLabel}
              tick={{ fontSize: 12, fill: "#717171" }}
              axisLine={false}
              tickLine={false}
            />
            <YAxis
              yAxisId="revenue"
              tickFormatter={formatCurrencyShort}
              tick={{ fontSize: 12, fill: "#717171" }}
              axisLine={false}
              tickLine={false}
              width={60}
            />
            <YAxis
              yAxisId="bookings"
              orientation="right"
              tick={{ fontSize: 12, fill: "#717171" }}
              axisLine={false}
              tickLine={false}
              width={40}
              allowDecimals={false}
            />
            <Tooltip content={<ChartTooltip />} />
            <Area
              yAxisId="revenue"
              type="monotone"
              dataKey="revenue"
              stroke="#ff385c"
              strokeWidth={2}
              fill="url(#revenueGradient)"
            />
            <Line
              yAxisId="bookings"
              type="monotone"
              dataKey="bookings"
              stroke="#222222"
              strokeWidth={2}
              dot={false}
              strokeDasharray="4 4"
            />
          </ComposedChart>
        </ResponsiveContainer>
      </CardContent>
    </Card>
  );
}
