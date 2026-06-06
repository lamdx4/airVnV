export interface PlatformSettings {
  id: string;
  platformFeePercent: number;
  minPayoutAmount: number;
  defaultCurrency: string;
  updatedAt: string;
  updatedBy: string | null;
}

export interface UpdatePlatformSettingsInput {
  platformFeePercent: number;
  minPayoutAmount: number;
  defaultCurrency: string;
}
