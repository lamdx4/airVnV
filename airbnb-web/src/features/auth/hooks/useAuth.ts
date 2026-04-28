import { useMutation } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '@/store/authStore';
import { loginUser, registerUser } from '../api/auth';
import type { LoginRequest, RegisterRequest } from '../types';

export const useLogin = () => {
  const navigate = useNavigate();
  const loginStore = useAuthStore();

  return useMutation({
    mutationFn: (data: LoginRequest) => loginUser(data),
    onSuccess: (data) => {
      if (data.token) {
        loginStore.login(data.token);
        navigate('/');
      }
    },
  });
};

export const useRegister = () => {
  const navigate = useNavigate();

  return useMutation({
    mutationFn: (data: RegisterRequest) => registerUser(data),
    onSuccess: () => {
      navigate('/login');
    },
  });
};
