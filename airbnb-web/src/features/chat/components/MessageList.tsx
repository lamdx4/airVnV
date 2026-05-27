import React, { useEffect, useLayoutEffect, useRef } from 'react';
import type { ChatMessage, Conversation } from '../types/model';
import { useMessages } from '../hooks/useMessages';
import { useChat } from '../context/ChatContext';
import { useInbox } from '../hooks/useInbox';
import { MessageBubble } from './MessageBubble';
import { formatChatDate } from '../utils/date';
import { Loading03Icon } from '@/components/common/Icons';
import { motion, AnimatePresence } from 'framer-motion';
import { useQueryClient } from '@tanstack/react-query';
import { chatApi } from '../api/chatApi';
import { Icon } from '@iconify/react';

interface MessageListProps {
  isTyping?: boolean;
}

export const MessageList: React.FC<MessageListProps> = ({ isTyping }) => {
  const { activeConversationId } = useChat();
  const { data, fetchNextPage, hasNextPage, isFetchingNextPage, isLoading } = useMessages(activeConversationId || '');
  const { data: inboxData } = useInbox();
  const scrollRef = useRef<HTMLDivElement>(null);
  const queryClient = useQueryClient();

  const messages = [...(data?.pages.flatMap(page => page.items) || [])]
    .sort((a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime());

  const lastMessageId = messages.length > 0 ? messages[messages.length - 1].id : null;

  const conversation = inboxData?.pages
    .flatMap(page => page.items)
    .find(c => c.id?.toLowerCase() === activeConversationId?.toLowerCase());

  const otherParticipantAvatar = conversation?.otherParticipantAvatar;
  const otherParticipantName = conversation?.otherParticipantName;

  // Tự động gọi API đánh dấu đã đọc (Mark as read) khi mở chat hoặc có tin nhắn mới
  useEffect(() => {
    if (!activeConversationId || !lastMessageId || lastMessageId.startsWith('temp-')) return;

    chatApi.markAsRead(activeConversationId, lastMessageId)
      .then(() => {
        // Cập nhật unreadCount của chính mình về 0 trong cache
        queryClient.setQueryData(['chat', 'inbox'], (old: any) => {
          if (!old) return old;
          const newPages = old.pages.map((page: any) => ({
            ...page,
            items: page.items.map((c: Conversation) => {
              if (c.id?.toLowerCase() === activeConversationId?.toLowerCase()) {
                return {
                  ...c,
                  unreadCount: 0
                };
              }
              return c;
            })
          }));
          return { ...old, pages: newPages };
        });
      })
      .catch(console.error);
  }, [activeConversationId, lastMessageId, queryClient]);

  const prevScrollHeightRef = useRef<number>(0);
  const prevLastMessageIdRef = useRef<string | null>(null);

  useLayoutEffect(() => {
    if (!scrollRef.current) return;
    const el = scrollRef.current;

    // Nếu ID tin nhắn cuối thay đổi (có tin nhắn mới hoặc đổi phòng) -> Cuộn xuống đáy
    if (prevLastMessageIdRef.current !== lastMessageId) {
      // Nếu có tin nhắn mới trong lúc đang chat thì cuộn mượt, nếu là lần đầu mở phòng thì cuộn tức thì
      if (prevLastMessageIdRef.current) {
        el.scrollTo({ top: el.scrollHeight, behavior: 'smooth' });
      } else {
        el.scrollTop = el.scrollHeight;
      }
    } 
    // Nếu load thêm tin nhắn cũ (lastMessageId không đổi nhưng scrollHeight tăng) -> Bù trừ vị trí cuộn
    else if (prevScrollHeightRef.current > 0) {
      const heightDiff = el.scrollHeight - prevScrollHeightRef.current;
      if (heightDiff > 0) {
        el.scrollTop += heightDiff;
      }
    }

    prevScrollHeightRef.current = el.scrollHeight;
    prevLastMessageIdRef.current = lastMessageId;
  }, [messages.length, lastMessageId, activeConversationId]);

  const handleScroll = (e: React.UIEvent<HTMLDivElement>) => {
    if (e.currentTarget.scrollTop === 0 && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  };

  if (!activeConversationId) return null;

  if (isLoading) {
    return (
      <div className="flex-1 flex flex-col items-center justify-center gap-4 bg-white">
        <Loading03Icon className="h-8 w-8 animate-spin text-pink-500" />
        <span className="text-xs font-semibold text-slate-400 uppercase tracking-widest">Loading history...</span>
      </div>
    );
  }

  // Group messages by date
  const groupedMessages: { date: string, items: ChatMessage[] }[] = [];
  messages.forEach(msg => {
    const dateStr = formatChatDate(new Date(msg.sentAt));
    const lastGroup = groupedMessages[groupedMessages.length - 1];
    if (lastGroup && lastGroup.date === dateStr) {
      lastGroup.items.push(msg);
    } else {
      groupedMessages.push({ date: dateStr, items: [msg] });
    }
  });

  return (
    <div 
      ref={scrollRef}
      className="flex-1 overflow-y-auto px-4 md:px-12 py-8 space-y-10 bg-[#ffffff] custom-scrollbar"
      onScroll={handleScroll}
    >
      {hasNextPage && (
        <div className="flex justify-center pb-4">
          <button 
            onClick={() => fetchNextPage()}
            disabled={isFetchingNextPage}
            className="text-[12px] font-semibold text-[#6a6a6a] hover:text-[#222222] transition-colors underline"
          >
            {isFetchingNextPage ? 'Loading...' : 'Load older messages'}
          </button>
        </div>
      )}

      {groupedMessages.map((group) => (
        <div key={group.date} className="space-y-8">
          <div className="flex justify-center sticky top-0 z-10 py-2">
            <span className="px-4 py-1 bg-white border border-[#ebebeb] rounded-full text-[12px] font-medium text-[#6a6a6a] shadow-xs">
              {group.date}
            </span>
          </div>

          <div className="space-y-1.5">
            <AnimatePresence initial={false}>
              {group.items.map((msg) => {
                const globalIndex = messages.findIndex(m => m.id === msg.id);
                const isFirstOfChain = globalIndex === 0 || messages[globalIndex - 1].senderId !== msg.senderId;
                const isLastOfChain = globalIndex === messages.length - 1 || messages[globalIndex + 1].senderId !== msg.senderId;
                const isLastMessageOfConversation = globalIndex === messages.length - 1;

                return (
                  <motion.div
                    key={msg.id}
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ duration: 0.18 }}
                  >
                    <MessageBubble 
                      message={msg} 
                      otherParticipantAvatar={otherParticipantAvatar}
                      otherParticipantName={otherParticipantName}
                      isFirstOfChain={isFirstOfChain}
                      isLastOfChain={isLastOfChain}
                      otherLastReadMessageId={conversation?.otherLastReadMessageId}
                      isLastMessageOfConversation={isLastMessageOfConversation}
                    />
                  </motion.div>
                );
              })}
            </AnimatePresence>
          </div>
        </div>
      ))}
      
      {isTyping && (
        <div className="flex items-start gap-3 mt-4">
          <div className="h-8 w-8 rounded-full overflow-hidden shrink-0 border border-[#ebebeb]">
            {otherParticipantAvatar ? (
              <img src={otherParticipantAvatar} alt="avatar" className="h-full w-full object-cover" />
            ) : (
              <div className="h-full w-full bg-[#f7f7f7] flex items-center justify-center">
                <Icon icon="hugeicons:user-01" className="text-[#b0b0b0]" />
              </div>
            )}
          </div>
          <div className="bg-[#f1f1f1] rounded-2xl rounded-tl-sm px-4 py-2 text-[#222222] self-start inline-flex items-center gap-1 h-10 w-16 justify-center">
            <span className="h-2 w-2 bg-[#b0b0b0] rounded-full animate-bounce [animation-delay:-0.3s]" />
            <span className="h-2 w-2 bg-[#b0b0b0] rounded-full animate-bounce [animation-delay:-0.15s]" />
            <span className="h-2 w-2 bg-[#b0b0b0] rounded-full animate-bounce" />
          </div>
        </div>
      )}

      <div className="h-4" />
    </div>
  );
};
