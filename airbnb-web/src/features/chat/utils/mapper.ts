import type { Conversation, ChatMessage } from '../types/model';

/**
 * Ánh xạ dữ liệu từ API thành Model dùng trong UI cho Conversation
 */
export const mapConversationDtoToModel = (dto: any, currentUserId: string | null): Conversation => {
  // Hỗ trợ cả 2 cấu trúc: dẹt (flat) từ thực tế API và cấu trúc lồng nhau (nested) cũ
  const id = dto.id || dto.conversationId;
  const otherParticipantName = dto.otherParticipantName || 
    (dto.participants?.find((p: any) => p.userId !== currentUserId)?.displayName) || 'Unknown User';
  const otherParticipantAvatar = dto.otherParticipantAvatar || 
    (dto.participants?.find((p: any) => p.userId !== currentUserId)?.avatarUrl);
  const otherParticipantId = dto.otherParticipantId || 
    (dto.participants?.find((p: any) => p.userId !== currentUserId)?.userId);

  return {
    id: id,
    propertyId: dto.propertyId || '',
    propertyTitle: dto.propertyTitle,
    createdAt: dto.createdAt ? new Date(dto.createdAt) : new Date(),
    lastMessageAt: dto.lastMessageAt ? new Date(dto.lastMessageAt) : undefined,
    isArchived: dto.isArchived || false,
    participants: dto.participants ? dto.participants.map((p: any) => ({
      userId: p.userId,
      role: p.role,
      displayName: p.displayName,
      avatarUrl: p.avatarUrl,
    })) : [],
    unreadCount: dto.unreadCount || 0,
    otherParticipantName: otherParticipantName,
    otherParticipantAvatar: otherParticipantAvatar,
    otherLastReadMessageId: dto.otherLastReadMessageId || dto.OtherLastReadMessageId,
    latestMessageContent: dto.latestMessageContent || dto.LatestMessageContent,
    latestMessageId: dto.latestMessageId || dto.LatestMessageId,
    otherParticipantId: otherParticipantId,
  };
};

/**
 * Ánh xạ dữ liệu từ API thành Model dùng trong UI cho Message
 */
export const mapMessageDtoToModel = (dto: any): ChatMessage => {
  const sentAt = dto.sentAt || dto.createdAt;

  return {
    id: dto.id,
    conversationId: dto.conversationId || '',
    senderId: dto.senderId,
    content: dto.content,
    sentAt: sentAt ? new Date(sentAt) : new Date(),
    messageType: typeof dto.messageType === 'string' ? dto.messageType : (dto.messageType === 1 || dto.isSystemMessage ? 'System' : (dto.messageType === 2 ? 'Image' : 'Text')),
  };
};
