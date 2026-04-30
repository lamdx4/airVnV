import { create } from 'zustand';
import { jwtDecode } from 'jwt-decode';

interface AuthState {
  accessToken: string | null;
  refreshToken: string | null;
  userId: string | null;
  isAuthenticated: boolean;
  login: (accessToken: string, refreshToken: string) => void;
  logout: () => void;
}

// Parse JWT claims safely
function extractUserId(token: string): string | null {
  try {
    const decoded = jwtDecode<Record<string, unknown>>(token);
    return (decoded?.UserId ?? decoded?.sub ?? decoded?.id) as string | null;
  } catch (error) {
    console.warn('Failed to decode JWT token:', error);
    return null;
  }
}

export const useAuthStore = create<AuthState>((set) => ({
  accessToken: localStorage.getItem('airbnb_access_token'),
  refreshToken: localStorage.getItem('airbnb_refresh_token'),
  userId: localStorage.getItem('airbnb_user_id'),
  isAuthenticated: !!localStorage.getItem('airbnb_access_token'),

  login: (accessToken: string, refreshToken: string) => {
    const userId = extractUserId(accessToken);

    localStorage.setItem('airbnb_access_token', accessToken);
    localStorage.setItem('airbnb_refresh_token', refreshToken);
    if (userId) localStorage.setItem('airbnb_user_id', userId);

    set({ accessToken, refreshToken, userId, isAuthenticated: true });
  },

  logout: () => {
    localStorage.removeItem('airbnb_access_token');
    localStorage.removeItem('airbnb_refresh_token');
    localStorage.removeItem('airbnb_user_id');
    set({ accessToken: null, refreshToken: null, userId: null, isAuthenticated: false });
  },
}));
