export enum PropertyStatus {
  Draft = 0,
  PendingReview = 1,
  Published = 2,
  Suspended = 3,
  Archived = 4
}

export enum ImageType {
  Cover = 0,
  Gallery = 1,
  Bedroom = 2,
  Bathroom = 3,
  LivingRoom = 4,
  Kitchen = 5,
  Outdoor = 6,
  Other = 7
}

export interface Pricing {
  basePrice: number;
  currencyCode: string;
  cleaningFee: number;
  serviceFee: number;
  weekendPremiumPercent: number;
}

export interface PropertyCapacity {
  guestCount: number;
  bedroomCount: number;
  bedCount: number;
  bathroomCount: number;
}

export interface HouseRules {
  allowPets: boolean;
  allowSmoking: boolean;
  allowEvents: boolean;
  checkInTime: string;
  checkOutTime: string;
  flexibleCheckOut: boolean;
}

export interface PropertyImage {
  id: string;
  url: string;
  publicId: string;
  type: ImageType;
  order: number;
}

export interface PropertyAmenity {
  amenityId: string;
  name: string;
  icon?: string;
  category?: string;
  additionalInfo?: string;
}

export interface PropertyDTO {
  id: string;
  hostId: string;
  title: string;
  description: string;
  slug: string;
  latitude: number;
  longitude: number;
  countryCode: string;
  displayAddress: string;
  status: PropertyStatus;
  pricing: Pricing;
  capacity: PropertyCapacity;
  houseRules: HouseRules;
  images: PropertyImage[];
  amenities: PropertyAmenity[];
  createdAt: string;
  updatedAt?: string;
}

// --- Form & Request Types ---
export interface PaginationParams {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string;
  status?: number;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface CreatePropertyFormData {
  title: string;
  description: string;
  basePrice: number;
  displayAddress: string;
  latitude: number;
  longitude: number;
}

export type CreatePropertyRequest = Omit<PropertyDTO, 'id' | 'hostId' | 'status' | 'images' | 'amenities' | 'createdAt' | 'updatedAt'>;

// --- UI Model ---
export interface PropertyModel extends PropertyDTO {
  isUpdating?: boolean;
}
