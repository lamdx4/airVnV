export const ReviewStatus = {
  PENDING: "Pending",
  APPROVED: "Approved",
  REJECTED: "Rejected",
  FLAGGED: "Flagged",
} as const;

export type ReviewStatusValue = (typeof ReviewStatus)[keyof typeof ReviewStatus];

export interface Review {
  id: string;
  propertyId: string;
  propertyTitle: string;
  userId: string;
  userName: string;
  rating: number;
  comment: string;
  status: ReviewStatusValue;
  createdAt: string;
}

export interface ReviewListParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: ReviewStatusValue;
  minRating?: number;
  maxRating?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}
