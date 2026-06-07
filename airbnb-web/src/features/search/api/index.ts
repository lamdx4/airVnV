import { api } from '@/lib/api';
import type { SearchResponseDto } from '../types';

export interface SearchQueryParams {
  latitude?: number;
  longitude?: number;
  radiusKm?: number;
  propertyType?: number;
  page?: number;
  pageSize?: number;
}

export const searchProperties = async (params: SearchQueryParams): Promise<SearchResponseDto> => {
  const query = new URLSearchParams({
    Page: (params.page || 1).toString(),
    PageSize: (params.pageSize || 20).toString()
  });

  if (params.latitude !== undefined && params.longitude !== undefined) {
    query.append('Latitude', params.latitude.toString());
    query.append('Longitude', params.longitude.toString());
    query.append('RadiusKm', (params.radiusKm || 50).toString());
  }

  if (params.propertyType) {
    query.append('PropertyType', params.propertyType.toString());
  }

  // Note: Assuming SearchService is exposed via YARP at /api/search
  const response = await api.get<any>(`/api/search?${query.toString()}`);
  return response as unknown as SearchResponseDto;
};
