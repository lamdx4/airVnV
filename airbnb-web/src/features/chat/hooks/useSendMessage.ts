import { useMutation, useQueryClient } from '@tanstack/react-query';
import { chatApi } from '../api/chatApi';
import type { ChatMessage } from '../types/model';
import { useAuthStore } from '../../../store/authStore';

export const useSendMessage = (conversationId: string | null) => {
  const queryClient = useQueryClient();
  const currentUserId = useAuthStore(state => state.userId) || '';

  return useMutation({
    mutationFn: async ({ content, messageType = 'Text' }: { content: string, messageType?: string }) => {
      if (!conversationId) throw new Error('No conversation selected');
      return await chatApi.sendMessage(conversationId, content, messageType);
    },
    onMutate: async ({ content, messageType = 'Text' }) => {
      if (!conversationId) return;

      // Cancel any outgoing refetches
      await queryClient.cancelQueries({ queryKey: ['chat', 'messages', conversationId] });

      // Snapshot the previous value
      const previousMessages = queryClient.getQueryData(['chat', 'messages', conversationId]);

      // Optimistically update to the new value
      queryClient.setQueryData(['chat', 'messages', conversationId], (old: any) => {
        if (!old) return old;

        const optimisticMsg: ChatMessage = {
          id: `temp-${Date.now()}`,
          conversationId,
          content,
          senderId: currentUserId,
          sentAt: new Date(),
          messageType,
        };

        const newPages = [...old.pages];
        // Thêm tin nhắn vào đầu trang 0 (vì ta lấy tin mới nhất trước)
        if (newPages.length > 0) {
          newPages[0] = {
            ...newPages[0],
            items: [optimisticMsg, ...newPages[0].items],
          };
        }

        return { ...old, pages: newPages };
      });

      return { previousMessages };
    },
    onError: (_err, _newContent, context) => {
      // Nếu lỗi, rollback lại data cũ
      if (context?.previousMessages && conversationId) {
        queryClient.setQueryData(['chat', 'messages', conversationId], context.previousMessages);
      }
    },
    onSettled: () => {
      // Bỏ invalidateQueries ở đây để tránh refetch thừa (SignalR sẽ đảm nhiệm việc sync cuối cùng)
    },
  });
};
