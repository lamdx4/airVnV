import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Search, Globe, Menu, User } from 'lucide-react';
import { useAuthStore } from '@/store/authStore';

export default function Header() {
  const navigate = useNavigate();
  const { isAuthenticated, logout } = useAuthStore();
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  return (
    <header className="sticky top-0 z-50 w-full border-b border-gray-200 bg-white px-6 md:px-12 py-4 flex items-center justify-between">
      {/* Logo */}
      <div className="flex items-center gap-2 cursor-pointer" onClick={() => navigate('/')}>
        <svg
          className="h-8 w-8 text-rausch"
          viewBox="0 0 32 32"
          fill="currentColor"
        >
          <path d="M16 1.8C10.2 1.8 5.5 6.5 5.5 12.3c0 7.5 7.8 15.5 10.5 17.9.2.2.5.2.7 0 2.7-2.4 10.5-10.4 10.5-17.9 0-5.8-4.7-10.5-10.5-10.5zm0 26c-2.4-2.2-9.1-9.2-9.1-13.7 0-4.9 4.1-9 9.1-9s9.1 4.1 9.1 9c0 4.5-6.7 11.5-9.1 13.7zm0-18.7c-2.6 0-4.7 2.1-4.7 4.7S13.4 18.5 16 18.5s4.7-2.1 4.7-4.7S18.6 9.1 16 9.1z" />
        </svg>
        <span className="text-rausch font-bold text-xl hidden md:block tracking-tight">
          airbnb
        </span>
      </div>

      {/* Search Bar */}
      <div className="flex items-center border border-gray-200 rounded-full shadow-sm hover:shadow-md transition duration-200 px-4 py-2 gap-4 cursor-pointer text-sm">
        <span className="font-medium text-hof px-2 border-r border-gray-200">Địa điểm bất kỳ</span>
        <span className="font-medium text-hof px-2 border-r border-gray-200">Tuần bất kỳ</span>
        <span className="text-foggy px-2">Thêm khách</span>
        <div className="bg-rausch text-white p-2 rounded-full">
          <Search className="h-4 w-4" />
        </div>
      </div>

      {/* User Menu */}
      <div className="flex items-center gap-4 text-sm text-hof font-medium">
        <span className="hover:bg-gray-100 px-4 py-2 rounded-full cursor-pointer hidden md:block">
          Cho thuê chỗ ở qua Airbnb
        </span>
        <Globe className="h-5 w-5 text-foggy hover:bg-gray-100 rounded-full p-1 box-content cursor-pointer" />

        <div 
          onClick={() => setIsMenuOpen(!isMenuOpen)}
          className="flex items-center gap-3 border border-gray-200 rounded-full px-3 py-2 hover:shadow-md transition cursor-pointer relative"
        >
          <Menu className="h-4 w-4 text-foggy" />
          <div className="bg-gray-500 text-white rounded-full p-1">
            <User className="h-4 w-4" />
          </div>

          {isMenuOpen && (
            <div className="absolute top-12 right-0 bg-white shadow-lg rounded-xl py-2 border border-gray-200/80 w-52 flex flex-col z-50">
              {isAuthenticated ? (
                <>
                  <button
                    onClick={(e) => { e.stopPropagation(); navigate('/profile'); setIsMenuOpen(false); }}
                    className="text-left px-4 py-3 hover:bg-gray-50 text-sm font-medium text-slate-800 transition border-b border-gray-100"
                  >
                    Hồ sơ cá nhân
                  </button>
                  <button
                    onClick={(e) => { e.stopPropagation(); logout(); setIsMenuOpen(false); }}
                    className="text-left px-4 py-3 hover:bg-gray-50 text-sm font-normal text-red-500 transition"
                  >
                    Đăng xuất
                  </button>
                </>
              ) : (
                <>
                  <button
                    onClick={(e) => { e.stopPropagation(); navigate('/login'); setIsMenuOpen(false); }}
                    className="text-left px-4 py-3 hover:bg-gray-50 text-sm font-medium text-slate-800 transition border-b border-gray-100"
                  >
                    Đăng nhập
                  </button>
                  <button
                    onClick={(e) => { e.stopPropagation(); navigate('/register'); setIsMenuOpen(false); }}
                    className="text-left px-4 py-3 hover:bg-gray-50 text-sm font-normal text-slate-600 transition"
                  >
                    Đăng ký
                  </button>
                </>
              )}
            </div>
          )}
        </div>
      </div>
    </header>
  );
}
