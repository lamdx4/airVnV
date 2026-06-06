import { UserStatus, UserRole } from "../types";
import type { UserStatusValue, UserRoleValue } from "../types";

const statusConfig: Record<
  UserStatusValue,
  { label: string; variant: "default" | "secondary" | "destructive" | "outline" | "success" | "warning" | "info" }
> = {
  [UserStatus.ACTIVE]: { label: "Active", variant: "success" },
  [UserStatus.SUSPENDED]: { label: "Suspended", variant: "warning" },
  [UserStatus.BANNED]: { label: "Banned", variant: "destructive" },
};

const roleConfig: Record<UserRoleValue, { label: string }> = {
  [UserRole.USER]: { label: "User" },
  [UserRole.MODERATOR]: { label: "Moderator" },
  [UserRole.ADMIN]: { label: "Admin" },
};

export function getUserStatusConfig(status: UserStatusValue) {
  return statusConfig[status] ?? { label: status, variant: "outline" as const };
}

export function getUserRoleConfig(role: UserRoleValue) {
  return roleConfig[role] ?? { label: role };
}
