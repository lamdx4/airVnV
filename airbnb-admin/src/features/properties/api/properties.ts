import { api } from "@/lib/api";
import type { ApiResponse, PaginatedResponse } from "@/types/api";

import type { Property, PropertyListParams } from "../types";

export { PropertyStatus } from "../types";
export type { PropertyStatusValue } from "../types";

export const propertiesApi = {
  getAll: (params?: PropertyListParams) =>
    api.get<ApiResponse<PaginatedResponse<Property>>>("/properties/admin", { params }),

  getById: (id: string) =>
    api.get<ApiResponse<Property>>(`/properties/${id}`),

  approve: (id: string) =>
    api.post<ApiResponse<{ id: string; status: string }>>(`/properties/${id}/approve`, {}),

  reject: (id: string, reason: string) =>
    api.post<ApiResponse<{ id: string; status: string; reason: string }>>(`/properties/${id}/reject`, { reason }),

  suspend: (id: string, reason: string) =>
    api.post<ApiResponse<{ id: string; status: string; reason: string }>>(`/properties/${id}/suspend`, { reason }),

  reinstate: (id: string) =>
    api.post<ApiResponse<{ id: string; status: string }>>(`/properties/${id}/reinstate`, {}),

  adminDelete: (id: string) =>
    api.delete<ApiResponse<{ id: string; message: string }>>(`/properties/${id}/admin-delete`),
};
