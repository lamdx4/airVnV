import { api } from "@/lib/api";
import type { ApiResponse, PaginatedResponse } from "@/types/api";

export const UserRole = {
  GUEST: "Guest",
  HOST: "Host",
  ADMIN: "Admin",
} as const;

export type UserRoleValue = (typeof UserRole)[keyof typeof UserRole];

export const UserStatus = {
  ACTIVE: "Active",
  SUSPENDED: "Suspended",
  BANNED: "Banned",
  PENDING_VERIFICATION: "PendingVerification",
} as const;

export type UserStatusValue = (typeof UserStatus)[keyof typeof UserStatus];

export interface User {
  id: string;
  email: string;
  fullName: string;
  role: UserRoleValue;
  status: UserStatusValue;
  avatarUrl?: string;
  phone?: string;
  createdAt: string;
  lastLoginAt?: string;
}

export interface UserListParams {
  page?: number;
  pageSize?: number;
  search?: string;
  role?: UserRoleValue;
  status?: UserStatusValue;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}

export const usersApi = {
  getAll: (params?: UserListParams) =>
    api.get<ApiResponse<PaginatedResponse<User>>>("/admin/users", { params }),

  getById: (id: string) =>
    api.get<ApiResponse<User>>(`/admin/users/${id}`),

  suspend: (id: string, reason: string) =>
    api.patch<ApiResponse<User>>(`/admin/users/${id}/suspend`, { reason }),

  ban: (id: string, reason: string) =>
    api.patch<ApiResponse<User>>(`/admin/users/${id}/ban`, { reason }),

  activate: (id: string) =>
    api.patch<ApiResponse<User>>(`/admin/users/${id}/activate`),

  updateRole: (id: string, role: UserRoleValue) =>
    api.patch<ApiResponse<User>>(`/admin/users/${id}/role`, { role }),

  delete: (id: string) =>
    api.delete<ApiResponse<null>>(`/admin/users/${id}`),
};
