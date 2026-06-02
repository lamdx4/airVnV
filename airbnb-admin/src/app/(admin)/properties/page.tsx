import { PropertiesList } from "@/features/properties";

export default function PropertiesPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">Properties</h1>
        <p className="text-sm text-muted-foreground">
          Review and moderate property listings
        </p>
      </div>
      <PropertiesList />
    </div>
  );
}
