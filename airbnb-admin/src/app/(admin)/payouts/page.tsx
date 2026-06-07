import { PayoutsList } from "@/features/payouts";

export default function PayoutsPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-[28px] font-bold text-[#222222]">Payouts</h1>
        <p className="mt-1 text-sm text-[#6a6a6a]">
          Money owed to hosts based on completed bookings. Approve to queue bank transfer.
        </p>
      </div>
      <PayoutsList />
    </div>
  );
}
