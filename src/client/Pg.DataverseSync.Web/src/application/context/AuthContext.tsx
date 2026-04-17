/**
 * Authentication context for sharing auth state across components
 */
import { createContext, useContext, useState, useCallback, useEffect } from 'react';
import type { ReactNode } from 'react';
import { AuthService } from '../services/authService';
import type { RegisterRequest, LoginRequest, AuthResponse, AuthState, User } from '../../domain/entities/auth';

const authService = new AuthService();

export interface AuthContextValue {
  authState: AuthState;
  register: (request: RegisterRequest) => Promise<AuthResponse>;
  login: (request: LoginRequest) => Promise<AuthResponse>;
  logout: () => Promise<void>;
  setUser: (user: User | null) => void;
  refreshUserInfo: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [authState, setAuthState] = useState<AuthState>({
    user: null,
    token: authService.getStoredToken(),
    isAuthenticated: authService.isAuthenticated(),
    isLoading: false,
  });

  // Load user info if authenticated
  useEffect(() => {
    const loadUserInfo = async () => {
      if (authState.isAuthenticated && !authState.user && !authState.isLoading) {
        setAuthState(prev => ({ ...prev, isLoading: true }));
        try {
          const user = await authService.getCurrentUser();
          setAuthState(prev => ({
            ...prev,
            user,
            isLoading: false,
          }));
        } catch {
          // If getting user info fails, clear auth state
          authService.clearTokens();
          setAuthState({
            user: null,
            token: null,
            isAuthenticated: false,
            isLoading: false,
          });
        }
      }
    };

    loadUserInfo();
  }, [authState.isAuthenticated, authState.user, authState.isLoading]);

  const register = useCallback(async (request: RegisterRequest): Promise<AuthResponse> => {
    setAuthState(prev => ({ ...prev, isLoading: true }));

    try {
      const response = await authService.register(request);

      if (response.success && response.token) {
        authService.storeTokens(response.token, response.refreshToken);

        setAuthState(prev => ({
          ...prev,
          token: response.token || null,
          user: response.user || null,
          isAuthenticated: true,
          isLoading: false,
        }));
      } else {
        setAuthState(prev => ({ ...prev, isLoading: false }));
      }

      return response;
    } catch (error) {
      setAuthState(prev => ({ ...prev, isLoading: false }));
      throw error;
    }
  }, []);

  const login = useCallback(async (request: LoginRequest): Promise<AuthResponse> => {
    setAuthState(prev => ({ ...prev, isLoading: true }));

    try {
      const response = await authService.login(request);

      if (response.success && response.token) {
        authService.storeTokens(response.token, response.refreshToken);

        setAuthState(prev => ({
          ...prev,
          token: response.token || null,
          user: response.user || null,
          isAuthenticated: true,
          isLoading: false,
        }));

        if (!response.user) {
          try {
            const user = await authService.getCurrentUser();
            setAuthState(prev => ({ ...prev, user }));
          } catch {
            console.error('Failed to load user info after login');
          }
        }
      } else {
        setAuthState(prev => ({ ...prev, isLoading: false }));
      }

      return response;
    } catch (error) {
      setAuthState(prev => ({ ...prev, isLoading: false }));
      throw error;
    }
  }, []);

  const handleLogout = useCallback(async (): Promise<void> => {
    setAuthState(prev => ({ ...prev, isLoading: true }));

    try {
      await authService.logout();
    } catch {
      console.error('Logout error');
    } finally {
      setAuthState({
        user: null,
        token: null,
        isAuthenticated: false,
        isLoading: false,
      });
    }
  }, []);

  const refreshUserInfo = useCallback(async (): Promise<void> => {
    if (!authState.isAuthenticated) return;

    try {
      const user = await authService.getCurrentUser();
      setAuthState(prev => ({ ...prev, user }));
    } catch (error) {
      console.error('Failed to refresh user info:', error);
      throw error;
    }
  }, [authState.isAuthenticated]);

  const setUser = useCallback((user: User | null) => {
    setAuthState(prev => ({ ...prev, user }));
  }, []);

  return (
    <AuthContext.Provider value={{
      authState,
      register,
      login,
      logout: handleLogout,
      setUser,
      refreshUserInfo,
    }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuthContext(): AuthContextValue {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuthContext must be used within an AuthProvider');
  }
  return context;
}
