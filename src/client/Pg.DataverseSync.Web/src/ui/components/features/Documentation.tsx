import viteLogo from '../../../assets/vite.svg';
import reactLogo from '../../../assets/react.svg';

/**
 * Documentation section component
 * Links to external documentation and resources
 */
export function Documentation() {
  return (
    <div id="docs">
      <svg className="icon" role="presentation" aria-hidden="true">
        <use href="/icons.svg#documentation-icon"></use>
      </svg>
      <h2>Documentation</h2>
      <p>Your questions, answered</p>
      <ul>
        <li>
          <a href="https://vite.dev/" target="_blank" rel="noreferrer">
            <span className="logo-container">
              <img className="logo" src={viteLogo} alt="Vite logo" />
              <span className="logo-text">Explore Vite</span>
            </span>
          </a>
        </li>
        <li>
          <a href="https://react.dev/" target="_blank" rel="noreferrer">
            <span className="logo-container">
              <img className="button-icon" src={reactLogo} alt="React logo" />
              <span className="logo-text">Learn more</span>
            </span>
          </a>
        </li>
      </ul>
    </div>
  );
}