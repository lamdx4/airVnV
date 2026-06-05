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

import { useUserGrowth } from "../hooks";
import { UserGrowthSkeleton } from "./skeletons";

interface UserGrowthViewProps {
  from: string;
  to: string;
}

type GroupBy = "day" | "week" | "month";

const GROUP_OPTIONS: { label: string; value: GroupBy }[] = [
  { label: "Day", value: "day" },
  { label: "Week", value: "week" },
  { label: "Month", value: "month" },
];

export function UserGrowthView({ from, to }: UserGrowthViewProps) {
  const [groupBy, setGroupBy] = useState<GroupBy>("day");
  const { data, isLoading, isError, refetch } = useUserGrowth(from, to, groupBy);

  if (isLoading) return <UserGrowthSkeleton />;
  if (isError) {
    return (
      <Card>
        <CardContent className="flex h-[300px] flex-col items-center justify-center gap-3">
          <ErrorDisplay message="Failed to load user growth." onRetry={() => refetch()} />
        </CardContent>
      </Card>
    );
  }

  const chartData = data ?? [];

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle>User Growth</CardTitle>
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
          <div className="flex h-[200px] items-center justify-center">
            <p className="text-sm text-[#6a6a6a]">No new users in this period.</p>
          </div>
        ) : (
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={chartData} margin={{ top: 5, right: 10, left: 0, bottom: 5 }}>
              <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#dddddd" />
              <XAxis
                dataKey="date"
                tick={{ fontSize: 12, fill: "#717171" }}
                axisLine={false}
                tickLine={false}
              />
              <YAxis
                tick={{ fontSize: 12, fill: "#717171" }}
                axisLine={false}
                tickLine={false}
                allowDecimals={false}
              />
              <Tooltip />
              <Legend />
              <Bar dataKey="guests" stackId="users" fill="#ff385c" name="Guests" />
              <Bar dataKey="hosts" stackId="users" fill="#460479" name="Hosts" />
            </BarChart>
          </ResponsiveContainer>
        )}
      </CardContent>
    </Card>
  );
}
