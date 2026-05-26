import { useEffect, useState } from 'react';
import { usePlatformFee } from '../hooks/usePlatformFee';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Skeleton } from '@/components/ui/skeleton';
import { toast } from 'sonner';

export function PlatformFeeConfig() {
  const { config, loading, error, updating, fetchConfig, updateConfig } = usePlatformFee();
  const [hostFee, setHostFee] = useState('');
  const [guestFee, setGuestFee] = useState('');

  useEffect(() => {
    fetchConfig();
  }, []);

  useEffect(() => {
    if (config) {
      setHostFee(config.hostFeePercent.toString());
      setGuestFee(config.guestFeePercent.toString());
    }
  }, [config]);

  const handleUpdate = async () => {
    const hostFeeNum = parseFloat(hostFee);
    const guestFeeNum = parseFloat(guestFee);

    if (isNaN(hostFeeNum) || hostFeeNum < 0 || hostFeeNum > 100) {
      toast.error('Host fee must be between 0 and 100');
      return;
    }
    if (isNaN(guestFeeNum) || guestFeeNum < 0 || guestFeeNum > 100) {
      toast.error('Guest fee must be between 0 and 100');
      return;
    }

    try {
      await updateConfig(hostFeeNum, guestFeeNum);
      toast.success('Platform fee updated successfully');
    } catch {
      toast.error('Failed to update platform fee');
    }
  };

  if (error) {
    return (
      <Card className="p-6">
        <p className="text-red-500">Error: {error}</p>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Platform Fee Configuration</CardTitle>
        <CardDescription>
          Configure the platform fee percentage charged on transactions
        </CardDescription>
      </CardHeader>
      <CardContent className="space-y-6">
        {loading || !config ? (
          <div className="space-y-4">
            <Skeleton className="h-10 w-full" />
            <Skeleton className="h-10 w-full" />
            <Skeleton className="h-10 w-32" />
          </div>
        ) : (
          <>
            <div className="space-y-2">
              <Label htmlFor="hostFee">Host Fee (%)</Label>
              <p className="text-sm text-muted-foreground mb-2">
                Platform fee deducted from host payout
              </p>
              <div className="flex items-center gap-2">
                <Input
                  id="hostFee"
                  type="number"
                  min="0"
                  max="100"
                  step="0.1"
                  value={hostFee}
                  onChange={(e) => setHostFee(e.target.value)}
                  className="w-32"
                />
                <span className="text-muted-foreground">%</span>
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="guestFee">Guest Fee (%)</Label>
              <p className="text-sm text-muted-foreground mb-2">
                Additional service fee charged to guest (if any)
              </p>
              <div className="flex items-center gap-2">
                <Input
                  id="guestFee"
                  type="number"
                  min="0"
                  max="100"
                  step="0.1"
                  value={guestFee}
                  onChange={(e) => setGuestFee(e.target.value)}
                  className="w-32"
                />
                <span className="text-muted-foreground">%</span>
              </div>
            </div>

            <div className="pt-4 border-t">
              <div className="flex justify-between items-center">
                <div className="text-sm text-muted-foreground">
                  <span>Total Platform Fee: </span>
                  <span className="font-medium text-foreground">
                    {(parseFloat(hostFee || '0') + parseFloat(guestFee || '0')).toFixed(1)}%
                  </span>
                </div>
                <Button onClick={handleUpdate} disabled={updating}>
                  {updating ? 'Updating...' : 'Update Configuration'}
                </Button>
              </div>
              <div className="text-xs text-muted-foreground mt-2">
                Last updated by {config.lastUpdatedBy} on{' '}
                {new Date(config.lastUpdatedAt).toLocaleString('vi-VN')}
              </div>
            </div>
          </>
        )}
      </CardContent>
    </Card>
  );
}
