import { useHostBookings, useApproveBooking, useRejectBooking, useCancelBooking, BookingStatus, BookingMode } from '@/features/booking';
import { format, parseISO } from 'date-fns';
import { Button } from '@/components/ui/button';
import { useState } from 'react';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { useTranslation } from 'react-i18next';

export default function Reservations() {
  const { t } = useTranslation();
  const { data: bookings, isLoading, isError } = useHostBookings();
  const approveBooking = useApproveBooking();
  const rejectBooking = useRejectBooking();
  const cancelBooking = useCancelBooking();
  const [bookingToReject, setBookingToReject] = useState<string | null>(null);
  const [bookingToCancel, setBookingToCancel] = useState<string | null>(null);

  if (isLoading) return <div className="p-8 text-center text-gray-500">{t('reservations.loading')}</div>;
  if (isError) return <div className="p-8 text-center text-red-500">{t('reservations.failedToLoad')}</div>;

  return (
    <div className="max-w-6xl mx-auto px-6 py-12">
      <h1 className="text-3xl font-bold mb-8 text-gray-900">{t('reservations.title')}</h1>

      {!bookings || bookings.length === 0 ? (
        <div className="bg-gray-50 rounded-xl p-8 border border-gray-200">
          <h2 className="text-xl font-semibold mb-2">{t('reservations.noReservations')}</h2>
          <p className="text-gray-600">{t('reservations.noReservationsDesc')}</p>
        </div>
      ) : (
        <div className="bg-white border border-gray-200 rounded-xl overflow-hidden shadow-sm">
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm">
              <thead className="bg-gray-50 border-b border-gray-200 text-gray-600">
                <tr>
                  <th className="px-6 py-4 font-semibold">{t('reservations.status')}</th>
                  <th className="px-6 py-4 font-semibold">{t('reservations.guest')}</th>
                  <th className="px-6 py-4 font-semibold">{t('reservations.property')}</th>
                  <th className="px-6 py-4 font-semibold">{t('reservations.dates')}</th>
                  <th className="px-6 py-4 font-semibold">{t('reservations.payout')}</th>
                  <th className="px-6 py-4 font-semibold text-right">{t('reservations.actions')}</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {bookings.map(booking => (
                  <tr key={booking.id} className="hover:bg-gray-50 transition">
                    <td className="px-6 py-4">
                      <span className={`px-2 py-1 text-xs font-bold rounded-full ${
                        booking.status === BookingStatus.Confirmed ? 'bg-green-100 text-green-800' :
                        booking.status === BookingStatus.Pending ? 'bg-yellow-100 text-yellow-800' :
                        booking.status === BookingStatus.AwaitingApproval ? 'bg-orange-100 text-orange-800' :
                        'bg-gray-100 text-gray-800'
                      }`}>
                        {booking.status === BookingStatus.AwaitingApproval ? t('reservations.awaitingApproval') : booking.status}
                      </span>
                    </td>
                    <td className="px-6 py-4">
                      <div className="font-medium text-gray-900">Guest {booking.guestId?.substring(0,6)}</div>
                      <div className="text-gray-500">{booking.guestCount} guests</div>
                      {booking.bookingMode && (
                        <div className="mt-2 text-[10px] uppercase font-bold tracking-wider">
                          {booking.bookingMode === BookingMode.InstantBook ? (
                            <span className="bg-blue-50 text-blue-700 px-2 py-0.5 rounded border border-blue-200 shadow-sm">⚡ Instant Book</span>
                          ) : (
                            <span className="bg-gray-100 text-gray-700 px-2 py-0.5 rounded border border-gray-200 shadow-sm">✉️ Request</span>
                          )}
                        </div>
                      )}
                    </td>
                    <td className="px-6 py-4 text-gray-600">
                      ID: {booking.propertyId.substring(0,8)}
                    </td>
                    <td className="px-6 py-4 text-gray-600">
                      {format(parseISO(booking.checkIn), 'MMM d')} - {format(parseISO(booking.checkOut), 'MMM d, yyyy')}
                      <div className="text-xs text-gray-400 mt-1">{booking.nightCount} nights</div>
                    </td>
                    <td className="px-6 py-4 font-medium text-gray-900">
                      {new Intl.NumberFormat('en-US', { style: 'currency', currency: booking.currencyCode }).format(booking.totalPrice)}
                    </td>
                    <td className="px-6 py-4 text-right space-x-2">
                      {booking.status === BookingStatus.AwaitingApproval && (
                        <>
                          <Button 
                            variant="default" 
                            size="sm" 
                            className="bg-green-600 hover:bg-green-700 text-white"
                            onClick={() => approveBooking.mutate(booking.id)}
                            disabled={approveBooking.isPending || rejectBooking.isPending}
                          >
                            {t('reservations.approve')}
                          </Button>
                          <Button 
                            variant="outline" 
                            size="sm" 
                            className="text-red-600 hover:bg-red-50 hover:text-red-700"
                            onClick={() => setBookingToReject(booking.id)}
                            disabled={approveBooking.isPending || rejectBooking.isPending}
                          >
                            {t('reservations.reject')}
                          </Button>
                        </>
                      )}
                      
                      {booking.status === BookingStatus.Confirmed && (
                        <Button 
                          variant="outline" 
                          size="sm" 
                          className="text-red-600 hover:bg-red-50 hover:text-red-700"
                          onClick={() => setBookingToCancel(booking.id)}
                          disabled={cancelBooking.isPending}
                        >
                          {t('reservations.cancel')}
                        </Button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      <ConfirmDialog
        open={bookingToReject !== null}
        title={t('reservations.rejectTitle')}
        description={t('reservations.rejectDesc')}
        confirmText={t('reservations.rejectConfirm')}
        cancelText={t('reservations.close')}
        variant="destructive"
        onConfirm={() => {
          if (bookingToReject) {
            rejectBooking.mutate(bookingToReject);
            setBookingToReject(null);
          }
        }}
        onCancel={() => setBookingToReject(null)}
      />

      <ConfirmDialog
        open={bookingToCancel !== null}
        title={t('reservations.cancelTitle')}
        description={t('reservations.cancelDesc')}
        confirmText={t('reservations.cancelConfirm')}
        cancelText={t('reservations.close')}
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

