import type { ConversationDto, MessageDto } from '../types/dto';
import type { Conversation, ChatMessage } from '../types/model';

/**
 * Ánh xạ dữ liệu từ API thành Model dùng trong UI cho Conversation
 */
export const mapConversationDtoToModel = (dto: ConversationDto, currentUserId: string | null): Conversation => {
  // Tìm người tham gia không phải là mình
  const otherParticipant = dto.participants.find(p => p.userId !== currentUserId);

  return {
    id: dto.id,
    propertyId: dto.propertyId,
    propertyTitle: dto.propertyTitle,
    createdAt: new Date(dto.createdAt),
    lastMessageAt: dto.lastMessageAt ? new Date(dto.lastMessageAt) : undefined,
    isArchived: dto.isArchived,
    participants: dto.participants.map(p => ({
      userId: p.userId,
      role: p.role,
      displayName: p.displayName,
      avatarUrl: p.avatarUrl,
    })),
    unreadCount: dto.unreadCount,
    otherParticipantName: otherParticipant?.displayName || 'Unknown User',
    otherParticipantAvatar: otherParticipant?.avatarUrl,
  };
};

/**
 * Ánh xạ dữ liệu từ API thành Model dùng trong UI cho Message
 */
export const mapMessageDtoToModel = (dto: MessageDto): ChatMessage => {
  return {
    id: dto.id,
    conversationId: dto.conversationId,
    senderId: dto.senderId,
    content: dto.content,
    sentAt: new Date(dto.sentAt),
    isSystemMessage: dto.isSystemMessage,
  };
};
