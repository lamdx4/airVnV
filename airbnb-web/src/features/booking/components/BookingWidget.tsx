import React, { useState, useMemo } from 'react';
import type { DateRange } from 'react-day-picker';
import { addDays, differenceInDays, format, isBefore, startOfDay, isFriday, isSaturday } from 'date-fns';
import { Calendar } from '@/components/ui/calendar';
import { Button } from '@/components/ui/button';
import { useCreateBooking } from '../hooks';
import { useNavigate } from 'react-router-dom';

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

  const createBooking = useCreateBooking();

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
      onSuccess: () => {
        navigate('/trips');
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
          <span className="text-gray-500 font-normal"> / night</span>
        </div>
      </div>

      <div className="border border-gray-300 rounded-xl overflow-hidden mb-4">
        <div className="flex cursor-pointer relative" onClick={() => setIsCalendarOpen(!isCalendarOpen)}>
          <div className="flex-1 p-3 border-r border-gray-300 hover:bg-gray-50 transition">
            <div className="text-[10px] font-bold uppercase text-gray-800">Check-in</div>
            <div className="text-sm text-gray-600">{date?.from ? format(date.from, 'MM/dd/yyyy') : 'Add date'}</div>
          </div>
          <div className="flex-1 p-3 hover:bg-gray-50 transition">
            <div className="text-[10px] font-bold uppercase text-gray-800">Checkout</div>
            <div className="text-sm text-gray-600">{date?.to ? format(date.to, 'MM/dd/yyyy') : 'Add date'}</div>
          </div>
        </div>
        
        <div className="p-3 border-t border-gray-300 hover:bg-gray-50 transition">
          <div className="text-[10px] font-bold uppercase text-gray-800">Guests</div>
          <select 
            className="w-full bg-transparent outline-none text-sm text-gray-600 cursor-pointer"
            value={guestCount}
            onChange={(e) => setGuestCount(Number(e.target.value))}
          >
            {[1,2,3,4,5,6].map(num => (
              <option key={num} value={num}>{num} guest{num > 1 ? 's' : ''}</option>
            ))}
          </select>
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
            <Button variant="ghost" size="sm" onClick={() => setIsCalendarOpen(false)}>Close</Button>
          </div>
        </div>
      )}

      <Button 
        className="w-full bg-[#ff385c] hover:bg-[#e31c5f] text-white py-6 text-lg font-semibold rounded-lg"
        onClick={handleBook}
        disabled={!calculation || createBooking.isPending}
      >
        {createBooking.isPending ? 'Reserving...' : 'Reserve'}
      </Button>

      <div className="text-center text-sm text-gray-500 mt-4 mb-6">
        You won't be charged yet
      </div>

      {calculation && (
        <div className="space-y-4 text-gray-700">
          <div className="flex justify-between">
            <span className="underline">{formatCurrency(basePrice)} x {calculation.nights} nights</span>
            <span>{formatCurrency(calculation.baseTotal)}</span>
          </div>
          
          {calculation.weekendPremiumTotal > 0 && (
            <div className="flex justify-between">
              <span className="underline">Weekend Premium</span>
              <span>{formatCurrency(calculation.weekendPremiumTotal)}</span>
            </div>
          )}

          <div className="flex justify-between">
            <span className="underline">Cleaning fee</span>
            <span>{formatCurrency(calculation.cleaningFee)}</span>
          </div>
          
          <div className="flex justify-between">
            <span className="underline">Service fee</span>
            <span>{formatCurrency(calculation.serviceFee)}</span>
          </div>

          <div className="pt-4 border-t border-gray-200 flex justify-between font-bold text-lg text-gray-900">
            <span>Total</span>
            <span>{formatCurrency(calculation.total)}</span>
          </div>
        </div>
      )}

      <div className="mt-6 p-4 bg-gray-50 rounded-lg border border-gray-200 text-xs text-gray-500 italic">
        Price shown in {currencyCode || 'VND'} for reference. You will be charged the exact equivalent amount by your local bank if this is not your native currency.
      </div>
    </div>
  );
};
