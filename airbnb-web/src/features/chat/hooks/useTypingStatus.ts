import { useState, useEffect, useRef, useCallback } from 'react';
import * as signalR from '@microsoft/signalr';

interface TypingData {
  conversationId: string;
  userId: string;
  isTyping: boolean;
}

export const useTypingStatus = (
  connection: signalR.HubConnection | null,
  activeConversationId: string | null
) => {
  // state kiểm tra xem đối phương có đang typing không (vì chat 1-1)
  const [isTyping, setIsTyping] = useState(false);
  
  const typingTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const isTypingRef = useRef(false);

  // 1. Subscriber: Lắng nghe sự kiện từ đối phương qua SignalR
  useEffect(() => {
    if (!connection) return;

    const handleUserTyping = (data: TypingData) => {
      // Chỉ quan tâm sự kiện của phòng hiện tại
      if (!activeConversationId || data.conversationId.toLowerCase() !== activeConversationId.toLowerCase()) {
        return;
      }

      setIsTyping(data.isTyping);
    };

    connection.on('UserTyping', handleUserTyping);

    return () => {
      connection.off('UserTyping', handleUserTyping);
    };
  }, [connection, activeConversationId]);

  // 2. Clear state khi đổi phòng chat
  useEffect(() => {
    setIsTyping(false);
    isTypingRef.current = false;
    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }
  }, [activeConversationId]);

  // 3. Publisher: Hàm xử lý khi mình gõ phím (sử dụng debounce)
  const handleTyping = useCallback(() => {
    if (!connection || !activeConversationId || connection.state !== signalR.HubConnectionState.Connected) return;

    // Nếu trước đó chưa gõ, phát tín hiệu true
    if (!isTypingRef.current) {
      isTypingRef.current = true;
      connection.invoke('SendTypingStatus', activeConversationId, true).catch(console.error);
    }

    // Reset lại timeout
    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }

    // Nếu sau 3 giây không gõ phím nào nữa -> phát tín hiệu false
    typingTimeoutRef.current = setTimeout(() => {
      isTypingRef.current = false;
      if (connection.state === signalR.HubConnectionState.Connected) {
        connection.invoke('SendTypingStatus', activeConversationId, false).catch(console.error);
      }
    }, 3000);
  }, [connection, activeConversationId]);

  // 4. Hàm chủ động báo ngừng gõ ngay lập tức (dùng khi vừa gửi tin nhắn xong)
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
    isTyping,
    handleTyping,
    stopTyping
  };
};
