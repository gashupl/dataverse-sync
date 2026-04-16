/**
 * Authentication service for handling user authentication operations
 */
import { apiClient } from '../../shared/utils/api';
import type { RegisterRequest, AuthResponse } from '../../domain/entities/auth';

export class AuthService {
  /**
   * Register a new user
   */
  async register(request: RegisterRequest): Promise<AuthResponse> {
    try {
      const response = await apiClient.post<AuthResponse>('/auth/register', request);
      return response.data;
    } catch (error) {
      console.error('Registration error:', error);
      throw error;
    }
  }

  /**
   * Login a user
   */
  async login(username: string, password: string): Promise<AuthResponse> {
    try {
      const response = await apiClient.post<AuthResponse>('/auth/login', {
        username,
        password
      });
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
      await apiClient.post('/auth/logout');
      // Clear local storage
      localStorage.removeItem('authToken');
      localStorage.removeItem('refreshToken');
    } catch (error) {
      console.error('Logout error:', error);
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
   * Clear stored tokens
   */
  clearTokens(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('refreshToken');
  }
}