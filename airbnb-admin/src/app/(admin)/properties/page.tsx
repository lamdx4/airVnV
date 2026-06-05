import { PropertiesList } from "@/features/properties";

export default function PropertiesPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-[28px] font-bold text-[#222222]">Properties</h1>
        <p className="text-sm text-[#6a6a6a] mt-1">
          Review and moderate property listings
        </p>
      </div>
      <PropertiesList />
    </div>
  );
}
