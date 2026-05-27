export interface ReviewDto {
  id: string;
  guestId: string;
  rating: number;
  comment: string;
  createdAt: string;
}

export interface GetReviewsResponseDto {
  totalCount: number;
  page: number;
  pageSize: number;
  reviews: ReviewDto[];
}

export interface ReviewModel {
  id: string;
  guestId: string;
  rating: number;
  comment: string;
  createdAt: Date;
}

export interface PagedReviewModel {
  totalCount: number;
  page: number;
  pageSize: number;
  items: ReviewModel[];
}

export interface ReviewFormData {
  rating: number;
  comment: string;
}
