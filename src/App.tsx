import { BrowserRouter, Routes, Route } from 'react-router-dom';
import GuardianPage from './pages/GuardianPage';
import AdminPage from './pages/AdminPage';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<GuardianPage />} />
        <Route path="/admin" element={<AdminPage />} />
      </Routes>
    </BrowserRouter>
  );
}