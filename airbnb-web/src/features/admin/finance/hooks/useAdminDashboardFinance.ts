import { useState, useEffect } from 'react';
import { adminFinanceApi, type GetDashboardFinanceParams } from '../api/adminFinance';
import type { DashboardFinance } from '../types';

export function useAdminDashboardFinance(params?: GetDashboardFinanceParams) {
  const [finance, setFinance] = useState<DashboardFinance | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchFinance = async () => {
      try {
        setLoading(true);
        setError(null);
        const data = await adminFinanceApi.getDashboard(params);
        setFinance(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to fetch finance data');
      } finally {
        setLoading(false);
      }
    };

    fetchFinance();
  }, [params?.fromDate, params?.toDate]);

  return { finance, loading, error };
}
