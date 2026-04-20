/**
 * Application layer exports
 * Centralizes access to hooks, context, and services
 */

// Context
export { AuthProvider, useAuthContext } from './context/AuthContext';

// Hooks
export { useAuth } from './hooks/useAuth';
export { useProfile } from './hooks/useProfile';

// Services
export { AuthService } from './services/authService';