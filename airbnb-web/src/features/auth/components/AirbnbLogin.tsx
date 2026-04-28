import { useState, useEffect } from 'react'
import BgLogin from '@/assets/bg-login.png'

// Tự động lấy toàn bộ ảnh trong thư mục locations
const locationImages = Object.values(
  import.meta.glob<{ default: string }>('../../../assets/locations/*.{png,jpg,jpeg,webp}', { eager: true })
).map(mod => mod.default);

import { LoginForm } from './LoginForm'
import { RegisterForm } from './RegisterForm'
import { ForgotPasswordForm } from './ForgotPasswordForm'

interface AirbnbLoginProps {
  view: 'login' | 'register' | 'forgot-password'
}

export function AirbnbLogin({ view }: AirbnbLoginProps) {
  const [currentImageIndex, setCurrentImageIndex] = useState(0)

  useEffect(() => {
    if (locationImages.length <= 1) return;
    const interval = setInterval(() => {
      setCurrentImageIndex((prev) => (prev + 1) % locationImages.length)
    }, 4000) // Chuyển ảnh sau mỗi 4 giây
    return () => clearInterval(interval)
  }, [])

  return (
    <div className="flex flex-col md:flex-row w-full min-h-[600px] rounded-3xl shadow-2xl overflow-hidden bg-white border border-slate-100 mt-4 font-sans">
      {/* Left Section - Slideshow for MD+ screens */}
      <div className="hidden md:block md:w-1/2 relative overflow-hidden bg-slate-900">
        {locationImages.map((src, index) => (
          <div
            key={src}
            className={`absolute inset-0 bg-cover bg-center transition-opacity duration-1000 ease-in-out ${
              index === currentImageIndex ? 'opacity-100' : 'opacity-0'
            }`}
            style={{ backgroundImage: `url(${src})` }}
          />
        ))}
        
        {/* Fallback if empty */}
        {locationImages.length === 0 && (
          <div
            className="absolute inset-0 bg-cover bg-center"
            style={{ backgroundImage: `url(${BgLogin})` }}
          />
        )}

        <div className="absolute inset-0 bg-gradient-to-r from-black/60 to-black/30 backdrop-blur-[1px] flex flex-col justify-end p-12 text-white z-10">
          <h2 className="text-4xl font-bold tracking-tight leading-tight mb-4 animate-fade-in">
            Tìm kiếm chỗ ở mơ ước cho chuyến đi tiếp theo.
          </h2>
          <p className="text-lg text-slate-200/90 font-light">
            Trải nghiệm kỳ nghỉ hoàn hảo với hàng triệu lựa chọn trên toàn Việt Nam.
          </p>
        </div>
      </div>

      {/* Right Section - Auth Form */}
      <div className="w-full md:w-1/2 flex items-center justify-center p-8 md:p-12 bg-slate-50/50">
        <div className="w-full max-w-md space-y-6 bg-white p-8 rounded-2xl shadow-xl border border-slate-100/80">
          
          {/* Airbnb Logo */}
          <div className="flex justify-center mb-2">
            <svg
              viewBox="0 0 32 32"
              className="w-10 h-10 text-rausch animate-pulse"
              fill="currentColor"
            >
              <path d="M16 1.8C10.2 1.8 5.5 6.5 5.5 12.3c0 7.5 7.8 15.5 10.5 17.9.2.2.5.2.7 0 2.7-2.4 10.5-10.4 10.5-17.9 0-5.8-4.7-10.5-10.5-10.5zm0 26c-2.4-2.2-9.1-9.2-9.1-13.7 0-4.9 4.1-9 9.1-9s9.1 4.1 9.1 9c0 4.5-6.7 11.5-9.1 13.7zm0-18.7c-2.6 0-4.7 2.1-4.7 4.7S13.4 18.5 16 18.5s4.7-2.1 4.7-4.7S18.6 9.1 16 9.1z" />
            </svg>
          </div>

          {/* Dynamic Sub-components */}
          {view === 'login' && <LoginForm />}
          {view === 'register' && <RegisterForm />}
          {view === 'forgot-password' && <ForgotPasswordForm />}
          
        </div>
      </div>
    </div>
  )
}
