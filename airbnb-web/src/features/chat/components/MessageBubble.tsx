import React from 'react';
import type { ChatMessage } from '../types/model';

interface MessageBubbleProps {
  message: ChatMessage;
  isCurrentUser: boolean;
}

export const MessageBubble: React.FC<MessageBubbleProps> = ({ message, isCurrentUser }) => {
  if (message.isSystemMessage) {
    return (
      <div className="flex justify-center my-4">
        <div className="bg-gray-100 text-gray-600 text-xs px-4 py-2 rounded-full shadow-sm text-center max-w-sm">
          {message.content}
        </div>
      </div>
    );
  }

  // Chờ gửi (optimistic)
  const isPending = message.id.startsWith('temp-');

  return (
    <div className={`flex w-full my-2 ${isCurrentUser ? 'justify-end' : 'justify-start'}`}>
      <div className="flex flex-col max-w-[70%]">
        <div 
          className={`px-4 py-2 rounded-2xl ${
            isCurrentUser 
              ? 'bg-blue-600 text-white rounded-br-sm' 
              : 'bg-gray-100 text-gray-900 rounded-bl-sm'
          } ${isPending ? 'opacity-70' : 'opacity-100'} transition-opacity shadow-sm`}
        >
          <p className="text-sm whitespace-pre-wrap break-words">{message.content}</p>
        </div>
        <div className={`text-[10px] text-gray-400 mt-1 flex ${isCurrentUser ? 'justify-end' : 'justify-start'}`}>
          {isPending ? 'Đang gửi...' : new Date(message.sentAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
        </div>
      </div>
    </div>
  );
};
