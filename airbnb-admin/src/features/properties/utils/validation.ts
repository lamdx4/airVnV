import { z } from "zod/v4";

export const rejectPropertySchema = z.object({
  reason: z.string().min(10, "Rejection reason must be at least 10 characters"),
});

export type RejectPropertyForm = z.infer<typeof rejectPropertySchema>;
