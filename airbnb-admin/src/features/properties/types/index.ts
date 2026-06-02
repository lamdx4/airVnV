export const PropertyStatus = {
  DRAFT: "Draft",
  PENDING_REVIEW: "PendingReview",
  ACTIVE: "Active",
  INACTIVE: "Inactive",
  REJECTED: "Rejected",
} as const;

export type PropertyStatusValue = (typeof PropertyStatus)[keyof typeof PropertyStatus];

export interface Property {
  id: string;
  title: string;
  hostId: string;
  hostName: string;
  propertyType: string;
  status: PropertyStatusValue;
  city: string;
  country: string;
  pricePerNight: number;
  maxGuests: number;
  bedrooms: number;
  bathrooms: number;
  rating: number;
  reviewCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface PropertyListParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: PropertyStatusValue;
  city?: string;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}
