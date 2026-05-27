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
    latitude: lat,
    longitude: lon,
    price: dto.basePrice,
    currency: dto.currencyCode || 'VND',
    rating: dto.rating,
    thumbnail: dto.thumbnailUrl,
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
