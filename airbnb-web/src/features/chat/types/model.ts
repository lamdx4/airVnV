export interface ConversationParticipant {
  userId: string;
  role: 'Host' | 'Guest' | 'System';
  displayName: string;
  avatarUrl?: string;
}

export interface Conversation {
  id: string;
  propertyId: string;
  propertyTitle: string;
  createdAt: Date;
  lastMessageAt?: Date;
  isArchived: boolean;
  participants: ConversationParticipant[];
  unreadCount: number;
  
  // Computed fields for UI convenience
  otherParticipantName: string;
  otherParticipantAvatar?: string;
  otherLastReadMessageId?: string;
}

export interface ChatMessage {
  id: string;
  conversationId: string;
  senderId: string;
  content: string;
  sentAt: Date;
  isSystemMessage: boolean;
}
