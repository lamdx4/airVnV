import { useState } from 'react';
import { financeTransactionsApi } from '../api/financeTransactions';
import type { PlatformFeeConfig } from '../../types';

export function usePlatformFee() {
  const [config, setConfig] = useState<PlatformFeeConfig | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [updating, setUpdating] = useState(false);

  const fetchConfig = async () => {
    try {
      setLoading(true);
      setError(null);
      const result = await financeTransactionsApi.getPlatformFee();
      setConfig(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch platform fee config');
    } finally {
      setLoading(false);
    }
  };

  const updateConfig = async (hostFeePercent?: number, guestFeePercent?: number) => {
    try {
      setUpdating(true);
      setError(null);
      const result = await financeTransactionsApi.updatePlatformFee({ hostFeePercent, guestFeePercent });
      setConfig(result);
      return result;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update platform fee');
      throw err;
    } finally {
      setUpdating(false);
    }
  };

  return { config, loading, error, updating, fetchConfig, updateConfig };
}
