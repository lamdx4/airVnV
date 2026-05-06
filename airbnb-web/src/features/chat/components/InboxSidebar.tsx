import React from 'react';
import type { Conversation } from '../types/model';
import { useInbox } from '../hooks/useInbox';

interface InboxSidebarProps {
  activeConversationId: string | null;
  onSelectConversation: (id: string) => void;
}

export const InboxSidebar: React.FC<InboxSidebarProps> = ({ activeConversationId, onSelectConversation }) => {
  const { data, fetchNextPage, hasNextPage, isFetchingNextPage, isLoading } = useInbox();

  const handleScroll = (e: React.UIEvent<HTMLDivElement>) => {
    const bottom = e.currentTarget.scrollHeight - e.currentTarget.scrollTop === e.currentTarget.clientHeight;
    if (bottom && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  };

  if (isLoading) return <div className="p-4 text-gray-500">Đang tải hộp thư...</div>;

  const conversations = data?.pages.flatMap(page => page.items) || [];

  return (
    <div className="w-80 border-r border-gray-200 h-full flex flex-col bg-white">
      <div className="p-4 border-b border-gray-200 font-semibold text-lg">
        Tin nhắn
      </div>
      <div 
        className="flex-1 overflow-y-auto"
        onScroll={handleScroll}
      >
        {conversations.length === 0 ? (
          <div className="p-4 text-sm text-gray-500">Chưa có hội thoại nào.</div>
        ) : (
          conversations.map((conv: Conversation) => (
            <div 
              key={conv.id}
              onClick={() => onSelectConversation(conv.id)}
              className={`p-4 border-b border-gray-100 cursor-pointer transition-colors hover:bg-gray-50 ${
                activeConversationId === conv.id ? 'bg-blue-50 border-l-4 border-l-blue-600' : 'border-l-4 border-l-transparent'
              }`}
            >
              <div className="flex items-center space-x-3">
                {conv.otherParticipantAvatar ? (
                  <img src={conv.otherParticipantAvatar} alt="avatar" className="w-10 h-10 rounded-full object-cover" />
                ) : (
                  <div className="w-10 h-10 rounded-full bg-gray-200 flex items-center justify-center text-gray-600 font-bold">
                    {conv.otherParticipantName.charAt(0).toUpperCase()}
                  </div>
                )}
                <div className="flex-1 min-w-0">
                  <div className="flex justify-between items-baseline">
                    <p className="text-sm font-medium text-gray-900 truncate">
                      {conv.otherParticipantName}
                    </p>
                    <p className="text-xs text-gray-500">
                      {conv.lastMessageAt ? new Date(conv.lastMessageAt).toLocaleDateString() : ''}
                    </p>
                  </div>
                  <p className="text-xs text-gray-500 truncate mt-1">
                    {conv.propertyTitle}
                  </p>
                </div>
                {conv.unreadCount > 0 && (
                  <div className="bg-red-500 text-white text-xs font-bold px-2 py-1 rounded-full">
                    {conv.unreadCount}
                  </div>
                )}
              </div>
            </div>
          ))
        )}
        {isFetchingNextPage && <div className="p-4 text-center text-xs text-gray-400">Đang tải thêm...</div>}
      </div>
    </div>
  );
};
