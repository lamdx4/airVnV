import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { useTranslation } from 'react-i18next'

export function ForgotPasswordForm() {
  const navigate = useNavigate()
  const { t } = useTranslation()
  const [email, setEmail] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [success, setSuccess] = useState<string | null>(null)

  const handleForgotPassword = (e: React.FormEvent) => {
    e.preventDefault()
    setIsLoading(true)
    setSuccess(null)

    // Mock API Forgot Password
    setTimeout(() => {
      setIsLoading(false)
      setSuccess(t('auth.resetLinkSent'))
    }, 1000)
  }

  return (
    <form onSubmit={handleForgotPassword} className="space-y-4">
      <div className="text-center">
        <h1 className="text-2xl font-bold text-slate-900 tracking-tight">{t('auth.forgotPasswordTitle')}</h1>
        <p className="text-slate-500 text-sm mt-1">{t('auth.enterEmailInstructions')}</p>
      </div>

      {success && (
        <div className="p-3 rounded-xl text-sm font-medium text-center bg-green-50 text-green-700 border border-green-100">
          {success}
        </div>
      )}

      <div className="space-y-3">
        <Input
          type="email"
          placeholder={t('auth.emailPlaceholder')}
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          className="h-12 px-4 border-slate-200 focus:border-rausch focus:ring-rausch rounded-xl"
        />
      </div>

      <Button
        type="submit"
        disabled={isLoading}
        className="w-full h-12 bg-rausch hover:bg-rose-700 text-white text-base font-semibold rounded-xl transition-all shadow-md active:scale-[98%]"
      >
        {isLoading ? t('auth.sending') : t('auth.sendResetLink')}
      </Button>

      <div className="text-center pt-2">
        <Button
          type="button"
          variant="link"
          onClick={() => navigate('/login')}
          className="text-sm text-rausch font-semibold hover:underline flex items-center justify-center gap-1 mx-auto h-auto p-0"
        >
          {t('auth.backToLogin')}
        </Button>
      </div>
    </form>
  )
}
