"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "sonner";
import { Pencil, History, Percent } from "lucide-react";

import { PageLoader } from "@/components/common/loading";
import { ErrorDisplay } from "@/components/common/error-display";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

import {
  useCurrentPlatformFee,
  usePlatformFeeHistory,
  useCreatePlatformFee,
} from "../hooks";
import { platformFeeSchema, type PlatformFeeFormData } from "../utils/validation";

export function PlatformFeeSection() {
  const { data: currentFee, isLoading: loadingCurrent, isError: errorCurrent, refetch } = useCurrentPlatformFee();
  const { data: historyData, isLoading: loadingHistory } = usePlatformFeeHistory({ pageSize: 10 });
  const createMutation = useCreatePlatformFee();

  const [isEditing, setIsEditing] = useState(false);
  const [showHistory, setShowHistory] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<PlatformFeeFormData>({
    resolver: zodResolver(platformFeeSchema),
    defaultValues: { feePercentage: currentFee?.feePercentage ?? 10, description: "" },
  });

  function onSubmit(data: PlatformFeeFormData) {
    createMutation.mutate(data, {
      onSuccess: () => {
        toast.success(`Platform fee updated to ${data.feePercentage}%`);
        setIsEditing(false);
        reset({ feePercentage: data.feePercentage, description: "" });
      },
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  if (loadingCurrent) return <PageLoader text="Loading platform fee..." />;
  if (errorCurrent || !currentFee) {
    return <ErrorDisplay message="Failed to load platform fee" onRetry={() => refetch()} />;
  }

  const history = historyData?.items ?? [];

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle className="flex items-center gap-2">
            <Percent className="h-5 w-5" />
            Platform Fee
          </CardTitle>
          <Button
            variant="outline"
            size="sm"
            onClick={() => setIsEditing(!isEditing)}
          >
            <Pencil className="h-4 w-4 mr-2" />
            {isEditing ? "Cancel" : "Edit Fee"}
          </Button>
        </CardHeader>
        <CardContent>
          {isEditing ? (
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="feePercentage">Fee Percentage (%)</Label>
                <Input
                  id="feePercentage"
                  type="number"
                  step="0.01"
                  min="0"
                  max="50"
                  {...register("feePercentage", { valueAsNumber: true })}
                />
                {errors.feePercentage && (
                  <p className="text-sm text-[#c13515]">{errors.feePercentage.message}</p>
                )}
              </div>
              <div className="space-y-2">
                <Label htmlFor="description">Description (optional)</Label>
                <Textarea
                  id="description"
                  placeholder="Reason for the change..."
                  {...register("description")}
                  rows={2}
                />
              </div>
              <div className="flex items-center gap-3">
                <Button type="submit" disabled={createMutation.isPending}>
                  {createMutation.isPending ? "Saving..." : "Save"}
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => {
                    setIsEditing(false);
                    reset({ feePercentage: currentFee.feePercentage, description: "" });
                  }}
                >
                  Cancel
                </Button>
              </div>
              <p className="text-xs text-[#6a6a6a]">
                Previous rate: {currentFee.feePercentage}%. Changes apply to new bookings only.
              </p>
            </form>
          ) : (
            <div className="space-y-3">
              <div className="flex items-center gap-3">
                <span className="text-3xl font-bold">{currentFee.feePercentage}%</span>
                <Badge variant="success">Active</Badge>
              </div>
              <p className="text-sm text-[#6a6a6a]">
                Last changed: {formatDate(currentFee.createdAt)}
              </p>
              {currentFee.previousValue !== null && currentFee.previousValue !== undefined && (
                <p className="text-sm text-[#6a6a6a]">
                  Previous value: {currentFee.previousValue}%
                </p>
              )}
              {currentFee.description && (
                <p className="text-sm text-[#6a6a6a]">
                  {currentFee.description}
                </p>
              )}
            </div>
          )}
        </CardContent>
      </Card>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle className="flex items-center gap-2">
            <History className="h-5 w-5" />
            Fee History
          </CardTitle>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => setShowHistory(!showHistory)}
          >
            {showHistory ? "Hide" : "View History"}
          </Button>
        </CardHeader>
        {showHistory && (
          <CardContent>
            {loadingHistory ? (
              <p className="text-sm text-[#6a6a6a]">Loading history...</p>
            ) : history.length === 0 ? (
              <p className="text-sm text-[#6a6a6a]">No history available.</p>
            ) : (
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Fee %</TableHead>
                    <TableHead>Previous</TableHead>
                    <TableHead>Description</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Date</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {history.map((item) => (
                    <TableRow key={item.id}>
                      <TableCell className="font-medium">{item.feePercentage}%</TableCell>
                      <TableCell>
                        {item.previousValue !== null && item.previousValue !== undefined
                          ? `${item.previousValue}%`
                          : "—"}
                      </TableCell>
                      <TableCell className="max-w-[200px] truncate">
                        {item.description ?? "—"}
                      </TableCell>
                      <TableCell>
                        {item.isActive ? (
                          <Badge variant="success">Active</Badge>
                        ) : (
                          <Badge variant="secondary">Inactive</Badge>
                        )}
                      </TableCell>
                      <TableCell>{formatDate(item.createdAt)}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            )}
          </CardContent>
        )}
      </Card>
    </div>
  );
}

function formatDate(date: string): string {
  return new Intl.DateTimeFormat("en-US", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(date));
}

function getApiErrorMessage(error: unknown): string {
  if (error && typeof error === "object" && "response" in error) {
    const body = (error as { response?: { data?: { message?: string } } }).response?.data;
    if (body?.message) return body.message;
  }
  return "An unexpected error occurred.";
}
