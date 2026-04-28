import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Layout from './components/common/Layout';
import Home from './pages/Home';
import { AirbnbLogin } from './features/auth/components/AirbnbLogin';

function App() {
  return (
    <BrowserRouter>
      <Layout>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<AirbnbLogin view="login" />} />
          <Route path="/register" element={<AirbnbLogin view="register" />} />
          <Route path="/forgot-password" element={<AirbnbLogin view="forgot-password" />} />
        </Routes>
      </Layout>
    </BrowserRouter>
  );
}

export default App;
