/// <reference types="vite/client" />

interface ImportMetaEnv {
  // Application Configuration
  readonly VITE_APP_TITLE: string;
  readonly VITE_APP_VERSION: string;

  // API Configuration
  readonly VITE_API_BASE_URL: string;
  readonly VITE_API_TIMEOUT: string;

  // Authentication Configuration
  readonly VITE_AUTH_ENABLED: string;
  readonly VITE_AUTH_PROVIDER: string;

  // Feature Flags
  readonly VITE_ENABLE_DEBUG_MODE: string;
  readonly VITE_ENABLE_ANALYTICS: string;

  // Development Tools
  readonly VITE_DEV_TOOLS_ENABLED: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}