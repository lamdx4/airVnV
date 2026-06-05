import { PaymentsList } from "@/features/payments";

export default function PaymentsPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-[28px] font-bold text-[#222222]">Payments</h1>
        <p className="text-sm text-[#6a6a6a] mt-1">
          View and manage payment transactions
        </p>
      </div>
      <PaymentsList />
    </div>
  );
}
