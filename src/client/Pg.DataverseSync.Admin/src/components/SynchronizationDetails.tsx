import './SynchronizationDetails.css';

interface SynchronizationDetailsProps {
  readonly syncTableId: string;
  readonly onClose: () => void;
}

export function SynchronizationDetails({ syncTableId, onClose }: SynchronizationDetailsProps) {
  return (
    <div className="sync-details-overlay">
      <div className="sync-details-modal">
        <h3>Synchronization Details</h3>

        <p>Sync table ID: <strong>{syncTableId}</strong></p>

        <div className="sync-details-actions">
          <button type="button" onClick={onClose}>
            Close
          </button>
        </div>
      </div>
    </div>
  );
}
