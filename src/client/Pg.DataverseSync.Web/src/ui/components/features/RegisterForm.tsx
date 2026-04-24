/**
 * Registration form component
 */
import React, { useState, useEffect } from 'react';
import { useAuth } from '../../../application';
import type { RegisterRequest } from '../../../domain/entities';

export interface RegisterFormProps {
  onClose: () => void;
  onSuccess?: () => void;
}

export function RegisterForm({ onClose, onSuccess }: RegisterFormProps) {
  const { authState, register } = useAuth();
  const [formData, setFormData] = useState<RegisterRequest>({
    username: '',
    email: '',
    password: '',
  });
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isSuccess, setIsSuccess] = useState(false);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError(null);

    // Validation
    if (!formData.username || !formData.email || !formData.password) {
      setError('All fields are required');
      return;
    }

    if (formData.password !== confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    if (formData.password.length < 6) {
      setError('Password must be at least 6 characters long');
      return;
    }

    // Email validation
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailRegex.test(formData.email)) {
      setError('Please enter a valid email address');
      return;
    }

    try {
      const response = await register(formData);
      
      if (response.success) {
        setIsSuccess(true);
        onSuccess?.();
      } else {
        setError(response.message || 'Registration failed');
      }
    } catch (error: any) {
      setError(error.message || 'Registration failed. Please try again.');
    }
  };

  const handleSuccessClose = () => {
    setIsSuccess(false);
    onClose();
  };

  const handleInputChange = (field: keyof RegisterRequest) => (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    setFormData(prev => ({
      ...prev,
      [field]: e.target.value,
    }));
  };

  // Handle Escape key for accessibility
  useEffect(() => {
    const handleEscapeKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape') {
        onClose();
      }
    };

    document.addEventListener('keydown', handleEscapeKey);
    return () => document.removeEventListener('keydown', handleEscapeKey);
  }, [onClose]);

  // Success view
  if (isSuccess) {
    return (
      <div 
        className="register-form-overlay" 
        onClick={handleSuccessClose}
        role="presentation"
      >
        <div 
          className="register-form-modal" 
          onClick={e => e.stopPropagation()}
          role="dialog"
          aria-modal="true"
          aria-labelledby="success-title"
        >
          <div className="register-form-header">
            <h2 id="success-title">Registration Successful! 🎉</h2>
            <button 
              className="close-button" 
              onClick={handleSuccessClose}
              aria-label="Close"
            >
              ×
            </button>
          </div>

          <div className="register-form success-content">
            <div className="success-message">
              <div className="success-icon">✅</div>
              <h3>Welcome, {formData.username}!</h3>
              <p>Your account has been created successfully.</p>
              <p>You are now logged in and ready to use DataverseSync.</p>
            </div>

            <div className="form-actions">
              <button 
                onClick={handleSuccessClose}
                className="submit-button"
              >
                Continue
              </button>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div 
      className="register-form-overlay" 
      onClick={onClose}
      role="presentation"
    >
      <div 
        className="register-form-modal" 
        onClick={e => e.stopPropagation()}
        role="dialog"
        aria-modal="true"
        aria-labelledby="register-title"
      >
        <div className="register-form-header">
          <h2 id="register-title">Register</h2>
          <button 
            className="close-button" 
            onClick={onClose}
            aria-label="Close"
          >
            ×
          </button>
        </div>

        <form onSubmit={handleSubmit} className="register-form">
          {error && (
            <div className="error-message">
              {error}
            </div>
          )}

          <div className="form-group">
            <label htmlFor="username">Username</label>
            <input
              type="text"
              id="username"
              value={formData.username}
              onChange={handleInputChange('username')}
              required
              disabled={authState.isLoading}
              placeholder="Enter your username"
            />
          </div>

          <div className="form-group">
            <label htmlFor="email">Email</label>
            <input
              type="email"
              id="email"
              value={formData.email}
              onChange={handleInputChange('email')}
              required
              disabled={authState.isLoading}
              placeholder="Enter your email"
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Password</label>
            <input
              type="password"
              id="password"
              value={formData.password}
              onChange={handleInputChange('password')}
              required
              disabled={authState.isLoading}
              placeholder="Enter your password"
              minLength={6}
            />
          </div>

          <div className="form-group">
            <label htmlFor="confirmPassword">Confirm Password</label>
            <input
              type="password"
              id="confirmPassword"
              value={confirmPassword}
              onChange={e => setConfirmPassword(e.target.value)}
              required
              disabled={authState.isLoading}
              placeholder="Confirm your password"
              minLength={6}
            />
          </div>

          <div className="form-actions">
            <button 
              type="button" 
              onClick={onClose}
              className="cancel-button"
              disabled={authState.isLoading}
            >
              Cancel
            </button>
            <button 
              type="submit" 
              className="submit-button"
              disabled={authState.isLoading}
            >
              {authState.isLoading ? 'Creating Account...' : 'Register'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}