import { ReportsView } from "@/features/reports";

export default function ReportsPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-[28px] font-bold text-[#222222]">Reports</h1>
        <p className="text-sm text-[#6a6a6a] mt-1">Platform analytics and insights</p>
      </div>
      <ReportsView />
    </div>
  );
}
