import type { ReactNode } from 'react';
import { Header } from '../components/common/Header';

interface MainLayoutProps {
  readonly children: ReactNode;
}

/**
 * Main layout wrapper component
 * Provides consistent layout for all pages with header
 */
export function MainLayout({ children }: MainLayoutProps) {
  return (
    <>
      <Header />
      <main className="main-content">
        {children}
      </main>
    </>
  );
}