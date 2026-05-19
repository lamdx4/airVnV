import { format, isToday, isYesterday } from 'date-fns';

export const formatChatDate = (date: Date): string => {
  if (isToday(date)) return 'Today';
  if (isYesterday(date)) return 'Yesterday';
  return format(date, 'EEEE, MMM d');
};

export const formatMessageTime = (date: Date): string => {
  return format(date, 'h:mm a');
};
