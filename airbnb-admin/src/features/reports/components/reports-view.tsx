"use client";

import { useState } from "react";

import { DateRangePicker } from "./date-range-picker";
import { OverviewView } from "./overview-view";
import { PropertiesView } from "./properties-view";
import { ReportsTabBar, type ReportsTab } from "./reports-tab-bar";
import { UsersView } from "./users-view";
import type { GroupBy } from "../types";

function daysAgoIso(days: number): string {
  const d = new Date();
  d.setDate(d.getDate() - days);
  return d.toISOString().slice(0, 10);
}

const GROUP_OPTIONS: { id: GroupBy; label: string }[] = [
  { id: "day", label: "Day" },
  { id: "week", label: "Week" },
  { id: "month", label: "Month" },
];

export function ReportsView() {
  const [activeTab, setActiveTab] = useState<ReportsTab>("overview");
  const [from, setFrom] = useState<string>(daysAgoIso(30));
  const [to, setTo] = useState<string>(daysAgoIso(0));
  const [groupBy, setGroupBy] = useState<GroupBy>("day");

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
        <DateRangePicker
          from={from}
          to={to}
          onChange={({ from: f, to: t }) => {
            setFrom(f);
            setTo(t);
          }}
        />
        {activeTab === "overview" && (
          <div className="flex items-center gap-1 rounded-[10px] border border-[#dddddd] bg-white p-1">
            {GROUP_OPTIONS.map((g) => (
              <button
                key={g.id}
                type="button"
                onClick={() => setGroupBy(g.id)}
                className={
                  "rounded-[8px] px-3 py-1.5 text-xs font-medium transition-colors " +
                  (groupBy === g.id
                    ? "bg-[#222222] text-white"
                    : "text-[#6a6a6a] hover:text-[#222222]")
                }
              >
                {g.label}
              </button>
            ))}
          </div>
        )}
      </div>

      <ReportsTabBar active={activeTab} onChange={setActiveTab} />

      <div>
        {activeTab === "overview" && <OverviewView from={from} to={to} groupBy={groupBy} />}
        {activeTab === "users" && <UsersView from={from} to={to} />}
        {activeTab === "properties" && <PropertiesView />}
      </div>
    </div>
  );
}
