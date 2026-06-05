import { useState } from 'react';
import { useReviews, useDeleteReview } from '../hooks/useReviews';
import type { ReviewModel } from '../types';
import { ReviewFormModal } from './ReviewFormModal';
import { Star, MoreVertical, Trash2, Edit2 } from 'lucide-react';
import { format } from 'date-fns';
import { Button } from '@/components/ui/button';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { useTranslation } from 'react-i18next';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';

export function ReviewList({ propertyId }: { propertyId: string }) {
  const { t } = useTranslation();
  const [page, setPage] = useState(1);
  const { data, isLoading, isError } = useReviews(propertyId, page, 10);
  const deleteMutation = useDeleteReview(propertyId);
  
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingReview, setEditingReview] = useState<ReviewModel | null>(null);
  const [reviewToDelete, setReviewToDelete] = useState<string | null>(null);

  // Auth mock for logic (Replace with actual Zustand store: `const { user } = useAuthStore()`)
  const currentUserId = localStorage.getItem('airbnb_user_id') || ''; 
  const isAuthenticated = !!currentUserId;

  if (isLoading) return <div className="py-8 animate-pulse text-gray-400">{t('reviews.loading')}</div>;
  if (isError) return <div className="py-8 text-red-500">{t('reviews.failedToLoad')}</div>;

  const handleEdit = (review: ReviewModel) => {
    setEditingReview(review);
    setIsModalOpen(true);
  };

  const handleDeleteConfirm = async () => {
    if (reviewToDelete) {
      await deleteMutation.mutateAsync(reviewToDelete);
      setReviewToDelete(null);
    }
  };

  const openNewReviewModal = () => {
    setEditingReview(null);
    setIsModalOpen(true);
  };

  return (
    <div className="py-12 border-t border-gray-200">
      <div className="flex justify-between items-center mb-8">
        <h2 className="text-2xl font-semibold text-gray-900 flex items-center gap-2">
          <Star className="w-6 h-6 fill-current" />
          {data?.totalCount || 0} {t('reviews.listTitle')}
        </h2>
        {isAuthenticated && (
          <Button 
            onClick={openNewReviewModal}
            variant="outline"
            className="border-gray-900 hover:bg-gray-50 text-gray-900 font-semibold"
          >
            {t('reviews.writeReview')}
          </Button>
        )}
      </div>

      {data?.items.length === 0 ? (
        <p className="text-gray-500">{t('reviews.noReviews')}</p>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-x-12 gap-y-10">
          {data?.items.map((review) => (
            <div key={review.id} className="relative group">
              <div className="flex items-center mb-4">
                <div className="w-12 h-12 bg-gray-200 rounded-full flex items-center justify-center font-bold text-gray-500 uppercase mr-4">
                  {review.guestId.substring(0, 2)}
                </div>
                <div>
                  <h3 className="font-semibold text-gray-900">Guest {review.guestId.substring(0, 6)}</h3>
                  <div className="flex items-center text-sm text-gray-500 gap-2">
                    <span>{format(review.createdAt, 'MMMM yyyy')}</span>
                  </div>
                </div>

                {/* Edit/Delete options for owner */}
                {review.guestId === currentUserId && (
                  <div className="ml-auto">
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button 
                          variant="ghost" 
                          size="icon"
                          className="rounded-full hover:bg-gray-100"
                        >
                          <MoreVertical className="w-5 h-5 text-gray-500" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end" className="w-32 p-1.5 rounded-xl">
                        <DropdownMenuItem 
                          onClick={() => handleEdit(review)}
                          className="gap-2 rounded-lg cursor-pointer text-sm"
                        >
                          <Edit2 className="w-4 h-4" /> {t('reviews.edit')}
                        </DropdownMenuItem>
                        <DropdownMenuItem 
                          onClick={() => setReviewToDelete(review.id)}
                          className="gap-2 rounded-lg cursor-pointer text-sm text-red-600 focus:text-red-600 focus:bg-red-50"
                        >
                          <Trash2 className="w-4 h-4" /> {t('reviews.delete')}
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </div>
                )}
              </div>
              <div className="flex items-center mb-2">
                {[...Array(5)].map((_, i) => (
                  <Star 
                    key={i} 
                    className={`w-3.5 h-3.5 ${i < review.rating ? 'fill-gray-900 text-gray-900' : 'text-gray-300'}`} 
                  />
                ))}
              </div>
              <p className="text-gray-700 leading-relaxed line-clamp-3">
                {review.comment}
              </p>
            </div>
          ))}
        </div>
      )}

      {/* Pagination (Simple) */}
      {(data?.totalCount || 0) > 10 && (
        <div className="mt-10 flex gap-4">
          <Button 
            disabled={page === 1}
            onClick={() => setPage(p => Math.max(1, p - 1))}
            variant="outline"
            className="border-gray-900 font-semibold"
          >
            {t('reviews.previous')}
          </Button>
          <Button 
            disabled={page * 10 >= (data?.totalCount || 0)}
            onClick={() => setPage(p => p + 1)}
            variant="outline"
            className="border-gray-900 font-semibold"
          >
            {t('reviews.next')}
          </Button>
        </div>
      )}

      <ReviewFormModal 
        propertyId={propertyId}
        isOpen={isModalOpen}
        onClose={() => {
          setIsModalOpen(false);
          setEditingReview(null);
        }}
        existingReview={editingReview}
      />

      <ConfirmDialog
        open={reviewToDelete !== null}
        title={t('reviews.deleteTitle')}
        description={t('reviews.deleteDesc')}
        confirmText={t('reviews.deleteConfirm')}
        cancelText={t('reviews.close')}
        variant="destructive"
        onConfirm={handleDeleteConfirm}
        onCancel={() => setReviewToDelete(null)}
      />
    </div>
  );
}
