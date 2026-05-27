import { useState } from 'react';
import { supportApi, type GetTicketsParams } from '../api/supportApi';
import type { TicketsResponse, TicketDetail } from '../types';

export function useSupportTickets() {
  const [data, setData] = useState<TicketsResponse | null>(null);
  const [selectedTicket, setSelectedTicket] = useState<TicketDetail | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchTickets = async (params?: GetTicketsParams) => {
    try {
      setLoading(true);
      setError(null);
      const result = await supportApi.getTickets(params);
      setData(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch tickets');
    } finally {
      setLoading(false);
    }
  };

  const fetchTicketById = async (ticketId: string) => {
    try {
      setLoading(true);
      setError(null);
      const result = await supportApi.getTicketById(ticketId);
      setSelectedTicket(result);
      return result;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch ticket');
      return null;
    } finally {
      setLoading(false);
    }
  };

  const assignTicket = async (ticketId: string, assignedToId: string) => {
    const result = await supportApi.assignTicket({ ticketId, assignedToId });
    setSelectedTicket(result);
    return result;
  };

  const updateStatus = async (ticketId: string, status: string, resolution?: string) => {
    const result = await supportApi.updateTicketStatus({ ticketId, status, resolution });
    setSelectedTicket(result);
    return result;
  };

  const addComment = async (ticketId: string, content: string, isInternal = false) => {
    return supportApi.addComment({ ticketId, content, isInternal });
  };

  return {
    data,
    selectedTicket,
    loading,
    error,
    fetchTickets,
    fetchTicketById,
    assignTicket,
    updateStatus,
    addComment,
    setSelectedTicket,
  };
}
