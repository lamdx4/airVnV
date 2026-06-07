export const APP_NAME = "Airbnb Admin";

export const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

export const ROUTES = {
  HOME: "/",
  LOGIN: "/login",
  FORGOT_PASSWORD: "/forgot-password",
  PROPERTIES: "/properties",
  PROPERTY_DETAIL: (id: string) => `/properties/${id}`,

  USERS: "/users",
  USER_DETAIL: (id: string) => `/users/${id}`,

  REPORTS: "/reports",

  PAYMENTS: "/payments",
  PAYMENT_DETAIL: (id: string) => `/payments/${id}`,

  PAYOUTS: "/payouts",
  PAYOUT_DETAIL: (id: string) => `/payouts/${id}`,

  HOST_BALANCES: "/host-balances",
  HOST_BALANCE_DETAIL: (id: string) => `/host-balances/${id}`,

  SETTINGS: "/settings",
} as const;

export const DEFAULT_PAGE_SIZE = 10;
export const MAX_PAGE_SIZE = 100;
