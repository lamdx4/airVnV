"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { ColumnDef } from "@tanstack/react-table";
import { CreditCard, Eye, Search } from "lucide-react";
import { toast } from "sonner";

import { ROUTES, DEFAULT_PAGE_SIZE } from "@/config/constants";
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
  PaymentStatusLabel,
  type Payment,
  type PaymentListParams,
  type PaymentStatusValue,
} from "../types";
import { getPaymentStatusConfig } from "../utils/status";

const statusFilterOptions: { value: string; label: string }[] = [
  { value: "all", label: "All Statuses" },
  { value: String(PaymentStatus.PENDING), label: "Pending" },
  { value: String(PaymentStatus.SUCCESS), label: "Success" },
  { value: String(PaymentStatus.FAILED), label: "Failed" },
  { value: String(PaymentStatus.EXPIRED), label: "Expired" },
  { value: String(PaymentStatus.REFUNDED), label: "Refunded" },
  { value: String(PaymentStatus.PARTIALLY_REFUNDED), label: "Partially Refunded" },
];

function StatusBadge({ status }: { status: PaymentStatusValue }) {
  const config = getPaymentStatusConfig(status);
  return <Badge variant={config.variant}>{config.label}</Badge>;
}

export function PaymentsList() {
  const router = useRouter();
  const [params, setParams] = useState<PaymentListParams>({
    page: 1,
    pageSize: DEFAULT_PAGE_SIZE,
  });
  const [statusFilter, setStatusFilter] = useState("all");
  const [searchInput, setSearchInput] = useState("");

  const { data, isLoading, isError, refetch } = usePayments(params);

  const payments = data?.items ?? [];
  const totalItems = data?.totalItems ?? 0;

  function handleStatusFilter(value: string) {
    setStatusFilter(value);
    setParams((prev) => ({
      ...prev,
      page: 1,
      status: value === "all" ? undefined : (Number(value) as PaymentStatusValue),
    }));
  }

  function handleSearch() {
    setParams((prev) => ({ ...prev, page: 1, search: searchInput || undefined }));
  }

  const columns: ColumnDef<Payment>[] = [
    {
      accessorKey: "transactionId",
      header: "Transaction ID",
      cell: ({ row }) => (
        <span className="font-mono text-xs">
          {row.original.transactionId ?? "—"}
        </span>
      ),
    },
    {
      accessorKey: "amount",
      header: "Amount",
      cell: ({ row }) => formatCurrency(row.original.amount, row.original.currency),
    },
    {
      accessorKey: "currency",
      header: "Currency",
    },
    {
      accessorKey: "provider",
      header: "Provider",
    },
    {
      accessorKey: "status",
      header: "Status",
      cell: ({ row }) => <StatusBadge status={row.original.status} />,
    },
    {
      accessorKey: "createdAt",
      header: "Created",
      cell: ({ row }) => formatDate(row.getValue("createdAt")),
    },
    {
      id: "actions",
      header: "Actions",
      cell: ({ row }) => (
        <Button
          variant="ghost"
          size="sm"
          onClick={() => router.push(ROUTES.PAYMENT_DETAIL(row.original.id))}
        >
          <Eye className="h-4 w-4" />
        </Button>
      ),
    },
  ];

  if (isError) {
    return (
      <div className="flex h-[50vh] items-center justify-center gap-4">
        <p className="text-[#6a6a6a]">Failed to load payments.</p>
        <Button variant="outline" onClick={() => refetch()}>
          Retry
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex items-center gap-3">
          <Select value={statusFilter} onValueChange={handleStatusFilter}>
            <SelectTrigger className="w-[180px]">
              <SelectValue placeholder="Filter by status" />
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
              placeholder="Search by transaction ID..."
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleSearch()}
              className="w-[240px]"
            />
            <Button variant="outline" size="sm" onClick={handleSearch}>
              <Search className="h-4 w-4" />
            </Button>
          </div>
        </div>

        <div className="flex items-center gap-2 text-sm text-[#6a6a6a]">
          <CreditCard className="h-4 w-4" />
          {totalItems} transactions
        </div>
      </div>

      <DataTable
        columns={columns}
        data={payments}
        totalItems={totalItems}
        pagination={{
          pageIndex: (params.page ?? 1) - 1,
          pageSize: params.pageSize ?? DEFAULT_PAGE_SIZE,
        }}
        onPaginationChange={(p) =>
          setParams((prev) => ({ ...prev, page: p.pageIndex + 1, pageSize: p.pageSize }))
        }
        isLoading={isLoading}
      />
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
