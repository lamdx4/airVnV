import { useState } from 'react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';

interface SuspendUserModalProps {
  isOpen: boolean;
  userName: string;
  onClose: () => void;
  onConfirm: (reason: string, durationDays?: number) => void;
  isLoading?: boolean;
}

export function SuspendUserModal({
  isOpen,
  userName,
  onClose,
  onConfirm,
  isLoading,
}: SuspendUserModalProps) {
  const [reason, setReason] = useState('');
  const [durationDays, setDurationDays] = useState<string>('30');

  const handleConfirm = () => {
    const days = durationDays ? parseInt(durationDays, 10) : undefined;
    onConfirm(reason, days);
  };

  return (
    <Dialog open={isOpen} onOpenChange={(open) => !open && onClose()}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Suspend User Account</DialogTitle>
          <DialogDescription>
            You are about to suspend <strong>{userName}</strong>'s account. 
            This action will prevent them from accessing the platform.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4 py-4">
          <div className="space-y-2">
            <Label htmlFor="reason">Reason for suspension *</Label>
            <Input
              id="reason"
              placeholder="Enter reason (e.g., violation of terms)"
              value={reason}
              onChange={(e) => setReason(e.target.value)}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="duration">Duration (days)</Label>
            <Input
              id="duration"
              type="number"
              placeholder="30"
              value={durationDays}
              onChange={(e) => setDurationDays(e.target.value)}
              min="1"
            />
            <p className="text-xs text-muted-foreground">
              Leave empty for permanent suspension. Enter a number for temporary suspension.
            </p>
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={onClose} disabled={isLoading}>
            Cancel
          </Button>
          <Button
            variant="destructive"
            onClick={handleConfirm}
            disabled={isLoading || !reason.trim()}
          >
            {isLoading ? 'Suspending...' : 'Suspend User'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}