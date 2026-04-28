import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { useRegister } from '../hooks/useAuth'

export function RegisterForm() {
  const navigate = useNavigate()
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [role, setRole] = useState<0 | 1>(0) // 0: Guest, 1: Host
  const [localError, setLocalError] = useState<string | null>(null)
  const registerMutation = useRegister()

  const handleRegister = (e: React.FormEvent) => {
    e.preventDefault()
    setLocalError(null)

    if (password !== confirmPassword) {
      setLocalError('Mật khẩu xác nhận không khớp!')
      return
    }

    registerMutation.mutate({ fullName: name, email, password, role })
  }

  const errorMessage = localError || (registerMutation.error ? 'Đăng ký thất bại. Vui lòng kiểm tra lại!' : null);

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

      {registerMutation.isSuccess && (
        <div className="p-3 rounded-xl text-sm font-medium text-center bg-green-50 text-green-700 border border-green-100">
          Đăng ký thành công! Đang chuyển hướng sang đăng nhập...
        </div>
      )}

      <div className="space-y-3">
        <div className="grid grid-cols-2 gap-3 mb-2">
          <button
            type="button"
            onClick={() => setRole(0)}
            className={`py-2.5 text-sm font-semibold rounded-xl transition-all ${
              role === 0 
                ? 'bg-slate-900 text-white shadow-sm' 
                : 'bg-slate-50 text-slate-600 hover:bg-slate-100 border border-slate-200'
            }`}
          >
            Khách thuê
          </button>
          <button
            type="button"
            onClick={() => setRole(1)}
            className={`py-2.5 text-sm font-semibold rounded-xl transition-all ${
              role === 1 
                ? 'bg-slate-900 text-white shadow-sm' 
                : 'bg-slate-50 text-slate-600 hover:bg-slate-100 border border-slate-200'
            }`}
          >
            Chủ nhà (Host)
          </button>
        </div>

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
        {registerMutation.isPending ? 'Đang tạo tài khoản...' : 'Đăng ký'}
      </Button>

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
