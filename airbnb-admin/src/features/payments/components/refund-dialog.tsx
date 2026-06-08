"use client";

import { useState } from "react";
import { toast } from "sonner";

import { ConfirmDialog } from "@/components/common/confirm-dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { getApiErrorMessage } from "@/lib/utils/helpers";

import { useRefundPayment } from "../hooks";

interface RefundDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  paymentId: string;
  remainingAmount: number;
  currency: string;
}

function formatMoney(amount: number, currency: string) {
  try {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency,
      maximumFractionDigits: currency === "VND" ? 0 : 2,
    }).format(amount);
  } catch {
    return `${amount.toFixed(2)} ${currency}`;
  }
}

export function RefundDialog({
  open,
  onOpenChange,
  paymentId,
  remainingAmount,
  currency,
}: RefundDialogProps) {
  const [amount, setAmount] = useState<string>(String(remainingAmount));
  const [reason, setReason] = useState("");
  const refund = useRefundPayment(paymentId);

  const numericAmount = Number(amount);
  const isValid =
    numericAmount > 0 && numericAmount <= remainingAmount && reason.trim().length >= 3;

  function handleConfirm() {
    if (!isValid) {
      toast.error("Enter a valid amount and a reason (min 3 chars).");
      return;
    }
    refund.mutate(
      { amount: numericAmount, reason: reason.trim() },
      {
        onSuccess: (result) => {
          toast.success(
            result.isFullRefund
              ? "Full refund issued."
              : `Partial refund of ${formatMoney(result.refundedNow, currency)} issued.`,
          );
          setReason("");
          onOpenChange(false);
        },
        onError: (err) => toast.error(getApiErrorMessage(err) ?? "Refund failed."),
      },
    );
  }

  return (
    <ConfirmDialog
      open={open}
      onOpenChange={onOpenChange}
      title="Issue refund"
      description={`Refundable up to ${formatMoney(remainingAmount, currency)}. Host's ledger will be deducted proportionally; if already paid out, refund will be rejected.`}
      confirmLabel="Refund"
      variant="destructive"
      isLoading={refund.isPending}
      onConfirm={handleConfirm}
    >
      <div className="space-y-3">
        <div className="space-y-1.5">
          <Label htmlFor="refund-amount">Amount ({currency})</Label>
          <Input
            id="refund-amount"
            type="number"
            min={0}
            max={remainingAmount}
            step={currency === "VND" ? 1000 : 0.01}
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
            disabled={refund.isPending}
          />
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="refund-reason">Reason</Label>
          <Textarea
            id="refund-reason"
            placeholder="e.g. Guest cancelled within free-cancellation window"
            value={reason}
            onChange={(e) => setReason(e.target.value)}
            disabled={refund.isPending}
            rows={3}
          />
        </div>
      </div>
    </ConfirmDialog>
  );
}
