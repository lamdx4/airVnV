export interface PropertyDocDto {
  id: string;
  hostId: string;
  title: string;
  slug: string;
  location: string; // "lat, lon"
  basePrice: number;
  currencyCode: string;
  rating: number;
  reviewCount: number;
  thumbnailUrl?: string;
  propertyType?: string;
  distanceKm?: number;
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
  thumbnail?: string;
}

export interface PagedSearchModel {
  totalCount: number;
  page: number;
  pageSize: number;
  items: PropertyMapMarker[];
}
