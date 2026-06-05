import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getSessions, revokeSession, changePassword } from '../api/accountApi';
import { toast } from 'sonner';
import { useTranslation } from 'react-i18next';

export const useSessions = () => {
  return useQuery({
    queryKey: ['sessions'],
    queryFn: getSessions
  });
};

export const useRevokeSession = () => {
  const queryClient = useQueryClient();
  const { t } = useTranslation();
  return useMutation({
    mutationFn: revokeSession,
    onSuccess: () => {
      toast.success(t('profile.logoutSuccess'));
      queryClient.invalidateQueries({ queryKey: ['sessions'] });
    },
    onError: (error: any) => {
      toast.error(error.message || t('profile.logoutFailed'));
    }
  });
};

export const useChangePassword = () => {
  const { t } = useTranslation();
  return useMutation({
    mutationFn: changePassword,
    onSuccess: () => {
      toast.success(t('profile.changePasswordSuccess'));
    },
    onError: (error: any) => {
      toast.error(error.message || t('profile.changePasswordFailed'));
    }
  });
};
