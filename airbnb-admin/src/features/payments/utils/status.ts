import { PaymentStatus, PaymentStatusLabel } from "../types";
import type { PaymentStatusValue } from "../types";

const statusConfig: Record<
  PaymentStatusValue,
  { label: string; variant: "default" | "secondary" | "destructive" | "outline" | "success" | "warning" | "info" }
> = {
  [PaymentStatus.PENDING]: { label: "Pending", variant: "warning" },
  [PaymentStatus.SUCCESS]: { label: "Success", variant: "success" },
  [PaymentStatus.FAILED]: { label: "Failed", variant: "destructive" },
  [PaymentStatus.EXPIRED]: { label: "Expired", variant: "secondary" },
  [PaymentStatus.REFUNDED]: { label: "Refunded", variant: "info" },
  [PaymentStatus.PARTIALLY_REFUNDED]: { label: "Partially Refunded", variant: "info" },
};

export function getPaymentStatusConfig(status: number) {
  return statusConfig[status as PaymentStatusValue] ?? { label: PaymentStatusLabel[status] ?? "Unknown", variant: "outline" as const };
}
