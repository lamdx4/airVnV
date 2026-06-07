import { PayoutDetail } from "@/features/payouts";

interface PageProps {
  params: Promise<{ id: string }>;
}

export default async function PayoutDetailPage({ params }: PageProps) {
  const { id } = await params;
  return <PayoutDetail payoutId={id} />;
}
