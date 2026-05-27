import { useQuery } from '@tanstack/react-query';
import { searchProperties } from '../api';
import type { SearchQueryParams } from '../api';
import { mapSearchResponseToModel } from '../utils/mapper';

export const useSearchProperties = (params: SearchQueryParams, enabled: boolean = true) => {
  return useQuery({
    queryKey: ['searchProperties', params.latitude, params.longitude, params.radiusKm, params.page, params.pageSize],
    queryFn: async () => {
      const dto = await searchProperties(params);
      return mapSearchResponseToModel(dto);
    },
    enabled: enabled && params.latitude !== 0 && params.longitude !== 0,
    staleTime: 60 * 1000, // Cache in UI for 1 minute (matches Redis cache)
  });
};
