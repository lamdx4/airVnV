"use client";

import { useState } from "react";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { ErrorDisplay } from "@/components/common/error-display";
import { cn } from "@/lib/utils";

import { useTopProperties } from "../hooks";
import { TopPropertiesSkeleton } from "./skeletons";

interface TopPropertiesViewProps {
  from: string;
  to: string;
}

const LIMIT_OPTIONS = [10, 25, 50] as const;

function formatCurrency(value: number) {
  if (value >= 1_000_000) return `$${(value / 1_000_000).toFixed(1)}M`;
  if (value >= 1_000) return `$${(value / 1_000).toFixed(1)}K`;
  return `$${value.toLocaleString()}`;
}

function formatPercent(value: number) {
  return `${(value * 100).toFixed(1)}%`;
}

export function TopPropertiesView({ from, to }: TopPropertiesViewProps) {
  const [limit, setLimit] = useState<number>(10);
  const { data, isLoading, isError, refetch } = useTopProperties(from, to, limit);

  if (isLoading) return <TopPropertiesSkeleton />;
  if (isError) {
    return (
      <Card>
        <CardContent className="flex h-[300px] flex-col items-center justify-center gap-3">
          <ErrorDisplay message="Failed to load top properties." onRetry={() => refetch()} />
        </CardContent>
      </Card>
    );
  }

  const rows = data ?? [];

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle>Top Properties</CardTitle>
        <div className="flex items-center gap-2">
          <span className="text-xs text-[#6a6a6a]">Show:</span>
          {LIMIT_OPTIONS.map((n) => (
            <Button
              key={n}
              variant={limit === n ? "default" : "ghost"}
              size="sm"
              className={cn("h-7 px-3 text-xs", limit === n && "bg-[#222222] text-white")}
              onClick={() => setLimit(n)}
            >
              {n}
            </Button>
          ))}
        </div>
      </CardHeader>
      <CardContent>
        {rows.length === 0 ? (
          <div className="flex h-[200px] items-center justify-center">
            <p className="text-sm text-[#6a6a6a]">No properties with bookings in this period.</p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-[#dddddd] text-left text-xs uppercase text-[#6a6a6a]">
                  <th className="py-2 pr-4 font-medium">Title</th>
                  <th className="py-2 pr-4 text-right font-medium">Revenue</th>
                  <th className="py-2 pr-4 text-right font-medium">Bookings</th>
                  <th className="py-2 pr-0 text-right font-medium">Occupancy</th>
                </tr>
              </thead>
              <tbody>
                {rows.map((row) => (
                  <tr key={row.id} className="border-b border-[#dddddd] last:border-0">
                    <td className="py-3 pr-4">
                      <Link
                        href={`/properties/${row.id}`}
                        className="text-[#222222] hover:underline"
                      >
                        {row.title}
                      </Link>
                    </td>
                    <td className="py-3 pr-4 text-right text-[#222222]">{formatCurrency(row.revenue)}</td>
                    <td className="py-3 pr-4 text-right text-[#222222]">{row.bookings.toLocaleString()}</td>
                    <td className="py-3 pr-0 text-right text-[#222222]">{formatPercent(row.occupancyRate)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
