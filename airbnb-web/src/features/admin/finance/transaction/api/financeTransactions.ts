import { api } from '@/lib/api';
import type { TransactionResponse, PlatformFeeConfig } from '../../types';

export interface TransactionParams {
  page?: number;
  pageSize?: number;
  transactionId?: string;
  status?: string;
  guestId?: string;
  hostId?: string;
  currency?: string;
  minAmount?: number;
  maxAmount?: number;
  fromDate?: string;
  toDate?: string;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

export const financeTransactionsApi = {
  getTransactions: (params?: TransactionParams): Promise<TransactionResponse> => {
    return api.get('/api/admin/finance/transactions', { params }) as Promise<TransactionResponse>;
  },

  getPlatformFee: (): Promise<PlatformFeeConfig> => {
    return api.get('/api/admin/finance/platform-fee') as Promise<PlatformFeeConfig>;
  },

  updatePlatformFee: (data: { hostFeePercent?: number; guestFeePercent?: number }): Promise<PlatformFeeConfig> => {
    return api.put('/api/admin/finance/platform-fee', data) as Promise<PlatformFeeConfig>;
  },
};
