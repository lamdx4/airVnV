"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Button } from "@/components/ui/button";

import { DateRangePicker } from "./date-range-picker";
import { ReportsTabBar, type ReportsTab } from "./reports-tab-bar";
import { SummaryView } from "./summary-view";
import { RevenueBreakdownView } from "./revenue-breakdown-view";
import { TopPropertiesView } from "./top-properties-view";
import { UserGrowthView } from "./user-growth-view";

function daysAgoIso(days: number): string {
  const d = new Date();
  d.setDate(d.getDate() - days);
  return d.toISOString().slice(0, 10);
}

export function ReportsView() {
  const router = useRouter();
  const [activeTab, setActiveTab] = useState<ReportsTab>("summary");
  const [from, setFrom] = useState<string>(daysAgoIso(30));
  const [to, setTo] = useState<string>(daysAgoIso(0));

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
        <DateRangePicker from={from} to={to} onChange={({ from: f, to: t }) => { setFrom(f); setTo(t); }} />
        {activeTab === "summary" && (
          <Button
            variant="ghost"
            size="sm"
            className="text-xs text-[#6a6a6a]"
            onClick={() => router.push("/dashboard")}
          >
            View All Activity →
          </Button>
        )}
        {activeTab !== "summary" && (
          <Link
            href="/dashboard"
            className="text-xs text-[#6a6a6a] hover:text-[#222222] hover:underline"
          >
            View All Activity →
          </Link>
        )}
      </div>

      <ReportsTabBar active={activeTab} onChange={setActiveTab} />

      <div>
        {activeTab === "summary" && <SummaryView from={from} to={to} />}
        {activeTab === "revenue" && <RevenueBreakdownView from={from} to={to} />}
        {activeTab === "top" && <TopPropertiesView from={from} to={to} />}
        {activeTab === "growth" && <UserGrowthView from={from} to={to} />}
      </div>
    </div>
  );
}
