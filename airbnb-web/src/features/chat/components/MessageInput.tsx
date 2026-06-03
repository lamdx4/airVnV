import React, { useState, useRef, useEffect } from 'react';
import { useSendMessage } from '../hooks/useSendMessage';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { Loading03Icon } from '@/components/common/Icons';
import { Icon } from '@iconify/react';
import type * as signalR from '@microsoft/signalr';
import { useTypingPublisher } from '../hooks/useTypingStatus';
import EmojiPicker from 'emoji-picker-react';
import { toast } from 'sonner';

const MAX_IMAGE_SIZE = 10 * 1024 * 1024; // 10MB
const MAX_FILE_SIZE = 20 * 1024 * 1024; // 20MB
interface MessageInputProps {
  conversationId: string;
  connection: signalR.HubConnection | null;
}

export const MessageInput: React.FC<MessageInputProps> = ({ conversationId, connection }) => {
  const { handleTyping, stopTyping } = useTypingPublisher(connection, conversationId);
  const [content, setContent] = useState('');
  const [showEmojiPicker, setShowEmojiPicker] = useState(false);
  const [isUploadingImage, setIsUploadingImage] = useState(false);
  const emojiPickerRef = useRef<HTMLDivElement>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const documentInputRef = useRef<HTMLInputElement>(null);
  const { mutate: sendMessage, isPending } = useSendMessage(conversationId);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (emojiPickerRef.current && !emojiPickerRef.current.contains(event.target as Node)) {
        setShowEmojiPicker(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, []);

  const handleEmojiClick = (emojiObject: any) => {
    setContent(prev => prev + emojiObject.emoji);
  };

  const handleSend = () => {
    if (!content.trim() || isPending || isUploadingImage) return;
    sendMessage({ content: content.trim() });
    setContent('');
    stopTyping?.();
  };

  const handleImageUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    if (file.size > MAX_IMAGE_SIZE) {
      toast.error('Image size exceeds 5MB limit.');
      if (fileInputRef.current) fileInputRef.current.value = '';
      return;
    }

    // Create a local blob URL for optimistic UI
    const blobUrl = URL.createObjectURL(file);
    
    // Optimistic send (upload happens inside mutation)
    sendMessage({ 
      content: blobUrl, 
      messageType: 'Image',
      file 
    });

    if (fileInputRef.current) {
      fileInputRef.current.value = ''; // Reset input
    }
  };

  const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    if (file.size > MAX_FILE_SIZE) {
      toast.error('File size exceeds 20MB limit.');
      if (documentInputRef.current) documentInputRef.current.value = '';
      return;
    }

    const blobUrl = URL.createObjectURL(file);
    
    // Lưu tạm dưới dạng JSON cho phần hiển thị optimistic UI
    const optimisticData = JSON.stringify({
      url: blobUrl,
      name: file.name,
      size: file.size
    });

    sendMessage({ 
      content: optimisticData, 
      messageType: 'File',
      file 
    });

    if (documentInputRef.current) {
      documentInputRef.current.value = ''; // Reset input
    }
  };


  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  return (
    <div className="p-5 border-t border-[#ebebeb] bg-white">
      <div className="flex items-center gap-3 max-w-5xl mx-auto rounded-[24px] border border-[#dddddd] bg-white px-4 py-2 focus-within:border-[#222222] transition-all">
        {/* Nút Emoji phía bên trái */}
        <div className="relative" ref={emojiPickerRef}>
          <Button 
            variant="ghost" 
            size="icon" 
            onClick={() => setShowEmojiPicker(!showEmojiPicker)}
            aria-label="Add emoji"
            className="h-10 w-10 rounded-full text-[#25D366] hover:bg-[#25D366]/10 transition-colors shrink-0"
          >
            <Icon icon="fluent:emoji-24-filled" className="size-5" />
          </Button>

          {showEmojiPicker && (
            <div className="absolute bottom-12 left-0 z-50">
              <EmojiPicker onEmojiClick={handleEmojiClick} />
            </div>
          )}
        </div>

        {/* Ô nhập tin nhắn */}
        <div className="relative flex-1 py-1">
          <Textarea
            value={content}
            onChange={(e) => {
              setContent(e.target.value);
              handleTyping?.();
            }}
            onKeyDown={handleKeyDown}
            placeholder="Type a message..."
            className="min-h-[24px] h-[24px] max-h-32 border-none shadow-none focus-visible:ring-0 py-0 px-0 transition-all resize-none overflow-y-auto scrollbar-none text-[15px] text-[#222222] placeholder:text-[#9a9a9a]"
            rows={1}
            aria-label="Message content"
          />
        </div>

        {/* Các nút chức năng phía bên phải */}
        <div className="flex items-center gap-1 shrink-0">
          <Button
            onClick={handleSend}
            disabled={!content.trim() || isPending || isUploadingImage}
            variant="ghost"
            size="icon"
            aria-label="Send message"
            className="h-10 w-10 rounded-full text-[#25D366] hover:bg-[#25D366]/10 transition-colors disabled:opacity-50"
          >
            {isPending ? (
              <Loading03Icon className="h-6 w-6 animate-spin" />
            ) : (
              <Icon icon="fluent:send-24-filled" className="size-5" />
            )}
          </Button>

          <input 
            type="file" 
            ref={fileInputRef} 
            onChange={handleImageUpload} 
            accept="image/*" 
            className="hidden" 
          />
          <input 
            type="file" 
            ref={documentInputRef} 
            onChange={handleFileUpload} 
            className="hidden" 
          />
          <Button 
            variant="ghost" 
            size="icon" 
            aria-label="Add image"
            onClick={() => fileInputRef.current?.click()}
            disabled={isUploadingImage}
            className="h-10 w-10 rounded-full text-[#25D366] hover:bg-[#25D366]/10 transition-colors disabled:opacity-50"
          >
            {isUploadingImage ? (
              <Loading03Icon className="h-6 w-6 animate-spin" />
            ) : (
              <Icon icon="fluent:image-24-filled" className="size-5" />
            )}
          </Button>

          <Button 
            variant="ghost" 
            size="icon" 
            aria-label="Add attachment"
            onClick={() => documentInputRef.current?.click()}
            className="h-10 w-10 rounded-full text-[#25D366] hover:bg-[#25D366]/10 transition-colors"
          >
            <Icon icon="fluent:attach-24-filled" className="size-5" />
          </Button>

          <Button 
            variant="ghost" 
            size="icon" 
            aria-label="Voice message"
            className="h-10 w-10 rounded-full text-[#25D366] hover:bg-[#25D366]/10 transition-colors"
          >
            <Icon icon="fluent:mic-24-filled" className="size-5" />
          </Button>
        </div>
      </div>
      
      <p className="text-[11px] text-center text-[#b0b0b0] mt-3 font-normal hidden md:block">
        Press Enter to send, Shift + Enter for new line
      </p>
    </div>
  );
};
