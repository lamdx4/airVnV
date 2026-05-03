import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Layout from './components/common/Layout';
import { ErrorBoundary } from './components/common/ErrorBoundary';
import Home from './pages/Home';
import Profile from './pages/Profile';
import { AirbnbLogin } from './features/auth/components/AirbnbLogin';

export default function App() {
  return (
    <ErrorBoundary>
      <BrowserRouter>
        <Layout>
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/profile" element={<Profile />} />
            <Route path="/login" element={<AirbnbLogin view="login" />} />
            <Route path="/register" element={<AirbnbLogin view="register" />} />
            <Route path="/forgot-password" element={<AirbnbLogin view="forgot-password" />} />
          </Routes>
        </Layout>
      </BrowserRouter>
    </ErrorBoundary>
  );
}
