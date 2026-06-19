import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Search01Icon, Globe02Icon, Menu01Icon, UserIcon } from 'hugeicons-react';
import { useAuthStore } from '@/store/authStore';
import { Icon } from '@iconify/react';
import { motion, AnimatePresence } from 'framer-motion';
import { useTranslation } from 'react-i18next';
import { useQueryClient } from '@tanstack/react-query';
import { SearchConsole } from './SearchConsole';

export default function Header() {
  const navigate = useNavigate();
  const location = useLocation();
  const { t, i18n } = useTranslation();
  const queryClient = useQueryClient();
  const { isAuthenticated, logout } = useAuthStore();
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isLangOpen, setIsLangOpen] = useState(false);
  const [isScrolled, setIsScrolled] = useState(false);
  const [isSearchExpanded, setIsSearchExpanded] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      setIsScrolled(window.scrollY > 0);
    };
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  const isHome = location.pathname === '/';

  return (
    <header 
      className={`sticky top-0 z-50 w-full transition-all duration-200 border-b ${
        isScrolled || !isHome 
          ? 'bg-white shadow-sm border-slate-200' 
          : 'bg-white border-transparent'
      }`}
    >
      <div className="max-w-[2520px] mx-auto xl:px-20 md:px-10 sm:px-2 px-4 flex items-center justify-between py-6">
        {/* Brand Identity */}
        <div 
          className="flex items-center gap-1.5 cursor-pointer text-[#FF5A5F] shrink-0 active:scale-95 transition-transform" 
          onClick={() => navigate('/')}
        >
          <img src="/logo.png" alt="airVnV Logo" className="h-8 w-8 object-contain" />
          <span className="font-semibold text-[22px] hidden lg:block tracking-tighter -mt-0.5">
            airVnV
          </span>
        </div>

        {/* Search Console */}
        <div className="hidden md:block flex-1 max-w-[850px] mx-auto px-6 relative z-50">
          <AnimatePresence mode="popLayout">
            {!isSearchExpanded ? (
              <motion.div 
                key="pill"
                initial={{ opacity: 0, scale: 0.9, y: -10 }}
                animate={{ opacity: 1, scale: 1, y: 0 }}
                exit={{ opacity: 0, scale: 0.95, y: -10 }}
                transition={{ duration: 0.2 }}
                onClick={() => setIsSearchExpanded(true)}
                className="flex items-center border border-slate-200 rounded-full shadow-sm hover:shadow-md transition-all duration-200 pl-6 pr-2 py-2 gap-0 cursor-pointer w-max mx-auto bg-white group"
              >
                <button className="text-[14px] font-semibold text-slate-900 px-4 border-r border-slate-200 hover:bg-slate-50 h-full transition-colors rounded-l-full">
                  Anywhere
                </button>
                <button className="text-[14px] font-semibold text-slate-900 px-4 border-r border-slate-200 hover:bg-slate-50 h-full transition-colors">
                  Any week
                </button>
                <button className="text-[14px] font-medium text-slate-500 px-4 hover:bg-slate-50 h-full transition-colors rounded-r-full flex-grow text-left">
                  Add guests
                </button>
                <div className="bg-[#FF5A5F] text-white p-2 rounded-full ml-auto group-hover:scale-105 transition-transform">
                  <Search01Icon className="h-4 w-4 stroke-[3]" />
                </div>
              </motion.div>
            ) : (
              <motion.div
                key="expanded"
                initial={{ opacity: 0, scale: 0.95, y: -20 }}
                animate={{ opacity: 1, scale: 1, y: 0 }}
                exit={{ opacity: 0, scale: 0.95, y: -10 }}
                transition={{ duration: 0.2, ease: "easeOut" }}
                className="absolute top-0 left-0 right-0"
              >
                {/* Search Overlay */}
                <div 
                  className="fixed inset-0 bg-black/20 z-[-1] top-[80px]" 
                  onClick={() => setIsSearchExpanded(false)}
                />
                
                {/* Expanded Search Bar */}
                <SearchConsole onClose={() => setIsSearchExpanded(false)} />
              </motion.div>
            )}
          </AnimatePresence>
        </div>

        {/* Control Center */}
        <div className="flex items-center gap-1 text-sm text-slate-900 font-semibold shrink-0">
          <div 
            onClick={() => navigate('/host/homes')}
            className="hover:bg-slate-50 px-4 py-3 rounded-full cursor-pointer hidden md:block transition-all"
          >
            {t('header.yourHome')}
          </div>
          
          <div className="relative">
            <div 
              onClick={() => setIsLangOpen(!isLangOpen)}
              className="p-3 hover:bg-slate-50 rounded-full cursor-pointer transition-all active:scale-95 flex items-center justify-center gap-1.5"
            >
              <Globe02Icon className="h-4.5 w-4.5 text-slate-700" />
              <span className="text-xs font-bold text-slate-600 uppercase">
                {i18n.language.startsWith('vi') ? 'VI' : 'EN'}
              </span>
            </div>

            <AnimatePresence>
              {isLangOpen && (
                <>
                  <div className="fixed inset-0 z-[-1]" onClick={() => setIsLangOpen(false)} />
                  <motion.div 
                    initial={{ opacity: 0, y: 10, scale: 0.95 }}
                    animate={{ opacity: 1, y: 0, scale: 1 }}
                    exit={{ opacity: 0, y: 10, scale: 0.95 }}
                    transition={{ duration: 0.15, ease: "easeOut" }}
                    className="absolute top-[52px] right-0 bg-white shadow-[0_0_36px_rgba(0,0,0,0.12)] rounded-xl py-2 border border-slate-100 w-44 flex flex-col z-[100] overflow-hidden"
                  >
                    <button
                      onClick={(e) => { 
                        e.stopPropagation(); 
                        i18n.changeLanguage('en'); 
                        queryClient.invalidateQueries();
                        setIsLangOpen(false); 
                      }}
                      className={`text-left px-4 py-3 hover:bg-slate-50 text-sm font-semibold transition flex items-center justify-between ${
                        i18n.language.startsWith('en') ? 'text-[#FF5A5F] bg-slate-50' : 'text-slate-800'
                      }`}
                    >
                      <span>English (EN)</span>
                      {i18n.language.startsWith('en') && (
                        <Icon icon="lucide:check" className="text-lg text-[#FF5A5F]" />
                      )}
                    </button>
                    <button
                      onClick={(e) => { 
                        e.stopPropagation(); 
                        i18n.changeLanguage('vi'); 
                        queryClient.invalidateQueries();
                        setIsLangOpen(false); 
                      }}
                      className={`text-left px-4 py-3 hover:bg-slate-50 text-sm font-semibold transition flex items-center justify-between ${
                        i18n.language.startsWith('vi') ? 'text-[#FF5A5F] bg-slate-50' : 'text-slate-800'
                      }`}
                    >
                      <span>Tiếng Việt (VI)</span>
                      {i18n.language.startsWith('vi') && (
                        <Icon icon="lucide:check" className="text-lg text-[#FF5A5F]" />
                      )}
                    </button>
                  </motion.div>
                </>
              )}
            </AnimatePresence>
          </div>

          <div className="relative">
            <div 
                onClick={() => setIsMenuOpen(!isMenuOpen)}
                className="flex items-center gap-3 border border-slate-200 rounded-full pl-3 pr-1.5 py-1.5 hover:shadow-md transition-all cursor-pointer bg-white active:scale-95"
            >
                <Menu01Icon className="h-4 w-4 text-slate-600" />
                <div className="bg-slate-500 text-white rounded-full w-8 h-8 flex items-center justify-center overflow-hidden border border-slate-200">
                   {isAuthenticated ? (
                       <Icon icon="hugeicons:user-check-01" className="text-lg" />
                   ) : (
                       <UserIcon className="h-5 w-5 fill-current" />
                   )}
                </div>
            </div>

            <AnimatePresence>
                {isMenuOpen && (
                <>
                    <div className="fixed inset-0 z-[-1]" onClick={() => setIsMenuOpen(false)} />
                    <motion.div 
                        initial={{ opacity: 0, y: 10, scale: 0.95 }}
                        animate={{ opacity: 1, y: 0, scale: 1 }}
                        exit={{ opacity: 0, y: 10, scale: 0.95 }}
                        transition={{ duration: 0.15, ease: "easeOut" }}
                        className="absolute top-[64px] right-0 bg-white shadow-[0_0_36px_rgba(0,0,0,0.12)] rounded-xl py-2 border border-slate-100 w-64 flex flex-col z-[100] overflow-hidden"
                    >
                        {isAuthenticated ? (
                        <>
                            <div className="flex flex-col border-b border-slate-100 pb-2">
                                <button
                                    onClick={(e) => { e.stopPropagation(); navigate('/messages'); setIsMenuOpen(false); }}
                                    className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-semibold text-slate-800 transition"
                                >
                                    {t('header.messages')}
                                </button>
                                <button
                                    onClick={(e) => { e.stopPropagation(); navigate('/trips'); setIsMenuOpen(false); }}
                                    className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-semibold text-slate-800 transition"
                                >
                                    {t('header.trips')}
                                </button>
                                <button
                                    onClick={(e) => { e.stopPropagation(); navigate('/profile'); setIsMenuOpen(false); }}
                                    className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-semibold text-slate-800 transition"
                                >
                                    {t('header.profile')}
                                </button>
                            </div>
                            <div className="flex flex-col py-2">
                                <button
                                    onClick={(e) => { e.stopPropagation(); navigate('/host/homes'); setIsMenuOpen(false); }}
                                    className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-normal text-slate-700 transition"
                                >
                                    {t('header.manageListings')}
                                </button>
                                <button
                                    onClick={(e) => { e.stopPropagation(); navigate('/host/reservations'); setIsMenuOpen(false); }}
                                    className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-normal text-slate-700 transition"
                                >
                                    {t('reservations.title')}
                                </button>
                                <button
                                    onClick={(e) => { e.stopPropagation(); logout(); setIsMenuOpen(false); }}
                                    className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-normal text-slate-700 transition"
                                >
                                    {t('header.logout')}
                                </button>
                            </div>
                        </>
                        ) : (
                        <>
                            <button
                                onClick={(e) => { e.stopPropagation(); navigate('/login'); setIsMenuOpen(false); }}
                                className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-semibold text-slate-800 transition"
                            >
                                {t('header.login')}
                            </button>
                            <button
                                onClick={(e) => { e.stopPropagation(); navigate('/register'); setIsMenuOpen(false); }}
                                className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-normal text-slate-700 transition"
                            >
                                {t('header.signup')}
                            </button>
                            <div className="border-t border-slate-100 mt-2 pt-2">
                                <button
                                    onClick={(e) => { e.stopPropagation(); navigate('/host/homes'); setIsMenuOpen(false); }}
                                    className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-normal text-slate-700 transition"
                                >
                                    {t('header.yourHome')}
                                </button>
                            </div>
                        </>
                        )}
                    </motion.div>
                </>
                )}
            </AnimatePresence>
          </div>
        </div>
      </div>
    </header>
  );
}
