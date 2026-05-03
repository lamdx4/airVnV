import type { UserRole } from "../../auth/types";

export interface ProfileResponse {
  id: string;
  email: string;
  fullName: string;
  avatarUrl?: string;
  phoneNumber?: string;
  bio?: string;
  role: UserRole;
}

export interface UpdateProfileRequest {
  fullName: string;
  avatarUrl?: string;
  phoneNumber?: string;
  bio?: string;
}

export interface UpdateProfileResponse extends ProfileResponse {}
