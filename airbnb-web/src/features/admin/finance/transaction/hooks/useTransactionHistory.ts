import { useState } from 'react';
import { financeTransactionsApi, type TransactionParams } from '../api/financeTransactions';
import type { TransactionResponse } from '../../types';

export function useTransactionHistory() {
  const [data, setData] = useState<TransactionResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchTransactions = async (params?: TransactionParams) => {
    try {
      setLoading(true);
      setError(null);
      const result = await financeTransactionsApi.getTransactions(params);
      setData(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch transactions');
    } finally {
      setLoading(false);
    }
  };

  return { data, loading, error, fetchTransactions };
}
