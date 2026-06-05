import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

import { settingsApi } from "../api/settings";
import type { UpdateProfileRequest, ChangePasswordRequest, CreatePlatformFeeRequest } from "../types";

const QUERY_KEYS = {
  PROFILE: ["admin", "settings", "profile"] as const,
  SYSTEM_SETTINGS: ["admin", "settings", "system"] as const,
  PLATFORM_FEE_CURRENT: ["admin", "settings", "platform-fee", "current"] as const,
  PLATFORM_FEE_HISTORY: ["admin", "settings", "platform-fee", "history"] as const,
} as const;

export function useAdminProfile() {
  return useQuery({
    queryKey: QUERY_KEYS.PROFILE,
    queryFn: () => settingsApi.getProfile(),
  });
}

export function useUpdateProfile() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: UpdateProfileRequest) => settingsApi.updateProfile(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.PROFILE });
    },
  });
}

export function useChangePassword() {
  return useMutation({
    mutationFn: (data: ChangePasswordRequest) => settingsApi.changePassword(data),
  });
}

export function useSystemSettings() {
  return useQuery({
    queryKey: QUERY_KEYS.SYSTEM_SETTINGS,
    queryFn: () => settingsApi.getSystemSettings(),
  });
}

export function useUpdateSystemSetting() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ key, value }: { key: string; value: string }) =>
      settingsApi.updateSystemSetting(key, value),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.SYSTEM_SETTINGS });
    },
  });
}

export function useCurrentPlatformFee() {
  return useQuery({
    queryKey: QUERY_KEYS.PLATFORM_FEE_CURRENT,
    queryFn: async () => {
      const response = await settingsApi.getCurrentPlatformFee();
      return response.data as unknown as import("../types").PlatformFeeConfig;
    },
  });
}

export function usePlatformFeeHistory(params?: { page?: number; pageSize?: number }) {
  return useQuery({
    queryKey: [...QUERY_KEYS.PLATFORM_FEE_HISTORY, params],
    queryFn: async () => {
      const response = await settingsApi.getPlatformFeeHistory(params);
      return response.data as unknown as import("@/types/api").PaginatedResponse<import("../types").PlatformFeeHistoryItem>;
    },
  });
}

export function useCreatePlatformFee() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (data: CreatePlatformFeeRequest) => {
      const response = await settingsApi.createPlatformFee(data);
      return response.data as unknown as import("../types").PlatformFeeConfig;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.PLATFORM_FEE_CURRENT });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.PLATFORM_FEE_HISTORY });
    },
  });
}
