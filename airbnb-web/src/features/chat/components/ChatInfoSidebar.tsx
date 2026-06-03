import React from 'react';
import { useChat } from '../context/ChatContext';
import { useInbox } from '../hooks/useInbox';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Icon } from '@iconify/react';

export const ChatInfoSidebar: React.FC = () => {
  const { isInfoSidebarOpen, toggleInfoSidebar, activeConversationId } = useChat();
  const { data } = useInbox();

  if (!isInfoSidebarOpen || !activeConversationId) return null;

  const conversation = data?.pages
    .flatMap(page => page.items)
    .find(c => c.id === activeConversationId);

  if (!conversation) return null;

  return (
    <div className="w-full md:w-[380px] bg-white border-l border-[#ebebeb] flex flex-col h-full shrink-0 animate-in slide-in-from-right-10 duration-200 absolute md:relative z-20 right-0">
      {/* Header */}
      <div className="h-20 px-6 border-b border-[#ebebeb] flex items-center justify-between shrink-0">
        <h2 className="text-[16px] font-semibold text-[#222222]">Details</h2>
        <button 
          onClick={toggleInfoSidebar}
          className="p-2 rounded-full hover:bg-[#f7f7f7] transition-colors"
          aria-label="Close sidebar"
        >
          <Icon icon="lucide:x" className="size-5 text-[#222222]" />
        </button>
      </div>

      <div className="flex-1 overflow-y-auto custom-scrollbar p-6 space-y-8">
        {/* Profile Section */}
        <div className="flex flex-col items-center text-center space-y-4">
          <Avatar className="h-24 w-24 ring-1 ring-black/5">
            <AvatarImage src={conversation.otherParticipantAvatar || ''} />
            <AvatarFallback className="bg-[#f2f2f2] text-[#222222] font-semibold text-3xl">
              {conversation.otherParticipantName.charAt(0)}
            </AvatarFallback>
          </Avatar>
          <div>
            <h3 className="text-xl font-semibold text-[#222222]">
              {conversation.otherParticipantName}
            </h3>
            <p className="text-[15px] text-[#6a6a6a] mt-1">Guest</p>
          </div>
        </div>

        <div className="w-full h-px bg-[#ebebeb]"></div>

        {/* Property Info (Static for now) */}
        {conversation.propertyTitle && (
          <div className="space-y-3">
            <h4 className="text-[15px] font-semibold text-[#222222]">Listing</h4>
            <div className="flex items-center gap-3 p-3 rounded-xl border border-[#ebebeb] hover:shadow-sm transition-shadow cursor-pointer">
              <div className="h-12 w-12 bg-slate-200 rounded-lg shrink-0 flex items-center justify-center text-slate-400">
                <Icon icon="lucide:home" className="size-5" />
              </div>
              <p className="text-[14px] font-medium text-[#222222] line-clamp-2">
                {conversation.propertyTitle}
              </p>
            </div>
          </div>
        )}

        <div className="w-full h-px bg-[#ebebeb]"></div>

        {/* Media / Files Placeholder */}
        <div className="space-y-4">
          <h4 className="text-[15px] font-semibold text-[#222222]">Shared Media</h4>
          <div className="grid grid-cols-3 gap-2">
            {[1, 2, 3, 4, 5, 6].map(i => (
              <div key={i} className="aspect-square bg-[#f7f7f7] rounded-lg border border-[#ebebeb] flex items-center justify-center">
                <Icon icon="fluent:image-24-regular" className="size-6 text-[#dddddd]" />
              </div>
            ))}
          </div>
        </div>

      </div>
    </div>
  );
};
