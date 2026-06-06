import { BookingDetail } from "@/features/bookings/components/booking-detail";

interface BookingDetailPageProps {
  params: Promise<{ id: string }>;
}

export default async function BookingDetailPage({ params }: BookingDetailPageProps) {
  const { id } = await params;
  return <BookingDetail bookingId={id} />;
}
