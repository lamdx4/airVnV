import { PropertyStatus } from "../types";
import type { PropertyStatusValue } from "../types";

const statusConfig: Record<
  PropertyStatusValue,
  { label: string; variant: "default" | "secondary" | "destructive" | "outline" }
> = {
  [PropertyStatus.DRAFT]: { label: "Draft", variant: "outline" },
  [PropertyStatus.PENDING_REVIEW]: { label: "Pending Review", variant: "secondary" },
  [PropertyStatus.PUBLISHED]: { label: "Published", variant: "default" },
  [PropertyStatus.SUSPENDED]: { label: "Suspended", variant: "outline" },
  [PropertyStatus.ARCHIVED]: { label: "Archived", variant: "outline" },
  [PropertyStatus.REJECTED]: { label: "Rejected", variant: "destructive" },
};

export function getPropertyStatusConfig(status: PropertyStatusValue) {
  return statusConfig[status];
}
