import { create } from 'zustand';

interface AuthState {
  token: string | null;
  userId: string | null;
  isAuthenticated: boolean;
  login: (token: string) => void;
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
  token: localStorage.getItem('airbnb_token'),
  userId: localStorage.getItem('airbnb_user_id'),
  isAuthenticated: !!localStorage.getItem('airbnb_token'),

  login: (token: string) => {
    const claims = parseJwt(token);
    const userId = claims?.["sub"] || claims?.["X-User-Id"] || claims?.["id"] || null;

    localStorage.setItem('airbnb_token', token);
    if (userId) localStorage.setItem('airbnb_user_id', userId);

    set({ token, userId, isAuthenticated: true });
  },

  logout: () => {
    localStorage.removeItem('airbnb_token');
    localStorage.removeItem('airbnb_user_id');
    set({ token: null, userId: null, isAuthenticated: false });
  },
}));
