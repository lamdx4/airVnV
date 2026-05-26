import { useInfiniteQuery } from '@tanstack/react-query';
import { chatApi } from '../api/chatApi';
import { mapMessageDtoToModel } from '../utils/mapper';

export const useMessages = (conversationId: string | null) => {
  return useInfiniteQuery({
    queryKey: ['chat', 'messages', conversationId],
    queryFn: async ({ pageParam }) => {
      if (!conversationId) throw new Error('No conversation selected');
      const response = await chatApi.getMessages(conversationId, pageParam as string | undefined);
      
      return {
        items: response.items.map(mapMessageDtoToModel),
        nextCursor: response.nextCursor,
      };
    },
    getNextPageParam: (lastPage) => lastPage.nextCursor,
    initialPageParam: undefined as string | undefined,
    enabled: !!conversationId, // Chỉ chạy khi có ID
  });
};
