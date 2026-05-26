import { useState } from 'react';
import { useUserDetail, useApproveKYC, useRejectKYC } from '../hooks/useAdminUsers';
import { Sheet, SheetContent, SheetHeader, SheetTitle } from '@/components/ui/sheet';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { Avatar, AvatarImage, AvatarFallback } from '@/components/ui/avatar';
import { Separator } from '@/components/ui/separator';
import { Textarea } from '@/components/ui/textarea';
import { toast } from 'sonner';

interface UserDetailDrawerProps {
  userId: string | null;
  isOpen: boolean;
  onClose: () => void;
  onSuspend: (user: { id: string; fullName: string }) => void;
  onUnsuspend: (userId: string) => void;
}

export function UserDetailDrawer({
  userId,
  isOpen,
  onClose,
  onSuspend,
  onUnsuspend,
}: UserDetailDrawerProps) {
  const [rejectReason, setRejectReason] = useState('');
  const [pendingKycActions, setPendingKycActions] = useState<Set<string>>(new Set());

  const { data: user, isLoading, error } = useUserDetail(userId ?? '');
  const approveKycMutation = useApproveKYC();
  const rejectKycMutation = useRejectKYC();

  const handleApproveKYC = async () => {
    if (!userId) return;
    setPendingKycActions((prev) => new Set(prev).add('approve'));
    try {
      await approveKycMutation.mutateAsync({ userId, data: {} });
    } finally {
      setPendingKycActions((prev) => {
        const next = new Set(prev);
        next.delete('approve');
        return next;
      });
    }
  };

  const handleRejectKYC = async () => {
    if (!userId || !rejectReason.trim()) {
      toast.error('Please provide a rejection reason');
      return;
    }
    setPendingKycActions((prev) => new Set(prev).add('reject'));
    try {
      await rejectKycMutation.mutateAsync({
        userId,
        data: { reason: rejectReason },
      });
      setRejectReason('');
    } finally {
      setPendingKycActions((prev) => {
        const next = new Set(prev);
        next.delete('reject');
        return next;
      });
    }
  };

  const formatDate = (dateStr?: string) => {
    if (!dateStr) return 'N/A';
    return new Date(dateStr).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const getKycStatusBadge = () => {
    if (!user) return null;
    switch (user.kycStatus) {
      case 'Approved':
        return <Badge className="bg-green-500 text-white">Approved</Badge>;
      case 'Pending':
        return <Badge className="bg-yellow-500 text-black">Pending Review</Badge>;
      case 'Rejected':
        return <Badge variant="destructive">Rejected</Badge>;
      default:
        return <Badge variant="secondary">Not Submitted</Badge>;
    }
  };

  const getStatusBadgeVariant = () => {
    if (!user) return 'secondary';
    switch (user.status) {
      case 'Active':
        return 'secondary' as const;
      case 'Suspended':
        return 'outline' as const;
      case 'Banned':
        return 'destructive' as const;
      default:
        return 'secondary' as const;
    }
  };

  return (
    <Sheet open={isOpen} onOpenChange={(open) => !open && onClose()}>
      <SheetContent className="w-full sm:max-w-[540px] overflow-y-auto">
        <SheetHeader>
          <SheetTitle>User Details</SheetTitle>
        </SheetHeader>

        {isLoading && (
          <div className="space-y-4 mt-4">
            <div className="flex items-center gap-4">
              <Skeleton className="w-16 h-16 rounded-full" />
              <div className="flex-1 space-y-2">
                <Skeleton className="h-5 w-1/3" />
                <Skeleton className="h-4 w-1/2" />
              </div>
            </div>
            <Skeleton className="h-20 w-full" />
            <Skeleton className="h-20 w-full" />
          </div>
        )}

        {error && (
          <div className="p-8 text-center">
            <p className="text-red-500">Failed to load user details</p>
          </div>
        )}

        {user && (
          <div className="space-y-6 mt-4">
            {/* User Profile Section */}
            <div className="flex items-center gap-4">
              <Avatar className="w-16 h-16">
                {user.avatarUrl && <AvatarImage src={user.avatarUrl} alt={user.fullName} />}
                <AvatarFallback className="text-lg">
                  {user.fullName.slice(0, 2).toUpperCase()}
                </AvatarFallback>
              </Avatar>
              <div>
                <div className="flex items-center gap-2">
                  <h2 className="text-xl font-semibold">{user.fullName}</h2>
                  {user.isVerified && (
                    <span className="text-blue-500 text-lg" title="Verified">✓</span>
                  )}
                </div>
                <p className="text-sm text-muted-foreground">{user.email}</p>
                <div className="flex items-center gap-2 mt-1">
                  <Badge variant="secondary">{user.role}</Badge>
                  <Badge variant={getStatusBadgeVariant()}>
                    {user.status}
                  </Badge>
                </div>
              </div>
            </div>

            <Separator />

            {/* Basic Info */}
            <div className="space-y-3">
              <h3 className="font-semibold">Account Information</h3>
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <p className="text-muted-foreground">User ID</p>
                  <p className="font-mono text-xs">{user.id}</p>
                </div>
                <div>
                  <p className="text-muted-foreground">Phone</p>
                  <p>{user.phoneNumber || 'Not provided'}</p>
                </div>
                <div>
                  <p className="text-muted-foreground">Joined</p>
                  <p>{formatDate(user.createdAt)}</p>
                </div>
                <div>
                  <p className="text-muted-foreground">Last Updated</p>
                  <p>{formatDate(user.updatedAt)}</p>
                </div>
              </div>
              {user.bio && (
                <div>
                  <p className="text-muted-foreground">Bio</p>
                  <p className="text-sm">{user.bio}</p>
                </div>
              )}
            </div>

            <Separator />

            {/* KYC Section */}
            <div className="space-y-3">
              <div className="flex items-center justify-between">
                <h3 className="font-semibold">Identity Verification (KYC)</h3>
                {getKycStatusBadge()}
              </div>

              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <p className="text-muted-foreground">KYC Status</p>
                  <p>{user.kycStatus}</p>
                </div>
                <div>
                  <p className="text-muted-foreground">Submitted At</p>
                  <p>{formatDate(user.kycSubmittedAt)}</p>
                </div>
                {user.kycVerifiedAt && (
                  <div>
                    <p className="text-muted-foreground">Verified At</p>
                    <p>{formatDate(user.kycVerifiedAt)}</p>
                  </div>
                )}
              </div>

              {user.kycRejectionReason && (
                <div className="p-3 bg-destructive/10 rounded-lg">
                  <p className="text-sm text-destructive font-medium">Rejection Reason:</p>
                  <p className="text-sm">{user.kycRejectionReason}</p>
                </div>
              )}

              {/* KYC Actions */}
              {user.kycStatus === 'Pending' && (
                <div className="space-y-3 pt-2 border-t">
                  <p className="text-sm text-muted-foreground">Review this KYC submission:</p>
                  
                  <div className="space-y-2">
                    <Textarea
                      placeholder="Rejection reason (required for rejection)..."
                      value={rejectReason}
                      onChange={(e) => setRejectReason(e.target.value)}
                      rows={2}
                    />
                    <div className="flex gap-2">
                      <Button
                        size="sm"
                        className="flex-1 bg-green-600 hover:bg-green-700"
                        onClick={handleApproveKYC}
                        disabled={pendingKycActions.has('approve')}
                      >
                        {pendingKycActions.has('approve') ? 'Approving...' : '✓ Approve KYC'}
                      </Button>
                      <Button
                        size="sm"
                        variant="destructive"
                        className="flex-1"
                        onClick={handleRejectKYC}
                        disabled={pendingKycActions.has('reject') || !rejectReason.trim()}
                      >
                        {pendingKycActions.has('reject') ? 'Rejecting...' : '✗ Reject KYC'}
                      </Button>
                    </div>
                  </div>
                </div>
              )}

              {user.kycStatus === 'Approved' && user.isVerified && (
                <div className="flex items-center gap-2 p-3 bg-green-50 rounded-lg">
                  <span className="text-green-600 text-xl">✓</span>
                  <p className="text-sm text-green-700">
                    User is verified. Blue checkmark displayed.
                  </p>
                </div>
              )}
            </div>

            <Separator />

            {/* Stats Section */}
            <div className="space-y-3">
              <h3 className="font-semibold">Statistics</h3>
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div className="p-3 bg-muted rounded-lg">
                  <p className="text-muted-foreground">Total Bookings</p>
                  <p className="text-2xl font-bold">{user.totalBookings ?? 0}</p>
                </div>
                <div className="p-3 bg-muted rounded-lg">
                  <p className="text-muted-foreground">Total Properties</p>
                  <p className="text-2xl font-bold">{user.totalProperties ?? 0}</p>
                </div>
              </div>
            </div>

            <Separator />

            {/* Account Actions */}
            <div className="space-y-3">
              <h3 className="font-semibold">Account Actions</h3>
              <div className="flex gap-2">
                {user.status === 'Active' ? (
                  <Button
                    variant="outline"
                    className="flex-1 text-orange-600 border-orange-600 hover:bg-orange-50"
                    onClick={() => onSuspend({ id: user.id, fullName: user.fullName })}
                  >
                    Suspend Account
                  </Button>
                ) : (
                  <Button
                    variant="outline"
                    className="flex-1 text-green-600 border-green-600 hover:bg-green-50"
                    onClick={() => onUnsuspend(user.id)}
                  >
                    Unsuspend Account
                  </Button>
                )}
              </div>
              {user.status === 'Suspended' && (
                <p className="text-sm text-muted-foreground text-center">
                  User's account is currently suspended and cannot access the platform.
                </p>
              )}
            </div>
          </div>
        )}
      </SheetContent>
    </Sheet>
  );
}