import axios from 'axios';

const PROVINCES_API_URL = 'https://provinces.open-api.vn/api/v2';

export interface ProvinceResponse {
  name: string;
  code: number;
  division_type: string;
  codename: string;
  phone_code: number;
}

export interface WardResponse {
  name: string;
  code: number;
  division_type: string;
  codename: string;
  province_code: number;
}

/**
 * Fetches the list of all provinces in Vietnam.
 */
export const fetchProvinces = async (): Promise<ProvinceResponse[]> => {
  const response = await axios.get<ProvinceResponse[]>(`${PROVINCES_API_URL}/p/`);
  return response.data;
};

/**
 * Fetches the list of wards for a specific province or globally if searched.
 */
export const fetchWards = async (provinceCode?: number, search?: string): Promise<WardResponse[]> => {
  const params = new URLSearchParams();
  if (provinceCode) params.append('province', provinceCode.toString());
  if (search) params.append('search', search);

  const response = await axios.get<WardResponse[]>(`${PROVINCES_API_URL}/w/?${params.toString()}`);
  return response.data;
};
