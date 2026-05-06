import { useParams, useNavigate } from 'react-router-dom';
import { InboxSidebar } from '../features/chat/components/InboxSidebar';
import { ChatWindow } from '../features/chat/components/ChatWindow';
import { useChatHub } from '../features/chat/hooks/useChatHub';

export default function Messages() {
  const { conversationId } = useParams();
  const navigate = useNavigate();

  // Khởi tạo kết nối SignalR ở mức Page để quản lý global cho toàn bộ nhánh chat
  useChatHub(conversationId || null);

  const handleSelectConversation = (id: string) => {
    navigate(`/messages/${id}`);
  };

  return (
    <div className="flex h-[calc(100vh-80px)] overflow-hidden bg-gray-50 border-t border-gray-200">
      <InboxSidebar 
        activeConversationId={conversationId || null} 
        onSelectConversation={handleSelectConversation} 
      />
      <div className="flex-1 min-w-0">
        <ChatWindow conversationId={conversationId || null} />
      </div>
    </div>
  );
}
