import { useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { ChatContainer } from '../features/chat/components/ChatContainer';
import { ChatProvider, useChat } from '../features/chat/context/ChatContext';

const MessagesContent = () => {
  const { conversationId } = useParams();
  const { setActiveConversationId } = useChat();

  useEffect(() => {
    if (conversationId) {
      setActiveConversationId(conversationId);
    }
  }, [conversationId, setActiveConversationId]);

  return <ChatContainer />;
};

export default function Messages() {
  return (
    <ChatProvider>
      <MessagesContent />
    </ChatProvider>
  );
}
