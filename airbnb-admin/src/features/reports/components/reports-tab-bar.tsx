"use client";

import { cn } from "@/lib/utils";

export type ReportsTab = "summary" | "revenue" | "top" | "growth";

interface ReportsTabBarProps {
  active: ReportsTab;
  onChange: (tab: ReportsTab) => void;
}

const TABS: { id: ReportsTab; label: string }[] = [
  { id: "summary", label: "Summary" },
  { id: "revenue", label: "Revenue Breakdown" },
  { id: "top", label: "Top Properties" },
  { id: "growth", label: "User Growth" },
];

export function ReportsTabBar({ active, onChange }: ReportsTabBarProps) {
  return (
    <div className="flex flex-wrap gap-1 border-b border-[#dddddd]">
      {TABS.map((t) => {
        const isActive = active === t.id;
        return (
          <button
            key={t.id}
            type="button"
            onClick={() => onChange(t.id)}
            className={cn(
              "px-4 py-2 text-sm font-medium transition-colors",
              "border-b-2 -mb-px",
              isActive
                ? "border-[#222222] text-[#222222]"
                : "border-transparent text-[#6a6a6a] hover:text-[#222222]"
            )}
          >
            {t.label}
          </button>
        );
      })}
    </div>
  );
}
