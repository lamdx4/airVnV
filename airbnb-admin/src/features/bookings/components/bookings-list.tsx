"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { ColumnDef } from "@tanstack/react-table";
import { CalendarCheck, Eye, Search, X, CheckCircle } from "lucide-react";
import { toast } from "sonner";

import { ROUTES, DEFAULT_PAGE_SIZE } from "@/config/constants";
import { DataTable } from "@/components/common/data-table";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { useBookings, useCancelBooking, useConfirmBooking } from "../hooks";
import { BookingStatus, type Booking, type BookingListParams, type BookingStatusValue } from "../types";
import { getBookingStatusConfig } from "../utils/status";
import { CancelBookingDialog } from "./cancel-booking-dialog";

const STATUS_FILTER_OPTIONS = [
  { value: "all",                         label: "All Statuses" },
  { value: BookingStatus.PENDING,         label: "Pending" },
  { value: BookingStatus.CONFIRMED,       label: "Confirmed" },
  { value: BookingStatus.AWAITING,        label: "Awaiting Approval" },
  { value: BookingStatus.CHECKED_IN,      label: "Checked In" },
  { value: BookingStatus.CHECKED_OUT,     label: "Checked Out" },
  { value: BookingStatus.CANCELLED,       label: "Cancelled" },
  { value: BookingStatus.REFUNDED,        label: "Refunded" },
];

function StatusBadge({ status }: { status: BookingStatusValue }) {
  const config = getBookingStatusConfig(status);
  return <Badge variant={config.variant}>{config.label}</Badge>;
}

function shortId(id: string) {
  return id.slice(0, 8).toUpperCase();
}

function formatDate(date: string): string {
  return new Intl.DateTimeFormat("en-US", { dateStyle: "medium" }).format(new Date(date));
}

function formatCurrency(amount: number, currency: string): string {
  return new Intl.NumberFormat(currency === "VND" ? "vi-VN" : "en-US", {
    style: "currency",
    currency,
    maximumFractionDigits: currency === "VND" ? 0 : 2,
  }).format(amount);
}

function getApiErrorMessage(error: unknown): string {
  if (error && typeof error === "object" && "response" in error) {
    const body = (error as { response?: { data?: { message?: string } } }).response?.data;
    if (body?.message) return body.message;
  }
  if (error instanceof Error) return error.message;
  return "Something went wrong. Please try again.";
}

export function BookingsList() {
  const router = useRouter();

  const [params, setParams] = useState<BookingListParams>({
    page: 1,
    pageSize: DEFAULT_PAGE_SIZE,
  });
  const [statusFilter, setStatusFilter] = useState("all");
  const [searchInput, setSearchInput] = useState("");
  const [cancelTarget, setCancelTarget] = useState<Booking | null>(null);

  const { data, isLoading, isError, refetch } = useBookings(params);
  const cancelMutation = useCancelBooking();
  const confirmMutation = useConfirmBooking();

  const bookings = data?.items ?? [];
  const totalItems = data?.totalItems ?? 0;

  function handleStatusFilter(value: string) {
    setStatusFilter(value);
    setParams((prev) => ({
      ...prev,
      page: 1,
      status: value === "all" ? undefined : (value as BookingStatusValue),
    }));
  }

  function handleSearch() {
    setParams((prev) => ({ ...prev, page: 1, search: searchInput || undefined }));
  }

  function handleConfirm(id: string) {
    confirmMutation.mutate(id, {
      onSuccess: () => toast.success("Booking confirmed"),
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  function handleCancelConfirm(reason: string) {
    if (!cancelTarget) return;
    cancelMutation.mutate(
      { id: cancelTarget.id, reason },
      {
        onSuccess: () => {
          toast.success("Booking cancelled");
          setCancelTarget(null);
        },
        onError: (err) => toast.error(getApiErrorMessage(err)),
      },
    );
  }

  const columns: ColumnDef<Booking>[] = [
    {
      accessorKey: "id",
      header: "Booking ID",
      cell: ({ row }) => (
        <span className="font-mono text-xs text-[#6a6a6a]">{shortId(row.original.id)}</span>
      ),
    },
    {
      accessorKey: "propertyId",
      header: "Property",
      cell: ({ row }) => (
        <span className="font-mono text-xs">{shortId(row.original.propertyId)}</span>
      ),
    },
    {
      accessorKey: "guestId",
      header: "Guest",
      cell: ({ row }) => (
        <span className="font-mono text-xs">{shortId(row.original.guestId)}</span>
      ),
    },
    {
      id: "stay",
      header: "Stay",
      cell: ({ row }) => (
        <div className="text-sm whitespace-nowrap">
          <span>{formatDate(row.original.checkIn)}</span>
          <span className="mx-1 text-[#aaaaaa]">→</span>
          <span>{formatDate(row.original.checkOut)}</span>
        </div>
      ),
    },
    {
      accessorKey: "nightCount",
      header: "Nights",
      cell: ({ row }) => <span className="text-sm">{row.original.nightCount}n</span>,
    },
    {
      accessorKey: "guestCount",
      header: "Guests",
      cell: ({ row }) => <span className="text-sm">{row.original.guestCount}</span>,
    },
    {
      accessorKey: "totalPrice",
      header: "Total",
      cell: ({ row }) => (
        <span className="text-sm font-medium whitespace-nowrap">
          {formatCurrency(row.original.totalPrice, row.original.currencyCode)}
        </span>
      ),
    },
    {
      accessorKey: "status",
      header: "Status",
      cell: ({ row }) => <StatusBadge status={row.original.status} />,
    },
    {
      accessorKey: "createdAt",
      header: "Booked",
      cell: ({ row }) => (
        <span className="text-sm text-[#6a6a6a] whitespace-nowrap">
          {formatDate(row.original.createdAt)}
        </span>
      ),
    },
    {
      id: "actions",
      header: "Actions",
      cell: ({ row }) => {
        const booking = row.original;
        const canConfirm = booking.status === BookingStatus.PENDING || booking.status === BookingStatus.AWAITING;
        const canCancel  = booking.status === BookingStatus.PENDING || booking.status === BookingStatus.CONFIRMED || booking.status === BookingStatus.AWAITING;

        return (
          <div className="flex items-center gap-1">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => router.push(ROUTES.BOOKING_DETAIL(booking.id))}
              title="View detail"
            >
              <Eye className="h-4 w-4" />
            </Button>
            {canConfirm && (
              <Button
                variant="ghost"
                size="sm"
                onClick={() => handleConfirm(booking.id)}
                disabled={confirmMutation.isPending}
                className="text-emerald-600 hover:text-emerald-700 hover:bg-emerald-50"
                title="Confirm booking"
              >
                <CheckCircle className="h-4 w-4" />
              </Button>
            )}
            {canCancel && (
              <Button
                variant="ghost"
                size="sm"
                onClick={() => setCancelTarget(booking)}
                className="text-[#c13515] hover:text-[#b32505] hover:bg-red-50"
                title="Cancel booking"
              >
                <X className="h-4 w-4" />
              </Button>
            )}
          </div>
        );
      },
    },
  ];

  if (isError) {
    return (
      <div className="flex h-[50vh] items-center justify-center gap-4">
        <p className="text-[#6a6a6a]">Failed to load bookings.</p>
        <Button variant="outline" onClick={() => refetch()}>Retry</Button>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex flex-wrap items-center gap-3">
          <Select value={statusFilter} onValueChange={handleStatusFilter}>
            <SelectTrigger className="w-[180px]">
              <SelectValue placeholder="Filter by status" />
            </SelectTrigger>
            <SelectContent>
              {STATUS_FILTER_OPTIONS.map((opt) => (
                <SelectItem key={opt.value} value={opt.value}>{opt.label}</SelectItem>
              ))}
            </SelectContent>
          </Select>

          <div className="flex items-center gap-2">
            <Input
              placeholder="Search by guest or property ID..."
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleSearch()}
              className="w-[260px]"
            />
            <Button variant="outline" size="sm" onClick={handleSearch}>
              <Search className="h-4 w-4" />
            </Button>
          </div>
        </div>

        <div className="flex items-center gap-2 text-sm text-[#6a6a6a]">
          <CalendarCheck className="h-4 w-4" />
          {totalItems} bookings
        </div>
      </div>

      <DataTable
        columns={columns}
        data={bookings}
        totalItems={totalItems}
        pagination={{
          pageIndex: (params.page ?? 1) - 1,
          pageSize: params.pageSize ?? DEFAULT_PAGE_SIZE,
        }}
        onPaginationChange={(p) =>
          setParams((prev) => ({ ...prev, page: p.pageIndex + 1, pageSize: p.pageSize }))
        }
        isLoading={isLoading}
      />

      <CancelBookingDialog
        open={!!cancelTarget}
        onOpenChange={(open) => !open && setCancelTarget(null)}
        bookingId={cancelTarget?.id ?? null}
        onConfirm={handleCancelConfirm}
        isLoading={cancelMutation.isPending}
      />
    </div>
  );
}
