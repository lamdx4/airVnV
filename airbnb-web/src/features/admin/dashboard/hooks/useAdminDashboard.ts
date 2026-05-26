import { useState, useEffect } from 'react';
import { adminDashboardApi, type GetDashboardStatsParams } from '../api/adminDashboard';
import type { DashboardStats } from '../types';

export function useAdminDashboard(params?: GetDashboardStatsParams) {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        setLoading(true);
        setError(null);
        const data = await adminDashboardApi.getStats(params);
        setStats(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to fetch dashboard stats');
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, [params?.fromDate, params?.toDate]);

  return { stats, loading, error };
}
