import { DataverseLogo } from './DataverseLogo';
import { AuthButtons } from './AuthButtons';
import { config } from '../../../shared/config';

/**
 * Application header component
 * Contains logo, title, and authentication buttons
 */
export function Header() {
  return (
    <header className="app-header">
      <div className="header-content">
        <div className="logo-title-container">
          <DataverseLogo />
          <h1 className="app-title">{config.app.title.toUpperCase()}</h1>
        </div>
        <AuthButtons />
      </div>
    </header>
  );
}