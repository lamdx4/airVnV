import { useState } from 'react';
import { adminFinanceApi, type GetPayoutsParams } from '../api/adminFinance';
import type { PayoutResponse } from '../types';

export function useAdminPayouts() {
  const [payouts, setPayouts] = useState<PayoutResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchPayouts = async (params?: GetPayoutsParams) => {
    try {
      setLoading(true);
      setError(null);
      const data = await adminFinanceApi.getPayouts(params);
      setPayouts(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch payouts');
    } finally {
      setLoading(false);
    }
  };

  return { payouts, loading, error, fetchPayouts };
}
