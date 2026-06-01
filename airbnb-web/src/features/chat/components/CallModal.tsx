import React, { useState, useEffect, useRef } from "react";
import { motion, AnimatePresence, useDragControls } from "framer-motion";
import { Icon } from "@iconify/react";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import type * as signalR from "@microsoft/signalr";

interface CallModalProps {
  isOpen: boolean;
  onClose: () => void;
  otherParticipantName: string;
  otherParticipantAvatar?: string;
  isVideoCall: boolean;
  connection?: signalR.HubConnection | null;
  onStreamReady?: (stream: MediaStream) => void;
  remoteStream?: MediaStream | null;
}

const CallModalComponent: React.FC<CallModalProps> = ({
  isOpen,
  onClose,
  otherParticipantName,
  otherParticipantAvatar,
  isVideoCall,
  onStreamReady,
  remoteStream,
}) => {
  const [isMuted, setIsMuted] = useState(false);
  const [isVideoOff, setIsVideoOff] = useState(!isVideoCall);
  const dragControls = useDragControls();

  const [localStream, setLocalStream] = useState<MediaStream | null>(null);
  const localVideoRef = useRef<HTMLVideoElement>(null);

  // Lấy Mic / Camera khi mở Modal
  useEffect(() => {
    let stream: MediaStream | null = null;

    if (isOpen) {
      navigator.mediaDevices
        .getUserMedia({
          audio: true,
          video: isVideoCall,
        })
        .then((s) => {
          stream = s;
          setLocalStream(s);
          if (localVideoRef.current) {
            localVideoRef.current.srcObject = s;
          }
          if (onStreamReady) {
            onStreamReady(s);
          }
        })
        .catch((err) => {
          console.error("Lỗi lấy quyền truy cập Camera/Mic:", err);
        });
    }

    return () => {
      // Dọn dẹp stream khi đóng Modal
      if (stream) {
        stream.getTracks().forEach((track) => track.stop());
      }
      setLocalStream(null);
    };
  }, [isOpen, isVideoCall]);

  // Xử lý Tắt/Mở Mic
  const toggleMic = () => {
    setIsMuted((prev) => {
      const next = !prev;
      if (localStream) {
        localStream.getAudioTracks().forEach((track) => {
          track.enabled = !next;
        });
      }
      return next;
    });
  };

  // Xử lý Tắt/Mở Camera
  const toggleVideo = async () => {
    try {
      if (isVideoOff) {
        // Muốn BẬT camera
        if (localStream && localStream.getVideoTracks().length === 0) {
          // Nếu stream hiện tại chưa có track video (do ban đầu gọi audio), xin cấp quyền
          const videoStream = await navigator.mediaDevices.getUserMedia({ video: true });
          const videoTrack = videoStream.getVideoTracks()[0];
          localStream.addTrack(videoTrack);
          
          // Gán lại srcObject để đảm bảo video element cập nhật
          if (localVideoRef.current) {
            localVideoRef.current.srcObject = null;
            localVideoRef.current.srcObject = localStream;
          }
        } else if (localStream) {
          // Nếu đã có track video, chỉ cần bật lại
          localStream.getVideoTracks().forEach((track) => {
            track.enabled = true;
          });
        }
        setIsVideoOff(false);
      } else {
        // Muốn TẮT camera
        if (localStream) {
          localStream.getVideoTracks().forEach((track) => {
            track.enabled = false;
          });
        }
        setIsVideoOff(true);
      }
    } catch (err) {
      console.error("Lỗi khi bật Camera:", err);
    }
  };

  return (
    <AnimatePresence>
      {isOpen && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          className="absolute inset-0 z-50 flex items-start justify-center pt-[100px] pointer-events-none rounded-[inherit]"
        >
          <motion.div
            initial={{ scale: 0.95, opacity: 0, y: 20 }}
            animate={{ scale: 1, opacity: 1, y: 0 }}
            exit={{ scale: 0.95, opacity: 0, y: 20 }}
            transition={{ type: "spring", stiffness: 300, damping: 30 }}
            className="pointer-events-auto relative w-[90%] max-w-[720px] aspect-video bg-[#1c1c1e] rounded-[32px] overflow-hidden shadow-2xl flex flex-col border border-white/10"
            drag
            dragControls={dragControls}
            dragListener={false}
            dragMomentum={false}
            style={{ touchAction: "none" }}
          >
            {/* Drag Handle Overlay */}
            <div 
              className="absolute top-0 left-0 right-0 h-28 z-30 cursor-grab active:cursor-grabbing" 
              onPointerDown={(e) => dragControls.start(e)}
              style={{ touchAction: "none" }}
            />

                {/* Header Info */}
                <div className="absolute top-0 left-0 right-0 pt-10 pb-4 flex flex-col items-center z-20 pointer-events-none">
                  <h2 className="text-white text-[22px] font-medium tracking-wide">
                    {otherParticipantName}
                  </h2>
                  <p className="text-white/60 text-sm mt-1.5 font-normal tracking-wide animate-pulse">
                    Calling...
                  </p>
                </div>

                {/* Center Avatar / Video Area */}
                <div className="flex-1 flex items-center justify-center relative overflow-hidden rounded-[32px]">
                  {remoteStream ? (
                    <video
                      autoPlay
                      playsInline
                      className="w-full h-full object-cover"
                      ref={(ref) => { if (ref) ref.srcObject = remoteStream; }}
                    />
                  ) : (
                    <>
                      {/* Thêm hiệu ứng pulse vòng tròn tỏa ra xung quanh avatar để diễn tả trạng thái đang gọi */}
                      <div className="absolute w-[140px] h-[140px] bg-white/5 rounded-full animate-[ping_2s_cubic-bezier(0,0,0.2,1)_infinite]" />
                      <div
                        className="absolute w-[180px] h-[180px] bg-white/5 rounded-full animate-[ping_2.5s_cubic-bezier(0,0,0.2,1)_infinite]"
                        style={{ animationDelay: "0.5s" }}
                      />

                      <Avatar className="w-24 h-24 border-[3px] border-[#2c2c2e] z-10 shadow-2xl">
                        <AvatarImage
                          src={otherParticipantAvatar || ""}
                          className="object-cover"
                        />
                        <AvatarFallback className="bg-[#3c3c3e] text-white text-3xl font-semibold">
                          {otherParticipantName.charAt(0)}
                        </AvatarFallback>
                      </Avatar>
                    </>
                  )}
                </div>

                {/* Local Video Stream (PiP) */}
                <div 
                  className={`absolute bottom-28 right-6 w-[100px] h-[150px] bg-[#2c2c2e] rounded-xl overflow-hidden shadow-xl border border-white/10 z-20 transition-all duration-300 ${
                    !isVideoOff && localStream ? 'opacity-100 scale-100' : 'opacity-0 scale-90 pointer-events-none'
                  }`}
                >
                  <video
                    ref={localVideoRef}
                    autoPlay
                    playsInline
                    muted // Local video must be muted to prevent echo
                    className="w-full h-full object-cover transform -scale-x-100" // Mirror local video
                  />
                  {isMuted && (
                    <div className="absolute bottom-2 right-2 bg-black/60 p-1.5 rounded-full backdrop-blur-md">
                      <Icon icon="fluent:mic-off-16-filled" className="text-white text-xs" />
                    </div>
                  )}
                </div>

                {/* Action Buttons (Bottom) */}
                <div className="absolute bottom-10 left-0 right-0 flex justify-center items-center gap-6 z-10">
                  {/* Camera Toggle */}
                  <button
                    onClick={toggleVideo}
                    className={`w-[52px] h-[52px] rounded-full flex items-center justify-center transition-all duration-200 ${
                      isVideoOff
                        ? "bg-white text-[#1c1c1e] scale-100"
                        : "bg-white/15 text-white hover:bg-white/25 backdrop-blur-md"
                    }`}
                  >
                    <Icon
                      icon={
                        isVideoOff
                          ? "fluent:video-off-24-filled"
                          : "fluent:video-24-filled"
                      }
                      className="w-[26px] h-[26px]"
                    />
                  </button>

                  {/* End Call (Red) */}
                  <button
                    onClick={onClose}
                    className="w-16 h-16 rounded-full flex items-center justify-center bg-[#FF3B30] hover:bg-[#ff3b30]/80 hover:scale-105 active:scale-95 transition-all duration-200 shadow-[0_0_20px_rgba(255,59,48,0.3)]"
                  >
                    <Icon
                      icon="fluent:call-end-24-filled"
                      className="w-[32px] h-[32px] text-white"
                    />
                  </button>

                  {/* Mic Toggle */}
                  <button
                    onClick={toggleMic}
                    className={`w-[52px] h-[52px] rounded-full flex items-center justify-center transition-all duration-200 ${
                      isMuted
                        ? "bg-white text-[#1c1c1e] scale-100"
                        : "bg-white/15 text-white hover:bg-white/25 backdrop-blur-md"
                    }`}
                  >
                    <Icon
                      icon={
                        isMuted
                          ? "fluent:mic-off-24-filled"
                          : "fluent:mic-24-filled"
                      }
                      className="w-[26px] h-[26px]"
                    />
                  </button>
                </div>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  );
};

export const CallModal = React.memo(
  CallModalComponent,
  (prevProps, nextProps) => {
    return (
      prevProps.isOpen === nextProps.isOpen &&
      prevProps.onClose === nextProps.onClose &&
      prevProps.isVideoCall === nextProps.isVideoCall &&
      prevProps.connection === nextProps.connection &&
      prevProps.remoteStream === nextProps.remoteStream
    );
  },
);
