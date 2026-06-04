import React from 'react';
import { Icon } from '@iconify/react';
import type { Conversation } from '../types/model';
import { useInbox } from '../hooks/useInbox';
import { useChat } from '../context/ChatContext';
import { Button } from '@/components/ui/button';
import { formatDistanceToNow } from 'date-fns';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Skeleton } from '@/components/ui/skeleton';
import { Search } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { Loading03Icon } from '@/components/common/Icons';
import { useNavigate } from 'react-router-dom';
import { ChevronLeft } from 'lucide-react';

export const ConversationList: React.FC = () => {
  const { activeConversationId, setActiveConversationId, closeSidebar } = useChat();
  const { data, fetchNextPage, hasNextPage, isFetchingNextPage, isLoading } = useInbox();
  const navigate = useNavigate();

  const handleScroll = (e: React.UIEvent<HTMLDivElement>) => {
    const bottom = e.currentTarget.scrollHeight - e.currentTarget.scrollTop === e.currentTarget.clientHeight;
    if (bottom && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  };

  const conversations = data?.pages.flatMap(page => page.items) || [];

  if (isLoading) {
    return (
      <div className="flex flex-col h-full bg-white border-r border-slate-200">
        <div className="p-6 pb-2">
            <h1 className="text-2xl font-semibold text-slate-900">Messages</h1>
        </div>
        <div className="p-6 space-y-4">
          {[1, 2, 3, 4, 5].map(i => (
            <div key={i} className="flex items-center gap-4">
              <Skeleton className="h-12 w-12 rounded-full" />
              <div className="flex-1 space-y-2">
                <Skeleton className="h-4 w-3/4" />
                <Skeleton className="h-3 w-1/2" />
              </div>
            </div>
          ))}
        </div>
      </div>
    );
  }

  return (
    <div className="flex flex-col h-full bg-[#fafafa] border-r border-[#ebebeb] w-full md:w-[380px] shrink-0">
      <div className="p-6 space-y-4">
        <div className="flex items-center gap-2">
          <Button 
            variant="ghost" 
            size="icon" 
            onClick={() => navigate('/')}
            className="h-8 w-8 rounded-full hover:bg-[#ebebeb] -ml-2"
          >
            <ChevronLeft className="h-5 w-5 text-[#222222]" />
          </Button>
          <h1 className="text-[22px] font-semibold text-[#222222] tracking-tight">Messages</h1>
        </div>
        <div className="relative group">
          <Search className="absolute left-4 top-1/2 -translate-y-1/2 h-4 w-4 text-[#b0b0b0] group-focus-within:text-[#222222] transition-colors" />
          <Input 
            placeholder="Search messages" 
            className="pl-11 h-[48px] bg-[#f7f7f7] border-transparent focus:bg-white focus:border-[#222222] transition-all rounded-full text-[14px]"
          />
        </div>
      </div>

      <div 
        className="flex-1 overflow-y-auto px-0 custom-scrollbar"
        onScroll={handleScroll}
      >
        {conversations.length === 0 ? (
          <div className="flex flex-col items-center justify-center h-64 p-8 text-center space-y-4">
            <div className="p-5 bg-white rounded-full shadow-xs border border-[#ebebeb]">
                <Icon icon="hugeicons:message-01" className="text-3xl text-[#b0b0b0]" />
            </div>
            <div className="space-y-1">
              <p className="text-[15px] font-semibold text-[#222222]">You have no unread messages</p>
              <p className="text-[13px] text-[#6a6a6a] leading-relaxed">When you book a trip or host an experience, your messages will show up here.</p>
            </div>
          </div>
        ) : (
          <div className="space-y-1 pb-4">
            {conversations.map((conv: Conversation) => (
              <Button 
                key={conv.id}
                variant="ghost"
                aria-label={`Chat with ${conv.otherParticipantName} about ${conv.propertyTitle}`}
                onClick={() => {
                  setActiveConversationId(conv.id);
                  if (window.innerWidth < 768) closeSidebar();
                }}
                className={`
                  w-[calc(100%-1.5rem)] h-auto group mx-3 my-1 rounded-2xl px-4 py-4
                  transition-all duration-200 text-left flex items-start gap-4 justify-start whitespace-normal hover:bg-white hover:shadow-xs border border-transparent
                  ${activeConversationId === conv.id 
                    ? 'bg-white shadow-sm border border-[#efefef] hover:border-[#efefef]' 
                    : ''
                  }
                `}
              >
                <div className="relative shrink-0">
                  <Avatar className="h-14 w-14 ring-1 ring-black/5">
                    <AvatarImage src={conv.otherParticipantAvatar || ''} alt={conv.otherParticipantName} />
                    <AvatarFallback className="bg-[#f2f2f2] text-[#222222] font-semibold text-lg">
                        {conv.otherParticipantName.charAt(0)}
                    </AvatarFallback>
                  </Avatar>
                  {conv.unreadCount > 0 && (
                    <span className="absolute -top-0.5 -right-0.5 flex h-3.5 w-3.5">
                        <span className="relative inline-flex rounded-full h-3.5 w-3.5 bg-[#FF5A5F] border-2 border-white"></span>
                    </span>
                  )}
                </div>
                
                <div className="flex-1 min-w-0">
                  <div className="flex justify-between items-baseline mb-1">
                    <h3 className={`text-[15px] truncate transition-colors ${
                        conv.unreadCount > 0 ? 'font-bold text-[#222222]' : 'font-semibold text-[#222222]'
                     }`}>
                      {conv.otherParticipantName}
                    </h3>
                    <span className="text-[12px] text-[#b0b0b0] font-normal">
                      {conv.lastMessageAt ? formatDistanceToNow(new Date(conv.lastMessageAt), { addSuffix: false }) : ''}
                    </span>
                  </div>
                  
                  <p className={`text-[14px] truncate leading-snug mb-1 ${
                      conv.unreadCount > 0 ? 'font-semibold text-[#222222]' : 'text-[#6a6a6a] font-normal'
                  }`}>
                    {conv.propertyTitle}
                  </p>
                  
                  <p className="text-[12px] text-[#b0b0b0] truncate font-normal">
                    Tap to view message history
                  </p>
                </div>
              </Button>
            ))}
          </div>
        )}
        {isFetchingNextPage && (
            <div className="flex justify-center p-6">
                <Loading03Icon className="h-6 w-6 animate-spin text-[#b0b0b0]" />
            </div>
        )}
      </div>
    </div>
  );
};

// Internal icon dependency
