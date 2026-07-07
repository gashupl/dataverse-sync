import { useState, useEffect, useCallback } from 'react';
import { Pg_gettablesService } from '../generated/services/Pg_gettablesService';
import { Pg_synctablesService } from '../generated/services/Pg_synctablesService';
import { TableService } from '../domain/TableService';
import type { Table } from '../domain/model/Table';

interface UseTablesResult {
  tables: Table[];
  loading: boolean;
  error: string | null;
  refresh: () => Promise<void>;
}

export function useTables(): UseTablesResult {
  const [tables, setTables] = useState<Table[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const [allTablesResult, syncTablesResult] = await Promise.all([
        Pg_gettablesService.pg_gettables(), 
        Pg_synctablesService.getAll()
      ]);

      const loadedTables = TableService.createList(allTablesResult, syncTablesResult);

      setTables(loadedTables);
    }
    catch (err) {
      setError('Failed to load tables: ' + (err as Error).message);
    }
    finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  return { tables, loading, error, refresh: load };
}
