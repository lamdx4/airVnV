import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { getProfile, updateProfile } from '../api/profile';
import type { UpdateProfileRequest } from '../types';
import { toast } from 'sonner';
import { useTranslation } from 'react-i18next';

export const useProfile = () => {
  return useQuery({
    queryKey: ['profile'],
    queryFn: getProfile,
  });
};

export const useUpdateProfile = () => {
  const queryClient = useQueryClient();
  const { t } = useTranslation();

  return useMutation({
    mutationFn: (data: UpdateProfileRequest) => updateProfile(data),
    onSuccess: () => {
      toast.success(t('profile.successUpdate'));
      // Refresh lại dữ liệu profile sau khi cập nhật
      void queryClient.invalidateQueries({ queryKey: ['profile'] });
    },
    onError: (error: any) => {
      toast.error(error.message || t('profile.failedUpdate'));
    }
  });
};
