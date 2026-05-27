import { useQuery } from "@tanstack/react-query";
import { getVietnamAddress } from "../api/getVietnamAddress";
import type { ProvinceResponse } from "../types/vietnam-address";

export const useVietnamAddress = () => {
  return useQuery<ProvinceResponse[]>({
    queryKey: ["vietnam-address"],
    queryFn: getVietnamAddress,
    staleTime: Infinity, // Address data rarely changes, keep it cached
    gcTime: Infinity,
  });
};
