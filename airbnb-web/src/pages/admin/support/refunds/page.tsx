import { RefundManagement } from '@/features/admin/support/components/RefundManagement';

export default function RefundsPage() {
  return (
    <div className="container mx-auto py-6 space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-3xl font-bold">Refund Management</h1>
      </div>

      <RefundManagement />
    </div>
  );
}