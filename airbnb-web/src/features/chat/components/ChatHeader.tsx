import React from 'react';
import { useChat } from '../context/ChatContext';
import { useInbox } from '../hooks/useInbox';
import { usePresence } from '../hooks/usePresence';
import { Button } from '@/components/ui/button';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { ChevronLeft, Info, MoreHorizontal } from 'lucide-react';
import { Skeleton } from '@/components/ui/skeleton';

export const ChatHeader: React.FC = () => {
  const { activeConversationId, setActiveConversationId, toggleSidebar } = useChat();
  const { data, isLoading } = useInbox();

  const conversation = data?.pages
    .flatMap(page => page.items)
    .find(c => c.id === activeConversationId);

  const { data: presence } = usePresence(conversation?.otherParticipantId);
  const isOnline = presence?.isOnline;

  if (!activeConversationId) return null;

  if (isLoading || !conversation) {
    return (
      <header className="h-[72px] border-b border-slate-200 bg-white/80 backdrop-blur-md px-4 flex items-center justify-between sticky top-0 z-10">
        <div className="flex items-center gap-4">
          <Skeleton className="h-10 w-10 rounded-full" />
          <div className="space-y-1.5">
            <Skeleton className="h-4 w-32" />
            <Skeleton className="h-3 w-24" />
          </div>
        </div>
      </header>
    );
  }

  return (
    <header className="h-20 px-6 border-b border-[#ebebeb] bg-white/95 backdrop-blur-md flex items-center justify-between sticky top-0 z-30">
      <div className="flex items-center gap-4 overflow-hidden">
        <Button 
          variant="ghost" 
          size="icon" 
          aria-label="Back to conversations"
          onClick={() => setActiveConversationId(null)}
          className="md:hidden h-10 w-10 rounded-full hover:bg-[#f7f7f7]"
        >
          <ChevronLeft className="h-5 w-5 text-[#222222]" />
        </Button>

        <div className="relative group cursor-pointer" onClick={toggleSidebar}>
            <Avatar className="h-11 w-11 ring-1 ring-black/5 group-hover:ring-2 ring-[#FF5A5F]/20 transition-all">
                <AvatarImage src={conversation.otherParticipantAvatar || ''} />
                <AvatarFallback className="bg-[#f2f2f2] text-[#222222] font-semibold text-lg">
                    {conversation.otherParticipantName.charAt(0)}
                </AvatarFallback>
            </Avatar>
            {isOnline && (
              <div className="absolute bottom-0 right-0 w-3.5 h-3.5 bg-[#00a699] border-2 border-white rounded-full z-10 shadow-sm transition-all duration-300 scale-in-center"></div>
            )}
        </div>

        <div className="flex flex-col min-w-0">
          <h2 className="text-[16px] font-semibold text-[#222222] truncate leading-tight">
            {conversation.otherParticipantName}
          </h2>
          <div className="flex items-center gap-1.5 mt-0.5">
            <p className="text-[13px] text-[#6a6a6a] truncate font-normal">
              {conversation.propertyTitle || 'Guest'}
            </p>
            <span className="text-[10px] text-[#dddddd]">•</span>
            <p className={`text-[13px] font-medium ${isOnline ? 'text-[#00a699]' : 'text-[#717171]'}`}>
              {isOnline ? 'Online' : 'Offline'}
            </p>
          </div>
        </div>
      </div>

      <div className="flex items-center gap-2 shrink-0">
        <Button variant="ghost" size="icon" className="h-10 w-10 rounded-full text-[#222222] hover:bg-[#f7f7f7]" aria-label="Conversation info">
          <Info className="h-5 w-5" />
        </Button>
        <Button variant="ghost" size="icon" className="h-10 w-10 rounded-full text-[#222222] hover:bg-[#f7f7f7]" aria-label="More options">
          <MoreHorizontal className="h-5 w-5" />
        </Button>
      </div>
    </header>
  );
};
