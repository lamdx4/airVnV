export interface PaginatedCursorResponseDto<T> {
  items: T[];
  nextCursor?: string;
}

export interface ConversationParticipantDto {
  userId: string;
  role: 'Host' | 'Guest' | 'System';
  displayName: string;
  avatarUrl?: string;
  joinedAt: string;
}

export interface ConversationDto {
  id: string;
  propertyId: string;
  propertyTitle: string;
  createdAt: string;
  lastMessageAt?: string;
  isArchived: boolean;
  participants: ConversationParticipantDto[];
  unreadCount: number;
  otherLastReadMessageId?: string;
  latestMessageContent?: string;
  latestMessageId?: string;
}

export interface MessageDto {
  id: string;
  conversationId: string;
  senderId: string;
  content: string;
  sentAt: string;
  isSystemMessage: boolean;
}

export interface CreateConversationRequestDto {
  propertyId: string;
  reservationId?: string;
}

export interface SendMessageRequestDto {
  content: string;
}
