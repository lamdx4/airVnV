import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Toaster } from 'sonner';
import Layout from './components/common/Layout';
import { ErrorBoundary } from './components/common/ErrorBoundary';
import Home from './pages/Home';
import Profile from './pages/Profile';
import HostDashboard from './pages/HostDashboard';
import EditProperty from './pages/EditProperty';
import { AirbnbLogin } from './features/auth/components/AirbnbLogin';
import Messages from './pages/Messages';
import Trips from './pages/Trips';
import Reservations from './pages/Reservations';
import PropertyDetail from './pages/PropertyDetail';
import NewProperty from './pages/NewProperty';
import ApprovalQueuePage from './pages/admin/properties/approval-queue/page';

export default function App() {
  return (
    <ErrorBoundary>
      <Toaster position="top-right" richColors />
      <BrowserRouter>
        <Routes>
          <Route path="/host/homes/new" element={<NewProperty />} />
          <Route path="/messages" element={<Messages />} />
          <Route path="/messages/:conversationId" element={<Messages />} />
          <Route element={<Layout />}>
            <Route path="/" element={<Home />} />
            <Route path="/profile" element={<Profile />} />
            <Route path="/host/homes" element={<HostDashboard />} />
            <Route path="/host/homes/:id/edit" element={<EditProperty />} />
            <Route path="/properties/:id" element={<PropertyDetail />} />
            <Route path="/login" element={<AirbnbLogin view="login" />} />
            <Route path="/register" element={<AirbnbLogin view="register" />} />
            <Route path="/forgot-password" element={<AirbnbLogin view="forgot-password" />} />
            <Route path="/trips" element={<Trips />} />
            <Route path="/host/reservations" element={<Reservations />} />
            <Route path="/admin/properties/approval-queue" element={<ApprovalQueuePage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </ErrorBoundary>
  );
}
