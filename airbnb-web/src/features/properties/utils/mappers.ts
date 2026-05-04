import { CreatePropertyFormData, CreatePropertyRequest } from '../types';

/**
 * Mapper chuyển đổi dữ liệu từ Form sang định dạng Request của Backend.
 * Không dùng 'any', đảm bảo Type Safety.
 */
export const toCreatePropertyRequest = (formData: CreatePropertyFormData): CreatePropertyRequest => {
  return {
    title: formData.title,
    description: formData.description,
    slug: formData.title
      .toLowerCase()
      .trim()
      .replace(/\s+/g, '-')
      .replace(/[^\w-]+/g, ''),
    latitude: formData.latitude,
    longitude: formData.longitude,
    countryCode: 'VN', // Có thể lấy từ Geocoding sau này
    displayAddress: formData.displayAddress,
    addressRaw: {
      street: formData.displayAddress,
      city: 'Ho Chi Minh City', // Giả định theo latitude/longitude
      state: 'HCMC',
      country: 'Vietnam',
      zipCode: '70000',
    },
    pricing: {
      basePrice: formData.basePrice,
      currencyCode: 'USD',
      cleaningFee: 0,
      serviceFee: 0,
      weekendPremiumPercent: 0,
    },
    capacity: {
      guestCount: 2,
      bedroomCount: 1,
      bedCount: 1,
      bathroomCount: 1,
    },
    houseRules: {
      allowPets: false,
      allowSmoking: false,
      allowEvents: false,
      checkInTime: '14:00',
      checkOutTime: '12:00',
      flexibleCheckOut: false,
    },
  };
};
