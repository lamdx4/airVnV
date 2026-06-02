import { create } from "zustand";

interface AdminUser {
  id: string;
  email: string;
  fullName: string;
  role: string;
  avatarUrl?: string;
}

interface AuthState {
  user: AdminUser | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  setAuth: (user: AdminUser, accessToken: string, refreshToken: string) => void;
  clearAuth: () => void;
  hydrate: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  accessToken: null,
  isAuthenticated: false,

  setAuth: (user, accessToken, refreshToken) => {
    localStorage.setItem("admin_access_token", accessToken);
    localStorage.setItem("admin_refresh_token", refreshToken);
    set({ user, accessToken, isAuthenticated: true });
  },

  clearAuth: () => {
    localStorage.removeItem("admin_access_token");
    localStorage.removeItem("admin_refresh_token");
    set({ user: null, accessToken: null, isAuthenticated: false });
  },

  hydrate: () => {
    const token = localStorage.getItem("admin_access_token");
    const userJson = localStorage.getItem("admin_user");
    if (token && userJson) {
      try {
        const user = JSON.parse(userJson) as AdminUser;
        set({ user, accessToken: token, isAuthenticated: true });
      } catch {
        localStorage.removeItem("admin_access_token");
        localStorage.removeItem("admin_refresh_token");
        localStorage.removeItem("admin_user");
      }
    }
  },
}));
