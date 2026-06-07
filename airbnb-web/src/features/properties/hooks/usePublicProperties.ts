import { useQuery } from '@tanstack/react-query';
import { propertiesApi } from '../api/properties';
import type { PropertyBasicInfo } from '../types';

export const usePublicProperties = (page: number = 1, pageSize: number = 20, propertyType?: number) => {
  return useQuery({
    queryKey: ['publicProperties', page, pageSize, propertyType],
    queryFn: async () => {
      const response = await propertiesApi.getPublicProperties(page, pageSize, propertyType);
      
      const mappedItems = response.items.map((rp: PropertyBasicInfo) => ({
        id: rp.id,
        title: rp.title,
        price: rp.price,
        currency: rp.currency || 'VND',
        rating: rp.rating,
        displayAddress: rp.displayAddress,
        latitude: rp.latitude,
        longitude: rp.longitude,
        thumbnail: rp.images?.[0], 
        images: rp.images || [],
      }));

      return {
        totalCount: response.totalCount,
        page,
        pageSize,
        items: mappedItems,
      };
    },
    staleTime: 60 * 1000,
  });
};
