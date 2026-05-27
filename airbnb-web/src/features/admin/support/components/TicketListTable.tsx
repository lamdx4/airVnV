import { useEffect, useState } from 'react';
import { useSupportTickets } from '../hooks/useSupportTickets';
import type { TicketSummary } from '../types';
import type { GetTicketsParams } from '../api/supportApi';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { formatDistanceToNow } from 'date-fns';

const statusColors: Record<string, string> = {
  Open: 'bg-blue-500',
  InProgress: 'bg-yellow-500',
  Resolved: 'bg-green-500',
  Closed: 'bg-gray-500',
  Escalated: 'bg-red-500',
};

const priorityColors: Record<string, string> = {
  Low: 'border-gray-400 text-gray-600',
  Medium: 'border-yellow-400 text-yellow-600',
  High: 'border-orange-400 text-orange-600',
  Urgent: 'border-red-500 text-red-600',
};

interface TicketListTableProps {
  onSelectTicket?: (ticket: TicketSummary) => void;
}

export function TicketListTable({ onSelectTicket }: TicketListTableProps) {
  const { data, loading, error, fetchTickets } = useSupportTickets();
  const [filters, setFilters] = useState<GetTicketsParams>({
    status: '',
    priority: '',
    search: '',
    sortBy: 'CreatedAt',
    sortOrder: 'desc',
    page: 1,
  });

  useEffect(() => {
    fetchTickets(filters);
  }, []);

  const handleFilterChange = <K extends keyof GetTicketsParams>(key: K, value: GetTicketsParams[K]) => {
    setFilters(prev => ({ ...prev, [key]: value }));
    fetchTickets({ ...filters, [key]: value });
  };

  const handlePageChange = (newPage: number) => {
    setFilters(prev => ({ ...prev, page: newPage }));
    fetchTickets({ ...filters, page: newPage });
  };

  if (error) {
    return (
      <Card>
        <CardContent className="p-6">
          <p className="text-red-500">Error: {error}</p>
        </CardContent>
      </Card>
    );
  }

  const stats = data?.stats;
  const tickets = data?.items || [];
  const totalCount = data?.totalCount || 0;

  return (
    <div className="space-y-4">
      {/* Stats Summary */}
      {stats && (
        <div className="grid grid-cols-6 gap-4">
          <StatCard label="Open" value={stats.totalOpen} color="text-blue-600" />
          <StatCard label="In Progress" value={stats.totalInProgress} color="text-yellow-600" />
          <StatCard label="Resolved" value={stats.totalResolved} color="text-green-600" />
          <StatCard label="Escalated" value={stats.totalEscalated} color="text-red-600" />
          <StatCard label="High Priority" value={stats.highPriorityCount} color="text-orange-600" />
          <StatCard label="Urgent" value={stats.urgentCount} color="text-red-600" />
        </div>
      )}

      {/* Filters */}
      <Card>
        <CardContent className="p-4">
          <div className="flex flex-wrap gap-4">
            <Input
              placeholder="Search tickets..."
              value={filters.search || ''}
              onChange={e => handleFilterChange('search', e.target.value)}
              className="w-64"
            />
            <Select value={filters.status || ''} onValueChange={v => handleFilterChange('status', v)}>
              <SelectTrigger className="w-40">
                <SelectValue placeholder="Status" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">All Status</SelectItem>
                <SelectItem value="Open">Open</SelectItem>
                <SelectItem value="InProgress">In Progress</SelectItem>
                <SelectItem value="Resolved">Resolved</SelectItem>
                <SelectItem value="Closed">Closed</SelectItem>
                <SelectItem value="Escalated">Escalated</SelectItem>
              </SelectContent>
            </Select>
            <Select value={filters.priority || ''} onValueChange={v => handleFilterChange('priority', v)}>
              <SelectTrigger className="w-40">
                <SelectValue placeholder="Priority" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">All Priority</SelectItem>
                <SelectItem value="Low">Low</SelectItem>
                <SelectItem value="Medium">Medium</SelectItem>
                <SelectItem value="High">High</SelectItem>
                <SelectItem value="Urgent">Urgent</SelectItem>
              </SelectContent>
            </Select>
            <Select value={filters.sortOrder || 'desc'} onValueChange={v => handleFilterChange('sortOrder', v as 'asc' | 'desc')}>
              <SelectTrigger className="w-32">
                <SelectValue placeholder="Order" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="desc">Newest First</SelectItem>
                <SelectItem value="asc">Oldest First</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </CardContent>
      </Card>

      {/* Table */}
      <Card>
        <CardHeader>
          <CardTitle>Support Tickets ({totalCount})</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Subject</TableHead>
                <TableHead>Category</TableHead>
                <TableHead>Priority</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Reporter</TableHead>
                <TableHead>Assigned To</TableHead>
                <TableHead>Created</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {loading ? (
                <TableRow>
                  <TableCell colSpan={8} className="text-center py-8">
                    Loading...
                  </TableCell>
                </TableRow>
              ) : tickets.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={8} className="text-center py-8 text-gray-500">
                    No tickets found
                  </TableCell>
                </TableRow>
              ) : (
                tickets.map(ticket => (
                  <TableRow key={ticket.id}>
                    <TableCell className="font-medium">
                      <div className="max-w-xs truncate">{ticket.subject}</div>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline">{ticket.category}</Badge>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline" className={priorityColors[ticket.priority]}>
                        {ticket.priority}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <div className={`w-2 h-2 rounded-full ${statusColors[ticket.status]}`} />
                        <span>{ticket.status}</span>
                      </div>
                    </TableCell>
                    <TableCell>{ticket.reporterName}</TableCell>
                    <TableCell>{ticket.assignedToName || '—'}</TableCell>
                    <TableCell>{formatDistanceToNow(new Date(ticket.createdAt), { addSuffix: true })}</TableCell>
                    <TableCell className="text-right">
                      <Button variant="ghost" size="sm" onClick={() => onSelectTicket?.(ticket)}>
                        View
                      </Button>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>

          {/* Pagination */}
          {totalCount > 0 && (
            <div className="flex justify-between items-center mt-4">
              <span className="text-sm text-gray-500">
                Showing {(filters.page! - 1) * 20 + 1} - {Math.min(filters.page! * 20, totalCount)} of {totalCount}
              </span>
              <div className="flex gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  disabled={filters.page === 1}
                  onClick={() => handlePageChange(filters.page! - 1)}
                >
                  Previous
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  disabled={filters.page! * 20 >= totalCount}
                  onClick={() => handlePageChange(filters.page! + 1)}
                >
                  Next
                </Button>
              </div>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}

function StatCard({ label, value, color }: { label: string; value: number; color: string }) {
  return (
    <Card>
      <CardContent className="p-4 text-center">
        <div className={`text-2xl font-bold ${color}`}>{value}</div>
        <div className="text-sm text-gray-500">{label}</div>
      </CardContent>
    </Card>
  );
}
