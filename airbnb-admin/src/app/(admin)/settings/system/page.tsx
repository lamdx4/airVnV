import { PlatformFeeSection } from "@/features/settings";

export default function SystemSettingsPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-[28px] font-bold text-[#222222]">System Settings</h1>
        <p className="text-sm text-[#6a6a6a]">
          Configure platform-wide settings
        </p>
      </div>
      <PlatformFeeSection />
    </div>
  );
}
