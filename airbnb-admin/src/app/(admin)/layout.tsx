"use client";

import { AdminSidebar, AdminHeader } from "@/components/layout";

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex h-screen overflow-hidden bg-white">
      <AdminSidebar />
      <div className="flex flex-1 flex-col overflow-hidden">
        <AdminHeader />
        <main className="flex-1 overflow-y-auto bg-[#f7f7f7] p-8">{children}</main>
      </div>
    </div>
  );
}
