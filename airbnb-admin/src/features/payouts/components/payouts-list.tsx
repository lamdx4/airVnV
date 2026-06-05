"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { ColumnDef } from "@tanstack/react-table";
import { Banknote, Eye, Search, Plus } from "lucide-react";
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

import { usePayouts, useGeneratePayouts } from "../hooks";
import {
  PayoutStatus,
  type Payout,
  type PayoutListParams,
  type PayoutStatusValue,
} from "../types";
import { getPayoutStatusConfig } from "../utils/status";

const statusFilterOptions: { value: string; label: string }[] = [
  { value: "all", label: "All Statuses" },
  { value: String(PayoutStatus.PENDING), label: "Pending" },
  { value: String(PayoutStatus.APPROVED), label: "Approved" },
  { value: String(PayoutStatus.PROCESSING), label: "Processing" },
  { value: String(PayoutStatus.COMPLETED), label: "Completed" },
  { value: String(PayoutStatus.FAILED), label: "Failed" },
  { value: String(PayoutStatus.CANCELLED), label: "Cancelled" },
];

function StatusBadge({ status }: { status: PayoutStatusValue }) {
  const config = getPayoutStatusConfig(status);
  return <Badge variant={config.variant}>{config.label}</Badge>;
}

export function PayoutsList() {
  const router = useRouter();
  const [params, setParams] = useState<PayoutListParams>({
    page: 1,
    pageSize: DEFAULT_PAGE_SIZE,
  });
  const [statusFilter, setStatusFilter] = useState("all");
  const [searchInput, setSearchInput] = useState("");

  const { data, isLoading, isError, refetch } = usePayouts(params);
  const generateMutation = useGeneratePayouts();

  const payouts = data?.items ?? [];
  const totalItems = data?.totalItems ?? 0;

  function handleStatusFilter(value: string) {
    setStatusFilter(value);
    setParams((prev) => ({
      ...prev,
      page: 1,
      status: value === "all" ? undefined : (Number(value) as PayoutStatusValue),
    }));
  }

  function handleSearch() {
    setParams((prev) => ({ ...prev, page: 1, search: searchInput || undefined }));
  }

  function handleGenerate() {
    generateMutation.mutate(undefined, {
      onSuccess: (result) => {
        toast.success(`${result.payoutsGenerated} payout(s) generated`);
        refetch();
      },
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  const columns: ColumnDef<Payout>[] = [
    {
      accessorKey: "hostId",
      header: "Host ID",
      cell: ({ row }) => (
        <span className="font-mono text-xs">{row.original.hostId.slice(0, 8)}...</span>
      ),
    },
    {
      accessorKey: "totalEarnings",
      header: "Total Earnings",
      cell: ({ row }) => formatCurrency(row.original.totalEarnings, row.original.currency),
    },
    {
      accessorKey: "platformFee",
      header: "Platform Fee",
      cell: ({ row }) => formatCurrency(row.original.platformFee, row.original.currency),
    },
    {
      accessorKey: "payoutAmount",
      header: "Payout Amount",
      cell: ({ row }) => (
        <span className="font-semibold">
          {formatCurrency(row.original.payoutAmount, row.original.currency)}
        </span>
      ),
    },
    {
      accessorKey: "itemCount",
      header: "Bookings",
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
          onClick={() => router.push(ROUTES.PAYOUT_DETAIL(row.original.id))}
        >
          <Eye className="h-4 w-4" />
        </Button>
      ),
    },
  ];

  if (isError) {
    return (
      <div className="flex h-[50vh] items-center justify-center gap-4">
        <p className="text-[#6a6a6a]">Failed to load payouts.</p>
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
              placeholder="Search by host..."
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

        <div className="flex items-center gap-3">
          <Button onClick={handleGenerate} disabled={generateMutation.isPending}>
            <Plus className="h-4 w-4 mr-2" />
            {generateMutation.isPending ? "Generating..." : "Generate Payouts"}
          </Button>
          <div className="flex items-center gap-2 text-sm text-[#6a6a6a]">
            <Banknote className="h-4 w-4" />
            {totalItems} payouts
          </div>
        </div>
      </div>

      <DataTable
        columns={columns}
        data={payouts}
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

function getApiErrorMessage(error: unknown): string {
  if (error && typeof error === "object" && "response" in error) {
    const body = (error as { response?: { data?: { message?: string } } }).response?.data;
    if (body?.message) return body.message;
  }
  return "An unexpected error occurred.";
}
