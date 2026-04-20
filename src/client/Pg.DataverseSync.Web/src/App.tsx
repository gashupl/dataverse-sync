import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './application';
import { MainLayout } from './ui/layouts/MainLayout';
import { HomePage } from './ui/pages/HomePage';
import { ProfilePage } from './ui/pages/ProfilePage';
import './styles/app.css';

/**
 * Root application component
 * Minimal entry point using layout and pages
 */
function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <MainLayout>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/profile" element={<ProfilePage />} />
          </Routes>
        </MainLayout>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
