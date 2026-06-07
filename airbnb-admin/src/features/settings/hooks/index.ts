import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

import { settingsApi } from "../api/settings";
import type { UpdatePlatformSettingsInput } from "../types";

const QUERY_KEYS = {
  PLATFORM: ["admin", "settings", "platform"] as const,
} as const;

export function usePlatformSettings() {
  return useQuery({
    queryKey: QUERY_KEYS.PLATFORM,
    queryFn: async () => (await settingsApi.getPlatform()).data,
  });
}

export function useUpdatePlatformSettings() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (input: UpdatePlatformSettingsInput) =>
      (await settingsApi.updatePlatform(input)).data,
    onSuccess: () => qc.invalidateQueries({ queryKey: QUERY_KEYS.PLATFORM }),
  });
}
