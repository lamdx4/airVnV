import { api } from '@/lib/api';
import type { InitiatePaymentRequest, InitiatePaymentResponse } from '../types';

export const initiatePayment = async (data: InitiatePaymentRequest): Promise<InitiatePaymentResponse> => {
  const response = await api.post<any>('/api/payments/initiate', data);
  return response as unknown as InitiatePaymentResponse;
};
