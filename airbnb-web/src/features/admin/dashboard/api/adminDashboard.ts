import { api } from '@/lib/api';
import type { DashboardStats } from '../types';

export interface GetDashboardStatsParams {
  fromDate?: string;
  toDate?: string;
}

export const adminDashboardApi = {
  getStats: (params?: GetDashboardStatsParams): Promise<DashboardStats> => {
    return api.get('/api/admin/dashboard/stats', { params }) as Promise<DashboardStats>;
  },
};
