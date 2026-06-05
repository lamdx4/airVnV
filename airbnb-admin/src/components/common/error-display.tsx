"use client";

import { AlertCircle } from "lucide-react";

import { Button } from "@/components/ui/button";

interface ErrorDisplayProps {
  message?: string;
  onRetry?: () => void;
}

export function ErrorDisplay({ message = "Something went wrong", onRetry }: ErrorDisplayProps) {
  return (
    <div className="flex h-[30vh] flex-col items-center justify-center gap-4">
      <AlertCircle className="h-10 w-10 text-[#c13515]" />
      <p className="text-sm text-[#6a6a6a]">{message}</p>
      {onRetry && (
        <Button variant="outline" onClick={onRetry}>
          Try again
        </Button>
      )}
    </div>
  );
}
