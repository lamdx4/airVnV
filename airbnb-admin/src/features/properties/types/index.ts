export const PropertyStatus = {
  DRAFT: "Draft",
  PENDING_REVIEW: "PendingReview",
  PUBLISHED: "Published",
  SUSPENDED: "Suspended",
  ARCHIVED: "Archived",
  REJECTED: "Rejected",
} as const;

export type PropertyStatusValue = (typeof PropertyStatus)[keyof typeof PropertyStatus];

export interface Property {
  id: string;
  hostId: string;
  title: string;
  displayAddress: string;
  type: string;
  status: PropertyStatusValue;
  basePrice: number;
  coverImageUrl: string | null;
  guestCount: number;
  bedroomCount: number;
  bathroomCount: number;
  averageRating: number;
  reviewCount: number;
  suspensionReason: string | null;
  rejectionReason: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface PropertyListParams {
  page?: number;
  pageSize?: number;
  searchTerm?: string;
  status?: number;
  sortBy?: string;
  sortOrder?: string;
}

export const PropertyStatusEnum: Record<PropertyStatusValue, number> = {
  Draft: 0,
  PendingReview: 1,
  Published: 2,
  Suspended: 3,
  Archived: 4,
  Rejected: 5,
};
