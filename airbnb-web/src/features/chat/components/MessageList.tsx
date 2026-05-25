import React, { useEffect, useRef } from 'react';
import type { ChatMessage } from '../types/model';
import { useMessages } from '../hooks/useMessages';
import { useChat } from '../context/ChatContext';
import { useInbox } from '../hooks/useInbox';
import { MessageBubble } from './MessageBubble';
import { formatChatDate } from '../utils/date';
import { Loading03Icon } from '@/components/common/Icons';
import { motion, AnimatePresence } from 'framer-motion';

export const MessageList: React.FC = () => {
  const { activeConversationId } = useChat();
  const { data, fetchNextPage, hasNextPage, isFetchingNextPage, isLoading } = useMessages(activeConversationId || '');
  const { data: inboxData } = useInbox();
  const scrollRef = useRef<HTMLDivElement>(null);

  const messages = data?.pages.flatMap(page => page.items).reverse() || [];

  const conversation = inboxData?.pages
    .flatMap(page => page.items)
    .find(c => c.id === activeConversationId);

  const otherParticipantAvatar = conversation?.otherParticipantAvatar;
  const otherParticipantName = conversation?.otherParticipantName;

  useEffect(() => {
    if (scrollRef.current && !isFetchingNextPage) {
      scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
    }
  }, [messages.length, isFetchingNextPage]);

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
      className="flex-1 overflow-y-auto px-4 md:px-12 py-8 space-y-10 bg-[#ffffff] scroll-smooth custom-scrollbar"
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
                    />
                  </motion.div>
                );
              })}
            </AnimatePresence>
          </div>
        </div>
      ))}
      
      <div className="h-4" />
    </div>
  );
};
