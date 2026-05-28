import { useEffect } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import * as signalR from '@microsoft/signalr';
import { chatApi } from '../api/chatApi';

export const usePresence = (userId?: string | null, connection?: signalR.HubConnection | null) => {
  const queryClient = useQueryClient();

  useEffect(() => {
    if (!connection) return;

    const handleUserStatusChanged = (changedUserId: string, status: string) => {
      queryClient.setQueryData(['presence', changedUserId?.toLowerCase()], { isOnline: status === 'online' });
    };

    connection.on('UserStatusChanged', handleUserStatusChanged);

    return () => {
      connection.off('UserStatusChanged', handleUserStatusChanged);
    };
  }, [connection, queryClient]);

  return useQuery({
    queryKey: ['presence', userId?.toLowerCase()],
    queryFn: async () => {
      if (!userId) return { isOnline: false };
      const response = await chatApi.getUserStatus(userId);
      return response;
    },
    enabled: !!userId,
    staleTime: Infinity, // SignalR updates will manually mutate cache
  });
};
