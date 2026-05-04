import type { CreatePropertyFormData, CreatePropertyRequest } from '../types';

/**
 * Mapper chuyển đổi dữ liệu từ Form sang định dạng Request của Backend.
 * Đảm bảo đồng bộ với cấu hình API mới nhất.
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
    countryCode: 'VN',
    displayAddress: formData.displayAddress,
    streetAddress: formData.displayAddress,
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
