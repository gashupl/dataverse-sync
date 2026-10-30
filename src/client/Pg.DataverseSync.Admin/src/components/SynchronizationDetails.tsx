import { useState, useEffect } from 'react';
import './SynchronizationDetails.css';
import type { Pg_synchronizationdetailses } from '../generated/models/Pg_synchronizationdetailsesModel';
import { Pg_synchronizationdetailsespg_targetrepository } from '../generated/models/Pg_synchronizationdetailsesModel';
import { Pg_synchronizationdetailsesService } from '../generated/services/Pg_synchronizationdetailsesService';

interface SynchronizationDetailsProps {
  readonly syncTableId: string;
  readonly tableName: string;
  readonly onClose: () => void;
}

export function SynchronizationDetails({ syncTableId, tableName, onClose }: SynchronizationDetailsProps) {
  const [records, setRecords] = useState<Pg_synchronizationdetailses[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let ignore = false;

    async function load() {
      setLoading(true);
      setError(null);

      try {
        const result = await Pg_synchronizationdetailsesService.getAll({
          filter: `_pg_synctableid_value eq ${syncTableId} and statecode eq 0`
        });

        if (!ignore) {
          setRecords(result.data ?? []);
        }
      }
      catch (err) {
        if (!ignore) {
          setError('Failed to load synchronization details: ' + (err as Error).message);
        }
      }
      finally {
        if (!ignore) {
          setLoading(false);
        }
      }
    }

    void load();

    return () => {
      ignore = true;
    };
  }, [syncTableId]);

  return (
    <div className="sync-details-overlay">
      <div className="sync-details-modal">
        <h3>Synchronization details for {tableName} table</h3>

        {loading && <p>Loading...</p>}

        {error && <p>{error}</p>}

        {!loading && !error && records.length === 0 && <p>No active synchronization details.</p>}

        {!loading && !error && records.length > 0 && (
          <table className="sync-details-table">
            <thead>
              <tr>
                <th>Target repository</th>
                <th>Checksum</th>
                <th>Last synchronization date</th>
              </tr>
            </thead>
            <tbody>
              {records.map((record) => (
                <tr key={record.pg_synchronizationdetailsid}>
                  <td>{record.pg_targetrepositoryname ?? Pg_synchronizationdetailsespg_targetrepository[record.pg_targetrepository]}</td>
                  <td>{record.pg_checksum}</td>
                  <td>{record.pg_lastsynchronizationdate ? new Date(record.pg_lastsynchronizationdate).toLocaleString() : ''}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}

        <div className="sync-details-actions">
          <button type="button" onClick={onClose}>
            Close
          </button>
        </div>
      </div>
    </div>
  );
}
