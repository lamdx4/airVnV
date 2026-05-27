import { api } from "../../../lib/api";
import type {
  ConversationDto,
  CreateConversationRequestDto,
  MessageDto,
  PaginatedCursorResponseDto,
  SendMessageRequestDto,
} from "../types/dto";

export const chatApi = {
  /**
   * Lấy danh sách Inbox (Conversations)
   */
  getInbox: async (
    before?: string,
  ): Promise<PaginatedCursorResponseDto<ConversationDto>> => {
    const params = new URLSearchParams();
    if (before) params.append("before", before);

    // axios interceptor đã tự extract response.data.data
    return api.get<any, PaginatedCursorResponseDto<ConversationDto>>(
      `/api/conversations?${params.toString()}`,
    );
  },

  /**
   * Lấy lịch sử tin nhắn của một Conversation
   */
  getMessages: async (
    conversationId: string,
    before?: string,
  ): Promise<PaginatedCursorResponseDto<MessageDto>> => {
    const params = new URLSearchParams();
    if (before) params.append("before", before);

    return api.get<any, PaginatedCursorResponseDto<MessageDto>>(
      `/api/conversations/${conversationId}/messages?${params.toString()}`,
    );
  },

  /**
   * Tạo hội thoại mới
   */
  createConversation: async (
    data: CreateConversationRequestDto,
  ): Promise<ConversationDto> => {
    return api.post<any, ConversationDto>("/api/conversations", data);
  },

  /**
   * Gửi tin nhắn
   */
  sendMessage: async (
    conversationId: string,
    content: string,
  ): Promise<MessageDto> => {
    const payload: SendMessageRequestDto = { content };
    return api.post<any, MessageDto>(
      `/api/conversations/${conversationId}/messages`,
      payload,
    );
  },

  /**
   * Đánh dấu đã đọc
   */
  markAsRead: async (
    conversationId: string,
    lastReadMessageId: string,
  ): Promise<boolean> => {
    return api.post<any, boolean>(`/api/conversations/${conversationId}/read`, {
      lastReadMessageId,
    });
  },

  /**
   * Lưu trữ (Archive) hội thoại
   */
  archiveConversation: async (conversationId: string): Promise<boolean> => {
    return api.patch<any, boolean>(
      `/api/conversations/${conversationId}/archive`,
    );
  },

  /**
   * Lấy trạng thái online của user
   */
  getUserStatus: async (userId: string): Promise<{ isOnline: boolean }> => {
    return api.get<any, { isOnline: boolean }>(`/api/conversations/users/${userId}/status`);
  },
};
