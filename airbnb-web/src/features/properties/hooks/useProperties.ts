import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { propertiesApi } from '../api/properties';
import type { CreatePropertyRequest, EditPropertyInput, UpdateLocationRequest } from '../types';

export const useMyProperties = (page = 1, pageSize = 10) => {
  return useQuery({
    queryKey: ['properties', 'my', page, pageSize],
    queryFn: () => propertiesApi.getMyProperties(page, pageSize)
  });
};

export const useProperty = (id: string) => {
  return useQuery({
    queryKey: ['properties', id],
    queryFn: () => propertiesApi.getProperty(id),
    enabled: !!id
  });
};

export const useCreateProperty = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ payload, files }: { payload: CreatePropertyRequest, files: File[] }) => 
      propertiesApi.createProperty(payload, files),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['properties', 'my'] });
    }
  });
};

export const useUpdateProperty = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ propertyId, data }: { propertyId: string, data: EditPropertyInput }) => 
      propertiesApi.updateProperty(propertyId, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['properties', variables.propertyId] });
    }
  });
};

export const useUpdateLocation = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ propertyId, data }: { propertyId: string, data: UpdateLocationRequest }) => 
      propertiesApi.updateLocation(propertyId, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['properties', variables.propertyId] });
    }
  });
};

export const useUpdateStatus = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ propertyId, status }: { propertyId: string, status: number }) => 
      propertiesApi.updateStatus(propertyId, status),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['properties', variables.propertyId] });
      queryClient.invalidateQueries({ queryKey: ['properties', 'my'] });
    }
  });
};

export const useAddImages = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ propertyId, files, type }: { propertyId: string, files: File[], type: number }) => 
      propertiesApi.addImages(propertyId, files, type),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['properties', variables.propertyId] });
    }
  });
};

export const useRemoveImage = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ propertyId, imageId }: { propertyId: string, imageId: string }) => 
      propertiesApi.removeImage(propertyId, imageId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['properties', variables.propertyId] });
    }
  });
};

export const useReorderImages = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ propertyId, orders }: { propertyId: string, orders: { imageId: string, displayOrder: number }[] }) => 
      propertiesApi.reorderImages(propertyId, orders),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['properties', variables.propertyId] });
    }
  });
};

export const useAvailableAmenities = () => {
  return useQuery({
    queryKey: ['amenities', 'available'],
    queryFn: () => propertiesApi.getAvailableAmenities()
  });
};

export const useAddAmenity = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ propertyId, amenityId }: { propertyId: string, amenityId: string }) => 
      propertiesApi.addAmenity(propertyId, amenityId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['properties', variables.propertyId] });
    }
  });
};

export const useRemoveAmenity = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ propertyId, amenityId }: { propertyId: string, amenityId: string }) => 
      propertiesApi.removeAmenity(propertyId, amenityId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['properties', variables.propertyId] });
    }
  });
};

export const useUpdateAmenityInfo = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ propertyId, amenityId, additionalInfo }: { propertyId: string, amenityId: string, additionalInfo: string }) => 
      propertiesApi.updateAmenityInfo(propertyId, amenityId, additionalInfo),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['properties', variables.propertyId] });
    }
  });
};

export const useBlockDates = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ propertyId, data }: { propertyId: string, data: { startDate: string, endDate: string, note?: string } }) => 
      propertiesApi.blockDates(propertyId, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['properties', variables.propertyId] });
    }
  });
};

export const useRemoveAvailability = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ propertyId, availabilityId }: { propertyId: string, availabilityId: string }) => 
      propertiesApi.removeAvailability(propertyId, availabilityId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['properties', variables.propertyId] });
    }
  });
};
