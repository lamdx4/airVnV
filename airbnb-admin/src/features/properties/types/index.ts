export const PropertyStatus = {
  DRAFT: 0,
  PENDING_REVIEW: 1,
  PUBLISHED: 2,
  SUSPENDED: 3,
  ARCHIVED: 4,
  REJECTED: 5,
} as const;

export type PropertyStatusValue = (typeof PropertyStatus)[keyof typeof PropertyStatus];

export interface PropertyImage {
  url: string;
  displayOrder: number;
}

export interface PropertyAmenity {
  name: string;
  category: string;
}

export interface Property {
  id: string;
  hostId: string;
  title: string;
  displayAddress: string;
  streetAddress: string;
  description: string;
  type: number;
  status: PropertyStatusValue;
  basePrice: number;
  coverImageUrl: string | null;
  images: PropertyImage[];
  amenities: PropertyAmenity[];
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

export const PropertyStatusLabel: Record<PropertyStatusValue, string> = {
  [PropertyStatus.DRAFT]: "Draft",
  [PropertyStatus.PENDING_REVIEW]: "Pending Review",
  [PropertyStatus.PUBLISHED]: "Published",
  [PropertyStatus.SUSPENDED]: "Suspended",
  [PropertyStatus.ARCHIVED]: "Archived",
  [PropertyStatus.REJECTED]: "Rejected",
};

export const PropertyTypeEnum: Record<number, string> = {
  1: "Apartment",
  2: "House",
  3: "Villa",
  4: "Homestay",
  5: "Hotel",
  6: "Resort",
};
