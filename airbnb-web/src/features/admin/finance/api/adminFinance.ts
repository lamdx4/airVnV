import { api } from '@/lib/api';
import type { PayoutResponse, DashboardFinance } from '../types';

export interface GetPayoutsParams {
  page?: number;
  pageSize?: number;
  hostId?: string;
  status?: string;
  fromDate?: string;
  toDate?: string;
}

export interface GetDashboardFinanceParams {
  fromDate?: string;
  toDate?: string;
}

export const adminFinanceApi = {
  getPayouts: (params?: GetPayoutsParams): Promise<PayoutResponse> => {
    return api.get('/api/admin/finance/payouts', { params }) as Promise<PayoutResponse>;
  },

  getDashboard: (params?: GetDashboardFinanceParams): Promise<DashboardFinance> => {
    return api.get('/api/admin/finance/dashboard', { params }) as Promise<DashboardFinance>;
  },
};
