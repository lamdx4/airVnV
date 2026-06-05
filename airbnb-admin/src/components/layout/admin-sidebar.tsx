"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { LogOut, User } from "lucide-react";
import { toast } from "sonner";

import { sidebarNav } from "@/config/navigation";
import { authApi } from "@/lib/auth";
import { useAuthStore } from "@/store";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";

export function AdminSidebar() {
  const pathname = usePathname();
  const router = useRouter();
  const { user, clearAuth } = useAuthStore();

  async function handleLogout() {
    try {
      await authApi.logout();
    } catch {
      // Ignore logout API errors — clear local state regardless
    }
    clearAuth();
    toast.success("Logged out successfully");
    router.replace("/login");
  }

  return (
    <aside className="flex h-full w-64 flex-col border-r border-[#dddddd] bg-white">
      <div className="flex h-20 items-center border-b border-[#dddddd] px-6">
        <Link href="/dashboard" className="flex items-center gap-2">
          <svg viewBox="0 0 32 32" xmlns="http://www.w3.org/2000/svg" aria-hidden="true" role="presentation" focusable="false" className="h-8 w-8">
            <path d="M16 1c2.008 0 3.463.963 4.751 3.269l.533 1.022c1.158 2.244 2.944 4.67 5.224 7.263l.654.727.835.916c2.085 2.3 3.179 4.186 3.332 5.72l.012.214.006.288c0 4.062-2.877 6.831-7.027 6.831-1.586 0-3.163-.465-4.638-1.322l-.227-.14-.277-.178-.246-.165-.136-.097-.276-.203-.166-.13c-.198-.156-.327-.272-.393-.348l-.035-.042-.035.042c-.066.076-.195.192-.393.348l-.166.13-.276.203-.136.097-.246.165-.277.178-.227.14c-1.475.857-3.052 1.322-4.638 1.322-4.15 0-7.027-2.769-7.027-6.831 0-1.694.864-3.536 2.477-5.59l.378-.465.835-.916.654-.727c2.28-2.593 4.066-5.02 5.224-7.263l.533-1.022C12.537 1.963 13.992 1 16 1z" fill="#ff385c"/>
          </svg>
          <span className="text-xl font-semibold text-[#222222] tracking-tight">
            Admin
          </span>
        </Link>
      </div>

      <nav className="flex-1 space-y-0.5 p-3">
        {sidebarNav.map((item) => {
          const isActive = pathname === item.href || pathname.startsWith(`${item.href}/`);
          return (
            <Link
              key={item.href}
              href={item.href}
              className={cn(
                "flex items-center gap-3 rounded-[8px] px-3 py-2.5 text-sm font-medium transition-colors",
                isActive
                  ? "bg-[#ff385c] text-white"
                  : "text-[#6a6a6a] hover:bg-[#f7f7f7] hover:text-[#222222]",
              )}
            >
              <item.icon className="h-[18px] w-[18px]" />
              {item.label}
              {item.badge && (
                <span className="ml-auto rounded-full bg-[#ff385c] px-2 py-0.5 text-[11px] font-semibold text-white">
                  {item.badge}
                </span>
              )}
            </Link>
          );
        })}
      </nav>

      <div className="border-t border-[#dddddd] p-4">
        <div className="flex items-center gap-3">
          <div className="flex h-9 w-9 items-center justify-center rounded-full bg-[#f7f7f7]">
            <User className="h-4 w-4 text-[#6a6a6a]" />
          </div>
          <div className="flex-1 truncate">
            <p className="text-sm font-medium text-[#222222] truncate">{user?.fullName ?? "Admin"}</p>
            <p className="text-xs text-[#6a6a6a] truncate">{user?.email ?? ""}</p>
          </div>
          <Button variant="ghost" size="icon-sm" onClick={handleLogout} title="Logout" className="text-[#6a6a6a] hover:text-[#222222]">
            <LogOut className="h-4 w-4" />
          </Button>
        </div>
      </div>
    </aside>
  );
}
