import type { PayoutStatusValue } from "../types";

type BadgeVariant = "default" | "secondary" | "destructive" | "outline";

export function getPayoutStatusConfig(status: PayoutStatusValue): {
  label: string;
  variant: BadgeVariant;
} {
  switch (status) {
    case "Pending":
      return { label: "Pending", variant: "secondary" };
    case "Approved":
      return { label: "Approved", variant: "default" };
    case "Processing":
      return { label: "Processing", variant: "default" };
    case "Completed":
      return { label: "Completed", variant: "default" };
    case "Failed":
      return { label: "Failed", variant: "destructive" };
    case "Cancelled":
      return { label: "Cancelled", variant: "outline" };
    default:
      return { label: status, variant: "outline" };
  }
}
