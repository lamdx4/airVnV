import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

import { reviewsApi } from "../api/reviews";
import type { ReviewListParams } from "../types";

const QUERY_KEYS = {
  ALL: ["admin", "reviews"] as const,
  LIST: (params?: ReviewListParams) => ["admin", "reviews", "list", params] as const,
} as const;

export function useReviews(params?: ReviewListParams) {
  return useQuery({
    queryKey: QUERY_KEYS.LIST(params),
    queryFn: () => reviewsApi.getAll(params),
  });
}

export function useApproveReview() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => reviewsApi.approve(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useRejectReview() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, reason }: { id: string; reason: string }) =>
      reviewsApi.reject(id, reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}

export function useDeleteReview() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => reviewsApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.ALL });
    },
  });
}
