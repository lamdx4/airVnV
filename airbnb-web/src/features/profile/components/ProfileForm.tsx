import { useEffect, useState, useRef } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { useProfile, useUpdateProfile } from '../hooks/useProfile';
import { useUpload } from '@/hooks/useUpload';
import { Camera, Mail, Phone, User, Loader2 } from 'lucide-react';

export function ProfileForm() {
  const { data: profile, isLoading } = useProfile();
  const updateMutation = useUpdateProfile();
  const { uploadImage, isUploading } = useUpload();
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [fullName, setFullName] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [bio, setBio] = useState('');
  const [avatarUrl, setAvatarUrl] = useState('');

  // Đồng bộ dữ liệu khi profile được tải xong
  useEffect(() => {
    if (profile) {
      setFullName(profile.fullName || '');
      setPhoneNumber(profile.phoneNumber || '');
      setBio(profile.bio || '');
      setAvatarUrl(profile.avatarUrl || '');
    }
  }, [profile]);

  // Xử lý khi chọn ảnh mới
  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      try {
        const uploadedUrl = await uploadImage(file, 'avatars');
        setAvatarUrl(uploadedUrl);
      } catch (err) {
        // Lỗi đã được toast trong hook useUpload
      }
    }
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    updateMutation.mutate({
      fullName,
      phoneNumber,
      bio,
      avatarUrl
    });
  };

  if (isLoading) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[400px] space-y-4">
        <Loader2 className="h-10 w-10 text-rausch animate-spin" />
        <p className="text-slate-500 font-medium">Đang tải thông tin cá nhân...</p>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto">
      {/* Profile Header Card */}
      <div className="relative mb-8">
        <div className="h-48 w-full bg-gradient-to-r from-rausch to-rose-400 rounded-3xl shadow-lg overflow-hidden">
          <div className="absolute inset-0 bg-white/10 backdrop-blur-[2px]"></div>
        </div>
        
        <div className="absolute -bottom-6 left-8 flex items-end gap-6">
          <div className="relative group">
            <div className="h-32 w-32 rounded-full border-4 border-white bg-slate-100 shadow-xl overflow-hidden relative">
              <img 
                src={avatarUrl || `https://api.dicebear.com/7.x/avataaars/svg?seed=${profile?.email}`} 
                alt="Avatar" 
                className="h-full w-full object-cover"
              />
              {isUploading && (
                <div className="absolute inset-0 bg-black/40 flex items-center justify-center">
                  <Loader2 className="h-8 w-8 text-white animate-spin" />
                </div>
              )}
            </div>
            <input 
              type="file" 
              ref={fileInputRef} 
              onChange={handleFileChange} 
              className="hidden" 
              accept="image/*"
            />
            <button 
              type="button"
              onClick={() => fileInputRef.current?.click()}
              disabled={isUploading}
              className="absolute bottom-0 right-0 p-2 bg-white rounded-full shadow-lg border border-slate-100 text-slate-600 hover:text-rausch transition-colors disabled:opacity-50"
            >
              <Camera size={18} />
            </button>
          </div>
          
          <div className="mb-8">
            <h2 className="text-2xl font-bold text-white drop-shadow-md">{profile?.fullName}</h2>
            <p className="text-white/90 text-sm font-medium drop-shadow-sm flex items-center gap-1">
              <Mail size={14} /> {profile?.email}
            </p>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-8 mt-12">
        <div className="space-y-6">
          <div className="bg-white p-6 rounded-3xl border border-slate-100 shadow-sm">
            <h3 className="font-bold text-slate-900 mb-4">Thông tin tài khoản</h3>
            <div className="space-y-4">
              <div className="flex items-center justify-between text-sm">
                <span className="text-slate-500">Vai trò</span>
                <span className="px-2 py-1 bg-rausch/10 text-rausch rounded-lg font-bold text-xs uppercase">
                  {profile?.role}
                </span>
              </div>
              <div className="flex items-center justify-between text-sm">
                <span className="text-slate-500">ID Người dùng</span>
                <span className="text-slate-900 font-mono text-[10px]">{profile?.id.substring(0, 8)}...</span>
              </div>
            </div>
          </div>
        </div>

        <div className="md:col-span-2">
          <form onSubmit={handleSubmit} className="bg-white p-8 rounded-3xl border border-slate-100 shadow-sm space-y-6">
            <div className="flex items-center justify-between border-b border-slate-50 pb-4 mb-2">
              <h3 className="text-xl font-bold text-slate-900">Chỉnh sửa hồ sơ</h3>
              <p className="text-slate-400 text-xs">Cập nhật thông tin công khai của bạn</p>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
              <div className="space-y-2">
                <label className="text-sm font-bold text-slate-700 ml-1 flex items-center gap-2">
                  <User size={16} className="text-slate-400" /> Họ và tên
                </label>
                <Input 
                  value={fullName}
                  onChange={(e) => setFullName(e.target.value)}
                  placeholder="Nhập họ tên đầy đủ"
                  className="h-12 rounded-xl border-slate-200 focus:border-rausch focus:ring-rausch"
                  required
                />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-bold text-slate-700 ml-1 flex items-center gap-2">
                  <Phone size={16} className="text-slate-400" /> Số điện thoại
                </label>
                <Input 
                  value={phoneNumber}
                  onChange={(e) => setPhoneNumber(e.target.value)}
                  placeholder="Nhập số điện thoại"
                  className="h-12 rounded-xl border-slate-200 focus:border-rausch focus:ring-rausch"
                />
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-sm font-bold text-slate-700 ml-1">Giới thiệu bản thân</label>
              <Textarea 
                value={bio}
                onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) => setBio(e.target.value)}
                placeholder="Hãy chia sẻ một chút về bản thân bạn..."
                className="min-h-[120px] rounded-xl border-slate-200 focus:border-rausch focus:ring-rausch resize-none"
              />
            </div>

            <div className="pt-4 flex justify-end gap-4">
              <Button 
                type="button" 
                variant="outline" 
                onClick={() => profile && setAvatarUrl(profile.avatarUrl || '')}
                className="h-12 px-8 rounded-xl border-slate-200 hover:bg-slate-50 text-slate-600 font-semibold"
              >
                Hủy bỏ
              </Button>
              <Button 
                type="submit" 
                disabled={updateMutation.isPending || isUploading}
                className="h-12 px-8 rounded-xl bg-rausch hover:bg-rose-700 text-white font-bold shadow-md shadow-rausch/20 transition-all active:scale-[98%]"
              >
                {updateMutation.isPending ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Đang lưu...
                  </>
                ) : 'Lưu thay đổi'}
              </Button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
