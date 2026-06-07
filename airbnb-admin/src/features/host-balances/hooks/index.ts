import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { hostBalancesApi } from "../api/host-balances";
import type { HostBalanceListParams } from "../types";

const QUERY_KEYS = {
  ALL: ["admin", "host-balances"] as const,
  LIST: (params?: HostBalanceListParams) =>
    ["admin", "host-balances", "list", params] as const,
  DETAIL: (id: string) => ["admin", "host-balances", "detail", id] as const,
} as const;

export function useHostBalances(params?: HostBalanceListParams) {
  return useQuery({
    queryKey: QUERY_KEYS.LIST(params),
    queryFn: async () => (await hostBalancesApi.getAll(params)).data,
  });
}

export function useHostBalance(id: string) {
  return useQuery({
    queryKey: QUERY_KEYS.DETAIL(id),
    queryFn: async () => (await hostBalancesApi.getById(id)).data,
    enabled: Boolean(id),
  });
}

export function useBootstrapHostBalances() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async () => hostBalancesApi.bootstrap(),
    onSuccess: () => qc.invalidateQueries({ queryKey: QUERY_KEYS.ALL }),
  });
}
