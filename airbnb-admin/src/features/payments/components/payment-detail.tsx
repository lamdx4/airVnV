"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import {
  ArrowLeft,
  CreditCard,
  DollarSign,
  RotateCcw,
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
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";

import { usePayment, useRefundPayment } from "../hooks";
import { PaymentStatus, type RefundRequest } from "../types";
import { getPaymentStatusConfig } from "../utils/status";

interface PaymentDetailProps {
  paymentId: string;
}

export function PaymentDetailView({ paymentId }: PaymentDetailProps) {
  const router = useRouter();
  const { data: payment, isLoading, isError, refetch } = usePayment(paymentId);
  const refundMutation = useRefundPayment();

  const [showRefundDialog, setShowRefundDialog] = useState(false);
  const [refundReason, setRefundReason] = useState("");
  const [refundAmount, setRefundAmount] = useState("");

  if (isLoading) return <PageLoader text="Loading payment..." />;
  if (isError || !payment) {
    return <ErrorDisplay message="Payment not found" onRetry={() => refetch()} />;
  }

  const statusConfig = getPaymentStatusConfig(payment.status);
  const canRefund =
    payment.status === PaymentStatus.SUCCESS ||
    payment.status === PaymentStatus.PARTIALLY_REFUNDED;

  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Payments", href: ROUTES.PAYMENTS },
    { label: payment.transactionId ?? payment.id.slice(0, 8) },
  ];

  function handleRefund() {
    if (!refundReason.trim()) {
      toast.error("Refund reason is required");
      return;
    }

    const data: RefundRequest = {
      reason: refundReason,
      ...(refundAmount ? { amount: parseFloat(refundAmount) } : {}),
    };

    refundMutation.mutate(
      { id: paymentId, data },
      {
        onSuccess: () => {
          toast.success("Refund processed successfully");
          setShowRefundDialog(false);
          setRefundReason("");
          setRefundAmount("");
        },
        onError: (err) => toast.error(getApiErrorMessage(err)),
      },
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="space-y-2">
          <Breadcrumbs items={breadcrumbs} />
          <div className="flex items-center gap-3">
            <Button variant="ghost" size="sm" onClick={() => router.push(ROUTES.PAYMENTS)}>
              <ArrowLeft className="h-4 w-4 mr-1" />
              Back
            </Button>
            <h1 className="text-[28px] font-bold text-[#222222]">Payment Detail</h1>
            <Badge variant={statusConfig.variant}>{statusConfig.label}</Badge>
          </div>
          <p className="text-sm text-[#6a6a6a]">
            Created {formatDate(payment.createdAt)}
          </p>
        </div>
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Payment Information</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <DetailRow icon={DollarSign} label="Amount" value={formatCurrency(payment.amount, payment.currency)} />
            <DetailRow label="Currency" value={payment.currency} />
            <DetailRow icon={Hash} label="Transaction ID" value={payment.transactionId ?? "—"} />
            <DetailRow icon={CreditCard} label="Provider" value={payment.provider} />
            <DetailRow icon={Calendar} label="Created" value={formatDate(payment.createdAt)} />
            {payment.expiresAt && (
              <DetailRow label="Expires" value={formatDate(payment.expiresAt)} />
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Booking Reference</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <DetailRow label="Booking ID" value={payment.bookingId} />
            <DetailRow label="Payment ID" value={payment.id} />
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Actions</CardTitle>
        </CardHeader>
        <CardContent>
          <Button
            variant="outline"
            onClick={() => setShowRefundDialog(true)}
            disabled={!canRefund}
          >
            <RotateCcw className="h-4 w-4 mr-2" />
            Refund
          </Button>
          {!canRefund && (
            <p className="mt-2 text-xs text-[#6a6a6a]">
              Refund is only available for successful or partially refunded payments.
            </p>
          )}
        </CardContent>
      </Card>

      {payment.refunds && payment.refunds.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle>Refund History</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {payment.refunds.map((refund) => (
                <div
                  key={refund.id}
                  className="flex items-center justify-between rounded-[8px] border border-[#dddddd] p-3"
                >
                  <div className="space-y-1">
                    <p className="text-sm font-medium">
                      {formatCurrency(refund.amount, payment.currency)}
                      {refund.isFullRefund && (
                        <Badge variant="secondary" className="ml-2">
                          Full
                        </Badge>
                      )}
                    </p>
                    <p className="text-xs text-[#6a6a6a]">{refund.reason}</p>
                  </div>
                  <p className="text-xs text-[#6a6a6a]">
                    {formatDate(refund.createdAt)}
                  </p>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      )}

      <ConfirmDialog
        open={showRefundDialog}
        onOpenChange={setShowRefundDialog}
        title="Refund Payment"
        description={`Issue a refund for ${formatCurrency(payment.amount, payment.currency)}? Leave amount empty for a full refund.`}
        confirmLabel="Process Refund"
        onConfirm={handleRefund}
        isLoading={refundMutation.isPending}
      >
        <div className="space-y-3">
          <div className="space-y-2">
            <Label htmlFor="refund-amount">Refund Amount (leave empty for full)</Label>
            <Input
              id="refund-amount"
              type="number"
              step="0.01"
              placeholder={`Full amount: ${payment.amount}`}
              value={refundAmount}
              onChange={(e) => setRefundAmount(e.target.value)}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="refund-reason">Reason *</Label>
            <Textarea
              id="refund-reason"
              placeholder="Reason for the refund..."
              value={refundReason}
              onChange={(e) => setRefundReason(e.target.value)}
              rows={3}
            />
          </div>
        </div>
      </ConfirmDialog>
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
  return new Intl.DateTimeFormat("en-US", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(date));
}

function getApiErrorMessage(error: unknown): string {
  if (error && typeof error === "object" && "response" in error) {
    const body = (error as { response?: { data?: { message?: string } } }).response?.data;
    if (body?.message) return body.message;
  }
  return "Failed to process refund. Please try again.";
}
