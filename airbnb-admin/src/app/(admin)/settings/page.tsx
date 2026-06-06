import { PlatformSettingsForm } from "@/features/settings";

export default function SettingsPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-[28px] font-bold text-[#222222]">Settings</h1>
        <p className="mt-1 text-sm text-[#6a6a6a]">
          Configure platform-wide payment policies.
        </p>
      </div>
      <PlatformSettingsForm />
    </div>
  );
}
