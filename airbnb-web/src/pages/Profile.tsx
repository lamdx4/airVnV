import { useState } from 'react';
import { ProfileForm } from '@/features/profile/components/ProfileForm';
import { SecuritySettings } from '@/features/account/components/SecuritySettings';
import { User, ShieldCheck } from 'lucide-react';
import { useTranslation } from 'react-i18next';

export default function Profile() {
  const [activeTab, setActiveTab] = useState<'profile' | 'security'>('profile');
  const { t } = useTranslation();

  return (
    <div className="min-h-screen bg-slate-50/30 py-12">
      <div className="container mx-auto px-4 max-w-5xl">
        <div className="flex flex-col md:flex-row md:items-end justify-between gap-6 mb-10">
          <div>
            <h1 className="text-4xl font-black text-slate-900 tracking-tight">{t('profile.settingsTitle')}</h1>
            <p className="text-slate-500 mt-2 font-medium">{t('profile.settingsSubtitle')}</p>
          </div>

          <div className="flex bg-white p-1.5 rounded-2xl shadow-sm border border-slate-100">
            <button
              onClick={() => setActiveTab('profile')}
              className={`flex items-center gap-2 px-6 py-2.5 rounded-xl text-sm font-bold transition-all ${
                activeTab === 'profile' 
                ? 'bg-rausch text-white shadow-md shadow-rausch/20' 
                : 'text-slate-500 hover:text-slate-700 hover:bg-slate-50'
              }`}
            >
              <User size={18} />
              {t('profile.profileTab')}
            </button>
            <button
              onClick={() => setActiveTab('security')}
              className={`flex items-center gap-2 px-6 py-2.5 rounded-xl text-sm font-bold transition-all ${
                activeTab === 'security' 
                ? 'bg-rausch text-white shadow-md shadow-rausch/20' 
                : 'text-slate-500 hover:text-slate-700 hover:bg-slate-50'
              }`}
            >
              <ShieldCheck size={18} />
              {t('profile.securityTab')}
            </button>
          </div>
        </div>
        
        <div className="mt-4">
          {activeTab === 'profile' ? <ProfileForm /> : <SecuritySettings />}
        </div>
      </div>
    </div>
  );
}
