"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { ColumnDef } from "@tanstack/react-table";
import { Building2, Eye, Search } from "lucide-react";
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

import {
  useProperties,
  useApproveProperty,
  useRejectProperty,
} from "../hooks";
import {
  PropertyStatus,
  PropertyTypeEnum,
  type Property,
  type PropertyListParams,
  type PropertyStatusValue,
} from "../types";
import { getPropertyStatusConfig } from "../utils/status";
import { RejectPropertyDialog } from "./reject-property-dialog";

const statusFilterOptions: { value: string; label: string }[] = [
  { value: "all", label: "All Statuses" },
  { value: String(PropertyStatus.PENDING_REVIEW), label: "Pending Review" },
  { value: String(PropertyStatus.PUBLISHED), label: "Published" },
  { value: String(PropertyStatus.SUSPENDED), label: "Suspended" },
  { value: String(PropertyStatus.REJECTED), label: "Rejected" },
  { value: String(PropertyStatus.DRAFT), label: "Draft" },
  { value: String(PropertyStatus.ARCHIVED), label: "Archived" },
];

function StatusBadge({ status }: { status: PropertyStatusValue }) {
  const config = getPropertyStatusConfig(status);
  return <Badge variant={config.variant}>{config.label}</Badge>;
}

export function PropertiesList() {
  const router = useRouter();
  const [params, setParams] = useState<PropertyListParams>({
    page: 1,
    pageSize: DEFAULT_PAGE_SIZE,
    status: PropertyStatus.PENDING_REVIEW,
  });
  const [statusFilter, setStatusFilter] = useState(String(PropertyStatus.PENDING_REVIEW));
  const [searchInput, setSearchInput] = useState("");
  const [rejectTarget, setRejectTarget] = useState<Property | null>(null);

  const { data, isLoading, isError, refetch } = useProperties(params);
  const approveMutation = useApproveProperty();
  const rejectMutation = useRejectProperty();

  const properties = data?.items ?? [];
  const totalItems = data?.totalItems ?? 0;

  function handleStatusFilter(value: string) {
    setStatusFilter(value);
    setParams((prev) => ({
      ...prev,
      page: 1,
      status: value === "all" ? undefined : Number(value),
    }));
  }

  function handleSearch() {
    setParams((prev) => ({ ...prev, page: 1, searchTerm: searchInput || undefined }));
  }

  function handleApprove(id: string) {
    approveMutation.mutate(id, {
      onSuccess: () => toast.success("Property approved successfully"),
      onError: (err) => toast.error(getApiErrorMessage(err)),
    });
  }

  function handleRejectConfirm({ id, reason }: { id: string; reason: string }) {
    rejectMutation.mutate(
      { id, reason },
      {
        onSuccess: () => {
          toast.success("Property rejected");
          setRejectTarget(null);
        },
        onError: (err) => toast.error(getApiErrorMessage(err)),
      },
    );
  }

  const columns: ColumnDef<Property>[] = [
    {
      accessorKey: "title",
      header: "Title",
      cell: ({ row }) => (
        <span className="font-medium">{row.getValue("title")}</span>
      ),
    },
    { accessorKey: "displayAddress", header: "Location" },
    {
      accessorKey: "type",
      header: "Type",
      cell: ({ row }) => PropertyTypeEnum[row.original.type] ?? "Unknown",
    },
    {
      accessorKey: "status",
      header: "Status",
      cell: ({ row }) => <StatusBadge status={row.original.status} />,
    },
    {
      accessorKey: "basePrice",
      header: "Price/Night",
      cell: ({ row }) => formatCurrency(row.getValue("basePrice")),
    },
    {
      accessorKey: "bedroomCount",
      header: "Beds",
    },
    {
      accessorKey: "createdAt",
      header: "Created",
      cell: ({ row }) => formatDate(row.getValue("createdAt")),
    },
    {
      id: "actions",
      header: "Actions",
      cell: ({ row }) => {
        const property = row.original;
        const isPending = property.status === PropertyStatus.PENDING_REVIEW;
        return (
          <div className="flex items-center gap-1">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => router.push(ROUTES.PROPERTY_DETAIL(property.id))}
            >
              <Eye className="h-4 w-4" />
            </Button>
            {isPending && (
              <>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => handleApprove(property.id)}
                  disabled={approveMutation.isPending}
                >
                  Approve
                </Button>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => setRejectTarget(property)}
                >
                  Reject
                </Button>
              </>
            )}
          </div>
        );
      },
    },
  ];

  if (isError) {
    return (
      <div className="flex h-[50vh] items-center justify-center gap-4">
        <p className="text-[#6a6a6a]">Failed to load properties.</p>
        <Button variant="outline" onClick={() => refetch()}>
          Retry
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex items-center gap-3">
          <Select value={statusFilter} onValueChange={handleStatusFilter}>
            <SelectTrigger className="w-[180px]">
              <SelectValue placeholder="Filter by status" />
            </SelectTrigger>
            <SelectContent>
              {statusFilterOptions.map((opt) => (
                <SelectItem key={opt.value} value={opt.value}>
                  {opt.label}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>

          <div className="flex items-center gap-2">
            <Input
              placeholder="Search by title..."
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleSearch()}
              className="w-[240px]"
            />
            <Button variant="outline" size="sm" onClick={handleSearch}>
              <Search className="h-4 w-4" />
            </Button>
          </div>
        </div>

        <div className="flex items-center gap-2 text-sm text-[#6a6a6a]">
          <Building2 className="h-4 w-4" />
          {totalItems} properties
        </div>
      </div>

      <DataTable
        columns={columns}
        data={properties}
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

      <RejectPropertyDialog
        open={!!rejectTarget}
        onOpenChange={(open) => !open && setRejectTarget(null)}
        property={rejectTarget}
        onConfirm={(reason) =>
          rejectTarget && handleRejectConfirm({ id: rejectTarget.id, reason })
        }
        isLoading={rejectMutation.isPending}
      />
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
