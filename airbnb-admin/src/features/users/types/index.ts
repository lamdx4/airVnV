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
