"use client";

import { UsersList } from "@/features/users/components/users-list";

export default function UsersPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Users</h1>
        <p className="text-sm text-muted-foreground">
          Manage user accounts, review KYC documents, and handle account status.
        </p>
      </div>
      <UsersList />
    </div>
  );
}
