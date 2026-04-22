/**
 * Application configuration module
 * Provides typed access to environment variables and configuration values
 */

export interface AppConfig {
  app: {
    title: string;
    version: string;
  };
  api: {
    baseUrl: string;
    timeout: number;
  };
  auth: {
    enabled: boolean;
    provider: string;
  };
  features: {
    debugMode: boolean;
    devTools: boolean;
    analytics: boolean;
  };
  dataverse: {
    instanceUrl: string;
  };
  env: 'development' | 'production' | 'test';
}

/**
 * Get environment variable with fallback
 */
function getEnvVar(key: string, fallback?: string): string {
  const value = import.meta.env[key];
  if (value === undefined) {
    if (fallback !== undefined) {
      return fallback;
    }
    console.warn(`Environment variable ${key} is not defined`);
    return '';
  }
  return value;
}

/**
 * Get boolean environment variable
 */
function getBooleanEnvVar(key: string, fallback = false): boolean {
  const value = getEnvVar(key);
  if (!value) return fallback;
  return value.toLowerCase() === 'true';
}

/**
 * Get number environment variable
 */
function getNumberEnvVar(key: string, fallback: number): number {
  const value = getEnvVar(key);
  if (!value) return fallback;
  const parsed = Number.parseInt(value, 10);
  return Number.isNaN(parsed) ? fallback : parsed;
}

/**
 * Application configuration object
 */
export const config: AppConfig = {
  app: {
    title: getEnvVar('VITE_APP_TITLE', 'DataverseSync'),
    version: getEnvVar('VITE_APP_VERSION', '1.0.0'),
  },
  api: {
    baseUrl: getEnvVar('VITE_API_BASE_URL', 'https://localhost:7116/api'),
    timeout: getNumberEnvVar('VITE_API_TIMEOUT', 30000),
  },
  auth: {
    enabled: getBooleanEnvVar('VITE_AUTH_ENABLED', true),
    provider: getEnvVar('VITE_AUTH_PROVIDER', 'azure-ad'),
  },
  features: {
    debugMode: getBooleanEnvVar('VITE_ENABLE_DEBUG_MODE', false),
    devTools: getBooleanEnvVar('VITE_DEV_TOOLS_ENABLED', false),
    analytics: getBooleanEnvVar('VITE_ENABLE_ANALYTICS', false),
  },
  dataverse: {
    instanceUrl: getEnvVar('VITE_DATAVERSE_INSTANCE_URL', ''),
  },
  env: (import.meta.env.MODE as AppConfig['env']) || 'development',
};

/**
 * Validate required configuration
 */
export function validateConfig(): void {
  const requiredVars = [
    'VITE_API_BASE_URL',
  ];

  const missing = requiredVars.filter(key => !import.meta.env[key]);
  
  if (missing.length > 0) {
    console.error('Missing required environment variables:', missing);
    throw new Error(`Missing required environment variables: ${missing.join(', ')}`);
  }
}

// Log configuration in development
if (config.features.debugMode) {
  console.log('App configuration:', {
    ...config,
    // Don't log sensitive values in production
    env: config.env,
  });
}

export default config;