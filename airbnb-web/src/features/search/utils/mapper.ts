import type { PropertyDocDto, SearchResponseDto, PagedSearchModel, PropertyMapMarker } from '../types';

export const mapPropertyDocToModel = (dto: PropertyDocDto): PropertyMapMarker => {
  // Parse location "lat,lon"
  let lat = 0;
  let lon = 0;
  if (dto.location) {
    const parts = dto.location.split(',');
    if (parts.length === 2) {
      lat = parseFloat(parts[0].trim());
      lon = parseFloat(parts[1].trim());
    }
  }

  return {
    id: dto.id,
    title: dto.title,
    latitude: dto.address?.latitude || lat,
    longitude: dto.address?.longitude || lon,
    price: dto.basePrice,
    currency: 'VND', // SearchService does not return currency, use default VND
    rating: dto.averageRating,
    displayAddress: dto.address?.displayAddress || '',
  };
};

export const mapSearchResponseToModel = (dto: SearchResponseDto): PagedSearchModel => {
  return {
    totalCount: dto.totalCount,
    page: dto.page,
    pageSize: dto.pageSize,
    items: dto.results.map(mapPropertyDocToModel),
  };
};
