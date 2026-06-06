"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import {
  ArrowLeft,
  CheckCircle2,
  X,
  CalendarDays,
  Users,
  DollarSign,
  MapPin,
  Clock,
  Hash,
} from "lucide-react";

import { ROUTES } from "@/config/constants";
import { Breadcrumbs, type BreadcrumbItem } from "@/components/layout/breadcrumbs";
import { PageLoader } from "@/components/common/loading";
import { ErrorDisplay } from "@/components/common/error-display";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

import { useBooking, useCancelBooking, useConfirmBooking } from "../hooks";
import { BookingStatus } from "../types";
import { getBookingStatusConfig } from "../utils/status";
import { CancelBookingDialog } from "./cancel-booking-dialog";

interface BookingDetailProps {
  bookingId: string;
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

export function BookingDetail({ bookingId }: BookingDetailProps) {
  const router = useRouter();
  const { data: booking, isLoading, isError, refetch } = useBooking(bookingId);
  const cancelMutation = useCancelBooking();
  const confirmMutation = useConfirmBooking();
  const [showCancelDialog, setShowCancelDialog] = useState(false);

  if (isLoading) return <PageLoader text="Loading booking..." />;
  if (isError || !booking) {
    return <ErrorDisplay message="Booking not found." onRetry={() => refetch()} />;
  }

  const statusConfig = getBookingStatusConfig(booking.status);
  const canConfirm = booking.status === BookingStatus.PENDING || booking.status === BookingStatus.AWAITING;
  const canCancel  = booking.status === BookingStatus.PENDING || booking.status === BookingStatus.CONFIRMED || booking.status === BookingStatus.AWAITING;

  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Bookings", href: ROUTES.BOOKINGS },
    { label: `#${bookingId.slice(0, 8).toUpperCase()}` },
  ];

  function handleConfirm() {
    confirmMutation.mutate(bookingId, {
      onSuccess: () => toast.success("Booking confirmed"),
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  function handleCancelConfirm(reason: string) {
    cancelMutation.mutate(
      { id: bookingId, reason },
      {
        onSuccess: () => { toast.success("Booking cancelled"); setShowCancelDialog(false); },
        onError: (err) => toast.error(getApiErrorMessage(err)),
      },
    );
  }

  return (
    <div className="space-y-6">
      <div className="space-y-2">
        <Breadcrumbs items={breadcrumbs} />
        <div className="flex items-center gap-3">
          <Button variant="ghost" size="sm" onClick={() => router.push(ROUTES.BOOKINGS)}>
            <ArrowLeft className="h-4 w-4 mr-1" />Back
          </Button>
          <h1 className="text-[28px] font-bold text-[#222222]">
            Booking #{bookingId.slice(0, 8).toUpperCase()}
          </h1>
          <Badge variant={statusConfig.variant}>{statusConfig.label}</Badge>
        </div>
        <p className="text-sm text-[#6a6a6a]">Created {formatDate(booking.createdAt)}</p>
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        {/* Stay */}
        <Card>
          <CardHeader><CardTitle>Stay Details</CardTitle></CardHeader>
          <CardContent className="space-y-3">
            <Row icon={Hash}        label="Property ID"  value={booking.propertyId} mono />
            <Row icon={CalendarDays} label="Check-in"    value={formatDate(booking.checkIn)} />
            <Row icon={CalendarDays} label="Check-out"   value={formatDate(booking.checkOut)} />
            <Row icon={Clock}        label="Nights"      value={String(booking.nightCount)} />
            <Row icon={Users}        label="Guests"      value={String(booking.guestCount)} />
            <Row icon={MapPin}       label="Country"     value={booking.countryCode} />
          </CardContent>
        </Card>

        {/* Pricing */}
        <Card>
          <CardHeader><CardTitle>Pricing Breakdown</CardTitle></CardHeader>
          <CardContent className="space-y-3">
            <Row icon={DollarSign} label="Base/Night"    value={formatCurrency(booking.basePricePerNight, booking.currencyCode)} />
            <Row                   label="Cleaning Fee"  value={formatCurrency(booking.cleaningFee, booking.currencyCode)} />
            <Row                   label="Service Fee"   value={formatCurrency(booking.serviceFee, booking.currencyCode)} />
            <Row                   label="Tax"           value={formatCurrency(booking.taxAmount, booking.currencyCode)} />
            <div className="border-t border-[#dddddd] pt-3">
              <Row icon={DollarSign} label="Total" value={formatCurrency(booking.totalPrice, booking.currencyCode)} bold />
            </div>
          </CardContent>
        </Card>

        {/* Parties */}
        <Card>
          <CardHeader><CardTitle>Parties Involved</CardTitle></CardHeader>
          <CardContent className="space-y-3">
            <div>
              <p className="text-xs font-medium uppercase tracking-wider text-[#aaaaaa] mb-2">Guest</p>
              <Row icon={Hash} label="Guest ID" value={booking.guestId} mono />
            </div>
            <div className="border-t border-[#dddddd] pt-3">
              <p className="text-xs font-medium uppercase tracking-wider text-[#aaaaaa] mb-2">Host</p>
              <Row icon={Hash} label="Host ID" value={booking.hostId} mono />
            </div>
          </CardContent>
        </Card>

        {/* Meta */}
        <Card>
          <CardHeader><CardTitle>Booking Info</CardTitle></CardHeader>
          <CardContent className="space-y-3">
            <Row icon={Hash} label="Booking ID"    value={booking.id} mono />
            <Row             label="Mode"          value={booking.bookingMode} />
            <Row             label="Currency"      value={booking.currencyCode} />
          </CardContent>
        </Card>
      </div>

      {(canConfirm || canCancel) && (
        <Card>
          <CardHeader><CardTitle>Actions</CardTitle></CardHeader>
          <CardContent>
            <div className="flex flex-wrap gap-3">
              {canConfirm && (
                <Button
                  onClick={handleConfirm}
                  disabled={confirmMutation.isPending}
                  className="bg-emerald-600 hover:bg-emerald-700 text-white"
                >
                  <CheckCircle2 className="h-4 w-4 mr-2" />
                  {confirmMutation.isPending ? "Confirming..." : "Confirm Booking"}
                </Button>
              )}
              {canCancel && (
                <Button
                  variant="outline"
                  onClick={() => setShowCancelDialog(true)}
                  className="text-[#c13515] border-[#c13515] hover:bg-red-50"
                >
                  <X className="h-4 w-4 mr-2" />Cancel Booking
                </Button>
              )}
            </div>
          </CardContent>
        </Card>
      )}

      <CancelBookingDialog
        open={showCancelDialog}
        onOpenChange={setShowCancelDialog}
        bookingId={bookingId}
        onConfirm={handleCancelConfirm}
        isLoading={cancelMutation.isPending}
      />
    </div>
  );
}

function Row({
  icon: Icon,
  label,
  value,
  mono,
  bold,
}: {
  icon?: React.ComponentType<{ className?: string }>;
  label: string;
  value: string;
  mono?: boolean;
  bold?: boolean;
}) {
  return (
    <div className="flex items-center justify-between text-sm py-0.5">
      <span className="flex items-center gap-2 text-[#6a6a6a] shrink-0">
        {Icon && <Icon className="h-4 w-4" />}
        {label}
      </span>
      <span className={`text-right max-w-[240px] truncate ${mono ? "font-mono text-xs" : ""} ${bold ? "font-semibold" : "font-medium"}`}>
        {value}
      </span>
    </div>
  );
}
