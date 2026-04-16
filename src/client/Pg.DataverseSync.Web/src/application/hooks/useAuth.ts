/**
 * Custom hook for authentication state and operations
 */
import { useState, useCallback } from 'react';
import { AuthService } from '../services/authService';
import type { RegisterRequest, AuthResponse, AuthState, User } from '../../domain/entities/auth';

const authService = new AuthService();

export interface UseAuthReturn {
  authState: AuthState;
  register: (request: RegisterRequest) => Promise<AuthResponse>;
  login: (username: string, password: string) => Promise<AuthResponse>;
  logout: () => Promise<void>;
  setUser: (user: User | null) => void;
}

export function useAuth(): UseAuthReturn {
  const [authState, setAuthState] = useState<AuthState>({
    user: null,
    token: authService.getStoredToken(),
    isAuthenticated: !!authService.getStoredToken(),
    isLoading: false,
  });

  const register = useCallback(async (request: RegisterRequest): Promise<AuthResponse> => {
    setAuthState(prev => ({ ...prev, isLoading: true }));
    
    try {
      const response = await authService.register(request);
      
      if (response.success && response.token) {
        // Store tokens
        authService.storeTokens(response.token, response.refreshToken);
        
        // Update auth state
        setAuthState(prev => ({
          ...prev,
          token: response.token || null,
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

  const login = useCallback(async (username: string, password: string): Promise<AuthResponse> => {
    setAuthState(prev => ({ ...prev, isLoading: true }));
    
    try {
      const response = await authService.login(username, password);
      
      if (response.success && response.token) {
        // Store tokens
        authService.storeTokens(response.token, response.refreshToken);
        
        // Update auth state
        setAuthState(prev => ({
          ...prev,
          token: response.token || null,
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

  const logout = useCallback(async (): Promise<void> => {
    setAuthState(prev => ({ ...prev, isLoading: true }));
    
    try {
      await authService.logout();
      
      // Clear auth state
      setAuthState({
        user: null,
        token: null,
        isAuthenticated: false,
        isLoading: false,
      });
    } catch (error) {
      setAuthState(prev => ({ ...prev, isLoading: false }));
      throw error;
    }
  }, []);

  const setUser = useCallback((user: User | null) => {
    setAuthState(prev => ({ ...prev, user }));
  }, []);

  return {
    authState,
    register,
    login,
    logout,
    setUser,
  };
}