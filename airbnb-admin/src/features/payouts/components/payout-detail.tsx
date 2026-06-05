"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import {
  ArrowLeft,
  CheckCircle2,
  Play,
  XCircle,
  RotateCcw,
  Banknote,
  DollarSign,
  Calendar,
  Hash,
} from "lucide-react";

import { ROUTES } from "@/config/constants";
import { Breadcrumbs, type BreadcrumbItem } from "@/components/layout/breadcrumbs";
import { PageLoader } from "@/components/common/loading";
import { ErrorDisplay } from "@/components/common/error-display";
import { ConfirmDialog } from "@/components/common/confirm-dialog";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

import {
  usePayout,
  useApprovePayout,
  useExecutePayout,
  useCancelPayout,
  useRetryPayout,
} from "../hooks";
import { PayoutStatus } from "../types";
import { getPayoutStatusConfig } from "../utils/status";

interface PayoutDetailProps {
  payoutId: string;
}

export function PayoutDetailView({ payoutId }: PayoutDetailProps) {
  const router = useRouter();
  const { data: payout, isLoading, isError, refetch } = usePayout(payoutId);
  const approveMutation = useApprovePayout();
  const executeMutation = useExecutePayout();
  const cancelMutation = useCancelPayout();
  const retryMutation = useRetryPayout();

  const [showApproveDialog, setShowApproveDialog] = useState(false);
  const [showExecuteDialog, setShowExecuteDialog] = useState(false);
  const [showCancelDialog, setShowCancelDialog] = useState(false);
  const [showRetryDialog, setShowRetryDialog] = useState(false);

  if (isLoading) return <PageLoader text="Loading payout..." />;
  if (isError || !payout) {
    return <ErrorDisplay message="Payout not found" onRetry={() => refetch()} />;
  }

  const statusConfig = getPayoutStatusConfig(payout.status);
  const canApprove = payout.status === PayoutStatus.PENDING;
  const canExecute = payout.status === PayoutStatus.APPROVED;
  const canCancel = payout.status === PayoutStatus.PENDING || payout.status === PayoutStatus.FAILED;
  const canRetry = payout.status === PayoutStatus.FAILED;

  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Payouts", href: ROUTES.PAYOUTS },
    { label: payout.id.slice(0, 8) },
  ];

  function handleApprove() {
    approveMutation.mutate(payoutId, {
      onSuccess: () => {
        toast.success("Payout approved");
        setShowApproveDialog(false);
      },
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  function handleExecute() {
    executeMutation.mutate(payoutId, {
      onSuccess: () => {
        toast.success("Payout executed and completed");
        setShowExecuteDialog(false);
      },
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  function handleCancel() {
    cancelMutation.mutate(payoutId, {
      onSuccess: () => {
        toast.success("Payout cancelled");
        setShowCancelDialog(false);
      },
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  function handleRetry() {
    retryMutation.mutate(payoutId, {
      onSuccess: () => {
        toast.success("Payout retried and completed");
        setShowRetryDialog(false);
      },
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  return (
    <div className="space-y-6">
      <div className="space-y-2">
        <Breadcrumbs items={breadcrumbs} />
        <div className="flex items-center gap-3">
          <Button variant="ghost" size="sm" onClick={() => router.push(ROUTES.PAYOUTS)}>
            <ArrowLeft className="h-4 w-4 mr-1" />
            Back
          </Button>
          <h1 className="text-[28px] font-bold text-[#222222]">Payout Detail</h1>
          <Badge variant={statusConfig.variant}>{statusConfig.label}</Badge>
        </div>
        <p className="text-sm text-[#6a6a6a]">
          Created {formatDate(payout.createdAt)}
          {payout.approvedAt && <> &middot; Approved {formatDate(payout.approvedAt)}</>}
          {payout.completedAt && <> &middot; Completed {formatDate(payout.completedAt)}</>}
        </p>
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Payout Summary</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <DetailRow icon={Hash} label="Payout ID" value={payout.id} />
            <DetailRow label="Host ID" value={payout.hostId} />
            <DetailRow icon={Banknote} label="Total Earnings" value={formatCurrency(payout.totalEarnings, payout.currency)} />
            <DetailRow label="Platform Fee" value={formatCurrency(payout.platformFee, payout.currency)} />
            <DetailRow icon={DollarSign} label="Payout Amount" value={formatCurrency(payout.payoutAmount, payout.currency)} />
            <DetailRow label="Bookings" value={String(payout.itemCount)} />
            <DetailRow label="Currency" value={payout.currency} />
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Actions</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex flex-wrap items-center gap-3">
              <Button
                variant="default"
                onClick={() => setShowApproveDialog(true)}
                disabled={!canApprove || approveMutation.isPending}
              >
                <CheckCircle2 className="h-4 w-4 mr-2" />
                {approveMutation.isPending ? "Approving..." : "Approve"}
              </Button>
              <Button
                variant="outline"
                onClick={() => setShowExecuteDialog(true)}
                disabled={!canExecute || executeMutation.isPending}
              >
                <Play className="h-4 w-4 mr-2" />
                {executeMutation.isPending ? "Executing..." : "Execute Payout"}
              </Button>
              <Button
                variant="outline"
                onClick={() => setShowRetryDialog(true)}
                disabled={!canRetry || retryMutation.isPending}
              >
                <RotateCcw className="h-4 w-4 mr-2" />
                Retry
              </Button>
              <Button
                variant="destructive"
                onClick={() => setShowCancelDialog(true)}
                disabled={!canCancel || cancelMutation.isPending}
              >
                <XCircle className="h-4 w-4 mr-2" />
                Cancel
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>

      {payout.items && payout.items.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle>Booking Breakdown</CardTitle>
          </CardHeader>
          <CardContent>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Property</TableHead>
                  <TableHead>Guest</TableHead>
                  <TableHead>Check-in</TableHead>
                  <TableHead>Check-out</TableHead>
                  <TableHead className="text-right">Booking Total</TableHead>
                  <TableHead className="text-right">Service Fee</TableHead>
                  <TableHead className="text-right">Host Earning</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {payout.items.map((item) => (
                  <TableRow key={item.id}>
                    <TableCell className="font-medium">{item.propertyTitle}</TableCell>
                    <TableCell>{item.guestName}</TableCell>
                    <TableCell>{formatDate(item.checkIn)}</TableCell>
                    <TableCell>{formatDate(item.checkOut)}</TableCell>
                    <TableCell className="text-right">{formatCurrency(item.bookingTotal, payout.currency)}</TableCell>
                    <TableCell className="text-right">{formatCurrency(item.serviceFee, payout.currency)}</TableCell>
                    <TableCell className="text-right font-medium">{formatCurrency(item.hostEarning, payout.currency)}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </CardContent>
        </Card>
      )}

      <ConfirmDialog
        open={showApproveDialog}
        onOpenChange={setShowApproveDialog}
        title="Approve Payout"
        description={`Approve payout of ${formatCurrency(payout.payoutAmount, payout.currency)}?`}
        confirmLabel="Approve"
        onConfirm={handleApprove}
        isLoading={approveMutation.isPending}
      />
      <ConfirmDialog
        open={showExecuteDialog}
        onOpenChange={setShowExecuteDialog}
        title="Execute Payout"
        description={`Execute disbursement of ${formatCurrency(payout.payoutAmount, payout.currency)}?`}
        confirmLabel="Execute"
        onConfirm={handleExecute}
        isLoading={executeMutation.isPending}
      />
      <ConfirmDialog
        open={showCancelDialog}
        onOpenChange={setShowCancelDialog}
        title="Cancel Payout"
        description="Are you sure you want to cancel this payout?"
        confirmLabel="Cancel Payout"
        onConfirm={handleCancel}
        isLoading={cancelMutation.isPending}
      />
      <ConfirmDialog
        open={showRetryDialog}
        onOpenChange={setShowRetryDialog}
        title="Retry Payout"
        description="Retry the failed payout disbursement?"
        confirmLabel="Retry"
        onConfirm={handleRetry}
        isLoading={retryMutation.isPending}
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

function formatCurrency(amount: number, currency: string): string {
  try {
    return new Intl.NumberFormat("en-US", { style: "currency", currency }).format(amount);
  } catch {
    return `${amount} ${currency}`;
  }
}

function formatDate(date: string): string {
  return new Intl.DateTimeFormat("en-US", { dateStyle: "medium" }).format(new Date(date));
}

function getApiErrorMessage(error: unknown): string {
  if (error && typeof error === "object" && "response" in error) {
    const body = (error as { response?: { data?: { message?: string } } }).response?.data;
    if (body?.message) return body.message;
  }
  return "An unexpected error occurred.";
}
