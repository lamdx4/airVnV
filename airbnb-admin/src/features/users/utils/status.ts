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
  [UserRole.GUEST]: { label: "Guest" },
  [UserRole.HOST]: { label: "Host" },
  [UserRole.ADMIN]: { label: "Admin" },
};

export function getUserStatusConfig(status: UserStatusValue) {
  return statusConfig[status];
}

export function getUserRoleConfig(role: UserRoleValue) {
  return roleConfig[role];
}
