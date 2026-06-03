import React from 'react';
import type { ChatMessage } from '../types/model';
import { formatMessageTime } from '../utils/date';
import { useAuthStore } from '@/store/authStore';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Loading03Icon } from '@/components/common/Icons';
import { Icon } from '@iconify/react';

interface MessageBubbleProps {
  message: ChatMessage;
  otherParticipantAvatar?: string;
  otherParticipantName?: string;
  isFirstOfChain: boolean;
  isLastOfChain: boolean;
  otherLastReadMessageId?: string;
  isLastMessageOfConversation: boolean;
}

export const MessageBubble = React.memo<MessageBubbleProps>(({ 
  message, 
  otherParticipantAvatar, 
  otherParticipantName,
  isFirstOfChain,
  isLastOfChain,
  otherLastReadMessageId,
  isLastMessageOfConversation
}) => {
  const { userId } = useAuthStore();
  const isOwnMessage = message.senderId?.toLowerCase() === userId?.toLowerCase();

  if (message.messageType === 'System') {
    return (
      <div className="flex w-full flex-col items-center my-3">
        <div className="bg-[#f7f7f7] text-[#6a6a6a] text-[12px] px-4 py-1.5 rounded-full font-medium border border-[#ebebeb] flex items-center gap-2">
          <Icon icon="logos:airbnb-icon" className="w-4 h-4 shrink-0" />
          <span>{message.content}</span>
        </div>
      </div>
    );
  }

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
        {message.messageType === 'Image' ? (
          <div className="relative rounded-[16px] overflow-hidden max-w-[240px] sm:max-w-xs border border-[#ebebeb] shadow-sm bg-gray-100">
            <img 
              src={message.content} 
              alt="Image message" 
              className={`w-full h-auto object-cover block transition-all ${message.id?.startsWith('temp-') ? 'opacity-50 blur-[2px]' : ''}`} 
            />
            {message.id?.startsWith('temp-') && (
              <div className="absolute inset-0 flex items-center justify-center">
                <Loading03Icon className="h-6 w-6 animate-spin text-[#222222]" />
              </div>
            )}
          </div>
        ) : (
          <div 
            className={`px-4 py-3 rounded-[20px] text-[15px] leading-[1.4] transition-all ${
              isOwnMessage 
                ? 'bg-[#25D366] text-white rounded-tr-sm' 
                : 'bg-[#f2f2f2] text-[#222222] rounded-tl-sm'
            }`}
          >
            {message.content}
          </div>
        )}
      </div>

      {/* Thời gian gửi phía dưới bong bóng (chỉ hiện khi là tin nhắn cuối cùng của chain) */}
      {isLastOfChain && (
        <div className={`flex items-center gap-2 mt-1 ${isOwnMessage ? 'justify-end' : 'justify-start'} ${!isOwnMessage ? 'pl-16' : 'pr-3'}`}>
          <span className="text-[11px] text-[#b0b0b0] font-normal">
            {formatMessageTime(new Date(message.sentAt))}
          </span>
        </div>
      )}

      {/* Avatar tròn nhỏ hiển thị trạng thái đã xem (seen status) của đối phương */}
      {((isOwnMessage && otherLastReadMessageId && message.id?.toLowerCase() === otherLastReadMessageId?.toLowerCase()) ||
        (!isOwnMessage && isLastMessageOfConversation)) && (
        <div className="flex justify-end mt-1 pr-1 w-full">
          <Avatar className="h-4 w-4 shrink-0 ring-1 ring-white shadow-xs">
            <AvatarImage src={otherParticipantAvatar || ''} alt={otherParticipantName || 'User'} />
            <AvatarFallback className="bg-[#f2f2f2] text-[#222222] font-semibold text-[8px] flex items-center justify-center">
              {otherParticipantName?.charAt(0) || 'U'}
            </AvatarFallback>
          </Avatar>
        </div>
      )}
    </div>
  );
}, (prevProps, nextProps) => {
  // Kiểm tra các prop thông thường, nếu thay đổi thì re-render
  if (
    prevProps.message.id !== nextProps.message.id ||
    prevProps.message.content !== nextProps.message.content ||
    prevProps.message.sentAt !== nextProps.message.sentAt ||
    prevProps.isFirstOfChain !== nextProps.isFirstOfChain ||
    prevProps.isLastOfChain !== nextProps.isLastOfChain ||
    prevProps.isLastMessageOfConversation !== nextProps.isLastMessageOfConversation ||
    prevProps.otherParticipantAvatar !== nextProps.otherParticipantAvatar ||
    prevProps.otherParticipantName !== nextProps.otherParticipantName
  ) {
    return false;
  }

  // Chỉ render lại nếu tin nhắn này chính là tin nhắn đối phương vừa đọc (hoặc vừa hết được đọc)
  const msgId = prevProps.message.id?.toLowerCase();
  const wasSeen = prevProps.otherLastReadMessageId?.toLowerCase() === msgId;
  const isSeen = nextProps.otherLastReadMessageId?.toLowerCase() === msgId;

  if (wasSeen !== isSeen) {
    return false;
  }

  return true; 
});

MessageBubble.displayName = 'MessageBubble';
