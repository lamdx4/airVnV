import { useGuestBookings, useCancelBooking } from '@/features/booking';
import { useInitiatePayment } from '@/features/payment/hooks';
import { format, parseISO } from 'date-fns';
import { Button } from '@/components/ui/button';

export default function Trips() {
  const { data: bookings, isLoading, isError } = useGuestBookings();
  const cancelBooking = useCancelBooking();
  const initiatePayment = useInitiatePayment();

  if (isLoading) return <div className="p-8 text-center text-gray-500">Loading your trips...</div>;
  if (isError) return <div className="p-8 text-center text-red-500">Failed to load trips.</div>;

  return (
    <div className="max-w-5xl mx-auto px-6 py-12">
      <h1 className="text-3xl font-bold mb-8 text-gray-900">Trips</h1>

      {!bookings || bookings.length === 0 ? (
        <div className="bg-gray-50 rounded-xl p-8 border border-gray-200">
          <h2 className="text-xl font-semibold mb-2">No trips booked... yet!</h2>
          <p className="text-gray-600 mb-4">Time to dust off your bags and start planning your next adventure.</p>
          <Button variant="outline" className="border-gray-900 font-semibold hover:bg-gray-100">
            Start searching
          </Button>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {bookings.map((booking) => (
            <div key={booking.id} className="border border-gray-200 rounded-xl overflow-hidden shadow-sm hover:shadow-md transition bg-white flex flex-col">
              <div className="h-40 bg-gray-200 flex items-center justify-center text-gray-400">
                {/* Normally we would fetch the Property Image here */}
                Property {booking.propertyId.substring(0, 8)}
              </div>
              <div className="p-5 flex-1 flex flex-col">
                <div className="flex justify-between items-start mb-2">
                  <h3 className="font-semibold text-lg text-gray-900 truncate">
                    {format(parseISO(booking.checkIn), 'MMM d')} - {format(parseISO(booking.checkOut), 'MMM d, yyyy')}
                  </h3>
                  <span className={`px-2 py-1 text-xs font-bold rounded-full ${
                    booking.status === 'Confirmed' ? 'bg-green-100 text-green-800' :
                    booking.status === 'Pending' ? 'bg-yellow-100 text-yellow-800' :
                    booking.status === 'AwaitingApproval' ? 'bg-orange-100 text-orange-800' :
                    'bg-gray-100 text-gray-800'
                  }`}>
                    {booking.status === 'AwaitingApproval' ? 'Awaiting Host Approval' : booking.status}
                  </span>
                </div>
                
                <p className="text-sm text-gray-500 mb-4">
                  {booking.nightCount} nights • {booking.guestCount} guests
                </p>

                <div className="text-sm text-gray-700 mb-4 font-medium">
                  Total: {new Intl.NumberFormat('en-US', { style: 'currency', currency: booking.currencyCode }).format(booking.totalPrice)}
                </div>

                <div className="mt-auto pt-4 border-t border-gray-100 flex flex-col gap-2">
                  {booking.status === 'Pending' && (
                    <Button 
                      className="w-full bg-[#ff385c] hover:bg-[#e31c5f] text-white"
                      onClick={() => {
                        initiatePayment.mutate({ bookingId: booking.id }, {
                          onSuccess: (data) => {
                            window.location.href = data.paymentUrl;
                          },
                          onError: (err) => {
                            console.error("Failed to initiate payment:", err);
                          }
                        });
                      }}
                      disabled={initiatePayment.isPending}
                    >
                      {initiatePayment.isPending ? 'Processing...' : 'Pay Now'}
                    </Button>
                  )}
                  {booking.status === 'Pending' || booking.status === 'AwaitingApproval' || booking.status === 'Confirmed' ? (
                    <Button 
                      variant="outline" 
                      className="w-full text-red-600 border-red-200 hover:bg-red-50 hover:text-red-700"
                      onClick={() => {
                        if (window.confirm("Are you sure you want to cancel this booking?")) {
                          cancelBooking.mutate(booking.id);
                        }
                      }}
                      disabled={cancelBooking.isPending || initiatePayment.isPending}
                    >
                      Cancel Reservation
                    </Button>
                  ) : (
                    <Button variant="outline" className="w-full" disabled>
                      {booking.status}
                    </Button>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
