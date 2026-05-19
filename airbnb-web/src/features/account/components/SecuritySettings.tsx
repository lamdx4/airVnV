import { useState } from 'react';
import { Button } from '@/components/ui/button';

import { Input } from '@/components/ui/input';
import { useSessions, useRevokeSession, useChangePassword } from '../hooks/useAccount';
import { Shield, Smartphone, Monitor, LogOut, Loader2, Key } from 'lucide-react';
import { formatDistanceToNow } from 'date-fns';
import { vi } from 'date-fns/locale';

export function SecuritySettings() {
  const { data: sessionsResponse, isLoading: sessionsLoading } = useSessions();
  const revokeMutation = useRevokeSession();
  const changePassMutation = useChangePassword();

  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const sessions = sessionsResponse || [];

  const handleRevoke = (id: string) => {
    if (confirm('Bạn có chắc chắn muốn đăng xuất thiết bị này?')) {
      revokeMutation.mutate(id);
    }
  };

  const handleChangePassword = (e: React.FormEvent) => {
    e.preventDefault();
    changePassMutation.mutate({
      currentPassword,
      newPassword,
      confirmPassword
    });
  };

  const getDeviceIcon = (ua: string | null) => {
    if (!ua) return <Monitor size={20} />;
    const lowerUa = ua.toLowerCase();
    if (lowerUa.includes('iphone') || lowerUa.includes('android')) return <Smartphone size={20} />;
    return <Monitor size={20} />;
  };

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-500">
      {/* Password Section */}
      <section className="bg-white p-8 rounded-3xl border border-slate-100 shadow-sm">
        <div className="flex items-center gap-3 mb-6">
          <div className="p-3 bg-rausch/10 rounded-2xl">
            <Key className="text-rausch" size={24} />
          </div>
          <div>
            <h3 className="text-xl font-bold text-slate-900">Đổi mật khẩu</h3>
            <p className="text-slate-500 text-sm">Đảm bảo tài khoản của bạn được an toàn</p>
          </div>
        </div>

        <form onSubmit={handleChangePassword} className="space-y-4 max-w-md">
          <div className="space-y-2">
            <label className="text-sm font-bold text-slate-700 ml-1">Mật khẩu hiện tại</label>
            <Input 
              type="password" 
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
              className="h-12 rounded-xl border-slate-200"
            />
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div className="space-y-2">
              <label className="text-sm font-bold text-slate-700 ml-1">Mật khẩu mới</label>
              <Input 
                type="password"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                className="h-12 rounded-xl border-slate-200"
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-bold text-slate-700 ml-1">Xác nhận lại</label>
              <Input 
                type="password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                className="h-12 rounded-xl border-slate-200"
              />
            </div>
          </div>
          <Button 
            type="submit" 
            disabled={changePassMutation.isPending}
            className="bg-rausch hover:bg-rose-700 text-white rounded-xl h-12 px-8 font-bold transition-all active:scale-95"
          >
            {changePassMutation.isPending ? <Loader2 className="animate-spin mr-2" size={18} /> : null}
            Cập nhật mật khẩu
          </Button>
        </form>
      </section>

      {/* Devices Section */}
      <section className="bg-white p-8 rounded-3xl border border-slate-100 shadow-sm">
        <div className="flex items-center gap-3 mb-6">
          <div className="p-3 bg-blue-50 rounded-2xl">
            <Shield className="text-blue-600" size={24} />
          </div>
          <div>
            <h3 className="text-xl font-bold text-slate-900">Thiết bị đang đăng nhập</h3>
            <p className="text-slate-500 text-sm">Quản lý các phiên đăng nhập đang hoạt động</p>
          </div>
        </div>

        {sessionsLoading ? (
          <div className="flex justify-center py-8">
            <Loader2 className="animate-spin text-rausch" size={32} />
          </div>
        ) : (
          <div className="space-y-4">
            {sessions.map((session) => (
              <div 
                key={session.id} 
                className="flex items-center justify-between p-4 rounded-2xl border border-slate-50 hover:border-slate-100 hover:bg-slate-50/50 transition-all"
              >
                <div className="flex items-center gap-4">
                  <div className="p-3 bg-slate-100 rounded-xl text-slate-600">
                    {getDeviceIcon(session.userAgent)}
                  </div>
                  <div>
                    <div className="flex items-center gap-2">
                      <span className="font-bold text-slate-900">{session.userAgent ? 'Trình duyệt Web' : 'Thiết bị lạ'}</span>
                      {session.isCurrent && (
                        <span className="px-2 py-0.5 bg-green-100 text-green-700 text-[10px] font-bold rounded-full uppercase">
                          Hiện tại
                        </span>
                      )}
                    </div>
                    <p className="text-slate-400 text-xs flex items-center gap-2">
                      {session.ipAddress} • Đăng nhập {formatDistanceToNow(new Date(session.loginAt), { addSuffix: true, locale: vi })}
                    </p>
                  </div>
                </div>
                
                {!session.isCurrent && (
                  <Button 
                    variant="ghost" 
                    onClick={() => handleRevoke(session.id)}
                    disabled={revokeMutation.isPending}
                    className="text-slate-400 hover:text-rausch hover:bg-rausch/5 rounded-xl"
                  >
                    <LogOut size={18} className="mr-2" />
                    Đăng xuất
                  </Button>
                )}
              </div>
            ))}
          </div>
        )}
      </section>
    </div>
  );
}
