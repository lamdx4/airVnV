import { useState } from 'react';
import { getUploadSignature, uploadToCloudinary } from '@/features/media/api/media';
import { toast } from 'sonner';

export const useUpload = () => {
  const [isUploading, setIsUploading] = useState(false);

  const uploadImage = async (file: File, folder: string = 'avatars') => {
    try {
      setIsUploading(true);

      // Bước 1: Lấy chữ ký từ Backend
      const signature = await getUploadSignature(folder);

      // Bước 2: Upload trực tiếp lên Cloudinary
      const result = await uploadToCloudinary(file, signature);

      return result.secure_url;
    } catch (error: any) {
      console.error('Upload error:', error);
      toast.error('Lỗi khi tải ảnh lên. Vui lòng thử lại!');
      throw error;
    } finally {
      setIsUploading(false);
    }
  };

  return {
    uploadImage,
    isUploading
  };
};
