import { useState } from 'react';
import { useReviews, useDeleteReview } from '../hooks/useReviews';
import type { ReviewModel } from '../types';
import { ReviewFormModal } from './ReviewFormModal';
import { Star, MoreVertical, Trash2, Edit2 } from 'lucide-react';
import { format } from 'date-fns';
// Ensure you have auth hook, for demo we mock it or use what's available
// import { useAuthStore } from '@/store/authStore'; 

export function ReviewList({ propertyId }: { propertyId: string }) {
  const [page, setPage] = useState(1);
  const { data, isLoading, isError } = useReviews(propertyId, page, 10);
  const deleteMutation = useDeleteReview(propertyId);
  
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingReview, setEditingReview] = useState<ReviewModel | null>(null);
  const [showOptionsId, setShowOptionsId] = useState<string | null>(null);

  // Auth mock for logic (Replace with actual Zustand store: `const { user } = useAuthStore()`)
  const currentUserId = localStorage.getItem('airbnb_user_id') || ''; 
  const isAuthenticated = !!currentUserId;

  if (isLoading) return <div className="py-8 animate-pulse text-gray-400">Loading reviews...</div>;
  if (isError) return <div className="py-8 text-red-500">Failed to load reviews.</div>;

  const handleEdit = (review: ReviewModel) => {
    setEditingReview(review);
    setIsModalOpen(true);
    setShowOptionsId(null);
  };

  const handleDelete = async (reviewId: string) => {
    if (confirm("Are you sure you want to delete this review?")) {
      await deleteMutation.mutateAsync(reviewId);
    }
    setShowOptionsId(null);
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
          {data?.totalCount || 0} reviews
        </h2>
        {isAuthenticated && (
          <button 
            onClick={openNewReviewModal}
            className="px-4 py-2 border border-gray-900 rounded-lg font-semibold text-gray-900 hover:bg-gray-50 transition-colors"
          >
            Write a Review
          </button>
        )}
      </div>

      {data?.items.length === 0 ? (
        <p className="text-gray-500">No reviews yet. Be the first to leave a review!</p>
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
                  <div className="ml-auto relative">
                    <button 
                      onClick={() => setShowOptionsId(showOptionsId === review.id ? null : review.id)}
                      className="p-2 rounded-full hover:bg-gray-100 transition-colors"
                    >
                      <MoreVertical className="w-5 h-5 text-gray-500" />
                    </button>
                    {showOptionsId === review.id && (
                      <div className="absolute right-0 top-full mt-1 bg-white border border-gray-200 rounded-xl shadow-lg py-2 w-32 z-10">
                        <button 
                          onClick={() => handleEdit(review)}
                          className="w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-50 flex items-center gap-2"
                        >
                          <Edit2 className="w-4 h-4" /> Edit
                        </button>
                        <button 
                          onClick={() => handleDelete(review.id)}
                          className="w-full text-left px-4 py-2 text-sm text-red-600 hover:bg-gray-50 flex items-center gap-2"
                        >
                          <Trash2 className="w-4 h-4" /> Delete
                        </button>
                      </div>
                    )}
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
          <button 
            disabled={page === 1}
            onClick={() => setPage(p => Math.max(1, p - 1))}
            className="px-4 py-2 border border-gray-900 rounded-lg font-semibold disabled:opacity-50 disabled:border-gray-300"
          >
            Previous
          </button>
          <button 
            disabled={page * 10 >= (data?.totalCount || 0)}
            onClick={() => setPage(p => p + 1)}
            className="px-4 py-2 border border-gray-900 rounded-lg font-semibold disabled:opacity-50 disabled:border-gray-300"
          >
            Next
          </button>
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
    </div>
  );
}
