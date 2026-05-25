import { useInfiniteQuery } from '@tanstack/react-query';
import { chatApi } from '../api/chatApi';
import { mapConversationDtoToModel } from '../utils/mapper';
import { useAuthStore } from '../../../store/authStore';

export const useInbox = () => {
  const currentUserId = useAuthStore(state => state.userId);

  return useInfiniteQuery({
    queryKey: ['chat', 'inbox'],
    queryFn: async ({ pageParam }) => {
      const response = await chatApi.getInbox(pageParam as string | undefined);
      return {
        items: response.items.map(dto => mapConversationDtoToModel(dto, currentUserId)),
        nextCursor: response.nextCursor,
      };
    },
    getNextPageParam: (lastPage) => lastPage.nextCursor,
    initialPageParam: undefined as string | undefined,
  });
};
