import { useState, useCallback } from 'react'
import { Pg_synctablesService } from '../generated/services/Pg_synctablesService'
import type { Pg_synctables } from '../generated/models/Pg_synctablesModel'

interface UseSyncTablesResult {
  syncTables: Pg_synctables[]
  loading: boolean
  error: string | null
  fetch: () => Promise<void>
}

export function useSyncTables(): UseSyncTablesResult {
  const [syncTables, setSyncTables] = useState<Pg_synctables[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const fetch = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const result = await Pg_synctablesService.getAll()
      setSyncTables(result.data ?? [])
    } catch (err) {
      setError('Failed to retrieve sync tables: ' + (err as Error).message)
    } finally {
      setLoading(false)
    }
  }, [])

  return { syncTables, loading, error, fetch }
}
