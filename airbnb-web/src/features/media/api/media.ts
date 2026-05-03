import { api } from '@/lib/api';
import type { SignatureResponse, CloudinaryUploadResponse } from '../types';
import axios from 'axios';

export const getUploadSignature = async (folder: string): Promise<SignatureResponse> => {
  return await api.get('/api/users/media/signature', { params: { folder } });
};

export const uploadToCloudinary = async (
  file: File, 
  sig: SignatureResponse
): Promise<CloudinaryUploadResponse> => {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('api_key', sig.apiKey);
  formData.append('timestamp', sig.timestamp.toString());
  formData.append('signature', sig.signature);
  formData.append('folder', sig.folder);

  const res = await axios.post<CloudinaryUploadResponse>(
    `https://api.cloudinary.com/v1_1/${sig.cloudName}/image/upload`,
    formData
  );

  return res.data;
};
