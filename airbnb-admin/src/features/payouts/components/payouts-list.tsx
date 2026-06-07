"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { ColumnDef } from "@tanstack/react-table";
import { Banknote, Eye, CheckCircle2, AlertCircle } from "lucide-react";
import { toast } from "sonner";

import { DEFAULT_PAGE_SIZE } from "@/config/constants";
import { DataTable } from "@/components/common/data-table";
import { ConfirmDialog } from "@/components/common/confirm-dialog";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { useApprovePayout, usePayouts } from "../hooks";
import {
  PayoutStatus,
  type AdminPayoutItem,
  type PayoutListParams,
  type PayoutStatusValue,
} from "../types";
import { getPayoutStatusConfig } from "../utils/status";

const statusFilterOptions: { value: string; label: string }[] = [
  { value: "all", label: "All Statuses" },
  { value: PayoutStatus.PENDING, label: "Pending" },
  { value: PayoutStatus.APPROVED, label: "Approved" },
  { value: PayoutStatus.PROCESSING, label: "Processing" },
  { value: PayoutStatus.COMPLETED, label: "Completed" },
  { value: PayoutStatus.FAILED, label: "Failed" },
  { value: PayoutStatus.CANCELLED, label: "Cancelled" },
];

function StatusBadge({ status }: { status: PayoutStatusValue }) {
  const config = getPayoutStatusConfig(status);
  return <Badge variant={config.variant}>{config.label}</Badge>;
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

function shortenId(id: string) {
  return `${id.slice(0, 8)}…${id.slice(-4)}`;
}

function getApiErrorMessage(error: unknown): string {
  if (error && typeof error === "object" && "response" in error) {
    const body = (error as { response?: { data?: { message?: string } } }).response?.data;
    if (body?.message) return body.message;
  }
  return "Failed to process. Please try again.";
}

export function PayoutsList() {
  const router = useRouter();
  const [params, setParams] = useState<PayoutListParams>({
    page: 1,
    pageSize: DEFAULT_PAGE_SIZE,
    status: PayoutStatus.PENDING,
  });
  const [statusFilter, setStatusFilter] = useState<string>(PayoutStatus.PENDING);
  const [approveTarget, setApproveTarget] = useState<AdminPayoutItem | null>(null);

  const { data, isLoading } = usePayouts(params);
  const approveMutation = useApprovePayout();

  const items = data?.items ?? [];
  const totalItems = data?.totalItems ?? 0;

  function handleStatusFilter(value: string) {
    setStatusFilter(value);
    setParams((prev) => ({
      ...prev,
      page: 1,
      status: value === "all" ? undefined : (value as PayoutStatusValue),
    }));
  }

  function handleApprove() {
    if (!approveTarget) return;
    approveMutation.mutate(approveTarget.id, {
      onSuccess: () => {
        toast.success("Payout approved");
        setApproveTarget(null);
      },
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  const pendingTotal = items
    .filter((p) => p.status === PayoutStatus.PENDING)
    .reduce<Record<string, number>>((acc, p) => {
      acc[p.currency] = (acc[p.currency] ?? 0) + p.payoutAmount;
      return acc;
    }, {});

  const columns: ColumnDef<AdminPayoutItem>[] = [
    {
      accessorKey: "id",
      header: "Host / Payout",
      cell: ({ row }) => (
        <div className="flex flex-col">
          <span className="text-sm font-medium text-[#222222]">
            {row.original.hostName ?? "Unknown host"}
          </span>
          <span className="font-mono text-[10px] text-[#6a6a6a]">
            payout {shortenId(row.original.id)}
          </span>
        </div>
      ),
    },
    {
      accessorKey: "itemCount",
      header: "Bookings",
      cell: ({ row }) => (
        <span className="text-sm text-[#222222]">{row.original.itemCount}</span>
      ),
    },
    {
      accessorKey: "payoutAmount",
      header: () => <div className="text-right">Payout</div>,
      cell: ({ row }) => (
        <div className="text-right">
          <div className="font-semibold text-[#222222]">
            {formatMoney(row.original.payoutAmount, row.original.currency)}
          </div>
          <div className="text-xs text-[#6a6a6a]">
            fee {formatMoney(row.original.platformFee, row.original.currency)}
          </div>
        </div>
      ),
    },
    {
      accessorKey: "status",
      header: "Status",
      cell: ({ row }) => <StatusBadge status={row.original.status} />,
    },
    {
      accessorKey: "createdAt",
      header: "Created",
      cell: ({ row }) => (
        <span className="text-sm text-[#6a6a6a]">
          {formatDateTime(row.original.createdAt)}
        </span>
      ),
    },
    {
      id: "actions",
      header: () => <div className="text-right">Actions</div>,
      cell: ({ row }) => (
        <div className="flex justify-end gap-1">
          {row.original.status === PayoutStatus.PENDING && (
            <Button
              variant="outline"
              size="sm"
              onClick={() => setApproveTarget(row.original)}
              aria-label="Approve payout"
            >
              <CheckCircle2 className="h-4 w-4" />
            </Button>
          )}
          <Button
            variant="ghost"
            size="sm"
            onClick={() => router.push(`/payouts/${row.original.id}`)}
            aria-label="View payout"
          >
            <Eye className="h-4 w-4" />
          </Button>
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-4">
      {Object.keys(pendingTotal).length > 0 && (
        <div className="flex items-start gap-3 rounded-[14px] border border-[#fde2e1] bg-[#fff6f6] p-4">
          <AlertCircle className="mt-0.5 h-5 w-5 flex-shrink-0 text-[#c13515]" />
          <div className="text-sm text-[#222222]">
            <span className="font-semibold">Pending payouts on this page:</span>{" "}
            {Object.entries(pendingTotal)
              .map(([cur, amt]) => formatMoney(amt, cur))
              .join(" · ")}
          </div>
        </div>
      )}

      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <Select value={statusFilter} onValueChange={handleStatusFilter}>
          <SelectTrigger className="w-[200px]">
            <SelectValue placeholder="Status" />
          </SelectTrigger>
          <SelectContent>
            {statusFilterOptions.map((opt) => (
              <SelectItem key={opt.value} value={opt.value}>
                {opt.label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>

        <div className="flex items-center gap-2 text-sm text-[#6a6a6a]">
          <Banknote className="h-4 w-4" />
          {totalItems} payouts
        </div>
      </div>

      <DataTable
        columns={columns}
        data={items}
        totalItems={totalItems}
        pagination={{
          pageIndex: (params.page ?? 1) - 1,
          pageSize: params.pageSize ?? DEFAULT_PAGE_SIZE,
        }}
        onPaginationChange={(p) =>
          setParams((prev) => ({
            ...prev,
            page: p.pageIndex + 1,
            pageSize: p.pageSize,
          }))
        }
        isLoading={isLoading}
      />

      <ConfirmDialog
        open={Boolean(approveTarget)}
        onOpenChange={(open) => !open && setApproveTarget(null)}
        title="Approve Payout"
        description={
          approveTarget
            ? `Approve ${formatMoney(approveTarget.payoutAmount, approveTarget.currency)} payout to host ${shortenId(approveTarget.hostId)} (${approveTarget.itemCount} booking${approveTarget.itemCount === 1 ? "" : "s"})?`
            : ""
        }
        confirmLabel="Approve"
        onConfirm={handleApprove}
        isLoading={approveMutation.isPending}
      />
    </div>
  );
}
