import { PropertyStatus, PropertyStatusLabel } from "../types";
import type { PropertyStatusValue } from "../types";

const statusConfig: Record<
  PropertyStatusValue,
  { label: string; variant: "default" | "secondary" | "destructive" | "outline" }
> = {
  [PropertyStatus.DRAFT]: { label: PropertyStatusLabel[PropertyStatus.DRAFT], variant: "outline" },
  [PropertyStatus.PENDING_REVIEW]: { label: PropertyStatusLabel[PropertyStatus.PENDING_REVIEW], variant: "secondary" },
  [PropertyStatus.PUBLISHED]: { label: PropertyStatusLabel[PropertyStatus.PUBLISHED], variant: "default" },
  [PropertyStatus.SUSPENDED]: { label: PropertyStatusLabel[PropertyStatus.SUSPENDED], variant: "outline" },
  [PropertyStatus.ARCHIVED]: { label: PropertyStatusLabel[PropertyStatus.ARCHIVED], variant: "outline" },
  [PropertyStatus.REJECTED]: { label: PropertyStatusLabel[PropertyStatus.REJECTED], variant: "destructive" },
};

export function getPropertyStatusConfig(status: PropertyStatusValue) {
  return statusConfig[status] ?? { label: "Unknown", variant: "outline" as const };
}
