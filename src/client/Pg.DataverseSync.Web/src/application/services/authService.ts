/**
 * Authentication service for handling user authentication operations
 */
import { apiClient } from '../../shared/utils/api';
import type { RegisterRequest, LoginRequest, LogoutRequest, AuthResponse, User } from '../../domain/entities/auth';

export class AuthService {
  /**
   * Register a new user
   */
  async register(request: RegisterRequest): Promise<AuthResponse> {
    try {
      const response = await apiClient.post<AuthResponse>('/Auth/register', request);
      return response.data;
    } catch (error) {
      console.error('Registration error:', error);
      throw error;
    }
  }

  /**
   * Login a user
   */
  async login(request: LoginRequest): Promise<AuthResponse> {
    try {
      const response = await apiClient.post<AuthResponse>('/Auth/login', request);
      return response.data;
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  }

  /**
   * Logout current user
   */
  async logout(): Promise<void> {
    try {
      const refreshToken = this.getStoredRefreshToken();
      if (refreshToken) {
        const logoutRequest: LogoutRequest = { refreshToken };
        await apiClient.post('/Auth/logout', logoutRequest);
      }
      // Clear local storage
      this.clearTokens();
    } catch (error) {
      console.error('Logout error:', error);
      // Even if logout fails on server, clear local tokens
      this.clearTokens();
      throw error;
    }
  }

  /**
   * Get current user info
   */
  async getCurrentUser(): Promise<User> {
    try {
      const response = await apiClient.get<User>('/Auth/me');
      return response.data;
    } catch (error) {
      console.error('Get current user error:', error);
      throw error;
    }
  }

  /**
   * Refresh authentication token
   */
  async refreshToken(): Promise<AuthResponse> {
    try {
      const refreshToken = this.getStoredRefreshToken();
      if (!refreshToken) {
        throw new Error('No refresh token available');
      }

      const response = await apiClient.post<AuthResponse>('/Auth/refresh', {
        refreshToken
      });
      return response.data;
    } catch (error) {
      console.error('Token refresh error:', error);
      throw error;
    }
  }

  /**
   * Store authentication tokens
   */
  storeTokens(token: string, refreshToken?: string): void {
    localStorage.setItem('authToken', token);
    if (refreshToken) {
      localStorage.setItem('refreshToken', refreshToken);
    }
  }

  /**
   * Get stored token
   */
  getStoredToken(): string | null {
    return localStorage.getItem('authToken');
  }

  /**
   * Get stored refresh token
   */
  getStoredRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  /**
   * Check if user is authenticated
   */
  isAuthenticated(): boolean {
    return !!this.getStoredToken();
  }

  /**
   * Clear stored tokens
   */
  clearTokens(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('refreshToken');
  }
}