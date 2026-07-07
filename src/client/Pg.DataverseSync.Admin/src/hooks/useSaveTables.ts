import { useState, useCallback } from 'react'
import { Pg_synctablesService } from '../generated/services/Pg_synctablesService'

interface UseSaveTablesResult {
  save: (pendingChanges: Map<string, boolean>) => Promise<void>
  saving: boolean
  error: string | null
}

export function useSaveTables(): UseSaveTablesResult {

  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const save = useCallback(async (pendingChanges: Map<string, boolean>) => {

    setSaving(true); 
    setError(null); 

    try {

      const toSync = [...pendingChanges.entries()].filter(([, selected]) => selected).map(([schemaName]) => schemaName);
      const toUnsync = [...pendingChanges.entries()].filter(([, selected]) => !selected).map(([schemaName]) => schemaName); 

      const createPromises = toSync.map((schemaName) =>
        Pg_synctablesService.create({ pg_name: schemaName, statecode: 0 })
      ); 

      if (toUnsync.length > 0) {

        const existing = await Pg_synctablesService.getAll(); 
        const existingMap = new Map(
          (existing.data ?? []).map((t) => [t.pg_name, t.pg_synctableid])
        );

        const deletePromises = toUnsync
          .map((schemaName) => existingMap.get(schemaName))
          .filter((id): id is string => id !== undefined)
          .map((id) => Pg_synctablesService.delete(id)); 

        await Promise.all([...createPromises, ...deletePromises]);   
      } 
      else {
        await Promise.all(createPromises);
      }
    } 
    catch (err) {
      setError('Failed to save changes: ' + (err as Error).message);
    } 
    finally {
      setSaving(false);
    }
  }, []); 

  return { save, saving, error }; 
}