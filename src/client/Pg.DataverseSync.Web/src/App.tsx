import { MainLayout } from './ui/layouts/MainLayout';
import { HomePage } from './ui/pages/HomePage';
import './styles/app.css';

/**
 * Root application component
 * Minimal entry point using layout and pages
 */
function App() {
  return (
    <MainLayout>
      <HomePage />
    </MainLayout>
  );
}

export default App;
