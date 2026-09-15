import { useState } from 'react';
import { useGetEnvironmentVariableValue } from '../hooks/useGetEnvironmentVariableValue';
import { ServiceEndpointModal } from './ServiceEndpointModal';
import './ServiceEndpointDetails.css';

export function ServiceEndpointDetails() {
  const { value, loading, refresh } = useGetEnvironmentVariableValue('pg_dataversesyncendpointid');
  const [isModalOpen, setIsModalOpen] = useState(false);

  const label = loading
    ? 'Loading service endpoint details...'
    : (value ?? 'No Service Endpoint configured.');

  const handleOpenModal = () => {
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
  };

  const handleSaved = async () => {
    await refresh();
  };

  return (
    <div className="service-endpoint-details-container">
      <div className="service-endpoint-info">
        <p className="service-endpoint-label">
          <strong>Endpoint:</strong> {label}
        </p>
        <button
          type="button"
          className="service-endpoint-button"
          onClick={handleOpenModal}
          disabled={loading}
        >
          Endpoint Details
        </button>
      </div>

      {isModalOpen && (
        <ServiceEndpointModal
          isOpen={isModalOpen}
          endpointId={value}
          onClose={handleCloseModal}
          onSaved={handleSaved}
        />
      )}
    </div>
  );
}
