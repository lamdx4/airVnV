import React from 'react';
import { ConversationList } from './ConversationList';
import { ChatHeader } from './ChatHeader';
import { MessageList } from './MessageList';
import { MessageInput } from './MessageInput';
import { useChat } from '../context/ChatContext';
import { useChatHub } from '../hooks/useChatHub';
import { motion, AnimatePresence } from 'framer-motion';
import { Icon } from '@iconify/react';

export const ChatContainer: React.FC = () => {
  const { activeConversationId, isSidebarOpen } = useChat();
  
  // Initialize SignalR
  useChatHub(activeConversationId);

  return (
    <div className="h-[100dvh] bg-[#f7f7f7] p-0 md:p-3">
      <div className="h-full md:rounded-[28px] border-x md:border border-[#ebebeb] bg-white overflow-hidden flex shadow-sm">
        {/* Sidebar - Desktop always visible, Mobile toggleable */}
        <div className={`
          ${isSidebarOpen ? 'flex' : 'hidden'} 
          md:flex shrink-0 h-full w-full md:w-[380px]
        `}>
          <ConversationList />
        </div>

        {/* Main Chat Area */}
        <div className={`
          flex-1 flex flex-col h-full bg-white relative
          ${!activeConversationId ? 'hidden md:flex' : 'flex'}
        `}>
          <AnimatePresence mode="wait">
            {activeConversationId ? (
              <motion.div 
                key={activeConversationId}
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                exit={{ opacity: 0 }}
                transition={{ duration: 0.18 }}
                className="flex-1 flex flex-col h-full"
              >
                <ChatHeader />
                <MessageList />
                <MessageInput conversationId={activeConversationId} />
              </motion.div>
            ) : (
              <motion.div 
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                className="flex-1 flex flex-col items-center justify-center p-8 text-center space-y-4 bg-[#fcfcfc]"
              >
                <div className="p-8 bg-white rounded-full shadow-xs border border-[#ebebeb]">
                  <Icon icon="hugeicons:message-02" className="text-6xl text-[#b0b0b0]" />
                </div>
                <div className="space-y-2">
                  <h2 className="text-[18px] font-semibold text-[#222222] tracking-tight">Select a message</h2>
                  <p className="text-[14px] text-[#6a6a6a] max-w-xs leading-relaxed">
                    Choose a conversation from the list to start messaging with hosts or guests.
                  </p>
                </div>
              </motion.div>
            )}
          </AnimatePresence>
        </div>
      </div>
    </div>
  );
};
