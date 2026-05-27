import { useEffect, useState } from 'react';
import { useSupportTickets } from '../hooks/useSupportTickets';
import type { TicketComment } from '../types';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from '@/components/ui/dialog';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { formatDistanceToNow, format } from 'date-fns';

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

interface TicketDetailPanelProps {
  ticketId: string;
  onClose: () => void;
}

export function TicketDetailPanel({ ticketId, onClose }: TicketDetailPanelProps) {
  const { selectedTicket, loading, error, fetchTicketById, updateStatus, addComment } = useSupportTickets();
  const [newComment, setNewComment] = useState('');
  const [isInternal, setIsInternal] = useState(false);
  const [saving, setSaving] = useState(false);
  const [statusDialog, setStatusDialog] = useState(false);
  const [newStatus, setNewStatus] = useState('');
  const [resolution, setResolution] = useState('');

  useEffect(() => {
    fetchTicketById(ticketId);
  }, [ticketId]);

  const handleAddComment = async () => {
    if (!newComment.trim()) return;
    setSaving(true);
    try {
      await addComment(ticketId, newComment, isInternal);
      setNewComment('');
      fetchTicketById(ticketId);
    } finally {
      setSaving(false);
    }
  };

  const handleStatusChange = async () => {
    setSaving(true);
    try {
      await updateStatus(ticketId, newStatus, resolution || undefined);
      setStatusDialog(false);
      fetchTicketById(ticketId);
    } finally {
      setSaving(false);
    }
  };

  if (loading && !selectedTicket) {
    return (
      <Card>
        <CardContent className="p-6 text-center">Loading ticket details...</CardContent>
      </Card>
    );
  }

  if (error || !selectedTicket) {
    return (
      <Card>
        <CardContent className="p-6">
          <p className="text-red-500">{error || 'Ticket not found'}</p>
          <Button variant="outline" className="mt-4" onClick={onClose}>
            Close
          </Button>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="space-y-4">
      {/* Header */}
      <Card>
        <CardHeader className="flex flex-row items-start justify-between">
          <div className="space-y-1">
            <CardTitle className="text-lg">{selectedTicket.subject}</CardTitle>
            <p className="text-sm text-gray-500">
              Ticket #{selectedTicket.id.slice(0, 8)} • Created {formatDistanceToNow(new Date(selectedTicket.createdAt), { addSuffix: true })}
            </p>
          </div>
          <div className="flex gap-2">
            <Badge variant="outline" className={priorityColors[selectedTicket.priority]}>
              {selectedTicket.priority}
            </Badge>
            <div className="flex items-center gap-2 px-3 py-1 border rounded-md">
              <div className={`w-2 h-2 rounded-full ${statusColors[selectedTicket.status]}`} />
              <span>{selectedTicket.status}</span>
            </div>
          </div>
        </CardHeader>
      </Card>

      {/* Details */}
      <div className="grid grid-cols-3 gap-4">
        {/* Ticket Info */}
        <Card className="col-span-2">
          <CardHeader>
            <CardTitle>Description</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="whitespace-pre-wrap">{selectedTicket.description}</p>

            <div className="mt-6 grid grid-cols-2 gap-4 border-t pt-4">
              <div>
                <Label className="text-gray-500">Category</Label>
                <p className="mt-1 font-medium">{selectedTicket.category}</p>
              </div>
              {selectedTicket.bookingId && (
                <div>
                  <Label className="text-gray-500">Booking ID</Label>
                  <p className="mt-1 font-mono text-sm">{selectedTicket.bookingId}</p>
                </div>
              )}
              {selectedTicket.propertyId && (
                <div>
                  <Label className="text-gray-500">Property ID</Label>
                  <p className="mt-1 font-mono text-sm">{selectedTicket.propertyId}</p>
                </div>
              )}
            </div>
          </CardContent>
        </Card>

        {/* Reporter Info */}
        <Card>
          <CardHeader>
            <CardTitle>Reporter</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">
            <div>
              <Label className="text-gray-500">Name</Label>
              <p className="mt-1 font-medium">{selectedTicket.reporterName}</p>
            </div>
            <div>
              <Label className="text-gray-500">Email</Label>
              <p className="mt-1 text-sm">{selectedTicket.reporterEmail}</p>
            </div>
            <div>
              <Label className="text-gray-500">Role</Label>
              <p className="mt-1">
                <Badge variant={selectedTicket.isReporterHost ? 'default' : 'secondary'}>
                  {selectedTicket.isReporterHost ? 'Host' : 'Guest'}
                </Badge>
              </p>
            </div>
          </CardContent>
        </Card>

        {/* Assignment */}
        <Card>
          <CardHeader>
            <CardTitle>Assignment</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">
            <div>
              <Label className="text-gray-500">Assigned To</Label>
              <p className="mt-1 font-medium">{selectedTicket.assignedToName || 'Unassigned'}</p>
            </div>
            {selectedTicket.assignedAt && (
              <div>
                <Label className="text-gray-500">Assigned At</Label>
                <p className="mt-1 text-sm">{format(new Date(selectedTicket.assignedAt), 'PPpp')}</p>
              </div>
            )}
            <div>
              <Label className="text-gray-500">Last Updated</Label>
              <p className="mt-1 text-sm">{formatDistanceToNow(new Date(selectedTicket.updatedAt), { addSuffix: true })}</p>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Comments */}
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>Comments ({selectedTicket.comments.length})</CardTitle>
          <Button variant="outline" size="sm" onClick={() => setStatusDialog(true)}>
            Update Status
          </Button>
        </CardHeader>
        <CardContent className="space-y-4">
          {selectedTicket.comments.length === 0 ? (
            <p className="text-center text-gray-500 py-4">No comments yet</p>
          ) : (
            selectedTicket.comments.map(comment => (
              <CommentItem key={comment.id} comment={comment} />
            ))
          )}

          {/* Add Comment */}
          <div className="border-t pt-4 space-y-3">
            <Textarea
              placeholder="Add a comment..."
              value={newComment}
              onChange={e => setNewComment(e.target.value)}
              rows={3}
            />
            <div className="flex items-center justify-between">
              <label className="flex items-center gap-2 text-sm">
                <input
                  type="checkbox"
                  checked={isInternal}
                  onChange={e => setIsInternal(e.target.checked)}
                  className="rounded border-gray-400"
                />
                Internal note (hidden from reporter)
              </label>
              <Button onClick={handleAddComment} disabled={saving || !newComment.trim()}>
                {saving ? 'Posting...' : 'Add Comment'}
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Update Status Dialog */}
      <Dialog open={statusDialog} onOpenChange={setStatusDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Update Ticket Status</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div>
              <Label>New Status</Label>
              <Select value={newStatus} onValueChange={setNewStatus}>
                <SelectTrigger className="mt-1">
                  <SelectValue placeholder="Select status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Open">Open</SelectItem>
                  <SelectItem value="InProgress">In Progress</SelectItem>
                  <SelectItem value="Resolved">Resolved</SelectItem>
                  <SelectItem value="Closed">Closed</SelectItem>
                  <SelectItem value="Escalated">Escalated</SelectItem>
                </SelectContent>
              </Select>
            </div>
            {(newStatus === 'Resolved' || newStatus === 'Closed') && (
              <div>
                <Label>Resolution Notes</Label>
                <Textarea
                  className="mt-1"
                  rows={3}
                  value={resolution}
                  onChange={e => setResolution(e.target.value)}
                  placeholder="Describe how this was resolved..."
                />
              </div>
            )}
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setStatusDialog(false)}>
              Cancel
            </Button>
            <Button onClick={handleStatusChange} disabled={saving || !newStatus}>
              Update
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}

function CommentItem({ comment }: { comment: TicketComment }) {
  return (
    <div className={`border rounded-lg p-3 ${comment.isInternal ? 'bg-yellow-50 border-yellow-200' : 'bg-gray-50'}`}>
      <div className="flex items-center justify-between mb-2">
        <div className="flex items-center gap-2">
          <span className="font-medium text-sm">{comment.authorName}</span>
          {comment.isInternal && (
            <Badge variant="outline" className="text-xs bg-yellow-100 border-yellow-300">
              Internal
            </Badge>
          )}
        </div>
        <span className="text-xs text-gray-500">{formatDistanceToNow(new Date(comment.createdAt), { addSuffix: true })}</span>
      </div>
      <p className="text-sm whitespace-pre-wrap">{comment.content}</p>
    </div>
  );
}
