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
            <img className="logo" src={viteLogo} alt=""/>{' '}
            Explore Vite
          </a>
        </li>
        <li>
          <a href="https://react.dev/" target="_blank" rel="noreferrer">
            <img className="button-icon" src={reactLogo} alt=""/>{' '}
            Learn more
          </a>
        </li>
      </ul>
    </div>
  );
}