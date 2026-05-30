export const PropertyStatus = {
  Draft: 0,
  PendingReview: 1,
  Published: 2,
  Suspended: 3,
  Archived: 4
} as const;

export type PropertyStatus = typeof PropertyStatus[keyof typeof PropertyStatus];

export const ImageType = {
  Cover: 0,
  Gallery: 1,
  Room: 2,
  Bathroom: 3,
  View: 4
} as const;

export type ImageType = typeof ImageType[keyof typeof ImageType];

export const PropertyType = {
  Apartment: 1,
  House: 2,
  Villa: 3,
  Homestay: 4,
  Hotel: 5,
  Resort: 6
} as const;

export type PropertyType = typeof PropertyType[keyof typeof PropertyType];

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
  type: PropertyType;
  status: PropertyStatus;
  
  // Location
  latitude: number;
  longitude: number;
  displayAddress: string;
  countryCode: string;
  streetAddress?: string;
  subDivisions?: Record<string, string>;
  
  // Nested Objects matching Backend DTOs
  pricing: {
    basePrice: number;
    cleaningFee: number;
    serviceFee: number;
    weekendPremiumPercent: number;
    currencyCode: string;
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
    customRules?: string[];
  };
  
  images: PropertyImage[];
  propertyAmenities: PropertyAmenity[];
  availabilities: PropertyAvailability[];
  
  createdAt: string;
  updatedAt?: string;
}

export interface PropertySummary {
  id: string;
  title: string;
  displayAddress: string;
  type: PropertyType;
  status: PropertyStatus;
  basePrice: number;
  coverImageUrl?: string;
  guestCount: number;
  bedroomCount: number;
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
  type: number;
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
    customRules?: string[];
  };
}

export interface UpdateLocationRequest {
  latitude: number;
  longitude: number;
  countryCode: string;
  displayAddress: string;
  streetAddress: string;
  unit?: string;
  postalCode?: string;
  subDivisions?: Record<string, string>;
  admin1Code?: string;
  admin2Code?: string;
}

export interface CreatePropertyFormData {
  title: string;
  description: string;
  type: number;
  
  // Pricing
  basePrice: number;
  cleaningFee: number;
  serviceFee: number;
  weekendPremiumPercent: number;
  currencyCode: string;
  
  // Location
  latitude: number;
  longitude: number;
  displayAddress: string;
  streetAddress: string;
  countryCode: string;
  admin1Code?: string;
  admin2Code?: string;

  // Capacity
  guestCount: number;
  bedroomCount: number;
  bedCount: number;
  bathroomCount: number;
  
  // House Rules
  allowPets: boolean;
  allowSmoking: boolean;
  allowEvents: boolean;
  checkInTime: string;
  checkOutTime: string;
  flexibleCheckOut: boolean;
  customRules?: string[];
}

export interface CreatePropertyRequest {
  title: string;
  description: string;
  slug: string;
  type: number;
  
  // Pricing (Flattened)
  basePrice: number;
  currencyCode: string;
  cleaningFee: number;
  serviceFee: number;
  weekendPremiumPercent: number;

  // Capacity (Flattened)
  guestCount: number;
  bedroomCount: number;
  bedCount: number;
  bathroomCount: number;

  // HouseRules (Flattened)
  allowPets: boolean;
  allowSmoking: boolean;
  allowEvents: boolean;
  checkInTime: string;
  checkOutTime: string;
  flexibleCheckOut: boolean;
  customRules?: string[];

  // Location
  latitude: number;
  longitude: number;
  displayAddress: string;
  countryCode: string;
  streetAddress: string;
  admin1Code?: string;
  admin2Code?: string;
  unit?: string;
  subDivisions?: Record<string, string>;
  amenityIds?: string[];
  imageMetadata?: { fileName: string; type: number }[];
}

export interface AddressFieldConfig {
  id: string;
  label: string;
  photonKeys: string[];
  isRequired: boolean;
  type?: string;
}

export interface TaxDto {
  type: string;
  rate: number;
}

export interface PaymentGatewayDto {
  provider: string;
  supportedCurrencies: string[];
}

export interface CountryMasterData {
  countryCode: string;
  name: string;
  nativeCurrency: string;
  isSupported: boolean;
  defaultLatitude: number;
  defaultLongitude: number;
  taxes: TaxDto[];
  paymentGateways: PaymentGatewayDto[];
  addressFormConfig?: AddressFieldConfig[];
}

export interface SupportedCountry {
  code: string;
  name: string;
  nativeCurrency: string;
  defaultLatitude: number;
  defaultLongitude: number;
}
