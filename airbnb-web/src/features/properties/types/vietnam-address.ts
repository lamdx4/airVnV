// features/properties/types/vietnam-address.ts

// Interface for Ward response
export interface WardResponse {
  name: string;
  code: number;
  division_type: string;
  codename: string;
  province_code: number;
}

// Interface for Province response
export interface ProvinceResponse {
  name: string;
  code: number;
  division_type: string;
  codename: string;
  phone_code: number;
  wards?: WardResponse[];
}
