import React, { useState } from 'react';
import { useChat } from '../context/ChatContext';
import { useInbox } from '../hooks/useInbox';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Icon } from '@iconify/react';

type SidebarView = 'main' | 'media';

export const ChatInfoSidebar: React.FC = () => {
  const { isInfoSidebarOpen, toggleInfoSidebar, activeConversationId } = useChat();
  const { data } = useInbox();
  const [currentView, setCurrentView] = useState<SidebarView>('main');
  const [activeMediaTab, setActiveMediaTab] = useState<'images' | 'files'>('images');

  // Reset view when sidebar is closed
  React.useEffect(() => {
    if (!isInfoSidebarOpen) {
      setCurrentView('main');
      setActiveMediaTab('images');
    }
  }, [isInfoSidebarOpen]);

  if (!isInfoSidebarOpen || !activeConversationId) return null;

  const conversation = data?.pages
    .flatMap(page => page.items)
    .find(c => c.id === activeConversationId);

  if (!conversation) return null;

  return (
    <div className="w-full md:w-[380px] bg-white border-l border-[#ebebeb] flex flex-col h-full shrink-0 animate-in slide-in-from-right-10 duration-200 absolute md:relative z-20 right-0">
      
      {currentView === 'main' && (
        <>
          <div className="h-20 px-6 border-b border-[#ebebeb] flex items-center justify-between shrink-0">
            <h2 className="text-[16px] font-semibold text-[#222222]">Details</h2>
            <button 
              onClick={toggleInfoSidebar}
              className="p-2 rounded-full hover:bg-[#f7f7f7] transition-colors cursor-pointer"
              aria-label="Close sidebar"
            >
              <Icon icon="lucide:x" className="size-5 text-[#222222]" />
            </button>
          </div>

          <div className="flex-1 overflow-y-auto custom-scrollbar p-6 space-y-8">
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

            <div className="flex flex-col gap-1">
              <button 
                onClick={() => {
                  setCurrentView('media');
                  setActiveMediaTab('images');
                }} 
                className="w-full flex items-center justify-between p-3 rounded-xl hover:bg-[#f7f7f7] transition-colors text-left group cursor-pointer"
              >
                <div className="flex items-center gap-3">
                  <div className="h-10 w-10 bg-[#f2f2f2] rounded-full flex items-center justify-center group-hover:bg-white transition-colors">
                    <Icon icon="fluent:image-24-regular" className="size-5 text-[#222222]" />
                  </div>
                  <span className="text-[15px] font-semibold text-[#222222]">Images</span>
                </div>
                <Icon icon="lucide:chevron-right" className="size-5 text-[#b0b0b0]" />
              </button>
              
              <button 
                onClick={() => {
                  setCurrentView('media');
                  setActiveMediaTab('files');
                }} 
                className="w-full flex items-center justify-between p-3 rounded-xl hover:bg-[#f7f7f7] transition-colors text-left group cursor-pointer"
              >
                <div className="flex items-center gap-3">
                  <div className="h-10 w-10 bg-[#f2f2f2] rounded-full flex items-center justify-center group-hover:bg-white transition-colors">
                    <Icon icon="fluent:document-24-regular" className="size-5 text-[#222222]" />
                  </div>
                  <span className="text-[15px] font-semibold text-[#222222]">Files</span>
                </div>
                <Icon icon="lucide:chevron-right" className="size-5 text-[#b0b0b0]" />
              </button>
            </div>
          </div>
        </>
      )}

      {currentView === 'media' && (
        <div className="flex flex-col h-full">
          <div className="h-20 px-6 border-b border-[#ebebeb] flex items-center shrink-0 gap-4">
            <button 
              onClick={() => setCurrentView('main')}
              className="p-2 -ml-2 rounded-full hover:bg-[#f7f7f7] transition-colors cursor-pointer"
              aria-label="Back"
            >
              <Icon icon="lucide:chevron-left" className="size-5 text-[#222222]" />
            </button>
            <h2 className="text-[16px] font-semibold text-[#222222]">Shared Media</h2>
          </div>

          {/* Tabs */}
          <div className="px-6 pt-5 shrink-0">
            <div className="flex bg-[#f7f7f7] p-1 rounded-xl">
              <button 
                onClick={() => setActiveMediaTab('images')}
                className={`flex-1 py-1.5 text-[14px] font-semibold rounded-lg transition-colors cursor-pointer ${activeMediaTab === 'images' ? 'bg-white shadow-sm text-[#222222]' : 'text-[#6a6a6a] hover:text-[#222222]'}`}
              >
                Images
              </button>
              <button 
                onClick={() => setActiveMediaTab('files')}
                className={`flex-1 py-1.5 text-[14px] font-semibold rounded-lg transition-colors cursor-pointer ${activeMediaTab === 'files' ? 'bg-white shadow-sm text-[#222222]' : 'text-[#6a6a6a] hover:text-[#222222]'}`}
              >
                Files
              </button>
            </div>
          </div>

          <div className="flex-1 overflow-y-auto custom-scrollbar p-6">
            {activeMediaTab === 'images' ? (
              <div className="grid grid-cols-3 gap-2">
                {[1, 2, 3, 4, 5, 6, 7, 8, 9].map(i => (
                  <div key={i} className="aspect-square bg-[#f7f7f7] rounded-lg border border-[#ebebeb] flex items-center justify-center cursor-pointer hover:opacity-80 transition-opacity">
                    <Icon icon="fluent:image-24-regular" className="size-6 text-[#dddddd]" />
                  </div>
                ))}
              </div>
            ) : (
              <div className="space-y-3">
                {[1, 2, 3].map(i => (
                  <div key={i} className="flex items-center gap-3 p-3 rounded-xl border border-[#ebebeb] cursor-pointer hover:bg-[#f7f7f7] transition-colors">
                    <div className="h-10 w-10 bg-[#f2f2f2] rounded-lg shrink-0 flex items-center justify-center">
                      <Icon icon="fluent:document-24-regular" className="size-5 text-[#222222]" />
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-[14px] font-medium text-[#222222] truncate">Document_{i}.pdf</p>
                      <p className="text-[12px] text-[#6a6a6a] mt-0.5">1.2 MB</p>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      )}

    </div>
  );
};
