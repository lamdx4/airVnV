import { api } from '@/lib/api';
import type { Property, Amenity, EditPropertyInput } from '../types';

export const propertiesApi = {
  // Get all properties for current host
  getMyProperties: (page = 1, pageSize = 10): Promise<{ data: Property[], totalCount: number }> => 
    api.get('/api/properties/my', { params: { page, pageSize } }),

  // Create property
  createProperty: (data: any): Promise<{ id: string, slug: string }> =>
    api.post('/api/properties', data),

  // Get single property details
  getProperty: (id: string): Promise<Property> => 
    api.get(`/api/properties/${id}`),

  // Update core info
  updateProperty: (id: string, data: EditPropertyInput): Promise<void> => 
    api.put(`/api/properties/${id}`, data),

  // Update location
  updateLocation: (id: string, data: any): Promise<void> =>
    api.put(`/api/properties/${id}/location`, data),

  // Update status (Publish/Archive)
  updateStatus: (propertyId: string, status: number): Promise<void> =>
    api.patch(`/api/properties/${propertyId}/status`, { status }),

  // Images
  addImages: (propertyId: string, files: File[], type: number): Promise<void> => {
    const formData = new FormData();
    files.forEach(file => formData.append('Files', file));
    formData.append('Type', type.toString());
    return api.post(`/api/properties/${propertyId}/images`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
  },

  removeImage: (propertyId: string, imageId: string): Promise<void> =>
    api.delete(`/api/properties/${propertyId}/images/${imageId}`),

  reorderImages: (propertyId: string, orders: { imageId: string, displayOrder: number }[]): Promise<void> =>
    api.put(`/api/properties/${propertyId}/images/reorder`, { orders }),

  // Amenities
  getAvailableAmenities: (): Promise<Amenity[]> => 
    api.get('/api/amenities'),

  addAmenity: (propertyId: string, amenityId: string): Promise<void> =>
    api.post(`/api/properties/${propertyId}/amenities`, { amenityId }),

  removeAmenity: (propertyId: string, amenityId: string): Promise<void> =>
    api.delete(`/api/properties/${propertyId}/amenities/${amenityId}`),

  updateAmenityInfo: (propertyId: string, amenityId: string, additionalInfo: string): Promise<void> =>
    api.patch(`/api/properties/${propertyId}/amenities/${amenityId}`, { additionalInfo }),

  // Availability/Calendar
  blockDates: (propertyId: string, data: { startDate: string, endDate: string, note?: string }): Promise<void> =>
    api.post(`/api/properties/${propertyId}/availability/block`, data),

  removeAvailability: (propertyId: string, availabilityId: string): Promise<void> =>
    api.delete(`/api/properties/${propertyId}/availability/${availabilityId}`),
};
