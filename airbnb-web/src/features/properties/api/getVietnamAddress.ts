import axios from "axios";
import type { ProvinceResponse } from "../types/vietnam-address";

export const getVietnamAddress = async (): Promise<ProvinceResponse[]> => {
  const response = await axios.get<ProvinceResponse[]>("https://provinces.open-api.vn/api/v2/?depth=2");
  return response.data;
};
