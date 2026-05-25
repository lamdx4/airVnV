export interface PendingProperty {
  id: string;
  title: string;
  thumbnailUrl: string;
  hostName: string;
  submittedAt: Date;
  status: 'PendingReview' | 'Published' | 'Suspended' | 'Archived';
}

export interface PropertyDetail {
  id: string;
  hostId: string;
  title: string;
  description: string;
  slug: string;
  status: string;
  latitude: number;
  longitude: number;
  displayAddress: string;
  countryCode: string;
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
  };
  images: {
    id: string;
    url: string;
    type: number;
    displayOrder: number;
  }[];
  propertyAmenities: {
    amenityId: string;
    name: string;
    iconCode: string;
    category: string;
    additionalInfo?: string;
  }[];
  availabilities: {
    id: string;
    startDate: string;
    endDate: string;
    type: number;
    note?: string;
  }[];
  createdAt: string;
  updatedAt?: string;
}

export interface AdminActionResponse {
  propertyId: string;
  newStatus: string;
  actionAt: string;
  adminId: string;
}