import { api } from '@/lib/api';
import { PropertyDTO, PropertyStatus, CreatePropertyRequest, PagedResponse, PaginationParams } from '../types';

export const propertiesApi = {
  // Get all properties of current host
  getMyProperties: (params: PaginationParams): Promise<PagedResponse<PropertyDTO>> => 
    api.get('/api/properties/my', { params }),

  // Get single property details
  getProperty: (id: string): Promise<PropertyDTO> => 
    api.get(`/api/properties/${id}`),

  // Create new property
  createProperty: (data: CreatePropertyRequest): Promise<{ id: string }> => 
    api.post('/api/properties', data),

  // Update property
  updateProperty: (id: string, data: Partial<CreatePropertyRequest>): Promise<{ id: string, updatedAt: string }> => 
    api.patch(`/api/properties/${id}`, data),

  // Delete property
  deleteProperty: (id: string): Promise<{ id: string, message: string }> => 
    api.delete(`/api/properties/${id}`),

  // --- Status Transitions ---
  submitProperty: (id: string): Promise<{ id: string }> => 
    api.post(`/api/properties/${id}/submit`),

  approveProperty: (id: string): Promise<{ id: string }> => 
    api.post(`/api/properties/${id}/approve`),

  suspendProperty: (id: string, reason: string): Promise<{ id: string }> => 
    api.post(`/api/properties/${id}/suspend`, { reason }),

  reinstateProperty: (id: string): Promise<{ id: string }> => 
    api.post(`/api/properties/${id}/reinstate`),

  archiveProperty: (id: string): Promise<{ id: string }> => 
    api.post(`/api/properties/${id}/archive`),

  // --- Media Management ---
  addImage: (propertyId: string, file: File, type: number): Promise<{ id: string, url: string }> => {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('type', type.toString());
    return api.post(`/api/properties/${propertyId}/images`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
  },

  removeImage: (propertyId: string, imageId: string): Promise<void> => 
    api.delete(`/api/properties/${propertyId}/images/${imageId}`),

  // Bulk add images
  addImages: (propertyId: string, files: File[], type: number): Promise<{ images: { id: string, url: string }[] }> => {
    const formData = new FormData();
    files.forEach(file => formData.append('Files', file));
    formData.append('Type', type.toString());
    return api.post(`/api/properties/${propertyId}/images/bulk`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
  },

  // --- Amenity Management ---
  addAmenity: (propertyId: string, amenityId: string, additionalInfo?: string): Promise<void> => 
    api.post(`/api/properties/${propertyId}/amenities/${amenityId}`, { additionalInfo }),

  removeAmenity: (propertyId: string, amenityId: string): Promise<void> => 
    api.delete(`/api/properties/${propertyId}/amenities/${amenityId}`),
};
