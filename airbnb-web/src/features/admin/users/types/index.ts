export type UserRole = 'User' | 'Moderator' | 'Admin';

export type UserStatus = 'Active' | 'Suspended' | 'Banned' | 'PendingVerification';

export interface User {
  id: string;
  email: string;
  fullName: string;
  role: UserRole;
  status: UserStatus;
  avatarUrl?: string;
  isVerified: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface UserDetail extends User {
  phoneNumber?: string;
  dateOfBirth?: string;
  address?: string;
  bio?: string;
  // KYC info
  kycStatus: 'NotSubmitted' | 'Pending' | 'Approved' | 'Rejected';
  kycSubmittedAt?: string;
  kycVerifiedAt?: string;
  kycRejectionReason?: string;
  // Stats
  totalBookings?: number;
  totalProperties?: number;
}

export interface UserListItem {
  id: string;
  email: string;
  fullName: string;
  role: UserRole;
  status: UserStatus;
  avatarUrl?: string;
  isVerified: boolean;
  kycStatus: 'NotSubmitted' | 'Pending' | 'Approved' | 'Rejected';
  createdAt: string;
}

export interface SuspendUserRequest {
  reason: string;
  durationDays?: number;
}

export interface KYCApproveRequest {
  notes?: string;
}

export interface KYCRejectRequest {
  reason: string;
}

export interface AdminActionResponse {
  userId: string;
  action: string;
  performedAt: string;
  adminId: string;
}