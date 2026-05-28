import { useEffect } from "react";
import { useInfiniteQuery, useQueryClient } from "@tanstack/react-query";
import { chatApi } from "../api/chatApi";
import { mapMessageDtoToModel } from "../utils/mapper";
import type { ChatMessage } from "../types/model";
import type * as signalR from "@microsoft/signalr";

export const useMessages = (
  conversationId: string | null,
  connection?: signalR.HubConnection | null
) => {
  const queryClient = useQueryClient();

  useEffect(() => {
    if (!connection) return;

    const handleReceiveMessage = (messageDto: any) => {
      const newMsg = mapMessageDtoToModel(messageDto);
      
      queryClient.setQueryData(['chat', 'messages', newMsg.conversationId], (old: any) => {
        if (!old) return old;
        const newPages = [...old.pages];
        if (newPages.length > 0) {
          const items = [...newPages[0].items];
          
          const tempIdx = items.findIndex((m: ChatMessage) => m.id.startsWith('temp-') && m.content === newMsg.content);
          
          if (tempIdx !== -1) {
            items[tempIdx] = newMsg;
            newPages[0] = { ...newPages[0], items };
          } else {
            const exists = items.some((m: ChatMessage) => m.id === newMsg.id);
            if (!exists) {
              newPages[0] = { ...newPages[0], items: [newMsg, ...items] };
            }
          }
        }
        return { ...old, pages: newPages };
      });
    };

    connection.on('ReceiveMessage', handleReceiveMessage);

    return () => {
      connection.off('ReceiveMessage', handleReceiveMessage);
    };
  }, [connection, queryClient]);

  return useInfiniteQuery({
    queryKey: ["chat", "messages", conversationId],
    queryFn: async ({ pageParam }) => {
      if (!conversationId) throw new Error("No conversation selected");
      const response = await chatApi.getMessages(
        conversationId,
        pageParam as string | undefined,
      );

      return {
        items: response.items.map(mapMessageDtoToModel),
        nextCursor: response.nextCursor,
      };
    },
    getNextPageParam: (lastPage) => lastPage.nextCursor,
    initialPageParam: undefined as string | undefined,
    enabled: !!conversationId, // Chỉ chạy khi có ID
    staleTime: Infinity,
  });
};
