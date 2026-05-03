import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { useRegister, useVerifyEmail, useGoogleAuth } from '../hooks/useAuth'
import { GoogleLogin } from '@react-oauth/google'
import { toast } from 'sonner'

export function RegisterForm() {
  const navigate = useNavigate()
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [role] = useState<import('../types').UserRole>('User')
  const [localError, setLocalError] = useState<string | null>(null)
  const [otpCode, setOtpCode] = useState('')
  const [isOtpSent, setIsOtpSent] = useState(false)
  const [devOtp, setDevOtp] = useState<string | null>(null)

  const registerMutation = useRegister()
  const verifyMutation = useVerifyEmail()
  const googleAuthMutation = useGoogleAuth()

  const handleRegister = (e: React.FormEvent) => {
    e.preventDefault()
    setLocalError(null)

    if (password !== confirmPassword) {
      setLocalError('Mật khẩu xác nhận không khớp!')
      return
    }

    registerMutation.mutate({ fullName: name, email, password, role }, {
      onSuccess: (data) => {
        setIsOtpSent(true);
        if (data.otpCode) {
          setDevOtp(data.otpCode); // Lưu tạm mã OTP dev
        }
      }
    })
  }

  const handleVerifyOtp = (e: React.FormEvent) => {
    e.preventDefault()
    setLocalError(null)
    
    if (otpCode.length !== 6) {
      setLocalError('Mã OTP phải bao gồm 6 số!')
      return
    }

    verifyMutation.mutate({ email, otpCode })
  }

  const regApiError = registerMutation.error as any;
  const verifyApiError = verifyMutation.error as any;

  const errorMessage = localError || 
    (regApiError 
      ? (regApiError.errorCode === 'USER_ALREADY_EXISTS' ? 'Email này đã tồn tại trong hệ thống!' : regApiError.message || 'Đăng ký thất bại!') 
      : null) ||
    (verifyApiError 
      ? (verifyApiError.errorCode === 'VERIFY_FAILED' ? 'Mã OTP không đúng hoặc đã hết hạn!' : verifyApiError.message || 'Xác thực thất bại!') 
      : null);

  // 1. Giao diện Nhập mã OTP
  if (isOtpSent) {
    return (
      <form onSubmit={handleVerifyOtp} className="space-y-4">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-slate-900 tracking-tight">Xác minh Email</h1>
          <p className="text-slate-500 text-sm mt-1">Mã xác thực OTP 6 số đã được gửi tới <b>{email}</b></p>
          
          {devOtp && (
            <div className="mt-2 p-2 bg-amber-50 border border-amber-200 text-amber-700 rounded-xl text-sm font-medium">
              🔒 Chế độ Dev: Mã OTP của bạn là: <span className="font-bold text-base tracking-widest">{devOtp}</span>
            </div>
          )}
        </div>

        {errorMessage && (
          <div className="p-3 rounded-xl text-sm font-medium text-center bg-red-50 text-red-600 border border-red-100">
            {errorMessage}
          </div>
        )}

        <div className="space-y-3">
          <Input
            type="text"
            maxLength={6}
            placeholder="Nhập 6 chữ số OTP"
            value={otpCode}
            onChange={(e) => setOtpCode(e.target.value.replace(/\D/g, ''))}
            required
            className="h-12 px-4 text-center text-lg font-bold tracking-widest border-slate-200 focus:border-rausch focus:ring-rausch rounded-xl"
          />
        </div>

        <Button
          type="submit"
          disabled={verifyMutation.isPending}
          className="w-full h-12 bg-rausch hover:bg-rose-700 text-white text-base font-semibold rounded-xl transition-all shadow-md active:scale-[98%]"
        >
          {verifyMutation.isPending ? 'Đang xác thực...' : 'Xác nhận'}
        </Button>

        <p className="text-center text-sm text-slate-600 pt-2">
          Không nhận được mã?{' '}
          <button
            type="button"
            onClick={handleRegister}
            className="text-rausch font-semibold hover:underline"
          >
            Gửi lại mã
          </button>
        </p>
      </form>
    )
  }

  // 2. Giao diện Đăng ký thông thường
  return (
    <form onSubmit={handleRegister} className="space-y-4">
      <div className="text-center">
        <h1 className="text-2xl font-bold text-slate-900 tracking-tight">Đăng ký tài khoản</h1>
        <p className="text-slate-500 text-sm mt-1">Gia nhập cộng đồng Airbnb</p>
      </div>

      {errorMessage && (
        <div className="p-3 rounded-xl text-sm font-medium text-center bg-red-50 text-red-600 border border-red-100">
          {errorMessage}
        </div>
      )}

      <div className="space-y-3">
        <Input
          type="text"
          placeholder="Họ và tên"
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
          className="h-12 px-4 border-slate-200 focus:border-rausch focus:ring-rausch rounded-xl"
        />
        <Input
          type="email"
          placeholder="Địa chỉ Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          className="h-12 px-4 border-slate-200 focus:border-rausch focus:ring-rausch rounded-xl"
        />
        <Input
          type="password"
          placeholder="Mật khẩu"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
          className="h-12 px-4 border-slate-200 focus:border-rausch focus:ring-rausch rounded-xl"
        />
        <Input
          type="password"
          placeholder="Xác nhận mật khẩu"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
          required
          className="h-12 px-4 border-slate-200 focus:border-rausch focus:ring-rausch rounded-xl"
        />
      </div>

      <Button
        type="submit"
        disabled={registerMutation.isPending}
        className="w-full h-12 bg-rausch hover:bg-rose-700 text-white text-base font-semibold rounded-xl transition-all shadow-md active:scale-[98%]"
      >
        {registerMutation.isPending ? 'Đang gửi yêu cầu...' : 'Đăng ký'}
      </Button>

      <div className="flex items-center gap-4 my-2">
        <div className="flex-1 h-px bg-slate-200"></div>
        <span className="text-slate-400 text-xs uppercase">Hoặc</span>
        <div className="flex-1 h-px bg-slate-200"></div>
      </div>

      <div className="flex justify-center w-full py-1">
        <GoogleLogin
          onSuccess={(credentialResponse) => {
            if (credentialResponse.credential) {
              googleAuthMutation.mutate({ idToken: credentialResponse.credential, role: 'User' });
            }
          }}
          onError={() => {
            toast.error('Đăng ký bằng Google thất bại!');
          }}
          useOneTap
        />
      </div>

      <p className="text-center text-sm text-slate-600 pt-2">
        Đã có tài khoản?{' '}
        <button
          type="button"
          onClick={() => navigate('/login')}
          className="text-rausch font-semibold hover:underline"
        >
          Đăng nhập
        </button>
      </p>
    </form>
  )
}
