import { useInfiniteQuery } from '@tanstack/react-query';
import { chatApi } from '../api/chatApi';
import { mapConversationDtoToModel } from '../utils/mapper';

export const useInbox = () => {
  const currentUserId = localStorage.getItem('airbnb_user_id');

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
