"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { ArrowLeft, CheckCircle2, CircleDollarSign } from "lucide-react";
import { toast } from "sonner";

import { Breadcrumbs, type BreadcrumbItem } from "@/components/layout/breadcrumbs";
import { PageLoader } from "@/components/common/loading";
import { ErrorDisplay } from "@/components/common/error-display";
import { ConfirmDialog } from "@/components/common/confirm-dialog";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

import {
  useApprovePayout,
  useMarkPayoutCompleted,
  usePayout,
} from "../hooks";
import { PayoutStatus } from "../types";
import { getPayoutStatusConfig } from "../utils/status";

interface PayoutDetailProps {
  payoutId: string;
}

function formatMoney(amount: number, currency: string) {
  try {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency,
      maximumFractionDigits: 2,
    }).format(amount);
  } catch {
    return `${amount.toFixed(2)} ${currency}`;
  }
}

function formatDateTime(iso: string) {
  return new Intl.DateTimeFormat("en-US", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(iso));
}

function formatDate(iso: string) {
  return new Intl.DateTimeFormat("en-US", { dateStyle: "medium" }).format(new Date(iso));
}

function getApiErrorMessage(error: unknown): string {
  if (error && typeof error === "object" && "response" in error) {
    const body = (error as { response?: { data?: { message?: string } } }).response?.data;
    if (body?.message) return body.message;
  }
  return "Failed to process. Please try again.";
}

export function PayoutDetail({ payoutId }: PayoutDetailProps) {
  const router = useRouter();
  const { data, isLoading, isError, refetch } = usePayout(payoutId);
  const approveMutation = useApprovePayout();
  const completeMutation = useMarkPayoutCompleted();

  const [showApprove, setShowApprove] = useState(false);
  const [showComplete, setShowComplete] = useState(false);

  if (isLoading) return <PageLoader text="Loading payout..." />;
  if (isError || !data) {
    return <ErrorDisplay message="Payout not found" onRetry={() => refetch()} />;
  }

  const statusConfig = getPayoutStatusConfig(data.status);
  const canApprove = data.status === PayoutStatus.PENDING;
  const canComplete =
    data.status === PayoutStatus.APPROVED || data.status === PayoutStatus.PROCESSING;

  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Payouts", href: "/payouts" },
    { label: data.id.slice(0, 8) },
  ];

  function handleApprove() {
    approveMutation.mutate(payoutId, {
      onSuccess: () => {
        toast.success("Payout approved");
        setShowApprove(false);
      },
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  function handleComplete() {
    completeMutation.mutate(payoutId, {
      onSuccess: () => {
        toast.success("Payout marked completed");
        setShowComplete(false);
      },
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  return (
    <div className="space-y-6">
      <div className="space-y-2">
        <Breadcrumbs items={breadcrumbs} />
        <div className="flex flex-wrap items-center gap-3">
          <Button variant="ghost" size="sm" onClick={() => router.push("/payouts")}>
            <ArrowLeft className="mr-1 h-4 w-4" />
            Back
          </Button>
          <h1 className="text-[28px] font-bold text-[#222222]">Payout Detail</h1>
          <Badge variant={statusConfig.variant}>{statusConfig.label}</Badge>
        </div>
        <p className="text-sm text-[#6a6a6a]">
          Created {formatDateTime(data.createdAt)}
          {data.approvedAt && <> · Approved {formatDateTime(data.approvedAt)}</>}
          {data.completedAt && <> · Completed {formatDateTime(data.completedAt)}</>}
        </p>
      </div>

      <div className="grid gap-6 md:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle className="text-sm font-medium text-[#6a6a6a]">
              Payout Amount
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-[#222222]">
              {formatMoney(data.payoutAmount, data.currency)}
            </div>
            <p className="mt-1 text-xs text-[#6a6a6a]">
              From {data.itemCount} booking{data.itemCount === 1 ? "" : "s"}
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="text-sm font-medium text-[#6a6a6a]">
              Total Earnings
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-[#222222]">
              {formatMoney(data.totalEarnings, data.currency)}
            </div>
            <p className="mt-1 text-xs text-[#6a6a6a]">Host gross income</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="text-sm font-medium text-[#6a6a6a]">
              Platform Fee
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-[#222222]">
              {formatMoney(data.platformFee, data.currency)}
            </div>
            <p className="mt-1 text-xs text-[#6a6a6a]">Withheld by platform</p>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>Booking Lines ({data.items.length})</CardTitle>
          <CircleDollarSign className="h-5 w-5 text-[#6a6a6a]" />
        </CardHeader>
        <CardContent>
          {data.items.length === 0 ? (
            <p className="text-sm text-[#6a6a6a]">No items.</p>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b border-[#dddddd] text-left text-xs uppercase text-[#6a6a6a]">
                    <th className="py-2 pr-4 font-medium">Property / Guest</th>
                    <th className="py-2 pr-4 font-medium">Stay</th>
                    <th className="py-2 pr-4 text-right font-medium">Booking</th>
                    <th className="py-2 pr-4 text-right font-medium">Fee</th>
                    <th className="py-2 text-right font-medium">Host earning</th>
                  </tr>
                </thead>
                <tbody>
                  {data.items.map((item) => (
                    <tr key={item.id} className="border-b border-[#f0f0f0]">
                      <td className="py-3 pr-4">
                        <div className="font-medium text-[#222222]">
                          {item.propertyTitle}
                        </div>
                        <div className="text-xs text-[#6a6a6a]">{item.guestName}</div>
                      </td>
                      <td className="py-3 pr-4 text-[#6a6a6a]">
                        {formatDate(item.checkIn)} → {formatDate(item.checkOut)}
                      </td>
                      <td className="py-3 pr-4 text-right text-[#222222]">
                        {formatMoney(item.bookingTotal, data.currency)}
                      </td>
                      <td className="py-3 pr-4 text-right text-[#6a6a6a]">
                        {formatMoney(item.serviceFee, data.currency)}
                      </td>
                      <td className="py-3 text-right font-semibold text-[#222222]">
                        {formatMoney(item.hostEarning, data.currency)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
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
              onClick={() => setShowApprove(true)}
              disabled={!canApprove || approveMutation.isPending}
            >
              <CheckCircle2 className="mr-2 h-4 w-4" />
              {approveMutation.isPending ? "Approving..." : "Approve"}
            </Button>
            <Button
              variant="outline"
              onClick={() => setShowComplete(true)}
              disabled={!canComplete || completeMutation.isPending}
            >
              <CheckCircle2 className="mr-2 h-4 w-4" />
              {completeMutation.isPending ? "Saving..." : "Mark Completed"}
            </Button>
          </div>
        </CardContent>
      </Card>

      <ConfirmDialog
        open={showApprove}
        onOpenChange={setShowApprove}
        title="Approve Payout"
        description={`Approve ${formatMoney(data.payoutAmount, data.currency)} payout to host?`}
        confirmLabel="Approve"
        onConfirm={handleApprove}
        isLoading={approveMutation.isPending}
      />

      <ConfirmDialog
        open={showComplete}
        onOpenChange={setShowComplete}
        title="Mark Payout Completed"
        description="Mark this payout as completed (bank transfer confirmed)?"
        confirmLabel="Mark Completed"
        onConfirm={handleComplete}
        isLoading={completeMutation.isPending}
      />
    </div>
  );
}
