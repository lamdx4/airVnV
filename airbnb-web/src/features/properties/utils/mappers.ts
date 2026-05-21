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
  formData: Partial<CreatePropertyFormData> & { displayAddress: string; subDivisions?: Record<string, string> }
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

    // Location
    latitude: Number(formData.latitude || 0),
    longitude: Number(formData.longitude || 0),
    displayAddress: formData.displayAddress,
    countryCode: formData.countryCode || 'US',
    streetAddress: formData.streetAddress || '',
    admin1Code: formData.admin1Code || undefined,
    admin2Code: formData.admin2Code || undefined,
    subDivisions: formData.subDivisions || undefined
  };
}
