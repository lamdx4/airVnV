import { useMutation } from '@tanstack/react-query';
import { initiatePayment } from '../api';
import type { InitiatePaymentRequest } from '../types';

export const useInitiatePayment = () => {
  return useMutation({
    mutationFn: (req: InitiatePaymentRequest) => initiatePayment(req),
  });
};
