import { useMutation, useQueryClient } from "@tanstack/react-query";
import { chatApi } from "../api/chatApi";
import type { CreateConversationRequestDto } from "../types/dto";

export const useInitConversation = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (data: CreateConversationRequestDto) => {
      // logic: nếu đã có conversation thì trả về ID cũ, chưa có thì tạo mới
      const response = await chatApi.createConversation(data);
      return response.conversationId;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["chat", "inbox"] });
    },
  });
};
