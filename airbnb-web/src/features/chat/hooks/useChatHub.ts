import { useEffect, useState, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import { useQueryClient } from "@tanstack/react-query";
import type { Conversation } from "../types/model";
import { mapMessageDtoToModel } from "../utils/mapper";
import { useAuthStore } from "../../../store/authStore";
import { jwtDecode } from "jwt-decode";
import { chatApi } from "../api/chatApi";

const HUB_URL =
  import.meta.env.VITE_CHAT_HUB_URL || "http://localhost:5136/hubs/chat";

export const useChatHub = (activeConversationId: string | null) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null,
  );
  const [isConnected, setIsConnected] = useState(false);
  const queryClient = useQueryClient();
  const currentUserId = useAuthStore((state) => state.userId);

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
        accessTokenFactory: async () => {
          let token = useAuthStore.getState().accessToken;
          if (!token) return "";

          try {
            const decoded = jwtDecode<{ exp: number }>(token);
            const expTime = decoded.exp * 1000;
            const now = Date.now();

            // Nếu token sắp hết hạn (dưới 1 phút), chủ động đi lấy token mới
            if (expTime - now < 1 * 60 * 1000) {
              const refreshToken = useAuthStore.getState().refreshToken;
              if (refreshToken) {
                const result = await chatApi.refreshSignalRToken(refreshToken);

                if (result.success && result.data?.accessToken) {
                  token = result.data.accessToken;
                  // Hàm login của store sẽ cập nhật cả localStorage và state
                  useAuthStore
                    .getState()
                    .login(result.data.accessToken, result.data.refreshToken);
                }
              }
            }
          } catch (e) {
            console.error("Lỗi khi kiểm tra token cho SignalR:", e);
          }

          return token;
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
        console.log("SignalR Connected.");
        setIsConnected(true);
      } catch (e) {
        console.error("SignalR Connection Error: ", e);
      }
    };

    // ReceiveMessage listener is moved to useMessages hook

    // Bắt sự kiện khi tự động kết nối lại thành công sau khi rớt mạng
    connection.onreconnected(() => {
      console.log("SignalR Reconnected. Invalidating all chat caches...");
      queryClient.invalidateQueries({ queryKey: ["chat", "inbox"] });
      queryClient.invalidateQueries({ queryKey: ["chat", "messages"] });
    });
    // Lắng nghe sự kiện Read realtime từ SignalR
    connection.on("MessageRead", (data: any) => {
      const conversationId = data?.conversationId || data?.ConversationId;
      const readerId = data?.readerId || data?.ReaderId;
      const lastReadMessageId =
        data?.lastReadMessageId || data?.LastReadMessageId;

      if (readerId?.toLowerCase() !== currentUserIdRef.current?.toLowerCase()) {
        queryClient.setQueryData(["chat", "inbox"], (old: any) => {
          if (!old) return old;
          const newPages = old.pages.map((page: any) => ({
            ...page,
            items: page.items.map((c: Conversation) => {
              if (c.id?.toLowerCase() === conversationId?.toLowerCase()) {
                return {
                  ...c,
                  otherLastReadMessageId: lastReadMessageId,
                };
              }
              return c;
            }),
          }));
          return { ...old, pages: newPages };
        });
      }
    });

    // Lắng nghe sự kiện NewMessage (từ SignalR gửi qua user_ group)
    connection.on("NewMessage", (messageDto: any) => {
      const newMsg = mapMessageDtoToModel(messageDto);
      const currentActiveId = activeConversationIdRef.current;
      const isMyMessage =
        newMsg.senderId?.toLowerCase() ===
        currentUserIdRef.current?.toLowerCase();

      queryClient.setQueryData(["chat", "inbox"], (old: any) => {
        if (!old) return old;
        const newPages = [...old.pages];

        let targetConv: Conversation | null = null;

        for (let i = 0; i < newPages.length; i++) {
          const items = [...newPages[i].items];
          const idx = items.findIndex(
            (c: Conversation) =>
              c.id?.toLowerCase() === newMsg.conversationId?.toLowerCase(),
          );
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
          const isCurrentlyActive =
            currentActiveId?.toLowerCase() ===
            newMsg.conversationId?.toLowerCase();
          if (!isCurrentlyActive) {
            if (!isMyMessage) {
              targetConv.unreadCount += 1;
            }

            queryClient.setQueryData(
              ["chat", "messages", newMsg.conversationId],
              (oldMsgs: any) => {
                if (!oldMsgs) return oldMsgs;
                const newMsgsPages = [...oldMsgs.pages];
                if (newMsgsPages.length > 0) {
                  const msgItems = [...newMsgsPages[0].items];
                  const exists = msgItems.some((m: any) => m.id === newMsg.id);
                  if (!exists) {
                    newMsgsPages[0] = {
                      ...newMsgsPages[0],
                      items: [newMsg, ...msgItems],
                    };
                  }
                }
                return { ...oldMsgs, pages: newMsgsPages };
              },
            );
          }

          if (newPages.length > 0) {
            newPages[0] = {
              ...newPages[0],
              items: [targetConv, ...newPages[0].items],
            };
          }
        } else {
          queryClient.invalidateQueries({ queryKey: ["chat", "inbox"] });
        }

        return { ...old, pages: newPages };
      });
    });

    // UserStatusChanged event listener is moved to usePresence hook

    startConnection();

    return () => {
      // Khi component unmount hoặc connection thay đổi, gỡ bỏ lắng nghe sự kiện
      connection.off("MessageRead");
      connection.off("NewMessage");
      setIsConnected(false);
    };
  }, [connection, queryClient]);

  // 3. Xử lý Join/Leave phòng chat mỗi khi đổi Conversation mà không ngắt Socket
  useEffect(() => {
    if (!connection || !isConnected || !activeConversationId) return;

    // Vào phòng mới
    connection
      .invoke("JoinConversation", activeConversationId)
      .then(() =>
        console.log(`Joined conversation group: ${activeConversationId}`),
      )
      .catch((err) => console.error("Error joining conversation group:", err));

    // Cleanup: Ra khỏi phòng khi đổi phòng hoặc đóng khung chat
    return () => {
      if (connection.state === signalR.HubConnectionState.Connected) {
        connection
          .invoke("LeaveConversation", activeConversationId)
          .then(() =>
            console.log(`Left conversation group: ${activeConversationId}`),
          )
          .catch((err) =>
            console.error("Error leaving conversation group:", err),
          );
      }
    };
  }, [connection, isConnected, activeConversationId]);

  // 4. Gửi Heartbeat định kỳ để gia hạn Redis TTL
  useEffect(() => {
    if (!connection || !isConnected) return;

    const interval = setInterval(() => {
      if (connection.state === signalR.HubConnectionState.Connected) {
        connection.invoke("Heartbeat").catch(console.error);
      }
    }, 45000); // Mỗi 45 giây

    return () => clearInterval(interval);
  }, [connection, isConnected]);

  return connection;
};
