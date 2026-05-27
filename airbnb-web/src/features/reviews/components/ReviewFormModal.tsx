import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { reviewFormSchema } from '../utils/validation';
import type { ReviewFormData, ReviewModel } from '../types';
import { useAddReview, useUpdateReview } from '../hooks/useReviews';
import { toast } from 'sonner';
import { Star } from 'lucide-react';

interface ReviewFormModalProps {
  propertyId: string;
  isOpen: boolean;
  onClose: () => void;
  existingReview?: ReviewModel | null;
  // In a real scenario, bookingId should be selected or passed if it's a new review.
  // For demo purposes, we will pass a dummy one if not provided.
  bookingId?: string;
}

export function ReviewFormModal({ propertyId, isOpen, onClose, existingReview, bookingId }: ReviewFormModalProps) {
  const [hoverRating, setHoverRating] = useState(0);
  
  const addMutation = useAddReview(propertyId);
  const updateMutation = useUpdateReview(propertyId);

  const { register, handleSubmit, setValue, watch, reset, formState: { errors, isSubmitting } } = useForm<ReviewFormData>({
    resolver: zodResolver(reviewFormSchema),
    defaultValues: {
      rating: 0,
      comment: '',
    }
  });

  const currentRating = watch('rating');

  useEffect(() => {
    if (isOpen) {
      if (existingReview) {
        setValue('rating', existingReview.rating);
        setValue('comment', existingReview.comment);
      } else {
        reset();
      }
    }
  }, [isOpen, existingReview, setValue, reset]);

  if (!isOpen) return null;

  const onSubmit = async (data: ReviewFormData) => {
    try {
      if (existingReview) {
        await updateMutation.mutateAsync({ reviewId: existingReview.id, formData: data });
        toast.success("Review updated successfully!");
      } else {
        await addMutation.mutateAsync({ bookingId: bookingId || '00000000-0000-0000-0000-000000000000', formData: data });
        toast.success("Review submitted successfully!");
      }
      onClose();
    } catch (error: any) {
      toast.error(error?.message || "Failed to submit review");
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
      <div className="bg-white rounded-xl shadow-xl w-full max-w-md overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center">
          <h2 className="text-xl font-semibold text-gray-900">
            {existingReview ? "Edit your review" : "Write a review"}
          </h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            ✕
          </button>
        </div>
        
        <form onSubmit={handleSubmit(onSubmit)} className="p-6 space-y-6">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Rating</label>
            <div className="flex gap-2">
              {[1, 2, 3, 4, 5].map((star) => (
                <button
                  key={star}
                  type="button"
                  onClick={() => setValue('rating', star, { shouldValidate: true })}
                  onMouseEnter={() => setHoverRating(star)}
                  onMouseLeave={() => setHoverRating(0)}
                  className="focus:outline-none transition-transform hover:scale-110"
                >
                  <Star 
                    className={`w-8 h-8 ${
                      (hoverRating || currentRating) >= star 
                        ? 'fill-yellow-400 text-yellow-400' 
                        : 'text-gray-300'
                    }`} 
                  />
                </button>
              ))}
            </div>
            {errors.rating && <p className="text-red-500 text-xs mt-1">{errors.rating.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Share your experience</label>
            <textarea
              {...register('comment')}
              rows={4}
              placeholder="What was it like staying here?"
              className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-slate-900 focus:border-slate-900 outline-none resize-none"
            />
            {errors.comment && <p className="text-red-500 text-xs mt-1">{errors.comment.message}</p>}
          </div>

          <div className="flex justify-end gap-3 pt-4">
            <button 
              type="button" 
              onClick={onClose}
              className="px-5 py-2.5 rounded-lg font-semibold text-gray-700 hover:bg-gray-100 transition-colors"
            >
              Cancel
            </button>
            <button 
              type="submit"
              disabled={isSubmitting}
              className="px-5 py-2.5 rounded-lg font-semibold bg-[#E51D53] text-white hover:bg-[#D70466] transition-colors disabled:opacity-50"
            >
              {isSubmitting ? "Submitting..." : (existingReview ? "Save Changes" : "Submit Review")}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
