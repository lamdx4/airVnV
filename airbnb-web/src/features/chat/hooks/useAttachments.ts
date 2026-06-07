import { useInfiniteQuery } from "@tanstack/react-query";
import { chatApi } from "../api/chatApi";
import { mapAttachmentDtoToModel } from "../utils/mapper";

export const useAttachments = (
  conversationId: string | null,
  type: "Image" | "File",
) => {
  return useInfiniteQuery({
    queryKey: ["chat", "attachments", conversationId, type],
    queryFn: async ({ pageParam }) => {
      if (!conversationId) throw new Error("No conversation selected");
      const response = await chatApi.getAttachments(
        conversationId,
        type,
        pageParam as string | undefined,
      );

      return {
        items: response.items.map(mapAttachmentDtoToModel),
        nextCursor: response.nextCursor,
      };
    },
    getNextPageParam: (lastPage) => lastPage.nextCursor,
    initialPageParam: undefined as string | undefined,
    enabled: !!conversationId, // Chỉ chạy khi có ID
    staleTime: 5 * 60 * 1000, // Cache 5 phút
  });
};
