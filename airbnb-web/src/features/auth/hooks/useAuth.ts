import { useMutation } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '@/store/authStore';
import { loginUser, registerUser, verifyEmail } from '../api/auth';
import type { LoginRequest, RegisterRequest, VerifyEmailRequest } from '../types';

export const useLogin = () => {
  const navigate = useNavigate();
  const loginStore = useAuthStore();

  return useMutation({
    mutationFn: (data: LoginRequest) => loginUser(data),
    onSuccess: (data) => {
      if (data.accessToken && data.refreshToken) {
        loginStore.login(data.accessToken, data.refreshToken);
        navigate('/');
      }
    },
  });
};

export const useRegister = () => {
  return useMutation({
    mutationFn: (data: RegisterRequest) => registerUser(data),
  });
};

export const useVerifyEmail = () => {
  const navigate = useNavigate();
  const loginStore = useAuthStore();

  return useMutation({
    mutationFn: (data: VerifyEmailRequest) => verifyEmail(data),
    onSuccess: (data) => {
      if (data.accessToken && data.refreshToken) {
        loginStore.login(data.accessToken, data.refreshToken);
        navigate('/');
      }
    },
  });
};

export const useGoogleAuth = () => {
  const navigate = useNavigate();
  const loginStore = useAuthStore();

  return useMutation({
    mutationFn: (data: import('../types').GoogleAuthRequest) => googleAuthUser(data),
    onSuccess: (data) => {
      if (data.accessToken && data.refreshToken) {
        loginStore.login(data.accessToken, data.refreshToken);
        navigate('/');
      }
    },
  });
};
