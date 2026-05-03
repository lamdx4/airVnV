import { ProfileForm } from '@/features/profile/components/ProfileForm';

export default function Profile() {
  return (
    <div className="min-h-screen bg-slate-50/50 py-12">
      <div className="container mx-auto px-4">
        <div className="mb-10 text-center md:text-left">
          <h1 className="text-3xl font-extrabold text-slate-900 tracking-tight">Hồ sơ cá nhân</h1>
          <p className="text-slate-500 mt-2">Quản lý thông tin và các thiết lập bảo mật của bạn</p>
        </div>
        
        <ProfileForm />
      </div>
    </div>
  );
}
