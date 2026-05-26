import { UserManagement } from '@/features/admin/users/components/UserManagement';

export default function UsersPage() {
  return (
    <div className="container max-w-5xl mx-auto px-4 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-semibold text-[#222222]">User Management</h1>
        <p className="text-[#717171] mt-1">Manage user accounts, KYC verification, and permissions</p>
      </div>

      <UserManagement />
    </div>
  );
}