"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { ColumnDef } from "@tanstack/react-table";
import { Eye, Vault, Wallet, Clock, RefreshCw } from "lucide-react";
import { toast } from "sonner";

import { DEFAULT_PAGE_SIZE } from "@/config/constants";
import { DataTable } from "@/components/common/data-table";
import { StatCard } from "@/components/common/stat-card";
import { Button } from "@/components/ui/button";

import { useBootstrapHostBalances, useHostBalances } from "../hooks";
import type { HostBalanceItem, HostBalanceListParams } from "../types";

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

export function HostBalancesList() {
  const router = useRouter();
  const [params, setParams] = useState<HostBalanceListParams>({
    page: 1,
    pageSize: DEFAULT_PAGE_SIZE,
  });

  const { data, isLoading } = useHostBalances(params);
  const bootstrap = useBootstrapHostBalances();

  const items = data?.page.items ?? [];
  const totalItems = data?.page.totalCount ?? 0;
  const totalVnd = data?.totalEscrowVnd ?? 0;
  const totalUsd = data?.totalEscrowUsd ?? 0;

  const columns: ColumnDef<HostBalanceItem>[] = [
    {
      accessorKey: "hostId",
      header: "Host",
      cell: ({ row }) => (
        <div className="flex flex-col">
          <span className="text-sm font-medium text-[#222222]">
            {row.original.hostName ?? "Unknown host"}
          </span>
          <span className="font-mono text-[10px] text-[#6a6a6a]">
            {row.original.hostEmail ?? shortenId(row.original.hostId)}
          </span>
        </div>
      ),
    },
    {
      accessorKey: "currency",
      header: "Currency",
      cell: ({ row }) => (
        <span className="font-medium text-[#222222]">{row.original.currency}</span>
      ),
    },
    {
      accessorKey: "pendingBalance",
      header: () => <div className="text-right">Pending</div>,
      cell: ({ row }) => (
        <div className="text-right">
          <div className="font-medium text-[#ffb400]">
            {formatMoney(row.original.pendingBalance, row.original.currency)}
          </div>
          <div className="text-[10px] text-[#6a6a6a]">awaiting check-out</div>
        </div>
      ),
    },
    {
      accessorKey: "availableBalance",
      header: () => <div className="text-right">Available</div>,
      cell: ({ row }) => (
        <div className="text-right">
          <div className="font-semibold text-[#3a8b56]">
            {formatMoney(row.original.availableBalance, row.original.currency)}
          </div>
          <div className="text-[10px] text-[#6a6a6a]">ready for payout</div>
        </div>
      ),
    },
    {
      accessorKey: "totalHeld",
      header: () => <div className="text-right">Total held</div>,
      cell: ({ row }) => (
        <div className="text-right font-semibold text-[#222222]">
          {formatMoney(row.original.totalHeld, row.original.currency)}
        </div>
      ),
    },
    {
      accessorKey: "updatedAt",
      header: "Updated",
      cell: ({ row }) => (
        <span className="text-sm text-[#6a6a6a]">
          {formatDateTime(row.original.updatedAt)}
        </span>
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
            onClick={() => router.push(`/host-balances/${row.original.id}`)}
            aria-label="View detail"
          >
            <Eye className="h-4 w-4" />
          </Button>
        </div>
      ),
    },
  ];

  function handleBootstrap() {
    bootstrap.mutate(undefined, {
      onSuccess: () => toast.success("Ledger rebuilt"),
      onError: () => toast.error("Failed to rebuild ledger"),
    });
  }

  return (
    <div className="space-y-6">
      <div className="rounded-[14px] border border-[#dddddd] bg-[#f7f7f7] p-4 text-sm text-[#222222]">
        <div className="flex items-start gap-3">
          <Vault className="mt-0.5 h-5 w-5 flex-shrink-0 text-[#222222]" />
          <div>
            <div className="font-semibold">Platform escrow ledger</div>
            <p className="mt-1 text-[#6a6a6a]">
              Total money the platform is holding on behalf of hosts. Each guest
              payment goes into{" "}
              <span className="font-medium text-[#ffb400]">Pending</span> until the
              guest checks out, then moves to{" "}
              <span className="font-medium text-[#3a8b56]">Available</span> and can
              be paid out.
            </p>
          </div>
        </div>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <StatCard
          title="Total escrow (VND)"
          value={formatMoney(totalVnd, "VND")}
          icon={Wallet}
        />
        <StatCard
          title="Total escrow (USD)"
          value={formatMoney(totalUsd, "USD")}
          icon={Wallet}
        />
        <StatCard
          title="Active host wallets"
          value={totalItems.toLocaleString()}
          icon={Clock}
        />
      </div>

      <div className="flex justify-end">
        <Button
          variant="outline"
          size="sm"
          onClick={handleBootstrap}
          disabled={bootstrap.isPending}
        >
          <RefreshCw className="mr-2 h-4 w-4" />
          {bootstrap.isPending ? "Rebuilding..." : "Rebuild ledger"}
        </Button>
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
