import { api } from "@/lib/api";
import type { BackendPage } from "@/types/api";

import type { Property, PropertyListParams } from "../types";

export { PropertyStatus } from "../types";
export type { PropertyStatusValue } from "../types";

export const propertiesApi = {
  getAll: (params?: PropertyListParams) =>
    api.get<BackendPage<Property>>("/properties/admin", { params }),

  getById: (id: string) =>
    api.get<Property>(`/properties/${id}`),

  approve: (id: string) =>
    api.post<{ id: string; status: string }>(`/properties/${id}/approve`, {}),

  reject: (id: string, reason: string) =>
    api.post<{ id: string; status: string; reason: string }>(`/properties/${id}/reject`, { reason }),

  suspend: (id: string, reason: string) =>
    api.post<{ id: string; status: string; reason: string }>(`/properties/${id}/suspend`, { reason }),

  reinstate: (id: string) =>
    api.post<{ id: string; status: string }>(`/properties/${id}/reinstate`, {}),

  adminDelete: (id: string) =>
    api.delete<{ id: string; message: string }>(`/properties/${id}/admin-delete`),
};
