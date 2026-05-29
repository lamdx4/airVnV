import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getReviews, addReview, updateReview, deleteReview } from '../api';
import { mapGetReviewsResponseToModel } from '../utils/mapper';
import type { ReviewFormData } from '../types';

export const useReviews = (propertyId: string, page: number = 1, pageSize: number = 10) => {
  return useQuery({
    queryKey: ['reviews', propertyId, page, pageSize],
    queryFn: async () => {
      const dto = await getReviews(propertyId, page, pageSize);
      return mapGetReviewsResponseToModel(dto);
    },
    enabled: !!propertyId,
  });
};

export const useAddReview = (propertyId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: { bookingId: string; formData: ReviewFormData }) => 
      addReview(propertyId, data.bookingId, data.formData),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['reviews', propertyId] });
      queryClient.invalidateQueries({ queryKey: ['property', propertyId] });
    },
  });
};

export const useUpdateReview = (propertyId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: { reviewId: string; formData: ReviewFormData }) => 
      updateReview(propertyId, data.reviewId, data.formData),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['reviews', propertyId] });
      queryClient.invalidateQueries({ queryKey: ['property', propertyId] });
    },
  });
};

export const useDeleteReview = (propertyId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (reviewId: string) => deleteReview(propertyId, reviewId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['reviews', propertyId] });
      queryClient.invalidateQueries({ queryKey: ['property', propertyId] });
    },
  });
};
