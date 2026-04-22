/**
 * API utility functions using configuration
 */
import { config } from '../config';

export interface ApiResponse<T = any> {
  data: T;
  status: number;
  message?: string;
}

export interface ApiError {
  message: string;
  status: number;
  details?: any;
}

/**
 * Base API client with configuration
 */
class ApiClient {
  private readonly baseUrl: string;
  private readonly timeout: number;
  private refreshPromise: Promise<string | null> | null = null;

  constructor() {
    this.baseUrl = config.api.baseUrl;
    this.timeout = config.api.timeout;
  }

  /**
   * Make HTTP request with standard configuration
   */
  private async request<T>(
    endpoint: string,
    options: RequestInit = {},
    skipAuthRefresh = false
  ): Promise<ApiResponse<T>> {
    const url = `${this.baseUrl}${endpoint}`;
    
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), this.timeout);

    // Get auth token from localStorage
    const token = localStorage.getItem('authToken');
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
    };

    // Add authorization header if token is available
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    // Merge any additional headers from options
    if (options.headers) {
      const optHeaders = new Headers(options.headers);
      optHeaders.forEach((value, key) => {
        headers[key] = value;
      });
    }

    try {
      const response = await fetch(url, {
        ...options,
        signal: controller.signal,
        headers,
      });

      clearTimeout(timeoutId);

      if (!response.ok) {
        // Attempt token refresh on 401, unless this is already a refresh-related request
        if (response.status === 401 && !skipAuthRefresh && localStorage.getItem('refreshToken')) {
          const newToken = await this.tryRefreshToken();
          if (newToken) {
            return this.request<T>(endpoint, options, true);
          }
        }

        let message: string;
        try {
          const errorBody = await response.json();
          message = errorBody.message || errorBody.title || errorBody.detail || `Request failed with status ${response.status}`;
        } catch {
          message = response.status === 401
            ? 'Invalid username or password'
            : `Request failed with status ${response.status}`;
        }

        throw {
          message,
          status: response.status,
        } as ApiError;
      }

      const data = await response.json();
      
      return {
        data,
        status: response.status,
      };
    } catch (error) {
      clearTimeout(timeoutId);
      
      if (error instanceof Error) {
        throw {
          message: error.message,
          status: 0,
          details: error,
        } as ApiError;
      }
      
      throw error;
    }
  }

  /**
   * Attempt to refresh the auth token. Deduplicates concurrent refresh calls.
   */
  private async tryRefreshToken(): Promise<string | null> {
    if (this.refreshPromise) {
      return this.refreshPromise;
    }

    this.refreshPromise = (async () => {
      try {
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) return null;

        const response = await this.request<{ success: boolean; token?: string; refreshToken?: string }>(
          '/Auth/refresh',
          { method: 'POST', body: JSON.stringify({ refreshToken }) },
          true
        );

        if (response.data.success && response.data.token) {
          localStorage.setItem('authToken', response.data.token);
          if (response.data.refreshToken) {
            localStorage.setItem('refreshToken', response.data.refreshToken);
          }
          return response.data.token;
        }

        return null;
      } catch {
        // Refresh failed — clear tokens so the user is logged out
        localStorage.removeItem('authToken');
        localStorage.removeItem('refreshToken');
        return null;
      } finally {
        this.refreshPromise = null;
      }
    })();

    return this.refreshPromise;
  }

  /**
   * GET request
   */
  async get<T>(endpoint: string): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, { method: 'GET' });
  }

  /**
   * POST request
   */
  async post<T>(endpoint: string, data?: any): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, {
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  /**
   * PUT request
   */
  async put<T>(endpoint: string, data?: any): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, {
      method: 'PUT',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  /**
   * DELETE request
   */
  async delete<T>(endpoint: string): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, { method: 'DELETE' });
  }
}

// Export singleton instance
export const apiClient = new ApiClient();
