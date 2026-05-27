import type { CreatePropertyFormData, CreatePropertyRequest } from '../types';

/**
 * Tạo slug tự động từ tiêu đề
 */
function slugify(text: string): string {
  return text
    .toString()
    .toLowerCase()
    .normalize('NFD') // Chuẩn hóa ký tự có dấu
    .replace(/[\u0300-\u036f]/g, '') // Xóa các dấu
    .replace(/[đĐ]/g, 'd') // Thay thế chữ đ/Đ
    .replace(/[^a-z0-9 -]/g, '') // Xóa ký tự đặc biệt
    .replace(/\s+/g, '-') // Thay khoảng trắng bằng dấu -
    .replace(/-+/g, '-') // Xóa các dấu - liên tiếp
    .replace(/^-+/, '') // Xóa dấu - ở đầu
    .replace(/-+$/, ''); // Xóa dấu - ở cuối
}

/**
 * Ánh xạ dữ liệu Form của Frontend sang Command Request phẳng gửi lên Backend API.
 */
export function toCreatePropertyRequest(
  formData: Partial<CreatePropertyFormData> & { displayAddress: string; subDivisions?: Record<string, string>; amenityIds?: string[] }
): CreatePropertyRequest {
  return {
    title: formData.title || '',
    description: formData.description || '',
    slug: slugify(formData.title || '') + '-' + Math.random().toString(36).substring(2, 9), // Auto slug có entropy tránh trùng lặp
    
    // Pricing
    basePrice: Number(formData.basePrice || 0),
    currencyCode: formData.currencyCode || 'USD',
    cleaningFee: Number(formData.cleaningFee || 0),
    serviceFee: Number(formData.serviceFee || 0),
    weekendPremiumPercent: Number(formData.weekendPremiumPercent || 0),

    // Capacity
    guestCount: Number(formData.guestCount || 1),
    bedroomCount: Number(formData.bedroomCount || 1),
    bedCount: Number(formData.bedCount || 1),
    bathroomCount: Number(formData.bathroomCount || 1),

    // House Rules
    allowPets: Boolean(formData.allowPets || false),
    allowSmoking: Boolean(formData.allowSmoking || false),
    allowEvents: Boolean(formData.allowEvents || false),
    checkInTime: formData.checkInTime || '14:00',
    checkOutTime: formData.checkOutTime || '12:00',
    flexibleCheckOut: Boolean(formData.flexibleCheckOut || false),
    customRules: formData.customRules || [],

    // Location
    latitude: Number(formData.latitude || 0),
    longitude: Number(formData.longitude || 0),
    displayAddress: formData.displayAddress,
    countryCode: formData.countryCode || 'US',
    streetAddress: formData.streetAddress || '',
    admin1Code: formData.admin1Code || undefined,
    admin2Code: formData.admin2Code || undefined,
    unit: formData.subDivisions?.unit || undefined,
    subDivisions: formData.subDivisions || undefined,
    amenityIds: formData.amenityIds || []
  };
}
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