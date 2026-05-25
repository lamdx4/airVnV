import React from 'react';
import type { ChatMessage } from '../types/model';
import { formatMessageTime } from '../utils/date';
import { useAuthStore } from '@/store/authStore';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';

interface MessageBubbleProps {
  message: ChatMessage;
  otherParticipantAvatar?: string;
  otherParticipantName?: string;
  isFirstOfChain: boolean;
  isLastOfChain: boolean;
}

export const MessageBubble: React.FC<MessageBubbleProps> = ({ 
  message, 
  otherParticipantAvatar, 
  otherParticipantName,
  isFirstOfChain,
  isLastOfChain
}) => {
  const { userId } = useAuthStore();
  const isOwnMessage = message.senderId === userId;

  return (
    <div className={`flex w-full flex-col ${isOwnMessage ? 'items-end' : 'items-start'}`}>
      <div className={`flex items-start gap-3 max-w-[75%] ${isOwnMessage ? 'flex-row-reverse' : 'flex-row'}`}>
        {!isOwnMessage && (
          isFirstOfChain ? (
            <Avatar className="h-9 w-9 shrink-0 ring-1 ring-black/5 relative -top-2">
              <AvatarImage src={otherParticipantAvatar || ''} alt={otherParticipantName || 'User'} />
              <AvatarFallback className="bg-[#f2f2f2] text-[#222222] font-semibold text-sm">
                {otherParticipantName?.charAt(0) || 'U'}
              </AvatarFallback>
            </Avatar>
          ) : (
            /* Spacer để căn thẳng hàng các tin nhắn tiếp theo trong chain */
            <div className="w-9 shrink-0" />
          )
        )}
        
        {/* Bong bóng tin nhắn */}
        <div 
          className={`px-4 py-3 rounded-[20px] text-[15px] leading-[1.4] transition-all ${
            isOwnMessage 
              ? 'bg-[#3b82f6] text-white rounded-tr-sm' 
              : 'bg-[#f2f2f2] text-[#222222] rounded-tl-sm'
          }`}
        >
          {message.content}
        </div>
      </div>

      {/* Thời gian gửi phía dưới bong bóng (chỉ hiện khi là tin nhắn cuối cùng của chain) */}
      {isLastOfChain && (
        <div className={`flex items-center gap-2 mt-1 ${isOwnMessage ? 'justify-end' : 'justify-start'} ${!isOwnMessage ? 'pl-16' : 'pr-3'}`}>
          <span className="text-[11px] text-[#b0b0b0] font-normal">
            {formatMessageTime(new Date(message.sentAt))}
          </span>
        </div>
      )}
    </div>
  );
};
