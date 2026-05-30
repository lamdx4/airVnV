import React, { useEffect, useLayoutEffect, useRef } from 'react';
import type { ChatMessage, Conversation } from '../types/model';
import { useMessages } from '../hooks/useMessages';
import { useInbox } from '../hooks/useInbox';
import { MessageBubble } from './MessageBubble';
import { formatChatDate } from '../utils/date';
import { Loading03Icon } from '@/components/common/Icons';
import { motion, AnimatePresence } from 'framer-motion';
import { useQueryClient } from '@tanstack/react-query';
import { chatApi } from '../api/chatApi';
import { Icon } from '@iconify/react';
import type * as signalR from '@microsoft/signalr';
import { useTypingSubscriber } from '../hooks/useTypingStatus';

interface MessageListProps {
  connection: signalR.HubConnection | null;
  activeConversationId: string;
}

export const MessageList: React.FC<MessageListProps> = ({ connection, activeConversationId }) => {
  const isTyping = useTypingSubscriber(connection, activeConversationId);
  const { data, fetchNextPage, hasNextPage, isFetchingNextPage, isLoading } = useMessages(activeConversationId || '', connection);
  const { data: inboxData } = useInbox();
  const scrollRef = useRef<HTMLDivElement>(null);
  const queryClient = useQueryClient();

  const messages = [...(data?.pages.flatMap(page => page.items) || [])]
    .sort((a, b) => new Date(b.sentAt).getTime() - new Date(a.sentAt).getTime()); // Sắp xếp giảm dần (mới nhất ở index 0)

  const lastMessageId = messages.length > 0 ? messages[0].id : null;

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

  const handleScroll = (e: React.UIEvent<HTMLDivElement>) => {
    // Trong flex-col-reverse, vị trí cuộn lên trên cùng có scrollTop âm (Chrome) hoặc dương tùy trình duyệt.
    // Cách an toàn nhất để check là giá trị tuyệt đối của scrollTop cộng clientHeight >= scrollHeight
    const target = e.currentTarget;
    if (Math.abs(target.scrollTop) + target.clientHeight >= target.scrollHeight - 1) {
      if (hasNextPage && !isFetchingNextPage) {
        fetchNextPage();
      }
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
      className="flex-1 overflow-y-auto px-4 md:px-12 py-8 flex flex-col-reverse bg-[#ffffff] custom-scrollbar"
      onScroll={handleScroll}
    >
      <div className="h-4 shrink-0" />

      {/* Typing indicator nằm ở visual bottom (DOM đầu tiên trong flex-col-reverse) */}
      {isTyping && (
        <div className="flex items-start gap-3 mb-4">
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

      {groupedMessages.map((group) => (
        <div key={group.date} className="flex flex-col-reverse mb-8">
          <div className="flex flex-col-reverse gap-1.5">
            <AnimatePresence initial={false}>
              {group.items.map((msg) => {
                const globalIndex = messages.findIndex(m => m.id === msg.id);
                // Vì mảng giảm dần (mới nhất ở 0)
                // First of chain (tin cũ nhất của block) là tin có tin cũ hơn (index + 1) khác người gửi
                const isFirstOfChain = globalIndex === messages.length - 1 || messages[globalIndex + 1].senderId !== msg.senderId;
                // Last of chain (tin mới nhất của block) là tin có tin mới hơn (index - 1) khác người gửi
                const isLastOfChain = globalIndex === 0 || messages[globalIndex - 1].senderId !== msg.senderId;
                const isLastMessageOfConversation = globalIndex === 0;

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
          
          <div className="flex justify-center sticky top-0 z-10 py-2 mb-2">
            <span className="px-4 py-1 bg-white border border-[#ebebeb] rounded-full text-[12px] font-medium text-[#6a6a6a] shadow-xs">
              {group.date}
            </span>
          </div>
        </div>
      ))}
      
      {/* Nút Load More nằm ở visual top (DOM cuối cùng) */}
      {hasNextPage && (
        <div className="flex justify-center pt-4 pb-8">
          <button 
            onClick={() => fetchNextPage()}
            disabled={isFetchingNextPage}
            className="text-[12px] font-semibold text-[#6a6a6a] hover:text-[#222222] transition-colors underline"
          >
            {isFetchingNextPage ? 'Loading...' : 'Load older messages'}
          </button>
        </div>
      )}
    </div>
  );
};
