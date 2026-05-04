import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Toaster } from 'sonner';
import Layout from './components/common/Layout';
import { ErrorBoundary } from './components/common/ErrorBoundary';
import Home from './pages/Home';
import Profile from './pages/Profile';
import HostDashboard from './pages/HostDashboard';
import EditProperty from './pages/EditProperty';
import { AirbnbLogin } from './features/auth/components/AirbnbLogin';

export default function App() {
  return (
    <ErrorBoundary>
      <Toaster position="top-right" richColors />
      <BrowserRouter>
        <Layout>
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/profile" element={<Profile />} />
            <Route path="/host/homes" element={<HostDashboard />} />
            <Route path="/host/homes/:id/edit" element={<EditProperty />} />
            <Route path="/login" element={<AirbnbLogin view="login" />} />
            <Route path="/register" element={<AirbnbLogin view="register" />} />
            <Route path="/forgot-password" element={<AirbnbLogin view="forgot-password" />} />
          </Routes>
        </Layout>
      </BrowserRouter>
    </ErrorBoundary>
  );
}
