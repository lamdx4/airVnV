import React, { useState } from 'react';
import { Button } from '../../../components/ui/button';
import { Textarea } from '../../../components/ui/textarea';

interface ChatInputProps {
  onSend: (content: string) => void;
  isLoading: boolean;
}

export const ChatInput: React.FC<ChatInputProps> = ({ onSend, isLoading }) => {
  const [content, setContent] = useState('');

  const handleSubmit = (e?: React.FormEvent) => {
    e?.preventDefault();
    if (!content.trim() || isLoading) return;
    
    onSend(content.trim());
    setContent('');
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSubmit();
    }
  };

  return (
    <div className="border-t border-gray-200 bg-white p-4">
      <form onSubmit={handleSubmit} className="flex flex-col gap-2 relative">
        <Textarea 
          value={content}
          onChange={(e) => setContent(e.target.value)}
          onKeyDown={handleKeyDown}
          placeholder="Nhập tin nhắn... (Nhấn Enter để gửi)"
          className="resize-none pr-20 bg-gray-50 focus-visible:ring-1 border-gray-200 focus-visible:ring-blue-500 rounded-xl"
          rows={2}
          maxLength={4000}
        />
        <Button 
          type="submit" 
          disabled={!content.trim() || isLoading}
          className="absolute bottom-2 right-2 rounded-lg bg-blue-600 hover:bg-blue-700"
          size="sm"
        >
          Gửi
        </Button>
      </form>
    </div>
  );
};
