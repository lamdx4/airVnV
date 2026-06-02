import { api } from "@/lib/api";
import type { ApiResponse, PaginatedResponse, PaginationParams } from "@/types/api";

export const PropertyStatus = {
  DRAFT: "Draft",
  PENDING_REVIEW: "PendingReview",
  ACTIVE: "Active",
  INACTIVE: "Inactive",
  REJECTED: "Rejected",
} as const;

export type PropertyStatusValue = (typeof PropertyStatus)[keyof typeof PropertyStatus];

export interface Property {
  id: string;
  title: string;
  hostId: string;
  hostName: string;
  propertyType: string;
  status: PropertyStatusValue;
  city: string;
  country: string;
  pricePerNight: number;
  maxGuests: number;
  bedrooms: number;
  bathrooms: number;
  rating: number;
  reviewCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface PropertyListParams extends PaginationParams {
  status?: PropertyStatusValue;
  city?: string;
}

export const propertiesApi = {
  getAll: (params?: PropertyListParams) =>
    api.get<ApiResponse<PaginatedResponse<Property>>>("/admin/properties", { params }),

  getById: (id: string) =>
    api.get<ApiResponse<Property>>(`/admin/properties/${id}`),

  approve: (id: string) =>
    api.patch<ApiResponse<Property>>(`/admin/properties/${id}/approve`),

  reject: (id: string, reason: string) =>
    api.patch<ApiResponse<Property>>(`/admin/properties/${id}/reject`, { reason }),

  deactivate: (id: string) =>
    api.patch<ApiResponse<Property>>(`/admin/properties/${id}/deactivate`),

  delete: (id: string) =>
    api.delete<ApiResponse<null>>(`/admin/properties/${id}`),
};
