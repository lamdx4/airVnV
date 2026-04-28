import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'

export function LoginForm() {
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const handleLogin = (e: React.FormEvent) => {
    e.preventDefault()
    setIsLoading(true)
    setError(null)
    // Mock API Authentication
    setTimeout(() => {
      setIsLoading(false)
      setError('Email hoặc mật khẩu không đúng!')
    }, 1000)
  }

  return (
    <form onSubmit={handleLogin} className="space-y-4">
      <div className="text-center">
        <h1 className="text-2xl font-bold text-slate-900 tracking-tight">Đăng nhập</h1>
        <p className="text-slate-500 text-sm mt-1">Chào mừng bạn quay trở lại</p>
      </div>

      {error && (
        <div className="p-3 rounded-xl text-sm font-medium text-center bg-red-50 text-red-600 border border-red-100">
          {error}
        </div>
      )}

      <div className="space-y-3">
        <Input
          type="email"
          placeholder="Email của bạn"
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
      </div>

      <div className="flex justify-end">
        <button 
          type="button"
          onClick={() => navigate('/forgot-password')}
          className="text-sm text-slate-600 hover:text-rausch font-medium transition-colors"
        >
          Quên mật khẩu?
        </button>
      </div>

      <Button
        type="submit"
        disabled={isLoading}
        className="w-full h-12 bg-rausch hover:bg-rose-700 text-white text-base font-semibold rounded-xl transition-all shadow-md active:scale-[98%]"
      >
        {isLoading ? 'Đang xác thực...' : 'Đăng nhập'}
      </Button>

      <div className="flex items-center gap-4 my-2">
        <div className="flex-1 h-px bg-slate-200"></div>
        <span className="text-slate-400 text-xs uppercase">Hoặc</span>
        <div className="flex-1 h-px bg-slate-200"></div>
      </div>

      <Button
        type="button"
        variant="outline"
        className="w-full h-12 border-slate-200 focus:border-rausch focus:ring-rausch rounded-xl font-medium text-slate-800 hover:bg-slate-50 flex items-center justify-center gap-2"
      >
        Tiếp tục với Google
      </Button>

      <p className="text-center text-sm text-slate-600 pt-2">
        Chưa có tài khoản?{' '}
        <button
          type="button"
          onClick={() => navigate('/register')}
          className="text-rausch font-semibold hover:underline"
        >
          Đăng ký ngay
        </button>
      </p>
    </form>
  )
}
