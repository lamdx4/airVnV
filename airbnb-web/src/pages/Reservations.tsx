import { useHostBookings, useApproveBooking, useRejectBooking, useCancelBooking } from '@/features/booking';
import { format, parseISO } from 'date-fns';
import { Button } from '@/components/ui/button';

export default function Reservations() {
  const { data: bookings, isLoading, isError } = useHostBookings();
  const approveBooking = useApproveBooking();
  const rejectBooking = useRejectBooking();
  const cancelBooking = useCancelBooking();

  if (isLoading) return <div className="p-8 text-center text-gray-500">Loading reservations...</div>;
  if (isError) return <div className="p-8 text-center text-red-500">Failed to load reservations.</div>;

  return (
    <div className="max-w-6xl mx-auto px-6 py-12">
      <h1 className="text-3xl font-bold mb-8 text-gray-900">Reservations</h1>

      {!bookings || bookings.length === 0 ? (
        <div className="bg-gray-50 rounded-xl p-8 border border-gray-200">
          <h2 className="text-xl font-semibold mb-2">No reservations yet</h2>
          <p className="text-gray-600">You don't have any guest reservations right now.</p>
        </div>
      ) : (
        <div className="bg-white border border-gray-200 rounded-xl overflow-hidden shadow-sm">
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm">
              <thead className="bg-gray-50 border-b border-gray-200 text-gray-600">
                <tr>
                  <th className="px-6 py-4 font-semibold">Status</th>
                  <th className="px-6 py-4 font-semibold">Guest</th>
                  <th className="px-6 py-4 font-semibold">Property</th>
                  <th className="px-6 py-4 font-semibold">Dates</th>
                  <th className="px-6 py-4 font-semibold">Payout</th>
                  <th className="px-6 py-4 font-semibold text-right">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {bookings.map(booking => (
                  <tr key={booking.id} className="hover:bg-gray-50 transition">
                    <td className="px-6 py-4">
                      <span className={`px-2 py-1 text-xs font-bold rounded-full ${
                        booking.status === 'Confirmed' ? 'bg-green-100 text-green-800' :
                        booking.status === 'Pending' ? 'bg-yellow-100 text-yellow-800' :
                        booking.status === 'AwaitingApproval' ? 'bg-orange-100 text-orange-800' :
                        'bg-gray-100 text-gray-800'
                      }`}>
                        {booking.status === 'AwaitingApproval' ? 'Awaiting Approval' : booking.status}
                      </span>
                    </td>
                    <td className="px-6 py-4">
                      <div className="font-medium text-gray-900">Guest {booking.guestId?.substring(0,6)}</div>
                      <div className="text-gray-500">{booking.guestCount} guests</div>
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
                      {booking.status === 'AwaitingApproval' && (
                        <>
                          <Button 
                            variant="default" 
                            size="sm" 
                            className="bg-green-600 hover:bg-green-700 text-white"
                            onClick={() => approveBooking.mutate(booking.id)}
                            disabled={approveBooking.isPending || rejectBooking.isPending}
                          >
                            Approve
                          </Button>
                          <Button 
                            variant="outline" 
                            size="sm" 
                            className="text-red-600 hover:bg-red-50 hover:text-red-700"
                            onClick={() => {
                              if(window.confirm("Are you sure you want to reject this request?")) {
                                rejectBooking.mutate(booking.id);
                              }
                            }}
                            disabled={approveBooking.isPending || rejectBooking.isPending}
                          >
                            Reject
                          </Button>
                        </>
                      )}
                      
                      {booking.status === 'Confirmed' && (
                        <Button 
                          variant="outline" 
                          size="sm" 
                          className="text-red-600 hover:bg-red-50 hover:text-red-700"
                          onClick={() => {
                            if(window.confirm("Cancel this confirmed reservation?")) {
                              cancelBooking.mutate(booking.id);
                            }
                          }}
                          disabled={cancelBooking.isPending}
                        >
                          Cancel
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
    </div>
  );
}
