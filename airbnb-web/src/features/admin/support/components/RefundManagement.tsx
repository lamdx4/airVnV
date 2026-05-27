import { useEffect, useState } from 'react';
import { useSupportRefunds } from '../hooks/useSupportRefunds';
import type { RefundDetail } from '../types';
import type { GetRefundsParams } from '../api/supportApi';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from '@/components/ui/dialog';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { formatDistanceToNow } from 'date-fns';

const statusColors: Record<string, string> = {
  Pending: 'bg-yellow-500',
  Processing: 'bg-blue-500',
  Completed: 'bg-green-500',
  Failed: 'bg-red-500',
  Cancelled: 'bg-gray-500',
};

const typeLabels: Record<string, string> = {
  Full: 'Full Refund',
  Partial: 'Partial Refund',
  Cancellation: 'Cancellation',
  GuestCompensation: 'Guest Compensation',
  HostCompensation: 'Host Compensation',
  Chargeback: 'Chargeback',
};

interface RefundManagementProps {
  onViewDetails?: (refund: RefundDetail) => void;
}

export function RefundManagement({ onViewDetails }: RefundManagementProps) {
  const { data, loading, error, fetchRefunds, processRefund } = useSupportRefunds();
  const [filters, setFilters] = useState<GetRefundsParams>({
    status: '',
    type: '',
    sortBy: 'CreatedAt',
    sortOrder: 'desc',
    page: 1,
  });
  const [actionDialog, setActionDialog] = useState<{
    open: boolean;
    refund: RefundDetail | null;
    action: 'approve' | 'reject' | 'process';
  }>({
    open: false,
    refund: null,
    action: 'approve',
  });
  const [rejectionReason, setRejectionReason] = useState('');
  const [processingId, setProcessingId] = useState<string | null>(null);

  useEffect(() => {
    fetchRefunds(filters);
  }, []);

  const handleFilterChange = <K extends keyof GetRefundsParams>(key: K, value: GetRefundsParams[K]) => {
    setFilters(prev => ({ ...prev, [key]: value }));
    fetchRefunds({ ...filters, [key]: value });
  };

  const handlePageChange = (newPage: number) => {
    setFilters(prev => ({ ...prev, page: newPage }));
    fetchRefunds({ ...filters, page: newPage });
  };

  const handleAction = async () => {
    if (!actionDialog.refund) return;
    setProcessingId(actionDialog.refund.id);
    try {
      await processRefund(actionDialog.refund.id, actionDialog.action, rejectionReason);
      setActionDialog({ open: false, refund: null, action: 'approve' });
      setRejectionReason('');
      fetchRefunds(filters);
    } finally {
      setProcessingId(null);
    }
  };

  if (error) {
    return (
      <Card>
        <CardContent className="p-6">
          <p className="text-red-500">Error: {error}</p>
        </CardContent>
      </Card>
    );
  }

  const stats = data?.stats;
  const refunds = data?.items || [];
  const totalCount = data?.totalCount || 0;

  return (
    <div className="space-y-4">
      {/* Stats Summary */}
      {stats && (
        <div className="grid grid-cols-5 gap-4">
          <StatCard label="Pending" value={stats.totalPending} color="text-yellow-600" amount={stats.totalPendingAmount} />
          <StatCard label="Processing" value={stats.totalProcessing} color="text-blue-600" />
          <StatCard label="Completed" value={stats.totalCompleted} color="text-green-600" />
          <StatCard label="Failed" value={stats.totalFailed} color="text-red-600" />
          <Card className="col-span-1">
            <CardContent className="p-4 text-center">
              <div className="text-lg font-semibold text-gray-900">
                {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(stats.totalPendingAmount)}
              </div>
              <div className="text-sm text-gray-500">Pending Amount</div>
            </CardContent>
          </Card>
        </div>
      )}

      {/* Filters */}
      <Card>
        <CardContent className="p-4">
          <div className="flex flex-wrap gap-4">
            <Select value={filters.status || ''} onValueChange={v => handleFilterChange('status', v)}>
              <SelectTrigger className="w-40">
                <SelectValue placeholder="Status" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">All Status</SelectItem>
                <SelectItem value="Pending">Pending</SelectItem>
                <SelectItem value="Processing">Processing</SelectItem>
                <SelectItem value="Completed">Completed</SelectItem>
                <SelectItem value="Failed">Failed</SelectItem>
                <SelectItem value="Cancelled">Cancelled</SelectItem>
              </SelectContent>
            </Select>
            <Select value={filters.type || ''} onValueChange={v => handleFilterChange('type', v)}>
              <SelectTrigger className="w-44">
                <SelectValue placeholder="Type" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">All Types</SelectItem>
                <SelectItem value="Full">Full Refund</SelectItem>
                <SelectItem value="Partial">Partial Refund</SelectItem>
                <SelectItem value="Cancellation">Cancellation</SelectItem>
                <SelectItem value="GuestCompensation">Guest Compensation</SelectItem>
                <SelectItem value="HostCompensation">Host Compensation</SelectItem>
                <SelectItem value="Chargeback">Chargeback</SelectItem>
              </SelectContent>
            </Select>
            <Select value={filters.sortOrder || 'desc'} onValueChange={v => handleFilterChange('sortOrder', v as 'asc' | 'desc')}>
              <SelectTrigger className="w-36">
                <SelectValue placeholder="Order" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="desc">Newest First</SelectItem>
                <SelectItem value="asc">Oldest First</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </CardContent>
      </Card>

      {/* Table */}
      <Card>
        <CardHeader>
          <CardTitle>Refund Requests ({totalCount})</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Booking ID</TableHead>
                <TableHead>Type</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Amount</TableHead>
                <TableHead>Guest</TableHead>
                <TableHead>Host</TableHead>
                <TableHead>Requested</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {loading ? (
                <TableRow>
                  <TableCell colSpan={8} className="text-center py-8">
                    Loading...
                  </TableCell>
                </TableRow>
              ) : refunds.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={8} className="text-center py-8 text-gray-500">
                    No refunds found
                  </TableCell>
                </TableRow>
              ) : (
                refunds.map(refund => (
                  <TableRow key={refund.id}>
                    <TableCell className="font-mono text-sm">{refund.bookingId.slice(0, 8)}...</TableCell>
                    <TableCell>
                      <Badge variant="outline">{typeLabels[refund.type] || refund.type}</Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <div className={`w-2 h-2 rounded-full ${statusColors[refund.status]}`} />
                        <span>{refund.status}</span>
                      </div>
                    </TableCell>
                    <TableCell className="font-semibold">
                      {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(refund.requestedAmount)}
                    </TableCell>
                    <TableCell className="font-mono text-sm">{refund.guestId.slice(0, 8)}...</TableCell>
                    <TableCell className="font-mono text-sm">{refund.hostId.slice(0, 8)}...</TableCell>
                    <TableCell>{formatDistanceToNow(new Date(refund.createdAt), { addSuffix: true })}</TableCell>
                    <TableCell className="text-right">
                      <div className="flex justify-end gap-2">
                        <Button variant="ghost" size="sm" onClick={() => onViewDetails?.(refund)}>
                          Details
                        </Button>
                        {refund.status === 'Pending' && (
                          <>
                            <Button
                              variant="outline"
                              size="sm"
                              className="text-green-600"
                              onClick={() => setActionDialog({ open: true, refund, action: 'approve' })}
                            >
                              Approve
                            </Button>
                            <Button
                              variant="outline"
                              size="sm"
                              className="text-red-600"
                              onClick={() => setActionDialog({ open: true, refund, action: 'reject' })}
                            >
                              Reject
                            </Button>
                          </>
                        )}
                        {refund.status === 'Processing' && (
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => setActionDialog({ open: true, refund, action: 'process' })}
                          >
                            Process
                          </Button>
                        )}
                      </div>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>

          {/* Pagination */}
          {totalCount > 0 && (
            <div className="flex justify-between items-center mt-4">
              <span className="text-sm text-gray-500">
                Showing {(filters.page! - 1) * 20 + 1} - {Math.min(filters.page! * 20, totalCount)} of {totalCount}
              </span>
              <div className="flex gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  disabled={filters.page === 1}
                  onClick={() => handlePageChange(filters.page! - 1)}
                >
                  Previous
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  disabled={filters.page! * 20 >= totalCount}
                  onClick={() => handlePageChange(filters.page! + 1)}
                >
                  Next
                </Button>
              </div>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Action Dialog */}
      <Dialog open={actionDialog.open} onOpenChange={open => setActionDialog(prev => ({ ...prev, open }))}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>
              {actionDialog.action === 'approve' && 'Approve Refund'}
              {actionDialog.action === 'reject' && 'Reject Refund'}
              {actionDialog.action === 'process' && 'Process Refund'}
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            {actionDialog.refund && (
              <div className="space-y-2 text-sm">
                <p>
                  <strong>Amount:</strong>{' '}
                  {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(actionDialog.refund.requestedAmount)}
                </p>
                <p>
                  <strong>Type:</strong> {typeLabels[actionDialog.refund.type] || actionDialog.refund.type}
                </p>
                <p>
                  <strong>Reason:</strong> {actionDialog.refund.reason}
                </p>
              </div>
            )}
            {actionDialog.action === 'reject' && (
              <div>
                <label className="text-sm font-medium">Rejection Reason</label>
                <textarea
                  className="mt-1 w-full border rounded-md p-2 text-sm"
                  rows={3}
                  value={rejectionReason}
                  onChange={e => setRejectionReason(e.target.value)}
                  placeholder="Enter reason for rejection..."
                />
              </div>
            )}
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setActionDialog(prev => ({ ...prev, open: false }))}>
              Cancel
            </Button>
            <Button
              onClick={handleAction}
              disabled={processingId !== null}
              className={actionDialog.action === 'reject' ? 'bg-red-600 hover:bg-red-700' : 'bg-green-600 hover:bg-green-700'}
            >
              {processingId !== null ? 'Processing...' : 'Confirm'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}

function StatCard({ label, value, color, amount }: { label: string; value: number; color: string; amount?: number }) {
  return (
    <Card>
      <CardContent className="p-4 text-center">
        <div className={`text-2xl font-bold ${color}`}>{value}</div>
        <div className="text-sm text-gray-500">{label}</div>
        {amount !== undefined && (
          <div className="text-lg font-semibold text-gray-900 mt-1">
            {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount)}
          </div>
        )}
      </CardContent>
    </Card>
  );
}
