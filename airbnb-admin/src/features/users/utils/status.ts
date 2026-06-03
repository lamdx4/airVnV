import { UserStatus, UserRole } from "../types";
import type { UserStatusValue, UserRoleValue } from "../types";

const statusConfig: Record<
  UserStatusValue,
  { label: string; variant: "default" | "secondary" | "destructive" | "outline" }
> = {
  [UserStatus.ACTIVE]: { label: "Active", variant: "default" },
  [UserStatus.SUSPENDED]: { label: "Suspended", variant: "secondary" },
  [UserStatus.BANNED]: { label: "Banned", variant: "destructive" },
  [UserStatus.PENDING_VERIFICATION]: { label: "Pending Verification", variant: "outline" },
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
