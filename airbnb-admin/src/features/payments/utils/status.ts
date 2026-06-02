import { PaymentStatus } from "../types";
import type { PaymentStatusValue } from "../types";

const statusConfig: Record<
  PaymentStatusValue,
  { label: string; variant: "default" | "secondary" | "destructive" | "outline" }
> = {
  [PaymentStatus.PENDING]: { label: "Pending", variant: "outline" },
  [PaymentStatus.COMPLETED]: { label: "Completed", variant: "default" },
  [PaymentStatus.FAILED]: { label: "Failed", variant: "destructive" },
  [PaymentStatus.REFUNDED]: { label: "Refunded", variant: "secondary" },
  [PaymentStatus.PARTIALLY_REFUNDED]: { label: "Partially Refunded", variant: "secondary" },
};

export function getPaymentStatusConfig(status: PaymentStatusValue) {
  return statusConfig[status];
}
