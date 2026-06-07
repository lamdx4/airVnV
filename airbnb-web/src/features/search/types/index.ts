export interface AddressVODto {
  countryCode: string;
  admin1Code?: string;
  admin2Code?: string;
  displayAddress: string;
  latitude: number;
  longitude: number;
}

export interface PropertyDocDto {
  id: string;
  hostId: string;
  title: string;
  description: string;
  slug: string;
  location: string; // "lat, lon"
  propertyType: number;
  basePrice: number;
  averageRating: number;
  reviewCount: number;
  address: AddressVODto;
  createdAt: string;
}

export interface SearchResponseDto {
  totalCount: number;
  page: number;
  pageSize: number;
  results: PropertyDocDto[];
}

export interface PropertyMapMarker {
  id: string;
  title: string;
  latitude: number;
  longitude: number;
  price: number;
  currency: string;
  rating: number;
  displayAddress: string;
  thumbnail?: string;
  images?: string[];
}

export interface PagedSearchModel {
  totalCount: number;
  page: number;
  pageSize: number;
  items: PropertyMapMarker[];
}
