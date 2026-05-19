import React from 'react';
import type { ChatMessage } from '../types/model';
import { formatMessageTime } from '../utils/date';
import { useAuthStore } from '@/store/authStore';

interface MessageBubbleProps {
  message: ChatMessage;
}

export const MessageBubble: React.FC<MessageBubbleProps> = ({ message }) => {
  const { userId } = useAuthStore();
  const isOwnMessage = message.senderId === userId;

  return (
    <div className={`flex w-full ${isOwnMessage ? 'justify-end' : 'justify-start'}`}>
      <div className={`max-w-[75%] space-y-1.5`}>
        <div 
          className={`px-4 py-3 rounded-[22px] text-[15px] leading-[1.4] transition-all ${
            isOwnMessage 
              ? 'bg-[#222222] text-white' 
              : 'bg-[#f2f2f2] text-[#222222]'
          }`}
        >
          {message.content}
        </div>
        
        <div className={`flex items-center px-3 ${isOwnMessage ? 'justify-end' : 'justify-start'}`}>
          <span className="text-[12px] text-[#b0b0b0] font-normal">
            {formatMessageTime(new Date(message.sentAt))}
          </span>
        </div>
      </div>
    </div>
  );
};
