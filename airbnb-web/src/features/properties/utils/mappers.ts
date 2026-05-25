import type { CreatePropertyFormData, CreatePropertyRequest } from '../types';

export function toCreatePropertyRequest(
  data: Partial<CreatePropertyFormData> & { 
    title: string;
    displayAddress?: string;
    latitude: number;
    longitude: number;
  }
): CreatePropertyRequest {
  const slug = data.title
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '')
    .substring(0, 60);

  return {
    title: data.title,
    description: data.description ?? '',
    slug,
    basePrice: data.basePrice ?? 50,
    currencyCode: data.currencyCode ?? 'USD',
    cleaningFee: data.cleaningFee ?? 0,
    serviceFee: data.serviceFee ?? 0,
    weekendPremiumPercent: data.weekendPremiumPercent ?? 0,
    guestCount: data.guestCount ?? 1,
    bedroomCount: data.bedroomCount ?? 1,
    bedCount: data.bedCount ?? 1,
    bathroomCount: data.bathroomCount ?? 1,
    allowPets: data.allowPets ?? false,
    allowSmoking: data.allowSmoking ?? false,
    allowEvents: data.allowEvents ?? false,
    checkInTime: data.checkInTime ?? '15:00',
    checkOutTime: data.checkOutTime ?? '11:00',
    flexibleCheckOut: data.flexibleCheckOut ?? false,
    latitude: data.latitude,
    longitude: data.longitude,
    displayAddress: data.displayAddress || '',
    countryCode: data.countryCode ?? 'VN',
    streetAddress: data.streetAddress ?? '',
    admin1Code: data.admin1Code,
    admin2Code: data.admin2Code,
  };
}