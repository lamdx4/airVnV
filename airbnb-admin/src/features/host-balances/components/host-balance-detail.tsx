"use client";

import { useRouter } from "next/navigation";
import {
  ArrowLeft,
  ArrowRightLeft,
  Banknote,
  CheckCircle2,
  CreditCard,
  Minus,
  Plus,
  Undo2,
  Wrench,
} from "lucide-react";

import { Breadcrumbs, type BreadcrumbItem } from "@/components/layout/breadcrumbs";
import { PageLoader } from "@/components/common/loading";
import { ErrorDisplay } from "@/components/common/error-display";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

import { useHostBalance } from "../hooks";
import type { BalanceEntry, BalanceEntryTypeValue } from "../types";

interface Props {
  balanceId: string;
}

function formatMoney(amount: number, currency: string) {
  try {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency,
      maximumFractionDigits: 2,
      signDisplay: amount === 0 ? "auto" : "exceptZero",
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

const ENTRY_CONFIG: Record<
  BalanceEntryTypeValue,
  { label: string; icon: React.ComponentType<{ className?: string }>; color: string }
> = {
  PaymentReceived: { label: "Payment received", icon: CreditCard, color: "#ffb400" },
  BookingCheckedOut: { label: "Booking checked-out", icon: ArrowRightLeft, color: "#3a8b56" },
  PayoutApproved: { label: "Payout approved", icon: Banknote, color: "#0078d4" },
  Refund: { label: "Refund", icon: Undo2, color: "#c13515" },
  Adjustment: { label: "Adjustment", icon: Wrench, color: "#6a6a6a" },
};

function EntryRow({ entry, currency }: { entry: BalanceEntry; currency: string }) {
  const cfg = ENTRY_CONFIG[entry.type];
  const Icon = cfg.icon;

  return (
    <div className="flex items-start gap-3 border-b border-[#f0f0f0] py-3 last:border-0">
      <div
        className="mt-0.5 flex h-8 w-8 flex-shrink-0 items-center justify-center rounded-full"
        style={{ backgroundColor: `${cfg.color}1a`, color: cfg.color }}
      >
        <Icon className="h-4 w-4" />
      </div>
      <div className="min-w-0 flex-1">
        <div className="flex items-center gap-2">
          <span className="text-sm font-medium text-[#222222]">{cfg.label}</span>
          <span className="text-xs text-[#6a6a6a]">{formatDateTime(entry.createdAt)}</span>
        </div>
        {entry.note && (
          <p className="mt-0.5 text-xs text-[#6a6a6a]">{entry.note}</p>
        )}
        <div className="mt-1 flex flex-wrap gap-2 text-[11px] text-[#6a6a6a]">
          {entry.paymentId && (
            <span className="font-mono">payment {shortenId(entry.paymentId)}</span>
          )}
          {entry.payoutId && (
            <span className="font-mono">payout {shortenId(entry.payoutId)}</span>
          )}
          {entry.bookingId && (
            <span className="font-mono">booking {shortenId(entry.bookingId)}</span>
          )}
        </div>
      </div>
      <div className="flex flex-shrink-0 flex-col items-end text-xs">
        {entry.pendingDelta !== 0 && (
          <div className="flex items-center gap-1 text-[#ffb400]">
            {entry.pendingDelta > 0 ? (
              <Plus className="h-3 w-3" />
            ) : (
              <Minus className="h-3 w-3" />
            )}
            <span className="font-semibold">
              {formatMoney(Math.abs(entry.pendingDelta), currency)}
            </span>
            <span>pending</span>
          </div>
        )}
        {entry.availableDelta !== 0 && (
          <div className="flex items-center gap-1 text-[#3a8b56]">
            {entry.availableDelta > 0 ? (
              <Plus className="h-3 w-3" />
            ) : (
              <Minus className="h-3 w-3" />
            )}
            <span className="font-semibold">
              {formatMoney(Math.abs(entry.availableDelta), currency)}
            </span>
            <span>available</span>
          </div>
        )}
      </div>
    </div>
  );
}

export function HostBalanceDetail({ balanceId }: Props) {
  const router = useRouter();
  const { data, isLoading, isError, refetch } = useHostBalance(balanceId);

  if (isLoading) return <PageLoader text="Loading balance..." />;
  if (isError || !data) {
    return <ErrorDisplay message="Balance not found" onRetry={() => refetch()} />;
  }

  const displayName = data.hostName ?? "Unknown host";
  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Host Balances", href: "/host-balances" },
    { label: displayName },
  ];

  return (
    <div className="space-y-6">
      <div className="space-y-2">
        <Breadcrumbs items={breadcrumbs} />
        <div className="flex flex-wrap items-center gap-3">
          <Button variant="ghost" size="sm" onClick={() => router.push("/host-balances")}>
            <ArrowLeft className="mr-1 h-4 w-4" />
            Back
          </Button>
          <h1 className="text-[28px] font-bold text-[#222222]">{displayName}</h1>
          <Badge variant="secondary">{data.currency}</Badge>
        </div>
        <p className="text-sm text-[#6a6a6a]">
          {data.hostEmail && <>{data.hostEmail} · </>}
          <span className="font-mono text-xs">{data.hostId}</span> · Last updated{" "}
          {formatDateTime(data.updatedAt)}
        </p>
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle className="text-sm font-medium text-[#6a6a6a]">
              Pending balance
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-[#ffb400]">
              {formatMoney(data.pendingBalance, data.currency)}
            </div>
            <p className="mt-1 text-xs text-[#6a6a6a]">
              Guest already paid — held until check-out / dispute window passes
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="text-sm font-medium text-[#6a6a6a]">
              Available balance
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-[#3a8b56]">
              {formatMoney(data.availableBalance, data.currency)}
            </div>
            <p className="mt-1 text-xs text-[#6a6a6a]">
              Ready to be paid out to host's bank account
            </p>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>Ledger ({data.entries.length})</CardTitle>
          <CheckCircle2 className="h-5 w-5 text-[#3a8b56]" />
        </CardHeader>
        <CardContent>
          {data.entries.length === 0 ? (
            <p className="py-8 text-center text-sm text-[#6a6a6a]">No entries yet.</p>
          ) : (
            <div>
              {data.entries.map((entry) => (
                <EntryRow key={entry.id} entry={entry} currency={data.currency} />
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
