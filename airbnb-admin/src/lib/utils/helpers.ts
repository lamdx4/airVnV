import { ZodError } from "zod";

import type { ApiResponse } from "@/types/api";

export function getErrorMessage(error: unknown): string {
  if (error instanceof ZodError) {
    return error.issues.map((e) => `${e.path.join(".")}: ${e.message}`).join(", ");
  }
  if (error instanceof Error) {
    return error.message;
  }
  if (typeof error === "string") {
    return error;
  }
  return "An unexpected error occurred";
}

export function getApiErrorMessage(error: unknown): string {
  if (error && typeof error === "object" && "response" in error) {
    const body = (error as { response?: { data?: ApiResponse<unknown> } }).response?.data;
    if (body?.message) return body.message;
    if (body?.errors) {
      return Object.values(body.errors).flat().join(", ");
    }
  }
  return getErrorMessage(error);
}

export function formatCurrency(amount: number, currency = "USD"): string {
  return new Intl.NumberFormat("en-US", { style: "currency", currency }).format(amount);
}

export function formatDate(date: string | Date, options?: Intl.DateTimeFormatOptions): string {
  return new Intl.DateTimeFormat("en-US", {
    dateStyle: "medium",
    ...options,
  }).format(new Date(date));
}

export function classNames(...classes: (string | false | null | undefined)[]): string {
  return classes.filter(Boolean).join(" ");
}
