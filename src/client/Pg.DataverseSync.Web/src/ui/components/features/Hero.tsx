import reactLogo from '../../../assets/react.svg';
import viteLogo from '../../../assets/vite.svg';
import heroImg from '../../../assets/hero.png';

/**
 * Hero section component
 * Displays welcome banner with logos
 */
export function Hero() {
  return (
    <div className="hero">
      <img src={heroImg} className="base" width="170" height="179" alt="Hero Banner" />
      <img src={reactLogo} className="framework" alt="React logo" />
      <img src={viteLogo} className="vite" alt="Vite logo" />
    </div>
  );
}