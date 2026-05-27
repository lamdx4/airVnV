import { useState, useEffect, useRef, useCallback } from 'react';
import * as signalR from '@microsoft/signalr';

interface TypingData {
  conversationId: string;
  userId: string;
  isTyping: boolean;
}

// Hook 1: Dành cho Component cần hiển thị UI (MessageList)
export const useTypingSubscriber = (
  connection: signalR.HubConnection | null,
  activeConversationId: string | null
) => {
  const [isTyping, setIsTyping] = useState(false);
  const activeConversationIdRef = useRef(activeConversationId);

  useEffect(() => {
    activeConversationIdRef.current = activeConversationId;
  }, [activeConversationId]);

  useEffect(() => {
    if (!connection) return;

    const handleUserTyping = (data: TypingData) => {
      const currentActiveId = activeConversationIdRef.current;
      if (!currentActiveId || data.conversationId.toLowerCase() !== currentActiveId.toLowerCase()) {
        return;
      }
      setIsTyping(data.isTyping);
    };

    connection.on('UserTyping', handleUserTyping);

    return () => {
      connection.off('UserTyping', handleUserTyping);
    };
  }, [connection]);

  useEffect(() => {
    setIsTyping(false);
  }, [activeConversationId]);

  return isTyping;
};

// Hook 2: Dành cho Component nhập văn bản (MessageInput)
export const useTypingPublisher = (
  connection: signalR.HubConnection | null,
  activeConversationId: string | null
) => {
  const typingTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const isTypingRef = useRef(false);

  useEffect(() => {
    isTypingRef.current = false;
    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }
  }, [activeConversationId]);

  const handleTyping = useCallback(() => {
    if (!connection || !activeConversationId || connection.state !== signalR.HubConnectionState.Connected) return;

    if (!isTypingRef.current) {
      isTypingRef.current = true;
      connection.invoke('SendTypingStatus', activeConversationId, true).catch(console.error);
    }

    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }

    typingTimeoutRef.current = setTimeout(() => {
      isTypingRef.current = false;
      if (connection.state === signalR.HubConnectionState.Connected) {
        connection.invoke('SendTypingStatus', activeConversationId, false).catch(console.error);
      }
    }, 3000);
  }, [connection, activeConversationId]);

  const stopTyping = useCallback(() => {
    if (!connection || !activeConversationId || connection.state !== signalR.HubConnectionState.Connected) return;
    
    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }

    if (isTypingRef.current) {
      isTypingRef.current = false;
      connection.invoke('SendTypingStatus', activeConversationId, false).catch(console.error);
    }
  }, [connection, activeConversationId]);

  return {
    handleTyping,
    stopTyping
  };
};
