/**
 * Ánh xạ (Map) các mã iconCode thô từ database sang các định danh Iconify hợp lệ.
 * Đảm bảo các icon hiển thị mượt mà bằng lucide prefix và tránh 404 API calls.
 */
export const getIconName = (code: string | null | undefined): string => {
  if (!code) return 'hugeicons:tick-02';
  return code.includes(':') ? code : `hugeicons:${code}`;
};
