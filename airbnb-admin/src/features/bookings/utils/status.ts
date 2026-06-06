import { BookingStatus } from "../types";
import type { BookingStatusValue } from "../types";

const statusConfig: Record<
  BookingStatusValue,
  { label: string; variant: "default" | "secondary" | "destructive" | "outline" | "success" | "warning" | "info" }
> = {
  [BookingStatus.PENDING]:     { label: "Pending",           variant: "warning" },
  [BookingStatus.AWAITING]:    { label: "Awaiting Approval", variant: "warning" },
  [BookingStatus.CONFIRMED]:   { label: "Confirmed",         variant: "success" },
  [BookingStatus.CHECKED_IN]:  { label: "Checked In",        variant: "info" },
  [BookingStatus.CHECKED_OUT]: { label: "Checked Out",       variant: "secondary" },
  [BookingStatus.CANCELLED]:   { label: "Cancelled",         variant: "destructive" },
  [BookingStatus.REFUNDED]:    { label: "Refunded",          variant: "outline" },
};

export function getBookingStatusConfig(status: BookingStatusValue) {
  return statusConfig[status] ?? { label: status, variant: "default" as const };
}
