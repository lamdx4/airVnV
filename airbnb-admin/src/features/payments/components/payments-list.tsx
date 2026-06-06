"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { ColumnDef } from "@tanstack/react-table";
import { CreditCard, Eye, Search } from "lucide-react";

import { DEFAULT_PAGE_SIZE } from "@/config/constants";
import { DataTable } from "@/components/common/data-table";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { usePayments } from "../hooks";
import {
  PaymentStatus,
  type AdminPaymentItem,
  type PaymentListParams,
  type PaymentStatusValue,
} from "../types";
import { getPaymentStatusConfig } from "../utils/status";

const statusFilterOptions: { value: string; label: string }[] = [
  { value: "all", label: "All Statuses" },
  { value: PaymentStatus.PENDING, label: "Pending" },
  { value: PaymentStatus.SUCCESS, label: "Success" },
  { value: PaymentStatus.FAILED, label: "Failed" },
  { value: PaymentStatus.EXPIRED, label: "Expired" },
  { value: PaymentStatus.REFUNDED, label: "Refunded" },
  { value: PaymentStatus.PARTIALLY_REFUNDED, label: "Partially refunded" },
];

function StatusBadge({ status }: { status: PaymentStatusValue }) {
  const config = getPaymentStatusConfig(status);
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

export function PaymentsList() {
  const router = useRouter();
  const [params, setParams] = useState<PaymentListParams>({
    page: 1,
    pageSize: DEFAULT_PAGE_SIZE,
    sortOrder: "desc",
  });
  const [statusFilter, setStatusFilter] = useState<string>("all");
  const [searchInput, setSearchInput] = useState("");

  const { data, isLoading } = usePayments(params);
  const items = data?.items ?? [];
  const totalItems = data?.totalItems ?? 0;

  function handleStatusFilter(value: string) {
    setStatusFilter(value);
    setParams((prev) => ({
      ...prev,
      page: 1,
      status: value === "all" ? undefined : (value as PaymentStatusValue),
    }));
  }

  function handleSearch() {
    setParams((prev) => ({
      ...prev,
      page: 1,
      search: searchInput.trim() || undefined,
    }));
  }

  const columns: ColumnDef<AdminPaymentItem>[] = [
    {
      accessorKey: "id",
      header: "Payment",
      cell: ({ row }) => (
        <div className="flex flex-col">
          <span className="font-mono text-xs text-[#222222]">{shortenId(row.original.id)}</span>
          {row.original.transactionId && (
            <span className="text-xs text-[#6a6a6a]">txn {row.original.transactionId}</span>
          )}
        </div>
      ),
    },
    {
      accessorKey: "bookingId",
      header: "Booking",
      cell: ({ row }) => (
        <span className="font-mono text-xs text-[#6a6a6a]">{shortenId(row.original.bookingId)}</span>
      ),
    },
    {
      accessorKey: "amount",
      header: () => <div className="text-right">Amount</div>,
      cell: ({ row }) => (
        <div className="text-right font-medium text-[#222222]">
          {formatMoney(row.original.amount, row.original.currency)}
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
        <span className="text-sm text-[#6a6a6a]">{formatDateTime(row.original.createdAt)}</span>
      ),
    },
    {
      id: "actions",
      header: () => <div className="text-right">Actions</div>,
      cell: ({ row }) => (
        <div className="flex justify-end">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => router.push(`/payments/${row.original.id}`)}
            aria-label="View payment"
          >
            <Eye className="h-4 w-4" />
          </Button>
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex flex-wrap items-center gap-2">
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

          <div className="flex items-center gap-2">
            <Input
              placeholder="BookingId / TransactionId..."
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleSearch()}
              className="w-[280px]"
            />
            <Button
              variant="outline"
              onClick={handleSearch}
              aria-label="Search"
              className="h-12 w-12 px-0"
            >
              <Search className="h-4 w-4" />
            </Button>
          </div>
        </div>

        <div className="flex items-center gap-2 text-sm text-[#6a6a6a]">
          <CreditCard className="h-4 w-4" />
          {totalItems} payments
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
    </div>
  );
}
