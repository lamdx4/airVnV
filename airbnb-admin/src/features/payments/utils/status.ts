import type { PaymentStatusValue } from "../types";

type BadgeVariant = "default" | "secondary" | "destructive" | "outline";

export function getPaymentStatusConfig(status: PaymentStatusValue): {
  label: string;
  variant: BadgeVariant;
} {
  switch (status) {
    case "Success":
      return { label: "Success", variant: "default" };
    case "Pending":
      return { label: "Pending", variant: "secondary" };
    case "Failed":
      return { label: "Failed", variant: "destructive" };
    case "Expired":
      return { label: "Expired", variant: "outline" };
    case "Refunded":
      return { label: "Refunded", variant: "outline" };
    case "PartiallyRefunded":
      return { label: "Part. refunded", variant: "outline" };
    default:
      return { label: status, variant: "outline" };
  }
}
