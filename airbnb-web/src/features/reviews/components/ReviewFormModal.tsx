import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { reviewFormSchema } from '../utils/validation';
import type { ReviewFormData, ReviewModel } from '../types';
import { useAddReview, useUpdateReview } from '../hooks/useReviews';
import { toast } from 'sonner';
import { Star } from 'lucide-react';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { useTranslation } from 'react-i18next';

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
  const { t } = useTranslation();
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

  const onSubmit = async (data: ReviewFormData) => {
    try {
      if (existingReview) {
        await updateMutation.mutateAsync({ reviewId: existingReview.id, formData: data });
        toast.success(t('reviews.successUpdate'));
      } else {
        await addMutation.mutateAsync({ bookingId: bookingId || '00000000-0000-0000-0000-000000000000', formData: data });
        toast.success(t('reviews.successAdd'));
      }
      onClose();
    } catch (error: any) {
      toast.error(error?.message || t('reviews.failedSubmit'));
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={(open) => { if (!open) onClose(); }}>
      <DialogContent showCloseButton={true} className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>
            {existingReview ? t('reviews.editTitle') : t('reviews.writeTitle')}
          </DialogTitle>
        </DialogHeader>
        
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6 pt-2">
          <div className="space-y-2">
            <Label className="text-sm font-medium text-gray-700">{t('reviews.rating')}</Label>
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

          <div className="space-y-2">
            <Label className="text-sm font-medium text-gray-700">{t('reviews.shareExperience')}</Label>
            <Textarea
              {...register('comment')}
              rows={4}
              placeholder={t('reviews.commentPlaceholder')}
              className="w-full resize-none border-gray-300 focus:border-slate-900 focus:ring-slate-900 rounded-lg"
            />
            {errors.comment && <p className="text-red-500 text-xs mt-1">{errors.comment.message}</p>}
          </div>

          <div className="flex justify-end gap-3 pt-4 border-t border-gray-100">
            <Button 
              type="button" 
              variant="ghost"
              onClick={onClose}
            >
              {t('reviews.cancel')}
            </Button>
            <Button 
              type="submit"
              disabled={isSubmitting}
              className="bg-[#E51D53] hover:bg-[#D70466] text-white"
            >
              {isSubmitting ? t('reviews.submitting') : (existingReview ? t('reviews.saveChanges') : t('reviews.submitReview'))}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
