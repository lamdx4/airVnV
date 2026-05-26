import { useEffect, useState, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { useQueryClient } from '@tanstack/react-query';
import type { ChatMessage, Conversation } from '../types/model';
import { mapMessageDtoToModel } from '../utils/mapper';
import { useAuthStore } from '../../../store/authStore';

const HUB_URL = import.meta.env.VITE_CHAT_HUB_URL || 'http://localhost:5136/hubs/chat';

export const useChatHub = (activeConversationId: string | null) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  const queryClient = useQueryClient();
  const currentUserId = useAuthStore(state => state.userId);
  
  // Dùng Ref để lưu lại các giá trị có thể thay đổi, tránh bị Stale Closure trong callback nhận tin nhắn
  const activeConversationIdRef = useRef(activeConversationId);
  const currentUserIdRef = useRef(currentUserId);
  
  useEffect(() => {
    activeConversationIdRef.current = activeConversationId;
  }, [activeConversationId]);

  useEffect(() => {
    currentUserIdRef.current = currentUserId;
  }, [currentUserId]);

  // 1. Khởi tạo Connection duy nhất 1 lần khi Component Mount
  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL, {
        accessTokenFactory: () => {
          // Lấy token động từ Zustand store, tự động cập nhật nếu có thao tác refresh_token xảy ra
          return useAuthStore.getState().accessToken || '';
        },
      })
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);

    // Cleanup: Ngắt kết nối khi thoát hẳn màn hình Chat
    return () => {
      newConnection.stop().catch(console.error);
    };
  }, []);

  // 2. Thiết lập Lắng nghe Sự kiện và Start Connection
  useEffect(() => {
    if (!connection) return;

    const startConnection = async () => {
      try {
        await connection.start();
        console.log('SignalR Connected.');
        setIsConnected(true);
      } catch (e) {
        console.error('SignalR Connection Error: ', e);
      }
    };

    // Đăng ký các sự kiện lắng nghe (chỉ đăng ký 1 lần duy nhất)
    connection.on('ReceiveMessage', (messageDto: any) => {
      // Dùng Ref để chống Closure trap.
      const newMsg = mapMessageDtoToModel(messageDto);
      const currentActiveId = activeConversationIdRef.current;
      const isMyMessage = newMsg.senderId?.toLowerCase() === currentUserIdRef.current?.toLowerCase();
      
      // 1. Cập nhật cache của Messages list (nếu đang mở đúng conversation đó)
      queryClient.setQueryData(['chat', 'messages', newMsg.conversationId], (old: any) => {
        if (!old) return old;
        const newPages = [...old.pages];
        if (newPages.length > 0) {
          const items = [...newPages[0].items];
          
          // Tìm xem có tin nhắn tạm (optimistic UI) của chính mình có nội dung giống để thay thế không
          const tempIdx = items.findIndex((m: ChatMessage) => m.id.startsWith('temp-') && m.content === newMsg.content);
          
          if (tempIdx !== -1) {
            // Thay thế tin nhắn tạm bằng tin nhắn chính thức từ server (có ID thật)
            items[tempIdx] = newMsg;
            newPages[0] = { ...newPages[0], items };
          } else {
            // Nếu không có tin nhắn tạm (hoặc tin nhắn từ người khác gửi tới), kiểm tra tránh trùng lặp ID
            const exists = items.some((m: ChatMessage) => m.id === newMsg.id);
            if (!exists) {
              newPages[0] = { ...newPages[0], items: [newMsg, ...items] };
            }
          }
        }
        return { ...old, pages: newPages };
      });

      // 2. Cập nhật cache của Inbox (đẩy conversation lên đầu, tăng unread, đổi last message)
      queryClient.setQueryData(['chat', 'inbox'], (old: any) => {
        if (!old) return old;
        const newPages = [...old.pages];
        
        let targetConv: Conversation | null = null;
        
        // Tìm và cắt cuộc hội thoại ra khỏi danh sách cũ (bất kể ở trang nào) để tránh nhân bản (duplicate)
        for (let i = 0; i < newPages.length; i++) {
          const items = [...newPages[i].items];
          const idx = items.findIndex((c: Conversation) => c.id?.toLowerCase() === newMsg.conversationId?.toLowerCase());
          if (idx !== -1) {
            targetConv = { ...items[idx] };
            items.splice(idx, 1);
            newPages[i] = { ...newPages[i], items };
            break;
          }
        }
        
        if (targetConv) {
          targetConv.lastMessageAt = newMsg.sentAt;
          targetConv.latestMessageContent = newMsg.content;
          targetConv.latestMessageId = newMsg.id;
          
          // CHỈ TĂNG BADGE NẾU: Tin nhắn đó KHÔNG phải do mình gửi VÀ mình đang KHÔNG mở phòng chat đó
          const isCurrentlyActive = currentActiveId?.toLowerCase() === newMsg.conversationId?.toLowerCase();
          if (!isMyMessage && !isCurrentlyActive) {
            targetConv.unreadCount += 1;
          }
          
          // Đẩy cuộc hội thoại đã cập nhật lên đầu trang 0
          if (newPages.length > 0) {
            newPages[0] = {
              ...newPages[0],
              items: [targetConv, ...newPages[0].items]
            };
          }
        } else {
          // Nếu cuộc hội thoại chưa tồn tại trong cache, invalidate để tải mới hoàn toàn
          queryClient.invalidateQueries({ queryKey: ['chat', 'inbox'] });
        }

        return { ...old, pages: newPages };
      });
    });

    // Lắng nghe sự kiện Read realtime từ SignalR
    connection.on('MessageRead', (data: any) => {
      const conversationId = data?.conversationId || data?.ConversationId;
      const readerId = data?.readerId || data?.ReaderId;
      const lastReadMessageId = data?.lastReadMessageId || data?.LastReadMessageId;

      if (readerId?.toLowerCase() !== currentUserIdRef.current?.toLowerCase()) {
        queryClient.setQueryData(['chat', 'inbox'], (old: any) => {
          if (!old) return old;
          const newPages = old.pages.map((page: any) => ({
            ...page,
            items: page.items.map((c: Conversation) => {
              if (c.id?.toLowerCase() === conversationId?.toLowerCase()) {
                return {
                  ...c,
                  otherLastReadMessageId: lastReadMessageId
                };
              }
              return c;
            })
          }));
          return { ...old, pages: newPages };
        });
      }
    });

    startConnection();

    return () => {
      // Khi component unmount hoặc connection thay đổi, gỡ bỏ lắng nghe sự kiện
      connection.off('ReceiveMessage');
      connection.off('MessageRead');
      setIsConnected(false);
    };
  }, [connection, queryClient]);

  // 3. Xử lý Join/Leave phòng chat mỗi khi đổi Conversation mà không ngắt Socket
  useEffect(() => {
    if (!connection || !isConnected || !activeConversationId) return;

    // Vào phòng mới
    connection.invoke('JoinConversation', activeConversationId)
      .then(() => console.log(`Joined conversation group: ${activeConversationId}`))
      .catch(err => console.error('Error joining conversation group:', err));

    // Cleanup: Ra khỏi phòng khi đổi phòng hoặc đóng khung chat
    return () => {
      if (connection.state === signalR.HubConnectionState.Connected) {
        connection.invoke('LeaveConversation', activeConversationId)
          .then(() => console.log(`Left conversation group: ${activeConversationId}`))
          .catch(err => console.error('Error leaving conversation group:', err));
      }
    };
  }, [connection, isConnected, activeConversationId]);

  return connection;
};
