import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { useQueryClient } from '@tanstack/react-query';
import type { ChatMessage, Conversation } from '../types/model';
import type { MessageDto } from '../types/dto';
import { mapMessageDtoToModel } from '../utils/mapper';

const HUB_URL = import.meta.env.VITE_CHAT_HUB_URL || 'http://localhost:5004/hubs/chat';

export const useChatHub = (activeConversationId: string | null) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const queryClient = useQueryClient();

  useEffect(() => {
    const token = localStorage.getItem('airbnb_access_token');
    if (!token) return;

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (!connection) return;

    const startConnection = async () => {
      try {
        await connection.start();
        console.log('SignalR Connected.');

        // Khi kết nối thành công, join vào group của conversation hiện tại (nếu có)
        if (activeConversationId) {
          await connection.invoke('JoinConversation', activeConversationId);
        }

        // Lắng nghe tin nhắn mới
        connection.on('ReceiveMessage', (messageDto: MessageDto) => {
          const currentUserId = localStorage.getItem('airbnb_user_id');
          // Bỏ qua message do chính mình gửi vì Optimistic update trong useSendMessage đã thêm rồi
          if (messageDto.senderId === currentUserId) return;

          const newMsg = mapMessageDtoToModel(messageDto);
          
          // 1. Cập nhật cache của Messages list (nếu đang mở đúng conversation đó)
          queryClient.setQueryData(['chat', 'messages', newMsg.conversationId], (old: any) => {
            if (!old) return old;
            const newPages = [...old.pages];
            if (newPages.length > 0) {
              // Tránh duplicate nếu optimistic UI đã add
              const exists = newPages[0].items.some((m: ChatMessage) => m.id === newMsg.id || m.id.startsWith('temp-'));
              if (!exists) {
                newPages[0] = { ...newPages[0], items: [newMsg, ...newPages[0].items] };
              }
            }
            return { ...old, pages: newPages };
          });

          // 2. Cập nhật cache của Inbox (đẩy conversation lên đầu, tăng unread, đổi last message)
          queryClient.setQueryData(['chat', 'inbox'], (old: any) => {
            if (!old) return old;
            const newPages = [...old.pages];
            
            // Tìm và sửa conversation
            let found = false;
            for (let i = 0; i < newPages.length; i++) {
              const items = [...newPages[i].items];
              const idx = items.findIndex((c: Conversation) => c.id === newMsg.conversationId);
              if (idx !== -1) {
                const conv = { ...items[idx] };
                conv.lastMessageAt = newMsg.sentAt;
                // Nếu tin nhắn mới không phải ở conversation đang mở thì tăng unread count
                if (activeConversationId !== newMsg.conversationId) {
                  conv.unreadCount += 1;
                }
                
                // Đẩy lên đầu danh sách của page 0
                items.splice(idx, 1);
                newPages[0].items = [conv, ...newPages[0].items];
                
                // Cập nhật lại các item còn lại cho đúng page (logic này có thể phức tạp, 
                // đơn giản nhất là invalidate lại inbox nếu muốn chính xác tuyệt đối)
                found = true;
                break;
              }
            }
            
            // Nếu không tìm thấy trong cache (chưa load tới), invalidate
            if (!found) {
              queryClient.invalidateQueries({ queryKey: ['chat', 'inbox'] });
            }

            return { ...old, pages: newPages };
          });
        });

        // Lắng nghe sự kiện Read
        connection.on('MessageRead', (_conversationId: string, _messageId: string) => {
           // Có thể trigger cập nhật trạng thái "Đã xem" ở UI
           // (Tùy thuộc model có lưu trường isRead hay không)
           queryClient.invalidateQueries({ queryKey: ['chat', 'inbox'] });
        });

      } catch (e) {
        console.error('SignalR Connection Error: ', e);
      }
    };

    startConnection();

    return () => {
      if (activeConversationId) {
        connection.invoke('LeaveConversation', activeConversationId).catch(console.error);
      }
      connection.stop();
    };
  }, [connection, activeConversationId, queryClient]);

  return connection;
};
