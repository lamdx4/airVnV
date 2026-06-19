import axios from "axios";
import { api } from "../../../lib/api";
import type {
  ConversationDto,
  CreateConversationRequestDto,
  MessageDto,
  PaginatedCursorResponseDto,
  SendMessageRequestDto,
  AttachmentDto,
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
   * Lấy danh sách file/ảnh đính kèm của một Conversation
   */
  getAttachments: async (
    conversationId: string,
    type: 'Image' | 'File',
    before?: string,
  ): Promise<PaginatedCursorResponseDto<AttachmentDto>> => {
    const params = new URLSearchParams({ type });
    if (before) params.append("before", before);

    return api.get<any, PaginatedCursorResponseDto<AttachmentDto>>(
      `/api/conversations/${conversationId}/attachments?${params.toString()}`,
    );
  },

  /**
   * Tạo hội thoại mới
   */
  createConversation: async (
    data: CreateConversationRequestDto,
  ): Promise<{ conversationId: string }> => {
    return api.post<any, { conversationId: string }>("/api/conversations", data);
  },

  /**
   * Gửi tin nhắn
   */
  sendMessage: async (
    conversationId: string,
    content: string,
    messageType: string = 'Text'
  ): Promise<MessageDto> => {
    const payload: SendMessageRequestDto = { content, messageType };
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

  getUserStatus: async (userId: string): Promise<{ isOnline: boolean }> => {
    return api.get<any, { isOnline: boolean }>(`/api/conversations/users/${userId}/status`);
  },

  /**
   * Lấy cấu hình WebRTC (STUN/TURN) từ Backend
   */
  getWebRtcCredentials: async (): Promise<{ iceServers: any[] }> => {
    return api.get<any, { iceServers: any[] }>("/api/chat/webrtc-credentials");
  },

  /**
   * Chủ động lấy token mới (dành riêng cho SignalR)
   */
  refreshSignalRToken: async (
    refreshToken: string,
  ): Promise<{ success: boolean; data?: { accessToken: string; refreshToken: string } }> => {
    const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5000";

    const response = await axios.post(`${API_URL}/api/users/refresh-token`, {
      refreshToken,
    });
    
    return response.data;
  },
};
