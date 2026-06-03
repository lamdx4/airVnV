import { api } from "@/lib/api";
import type { ApiResponse, PaginatedResponse } from "@/types/api";

import type { User, UserDetail, UserListParams, KycDocument } from "../types";

export const UserRole = {
  USER: "User",
  MODERATOR: "Moderator",
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

export const usersApi = {
  getAll: (params?: UserListParams) =>
    api.get<ApiResponse<PaginatedResponse<User>>>("/admin/users", { params }),

  getById: (id: string) =>
    api.get<ApiResponse<UserDetail>>(`/admin/users/${id}`),

  suspend: (id: string, reason: string) =>
    api.patch<ApiResponse<{ id: string; status: string }>>(`/admin/users/${id}/suspend`, { reason }),

  ban: (id: string, reason: string) =>
    api.patch<ApiResponse<{ id: string; status: string }>>(`/admin/users/${id}/ban`, { reason }),

  activate: (id: string) =>
    api.patch<ApiResponse<{ id: string; status: string }>>(`/admin/users/${id}/activate`),

  updateRole: (id: string, role: UserRoleValue) =>
    api.patch<ApiResponse<User>>(`/admin/users/${id}/role`, { role }),

  delete: (id: string) =>
    api.delete<ApiResponse<null>>(`/admin/users/${id}`),

  getKycDocuments: (id: string) =>
    api.get<ApiResponse<KycDocument[]>>(`/admin/users/${id}/kyc-documents`),

  approveVerification: (id: string) =>
    api.patch<ApiResponse<{ id: string; isVerified: boolean; status: string }>>(`/admin/users/${id}/verify`),

  rejectVerification: (id: string, reason: string) =>
    api.patch<ApiResponse<{ id: string; isVerified: boolean }>>(`/admin/users/${id}/reject-verification`, { reason }),
};
