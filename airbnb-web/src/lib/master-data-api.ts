import { api } from './api';

export interface SupportedCountry {
  code: string;
  name: string;
  nativeCurrency: string;
  defaultLatitude: number;
  defaultLongitude: number;
}

export const masterDataApi = {
  getSupportedCountries: (): Promise<SupportedCountry[]> =>
    api.get('/api/properties/countries') as any,
};
