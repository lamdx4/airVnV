"use client";
import { useState } from "react";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import {
  ArrowLeft,
  Ban,
  Mail,
  Phone,
  Shield,
  ShieldCheck,
  XCircle,
  RotateCcw,
} from "lucide-react";
import { ROUTES } from "@/config/constants";
import { Breadcrumbs, type BreadcrumbItem } from "@/components/layout/breadcrumbs";
import { PageLoader } from "@/components/common/loading";
import { ErrorDisplay } from "@/components/common/error-display";
import { ConfirmDialog } from "@/components/common/confirm-dialog";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  useUser,
  useSuspendUser,
  useBanUser,
  useActivateUser,
} from "../hooks";
import { UserStatus, UserRole, type UserDetail as UserDetailType } from "../types";
import { getUserStatusConfig, getUserRoleConfig } from "../utils/status";

interface UserDetailProps {
  userId: string;
}

export function UserDetail({ userId }: UserDetailProps) {
  const router = useRouter();
  const {
    data: user,
    isLoading,
    isError,
    refetch,
  } = useUser(userId);
  const suspendMutation = useSuspendUser();
  const banMutation = useBanUser();
  const activateMutation = useActivateUser();

  const [showSuspendDialog, setShowSuspendDialog] = useState(false);
  const [suspendReason, setSuspendReason] = useState("");
  const [showBanDialog, setShowBanDialog] = useState(false);
  const [banReason, setBanReason] = useState("");
  const [showActivateDialog, setShowActivateDialog] = useState(false);

  if (isLoading) return <PageLoader text="Loading user..." />;
  if (isError || !user) {
    return <ErrorDisplay message="User not found" onRetry={() => refetch()} />;
  }

  const statusConfig = getUserStatusConfig(user.status);
  const roleConfig = getUserRoleConfig(user.role);

  const canSuspend = user.status === UserStatus.ACTIVE;
  const canBan = user.status === UserStatus.ACTIVE || user.status === UserStatus.SUSPENDED;
  const canActivate = user.status === UserStatus.SUSPENDED || user.status === UserStatus.BANNED;

  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Users", href: ROUTES.USERS },
    { label: user.fullName },
  ];

  function handleSuspend() {
    if (!suspendReason.trim()) {
      toast.error("Suspension reason is required");
      return;
    }
    suspendMutation.mutate(
      { id: userId, reason: suspendReason },
      {
        onSuccess: () => {
          toast.success("User suspended successfully");
          setShowSuspendDialog(false);
          setSuspendReason("");
        },
        onError: (err) => toast.error(getApiErrorMessage(err)),
      },
    );
  }

  function handleBan() {
    if (!banReason.trim()) {
      toast.error("Ban reason is required");
      return;
    }
    banMutation.mutate(
      { id: userId, reason: banReason },
      {
        onSuccess: () => {
          toast.success("User banned successfully");
          setShowBanDialog(false);
          setBanReason("");
        },
        onError: (err) => toast.error(getApiErrorMessage(err)),
      },
    );
  }

  function handleActivate() {
    activateMutation.mutate(userId, {
      onSuccess: () => {
        toast.success("User activated successfully");
        setShowActivateDialog(false);
      },
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="space-y-2">
          <Breadcrumbs items={breadcrumbs} />
          <div className="flex items-center gap-3">
            <Button variant="ghost" size="sm" onClick={() => router.push(ROUTES.USERS)}>
              <ArrowLeft className="h-4 w-4 mr-1" />
              Back
            </Button>
            <h1 className="text-[28px] font-bold text-[#222222]">{user.fullName}</h1>
            <Badge variant={statusConfig.variant}>{statusConfig.label}</Badge>
            <Badge variant="outline">{roleConfig.label}</Badge>
            {user.isVerified && (
              <Badge variant="success">
                <ShieldCheck className="h-3 w-3 mr-1" />
                Verified
              </Badge>
            )}
          </div>
          <p className="text-sm text-[#6a6a6a]">
            Created {formatDate(user.createdAt)}
            {user.lastLoginAt && <> &middot; Last login {formatDate(user.lastLoginAt)}</>}
          </p>
        </div>
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Account Information</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <DetailRow icon={Mail} label="Email" value={user.email} />
            {user.phone && <DetailRow icon={Phone} label="Phone" value={user.phone} />}
            <DetailRow icon={Shield} label="Role" value={roleConfig.label} />
            <DetailRow label="Status" value={statusConfig.label} />
            <DetailRow label="Verified" value={user.isVerified ? "Yes" : "No"} />
            <DetailRow label="Created" value={formatDate(user.createdAt)} />
            <DetailRow label="Last Login" value={user.lastLoginAt ? formatDate(user.lastLoginAt) : "Never"} />
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Account Status Actions</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex flex-wrap items-center gap-3">
              <Button
                variant="outline"
                onClick={() => setShowSuspendDialog(true)}
                disabled={!canSuspend}
              >
                <Ban className="h-4 w-4 mr-2" />
                Suspend
              </Button>
              <Button
                variant="destructive"
                onClick={() => setShowBanDialog(true)}
                disabled={!canBan}
              >
                <XCircle className="h-4 w-4 mr-2" />
                Ban
              </Button>
              <Button
                variant="outline"
                onClick={() => setShowActivateDialog(true)}
                disabled={!canActivate}
              >
                <RotateCcw className="h-4 w-4 mr-2" />
                Activate
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>

      {user.suspensionReason && (
        <Card>
          <CardHeader>
            <CardTitle className="text-[#6a6a6a]">Suspension Reason</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-sm">{user.suspensionReason}</p>
          </CardContent>
        </Card>
      )}

      {user.banReason && (
        <Card>
          <CardHeader>
            <CardTitle className="text-[#c13515]">Ban Reason</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-sm">{user.banReason}</p>
          </CardContent>
        </Card>
      )}

      {/* Suspend Dialog */}
      <ConfirmDialog
        open={showSuspendDialog}
        onOpenChange={(open) => {
          setShowSuspendDialog(open);
          if (!open) setSuspendReason("");
        }}
        title="Suspend User"
        description="Are you sure you want to suspend this user? They will not be able to log in."
        confirmLabel="Suspend"
        variant="destructive"
        onConfirm={handleSuspend}
        isLoading={suspendMutation.isPending}
      >
        <div className="space-y-2 py-2">
          <Label htmlFor="suspend-reason">Reason (required)</Label>
          <Input
            id="suspend-reason"
            value={suspendReason}
            onChange={(e) => setSuspendReason(e.target.value)}
            placeholder="Enter suspension reason..."
          />
        </div>
      </ConfirmDialog>

      {/* Ban Dialog */}
      <ConfirmDialog
        open={showBanDialog}
        onOpenChange={(open) => {
          setShowBanDialog(open);
          if (!open) setBanReason("");
        }}
        title="Ban User"
        description="Are you sure you want to ban this user? This action is permanent."
        confirmLabel="Ban"
        variant="destructive"
        onConfirm={handleBan}
        isLoading={banMutation.isPending}
      >
        <div className="space-y-2 py-2">
          <Label htmlFor="ban-reason">Reason (required)</Label>
          <Input
            id="ban-reason"
            value={banReason}
            onChange={(e) => setBanReason(e.target.value)}
            placeholder="Enter ban reason..."
          />
        </div>
      </ConfirmDialog>

      {/* Activate Dialog */}
      <ConfirmDialog
        open={showActivateDialog}
        onOpenChange={setShowActivateDialog}
        title="Activate User"
        description="Are you sure you want to reactivate this user? They will be able to log in again."
        confirmLabel="Activate"
        onConfirm={handleActivate}
        isLoading={activateMutation.isPending}
      />
    </div>
  );
}

function DetailRow({
  icon: Icon,
  label,
  value,
}: {
  icon?: React.ComponentType<{ className?: string }>;
  label: string;
  value: string;
}) {
  return (
    <div className="flex items-center justify-between text-sm">
      <span className="flex items-center gap-2 text-[#6a6a6a]">
        {Icon && <Icon className="h-4 w-4" />}
        {label}
      </span>
      <span className="font-medium">{value}</span>
    </div>
  );
}

function getApiErrorMessage(error: unknown): string {
  if (error && typeof error === "object" && "response" in error) {
    const body = (error as { response?: { data?: { message?: string } } }).response?.data;
    if (body?.message) return body.message;
  }
  return "An error occurred. Please try again.";
}

function formatDate(date: string): string {
  return new Intl.DateTimeFormat("en-US", { dateStyle: "medium" }).format(new Date(date));
}