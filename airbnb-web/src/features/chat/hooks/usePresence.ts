import { useQuery } from '@tanstack/react-query';
import { chatApi } from '../api/chatApi';

export const usePresence = (userId?: string | null) => {
  return useQuery({
    queryKey: ['presence', userId?.toLowerCase()],
    queryFn: async () => {
      if (!userId) return { isOnline: false };
      const response = await chatApi.getUserStatus(userId);
      return response;
    },
    enabled: !!userId,
    staleTime: Infinity, // SignalR updates will manually mutate cache
  });
};
