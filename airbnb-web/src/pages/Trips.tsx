import { useGuestBookings, useCancelBooking, BookingStatus } from '@/features/booking';
import { useInitiatePayment } from '@/features/payment/hooks';
import { useProperty } from '@/features/properties/hooks/useProperties';
import { format, parseISO } from 'date-fns';
import { Button } from '@/components/ui/button';
import { useState } from 'react';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { useTranslation } from 'react-i18next';

function BookingCard({ booking, onPay, onCancel, isPaying }: {
  booking: {
    id: string;
    propertyId: string;
    hostId: string;
    guestId: string;
    checkIn: string;
    checkOut: string;
    guestCount: number;
    nightCount: number;
    totalPrice: number;
    currencyCode: string;
    status: BookingStatus;
  };
  onPay: (bookingId: string) => void;
  onCancel: (bookingId: string) => void;
  isPaying: boolean;
}) {
  const { data: property } = useProperty(booking.propertyId);
  const thumbnail = property?.images?.find(img => img.type === 0)?.url || property?.images?.[0]?.url;
  const { t } = useTranslation();

  return (
    <div className="border border-gray-200 rounded-xl overflow-hidden shadow-sm hover:shadow-md transition bg-white flex flex-col">
      <div className="h-40 bg-gray-200 overflow-hidden">
        {thumbnail ? (
          <img src={thumbnail} alt={property?.title || 'Property'} className="w-full h-full object-cover" />
        ) : (
          <div className="w-full h-full flex items-center justify-center text-gray-400">
            {property?.title || 'Property'}
          </div>
        )}
      </div>
      <div className="p-5 flex-1 flex flex-col">
        <div className="flex justify-between items-start mb-2">
          <h3 className="font-semibold text-lg text-gray-900 truncate">
            {format(parseISO(booking.checkIn), 'MMM d')} - {format(parseISO(booking.checkOut), 'MMM d, yyyy')}
          </h3>
          <span className={`px-2 py-1 text-xs font-bold rounded-full ${
            booking.status === BookingStatus.Confirmed ? 'bg-green-100 text-green-800' :
            booking.status === BookingStatus.Pending ? 'bg-yellow-100 text-yellow-800' :
            booking.status === BookingStatus.AwaitingApproval ? 'bg-orange-100 text-orange-800' :
            'bg-gray-100 text-gray-800'
          }`}>
            {booking.status === BookingStatus.AwaitingApproval ? t('trips.awaitingHostApproval') : booking.status}
          </span>
        </div>
        
        <p className="text-sm text-gray-500 mb-4">
          {booking.nightCount} nights • {booking.guestCount} guests
        </p>

        <div className="text-sm text-gray-700 mb-4 font-medium">
          Total: {new Intl.NumberFormat('en-US', { style: 'currency', currency: booking.currencyCode }).format(booking.totalPrice)}
        </div>

        <div className="mt-auto pt-4 border-t border-gray-100 flex flex-col gap-2">
          {booking.status === BookingStatus.Pending && (
            <Button 
              className="w-full bg-[#ff385c] hover:bg-[#e31c5f] text-white"
              onClick={() => onPay(booking.id)}
              disabled={isPaying}
            >
              {isPaying ? t('trips.processing') : t('trips.payNow')}
            </Button>
          )}
          {booking.status === BookingStatus.Pending || booking.status === BookingStatus.AwaitingApproval || booking.status === BookingStatus.Confirmed ? (
            <Button 
              variant="outline" 
              className="w-full text-red-600 border-red-200 hover:bg-red-50 hover:text-red-700"
              onClick={() => onCancel(booking.id)}
            >
              {t('trips.cancelReservation')}
            </Button>
          ) : (
            <Button variant="outline" className="w-full" disabled>
              {booking.status}
            </Button>
          )}
        </div>
      </div>
    </div>
  );
}

export default function Trips() {
  const { t } = useTranslation();
  const { data: bookings, isLoading, isError } = useGuestBookings();
  const cancelBooking = useCancelBooking();
  const initiatePayment = useInitiatePayment();
  const [bookingToCancel, setBookingToCancel] = useState<string | null>(null);

  if (isLoading) return <div className="p-8 text-center text-gray-500">{t('trips.loading')}</div>;
  if (isError) return <div className="p-8 text-center text-red-500">{t('trips.failedToLoad')}</div>;

  return (
    <div className="max-w-5xl mx-auto px-6 py-12">
      <h1 className="text-3xl font-bold mb-8 text-gray-900">{t('trips.title')}</h1>

      {!bookings || bookings.length === 0 ? (
        <div className="bg-gray-50 rounded-xl p-8 border border-gray-200">
          <h2 className="text-xl font-semibold mb-2">{t('trips.noTrips')}</h2>
          <p className="text-gray-600 mb-4">{t('trips.noTripsDesc')}</p>
          <Button variant="outline" className="border-gray-900 font-semibold hover:bg-gray-100">
            {t('trips.startSearching')}
          </Button>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {bookings.map((booking) => (
            <BookingCard
              key={booking.id}
              booking={booking}
              onPay={(bookingId) => {
                initiatePayment.mutate({ bookingId }, {
                  onSuccess: (data) => {
                    window.location.href = data.paymentUrl;
                  },
                  onError: (err) => {
                    console.error("Failed to initiate payment:", err);
                  }
                });
              }}
              onCancel={(bookingId) => setBookingToCancel(bookingId)}
              isPaying={initiatePayment.isPending}
            />
          ))}
        </div>
      )}

      <ConfirmDialog
        open={bookingToCancel !== null}
        title={t('trips.cancelTitle')}
        description={t('trips.cancelDesc')}
        confirmText={t('trips.cancelConfirm')}
        cancelText={t('trips.close')}
        variant="destructive"
        onConfirm={() => {
          if (bookingToCancel) {
            cancelBooking.mutate(bookingToCancel);
            setBookingToCancel(null);
          }
        }}
        onCancel={() => setBookingToCancel(null)}
      />
    </div>
  );
}

