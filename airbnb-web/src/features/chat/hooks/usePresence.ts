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
    staleTime: 60 * 1000, // 60 giây
    refetchInterval: 60 * 1000, // Tự động polling để check trạng thái phòng khi bạn bè tắt tab (Redis hết hạn)
  });
};
