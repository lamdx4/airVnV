import { BookingStatus } from "../types";
import type { BookingStatusValue } from "../types";

const statusConfig: Record<
  BookingStatusValue,
  { label: string; variant: "default" | "secondary" | "destructive" | "outline" }
> = {
  [BookingStatus.PENDING]: { label: "Pending", variant: "outline" },
  [BookingStatus.CONFIRMED]: { label: "Confirmed", variant: "default" },
  [BookingStatus.CHECKED_IN]: { label: "Checked In", variant: "secondary" },
  [BookingStatus.CHECKED_OUT]: { label: "Checked Out", variant: "secondary" },
  [BookingStatus.CANCELLED]: { label: "Cancelled", variant: "destructive" },
  [BookingStatus.REFUNDED]: { label: "Refunded", variant: "outline" },
};

export function getBookingStatusConfig(status: BookingStatusValue) {
  return statusConfig[status];
}
