import { useState, useEffect } from 'react';
import { ServiceendpointsService } from '../generated/services/ServiceendpointsService';
import { EnvironmentvariabledefinitionsService } from '../generated/services/EnvironmentvariabledefinitionsService';
import { EnvironmentvariablevaluesService } from '../generated/services/EnvironmentvariablevaluesService';
import type { EnvironmentvariablevaluesBase } from '../generated/models/EnvironmentvariablevaluesModel';
import type { ServiceendpointsBase } from '../generated/models/ServiceendpointsModel';

interface ServiceEndpointModalProps {
  readonly isOpen: boolean;
  readonly endpointId?: string;
  readonly onClose: () => void;
  readonly onSaved: () => void;
}

interface FormData {
  name: string;
  namespaceAddress: string;
  queueName: string;
  sasKeyName: string;
  sasKey: string;
}

const initialFormData: FormData = {
  name: '',
  namespaceAddress: '',
  queueName: '',
  sasKeyName: '',
  sasKey: '',
};

export function ServiceEndpointModal({
  isOpen,
  endpointId,
  onClose,
  onSaved,
}: ServiceEndpointModalProps) {
  const [formData, setFormData] = useState<FormData>(initialFormData);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let ignore = false;

    if (!isOpen || !endpointId) {
      return;
    }

    const loadEndpoint = async () => {
      setLoading(true);
      setError(null);

      try {
        const result = await ServiceendpointsService.get(endpointId);

        if (!ignore && result.data) {
          setFormData({
            name: result.data.name ?? '',
            namespaceAddress: result.data.namespaceaddress ?? '',
            queueName: result.data.path ?? '',
            sasKeyName: result.data.saskeyname ?? '',
            sasKey: '',
          });
        }
      }
      catch (err) {
        if (!ignore) {
          setError('Failed to load endpoint details: ' + (err as Error).message);
        }
      }
      finally {
        if (!ignore) {
          setLoading(false);
        }
      }
    };

    void loadEndpoint();

    return () => {
      ignore = true;
    };
  }, [isOpen, endpointId]);

  const handleChange = (field: keyof FormData, value: string) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();

    setSaving(true);
    setError(null);

    try {
      if (endpointId) {
        const updateData: Partial<Omit<ServiceendpointsBase, 'serviceendpointid'>> = {
          name: formData.name,
          namespaceaddress: formData.namespaceAddress,
          path: formData.queueName,
          saskeyname: formData.sasKeyName,
        };

        if (formData.sasKey.trim() !== '') {
          updateData.saskey = formData.sasKey;
        }

        const updateResult = await ServiceendpointsService.update(endpointId, updateData);

        if (!updateResult.success) {
          console.log(updateResult.error?.message);
          setError('Failed to save service endpoint');
          return;
        }
      }
      else {
        const newEndpoint: Omit<ServiceendpointsBase, 'serviceendpointid' | 'iscustomizable'> = {
          name: formData.name,
          namespaceaddress: formData.namespaceAddress,
          path: formData.queueName,
          saskeyname: formData.sasKeyName,
          saskey: formData.sasKey,
          contract: 6, // Queue (Persistent)
          messageformat: 2, // JSON
          authtype: 2, // SASKey
          userclaim: 2, // UserID
          connectionmode: 1, // Normal
          namespaceformat: 2, // Namespace Address
          solutionnamespace: '',
        };

        const createResult = await ServiceendpointsService.create(
          newEndpoint as unknown as Omit<ServiceendpointsBase, 'serviceendpointid'>
        );

        if (!createResult.success) {
          console.log(createResult.error?.message);
          setError('Failed to save service endpoint');
          return;
        }

        const createdId = createResult.data?.serviceendpointid;

        if (createdId) {
          const definitionResult = await EnvironmentvariabledefinitionsService.getAll({
            select: ['environmentvariabledefinitionid'],
            filter: "schemaname eq 'pg_dataversesyncendpointid'",
          });

          const definition = definitionResult.data?.[0];

          if (definition) {
            const valueResult = await EnvironmentvariablevaluesService.getAll({
              select: ['environmentvariablevalueid'],
              filter: `_environmentvariabledefinitionid_value eq ${definition.environmentvariabledefinitionid}`,
            });

            const existingValue = valueResult.data?.[0];

            if (existingValue) {
              await EnvironmentvariablevaluesService.update(existingValue.environmentvariablevalueid, {
                value: createdId,
              });
            }
            else {
              await EnvironmentvariablevaluesService.create({
                value: createdId,
                'EnvironmentVariableDefinitionId@odata.bind': `/environmentvariabledefinitions(${definition.environmentvariabledefinitionid})`,
              } as unknown as Omit<EnvironmentvariablevaluesBase, 'environmentvariablevalueid'>);
            }
          }
        }
      }

      onSaved();
      onClose();
    }
    catch (err) {
      setError('Failed to save service endpoint: ' + (err as Error).message);
    }
    finally {
      setSaving(false);
    }
  };

  if (!isOpen) {
    return null;
  }

  return (
    <div className="service-endpoint-modal-overlay">
      <div className="service-endpoint-modal">
        <h3>{endpointId ? 'Edit Service Endpoint' : 'New Service Endpoint'}</h3>

        {loading && <div className="service-endpoint-loading">Loading endpoint details...</div>}
        {error && <div className="service-endpoint-error">Error: {error}</div>}

        {!loading && (
          <form className="service-endpoint-form" onSubmit={handleSave}>
            <div className="service-endpoint-form-group">
              <label htmlFor="endpoint-name">Name</label>
              <input
                id="endpoint-name"
                type="text"
                required
                value={formData.name}
                onChange={(e) => handleChange('name', e.target.value)}
              />
            </div>

            <div className="service-endpoint-form-group">
              <label htmlFor="endpoint-namespace-address">Namespace Address</label>
              <input
                id="endpoint-namespace-address"
                type="text"
                required
                value={formData.namespaceAddress}
                onChange={(e) => handleChange('namespaceAddress', e.target.value)}
              />
            </div>

            <div className="service-endpoint-form-group">
              <label htmlFor="endpoint-queue-name">Queue Name</label>
              <input
                id="endpoint-queue-name"
                type="text"
                required
                value={formData.queueName}
                onChange={(e) => handleChange('queueName', e.target.value)}
              />
            </div>

            <div className="service-endpoint-form-group">
              <label htmlFor="endpoint-saskey-name">SAS Key Name</label>
              <input
                id="endpoint-saskey-name"
                type="text"
                required
                value={formData.sasKeyName}
                onChange={(e) => handleChange('sasKeyName', e.target.value)}
              />
            </div>

            <div className="service-endpoint-form-group">
              <label htmlFor="endpoint-saskey">SAS Key</label>
              <input
                id="endpoint-saskey"
                type="password"
                required={!endpointId}
                value={formData.sasKey}
                onChange={(e) => handleChange('sasKey', e.target.value)}
              />
            </div>

            <div className="service-endpoint-modal-actions">
              <button type="button" onClick={onClose} disabled={saving}>
                Cancel
              </button>
              <button type="submit" disabled={saving}>
                {saving ? 'Saving...' : 'Save'}
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  );
}
