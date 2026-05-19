import React from 'react';
import { ChatContainer } from '@/features/chat/components/ChatContainer';
import { ChatProvider } from '@/features/chat/context/ChatContext';
import Header from '@/components/common/Header';

const ChatPage: React.FC = () => {
  return (
    <ChatProvider>
      <div className="flex flex-col h-screen bg-white">
        <Header />
        <main className="flex-1 overflow-hidden">
          <ChatContainer />
        </main>
      </div>
    </ChatProvider>
  );
};

export default ChatPage;
