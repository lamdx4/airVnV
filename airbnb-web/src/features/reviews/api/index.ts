import { api } from '@/lib/api';
import type { GetReviewsResponseDto, ReviewFormData } from '../types';

export const getReviews = async (propertyId: string, page: number = 1, pageSize: number = 10): Promise<GetReviewsResponseDto> => {
  const response = await api.get<any>(`/api/properties/${propertyId}/reviews?Page=${page}&PageSize=${pageSize}`);
  return response as unknown as GetReviewsResponseDto;
};

export const addReview = async (propertyId: string, bookingId: string, data: ReviewFormData): Promise<any> => {
  return await api.post(`/api/properties/${propertyId}/reviews`, {
    bookingId,
    rating: data.rating,
    comment: data.comment,
  });
};

export const updateReview = async (propertyId: string, reviewId: string, data: ReviewFormData): Promise<any> => {
  return await api.put(`/api/properties/${propertyId}/reviews/${reviewId}`, data);
};

export const deleteReview = async (propertyId: string, reviewId: string): Promise<any> => {
  return await api.delete(`/api/properties/${propertyId}/reviews/${reviewId}`);
};  
