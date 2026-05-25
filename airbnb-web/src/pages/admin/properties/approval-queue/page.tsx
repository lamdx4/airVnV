import { PropertyApprovalQueue } from '@/features/admin/properties/components/PropertyApprovalQueue';

export default function ApprovalQueuePage() {
  return (
    <div className="container max-w-5xl mx-auto px-4 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-semibold text-[#222222]">Property Moderation</h1>
        <p className="text-[#717171] mt-1">Review and approve pending property listings</p>
      </div>

      <PropertyApprovalQueue />
    </div>
  );
}