import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { propertiesApi } from '../api/properties';
import { PropertyDTO } from '../types';

import { PropertyDTO, PaginationParams } from '../types';

export const useMyProperties = (params: PaginationParams) => {
  return useQuery({
    queryKey: ['properties', 'my', params],
    queryFn: () => propertiesApi.getMyProperties(params),
  });
};

export const useProperty = (id: string) => {
  return useQuery({
    queryKey: ['properties', id],
    queryFn: () => propertiesApi.getProperty(id),
    enabled: !!id,
  });
};

export const useCreateProperty = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: any) => propertiesApi.createProperty(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['properties', 'my'] });
    },
  });
};

export const useUpdateProperty = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string, data: any }) => propertiesApi.updateProperty(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['properties', 'my'] });
      queryClient.invalidateQueries({ queryKey: ['properties', variables.id] });
    },
  });
};

export const useSubmitProperty = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => propertiesApi.submitProperty(id),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ['properties', 'my'] });
      queryClient.invalidateQueries({ queryKey: ['properties', id] });
    },
  });
};

export const useDeleteProperty = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => propertiesApi.deleteProperty(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['properties', 'my'] });
    },
  });
};

export const useArchiveProperty = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => propertiesApi.archiveProperty(id),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ['properties', 'my'] });
      queryClient.invalidateQueries({ queryKey: ['properties', id] });
    },
  });
};

export const useApproveProperty = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => propertiesApi.approveProperty(id),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ['properties', 'my'] });
      queryClient.invalidateQueries({ queryKey: ['properties', id] });
    },
  });
};

export const useSuspendProperty = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, reason }: { id: string, reason: string }) => propertiesApi.suspendProperty(id, reason),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['properties', 'my'] });
      queryClient.invalidateQueries({ queryKey: ['properties', variables.id] });
    },
  });
};

export const useReinstateProperty = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => propertiesApi.reinstateProperty(id),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ['properties', 'my'] });
      queryClient.invalidateQueries({ queryKey: ['properties', id] });
    },
  });
};
