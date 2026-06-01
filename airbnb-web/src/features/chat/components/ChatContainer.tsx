import React from 'react';
import { ConversationList } from './ConversationList';
import { ChatHeader } from './ChatHeader';
import { MessageList } from './MessageList';
import { MessageInput } from './MessageInput';
import { useChat } from '../context/ChatContext';
import { useChatHub } from '../hooks/useChatHub';
import { motion, AnimatePresence } from 'framer-motion';
import { Icon } from '@iconify/react';
import { useState, useCallback, useEffect, useRef } from 'react';
import { CallModal } from './CallModal';
import { useInbox } from '../hooks/useInbox';
import { useWebRTC } from '../hooks/useWebRTC';

export const ChatContainer: React.FC = () => {
  const { activeConversationId, isSidebarOpen } = useChat();
  const [isCallModalOpen, setIsCallModalOpen] = useState(false);
  const [isVideoCall, setIsVideoCall] = useState(false);
  const { data } = useInbox();

  const conversation = data?.pages
    .flatMap(page => page.items)
    .find(c => c.id === activeConversationId);
  
  // Initialize SignalR
  const connection = useChatHub(activeConversationId);
  const { callState, incomingCall, remoteStream, startCall, acceptCall, rejectCall, endCall } = useWebRTC(connection);
  
  const [isAnswering, setIsAnswering] = useState(false);
  
  // Tự động đóng modal nếu đầu dây bên kia cúp máy (state chuyển từ trạng thái khác về idle)
  const prevCallState = useRef(callState);
  useEffect(() => {
    if (callState === 'idle' && prevCallState.current !== 'idle' && isCallModalOpen) {
      setIsCallModalOpen(false);
    }
    prevCallState.current = callState;
  }, [callState, isCallModalOpen]);

  const handleStartCall = (video: boolean) => {
    setIsVideoCall(video);
    setIsAnswering(false);
    setIsCallModalOpen(true);
  };

  const handleCloseCallModal = useCallback(() => {
    setIsCallModalOpen(false);
    endCall();
  }, [endCall]);

  const handleAcceptCall = () => {
    setIsVideoCall(true); // Default to video call layout for now
    setIsAnswering(true);
    setIsCallModalOpen(true);
  };

  const handleDeclineCall = () => {
    rejectCall();
  };

  // Find info of the person calling us
  const incomingConversation = data?.pages
    .flatMap(page => page.items)
    .find(c => c.otherParticipantId === incomingCall?.callerId);

  const incomingName = incomingConversation?.otherParticipantName || "Someone";
  const incomingAvatar = incomingConversation?.otherParticipantAvatar;

  return (
    <div className="h-[100dvh] bg-[#f7f7f7] p-0 md:p-3">
      <div className="h-full md:rounded-[28px] border-x md:border border-[#ebebeb] bg-white overflow-hidden flex shadow-sm relative">
        {/* Sidebar - Desktop always visible, Mobile toggleable */}
        <div className={`
          ${isSidebarOpen ? 'flex' : 'hidden'} 
          md:flex shrink-0 h-full w-full md:w-[380px]
        `}>
          <ConversationList />
        </div>

        {/* Main Chat Area */}
        <div className={`
          flex-1 flex flex-col h-full bg-white relative
          ${!activeConversationId ? 'hidden md:flex' : 'flex'}
        `}>
          <AnimatePresence mode="wait">
            {activeConversationId ? (
              <motion.div 
                key={activeConversationId}
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                exit={{ opacity: 0 }}
                transition={{ duration: 0.18 }}
                className="flex-1 flex flex-col h-full"
              >
                <ChatHeader connection={connection} onStartCall={handleStartCall} />
                <MessageList connection={connection} activeConversationId={activeConversationId} />
                <MessageInput 
                  conversationId={activeConversationId} 
                  connection={connection}
                />
              </motion.div>
            ) : (
              <motion.div 
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                className="flex-1 flex flex-col items-center justify-center p-8 text-center space-y-4 bg-[#fcfcfc]"
              >
                <div className="p-8 bg-white rounded-full shadow-xs border border-[#ebebeb]">
                  <Icon icon="hugeicons:message-02" className="text-6xl text-[#b0b0b0]" />
                </div>
                <div className="space-y-2">
                  <h2 className="text-[18px] font-semibold text-[#222222] tracking-tight">Select a message</h2>
                  <p className="text-[14px] text-[#6a6a6a] max-w-xs leading-relaxed">
                    Choose a conversation from the list to start messaging with hosts or guests.
                  </p>
                </div>
              </motion.div>
            )}
          </AnimatePresence>
        </div>

        {/* Incoming Call Notification */}
        <AnimatePresence>
          {incomingCall && callState === 'ringing' && !isCallModalOpen && (
            <motion.div 
              initial={{ opacity: 0, y: -50 }}
              animate={{ opacity: 1, y: 20 }}
              exit={{ opacity: 0, y: -50 }}
              className="absolute top-4 left-1/2 -translate-x-1/2 z-[100] bg-[#1c1c1e] text-white p-4 pr-6 rounded-[32px] shadow-2xl flex items-center gap-4 border border-white/10"
            >
              {incomingAvatar ? (
                <img src={incomingAvatar} alt="avatar" className="w-12 h-12 rounded-full object-cover bg-white/10" />
              ) : (
                <div className="w-12 h-12 rounded-full bg-white/10 flex items-center justify-center">
                  <Icon icon="fluent:person-24-filled" className="text-white/60" />
                </div>
              )}
              <div className="flex flex-col mr-4">
                <span className="font-medium">{incomingName}</span>
                <span className="text-xs text-white/60">Incoming call...</span>
              </div>
              <button 
                onClick={handleDeclineCall}
                className="w-10 h-10 rounded-full bg-[#FF3B30] flex items-center justify-center hover:scale-105 transition-transform"
              >
                <Icon icon="fluent:call-end-24-filled" className="text-white text-xl" />
              </button>
              <button 
                onClick={handleAcceptCall}
                className="w-10 h-10 rounded-full bg-[#34C759] flex items-center justify-center hover:scale-105 transition-transform animate-pulse"
              >
                <Icon icon="fluent:call-24-filled" className="text-white text-xl" />
              </button>
            </motion.div>
          )}
        </AnimatePresence>

        {(conversation || incomingConversation) && (
          <CallModal 
            isOpen={isCallModalOpen} 
            onClose={handleCloseCallModal} 
            otherParticipantName={(conversation || incomingConversation)!.otherParticipantName} 
            otherParticipantAvatar={(conversation || incomingConversation)!.otherParticipantAvatar} 
            isVideoCall={isVideoCall} 
            connection={connection}
            remoteStream={remoteStream}
            onStreamReady={(stream) => {
              if (isAnswering) {
                acceptCall(stream);
              } else if (conversation) {
                startCall(conversation.otherParticipantId, stream);
              }
            }}
          />
        )}
      </div>
    </div>
  );
};
