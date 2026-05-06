import React, { useEffect, useRef } from 'react';
import { useMessages } from '../hooks/useMessages';
import { useSendMessage } from '../hooks/useSendMessage';
import { MessageBubble } from './MessageBubble';
import { ChatInput } from './ChatInput';
import { useInbox } from '../hooks/useInbox';
import { chatApi } from '../api/chatApi';
import { useQueryClient } from '@tanstack/react-query';

interface ChatWindowProps {
  conversationId: string | null;
}

export const ChatWindow: React.FC<ChatWindowProps> = ({ conversationId }) => {
  const { data: inboxData } = useInbox();
  const { data: messagesData, fetchNextPage, hasNextPage, isFetchingNextPage, isLoading } = useMessages(conversationId);
  const sendMessageMutation = useSendMessage(conversationId);
  const currentUserId = localStorage.getItem('airbnb_user_id') || '';
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const queryClient = useQueryClient();

  // Lấy thông tin Header từ Inbox list
  const activeConversation = inboxData?.pages.flatMap(p => p.items).find(c => c.id === conversationId);

  // Cuộn xuống cuối (hoặc lên đầu tùy flex) khi có tin nhắn mới
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messagesData?.pages[0]?.items.length]); // Theo dõi số lượng tin nhắn trang 0

  const handleScroll = (e: React.UIEvent<HTMLDivElement>) => {
    // Vì danh sách đảo ngược (flex-col-reverse), scroll top sẽ nằm ở số âm hoặc 0 tùy trình duyệt
    // Đoạn code này phụ thuộc vào cách CSS cấu hình. Nếu scroll bình thường từ trên xuống:
    if (e.currentTarget.scrollTop === 0 && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  };

  // Logic markAsRead
  useEffect(() => {
    if (!conversationId || !messagesData) return;

    const messages = messagesData.pages.flatMap(p => p.items);
    if (messages.length === 0) return;

    // Tìm tin nhắn mới nhất không phải của mình
    const latestOtherMessage = messages.find(m => m.senderId !== currentUserId);
    if (!latestOtherMessage) return;

    // Đánh dấu đã đọc nếu activeConversation đang có unreadCount > 0
    // Hoặc gọi an toàn luôn khi focus
    const handleFocus = () => {
      chatApi.markAsRead(conversationId, latestOtherMessage.id)
        .then(() => {
          queryClient.invalidateQueries({ queryKey: ['chat', 'inbox'] });
        })
        .catch(console.error);
    };

    // Khi vừa mở chat window, nếu có unreadCount > 0
    if (activeConversation && activeConversation.unreadCount > 0) {
      handleFocus();
    }

    // Lắng nghe sự kiện window focus để markAsRead lại
    window.addEventListener('focus', handleFocus);
    return () => window.removeEventListener('focus', handleFocus);
  }, [conversationId, messagesData, activeConversation?.unreadCount, currentUserId, queryClient]);

  if (!conversationId) {
    return (
      <div className="flex-1 flex items-center justify-center bg-gray-50 h-full">
        <div className="text-center">
          <div className="w-16 h-16 bg-gray-200 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg className="w-8 h-8 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
            </svg>
          </div>
          <h3 className="text-lg font-medium text-gray-900">Tin nhắn của bạn</h3>
          <p className="text-sm text-gray-500 mt-1">Chọn một hội thoại để bắt đầu trò chuyện</p>
        </div>
      </div>
    );
  }

  const messages = messagesData?.pages.flatMap(page => page.items) || [];
  // Backend trả về mới nhất trước, nên ta cần map từ dưới lên, hoặc dùng flex-col-reverse
  // Ở đây ta đảo ngược mảng để render từ trên xuống dưới (cũ đến mới)
  const reversedMessages = [...messages].reverse();

  return (
    <div className="flex-1 flex flex-col h-full bg-white relative">
      {/* Header */}
      <div className="h-16 border-b border-gray-200 flex items-center px-6 bg-white shadow-sm z-10">
        <div className="flex flex-col">
          <h2 className="font-semibold text-gray-900 text-lg">{activeConversation?.otherParticipantName || 'Đang tải...'}</h2>
          <span className="text-xs text-gray-500">{activeConversation?.propertyTitle}</span>
        </div>
      </div>

      {/* Body */}
      <div 
        className="flex-1 overflow-y-auto p-6 bg-gray-50"
        onScroll={handleScroll}
      >
        {isFetchingNextPage && <div className="text-center text-xs text-gray-400 py-2">Đang tải lịch sử...</div>}
        
        {isLoading ? (
          <div className="flex justify-center items-center h-full text-gray-500">Đang tải tin nhắn...</div>
        ) : (
          <div className="flex flex-col space-y-2">
            {reversedMessages.map((msg) => (
              <MessageBubble 
                key={msg.id} 
                message={msg} 
                isCurrentUser={msg.senderId === currentUserId} 
              />
            ))}
            <div ref={messagesEndRef} />
          </div>
        )}
      </div>

      {/* Input */}
      <ChatInput 
        onSend={(content) => sendMessageMutation.mutate(content)} 
        isLoading={sendMessageMutation.isPending}
      />
    </div>
  );
};
