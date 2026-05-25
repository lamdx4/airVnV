import { api } from '@/lib/api';

export interface PendingPropertyDto {
  propertyId: string;
  title: string;
  thumbnailUrl: string;
  hostName: string;
  submittedAt: string;
  status: string;
}

export interface GetPendingPropertiesResponse {
  items: PendingPropertyDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface RejectPropertyResponse {
  propertyId: string;
  status: string;
  actionAt: string;
  adminId: string;
}

export const adminPropertiesApi = {
  getPendingProperties: (params: {
    page?: number;
    pageSize?: number;
    search?: string;
    sortBy?: string;
    sortOrder?: 'asc' | 'desc';
  }): Promise<GetPendingPropertiesResponse> => {
    return api.get('/api/admin/properties/pending', { params }) as Promise<GetPendingPropertiesResponse>;
  },

  approveProperty: (propertyId: string): Promise<{ id: string; status: string }> => {
    return api.post(`/api/admin/properties/${propertyId}/approve`) as Promise<{ id: string; status: string }>;
  },

  rejectProperty: (propertyId: string, reason?: string): Promise<RejectPropertyResponse> => {
    return api.post(`/api/admin/properties/${propertyId}/reject`, { reason }) as Promise<RejectPropertyResponse>;
  },
};