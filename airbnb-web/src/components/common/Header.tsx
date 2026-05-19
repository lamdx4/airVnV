import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Search01Icon, Globe02Icon, Menu01Icon, UserIcon } from 'hugeicons-react';
import { useAuthStore } from '@/store/authStore';
import { Icon } from '@iconify/react';
import { motion, AnimatePresence } from 'framer-motion';

export default function Header() {
  const navigate = useNavigate();
  const location = useLocation();
  const { isAuthenticated, logout } = useAuthStore();
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isScrolled, setIsScrolled] = useState(false);

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
          <Icon icon="logos:airbnb-icon" className="text-[32px]" />
          <span className="font-semibold text-[22px] hidden lg:block tracking-tighter -mt-0.5">
            airbnb
          </span>
        </div>

        {/* Standard Search Console */}
        <div className="hidden md:flex items-center border border-slate-200 rounded-full shadow-sm hover:shadow-md transition-all duration-200 pl-6 pr-2 py-2 gap-0 cursor-pointer min-w-[320px] bg-white group">
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
        </div>

        {/* Control Center */}
        <div className="flex items-center gap-1 text-sm text-slate-900 font-semibold shrink-0">
          <div 
            onClick={() => navigate('/host/homes')}
            className="hover:bg-slate-50 px-4 py-3 rounded-full cursor-pointer hidden md:block transition-all"
          >
            Airbnb your home
          </div>
          
          <div className="p-3 hover:bg-slate-50 rounded-full cursor-pointer transition-all">
            <Globe02Icon className="h-4.5 w-4.5 text-slate-700" />
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
                                    Messages
                                </button>
                                <button
                                    onClick={(e) => { e.stopPropagation(); navigate('/trips'); setIsMenuOpen(false); }}
                                    className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-semibold text-slate-800 transition"
                                >
                                    Trips
                                </button>
                                <button
                                    onClick={(e) => { e.stopPropagation(); navigate('/profile'); setIsMenuOpen(false); }}
                                    className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-semibold text-slate-800 transition"
                                >
                                    Profile
                                </button>
                            </div>
                            <div className="flex flex-col py-2">
                                <button
                                    onClick={(e) => { e.stopPropagation(); navigate('/host/homes'); setIsMenuOpen(false); }}
                                    className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-normal text-slate-700 transition"
                                >
                                    Manage listings
                                </button>
                                <button
                                    onClick={(e) => { e.stopPropagation(); logout(); setIsMenuOpen(false); }}
                                    className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-normal text-slate-700 transition"
                                >
                                    Log out
                                </button>
                            </div>
                        </>
                        ) : (
                        <>
                            <button
                                onClick={(e) => { e.stopPropagation(); navigate('/login'); setIsMenuOpen(false); }}
                                className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-semibold text-slate-800 transition"
                            >
                                Log in
                            </button>
                            <button
                                onClick={(e) => { e.stopPropagation(); navigate('/register'); setIsMenuOpen(false); }}
                                className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-normal text-slate-700 transition"
                            >
                                Sign up
                            </button>
                            <div className="border-t border-slate-100 mt-2 pt-2">
                                <button
                                    onClick={(e) => { e.stopPropagation(); navigate('/host/homes'); setIsMenuOpen(false); }}
                                    className="text-left px-4 py-3 hover:bg-slate-50 text-sm font-normal text-slate-700 transition"
                                >
                                    Airbnb your home
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
