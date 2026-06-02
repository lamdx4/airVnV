export const APP_NAME = "Airbnb Admin";

export const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

export const ROUTES = {
  HOME: "/",
  LOGIN: "/login",
  FORGOT_PASSWORD: "/forgot-password",
  DASHBOARD: "/dashboard",
  PROPERTIES: "/properties",
  PROPERTY_DETAIL: (id: string) => `/properties/${id}`,
  BOOKINGS: "/bookings",
  BOOKING_DETAIL: (id: string) => `/bookings/${id}`,
  USERS: "/users",
  USER_DETAIL: (id: string) => `/users/${id}`,
  PAYMENTS: "/payments",
  PAYMENT_DETAIL: (id: string) => `/payments/${id}`,
  REVIEWS: "/reviews",
  REPORTS: "/reports",
  SETTINGS: "/settings",
  SETTINGS_PROFILE: "/settings/profile",
  SETTINGS_SYSTEM: "/settings/system",
} as const;

export const DEFAULT_PAGE_SIZE = 20;
export const MAX_PAGE_SIZE = 100;
