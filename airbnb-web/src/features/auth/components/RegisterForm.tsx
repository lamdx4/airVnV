import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { useRegister, useVerifyEmail, useGoogleAuth } from '../hooks/useAuth'
import { GoogleLogin } from '@react-oauth/google'
import { toast } from 'sonner'
import { useTranslation } from 'react-i18next'

export function RegisterForm() {
  const { t } = useTranslation()
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
      setLocalError(t('auth.confirmPasswordMismatch'))
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
      setLocalError(t('auth.otpLengthError'))
      return
    }

    verifyMutation.mutate({ email, otpCode })
  }

  const regApiError = registerMutation.error as any;
  const verifyApiError = verifyMutation.error as any;

  const errorMessage = localError || 
    (regApiError 
      ? (regApiError.errorCode === 'USER_ALREADY_EXISTS' ? t('auth.userExists') : regApiError.message || t('auth.registerFailed')) 
      : null) ||
    (verifyApiError 
      ? (verifyApiError.errorCode === 'VERIFY_FAILED' ? t('auth.otpIncorrect') : verifyApiError.message || t('auth.verificationFailed')) 
      : null);

  // 1. Giao diện Nhập mã OTP
  if (isOtpSent) {
    return (
      <form onSubmit={handleVerifyOtp} className="space-y-4">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-slate-900 tracking-tight">{t('auth.verifyEmail')}</h1>
          <p className="text-slate-500 text-sm mt-1">{t('auth.otpSentMessage', { email })}</p>
          
          {devOtp && (
            <div className="mt-2 p-2 bg-amber-50 border border-amber-200 text-amber-700 rounded-xl text-sm font-medium">
              🔒 {t('auth.devOtpMode', { otp: devOtp })}
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
            placeholder={t('auth.otpPlaceholder')}
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
          {verifyMutation.isPending ? t('auth.authenticating') : t('auth.confirm')}
        </Button>

        <p className="text-center text-sm text-slate-600 pt-2">
          {t('auth.didNotReceive')}{' '}
          <Button
            type="button"
            variant="link"
            onClick={handleRegister}
            className="text-rausch font-semibold hover:underline h-auto p-0"
          >
            {t('auth.resendCode')}
          </Button>
        </p>
      </form>
    )
  }

  // 2. Giao diện Đăng ký thông thường
  return (
    <form onSubmit={handleRegister} className="space-y-4">
      <div className="text-center">
        <h1 className="text-2xl font-bold text-slate-900 tracking-tight">{t('auth.register')}</h1>
        <p className="text-slate-500 text-sm mt-1">{t('auth.joinCommunity')}</p>
      </div>

      {errorMessage && (
        <div className="p-3 rounded-xl text-sm font-medium text-center bg-red-50 text-red-600 border border-red-100">
          {errorMessage}
        </div>
      )}

      <div className="space-y-3">
        <Input
          type="text"
          placeholder={t('auth.fullName')}
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
          className="h-12 px-4 border-slate-200 focus:border-rausch focus:ring-rausch rounded-xl"
        />
        <Input
          type="email"
          placeholder={t('auth.email')}
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          className="h-12 px-4 border-slate-200 focus:border-rausch focus:ring-rausch rounded-xl"
        />
        <Input
          type="password"
          placeholder={t('auth.password')}
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
          className="h-12 px-4 border-slate-200 focus:border-rausch focus:ring-rausch rounded-xl"
        />
        <Input
          type="password"
          placeholder={t('auth.confirmPassword')}
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
        {registerMutation.isPending ? t('auth.submittingRequest') : t('auth.signUp')}
      </Button>

      <div className="flex items-center gap-4 my-2">
        <div className="flex-1 h-px bg-slate-200"></div>
        <span className="text-slate-400 text-xs uppercase">{t('auth.or')}</span>
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
            toast.error(t('auth.googleRegisterFailed'));
          }}
          useOneTap
        />
      </div>

      <p className="text-center text-sm text-slate-600 pt-2">
        {t('auth.alreadyHaveAccount')}{' '}
        <Button
          type="button"
          variant="link"
          onClick={() => navigate('/login')}
          className="text-rausch font-semibold hover:underline h-auto p-0"
        >
          {t('auth.login')}
        </Button>
      </p>
    </form>
  )
}
