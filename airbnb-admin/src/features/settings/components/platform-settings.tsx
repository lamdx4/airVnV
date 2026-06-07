"use client";

import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Save } from "lucide-react";
import { toast } from "sonner";

import { PageLoader } from "@/components/common/loading";
import { ErrorDisplay } from "@/components/common/error-display";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import { usePlatformSettings, useUpdatePlatformSettings } from "../hooks";
import {
  platformSettingsSchema,
  type PlatformSettingsFormValues,
} from "../utils/validation";

function getApiErrorMessage(error: unknown): string {
  if (error && typeof error === "object" && "response" in error) {
    const body = (error as { response?: { data?: { message?: string } } }).response?.data;
    if (body?.message) return body.message;
  }
  return "Failed to save settings. Please try again.";
}

function formatDateTime(iso: string) {
  return new Intl.DateTimeFormat("en-US", { dateStyle: "medium", timeStyle: "short" }).format(
    new Date(iso),
  );
}

export function PlatformSettingsForm() {
  const { data, isLoading, isError, refetch } = usePlatformSettings();
  const update = useUpdatePlatformSettings();

  const form = useForm<PlatformSettingsFormValues>({
    resolver: zodResolver(platformSettingsSchema),
    defaultValues: {
      platformFeePercent: 10,
      minPayoutAmount: 50,
      defaultCurrency: "USD",
    },
  });

  useEffect(() => {
    if (data) {
      form.reset({
        platformFeePercent: data.platformFeePercent,
        minPayoutAmount: data.minPayoutAmount,
        defaultCurrency: data.defaultCurrency,
      });
    }
  }, [data, form]);

  if (isLoading) return <PageLoader text="Loading settings..." />;
  if (isError || !data) {
    return <ErrorDisplay message="Failed to load settings" onRetry={() => refetch()} />;
  }

  function onSubmit(values: PlatformSettingsFormValues) {
    update.mutate(values, {
      onSuccess: () => toast.success("Platform settings updated"),
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  const errors = form.formState.errors;

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Platform Fee Configuration</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-5">
            <div className="grid gap-5 md:grid-cols-2">
              <Field
                label="Platform Fee (%)"
                hint="Percentage charged on every guest pay-in (0–100)."
                error={errors.platformFeePercent?.message}
              >
                <Input
                  type="number"
                  step="0.01"
                  min={0}
                  max={100}
                  {...form.register("platformFeePercent", { valueAsNumber: true })}
                />
              </Field>

              <Field
                label="Minimum Payout Amount"
                hint="Hosts can only request a payout above this threshold."
                error={errors.minPayoutAmount?.message}
              >
                <Input
                  type="number"
                  step="0.01"
                  min={0}
                  {...form.register("minPayoutAmount", { valueAsNumber: true })}
                />
              </Field>

              <Field
                label="Default Currency"
                hint="3-letter ISO code (e.g. USD, VND, EUR)."
                error={errors.defaultCurrency?.message}
              >
                <Input
                  type="text"
                  maxLength={3}
                  className="uppercase"
                  {...form.register("defaultCurrency")}
                />
              </Field>
            </div>

            <div className="flex items-center justify-between border-t border-[#dddddd] pt-4">
              <p className="text-xs text-[#6a6a6a]">
                Last updated {formatDateTime(data.updatedAt)}
                {data.updatedBy && <> by {data.updatedBy}</>}
              </p>
              <Button type="submit" disabled={update.isPending || !form.formState.isDirty}>
                <Save className="mr-2 h-4 w-4" />
                {update.isPending ? "Saving..." : "Save Changes"}
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}

function Field({
  label,
  hint,
  error,
  children,
}: {
  label: string;
  hint?: string;
  error?: string;
  children: React.ReactNode;
}) {
  return (
    <div className="space-y-1.5">
      <Label className="text-sm font-medium text-[#222222]">{label}</Label>
      {children}
      {hint && !error && <p className="text-xs text-[#6a6a6a]">{hint}</p>}
      {error && <p className="text-xs text-[#c13515]">{error}</p>}
    </div>
  );
}
