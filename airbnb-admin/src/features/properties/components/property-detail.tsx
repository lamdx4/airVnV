"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
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
      <div className="flex items-center justify-between">
        <div className="space-y-2">
          <Breadcrumbs items={breadcrumbs} />
          <div className="flex items-center gap-3">
            <Button variant="ghost" size="sm" onClick={() => router.push(ROUTES.PROPERTIES)}>
              <ArrowLeft className="h-4 w-4 mr-1" />
              Back
            </Button>
            <h1 className="text-2xl font-semibold">{property.title}</h1>
            <Badge variant={statusConfig.variant}>{statusConfig.label}</Badge>
          </div>
          <p className="text-sm text-muted-foreground">
            Created {formatDate(property.createdAt)}
            {property.updatedAt && <> &middot; Updated {formatDate(property.updatedAt)}</>}
          </p>
        </div>
      </div>

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
                <span className="text-sm text-muted-foreground">
                  ({property.reviewCount} reviews)
                </span>
              )}
            </div>
          </CardContent>
        </Card>
      </div>

      {property.status === PropertyStatus.REJECTED && property.rejectionReason && (
        <Card>
          <CardHeader>
            <CardTitle className="text-destructive">Rejection Reason</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-sm">{property.rejectionReason}</p>
          </CardContent>
        </Card>
      )}

      {property.status === PropertyStatus.SUSPENDED && property.suspensionReason && (
        <Card>
          <CardHeader>
            <CardTitle className="text-amber-600">Suspension Reason</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-sm">{property.suspensionReason}</p>
          </CardContent>
        </Card>
      )}

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
            <Label htmlFor="delete-confirm" className="text-sm text-muted-foreground">
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
      <span className="flex items-center gap-2 text-muted-foreground">
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
