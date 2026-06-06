import { PaymentDetail } from "@/features/payments";

interface PageProps {
  params: Promise<{ id: string }>;
}

export default async function PaymentDetailPage({ params }: PageProps) {
  const { id } = await params;
  return <PaymentDetail paymentId={id} />;
}
