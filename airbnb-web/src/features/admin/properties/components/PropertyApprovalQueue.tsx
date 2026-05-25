import { useState } from 'react';
import { usePendingProperties, useApproveProperty, useRejectProperty } from '../hooks/useAdminPropertyModeration';
import { formatTimeSince } from '../utils/mappers';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { PropertyDetailDrawer } from './PropertyDetailDrawer';
import { Input } from '@/components/ui/input';
import { toast } from 'sonner';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';

interface PropertyApprovalQueueProps {
  onViewDetails?: (propertyId: string) => void;
}

export interface RejectModalState {
  isOpen: boolean;
  propertyId: string | null;
  reason: string;
}

export function PropertyApprovalQueue({ onViewDetails }: PropertyApprovalQueueProps) {
  const [page, setPage] = useState(1);
  const [selectedPropertyId, setSelectedPropertyId] = useState<string | null>(null);
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);
  const [pendingApprovals, setPendingApprovals] = useState<Set<string>>(new Set());
  const [pendingRejections, setPendingRejections] = useState<Set<string>>(new Set());
  const [rejectModal, setRejectModal] = useState<RejectModalState>({
    isOpen: false,
    propertyId: null,
    reason: '',
  });

  const { data, isLoading, error } = usePendingProperties({
    page,
    pageSize: 20,
  });

  const approveMutation = useApproveProperty();
  const rejectMutation = useRejectProperty();

  const handleViewDetails = (propertyId: string) => {
    setSelectedPropertyId(propertyId);
    setIsDrawerOpen(true);
    onViewDetails?.(propertyId);
  };

  const handleCloseDrawer = () => {
    setIsDrawerOpen(false);
    setSelectedPropertyId(null);
  };

  const handleApprove = async (propertyId: string) => {
    setPendingApprovals((prev) => new Set(prev).add(propertyId));
    try {
      await approveMutation.mutateAsync(propertyId);
      toast.success('Property approved and published!');
    } catch {
      toast.error('Failed to approve property');
    } finally {
      setPendingApprovals((prev) => {
        const next = new Set(prev);
        next.delete(propertyId);
        return next;
      });
    }
  };

  const handleRejectClick = (propertyId: string) => {
    setRejectModal({ isOpen: true, propertyId, reason: '' });
  };

  const handleRejectConfirm = async () => {
    if (!rejectModal.propertyId) return;
    const propertyId = rejectModal.propertyId;
    const reason = rejectModal.reason.trim() || undefined;

    setRejectModal((prev) => ({ ...prev, isOpen: false }));
    setPendingRejections((prev) => new Set(prev).add(propertyId));

    try {
      await rejectMutation.mutateAsync({ propertyId, reason });
      toast.success('Property rejected');
    } catch {
      toast.error('Failed to reject property');
    } finally {
      setPendingRejections((prev) => {
        const next = new Set(prev);
        next.delete(propertyId);
        return next;
      });
      setRejectModal({ isOpen: false, propertyId: null, reason: '' });
    }
  };

  const handleRejectCancel = () => {
    setRejectModal({ isOpen: false, propertyId: null, reason: '' });
  };

  if (isLoading) {
    return (
      <div className="space-y-4">
        {[...Array(5)].map((_, i) => (
          <Card key={i} className="p-4">
            <div className="flex gap-4">
              <Skeleton className="w-24 h-24 rounded-lg" />
              <div className="flex-1 space-y-2">
                <Skeleton className="h-5 w-3/4" />
                <Skeleton className="h-4 w-1/2" />
                <Skeleton className="h-4 w-1/4" />
              </div>
            </div>
          </Card>
        ))}
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-8 text-center">
        <p className="text-[#D93025]">Failed to load pending properties</p>
        <Button variant="outline" onClick={() => window.location.reload()} className="mt-4">
          Retry
        </Button>
      </div>
    );
  }

  if (!data?.items.length) {
    return (
      <div className="p-16 text-center">
        <div className="text-6xl mb-4">✓</div>
        <h3 className="text-xl font-semibold text-[#222222] mb-2">All caught up!</h3>
        <p className="text-[#717171]">No pending properties to review.</p>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-lg font-semibold text-[#222222]">
          {data.totalCount} {data.totalCount === 1 ? 'property' : 'properties'} pending review
        </h2>
      </div>

      <div className="space-y-3">
        {data.items.map((property) => (
          <Card key={property.id} className="p-4 hover:shadow-md transition-shadow duration-200">
            <div className="flex gap-4">
              <div
                className="w-24 h-24 rounded-lg bg-[#F7F7F7] bg-cover bg-center cursor-pointer"
                style={{ backgroundImage: property.thumbnailUrl ? `url(${property.thumbnailUrl})` : undefined }}
                onClick={() => handleViewDetails(property.id)}
              />
              <div className="flex-1 min-w-0">
                <div className="flex items-start justify-between gap-2">
                  <div className="min-w-0">
                    <h3
                      className="font-semibold text-[#222222] truncate cursor-pointer hover:text-[#FF5A5F]"
                      onClick={() => handleViewDetails(property.id)}
                    >
                      {property.title}
                    </h3>
                    <p className="text-sm text-[#717171]">Host: {property.hostName}</p>
                    <p className="text-sm text-[#717171]">Submitted {formatTimeSince(property.submittedAt)}</p>
                  </div>
                  <Badge variant="secondary" className="bg-[#FF5A5F] text-white">
                    Pending Review
                  </Badge>
                </div>
                <div className="flex gap-2 mt-3">
                  <Button
                    size="sm"
                    className="h-10 px-4 bg-[#FF5A5F] hover:bg-[#E31C5F] text-white rounded-xl"
                    onClick={() => handleApprove(property.id)}
                    disabled={pendingApprovals.has(property.id)}
                  >
                    {pendingApprovals.has(property.id) ? 'Approving...' : 'Approve'}
                  </Button>
                  <Button
                    size="sm"
                    variant="outline"
                    className="h-10 px-4 border-[#FF5A5F] text-[#FF5A5F] hover:bg-[#FFF5F5] rounded-xl"
                    onClick={() => handleRejectClick(property.id)}
                    disabled={pendingRejections.has(property.id)}
                  >
                    {pendingRejections.has(property.id) ? 'Rejecting...' : 'Reject'}
                  </Button>
                  <Button
                    size="sm"
                    variant="ghost"
                    className="h-10 px-4 text-[#717171] hover:text-[#222222] rounded-xl"
                    onClick={() => handleViewDetails(property.id)}
                  >
                    View Details
                  </Button>
                </div>
              </div>
            </div>
          </Card>
        ))}
      </div>

      {data.totalCount > data.pageSize && (
        <div className="flex justify-center gap-2 mt-6">
          <Button
            variant="outline"
            disabled={page === 1}
            onClick={() => setPage((p) => p - 1)}
            className="rounded-xl"
          >
            Previous
          </Button>
          <span className="flex items-center px-4 text-sm text-[#717171]">
            Page {page} of {Math.ceil(data.totalCount / data.pageSize)}
          </span>
          <Button
            variant="outline"
            disabled={page * data.pageSize >= data.totalCount}
            onClick={() => setPage((p) => p + 1)}
            className="rounded-xl"
          >
            Next
          </Button>
        </div>
      )}

      <PropertyDetailDrawer
        propertyId={selectedPropertyId}
        isOpen={isDrawerOpen}
        onClose={handleCloseDrawer}
        onApprove={handleApprove}
        onReject={handleRejectClick}
      />

      {/* Reject Reason Modal */}
      <Dialog open={rejectModal.isOpen} onOpenChange={(open) => !open && handleRejectCancel()}>
        <DialogContent className="sm:max-w-[425px]">
          <DialogHeader>
            <DialogTitle>Reject Property</DialogTitle>
            <DialogDescription>
              Please provide a reason for rejecting this property. This will be shared with the host.
            </DialogDescription>
          </DialogHeader>
          <div className="py-4">
            <Input
              placeholder="Rejection reason (optional)"
              value={rejectModal.reason}
              onChange={(e) => setRejectModal((prev) => ({ ...prev, reason: e.target.value }))}
              onKeyDown={(e) => e.key === 'Enter' && handleRejectConfirm()}
            />
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={handleRejectCancel}>
              Cancel
            </Button>
            <Button
              className="bg-[#FF5A5F] hover:bg-[#E31C5F] text-white"
              onClick={handleRejectConfirm}
              disabled={pendingRejections.has(rejectModal.propertyId ?? '')}
            >
              {pendingRejections.has(rejectModal.propertyId ?? '') ? 'Rejecting...' : 'Confirm Rejection'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}