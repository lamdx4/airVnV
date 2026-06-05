import { DashboardView } from "@/features/dashboard/components/dashboard-view";

export default function DashboardPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-[28px] font-bold text-[#222222]">Dashboard</h1>
        <p className="text-sm text-[#6a6a6a] mt-1">Overview of your platform</p>
      </div>
      <DashboardView />
    </div>
  );
}
