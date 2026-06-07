import { HostBalancesList } from "@/features/host-balances";

export default function HostBalancesPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-[28px] font-bold text-[#222222]">Host Balances</h1>
        <p className="mt-1 text-sm text-[#6a6a6a]">
          Escrow ledger — money the platform is currently holding on behalf of hosts.
        </p>
      </div>
      <HostBalancesList />
    </div>
  );
}
