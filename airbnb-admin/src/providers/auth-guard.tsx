"use client";

import { useEffect, useRef, useState } from "react";
import { usePathname, useRouter } from "next/navigation";

import { useAuthStore } from "@/store";
import { ROUTES } from "@/config/constants";
import { PageLoader } from "@/components/common/loading";

const AUTH_ROUTES = ["/login", "/forgot-password"];

export function AuthGuard({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const pathname = usePathname();
  const { isAuthenticated, hydrate } = useAuthStore();
  const hasHydrated = useRef(false);
  const [hydrated, setHydrated] = useState(false);

  useEffect(() => {
    hydrate();
    hasHydrated.current = true;
    setHydrated(true);
  }, [hydrate]);

  const isAuthRoute = AUTH_ROUTES.some((route) => pathname.startsWith(route));

  useEffect(() => {
    if (!hydrated) return;

    if (!isAuthenticated && !isAuthRoute) {
      router.replace(`${ROUTES.LOGIN}?redirect=${encodeURIComponent(pathname)}`);
    } else if (isAuthenticated && isAuthRoute) {
      router.replace(ROUTES.PROPERTIES);
    }
  }, [isAuthenticated, isAuthRoute, pathname, router, hydrated]);

  if (!hydrated) {
    return <PageLoader text="Loading..." />;
  }

  return <>{children}</>;
}
