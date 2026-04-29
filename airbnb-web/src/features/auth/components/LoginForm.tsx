import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { useLogin, useGoogleAuth } from '../hooks/useAuth'
import { GoogleLogin } from '@react-oauth/google'
import { toast } from 'sonner'

export function LoginForm() {
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const loginMutation = useLogin()
  const googleAuthMutation = useGoogleAuth()

  const handleLogin = (e: React.FormEvent) => {
    e.preventDefault()
    loginMutation.mutate({ email, password })
  }

  const errorMessage = loginMutation.error
    ? ((loginMutation.error as any).response?.status === 401 
        ? 'Email hoặc mật khẩu không đúng!' 
        : 'Đăng nhập thất bại. Vui lòng thử lại!')
    : null;

  return (
    <form onSubmit={handleLogin} className="space-y-4">
      <div className="text-center">
        <h1 className="text-2xl font-bold text-slate-900 tracking-tight">Đăng nhập</h1>
        <p className="text-slate-500 text-sm mt-1">Chào mừng bạn quay trở lại</p>
      </div>

      {errorMessage && (
        <div className="p-3 rounded-xl text-sm font-medium text-center bg-red-50 text-red-600 border border-red-100">
          {errorMessage}
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
        disabled={loginMutation.isPending}
        className="w-full h-12 bg-rausch hover:bg-rose-700 text-white text-base font-semibold rounded-xl transition-all shadow-md active:scale-[98%]"
      >
        {loginMutation.isPending ? 'Đang xác thực...' : 'Đăng nhập'}
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
            toast.error('Đăng nhập bằng Google thất bại!');
          }}
          useOneTap
        />
      </div>

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
