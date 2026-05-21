import type { CreatePropertyFormData, CreatePropertyRequest } from '../types';

export function toCreatePropertyRequest(
  data: CreatePropertyFormData & { displayAddress?: string }
): CreatePropertyRequest {
  const slug = data.title
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '')
    .substring(0, 60);

  return {
    title: data.title,
    description: data.description,
    slug,
    basePrice: data.basePrice,
    currencyCode: data.currencyCode,
    cleaningFee: data.cleaningFee,
    serviceFee: data.serviceFee,
    weekendPremiumPercent: data.weekendPremiumPercent,
    guestCount: data.guestCount,
    bedroomCount: data.bedroomCount,
    bedCount: data.bedCount,
    bathroomCount: data.bathroomCount,
    allowPets: data.allowPets,
    allowSmoking: data.allowSmoking,
    allowEvents: data.allowEvents,
    checkInTime: data.checkInTime,
    checkOutTime: data.checkOutTime,
    flexibleCheckOut: data.flexibleCheckOut,
    latitude: data.latitude,
    longitude: data.longitude,
    displayAddress: data.displayAddress || '',
    countryCode: data.countryCode,
    streetAddress: data.streetAddress,
    admin1Code: data.admin1Code,
    admin2Code: data.admin2Code,
  };
}