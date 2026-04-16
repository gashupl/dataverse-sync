import { useState } from 'react';
import { config } from '../../../shared/config';
import { RegisterForm } from '../features/RegisterForm';

/**
 * Authentication buttons component
 * Displays login/register buttons if authentication is enabled
 */
export function AuthButtons() {
  const [showRegisterForm, setShowRegisterForm] = useState(false);

  if (!config.auth.enabled) {
    return null;
  }

  const handleRegisterClick = () => {
    setShowRegisterForm(true);
  };

  const handleCloseRegisterForm = () => {
    setShowRegisterForm(false);
  };

  const handleRegisterSuccess = () => {
    // Handle successful registration (e.g., show success message, redirect)
    console.log('Registration successful!');
  };

  return (
    <>
      <div className="auth-buttons">
        <button className="auth-btn login-btn">Login</button>
        <button 
          className="auth-btn register-btn"
          onClick={handleRegisterClick}
        >
          Register
        </button>
      </div>

      {showRegisterForm && (
        <RegisterForm
          onClose={handleCloseRegisterForm}
          onSuccess={handleRegisterSuccess}
        />
      )}
    </>
  );
}