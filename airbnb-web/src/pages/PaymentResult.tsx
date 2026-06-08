import React, { useEffect, useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { CheckCircle2Icon, XCircleIcon } from 'lucide-react';
import { useQueryClient } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5136';

const PaymentResult: React.FC = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { t } = useTranslation();
  const [status, setStatus] = useState<'success' | 'failed' | 'loading'>('loading');
  const [message, setMessage] = useState('');

  useEffect(() => {
    const responseCode = searchParams.get('vnp_ResponseCode');
    const orderInfo = searchParams.get('vnp_OrderInfo');

    if (!responseCode) {
      navigate('/');
      return;
    }

    // Dev workaround: VNPay IPN cannot reach localhost, so the browser proxies
    // the return params back to the BE so it can validate signature and mark
    // Payment as Success (which publishes the PaymentSucceededEvent).
    const ipnUrl = `${API_URL}/api/payments/vnpay/ipn?${searchParams.toString()}`;
    fetch(ipnUrl)
      .catch(err => console.warn('IPN proxy call failed:', err))
      .finally(() => {
        if (responseCode === '00') {
          setStatus('success');
          setMessage(orderInfo || t('payment.successMsg'));
          queryClient.invalidateQueries({ queryKey: ['guest_bookings'] });
        } else {
          setStatus('failed');
          setMessage(orderInfo || t('payment.failedMsg'));
        }
      });
  }, [searchParams, navigate, queryClient, t]);

  if (status === 'loading') {
    return <div className="min-h-screen flex items-center justify-center">{t('payment.processing')}</div>;
  }

  return (
    <div className="min-h-[70vh] flex flex-col items-center justify-center p-4">
      <div className="max-w-md w-full bg-white rounded-2xl shadow-xl p-8 text-center border border-gray-100">
        {status === 'success' ? (
          <>
            <div className="mx-auto w-20 h-20 bg-green-100 rounded-full flex items-center justify-center mb-6">
              <CheckCircle2Icon className="w-10 h-10 text-green-600" />
            </div>
            <h1 className="text-3xl font-bold text-gray-900 mb-2">{t('payment.successTitle')}</h1>
            <p className="text-gray-500 mb-8">{message}</p>
            <Button 
              onClick={() => navigate('/trips')} 
              className="w-full bg-slate-900 text-white rounded-xl py-6 text-lg font-semibold hover:scale-[1.02] transition-transform"
            >
              {t('payment.viewTrips')}
            </Button>
          </>
        ) : (
          <>
            <div className="mx-auto w-20 h-20 bg-red-100 rounded-full flex items-center justify-center mb-6">
              <XCircleIcon className="w-10 h-10 text-red-600" />
            </div>
            <h1 className="text-3xl font-bold text-gray-900 mb-2">{t('payment.failedTitle')}</h1>
            <p className="text-gray-500 mb-8">{message}</p>
            <Button 
              onClick={() => navigate('/trips')} 
              className="w-full bg-[#ff385c] hover:bg-[#e31c5f] text-white rounded-xl py-6 text-lg font-semibold hover:scale-[1.02] transition-transform"
            >
              {t('payment.tryAgain')}
            </Button>
          </>
        )}
      </div>
    </div>
  );
};

export default PaymentResult;
