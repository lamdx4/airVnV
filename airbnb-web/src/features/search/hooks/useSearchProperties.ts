import { useQuery } from '@tanstack/react-query';
import { searchProperties } from '../api';
import type { SearchQueryParams } from '../api';
import { mapSearchResponseToModel } from '../utils/mapper';

import { propertiesApi } from '../../properties/api/properties';

export const useSearchProperties = (params: SearchQueryParams, enabled: boolean = true) => {
  return useQuery({
    queryKey: ['searchProperties', params.latitude, params.longitude, params.radiusKm, params.page, params.pageSize, params.propertyType],
    queryFn: async () => {
      // 1. Get IDs from Search Service
      const searchDto = await searchProperties(params);
      
      if (!searchDto.results || searchDto.results.length === 0) {
        return mapSearchResponseToModel(searchDto);
      }

      // 2. Fetch Rich Data from Property Service
      const ids = searchDto.results.map(r => r.id);
      const richProperties = await propertiesApi.getPropertiesByIds(ids);

      // 3. Map to PropertyMapMarker using rich data while preserving Elasticsearch order
      const richItems = searchDto.results.map(searchResult => {
        // Parse lat/lon if address is missing
        let lat = searchResult.address?.latitude || 0;
        let lon = searchResult.address?.longitude || 0;
        if (!lat && !lon && searchResult.location) {
          const parts = searchResult.location.split(',');
          if (parts.length === 2) {
            lat = parseFloat(parts[0].trim());
            lon = parseFloat(parts[1].trim());
          }
        }

        const richData = richProperties.find(rp => rp.id === searchResult.id);
        
        return {
          id: searchResult.id,
          title: searchResult.title,
          price: searchResult.basePrice,
          currency: richData?.currency || 'VND',
          rating: searchResult.averageRating,
          displayAddress: searchResult.address?.displayAddress || '',
          latitude: lat,
          longitude: lon,
          thumbnail: richData?.images?.[0], // fallback thumbnail
          images: richData?.images || [],
        };
      });

      return {
        totalCount: searchDto.totalCount,
        page: searchDto.page,
        pageSize: searchDto.pageSize,
        items: richItems,
      };
    },
    enabled: enabled,
    staleTime: 60 * 1000,
  });
};
