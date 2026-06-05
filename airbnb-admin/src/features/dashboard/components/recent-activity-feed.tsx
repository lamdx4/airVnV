"use client";

import {
  Building2,
  CalendarCheck,
  CreditCard,
  Star,
  UserPlus,
} from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { useRecentActivity } from "../hooks";
import type { RecentActivity } from "../types";
import { RecentActivitySkeleton } from "./skeletons";

const ActivityTypeConfig = {
  booking:  { icon: CalendarCheck, color: "text-[#ff385c]",  bg: "bg-[#ff385c]/10"  },
  property: { icon: Building2,     color: "text-emerald-600", bg: "bg-emerald-50" },
  user:     { icon: UserPlus,      color: "text-[#460479]", bg: "bg-[#460479]/10" },
  payment:  { icon: CreditCard,    color: "text-amber-600",  bg: "bg-amber-50"  },
  review:   { icon: Star,          color: "text-[#92174d]",   bg: "bg-[#92174d]/10"   },
} as const;

type ActivityType = keyof typeof ActivityTypeConfig;

function getRelativeTime(timestamp: string): string {
  const now = Date.now();
  const then = new Date(timestamp).getTime();
  const diffMs = now - then;

  const seconds = Math.floor(diffMs / 1000);
  const minutes = Math.floor(seconds / 60);
  const hours = Math.floor(minutes / 60);
  const days = Math.floor(hours / 24);

  if (days > 0) return `${days}d ago`;
  if (hours > 0) return `${hours}h ago`;
  if (minutes > 0) return `${minutes}m ago`;
  return "just now";
}

function ActivityItem({ activity }: { activity: RecentActivity }) {
  const type = activity.type as ActivityType;
  const config = ActivityTypeConfig[type] ?? ActivityTypeConfig.booking;
  const Icon = config.icon;

  return (
    <div className="flex items-center gap-3 py-2">
      <div className={cn("flex h-8 w-8 shrink-0 items-center justify-center rounded-full", config.bg)}>
        <Icon className={cn("h-4 w-4", config.color)} />
      </div>
      <div className="min-w-0 flex-1">
        <p className="truncate text-sm text-[#222222]">{activity.description}</p>
        <p className="text-xs text-[#6a6a6a]">{getRelativeTime(activity.timestamp)}</p>
      </div>
    </div>
  );
}

export function RecentActivityFeed() {
  const { data, isLoading, isError, refetch } = useRecentActivity(10);

  if (isLoading) return <RecentActivitySkeleton />;

  if (isError) {
    return (
      <Card>
        <CardContent className="flex h-[300px] items-center justify-center">
          <div className="flex flex-col items-center gap-3">
            <p className="text-sm text-[#6a6a6a]">Failed to load recent activity.</p>
            <Button variant="outline" size="sm" onClick={() => refetch()}>
              Retry
            </Button>
          </div>
        </CardContent>
      </Card>
    );
  }

  const activities: RecentActivity[] = data ?? [];

  if (activities.length === 0) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Recent Activity</CardTitle>
        </CardHeader>
        <CardContent className="flex h-[200px] items-center justify-center">
          <p className="text-sm text-[#6a6a6a]">No recent activity to display.</p>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle>Recent Activity</CardTitle>
        <Button variant="ghost" size="sm" className="text-xs text-[#6a6a6a]">
          View All
        </Button>
      </CardHeader>
      <CardContent>
        <div className="divide-y divide-[#dddddd]">
          {activities.map((activity) => (
            <ActivityItem key={activity.id} activity={activity} />
          ))}
        </div>
      </CardContent>
    </Card>
  );
}
