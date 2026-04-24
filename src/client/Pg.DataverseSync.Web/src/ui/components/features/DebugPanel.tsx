import { config } from '../../../shared/config';

/**
 * Debug panel component
 * Displays API configuration and test utilities (only in debug mode)
 */
export function DebugPanel() {
  if (!config.features.debugMode) {
    return null;
  }

  const handleApiTest = async () => {
    try {
      const mockResponse = {};
      console.log('API Response:', mockResponse);
    } catch (error) {
      console.error('API Error:', error);
    }
  };

  return (
    <div style={{ marginTop: '1rem' }}>
      <button
        onClick={handleApiTest}
        style={{
          padding: '0.5rem 1rem',
          backgroundColor: '#646cff',
          color: 'white',
          border: 'none',
          borderRadius: '4px',
          cursor: 'pointer'
        }}
      >
        Test API Connection
      </button>
      <p style={{ fontSize: '0.8rem', color: '#666', marginTop: '0.5rem' }}>
        API Base URL: {config.api.baseUrl}
      </p>
    </div>
  );
}