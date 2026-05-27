import type { ReviewDto, GetReviewsResponseDto, ReviewModel, PagedReviewModel } from '../types';

export const mapReviewDtoToModel = (dto: ReviewDto): ReviewModel => ({
  id: dto.id,
  guestId: dto.guestId,
  rating: dto.rating,
  comment: dto.comment,
  createdAt: new Date(dto.createdAt),
});

export const mapGetReviewsResponseToModel = (dto: GetReviewsResponseDto): PagedReviewModel => ({
  totalCount: dto.totalCount,
  page: dto.page,
  pageSize: dto.pageSize,
  items: dto.reviews.map(mapReviewDtoToModel),
});
