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

export interface User {
  id: string;
  email: string;
  fullName: string;
  role: UserRoleValue;
  status: UserStatusValue;
  isVerified: boolean;
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

export interface KycDocument {
  id: string;
  status: "Submitted" | "Approved" | "Rejected" | "Expired";
  documentType?: string;
  rejectionReason?: string;
  submittedAt: string;
  reviewedAt?: string;
  images: KycDocumentImage[];
}

export interface KycDocumentImage {
  id: string;
  imageUrl: string;
  label?: string;
}

export interface UserDetail extends User {
  bio?: string;
  suspensionReason?: string;
  banReason?: string;
  kycDocuments?: KycDocument[];
}
