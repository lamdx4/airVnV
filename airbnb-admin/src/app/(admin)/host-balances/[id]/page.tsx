import { HostBalanceDetail } from "@/features/host-balances";

interface PageProps {
  params: Promise<{ id: string }>;
}

export default async function HostBalanceDetailPage({ params }: PageProps) {
  const { id } = await params;
  return <HostBalanceDetail balanceId={id} />;
}
