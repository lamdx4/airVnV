"use client";

import { useState, useEffect, useCallback } from "react";
import { useRouter } from "next/navigation";
import Image from "next/image";
import { toast } from "sonner";
import {
  ArrowLeft,
  CheckCircle2,
  XCircle,
  Ban,
  Trash2,
  RotateCcw,
  MapPin,
  Users,
  BedDouble,
  Bath,
  Star,
  DollarSign,
  Home,
  Grid3x3,
  X,
  ChevronLeft,
  ChevronRight,
  ImageIcon,
} from "lucide-react";

import { ROUTES } from "@/config/constants";
import { Breadcrumbs, type BreadcrumbItem } from "@/components/layout/breadcrumbs";
import { PageLoader } from "@/components/common/loading";
import { ErrorDisplay } from "@/components/common/error-display";
import { ConfirmDialog } from "@/components/common/confirm-dialog";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import {
  useProperty,
  useApproveProperty,
  useRejectProperty,
  useSuspendProperty,
  useReinstateProperty,
  useAdminDeleteProperty,
} from "../hooks";
import { PropertyStatus, PropertyTypeEnum } from "../types";
import { getPropertyStatusConfig } from "../utils/status";
import { RejectPropertyDialog } from "./reject-property-dialog";

interface PropertyDetailProps {
  propertyId: string;
}

export function PropertyDetail({ propertyId }: PropertyDetailProps) {
  const router = useRouter();
  const {
    data: property,
    isLoading,
    isError,
    refetch,
  } = useProperty(propertyId);

  const approveMutation = useApproveProperty();
  const rejectMutation = useRejectProperty();
  const suspendMutation = useSuspendProperty();
  const reinstateMutation = useReinstateProperty();
  const deleteMutation = useAdminDeleteProperty();

  const [showRejectDialog, setShowRejectDialog] = useState(false);
  const [showSuspendDialog, setShowSuspendDialog] = useState(false);
  const [showReinstateDialog, setShowReinstateDialog] = useState(false);
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const [deleteConfirmTitle, setDeleteConfirmTitle] = useState("");
  const [lightboxIndex, setLightboxIndex] = useState<number | null>(null);

  if (isLoading) return <PageLoader text="Loading property..." />;
  if (isError || !property) {
    return <ErrorDisplay message="Property not found" onRetry={() => refetch()} />;
  }

  const statusConfig = getPropertyStatusConfig(property.status);
  const canApprove =
    property.status === PropertyStatus.PENDING_REVIEW ||
    property.status === PropertyStatus.REJECTED;
  const canReject = property.status === PropertyStatus.PENDING_REVIEW;
  const canSuspend = property.status === PropertyStatus.PUBLISHED;
  const canReinstate = property.status === PropertyStatus.SUSPENDED;
  const canDelete =
    property.status === PropertyStatus.PUBLISHED ||
    property.status === PropertyStatus.SUSPENDED;
  const titleMatchesDelete = deleteConfirmTitle === property.title;

  const breadcrumbs: BreadcrumbItem[] = [
    { label: "Properties", href: ROUTES.PROPERTIES },
    { label: property.title },
  ];

  // Build gallery images: sorted by displayOrder, fallback to coverImageUrl
  const sortedImages = [...property.images].sort((a, b) => a.displayOrder - b.displayOrder);
  const galleryImages =
    sortedImages.length > 0
      ? sortedImages
      : property.coverImageUrl
        ? [{ url: property.coverImageUrl, displayOrder: 0 }]
        : [];

  // Group amenities by category
  const amenitiesByCategory = property.amenities.reduce<Record<string, string[]>>((acc, am) => {
    if (!acc[am.category]) acc[am.category] = [];
    acc[am.category].push(am.name);
    return acc;
  }, {});

  function handleApprove() {
    approveMutation.mutate(propertyId, {
      onSuccess: () => toast.success("Property approved successfully"),
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  function handleReject(reason: string) {
    rejectMutation.mutate(
      { id: propertyId, reason },
      {
        onSuccess: () => {
          toast.success("Property rejected");
          setShowRejectDialog(false);
        },
        onError: (err) => toast.error(getApiErrorMessage(err)),
      },
    );
  }

  function handleSuspend() {
    suspendMutation.mutate({ id: propertyId }, {
      onSuccess: () => {
        toast.success("Property suspended successfully");
        setShowSuspendDialog(false);
      },
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  function handleReinstate() {
    reinstateMutation.mutate(propertyId, {
      onSuccess: () => {
        toast.success("Property reinstated successfully");
        setShowReinstateDialog(false);
      },
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  function handleDelete() {
    deleteMutation.mutate(propertyId, {
      onSuccess: () => {
        toast.success("Property deleted");
        router.push(ROUTES.PROPERTIES);
      },
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="space-y-2">
          <Breadcrumbs items={breadcrumbs} />
          <div className="flex items-center gap-3">
            <Button variant="ghost" size="sm" onClick={() => router.push(ROUTES.PROPERTIES)}>
              <ArrowLeft className="h-4 w-4 mr-1" />
              Back
            </Button>
            <h1 className="text-[28px] font-bold text-[#222222]">{property.title}</h1>
            <Badge variant={statusConfig.variant}>{statusConfig.label}</Badge>
          </div>
          <p className="text-sm text-[#6a6a6a]">
            Created {formatDate(property.createdAt)}
            {property.updatedAt && <> &middot; Updated {formatDate(property.updatedAt)}</>}
          </p>
        </div>
      </div>

      {/* Row 1: Image Gallery */}
      {galleryImages.length > 0 ? (
        <PropertyGallery
          images={galleryImages}
          title={property.title}
          onOpenLightbox={setLightboxIndex}
        />
      ) : (
        <div className="flex h-[280px] items-center justify-center rounded-[20px] border border-dashed border-[#dddddd] bg-[#f7f7f7] text-[#6a6a6a]">
          <div className="flex flex-col items-center gap-2 text-sm">
            <ImageIcon className="h-8 w-8" />
            <span>No images uploaded for this listing</span>
          </div>
        </div>
      )}

      {lightboxIndex !== null && (
        <Lightbox
          images={galleryImages}
          startIndex={lightboxIndex}
          title={property.title}
          onClose={() => setLightboxIndex(null)}
        />
      )}

      {/* Row 2: Property Info + Host & Rating */}
      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Property Information</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <DetailRow icon={MapPin} label="Location" value={property.displayAddress} />
            <DetailRow label="Type" value={PropertyTypeEnum[property.type] ?? "Unknown"} />
            <DetailRow icon={DollarSign} label="Price per Night" value={formatCurrency(property.basePrice)} />
            <DetailRow icon={Users} label="Max Guests" value={String(property.guestCount)} />
            <DetailRow icon={BedDouble} label="Bedrooms" value={String(property.bedroomCount)} />
            <DetailRow icon={Bath} label="Bathrooms" value={String(property.bathroomCount)} />
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Host & Rating</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <DetailRow label="Host ID" value={property.hostId} />
            <div className="flex items-center gap-2">
              <Star className="h-4 w-4 text-amber-500" />
              <span className="text-sm font-medium">
                {property.averageRating > 0 ? property.averageRating.toFixed(1) : "No reviews yet"}
              </span>
              {property.reviewCount > 0 && (
                <span className="text-sm text-[#6a6a6a]">
                  ({property.reviewCount} reviews)
                </span>
              )}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Row 3: Description */}
      {property.description && (
        <Card>
          <CardHeader>
            <CardTitle>Description</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-sm leading-relaxed text-[#222222] whitespace-pre-line">
              {property.description}
            </p>
          </CardContent>
        </Card>
      )}

      {/* Row 4: Amenities */}
      {property.amenities.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle>Amenities</CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            {Object.entries(amenitiesByCategory).map(([category, names]) => (
              <div key={category}>
                <h4 className="text-sm font-semibold text-[#222222] mb-2">{category}</h4>
                <div className="flex flex-wrap gap-2">
                  {names.map((name) => (
                    <Badge key={name} variant="secondary" className="font-normal">
                      {name}
                    </Badge>
                  ))}
                </div>
              </div>
            ))}
          </CardContent>
        </Card>
      )}

      {/* Row 5: Address Details */}
      <Card>
        <CardHeader>
          <CardTitle>Address Details</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <DetailRow icon={MapPin} label="Display Address" value={property.displayAddress} />
          <DetailRow icon={MapPin} label="Street Address" value={property.streetAddress} />
        </CardContent>
      </Card>

      {/* Status reason cards */}
      {property.status === PropertyStatus.REJECTED && property.rejectionReason && (
        <Card>
          <CardHeader>
            <CardTitle className="text-[#c13515]">Rejection Reason</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-sm">{property.rejectionReason}</p>
          </CardContent>
        </Card>
      )}

      {property.status === PropertyStatus.SUSPENDED && property.suspensionReason && (
        <Card>
          <CardHeader>
            <CardTitle className="text-[#6a6a6a]">Suspension Reason</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-sm">{property.suspensionReason}</p>
          </CardContent>
        </Card>
      )}

      {/* Moderation Actions */}
      <Card>
        <CardHeader>
          <CardTitle>Moderation Actions</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex flex-wrap items-center gap-3">
            <Button
              variant="default"
              onClick={handleApprove}
              disabled={!canApprove || approveMutation.isPending}
            >
              <CheckCircle2 className="h-4 w-4 mr-2" />
              {approveMutation.isPending ? "Approving..." : "Approve"}
            </Button>
            <Button
              variant="outline"
              onClick={() => setShowRejectDialog(true)}
              disabled={!canReject}
            >
              <XCircle className="h-4 w-4 mr-2" />
              Reject
            </Button>
            <Button
              variant="outline"
              onClick={() => setShowSuspendDialog(true)}
              disabled={!canSuspend}
            >
              <Ban className="h-4 w-4 mr-2" />
              Suspend
            </Button>
            <Button
              variant="outline"
              onClick={() => setShowReinstateDialog(true)}
              disabled={!canReinstate}
            >
              <RotateCcw className="h-4 w-4 mr-2" />
              Reinstate
            </Button>
            <Button
              variant="destructive"
              onClick={() => setShowDeleteDialog(true)}
              disabled={!canDelete}
            >
              <Trash2 className="h-4 w-4 mr-2" />
              Delete
            </Button>
          </div>
        </CardContent>
      </Card>

      <RejectPropertyDialog
        open={showRejectDialog}
        onOpenChange={setShowRejectDialog}
        property={property}
        onConfirm={handleReject}
        isLoading={rejectMutation.isPending}
      />

      <ConfirmDialog
        open={showSuspendDialog}
        onOpenChange={setShowSuspendDialog}
        title="Suspend Listing"
        description="Are you sure you want to suspend this listing? It will be hidden from search results immediately."
        confirmLabel="Suspend"
        onConfirm={handleSuspend}
        isLoading={suspendMutation.isPending}
      />

      <ConfirmDialog
        open={showReinstateDialog}
        onOpenChange={setShowReinstateDialog}
        title="Reinstate Listing"
        description="Are you sure you want to reinstate this listing? It will become visible in search results again."
        confirmLabel="Reinstate"
        onConfirm={handleReinstate}
        isLoading={reinstateMutation.isPending}
      />

      <AlertDialog open={showDeleteDialog} onOpenChange={(open) => { setShowDeleteDialog(open); if (!open) setDeleteConfirmTitle(""); }}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Listing</AlertDialogTitle>
            <AlertDialogDescription>
              This action cannot be undone. Please type the property title to confirm.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <div className="space-y-2">
            <Label htmlFor="delete-confirm" className="text-sm text-[#6a6a6a]">
              Type &ldquo;{property.title}&rdquo; to confirm
            </Label>
            <Input
              id="delete-confirm"
              value={deleteConfirmTitle}
              onChange={(e) => setDeleteConfirmTitle(e.target.value)}
              placeholder={property.title}
            />
          </div>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={deleteMutation.isPending}>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDelete}
              disabled={!titleMatchesDelete || deleteMutation.isPending}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {deleteMutation.isPending ? "Processing..." : "Confirm Delete"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}

type GalleryImage = { url: string; displayOrder: number };

function PropertyGallery({
  images,
  title,
  onOpenLightbox,
}: {
  images: GalleryImage[];
  title: string;
  onOpenLightbox: (index: number) => void;
}) {
  const cover = images[0];
  const thumbs = images.slice(1, 5);
  const remaining = Math.max(0, images.length - 5);

  return (
    <div className="relative">
      <div className="grid h-[420px] grid-cols-1 gap-2 overflow-hidden rounded-[20px] md:grid-cols-4 md:grid-rows-2">
        {/* Cover */}
        <button
          type="button"
          onClick={() => onOpenLightbox(0)}
          className="group relative col-span-1 row-span-1 overflow-hidden md:col-span-2 md:row-span-2"
          aria-label="Open cover image"
        >
          <Image
            src={cover.url}
            alt={`${title} cover`}
            fill
            className="object-cover transition-transform duration-500 ease-out group-hover:scale-[1.03]"
            sizes="(max-width: 768px) 100vw, 50vw"
            priority
          />
          <div className="absolute inset-0 bg-black/0 transition-colors duration-300 group-hover:bg-black/10" />
          <Badge className="absolute left-4 top-4 bg-white/95 font-medium text-[#222222] shadow-sm hover:bg-white">
            Cover
          </Badge>
        </button>

        {/* Thumbnails */}
        {thumbs.map((img, idx) => {
          const realIdx = idx + 1;
          const isLastVisible = idx === thumbs.length - 1 && remaining > 0;
          return (
            <button
              key={realIdx}
              type="button"
              onClick={() => onOpenLightbox(realIdx)}
              className="group relative hidden overflow-hidden md:block"
              aria-label={`Open image ${realIdx + 1}`}
            >
              <Image
                src={img.url}
                alt={`${title} ${realIdx + 1}`}
                fill
                className="object-cover transition-transform duration-500 ease-out group-hover:scale-[1.05]"
                sizes="25vw"
              />
              <div className="absolute inset-0 bg-black/0 transition-colors duration-300 group-hover:bg-black/10" />
              {isLastVisible && (
                <div className="absolute inset-0 flex items-center justify-center bg-black/45 text-white">
                  <span className="text-lg font-semibold">+{remaining}</span>
                </div>
              )}
            </button>
          );
        })}

        {/* Fill empty cells with subtle placeholders so layout stays clean */}
        {Array.from({ length: Math.max(0, 4 - thumbs.length) }).map((_, i) => (
          <div
            key={`ph-${i}`}
            className="hidden bg-[#f7f7f7] md:block"
            aria-hidden
          />
        ))}
      </div>

      {/* Count pill (mobile-visible) */}
      <div className="pointer-events-none absolute left-4 top-4 md:hidden">
        <Badge className="bg-white/95 font-medium text-[#222222] shadow-sm">
          1 / {images.length}
        </Badge>
      </div>

      {/* Show all photos */}
      <Button
        variant="outline"
        size="sm"
        onClick={() => onOpenLightbox(0)}
        className="absolute bottom-4 right-4 h-9 rounded-[10px] border-[#222222] bg-white text-[13px] font-semibold text-[#222222] shadow-sm hover:bg-white hover:shadow-md"
      >
        <Grid3x3 className="mr-2 h-4 w-4" />
        Show all photos ({images.length})
      </Button>
    </div>
  );
}

function Lightbox({
  images,
  startIndex,
  title,
  onClose,
}: {
  images: GalleryImage[];
  startIndex: number;
  title: string;
  onClose: () => void;
}) {
  const [index, setIndex] = useState(startIndex);

  const next = useCallback(
    () => setIndex((i) => (i + 1) % images.length),
    [images.length],
  );
  const prev = useCallback(
    () => setIndex((i) => (i - 1 + images.length) % images.length),
    [images.length],
  );

  useEffect(() => {
    function onKey(e: KeyboardEvent) {
      if (e.key === "Escape") onClose();
      else if (e.key === "ArrowRight") next();
      else if (e.key === "ArrowLeft") prev();
    }
    document.addEventListener("keydown", onKey);
    const prevOverflow = document.body.style.overflow;
    document.body.style.overflow = "hidden";
    return () => {
      document.removeEventListener("keydown", onKey);
      document.body.style.overflow = prevOverflow;
    };
  }, [next, prev, onClose]);

  const current = images[index];

  return (
    <div
      className="fixed inset-0 z-50 flex flex-col bg-black/95 backdrop-blur-sm"
      role="dialog"
      aria-modal="true"
      aria-label="Image gallery"
    >
      {/* Top bar */}
      <div className="flex items-center justify-between px-4 py-3 text-white md:px-6">
        <button
          type="button"
          onClick={onClose}
          className="flex h-10 w-10 items-center justify-center rounded-full hover:bg-white/10"
          aria-label="Close gallery"
        >
          <X className="h-5 w-5" />
        </button>
        <span className="text-sm font-medium tabular-nums">
          {index + 1} / {images.length}
        </span>
        <div className="w-10" />
      </div>

      {/* Main image */}
      <div className="relative flex flex-1 items-center justify-center px-4 pb-4">
        <button
          type="button"
          onClick={prev}
          className="absolute left-4 z-10 flex h-11 w-11 items-center justify-center rounded-full bg-white/90 text-[#222222] shadow-md transition hover:bg-white md:left-8"
          aria-label="Previous image"
        >
          <ChevronLeft className="h-5 w-5" />
        </button>

        <div className="relative h-full w-full max-w-5xl">
          <Image
            key={current.url}
            src={current.url}
            alt={`${title} ${index + 1}`}
            fill
            className="object-contain"
            sizes="100vw"
            priority
          />
        </div>

        <button
          type="button"
          onClick={next}
          className="absolute right-4 z-10 flex h-11 w-11 items-center justify-center rounded-full bg-white/90 text-[#222222] shadow-md transition hover:bg-white md:right-8"
          aria-label="Next image"
        >
          <ChevronRight className="h-5 w-5" />
        </button>
      </div>

      {/* Thumbnail strip */}
      <div className="border-t border-white/10 bg-black/60 px-4 py-3">
        <div className="mx-auto flex max-w-5xl gap-2 overflow-x-auto">
          {images.map((img, i) => (
            <button
              key={i}
              type="button"
              onClick={() => setIndex(i)}
              className={`relative h-16 w-24 flex-shrink-0 overflow-hidden rounded-[8px] transition ${
                i === index
                  ? "ring-2 ring-white"
                  : "opacity-60 hover:opacity-100"
              }`}
              aria-label={`Go to image ${i + 1}`}
            >
              <Image
                src={img.url}
                alt=""
                fill
                className="object-cover"
                sizes="96px"
              />
            </button>
          ))}
        </div>
      </div>
    </div>
  );
}

function DetailRow({
  icon: Icon,
  label,
  value,
}: {
  icon?: React.ComponentType<{ className?: string }>;
  label: string;
  value: string;
}) {
  return (
    <div className="flex items-center justify-between text-sm">
      <span className="flex items-center gap-2 text-[#6a6a6a]">
        {Icon && <Icon className="h-4 w-4" />}
        {label}
      </span>
      <span className="font-medium">{value}</span>
    </div>
  );
}

function getApiErrorMessage(error: unknown): string {
  if (error && typeof error === "object" && "response" in error) {
    const body = (error as { response?: { data?: { message?: string } } }).response?.data;
    if (body?.message) return body.message;
  }
  return "Failed to update property status. Please try again.";
}

function formatCurrency(amount: number): string {
  return new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" }).format(amount);
}

function formatDate(date: string): string {
  return new Intl.DateTimeFormat("en-US", { dateStyle: "medium" }).format(new Date(date));
}
