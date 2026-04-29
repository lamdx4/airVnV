import { create } from 'zustand';

interface AuthState {
  accessToken: string | null;
  refreshToken: string | null;
  userId: string | null;
  isAuthenticated: boolean;
  login: (accessToken: string, refreshToken: string) => void;
  logout: () => void;
}

// Helper parse JWT claims không cần thư viện ngoài
function parseJwt(token: string) {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      window
        .atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch (e) {
    return null;
  }
}

export const useAuthStore = create<AuthState>((set) => ({
  accessToken: localStorage.getItem('airbnb_access_token'),
  refreshToken: localStorage.getItem('airbnb_refresh_token'),
  userId: localStorage.getItem('airbnb_user_id'),
  isAuthenticated: !!localStorage.getItem('airbnb_access_token'),

  login: (accessToken: string, refreshToken: string) => {
    const claims = parseJwt(accessToken);
    const userId = claims?.["UserId"] || claims?.["sub"] || claims?.["id"] || null;

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
