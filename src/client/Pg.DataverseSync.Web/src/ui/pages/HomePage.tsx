import { Hero } from '../components/features/Hero';
import { Counter } from '../components/features/Counter';
import { DebugPanel } from '../components/features/DebugPanel';
import { Documentation } from '../components/features/Documentation';

/**
 * Home page component
 * Main landing page of the application
 */
export function HomePage() {
  return (
    <>
      <section id="center">
        <Hero />
        <Counter />
        <DebugPanel />
      </section>

      <div className="ticks"></div>

      <section id="next-steps">
        <Documentation />
        <div id="social">
          {/* Social links can go here */}
        </div>
      </section>

      <div className="ticks"></div>
      <section id="spacer"></section>
    </>
  );
}