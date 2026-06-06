import { PaymentsList } from "@/features/payments";

export default function PaymentsPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-[28px] font-bold text-[#222222]">Payments</h1>
        <p className="mt-1 text-sm text-[#6a6a6a]">
          Pay-in transactions from guests for bookings.
        </p>
      </div>
      <PaymentsList />
    </div>
  );
}
