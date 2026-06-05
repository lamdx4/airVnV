import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { useSessions, useRevokeSession, useChangePassword } from '../hooks/useAccount';
import { Shield, Smartphone, Monitor, LogOut, Loader2, Key } from 'lucide-react';
import { formatDistanceToNow } from 'date-fns';
import { vi, enUS } from 'date-fns/locale';
import { useTranslation } from 'react-i18next';

export function SecuritySettings() {
  const { data: sessionsResponse, isLoading: sessionsLoading } = useSessions();
  const revokeMutation = useRevokeSession();
  const changePassMutation = useChangePassword();
  const { t, i18n } = useTranslation();

  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const sessions = sessionsResponse || [];
  const currentLocale = i18n.language.startsWith('vi') ? vi : enUS;

  const handleRevoke = (id: string) => {
    if (confirm(t('profile.logoutConfirm'))) {
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
            <h3 className="text-xl font-bold text-slate-900">{t('profile.changePasswordSuccess').replace('thành công', '').replace('successfully', '')}</h3>
            <p className="text-slate-500 text-sm">{t('profile.settingsSubtitle')}</p>
          </div>
        </div>

        <form onSubmit={handleChangePassword} className="space-y-4 max-w-md">
          <div className="space-y-2">
            <label className="text-sm font-bold text-slate-700 ml-1">{t('profile.currentPassword')}</label>
            <Input 
              type="password" 
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
              className="h-12 rounded-xl border-slate-200"
            />
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div className="space-y-2">
              <label className="text-sm font-bold text-slate-700 ml-1">{t('profile.newPassword')}</label>
              <Input 
                type="password"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                className="h-12 rounded-xl border-slate-200"
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-bold text-slate-700 ml-1">{t('profile.confirmPassword')}</label>
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
            {t('profile.updatePassword')}
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
            <h3 className="text-xl font-bold text-slate-900">{t('profile.activeSessions')}</h3>
            <p className="text-slate-500 text-sm">{t('profile.activeSessionsDesc')}</p>
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
                      <span className="font-bold text-slate-900">
                        {session.userAgent ? t('profile.webBrowser') : t('profile.unknownDevice')}
                      </span>
                      {session.isCurrent && (
                        <span className="px-2 py-0.5 bg-green-100 text-green-700 text-[10px] font-bold rounded-full uppercase">
                          {t('profile.currentDevice')}
                        </span>
                      )}
                    </div>
                    <p className="text-slate-400 text-xs flex items-center gap-2">
                      {session.ipAddress} • {t('header.login')} {formatDistanceToNow(new Date(session.loginAt), { addSuffix: true, locale: currentLocale })}
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
                    {t('profile.logoutDevice')}
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
