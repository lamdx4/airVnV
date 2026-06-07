import { api } from "@/lib/api";
import type { BackendPage } from "@/types/api";

import type { User, UserDetail, UserListParams } from "../types";

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
} as const;

export type UserStatusValue = (typeof UserStatus)[keyof typeof UserStatus];

export const usersApi = {
  getAll: (params?: UserListParams) =>
    api.get<BackendPage<User>>("/admin/users", { params }),

  getById: (id: string) =>
    api.get<UserDetail>(`/admin/users/${id}`),

  suspend: (id: string, reason: string) =>
    api.patch<{ id: string; status: string }>(`/admin/users/${id}/suspend`, { reason }),

  ban: (id: string, reason: string) =>
    api.patch<{ id: string; status: string }>(`/admin/users/${id}/ban`, { reason }),

  activate: (id: string) =>
    api.patch<{ id: string; status: string }>(`/admin/users/${id}/activate`),

  updateRole: (id: string, role: UserRoleValue) =>
    api.patch<User>(`/admin/users/${id}/role`, { role }),

  delete: (id: string) =>
    api.delete<null>(`/admin/users/${id}`),
};
