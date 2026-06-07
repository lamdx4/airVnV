import { TrendingUp, TrendingDown } from "lucide-react";

import { cn } from "@/lib/utils";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

interface StatCardProps {
  title: string;
  value: string | number;
  description?: string;
  trend?: {
    value: number;
    isPositive: boolean;
  };
  icon?: React.ComponentType<{ className?: string }>;
}

export function StatCard({ title, value, description, trend, icon: Icon }: StatCardProps) {
  return (
    <Card className="shadow-none hover:shadow-[rgba(0,0,0,0.02)_0_0_0_1px,rgba(0,0,0,0.04)_0_2px_6px,rgba(0,0,0,0.1)_0_4px_8px] transition-shadow duration-200">
      <CardHeader className="flex flex-row items-center justify-between pb-2">
        <CardTitle className="text-sm font-medium text-[#6a6a6a]">{title}</CardTitle>
        {Icon && <Icon className="h-[18px] w-[18px] text-[#929292]" />}
      </CardHeader>
      <CardContent>
        <div className="text-2xl font-bold text-[#222222]">{value}</div>
        {(description || trend) && (
          <div className="flex items-center gap-1 text-xs text-[#6a6a6a] mt-1">
            {trend && (
              <span
                className={cn(
                  "flex items-center gap-0.5 font-medium",
                  trend.isPositive ? "text-emerald-600" : "text-[#ff385c]",
                )}
              >
                {trend.isPositive ? <TrendingUp className="h-3 w-3" /> : <TrendingDown className="h-3 w-3" />}
                {Math.abs(trend.value)}%
              </span>
            )}
            {description && <span>{description}</span>}
          </div>
        )}
      </CardContent>
    </Card>
  );
}
