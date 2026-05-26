import { useState } from 'react';
import { useUsers, useSuspendUser, useUnsuspendUser } from '../hooks/useAdminUsers';
import type { GetUsersParams } from '../hooks/useAdminUsers';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { Input } from '@/components/ui/input';
import { UserDetailDrawer } from './UserDetailDrawer';
import { SuspendUserModal } from './SuspendUserModal';
import { Avatar, AvatarImage, AvatarFallback } from '@/components/ui/avatar';
import { AiSearchIcon } from 'hugeicons-react';
import type { UserRole, UserStatus } from '../types';

export function UserManagement() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [roleFilter, setRoleFilter] = useState<string>('');
  const [statusFilter, setStatusFilter] = useState<string>('');
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);
  const [suspendModal, setSuspendModal] = useState<{
    isOpen: boolean;
    userId: string | null;
    userName: string;
  }>({
    isOpen: false,
    userId: null,
    userName: '',
  });
  const [pendingActions, setPendingActions] = useState<Set<string>>(new Set());

  const params: GetUsersParams = {
    page,
    pageSize: 20,
    search: search || undefined,
    role: roleFilter || undefined,
    status: statusFilter || undefined,
  };

  const { data, isLoading, error, refetch } = useUsers(params);
  const suspendMutation = useSuspendUser();
  const unsuspendMutation = useUnsuspendUser();

  const handleViewDetails = (userId: string) => {
    setSelectedUserId(userId);
    setIsDrawerOpen(true);
  };

  const handleCloseDrawer = () => {
    setIsDrawerOpen(false);
    setSelectedUserId(null);
  };

  const handleSuspendClick = (user: { id: string; fullName: string }) => {
    setSuspendModal({ isOpen: true, userId: user.id, userName: user.fullName });
  };

  const handleSuspendConfirm = async (reason: string, durationDays?: number) => {
    if (!suspendModal.userId) return;
    setPendingActions((prev) => new Set(prev).add(suspendModal.userId!));
    try {
      await suspendMutation.mutateAsync({
        userId: suspendModal.userId,
        data: { reason, durationDays },
      });
      setSuspendModal({ isOpen: false, userId: null, userName: '' });
    } finally {
      setPendingActions((prev) => {
        const next = new Set(prev);
        next.delete(suspendModal.userId!);
        return next;
      });
    }
  };

  const handleUnsuspend = async (userId: string) => {
    setPendingActions((prev) => new Set(prev).add(userId));
    try {
      await unsuspendMutation.mutateAsync(userId);
    } finally {
      setPendingActions((prev) => {
        const next = new Set(prev);
        next.delete(userId);
        return next;
      });
    }
  };

  const getRoleBadgeVariant = (role: UserRole) => {
    switch (role) {
      case 'Admin':
        return 'destructive' as const;
      case 'Moderator':
        return 'default' as const;
      default:
        return 'secondary' as const;
    }
  };

  const getStatusBadgeVariant = (status: UserStatus) => {
    switch (status) {
      case 'Active':
        return 'secondary' as const;
      case 'Suspended':
        return 'outline' as const;
      case 'Banned':
        return 'destructive' as const;
      case 'PendingVerification':
        return 'secondary' as const;
      default:
        return 'secondary' as const;
    }
  };

  const getKYCStatusBadge = (kycStatus: string) => {
    switch (kycStatus) {
      case 'Approved':
        return <Badge className="bg-green-500 text-white">Verified</Badge>;
      case 'Pending':
        return <Badge className="bg-yellow-500 text-black">Pending</Badge>;
      case 'Rejected':
        return <Badge variant="destructive">Rejected</Badge>;
      default:
        return <Badge variant="secondary">Not Submitted</Badge>;
    }
  };

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
    });
  };

  if (isLoading) {
    return (
      <div className="space-y-4">
        {[...Array(5)].map((_, i) => (
          <Card key={i} className="p-4">
            <div className="flex items-center gap-4">
              <Skeleton className="w-12 h-12 rounded-full" />
              <div className="flex-1 space-y-2">
                <Skeleton className="h-5 w-1/3" />
                <Skeleton className="h-4 w-1/2" />
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
        <p className="text-red-500">Failed to load users</p>
        <Button variant="outline" onClick={() => refetch()} className="mt-4">
          Retry
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header & Filters */}
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <h2 className="text-xl font-semibold text-[#222222]">
          User Management
          {data?.totalCount !== undefined && (
            <span className="ml-2 text-sm font-normal text-[#717171]">
              ({data.totalCount} users)
            </span>
          )}
        </h2>

        <div className="flex flex-wrap gap-2">
          {/* Search */}
          <div className="relative">
            <AiSearchIcon className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Search users..."
              value={search}
              onChange={(e) => {
                setSearch(e.target.value);
                setPage(1);
              }}
              className="pl-9 w-48"
            />
          </div>

          {/* Role Filter */}
          <select
            value={roleFilter}
            onChange={(e) => {
              setRoleFilter(e.target.value);
              setPage(1);
            }}
            className="h-10 px-3 rounded-lg border border-input bg-background text-sm"
          >
            <option value="">All Roles</option>
            <option value="User">User</option>
            <option value="Moderator">Moderator</option>
            <option value="Admin">Admin</option>
          </select>

          {/* Status Filter */}
          <select
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value);
              setPage(1);
            }}
            className="h-10 px-3 rounded-lg border border-input bg-background text-sm"
          >
            <option value="">All Status</option>
            <option value="Active">Active</option>
            <option value="Suspended">Suspended</option>
            <option value="Banned">Banned</option>
            <option value="PendingVerification">Pending</option>
          </select>
        </div>
      </div>

      {/* User List */}
      {!data?.items.length ? (
        <div className="p-16 text-center">
          <div className="text-6xl mb-4">👥</div>
          <h3 className="text-xl font-semibold text-[#222222] mb-2">No users found</h3>
          <p className="text-[#717171]">Try adjusting your search or filters.</p>
        </div>
      ) : (
        <div className="space-y-3">
          {data.items.map((user) => (
            <Card
              key={user.id}
              className="p-4 hover:shadow-md transition-shadow duration-200"
            >
              <div className="flex items-center gap-4">
                <Avatar className="w-12 h-12">
                  {user.avatarUrl && <AvatarImage src={user.avatarUrl} alt={user.fullName} />}
                  <AvatarFallback>{user.fullName.slice(0, 2).toUpperCase()}</AvatarFallback>
                </Avatar>

                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2 flex-wrap">
                    <h3
                      className="font-semibold text-[#222222] cursor-pointer hover:text-[#FF5A5F]"
                      onClick={() => handleViewDetails(user.id)}
                    >
                      {user.fullName}
                    </h3>
                    {user.isVerified && (
                      <span className="text-blue-500" title="Verified">✓</span>
                    )}
                    {getKYCStatusBadge(user.kycStatus)}
                  </div>
                  <p className="text-sm text-[#717171]">{user.email}</p>
                  <p className="text-xs text-[#717171]">Joined {formatDate(user.createdAt)}</p>
                </div>

                <div className="flex items-center gap-2 flex-wrap">
                  <Badge variant={getRoleBadgeVariant(user.role)}>
                    {user.role}
                  </Badge>
                  <Badge variant={getStatusBadgeVariant(user.status)}>
                    {user.status}
                  </Badge>
                </div>

                <div className="flex gap-2">
                  {user.status === 'Active' ? (
                    <Button
                      size="sm"
                      variant="outline"
                      className="text-orange-600 border-orange-600 hover:bg-orange-50"
                      onClick={() => handleSuspendClick(user)}
                      disabled={pendingActions.has(user.id)}
                    >
                      {pendingActions.has(user.id) ? 'Processing...' : 'Suspend'}
                    </Button>
                  ) : (
                    <Button
                      size="sm"
                      variant="outline"
                      className="text-green-600 border-green-600 hover:bg-green-50"
                      onClick={() => handleUnsuspend(user.id)}
                      disabled={pendingActions.has(user.id)}
                    >
                      {pendingActions.has(user.id) ? 'Processing...' : 'Unsuspend'}
                    </Button>
                  )}
                  <Button
                    size="sm"
                    variant="ghost"
                    onClick={() => handleViewDetails(user.id)}
                  >
                    View Details
                  </Button>
                </div>
              </div>
            </Card>
          ))}
        </div>
      )}

      {/* Pagination */}
      {data && data.totalCount > data.pageSize && (
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

      {/* Drawers & Modals */}
      <UserDetailDrawer
        userId={selectedUserId}
        isOpen={isDrawerOpen}
        onClose={handleCloseDrawer}
        onSuspend={handleSuspendClick}
        onUnsuspend={(id) => handleUnsuspend(id)}
      />

      <SuspendUserModal
        isOpen={suspendModal.isOpen}
        userName={suspendModal.userName}
        onClose={() => setSuspendModal({ isOpen: false, userId: null, userName: '' })}
        onConfirm={handleSuspendConfirm}
        isLoading={suspendMutation.isPending}
      />
    </div>
  );
}