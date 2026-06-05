"use client";

import { Bell, Search } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

export function AdminHeader() {
  return (
    <header className="flex h-20 items-center justify-between border-b border-[#dddddd] bg-white px-8">
      <div className="relative w-80">
        <Search className="absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-[#6a6a6a]" />
        <Input
          placeholder="Search..."
          className="h-12 rounded-full border-[#dddddd] bg-[#f7f7f7] pl-11 pr-4 text-sm text-[#222222] placeholder:text-[#929292] focus-visible:border-[#222222] focus-visible:bg-white"
        />
      </div>

      <div className="flex items-center gap-2">
        <Button variant="ghost" size="icon" title="Notifications" className="h-10 w-10 rounded-full text-[#6a6a6a] hover:bg-[#f7f7f7] hover:text-[#222222]">
          <Bell className="h-[18px] w-[18px]" />
        </Button>
      </div>
    </header>
  );
}
