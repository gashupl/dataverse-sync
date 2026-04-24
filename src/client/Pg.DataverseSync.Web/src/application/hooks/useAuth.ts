/**
 * Custom hook for authentication state and operations
 * Delegates to AuthContext for shared state across components
 */
export { useAuthContext as useAuth } from '../context/AuthContext';
export type { AuthContextValue as UseAuthReturn } from '../context/AuthContext';