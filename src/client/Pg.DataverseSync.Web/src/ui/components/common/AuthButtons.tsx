import { config } from '../../../shared/config';

/**
 * Authentication buttons component
 * Displays login/register buttons if authentication is enabled
 */
export function AuthButtons() {
  if (!config.auth.enabled) {
    return null;
  }

  return (
    <div className="auth-buttons">
      <button className="auth-btn login-btn">Login</button>
      <button className="auth-btn register-btn">Register</button>
    </div>
  );
}