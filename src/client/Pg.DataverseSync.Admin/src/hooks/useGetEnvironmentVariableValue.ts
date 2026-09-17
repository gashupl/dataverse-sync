import { useState, useEffect, useCallback } from 'react';
import { EnvironmentvariabledefinitionsService } from '../generated/services/EnvironmentvariabledefinitionsService';
import { EnvironmentvariablevaluesService } from '../generated/services/EnvironmentvariablevaluesService';

interface UseGetEnvironmentVariableValueResult {
  value: string | undefined;
  loading: boolean;
  error: string | null;
  refresh: () => Promise<void>;
}

export function useGetEnvironmentVariableValue(schemaName: string): UseGetEnvironmentVariableValueResult {
  const [value, setValue] = useState<string | undefined>(undefined);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const definitionResult = await EnvironmentvariabledefinitionsService.getAll({
        select: ['environmentvariabledefinitionid', 'defaultvalue'],
        filter: `schemaname eq '${schemaName}'`,
      });

      const definition = definitionResult.data?.[0];

      if (!definition) {
        setValue(undefined);
        return;
      }

      const valueResult = await EnvironmentvariablevaluesService.getAll({
        select: ['value'],
        filter: `_environmentvariabledefinitionid_value eq ${definition.environmentvariabledefinitionid}`,
      });

      // Override value wins, otherwise fall back to the default value
      setValue(valueResult.data?.[0]?.value ?? definition.defaultvalue);
    }
    catch (err) {
      setError('Failed to load environment variable value: ' + (err as Error).message);
    }
    finally {
      setLoading(false);
    }
  }, [schemaName]);

  useEffect(() => {
    load();
  }, [load]);

  return { value, loading, error, refresh: load };
}