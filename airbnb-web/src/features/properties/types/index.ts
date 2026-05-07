export const PropertyStatus = {
  Draft: 0,
  PendingReview: 1,
  Published: 2,
  Suspended: 3,
  Archived: 4
} as const;

export type PropertyStatus = typeof PropertyStatus[keyof typeof PropertyStatus];

export const ImageType = {
  Gallery: 0,
  Cover: 1
} as const;

export type ImageType = typeof ImageType[keyof typeof ImageType];

export interface PropertyImage {
  id: string;
  url: string;
  type: ImageType;
  displayOrder: number;
}

export interface PropertyAmenity {
  amenityId: string;
  name: string;
  iconCode: string;
  category: string;
  additionalInfo?: string;
}

export interface PropertyAvailability {
  id: string;
  startDate: string; // ISO Date yyyy-MM-dd
  endDate: string;
  type: number;
  note?: string;
}

export interface Property {
  id: string;
  hostId: string;
  title: string;
  description: string;
  slug: string;
  status: PropertyStatus;
  
  // Location
  latitude: number;
  longitude: number;
  displayAddress: string;
  countryCode: string;
  subDivisions?: Record<string, string>;
  
  // Details
  basePrice: number;
  cleaningFee: number;
  serviceFee: number;
  weekendPremiumPercent: number;
  currencyCode: string;
  
  // Rules
  allowPets: boolean;
  allowSmoking: boolean;
  allowEvents: boolean;
  checkInTime: string;
  checkOutTime: string;
  flexibleCheckOut: boolean;
  
  images: PropertyImage[];
  propertyAmenities: PropertyAmenity[];
  availabilities: PropertyAvailability[];
  
  createdAt: string;
  updatedAt?: string;
}

export interface Amenity {
  id: string;
  name: string;
  description: string;
  iconCode: string;
  category: string;
}

export interface EditPropertyInput {
  title: string;
  description: string;
  pricing: {
    basePrice: number;
    cleaningFee: number;
  };
  houseRules: {
    allowPets: boolean;
    allowSmoking: boolean;
    allowEvents: boolean;
    checkInTime: string;
    checkOutTime: string;
    flexibleCheckOut: boolean;
  };
}

export interface CreatePropertyFormData {
  title: string;
  description: string;
  basePrice: number;
  latitude: number;
  longitude: number;
  displayAddress: string;
}

export interface CreatePropertyRequest {
  title: string;
  description: string;
  slug: string;
  latitude: number;
  longitude: number;
  countryCode: string;
  displayAddress: string;
  streetAddress: string;
  unit?: string;
  postalCode?: string;
  admin1Code?: string;
  admin2Code?: string;
  pricing: {
    basePrice: number;
    currencyCode: string;
    cleaningFee: number;
    serviceFee: number;
    weekendPremiumPercent: number;
  };
  capacity: {
    guestCount: number;
    bedroomCount: number;
    bedCount: number;
    bathroomCount: number;
  };
  houseRules: {
    allowPets: boolean;
    allowSmoking: boolean;
    allowEvents: boolean;
    checkInTime: string;
    checkOutTime: string;
    flexibleCheckOut: boolean;
  };
}
