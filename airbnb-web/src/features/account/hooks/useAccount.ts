import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getSessions, revokeSession, changePassword } from '../api/accountApi';
import { toast } from 'sonner';

export const useSessions = () => {
  return useQuery({
    queryKey: ['sessions'],
    queryFn: getSessions
  });
};

export const useRevokeSession = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: revokeSession,
    onSuccess: () => {
      toast.success('Đã đăng xuất thiết bị');
      queryClient.invalidateQueries({ queryKey: ['sessions'] });
    },
    onError: (error: any) => {
      toast.error(error.message || 'Đăng xuất thất bại');
    }
  });
};

export const useChangePassword = () => {
  return useMutation({
    mutationFn: changePassword,
    onSuccess: () => {
      toast.success('Đổi mật khẩu thành công');
    },
    onError: (error: any) => {
      toast.error(error.message || 'Đổi mật khẩu thất bại');
    }
  });
};
