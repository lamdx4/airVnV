import { DashboardView } from "@/features/dashboard/components/dashboard-view";

export default function DashboardPage() {
  return (
    <div className="space-y-2">
      <h1 className="text-2xl font-semibold text-ink">Dashboard</h1>
      <DashboardView />
    </div>
  );
}
