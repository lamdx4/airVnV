import { PayoutStatus, PayoutStatusLabel } from "../types";
import type { PayoutStatusValue } from "../types";

const statusConfig: Record<
  PayoutStatusValue,
  { label: string; variant: "default" | "secondary" | "destructive" | "outline" | "success" | "warning" | "info" }
> = {
  [PayoutStatus.PENDING]: { label: "Pending", variant: "warning" },
  [PayoutStatus.APPROVED]: { label: "Approved", variant: "info" },
  [PayoutStatus.PROCESSING]: { label: "Processing", variant: "default" },
  [PayoutStatus.COMPLETED]: { label: "Completed", variant: "success" },
  [PayoutStatus.FAILED]: { label: "Failed", variant: "destructive" },
  [PayoutStatus.CANCELLED]: { label: "Cancelled", variant: "secondary" },
};

export function getPayoutStatusConfig(status: number) {
  return statusConfig[status as PayoutStatusValue] ?? { label: PayoutStatusLabel[status] ?? "Unknown", variant: "outline" as const };
}
