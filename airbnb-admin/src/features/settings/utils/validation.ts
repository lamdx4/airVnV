import { z } from "zod/v4";

export const updateProfileSchema = z.object({
  fullName: z.string().min(2, "Full name must be at least 2 characters"),
  phone: z.string().optional(),
});

export const changePasswordSchema = z
  .object({
    currentPassword: z.string().min(1, "Current password is required"),
    newPassword: z.string().min(8, "New password must be at least 8 characters"),
    confirmPassword: z.string().min(1, "Please confirm your password"),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  });

export const platformFeeSchema = z.object({
  feePercentage: z
    .number({ message: "Fee percentage is required" })
    .min(0, "Fee percentage must be between 0% and 50%")
    .max(50, "Fee percentage must be between 0% and 50%")
    .refine((val) => Math.round(val * 100) / 100 === val, {
      message: "Maximum 2 decimal places allowed",
    }),
  description: z.string().optional(),
});

export type UpdateProfileFormData = z.infer<typeof updateProfileSchema>;
export type ChangePasswordFormData = z.infer<typeof changePasswordSchema>;
export type PlatformFeeFormData = z.infer<typeof platformFeeSchema>;
