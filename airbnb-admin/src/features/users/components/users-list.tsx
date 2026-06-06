"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { ColumnDef } from "@tanstack/react-table";
import { Eye, Search, Users } from "lucide-react";
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

import { useUsers } from "../hooks";
import {
  UserStatus,
  UserRole,
  type User,
  type UserListParams,
  type UserStatusValue,
  type UserRoleValue,
} from "../types";
import { getUserStatusConfig, getUserRoleConfig } from "../utils/status";

const statusFilterOptions: { value: string; label: string }[] = [
  { value: "all", label: "All Statuses" },
  { value: UserStatus.ACTIVE, label: "Active" },
  { value: UserStatus.SUSPENDED, label: "Suspended" },
  { value: UserStatus.BANNED, label: "Banned" },
];

function StatusBadge({ status }: { status: UserStatusValue }) {
  const config = getUserStatusConfig(status);
  return <Badge variant={config.variant}>{config.label}</Badge>;
}

function RoleBadge({ role }: { role: UserRoleValue }) {
  const config = getUserRoleConfig(role);
  return <Badge variant="outline">{config.label}</Badge>;
}

export function UsersList() {
  const router = useRouter();
  const [params, setParams] = useState<UserListParams>({
    page: 1,
    pageSize: DEFAULT_PAGE_SIZE,
    role: UserRole.USER,
  });
  const [statusFilter, setStatusFilter] = useState("all");
  const [searchInput, setSearchInput] = useState("");

  const { data, isLoading, isError, refetch } = useUsers(params);

  const users = data?.items ?? [];
  const totalItems = data?.totalItems ?? 0;

  function handleStatusFilter(value: string) {
    setStatusFilter(value);
    setParams((prev) => ({
      ...prev,
      page: 1,
      status: value === "all" ? undefined : (value as UserStatusValue),
    }));
  }

  function handleSearch() {
    setParams((prev) => ({ ...prev, page: 1, search: searchInput || undefined }));
  }

  const columns: ColumnDef<User>[] = [
    {
      accessorKey: "fullName",
      header: "Name",
      cell: ({ row }) => (
        <span className="font-medium">{row.getValue("fullName")}</span>
      ),
    },
    {
      accessorKey: "email",
      header: "Email",
    },
    {
      accessorKey: "role",
      header: "Role",
      cell: ({ row }) => <RoleBadge role={row.original.role} />,
    },
    {
      accessorKey: "status",
      header: "Status",
      cell: ({ row }) => <StatusBadge status={row.original.status} />,
    },
    {
      accessorKey: "isVerified",
      header: "Verified",
      cell: ({ row }) =>
        row.original.isVerified ? (
          <Badge variant="success">Verified</Badge>
        ) : (
          <Badge variant="outline">Unverified</Badge>
        ),
    },
    {
      accessorKey: "createdAt",
      header: "Created",
      cell: ({ row }) => formatDate(row.getValue("createdAt")),
    },
    {
      accessorKey: "lastLoginAt",
      header: "Last Login",
      cell: ({ row }) =>
        row.original.lastLoginAt ? formatDate(row.original.lastLoginAt) : "Never",
    },
    {
      id: "actions",
      header: "Actions",
      cell: ({ row }) => (
        <Button
          variant="ghost"
          size="sm"
          onClick={() => router.push(ROUTES.USER_DETAIL(row.original.id))}
        >
          <Eye className="h-4 w-4" />
        </Button>
      ),
    },
  ];

  if (isError) {
    return (
      <div className="flex h-[50vh] items-center justify-center gap-4">
        <p className="text-[#6a6a6a]">Failed to load users.</p>
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
              placeholder="Search by name or email..."
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
          <Users className="h-4 w-4" />
          {totalItems} users
        </div>
      </div>

      <DataTable
        columns={columns}
        data={users}
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
    </div>
  );
}

function formatDate(date: string): string {
  return new Intl.DateTimeFormat("en-US", { dateStyle: "medium" }).format(new Date(date));
}
