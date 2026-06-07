import { api } from "@/lib/api";

import type { PlatformSettings, UpdatePlatformSettingsInput } from "../types";

export const settingsApi = {
  getPlatform: () => api.get<PlatformSettings>("/admin/settings/platform"),

  updatePlatform: (input: UpdatePlatformSettingsInput) =>
    api.put<PlatformSettings>("/admin/settings/platform", input),
};
