import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { config } from '../../../shared/config';
import { useAuth } from '../../../application';
import { RegisterForm } from '../features/RegisterForm';
import { LoginForm } from '../features/LoginForm';

/**
 * Authentication buttons component
 * Displays login/register buttons if not authenticated, user info and logout if authenticated
 */
export function AuthButtons() {
  const { authState, logout } = useAuth();
  const navigate = useNavigate();
  const [showRegisterForm, setShowRegisterForm] = useState(false);
  const [showLoginForm, setShowLoginForm] = useState(false);

  if (!config.auth.enabled) {
    return null;
  }

  const handleRegisterClick = () => {
    setShowRegisterForm(true);
  };

  const handleLoginClick = () => {
    setShowLoginForm(true);
  };

  const handleLogoutClick = async () => {
    try {
      await logout();
      navigate('/');
    } catch (error) {
      console.error('Logout failed:', error);
      // Logout should clear local state even if server call fails
      navigate('/');
    }
  };

  const handleCloseRegisterForm = () => {
    setShowRegisterForm(false);
  };

  const handleCloseLoginForm = () => {
    setShowLoginForm(false);
  };

  const handleRegisterSuccess = () => {
    // Don't close the form here — let the RegisterForm show its success message.
    // The form will call onClose when the user dismisses the success view.
  };

  const handleLoginSuccess = () => {
    setShowLoginForm(false);
  };

  // Show user info and logout button when authenticated
  if (authState.isAuthenticated) {
    return (
      <div className="auth-buttons authenticated">
        <div className="user-info">
          {authState.user && (
            <span className="username">
              Welcome, <Link to="/profile" className="username-link">{authState.user.username}</Link>!
            </span>
          )}
        </div>
        <button 
          className="auth-btn logout-btn"
          onClick={handleLogoutClick}
          disabled={authState.isLoading}
        >
          {authState.isLoading ? 'Logging out...' : 'Logout'}
        </button>
      </div>
    );
  }

  // Show login/register buttons when not authenticated
  return (
    <>
      <div className="auth-buttons">
        <button 
          className="auth-btn login-btn"
          onClick={handleLoginClick}
        >
          Login
        </button>
        <button 
          className="auth-btn register-btn"
          onClick={handleRegisterClick}
        >
          Register
        </button>
      </div>

      {showLoginForm && (
        <LoginForm
          onClose={handleCloseLoginForm}
          onSuccess={handleLoginSuccess}
        />
      )}

      {showRegisterForm && (
        <RegisterForm
          onClose={handleCloseRegisterForm}
          onSuccess={handleRegisterSuccess}
        />
      )}
    </>
  );
}