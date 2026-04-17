/**
 * Login form component
 */
import { useState } from 'react';
import type { FormEvent } from 'react';
import { useAuth } from '../../../application';
import type { LoginRequest } from '../../../domain/entities';

export interface LoginFormProps {
  onClose: () => void;
  onSuccess?: () => void;
}

export function LoginForm({ onClose, onSuccess }: LoginFormProps) {
  const { authState, login } = useAuth();
  const [formData, setFormData] = useState<LoginRequest>({
    username: '',
    password: '',
  });
  const [error, setError] = useState<string | null>(null);
  const [isSuccess, setIsSuccess] = useState(false);

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError(null);

    // Validation
    if (!formData.username || !formData.password) {
      setError('Username and password are required');
      return;
    }

    try {
      const response = await login(formData);
      
      if (response.success) {
        setIsSuccess(true);
        onSuccess?.();
      } else {
        setError(response.message || 'Login failed');
      }
    } catch (error: any) {
      setError(error.message || 'Login failed. Please try again.');
    }
  };

  const handleSuccessClose = () => {
    setIsSuccess(false);
    onClose();
  };

  const handleInputChange = (field: keyof LoginRequest) => (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    setFormData(prev => ({
      ...prev,
      [field]: e.target.value,
    }));
  };

  // Success view
  if (isSuccess) {
    return (
      <div className="login-form-overlay" onClick={handleSuccessClose}>
        <div className="login-form-modal" onClick={e => e.stopPropagation()}>
          <div className="login-form-header">
            <h2>Welcome Back! 👋</h2>
            <button 
              className="close-button" 
              onClick={handleSuccessClose}
              aria-label="Close"
            >
              ×
            </button>
          </div>

          <div className="login-form success-content">
            <div className="success-message">
              <div className="success-icon">✅</div>
              <h3>Login Successful!</h3>
              <p>You have been successfully logged in.</p>
              <button 
                className="success-button"
                onClick={handleSuccessClose}
              >
                Continue
              </button>
            </div>
          </div>
        </div>
      </div>
    );
  }

  // Form view
  return (
    <div className="login-form-overlay" onClick={onClose}>
      <div className="login-form-modal" onClick={e => e.stopPropagation()}>
        <div className="login-form-header">
          <h2>Login</h2>
          <button 
            className="close-button" 
            onClick={onClose}
            aria-label="Close"
          >
            ×
          </button>
        </div>

        <form className="login-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="username">Username</label>
            <input
              type="text"
              id="username"
              value={formData.username}
              onChange={handleInputChange('username')}
              disabled={authState.isLoading}
              autoComplete="username"
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Password</label>
            <input
              type="password"
              id="password"
              value={formData.password}
              onChange={handleInputChange('password')}
              disabled={authState.isLoading}
              autoComplete="current-password"
              required
            />
          </div>

          {error && (
            <div className="error-message" role="alert">
              {error}
            </div>
          )}

          <button 
            type="submit" 
            className="submit-button"
            disabled={authState.isLoading}
          >
            {authState.isLoading ? 'Signing in...' : 'Sign In'}
          </button>

          <div className="form-footer">
            <p>
              Don't have an account?{' '}
              <button 
                type="button" 
                className="link-button"
                onClick={onClose} // This will close login form, allowing user to click register
              >
                Register here
              </button>
            </p>
          </div>
        </form>
      </div>
    </div>
  );
}