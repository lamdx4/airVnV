import React, { useState } from 'react';
import { useSendMessage } from '../hooks/useSendMessage';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { Smile, SendHorizontal, Image, Paperclip, Mic } from 'lucide-react';
import { Loading03Icon } from '@/components/common/Icons';

interface MessageInputProps {
  conversationId: string;
}

export const MessageInput: React.FC<MessageInputProps> = ({ conversationId }) => {
  const [content, setContent] = useState('');
  const { mutate: sendMessage, isPending } = useSendMessage(conversationId);

  const handleSend = () => {
    if (!content.trim() || isPending) return;
    sendMessage(content.trim());
    setContent('');
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
        <Button 
          variant="ghost" 
          size="icon" 
          aria-label="Add emoji"
          className="h-10 w-10 rounded-full text-[#6a6a6a] hover:text-[#222222] hover:bg-[#f7f7f7] transition-colors shrink-0"
        >
          <Smile className="h-5 w-5" />
        </Button>

        {/* Ô nhập tin nhắn */}
        <div className="relative flex-1 py-1">
          <Textarea
            value={content}
            onChange={(e) => setContent(e.target.value)}
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
            disabled={!content.trim() || isPending}
            variant="ghost"
            size="icon"
            aria-label="Send message"
            className="h-10 w-10 rounded-full text-[#6a6a6a] hover:text-[#222222] hover:bg-[#f7f7f7] transition-colors disabled:opacity-50"
          >
            {isPending ? (
              <Loading03Icon className="h-5 w-5 animate-spin" />
            ) : (
              <SendHorizontal className="h-5 w-5" />
            )}
          </Button>

          <Button 
            variant="ghost" 
            size="icon" 
            aria-label="Add image"
            className="h-10 w-10 rounded-full text-[#6a6a6a] hover:text-[#222222] hover:bg-[#f7f7f7] transition-colors"
          >
            <Image className="h-5 w-5" />
          </Button>

          <Button 
            variant="ghost" 
            size="icon" 
            aria-label="Add attachment"
            className="h-10 w-10 rounded-full text-[#6a6a6a] hover:text-[#222222] hover:bg-[#f7f7f7] transition-colors"
          >
            <Paperclip className="h-5 w-5" />
          </Button>

          <Button 
            variant="ghost" 
            size="icon" 
            aria-label="Voice message"
            className="h-10 w-10 rounded-full text-[#6a6a6a] hover:text-[#222222] hover:bg-[#f7f7f7] transition-colors"
          >
            <Mic className="h-5 w-5" />
          </Button>
        </div>
      </div>
      
      <p className="text-[11px] text-center text-[#b0b0b0] mt-3 font-normal hidden md:block">
        Press Enter to send, Shift + Enter for new line
      </p>
    </div>
  );
};
