import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminPropertiesApi } from '../api/adminProperties';
import { toPendingPropertyList } from '../utils/mappers';
import { toast } from 'sonner';

export function usePendingProperties(params: {
  page?: number;
  pageSize?: number;
  search?: string;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}) {
  return useQuery({
    queryKey: ['admin', 'properties', 'pending', params],
    queryFn: async () => {
      const response = await adminPropertiesApi.getPendingProperties(params);
      return {
        ...response,
        items: toPendingPropertyList(response),
      };
    },
    staleTime: 30_000,
  });
}

export function useApproveProperty() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (propertyId: string) => adminPropertiesApi.approveProperty(propertyId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin', 'properties'] });
      toast.success('Property approved and published');
    },
    onError: (error: { message?: string; errorCode?: string }) => {
      toast.error(error.message || 'Failed to approve property');
    },
  });
}

export function useRejectProperty() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ propertyId, reason }: { propertyId: string; reason?: string }) =>
      adminPropertiesApi.rejectProperty(propertyId, reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin', 'properties'] });
      toast.success('Property rejected');
    },
    onError: (error: { message?: string; errorCode?: string }) => {
      toast.error(error.message || 'Failed to reject property');
    },
  });
}