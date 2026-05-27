import { useState } from 'react';
import { TicketListTable } from '@/features/admin/support/components/TicketListTable';
import { TicketDetailPanel } from '@/features/admin/support/components/TicketDetailPanel';
import type { TicketSummary } from '@/features/admin/support/types';

export default function SupportTicketsPage() {
  const [selectedTicketId, setSelectedTicketId] = useState<string | null>(null);

  const handleSelectTicket = (ticket: TicketSummary) => {
    setSelectedTicketId(ticket.id);
  };

  const handleCloseDetail = () => {
    setSelectedTicketId(null);
  };

  return (
    <div className="container mx-auto py-6 space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-3xl font-bold">Support Tickets</h1>
      </div>

      {selectedTicketId ? (
        <div className="space-y-4">
          <button
            onClick={handleCloseDetail}
            className="text-blue-600 hover:underline flex items-center gap-1"
          >
            ← Back to list
          </button>
          <TicketDetailPanel ticketId={selectedTicketId} onClose={handleCloseDetail} />
        </div>
      ) : (
        <TicketListTable onSelectTicket={handleSelectTicket} />
      )}
    </div>
  );
}