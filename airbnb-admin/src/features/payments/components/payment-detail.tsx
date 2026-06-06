"use client";

import { useRouter } from "next/navigation";
import { ArrowLeft, ExternalLink } from "lucide-react";

import { Breadcrumbs, type BreadcrumbItem } from "@/components/layout/breadcrumbs";
import { PageLoader } from "@/components/common/loading";
import { ErrorDisplay } from "@/components/common/error-display";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

import { usePayment } from "../hooks";
import { getPaymentStatusConfig } from "../utils/status";

interface PaymentDetailProps {
  paymentId: string;
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
  return new Intl.DateTimeFormat("en-US", { dateStyle: "medium", timeStyle: "short" }).format(
    new Date(iso),
  );
}

export function PaymentDetail({ paymentId }: PaymentDetailProps) {
  const router = useRouter();
  const { data, isLoading, isError, refetch } = usePayment(paymentId);

  if (isLoading) return <PageLoader text="Loading payment..." />;
  if (isError || !data) {
    return <ErrorDisplay message="Payment not found" onRetry={() => refetch()} />;
  }

  const statusConfig = getPaymentStatusConfig(data.status);
  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Payments", href: "/payments" },
    { label: data.id.slice(0, 8) },
  ];

  return (
    <div className="space-y-6">
      <div className="space-y-2">
        <Breadcrumbs items={breadcrumbs} />
        <div className="flex items-center gap-3">
          <Button variant="ghost" size="sm" onClick={() => router.push("/payments")}>
            <ArrowLeft className="mr-1 h-4 w-4" />
            Back
          </Button>
          <h1 className="text-[28px] font-bold text-[#222222]">Payment Detail</h1>
          <Badge variant={statusConfig.variant}>{statusConfig.label}</Badge>
        </div>
        <p className="text-sm text-[#6a6a6a]">
          Created {formatDateTime(data.createdAt)}
          {data.expiresAt && <> · Expires {formatDateTime(data.expiresAt)}</>}
        </p>
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Transaction</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3 text-sm">
            <Row label="Payment ID" value={data.id} mono />
            <Row label="Booking ID" value={data.bookingId} mono />
            <Row
              label="Transaction ID"
              value={data.transactionId ?? "—"}
              mono={Boolean(data.transactionId)}
            />
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Amount</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3 text-sm">
            <Row label="Amount" value={formatMoney(data.amount, data.currency)} />
            <Row label="Currency" value={data.currency} />
            <Row label="Status" value={statusConfig.label} />
          </CardContent>
        </Card>
      </div>

      {data.paymentUrl && (
        <Card>
          <CardHeader>
            <CardTitle>Provider link</CardTitle>
          </CardHeader>
          <CardContent>
            <a
              href={data.paymentUrl}
              target="_blank"
              rel="noopener noreferrer"
              className="inline-flex items-center gap-2 text-sm font-medium text-[#ff385c] hover:underline"
            >
              Open hosted payment page
              <ExternalLink className="h-4 w-4" />
            </a>
          </CardContent>
        </Card>
      )}
    </div>
  );
}

function Row({ label, value, mono }: { label: string; value: string; mono?: boolean }) {
  return (
    <div className="flex items-start justify-between gap-3">
      <span className="text-[#6a6a6a]">{label}</span>
      <span
        className={mono ? "break-all text-right font-mono text-xs text-[#222222]" : "text-right font-medium text-[#222222]"}
      >
        {value}
      </span>
    </div>
  );
}
