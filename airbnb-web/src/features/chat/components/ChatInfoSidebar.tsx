import React, { useState, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { useChat } from "../context/ChatContext";
import { useInbox } from "../hooks/useInbox";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Icon } from "@iconify/react";
import { useAttachments } from "../hooks/useAttachments";
import { PhotoProvider, PhotoView } from "react-photo-view";
import { Loading03Icon } from "@/components/common/Icons";
import { toast } from "sonner";

type SidebarView = "main" | "media";

export const ChatInfoSidebar: React.FC = () => {
  const { isInfoSidebarOpen, toggleInfoSidebar, activeConversationId } =
    useChat();
  const { data } = useInbox();
  const navigate = useNavigate();
  const [currentView, setCurrentView] = useState<SidebarView>("main");
  const [activeMediaTab, setActiveMediaTab] = useState<"images" | "files">(
    "images",
  );

  const handleFileClick = (e: React.MouseEvent<HTMLAnchorElement>, url: string) => {
    if (!url || url === '#' || (!url.startsWith('http') && !url.startsWith('blob:'))) {
      e.preventDefault();
      toast.error('This file link is corrupted or unavailable.');
    }
  };

  // Reset view when sidebar is closed
  React.useEffect(() => {
    if (!isInfoSidebarOpen) {
      setCurrentView("main");
      setActiveMediaTab("images");
    }
  }, [isInfoSidebarOpen]);

  const typeParam = activeMediaTab === "images" ? "Image" : "File";
  const {
    data: attachmentsData,
    isLoading: isLoadingAttachments,
    hasNextPage,
    fetchNextPage,
    isFetchingNextPage,
  } = useAttachments(
    isInfoSidebarOpen ? activeConversationId : null,
    typeParam,
  );

  const attachments = attachmentsData?.pages.flatMap((p) => p.items) || [];

  const groupedAttachments = useMemo(() => {
    const groups: { monthYear: string; items: typeof attachments }[] = [];
    attachments.forEach((att) => {
      const date = new Date(att.createdAt);
      const monthYear = date.toLocaleString("en-US", {
        month: "long",
        year: "numeric",
      }); // e.g. "June 2026"

      const lastGroup = groups[groups.length - 1];
      if (lastGroup && lastGroup.monthYear === monthYear) {
        lastGroup.items.push(att);
      } else {
        groups.push({ monthYear, items: [att] });
      }
    });
    return groups;
  }, [attachments]);

  if (!isInfoSidebarOpen || !activeConversationId) return null;

  const conversation = data?.pages
    .flatMap((page) => page.items)
    .find((c) => c.id === activeConversationId);

  if (!conversation) return null;

  return (
    <div className="w-full md:w-[380px] bg-white border-l border-[#ebebeb] flex flex-col h-full shrink-0 animate-in slide-in-from-right-10 duration-200 absolute md:relative z-20 right-0">
      {currentView === "main" && (
        <>
          <div className="h-20 px-6 border-b border-[#ebebeb] flex items-center justify-between shrink-0">
            <h2 className="text-[16px] font-semibold text-[#222222]">
              Details
            </h2>
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
                <AvatarImage src={conversation.otherParticipantAvatar || ""} />
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
                <h4 className="text-[15px] font-semibold text-[#222222]">
                  Listing
                </h4>
                <div 
                  onClick={() => navigate(`/properties/${conversation.propertyId}`)}
                  className="flex items-center gap-3 p-3 rounded-xl border border-[#ebebeb] hover:shadow-sm transition-shadow cursor-pointer"
                >
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

            <div className="space-y-3">
              <h4 className="text-[15px] font-semibold text-[#222222]">
                Files and Images
              </h4>
              <div className="flex flex-col gap-1">
              <button
                onClick={() => {
                  setCurrentView("media");
                  setActiveMediaTab("images");
                }}
                className="w-full flex items-center justify-between p-3 rounded-xl hover:bg-[#f7f7f7] transition-colors text-left group cursor-pointer"
              >
                <div className="flex items-center gap-3">
                  <div className="h-10 w-10 bg-[#f2f2f2] rounded-full flex items-center justify-center group-hover:bg-white transition-colors">
                    <Icon
                      icon="fluent:image-24-regular"
                      className="size-5 text-[#222222]"
                    />
                  </div>
                  <span className="text-[15px] font-semibold text-[#222222]">
                    Images
                  </span>
                </div>
                <Icon
                  icon="lucide:chevron-right"
                  className="size-5 text-[#b0b0b0]"
                />
              </button>

              <button
                onClick={() => {
                  setCurrentView("media");
                  setActiveMediaTab("files");
                }}
                className="w-full flex items-center justify-between p-3 rounded-xl hover:bg-[#f7f7f7] transition-colors text-left group cursor-pointer"
              >
                <div className="flex items-center gap-3">
                  <div className="h-10 w-10 bg-[#f2f2f2] rounded-full flex items-center justify-center group-hover:bg-white transition-colors">
                    <Icon
                      icon="fluent:document-24-regular"
                      className="size-5 text-[#222222]"
                    />
                  </div>
                  <span className="text-[15px] font-semibold text-[#222222]">
                    Files
                  </span>
                </div>
                <Icon
                  icon="lucide:chevron-right"
                  className="size-5 text-[#b0b0b0]"
                />
              </button>
            </div>
            </div>
          </div>
        </>
      )}

      {currentView === "media" && (
        <div className="flex flex-col h-full">
          <div className="h-20 px-6 border-b border-[#ebebeb] flex items-center shrink-0 gap-4">
            <button
              onClick={() => setCurrentView("main")}
              className="p-2 -ml-2 rounded-full hover:bg-[#f7f7f7] transition-colors cursor-pointer"
              aria-label="Back"
            >
              <Icon
                icon="lucide:chevron-left"
                className="size-5 text-[#222222]"
              />
            </button>
            <h2 className="text-[16px] font-semibold text-[#222222]">
              Shared Media
            </h2>
          </div>

          {/* Tabs */}
          <div className="px-6 pt-5 shrink-0">
            <div className="flex bg-[#f7f7f7] p-1 rounded-xl">
              <button
                onClick={() => setActiveMediaTab("images")}
                className={`flex-1 py-1.5 text-[14px] font-semibold rounded-lg transition-colors cursor-pointer ${activeMediaTab === "images" ? "bg-white shadow-sm text-[#222222]" : "text-[#6a6a6a] hover:text-[#222222]"}`}
              >
                Images
              </button>
              <button
                onClick={() => setActiveMediaTab("files")}
                className={`flex-1 py-1.5 text-[14px] font-semibold rounded-lg transition-colors cursor-pointer ${activeMediaTab === "files" ? "bg-white shadow-sm text-[#222222]" : "text-[#6a6a6a] hover:text-[#222222]"}`}
              >
                Files
              </button>
            </div>
          </div>

          <div className="flex-1 overflow-y-auto custom-scrollbar p-6">
            {isLoadingAttachments ? (
              <div className="flex justify-center mt-10">
                <Loading03Icon className="h-6 w-6 animate-spin text-[#b0b0b0]" />
              </div>
            ) : attachments.length === 0 ? (
              <div className="text-center mt-10">
                <p className="text-[14px] text-[#6a6a6a]">
                  No {activeMediaTab} shared yet.
                </p>
              </div>
            ) : activeMediaTab === "images" ? (
              <PhotoProvider>
                <div className="space-y-6">
                  {groupedAttachments.map((group) => (
                    <div key={group.monthYear}>
                      <h3 className="text-[14px] font-semibold text-[#222222] mb-3">
                        {group.monthYear}
                      </h3>
                      <div className="grid grid-cols-3 gap-2">
                        {group.items.map((att) => (
                          <PhotoView key={att.messageId} src={att.content}>
                            <div className="aspect-square bg-[#f7f7f7] rounded-lg border border-[#ebebeb] flex items-center justify-center cursor-pointer hover:opacity-80 transition-opacity overflow-hidden relative">
                              <img
                                src={att.content}
                                alt="Shared Image"
                                className="w-full h-full object-cover"
                              />
                            </div>
                          </PhotoView>
                        ))}
                      </div>
                    </div>
                  ))}
                </div>
              </PhotoProvider>
            ) : (
              <div className="space-y-6">
                {groupedAttachments.map((group) => (
                  <div key={group.monthYear}>
                    <h3 className="text-[14px] font-semibold text-[#222222] mb-3">
                      {group.monthYear}
                    </h3>
                    <div className="space-y-3">
                      {group.items.map((att) => {
                        let fileData = { url: "", name: "Attachment", size: 0 };
                        try {
                          fileData = JSON.parse(att.content);
                        } catch (e) {}
                        return (
                          <a
                            key={att.messageId}
                            href={fileData.url || '#'}
                            onClick={(e) => handleFileClick(e, fileData.url)}
                            target="_blank"
                            rel="noopener noreferrer"
                            className="flex items-center gap-3 p-3 rounded-xl border border-[#ebebeb] cursor-pointer hover:bg-[#f7f7f7] transition-colors"
                          >
                            <div className="h-10 w-10 bg-[#f2f2f2] rounded-lg shrink-0 flex items-center justify-center">
                              <Icon
                                icon="fluent:document-24-regular"
                                className="size-5 text-[#222222]"
                              />
                            </div>
                            <div className="flex-1 min-w-0">
                              <p className="text-[14px] font-medium text-[#222222] truncate">
                                {fileData.name}
                              </p>
                              {fileData.size > 0 && (
                                <p className="text-[12px] text-[#6a6a6a] mt-0.5">
                                  {fileData.size / 1024 / 1024 >= 1
                                    ? `${(fileData.size / 1024 / 1024).toFixed(1)} MB`
                                    : `${(fileData.size / 1024).toFixed(0)} KB`}
                                </p>
                              )}
                            </div>
                          </a>
                        );
                      })}
                    </div>
                  </div>
                ))}
              </div>
            )}

            {hasNextPage && (
              <div className="flex justify-center mt-6 mb-2">
                <button
                  onClick={() => fetchNextPage()}
                  disabled={isFetchingNextPage}
                  className="text-[12px] font-semibold text-[#6a6a6a] hover:text-[#222222] transition-colors underline cursor-pointer"
                >
                  {isFetchingNextPage ? "Loading..." : "Load more"}
                </button>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
};
