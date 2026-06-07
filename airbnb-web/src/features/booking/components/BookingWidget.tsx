import React, { useState, useMemo } from 'react';
import type { DateRange } from 'react-day-picker';
import { addDays, differenceInDays, format, isBefore, startOfDay, isFriday, isSaturday } from 'date-fns';
import { Calendar } from '@/components/ui/calendar';
import { Button } from '@/components/ui/button';
import { useCreateBooking } from '../hooks';
import { useInitiatePayment } from '@/features/payment/hooks';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';

interface BookingWidgetProps {
  propertyId: string;
  basePrice: number;
  cleaningFee: number;
  serviceFee: number;
  weekendPremiumPercent: number;
  currencyCode: string;
}

export const BookingWidget: React.FC<BookingWidgetProps> = ({
  propertyId,
  basePrice,
  cleaningFee,
  serviceFee,
  weekendPremiumPercent,
  currencyCode
}) => {
  const [date, setDate] = useState<DateRange | undefined>({
    from: startOfDay(new Date()),
    to: addDays(startOfDay(new Date()), 3),
  });
  const [guestCount, setGuestCount] = useState<number>(1);
  const [isCalendarOpen, setIsCalendarOpen] = useState(false);
  const navigate = useNavigate();
  const { t } = useTranslation();

  const createBooking = useCreateBooking();
  const initiatePayment = useInitiatePayment();

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currencyCode || 'VND',
    }).format(amount);
  };

  const calculation = useMemo(() => {
    if (!date?.from || !date?.to) return null;
    const nights = differenceInDays(date.to, date.from);
    if (nights <= 0) return null;

    let weekendPremiumTotal = 0;
    if (weekendPremiumPercent > 0) {
      const premiumPerNight = basePrice * (weekendPremiumPercent / 100);
      for (let i = 0; i < nights; i++) {
        const currentDay = addDays(date.from, i);
        if (isFriday(currentDay) || isSaturday(currentDay)) {
          weekendPremiumTotal += premiumPerNight;
        }
      }
    }

    const baseTotal = nights * basePrice;
    const total = baseTotal + weekendPremiumTotal + cleaningFee + serviceFee;

    return { nights, baseTotal, weekendPremiumTotal, cleaningFee, serviceFee, total };
  }, [date, basePrice, cleaningFee, serviceFee, weekendPremiumPercent]);

  const handleBook = () => {
    if (!date?.from || !date?.to) return;
    
    // The requirement says we shouldn't send TotalPrice, we let backend calculate.
    // We send checkIn and checkOut as DateOnly strings
    createBooking.mutate({
      propertyId,
      checkIn: format(date.from, 'yyyy-MM-dd'),
      checkOut: format(date.to, 'yyyy-MM-dd'),
      guestCount
    }, {
      onSuccess: (data: any) => {
        // Chain the payment initiation
        initiatePayment.mutate({ bookingId: data.bookingId }, {
          onSuccess: (paymentData) => {
            // Redirect to VNPay
            window.location.href = paymentData.paymentUrl;
          },
          onError: (err: any) => {
            console.error("Payment initiation failed:", err);
            // If payment initiation fails, still go to trips to see the pending booking
            navigate('/trips');
          }
        });
      },
      onError: (err: any) => {
        // Here we could show a toast
        console.error("Booking failed:", err);
      }
    });
  };

  return (
    <div className="bg-white rounded-xl shadow-lg border border-gray-200 p-6 sticky top-24">
      <div className="flex items-end justify-between mb-6">
        <div>
          <span className="text-2xl font-bold">{formatCurrency(basePrice)}</span>
          <span className="text-gray-500 font-normal"> {t('bookingWidget.perNight')}</span>
        </div>
      </div>

      <div className="border border-gray-300 rounded-xl overflow-hidden mb-4">
        <div className="flex cursor-pointer relative" onClick={() => setIsCalendarOpen(!isCalendarOpen)}>
          <div className="flex-1 p-3 border-r border-gray-300 hover:bg-gray-50 transition">
            <div className="text-[10px] font-bold uppercase text-gray-800">{t('bookingWidget.checkIn')}</div>
            <div className="text-sm text-gray-600">{date?.from ? format(date.from, 'MM/dd/yyyy') : t('bookingWidget.addDate')}</div>
          </div>
          <div className="flex-1 p-3 hover:bg-gray-50 transition">
            <div className="text-[10px] font-bold uppercase text-gray-800">{t('bookingWidget.checkOut')}</div>
            <div className="text-sm text-gray-600">{date?.to ? format(date.to, 'MM/dd/yyyy') : t('bookingWidget.addDate')}</div>
          </div>
        </div>
        
        <div className="p-3 border-t border-gray-300 hover:bg-gray-50 transition">
          <div className="text-[10px] font-bold uppercase text-gray-800 mb-1">{t('bookingWidget.guests')}</div>
          <Select 
            value={String(guestCount)} 
            onValueChange={(val) => setGuestCount(Number(val))}
          >
            <SelectTrigger className="w-full bg-transparent border-0 shadow-none h-6 p-0 focus:ring-0 focus:ring-offset-0 text-sm text-gray-600 cursor-pointer focus-visible:ring-0">
              <SelectValue />
            </SelectTrigger>
            <SelectContent className="rounded-xl">
              {[1, 2, 3, 4, 5, 6].map(num => (
                <SelectItem key={num} value={String(num)}>
                  {num} {num > 1 ? t('bookingWidget.guestsPlural') : t('bookingWidget.guest')}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      {isCalendarOpen && (
        <div className="absolute z-10 bg-white border border-gray-200 shadow-xl rounded-xl mt-1 -ml-6 p-4">
          <Calendar
            initialFocus
            mode="range"
            defaultMonth={date?.from}
            selected={date}
            onSelect={setDate}
            numberOfMonths={2}
            disabled={(date) => isBefore(date, startOfDay(new Date()))}
          />
          <div className="flex justify-end mt-4">
            <Button variant="ghost" size="sm" onClick={() => setIsCalendarOpen(false)}>{t('bookingWidget.close')}</Button>
          </div>
        </div>
      )}

      <Button 
        className="w-full bg-[#ff385c] hover:bg-[#e31c5f] text-white py-6 text-lg font-semibold rounded-lg"
        onClick={handleBook}
        disabled={!calculation || createBooking.isPending}
      >
        {createBooking.isPending ? t('bookingWidget.reserving') : t('bookingWidget.reserve')}
      </Button>

      <div className="text-center text-sm text-gray-500 mt-4 mb-6">
        {t('bookingWidget.notChargedYet')}
      </div>

      {calculation && (
        <div className="space-y-4 text-gray-700">
          <div className="flex justify-between">
            <span className="underline">{formatCurrency(basePrice)} x {calculation.nights} {t('bookingWidget.nights')}</span>
            <span>{formatCurrency(calculation.baseTotal)}</span>
          </div>
          
          {calculation.weekendPremiumTotal > 0 && (
            <div className="flex justify-between">
              <span className="underline">{t('bookingWidget.weekendPremium')}</span>
              <span>{formatCurrency(calculation.weekendPremiumTotal)}</span>
            </div>
          )}

          <div className="flex justify-between">
            <span className="underline">{t('bookingWidget.cleaningFee')}</span>
            <span>{formatCurrency(calculation.cleaningFee)}</span>
          </div>
          
          <div className="flex justify-between">
            <span className="underline">{t('bookingWidget.serviceFee')}</span>
            <span>{formatCurrency(calculation.serviceFee)}</span>
          </div>

          <div className="pt-4 border-t border-gray-200 flex justify-between font-bold text-lg text-gray-900">
            <span>{t('bookingWidget.total')}</span>
            <span>{formatCurrency(calculation.total)}</span>
          </div>
        </div>
      )}

      <div className="mt-6 p-4 bg-gray-50 rounded-lg border border-gray-200 text-xs text-gray-500 italic">
        {t('bookingWidget.priceReference', { currency: currencyCode || 'VND' })}
      </div>

      <Button 
        variant="outline" 
        className="w-full mt-4 font-semibold border-gray-400"
        onClick={() => navigate(`/messages?propertyId=${propertyId}`)}
      >
        {t('bookingWidget.contactHost')}
      </Button>
    </div>
  );
};
