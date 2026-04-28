import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'

export function RegisterForm() {
  const navigate = useNavigate()
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  const handleRegister = (e: React.FormEvent) => {
    e.preventDefault()
    setIsLoading(true)
    setError(null)
    setSuccess(null)

    if (password !== confirmPassword) {
      setIsLoading(false)
      setError('Mật khẩu xác nhận không khớp!')
      return
    }

    // Mock API Registration
    setTimeout(() => {
      setIsLoading(false)
      setSuccess('Đăng ký thành công! Đang chuyển hướng sang đăng nhập...')
      setTimeout(() => {
        navigate('/login')
      }, 1500)
    }, 1200)
  }

  return (
    <form onSubmit={handleRegister} className="space-y-4">
      <div className="text-center">
        <h1 className="text-2xl font-bold text-slate-900 tracking-tight">Đăng ký tài khoản</h1>
        <p className="text-slate-500 text-sm mt-1">Gia nhập cộng đồng Airbnb</p>
      </div>

      {error && (
        <div className="p-3 rounded-xl text-sm font-medium text-center bg-red-50 text-red-600 border border-red-100">
          {error}
        </div>
      )}

      {success && (
        <div className="p-3 rounded-xl text-sm font-medium text-center bg-green-50 text-green-700 border border-green-100">
          {success}
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
        disabled={isLoading}
        className="w-full h-12 bg-rausch hover:bg-rose-700 text-white text-base font-semibold rounded-xl transition-all shadow-md active:scale-[98%]"
      >
        {isLoading ? 'Đang tạo tài khoản...' : 'Đăng ký'}
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
