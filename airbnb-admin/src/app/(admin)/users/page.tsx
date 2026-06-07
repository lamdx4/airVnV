"use client";

import { UsersList } from "@/features/users/components/users-list";

export default function UsersPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-[28px] font-bold text-[#222222]">Users</h1>
        <p className="text-sm text-[#6a6a6a] mt-1">
          Manage user accounts and handle account status.
        </p>
      </div>
      <UsersList />
    </div>
  );
}
