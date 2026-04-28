import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { QueryClient, QueryClientProvider, QueryCache, MutationCache } from '@tanstack/react-query'
import { Toaster, toast } from 'sonner'
import './index.css'
import App from './App.tsx'

const queryClient = new QueryClient({
  queryCache: new QueryCache({
    onError: (error: any) => {
      toast.error(`Lỗi tải dữ liệu: ${error.response?.data?.message || error.message || 'Đã có lỗi xảy ra!'}`);
    },
  }),
  mutationCache: new MutationCache({
    onError: (error: any) => {
      toast.error(`Thao tác thất bại: ${error.response?.data?.message || error.message || 'Đã có lỗi xảy ra!'}`);
    },
  }),
})

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <App />
      <Toaster position="top-right" richColors closeButton />
    </QueryClientProvider>
  </StrictMode>,
)
