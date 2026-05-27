// Ticket Types
export interface TicketSummary {
  id: string;
  subject: string;
  category: string;
  priority: string;
  status: string;
  reporterName: string;
  assignedToName: string | null;
  createdAt: string;
  commentCount: number;
}

export interface TicketComment {
  id: string;
  authorId: string;
  authorName: string;
  content: string;
  isInternal: boolean;
  createdAt: string;
}

export interface TicketDetail {
  id: string;
  subject: string;
  description: string;
  category: string;
  priority: string;
  status: string;
  bookingId: string | null;
  propertyId: string | null;
  reporterId: string;
  reporterEmail: string;
  reporterName: string;
  isReporterHost: boolean;
  assignedToId: string | null;
  assignedToName: string | null;
  assignedAt: string | null;
  resolution: string | null;
  createdAt: string;
  updatedAt: string;
  comments: TicketComment[];
  commentCount: number;
  attachmentCount: number;
}

export interface TicketStats {
  totalOpen: number;
  totalInProgress: number;
  totalResolved: number;
  totalEscalated: number;
  highPriorityCount: number;
  urgentCount: number;
}

export interface TicketsResponse {
  items: TicketSummary[];
  totalCount: number;
  page: number;
  pageSize: number;
  stats: TicketStats;
}

// Refund Types
export interface RefundDetail {
  id: string;
  bookingId: string;
  guestId: string;
  hostId: string;
  type: string;
  totalAmount: number;
  requestedAmount: number;
  currency: string;
  reason: string;
  category: string;
  status: string;
  transactionId: string | null;
  processedById: string | null;
  processedByName: string | null;
  processedAt: string | null;
  relatedTicketId: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface RefundStats {
  totalPendingAmount: number;
  totalPending: number;
  totalProcessing: number;
  totalCompleted: number;
  totalFailed: number;
}

export interface RefundsResponse {
  items: RefundDetail[];
  totalCount: number;
  page: number;
  pageSize: number;
  stats: RefundStats;
}
