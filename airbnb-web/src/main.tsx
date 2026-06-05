import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { QueryClient, QueryClientProvider, QueryCache, MutationCache } from '@tanstack/react-query'
import { Toaster, toast } from 'sonner'
import i18n from './lib/i18n'
import './index.css'
// @ts-expect-error - no types available for font packages
import '@fontsource-variable/outfit';
// @ts-expect-error - no types available for font packages
import '@fontsource-variable/inter';
// @ts-expect-error - no types available for font packages
import '@fontsource-variable/geist';
import App from './App.tsx'

import { GoogleOAuthProvider } from '@react-oauth/google'

const queryClient = new QueryClient({
  queryCache: new QueryCache({
    onError: (error: any) => {
      const errMsg = error.response?.data?.message || error.message || i18n.t('common.errorOccurred');
      toast.error(i18n.t('common.loadError', { message: errMsg }));
    },
  }),
  mutationCache: new MutationCache({
    onError: (error: any) => {
      const errMsg = error.response?.data?.message || error.message || i18n.t('common.errorOccurred');
      toast.error(i18n.t('common.actionFailed', { message: errMsg }));
    },
  }),
})

const GOOGLE_CLIENT_ID = import.meta.env.VITE_GOOGLE_CLIENT_ID || 'YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <GoogleOAuthProvider clientId={GOOGLE_CLIENT_ID}>
      <QueryClientProvider client={queryClient}>
        <App />
        <Toaster position="top-right" richColors closeButton />
      </QueryClientProvider>
    </GoogleOAuthProvider>
  </StrictMode>,
)
