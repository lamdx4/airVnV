import { api } from "@/lib/api";

import type {
  HostBalanceDetailResponse,
  HostBalanceListParams,
  HostBalancesListResponse,
} from "../types";

export const hostBalancesApi = {
  getAll: (params?: HostBalanceListParams) =>
    api.get<HostBalancesListResponse>("/admin/host-balances", { params }),

  getById: (id: string) =>
    api.get<HostBalanceDetailResponse>(`/admin/host-balances/${id}`),

  bootstrap: () => api.post("/admin/host-balances/bootstrap", {}),
};
