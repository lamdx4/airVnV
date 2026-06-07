import { z } from "zod/v4";

export const platformSettingsSchema = z.object({
  platformFeePercent: z
    .number({ message: "Platform fee is required" })
    .min(0, "Must be ≥ 0")
    .max(100, "Must be ≤ 100"),
  minPayoutAmount: z
    .number({ message: "Minimum payout is required" })
    .min(0, "Must be ≥ 0"),
  defaultCurrency: z
    .string()
    .trim()
    .length(3, "Must be a 3-letter ISO code")
    .regex(/^[A-Za-z]{3}$/, "Letters only"),
});

export type PlatformSettingsFormValues = z.infer<typeof platformSettingsSchema>;
