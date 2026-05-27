import { useState, useEffect } from 'react';
import { Icon, loadIcon } from '@iconify/react';

interface SafeIconProps {
  icon: string | null | undefined;
  className?: string;
  fallback?: string;
}

/**
 * Component hiển thị Icon thông minh và an toàn.
 * Tự động thăm dò (probe) bất đồng bộ các namespace phổ biến (hugeicons, lucide)
 * trên Iconify API để tìm ra icon hợp lệ, tránh lỗi 404 hiển thị trống.
 */
export function SafeIcon({ icon, className, fallback = 'hugeicons:tick-02' }: SafeIconProps) {
  const [resolvedIcon, setResolvedIcon] = useState<string>(fallback);

  useEffect(() => {
    if (!icon) {
      setResolvedIcon(fallback);
      return;
    }

    const code = icon.trim();

    // Nếu iconCode đã có chứa prefix chuẩn (ví dụ 'lucide:wifi'), sử dụng ngay lập tức
    if (code.includes(':')) {
      setResolvedIcon(code);
      return;
    }

    const prefixes = ['hugeicons', 'lucide'];
    let isMounted = true;

    async function probeIcon() {
      for (const prefix of prefixes) {
        const fullName = `${prefix}:${code}`;
        try {
          // Thử load icon bất đồng bộ từ registry của Iconify
          await loadIcon(fullName);
          if (isMounted) {
            setResolvedIcon(fullName);
            return; // Đã tìm thấy icon khả dụng, dừng vòng lặp!
          }
        } catch {
          // Nếu bị 404, bỏ qua và thử prefix tiếp theo trong danh sách
        }
      }

      // Nếu duyệt qua toàn bộ prefix mà không tìm thấy icon hợp lệ, dùng fallback
      if (isMounted) {
        setResolvedIcon(fallback);
      }
    }

    probeIcon();

    return () => {
      isMounted = false;
    };
  }, [icon, fallback]);

  return <Icon icon={resolvedIcon} className={className} />;
}
