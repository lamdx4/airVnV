import { api } from '@/lib/api';
import type { TicketsResponse, RefundsResponse, TicketDetail, TicketComment, RefundDetail } from '../types';

export interface GetTicketsParams {
  page?: number;
  pageSize?: number;
  status?: string;
  priority?: string;
  category?: string;
  assignedToId?: string;
  reporterId?: string;
  search?: string;
  fromDate?: string;
  toDate?: string;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

export interface GetRefundsParams {
  page?: number;
  pageSize?: number;
  status?: string;
  type?: string;
  guestId?: string;
  hostId?: string;
  bookingId?: string;
  fromDate?: string;
  toDate?: string;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

export interface AssignTicketRequest {
  ticketId: string;
  assignedToId: string;
}

export interface UpdateTicketStatusRequest {
  ticketId: string;
  status: string;
  resolution?: string;
}

export interface AddCommentRequest {
  ticketId: string;
  content: string;
  isInternal?: boolean;
}

export interface ProcessRefundRequest {
  refundId: string;
  action: 'approve' | 'reject' | 'process';
  rejectionReason?: string;
}

export const supportApi = {
  // Tickets
  getTickets: (params?: GetTicketsParams): Promise<TicketsResponse> => {
    return api.get('/api/admin/support/tickets', { params }) as Promise<TicketsResponse>;
  },

  getTicketById: (ticketId: string): Promise<TicketDetail> => {
    return api.get(`/api/admin/support/tickets/${ticketId}`) as Promise<TicketDetail>;
  },

  assignTicket: (data: AssignTicketRequest): Promise<TicketDetail> => {
    return api.post('/api/admin/support/tickets/assign', data) as Promise<TicketDetail>;
  },

  updateTicketStatus: (data: UpdateTicketStatusRequest): Promise<TicketDetail> => {
    return api.put('/api/admin/support/tickets/status', data) as Promise<TicketDetail>;
  },

  addComment: (data: AddCommentRequest): Promise<TicketComment> => {
    return api.post('/api/admin/support/tickets/comments', data) as Promise<TicketComment>;
  },

  // Refunds
  getRefunds: (params?: GetRefundsParams): Promise<RefundsResponse> => {
    return api.get('/api/admin/support/refunds', { params }) as Promise<RefundsResponse>;
  },

  processRefund: (data: ProcessRefundRequest): Promise<RefundDetail> => {
    return api.post('/api/admin/support/refunds/process', data) as Promise<RefundDetail>;
  },

  createRefund: (data: {
    bookingId: string;
    guestId: string;
    hostId: string;
    type: string;
    requestedAmount: number;
    reason: string;
    category: string;
    relatedTicketId?: string;
  }): Promise<RefundDetail> => {
    return api.post('/api/admin/support/refunds', data) as Promise<RefundDetail>;
  },
};
