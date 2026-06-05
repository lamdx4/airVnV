"use client";

import { useForm } from "react-hook-form";

import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogCancel,
} from "@/components/ui/alert-dialog";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";

import { rejectPropertySchema, type RejectPropertyForm } from "../utils/validation";
import type { Property } from "../types";

interface RejectPropertyDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  property: Property | null;
  onConfirm: (reason: string) => void;
  isLoading?: boolean;
}

export function RejectPropertyDialog({
  open,
  onOpenChange,
  property,
  onConfirm,
  isLoading,
}: RejectPropertyDialogProps) {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isValid },
  } = useForm<RejectPropertyForm>({
    defaultValues: { reason: "" },
    mode: "onChange",
  });

  function onSubmit(data: RejectPropertyForm) {
    onConfirm(data.reason);
  }

  function handleOpenChange(open: boolean) {
    if (!open) {
      reset();
    }
    onOpenChange(open);
  }

  return (
    <AlertDialog open={open} onOpenChange={handleOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Reject Property</AlertDialogTitle>
          <AlertDialogDescription>
            Reject &ldquo;{property?.title}&rdquo; with a reason. The Host will be notified.
          </AlertDialogDescription>
        </AlertDialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-3">
          <div className="space-y-2">
            <Label htmlFor="reject-reason">Rejection Reason</Label>
            <Textarea
              id="reject-reason"
              placeholder="Explain why this listing is being rejected..."
              {...register("reason", {
                validate: (value) => {
                  const result = rejectPropertySchema.shape.reason.safeParse(value);
                  return result.success || result.error?.issues[0]?.message;
                },
              })}
              aria-invalid={!!errors.reason}
              className={errors.reason ? "border-[#c13515]" : ""}
              rows={4}
            />
            {errors.reason && (
              <p className="text-xs text-[#c13515]">{errors.reason.message}</p>
            )}
          </div>

          <AlertDialogFooter>
            <AlertDialogCancel disabled={isLoading}>Cancel</AlertDialogCancel>
            <Button
              type="submit"
              variant="destructive"
              disabled={!isValid || isLoading}
            >
              {isLoading ? "Processing..." : "Confirm Reject"}
            </Button>
          </AlertDialogFooter>
        </form>
      </AlertDialogContent>
    </AlertDialog>
  );
}
