/**
 * API utility functions using configuration
 */
import { config } from '../config';

export interface ApiResponse<T = any> {
  data: T;
  status: number;
  message?: string;
}

export class ApiError extends Error {
  status: number;
  details?: any;

  constructor(message: string, status: number, details?: any) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.details = details;
    Object.setPrototypeOf(this, ApiError.prototype);
  }
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
   * Build headers with auth token and merge with provided options
   */
  private buildHeaders(options: RequestInit): Record<string, string> {
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
    };

    const token = localStorage.getItem('authToken');
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    if (options.headers) {
      const optHeaders = new Headers(options.headers);
      optHeaders.forEach((value, key) => {
        headers[key] = value;
      });
    }

    return headers;
  }

  /**
   * Extract error message from response or generate default
   */
  private async parseErrorMessage(response: Response): Promise<string> {
    try {
      const errorBody = await response.json();
      return errorBody.message || errorBody.title || errorBody.detail || `Request failed with status ${response.status}`;
    } catch {
      return response.status === 401
        ? 'Invalid username or password'
        : `Request failed with status ${response.status}`;
    }
  }

  /**
   * Handle 401 response with token refresh attempt
   */
  private async handle401Response<T>(
    endpoint: string,
    options: RequestInit,
    skipAuthRefresh: boolean
  ): Promise<ApiResponse<T> | null> {
    if (skipAuthRefresh || !localStorage.getItem('refreshToken')) {
      return null;
    }

    const newToken = await this.tryRefreshToken();
    if (newToken) {
      return this.request<T>(endpoint, options, true);
    }

    return null;
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
    const headers = this.buildHeaders(options);

    try {
      const response = await fetch(url, {
        ...options,
        signal: controller.signal,
        headers,
      });

      clearTimeout(timeoutId);

      if (!response.ok) {
        if (response.status === 401) {
          const refreshedResponse = await this.handle401Response<T>(endpoint, options, skipAuthRefresh);
          if (refreshedResponse) {
            return refreshedResponse;
          }
        }

        const message = await this.parseErrorMessage(response);
        throw new ApiError(message, response.status);
      }

      const data = await response.json();
      return {
        data,
        status: response.status,
      };
    } catch (error) {
      clearTimeout(timeoutId);
      
      if (error instanceof Error) {
        throw new ApiError(error.message, 0, error);
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
