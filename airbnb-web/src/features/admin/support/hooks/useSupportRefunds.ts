import { useState } from 'react';
import { supportApi, type GetRefundsParams } from '../api/supportApi';
import type { RefundsResponse, RefundDetail } from '../types';

export function useSupportRefunds() {
  const [data, setData] = useState<RefundsResponse | null>(null);
  const [selectedRefund, setSelectedRefund] = useState<RefundDetail | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchRefunds = async (params?: GetRefundsParams) => {
    try {
      setLoading(true);
      setError(null);
      const result = await supportApi.getRefunds(params);
      setData(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch refunds');
    } finally {
      setLoading(false);
    }
  };

  const processRefund = async (refundId: string, action: 'approve' | 'reject' | 'process', rejectionReason?: string) => {
    const result = await supportApi.processRefund({ refundId, action, rejectionReason });
    setSelectedRefund(result);
    return result;
  };

  return {
    data,
    selectedRefund,
    loading,
    error,
    fetchRefunds,
    processRefund,
    setSelectedRefund,
  };
}
