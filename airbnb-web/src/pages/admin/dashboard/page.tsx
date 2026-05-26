import { useAdminDashboard } from '@/features/admin/dashboard/hooks/useAdminDashboard';
import { AdminDashboardStats } from '@/features/admin/dashboard/components/AdminDashboardStats';
import { useAdminDashboardFinance } from '@/features/admin/finance/hooks/useAdminDashboardFinance';
import { AdminDashboardFinance } from '@/features/admin/finance/components/AdminDashboardFinance';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';

export default function AdminDashboardPage() {
  const { stats, loading: statsLoading, error: statsError } = useAdminDashboard();
  const { finance, loading: financeLoading, error: financeError } = useAdminDashboardFinance();

  return (
    <div className="container mx-auto py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold">Admin Dashboard</h1>
        <p className="text-muted-foreground">Overview of platform metrics and performance</p>
      </div>

      <Tabs defaultValue="overview" className="space-y-6">
        <TabsList>
          <TabsTrigger value="overview">Overview</TabsTrigger>
          <TabsTrigger value="finance">Finance</TabsTrigger>
        </TabsList>

        <TabsContent value="overview" className="space-y-6">
          <AdminDashboardStats stats={stats} loading={statsLoading} error={statsError} />
        </TabsContent>

        <TabsContent value="finance" className="space-y-6">
          <AdminDashboardFinance finance={finance} loading={financeLoading} error={financeError} />
        </TabsContent>
      </Tabs>
    </div>
  );
}
