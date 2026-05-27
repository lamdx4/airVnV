import { api } from '@/lib/api';
import type { SearchResponseDto } from '../types';

export interface SearchQueryParams {
  latitude: number;
  longitude: number;
  radiusKm?: number;
  page?: number;
  pageSize?: number;
}

export const searchProperties = async (params: SearchQueryParams): Promise<SearchResponseDto> => {
  const query = new URLSearchParams({
    Latitude: params.latitude.toString(),
    Longitude: params.longitude.toString(),
    RadiusKm: (params.radiusKm || 50).toString(),
    Page: (params.page || 1).toString(),
    PageSize: (params.pageSize || 20).toString()
  });

  // Note: Assuming SearchService is exposed via YARP at /api/search
  const response = await api.get<any>(`/api/search?${query.toString()}`);
  return response as unknown as SearchResponseDto;
};
