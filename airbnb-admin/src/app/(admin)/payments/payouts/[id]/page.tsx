interface PageProps {
  params: Promise<{ id: string }>;
}

export default async function PayoutDetailPage({ params }: PageProps) {
  const { id } = await params;
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-[28px] font-bold text-[#222222]">Payout Detail</h1>
        <p className="text-sm text-[#6a6a6a]">Coming soon</p>
      </div>
    </div>
  );
}
